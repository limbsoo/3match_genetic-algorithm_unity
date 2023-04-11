using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
    using UnityEditor;
#endif

namespace Mkey
{
    public enum AutoWin { Fast, Slow, Bombs}
    public enum MoveDir {Left, Right, Up, Down }
    public class GameBoard : MonoBehaviour
    {
        #region bomb setting
        [Space(8)]
        [Header("Bomb setting")]
        public BombType bombType = BombType.DynamicClick;
        public bool showBombExplode = true;
        #endregion bomb setting

        #region settings 
        [Space(8)]
        [Header("Game settings")]
        public AutoWin autoWin = AutoWin.Fast;

        public bool showAlmostMessage =true;

        public static bool showMission = true;
      
        public int almostCoins = 100;
        public bool flyCollected = true;
        public FillType fillType;
        public bool showScore;

        [HideInInspector]
        [SerializeField]
        private MoveDir moveDir = MoveDir.Right;
        #endregion settings

        #region references
        [Header("Main references")]
        [Space(8)]
        public Transform GridContainer;
        public Transform addPosition;
        [SerializeField]
        private RectTransform flyTarget;
        public SpriteRenderer backGround;
        public GameConstructor gConstructor;
        public BombCreator bombCreator;
        [SerializeField]
        private WinController winController;
        [SerializeField]
        private ScoreController scoreController;
        [SerializeField]
        private PopUpsController goodPrefab;
        [SerializeField]
        private PopUpsController greatPrefab;
        [SerializeField]
        private PopUpsController excellentPrefab;
        [SerializeField]
        private PopUpsController timeLeftMessagePrefab;
        [SerializeField]
        private PopUpsController movesLeftMessagePrefab;
        [SerializeField]
        public PopUpsController AutoVictoryPrefab;
        [SerializeField]
        private PopUpsController missionPrefab;
        #endregion references

        #region spawn
        [Space(8)]
        [Header("Spawn")]
        public Spawner spawnerPrefab;
        public SpawnerStyle spawnerStyle;
        #endregion spawn

        #region grids
        public static float MaxDragDistance;
        public MatchGrid MainGrid { get; private set; }
        public MatchGrid CurrentGrid { get; private set; }
        public Dictionary<LevelConstructSet, MatchGrid> AdditionalGrids { get; private set; }
        public int AddGridIndex { get { int index = -1; if (AdditionalGrids != null) index = (new List<MatchGrid>(AdditionalGrids.Values)).IndexOf(CurrentGrid); return index; } }
        public int AdditGridsCount { get { return (AdditionalGrids != null) ? AdditionalGrids.Count : 0; } }
        public bool HaveAdditGrids { get { return AdditGridsCount > 0; } }
        public bool HaveNextGrid { get { return (MainGrid == CurrentGrid && HaveAdditGrids) || (HaveAdditGrids && AddGridIndex < AdditGridsCount - 1); } }
        public MatchGrid NextGrid { get { return (HaveNextGrid) ? (new List<MatchGrid>(AdditionalGrids.Values))[AddGridIndex + 1] : null; } }
        #endregion grids

        #region curves
        [SerializeField]
        private AnimationCurve explodeCurve;
        [SerializeField]
        public AnimationCurve arcCurve;
        #endregion curves

        #region states
        public static GameMode GMode = GameMode.Play; // Play or Edit

        public MatchBoardState MbState { get; private set; }
        #endregion states

        #region properties
        public Vector3 FlyTarget
        {
            get { return flyTarget.transform.position; } //return Coordinats.CanvasToWorld(flyTarget.gameObject); }
        }

        public Sprite BackGround
        {
            get { return backGround.sprite; }
            set { if (backGround) backGround.sprite = value; }
        }

        public int AverageScore
        {
            get;
            private set;
        }

        private SoundMaster MSound { get { return SoundMaster.Instance; } }

        public WinController WinContr { get { return winController; } }

        public Dictionary<int, TargetData> Targets { get; private set; }

        public Dictionary<int, TargetData> CurTargets { get; private set; } // summary targets for current grid

        public GuiController MGui => GuiController.Instance; 

        public bool IsMainGridActive { get { return CurrentGrid == MainGrid; } }

        public int MatchScore { get; private set; }
        #endregion properties

        #region sets
        private GameConstructSet GCSet { get { return GameConstructSet.Instance; } }
        private LevelConstructSet LCSet { get { return GCSet.GetLevelConstructSet(GameLevelHolder.CurrentLevel); } }
        private GameObjectsSet GOSet { get { return GCSet.GOSet; } }
        private LevelConstructSet MainLCSet { get { return LCSet; }}
        private List<LevelConstructSet> LcSetAdds { get { return GCSet.GetAddLevelConstructSets(GameLevelHolder.CurrentLevel); }}
        public MissionConstruct FullLevelMission { get; private set; }
        private GameLevelHolder MGLevel => GameLevelHolder.Instance;
        #endregion sets

        #region events
        public static Action <GameBoard> ChangeCurrentBoardEvent;  // use for game with many boards
        public Action<GameBoard> BeforeFillBoardEvent;             // 
        public Action<GameBoard> AfterFillBoardEvent;             // 
        public Action<GameBoard> BeforeCollectBoardEvent;          // 
        public Action<GameBoard> BeforeStepBoardEvent;             // 
		public Action<GameBoard> AfterStepBoardEvent;
        #endregion events

        #region temp
        public MatchGroupsHelper CollectGroups { get; private set; }
        public MatchGroupsHelper EstimateGroups { get;  set; }
        private bool testing = true;
        private float lastiEstimateShowed;
        private float lastPlayTime;
        private bool canPlay = false;
        private int collected = 0; // collected counter
        private float collectDelay = 0;// 0.1f; timespan between cells collecting
		private bool manualStep = false;
        #endregion temp

        #region debug
        [Header("Debug")]
        [SerializeField]
        private bool anySwap = false;
        #endregion debug

        public static GameBoard Instance  { get; private set; }

        #region regular
        private void Awake()
        {
            if (Instance) Destroy(gameObject);
            else
            {
                Instance = this;
            }
#if UNITY_EDITOR
          if(GCSet && GCSet.testMode)  GameLevelHolder.CurrentLevel = Mathf.Abs(GCSet.testLevel);
#endif
            ScoreHolder.Instance.SetCount(0);
        }

        private void Start()
        {
            #region game sets 
            if (!GCSet)
            {
                Debug.Log("Game construct set not found!!!");
                return;
            }

            if (!MainLCSet)
            {
                Debug.Log("Level construct set not found!!! - " + GameLevelHolder.CurrentLevel);
                return;
            }
            
            if (!GOSet)
            {
                Debug.Log("MatcSet not found!!! - " + GameLevelHolder.CurrentLevel);
                return;
            }
            #endregion game sets 
            
            FullLevelMission = MainLCSet.levelMission;
            if (LcSetAdds != null && LcSetAdds.Count > 0)
            {
                foreach (var item in LcSetAdds)
                {
                  if(item)  FullLevelMission = FullLevelMission + item.levelMission;
                }
            }

            Debug.Log("targets");
            #region targets
            Targets = new Dictionary<int, TargetData>();
            CurTargets = new Dictionary<int, TargetData>();
            #endregion targets

            DestroyGrid();
            CreateGameBoard();
            GameLevelHolder.StartLevel();

            if (GMode == GameMode.Edit)
            {
#if UNITY_EDITOR
                Debug.Log("start edit mode");
                foreach (var item in GOSet.TargetObjects) // add all possible targets
                {
                    Targets [item.ID] = new TargetData(item.ID, FullLevelMission.GetTargetCount(item.ID));
                }

                if (gConstructor)
                {
                    gConstructor.gameObject.SetActive(true);
                    gConstructor.InitStart();
                }
                foreach (var item in GOSet.BoosterObjects)
                {
                    if (item.Use) item.ChangeUse();
                }
#endif
            }

            else if (GMode == GameMode.Play)
            {
                MatchScore = 10;
                Debug.Log("start play mode");
                WinContr.InitStart();

                if (gConstructor) DestroyImmediate(gConstructor.gameObject);

                ScoreHolder.Instance.SetAverageScore(WinContr.IsTimeLevel? Mathf.Max(40, WinContr.MovesRest) * 30 : WinContr.MovesRest * 30  );
                WinContr.TimerLeft30Event += () =>
                {
                    if (timeLeftMessagePrefab)
                    {
                        timeLeftMessagePrefab.CreateWindowAndClose(1);
                    }
                };

                WinContr.MovesLeft5Event += () => 
                {
                    if (WinContr.Result != GameResult.WinAuto)
                    {
                        if (movesLeftMessagePrefab) movesLeftMessagePrefab.CreateWindowAndClose(1);
                    }
                };

                WinContr.LevelWinEvent += () =>
                {
                    MGui.ShowPopUpByDescription("victory");
                    MGLevel.PassLevel();
                    foreach (var item in  GOSet.BoosterObjects)
                    {
                        if (item.Use) item.ChangeUse();
                    }
                };

                WinContr.LevelPreLooseEvent += () =>
                {
                   if(CoinsHolder.Count>=almostCoins) MGui.ShowPopUpByDescription("almost");
                };

                WinContr.LevelLooseEvent += () =>
                {
                    MGui.ShowPopUpByDescription("failed");
                    if(!GCSet.UnLimited)  LifesHolder.Add(-1);
                    foreach (var item in GOSet.BoosterObjects)
                    {
                        if (item.Use) item.ChangeUse();
                    }
                };
                WinContr.AutoWinEvent += () =>
                {
                    if(AutoVictoryPrefab) AutoVictoryPrefab.CreateWindowAndClose(1);
                };

                foreach (var item in GOSet.TargetObjects)
                {
                    if (FullLevelMission.Targets.ContainObjectID(item.ID) && (FullLevelMission.Targets.CountByID(item.ID) > 0))
                        Targets[item.ID] = new TargetData(item.ID, FullLevelMission.GetTargetCount(item.ID));
                }

                foreach (var item in GOSet.TargetObjects)
                {
                    if (MainGrid.LcSet.levelMission.Targets.ContainObjectID(item.ID) && (MainGrid.LcSet.levelMission.Targets.CountByID(item.ID) > 0))
                        CurTargets [item.ID] =  new TargetData(item.ID, MainGrid.LcSet.levelMission.GetTargetCount(item.ID));
                }

                // collect preloaded hidden
                HiddenObject hObject = null;
                foreach (var item in CurrentGrid.Cells)
                {
                    HiddenObject hO = item.Hidden;
                    if (hO)
                    {
                        hO.Hit(null, null);
                        hObject = hO;
                    }
                }

                Action missionAction = () => {
                    if (showMission && missionPrefab)
                    {
                        MGui.ShowPopUp(missionPrefab, () =>
                        {
                            if (WinContr.IsTimeLevel) WinContr.Timer.Start();
                            MbState = MatchBoardState.Fill;
                            canPlay = true;
                        });
                    }
                    else
                    {
                        if (WinContr.IsTimeLevel) WinContr.Timer.Start();
                        MbState = MatchBoardState.Fill;
                        canPlay = true;
                    }
                };

                if (MainLCSet.LevelStartStoryPage)
                {
                    MGui.ShowPopUp(MainLCSet.LevelStartStoryPage, missionAction);
                }
                else
                {
                    missionAction?.Invoke();
                }
                showMission = true;
            }

            ShowGrid(MainGrid, 0, null);
            CurrentGrid.CalcObjects();
        }

        private void Update()
        {
            if (!canPlay) return;
            if (WinContr.Result == GameResult.Win) return;
            if (WinContr.Result == GameResult.Loose) return;

            WinContr.UpdateTimer(Time.time);

            // check board state
            switch (MbState)
            {
                case MatchBoardState.ShowEstimate:
                    ShowEstimateState();
                    break;

                case MatchBoardState.Fill:
                    FillState();
                    break;

                case MatchBoardState.Collect:
                    CollectState();
                    break;

                case MatchBoardState.Iddle:
                    IddleState();
                    break;
            }
        }
        #endregion regular

        #region grid construct
        public void CreateGameBoard()
        {
            Debug.Log("Create gameboard ");
            Debug.Log("level set: " + MainLCSet.name);
            Debug.Log("additional level sets: " + ((LcSetAdds!=null) ? LcSetAdds.Count.ToString()  : "not found"));
            Debug.Log("curr level: " + GameLevelHolder.CurrentLevel);

            BackGround = GOSet.GetBackGround(MainLCSet.BackGround);

            if (GMode == GameMode.Play)
            {
                Func<LevelConstructSet, Transform, MatchGrid> create = (lC, cont) =>
                {
                    MatchGrid g = new MatchGrid(lC, GOSet, cont, SortingOrder.Base, GMode);
                    // set cells delegates
                    for (int i = 0; i < g.Cells.Count; i++)
                    {
                        g.Cells[i].GCPointerDownEvent = MatchPointerDownHandler;
                        g.Cells[i].GCDragEnterEvent = MatchDragEnterHandler;
                        g.Cells[i].GCDoubleClickEvent = MatchDoubleClickHandler;
                    }
                    MaxDragDistance = Vector3.Distance(g.Cells[0].transform.position, g.Cells[1].transform.position);
                    g.FillGrid(true);

                    // create spawners
                    g.haveFillPath = lC.HaveFillPath(g);
                    if (g.haveFillPath)
                    {
                        foreach (var item in lC.spawnCells)
                        {
                          if(g[item.Row, item.Column])  g[item.Row, item.Column].CreateSpawner(spawnerPrefab, Vector2.zero);
                        }

                    }
                    else
                    {
                        g.Columns.ForEach((c) =>
                        {
                            c.CreateTopSpawner(spawnerPrefab, spawnerStyle, GridContainer.lossyScale, transform);
                        });
                    }

                    // create pathes to spawners
                    CreateFillPath(g);

                    g.Cells.ForEach((c) =>
                    {
                        c.CreateBorder();
                    });
                    return g;
                };

                SwapHelper.SwapEndEvent = MatchEndSwapHandler;
                SwapHelper.SwapBeginEvent = MatchBeginSwapHandler;
                BombCombiner.CombineCompleteEvent = () => {
                    SetControlActivity(true, true);
                    WinContr.MakeMove();
                    MbState = MatchBoardState.Fill;
                };
                MainGrid = create(MainLCSet, GridContainer);
                AdditionalGrids = new Dictionary<LevelConstructSet, MatchGrid>();
                if (LcSetAdds != null)
                    foreach (var item in LcSetAdds)
                    {
                        if (item)
                        {
                            GameObject g = new GameObject("addGrid " + item.name);
                            g.transform.parent = transform;
                            g.transform.localPosition = GridContainer.localPosition + new Vector3(-20, 0, 0);
                            MatchGrid mG = create(item, g.transform);
                            AdditionalGrids.Add(item, mG);
                        }
                    }
            }
            else // edit mode
            {
#if UNITY_EDITOR

                FullLevelMission = MainLCSet.levelMission;
                if (LcSetAdds != null && LcSetAdds.Count > 0)
                {
                    foreach (var item in LcSetAdds)
                    {
                        if (item) FullLevelMission = FullLevelMission + item.levelMission;
                    }
                }

                // main grid
                if (MainGrid != null && MainGrid.LcSet == MainLCSet)
                {
                    MainGrid.Rebuild(GOSet, GMode);
                }
                else
                {
                    DestroyGrid();
                    MainGrid = new MatchGrid(MainLCSet, GOSet, GridContainer, SortingOrder.Base, GMode);
                }

                // set cells delegates for constructor
                for (int i = 0; i < MainGrid.Cells.Count; i++)
                {
                    MainGrid.Cells[i].GCPointerDownEvent = (c) =>
                     {
                         gConstructor.GetComponent<GameConstructor>().Cell_Click(c);
                     };

                    MainGrid.Cells[i].GCDragEnterEvent = (c) =>
                    {
                        gConstructor.GetComponent<GameConstructor>().Cell_Click(c);
                    };
                }

                // additional grids
                if (AdditionalGrids == null) AdditionalGrids = new Dictionary<LevelConstructSet, MatchGrid>();
                Dictionary<LevelConstructSet, MatchGrid> newAddGrids = new Dictionary<LevelConstructSet, MatchGrid>();

                if (LcSetAdds != null)
                {
                    foreach (var item in LcSetAdds)
                    {
                        if (item && AdditionalGrids.ContainsKey(item))
                        {
                            AdditionalGrids[item].Rebuild(GOSet, GMode);
                            newAddGrids.Add(item, AdditionalGrids[item]);
                            AdditionalGrids.Remove(item);
                        }
                        else if (item)
                        {
                            GameObject g = new GameObject("addGrid " + item.name);
                            g.transform.parent = transform;
                            g.transform.localPosition = GridContainer.localPosition + new Vector3(-20, 0, 0);
                            newAddGrids.Add(item, new MatchGrid(item, GOSet, g.transform, SortingOrder.Base, GMode));
                        }
                    }
                }

                foreach (var item in AdditionalGrids.Values)
                {
                    if (item != null)
                    {
                        Destroy(item.Parent.gameObject);
                    }
                }

                AdditionalGrids = newAddGrids;

                foreach (var item in AdditionalGrids.Values)
                {
                    // set cells delegates for constructor
                    for (int i = 0; i < item.Cells.Count; i++)
                    {
                        item.Cells[i].GCPointerDownEvent = (c) =>
                        {
                            gConstructor.GetComponent<GameConstructor>().Cell_Click(c);
                        };

                        item.Cells[i].GCDragEnterEvent = (c) =>
                        {
                            gConstructor.GetComponent<GameConstructor>().Cell_Click(c);
                        };
                    }
                }
#endif
            }
            ShowGrid(CurrentGrid, 0, null);
        }

        private void CreateFillPath(MatchGrid g)
        {
            if (!g.haveFillPath)
            {
                Debug.Log("Make gravity fill path");
                Map map = new Map(g);
                PathFinder pF = new PathFinder();

                g.Cells.ForEach((c) =>
                {
                    if (!c.Blocked && !c.IsDisabled && !c.MovementBlocked)
                    {
                        int length = int.MaxValue;
                        List<GridCell> path = null;
                        g.Columns.ForEach((col) =>
                        {
                            if (col.Spawn)
                            {
                                if (col.Spawn.gridCell != c)
                                {
                                    pF.CreatePath(map, c.pfCell, col.Spawn.gridCell.pfCell);
                                    if (pF.FullPath != null && pF.PathLenght < length) { path = pF.GCPath(); length = pF.PathLenght; }
                                }
                                else
                                {
                                    length = 0;
                                    path = new List<GridCell>();
                                }
                            }
                        });
                        c.fillPathToSpawner = path;
                    }
                });
            }
            else
            {
                Debug.Log("Have predefined fill path");
                PBoard pBoard = g.LcSet.GetBoard(g);
                g.Cells.ForEach((c) =>
                {
                    if (!c.Blocked && !c.IsDisabled && !c.MovementBlocked && !c.spawner)
                    {
                     //   Debug.Log("path for " + c);
                        GridCell next = c;
                        List<GridCell> path = new List<GridCell>();
                        GridCell mather = null;
                        GridCell neigh = null;
                        bool end = false;
                        DirMather dir = DirMather.None;
                        bool clampDir = false;
                        while (!end)
                        {
                            dir = (!clampDir) ? pBoard[next.Row, next.Column] : dir;
                            NeighBors nS = next.Neighbors;
                       //     Debug.Log(dir);
                            switch (dir)
                            {
                                case DirMather.None:
                                    neigh = null;
                                    break;
                                case DirMather.Top:
                                    neigh = nS.Top;
                                    break;
                                case DirMather.Right:
                                    neigh = nS.Right;
                                    break;
                                case DirMather.Bottom:
                                    neigh = nS.Bottom;
                                    break;
                                case DirMather.Left:
                                    neigh = nS.Left;
                                    break;
                            }

                            if (neigh && neigh.spawner)
                            {
                                //  Debug.Log("spawner neigh " + neigh);
                                path.Add(neigh);
                                if (mather) mather = neigh;
                                end = true;
                            }
                            else if (!neigh)
                            {
                                //  Debug.Log("none neigh ");
                                end = true;
                                path = null;
                            }
                            else if (neigh)
                            {
                                if (!neigh.Blocked && !neigh.IsDisabled && !neigh.MovementBlocked)
                                {
                                    if (path.Contains(neigh)) // corrupted path
                                    {
                                        // Debug.Log("corruptred neigh " + neigh);
                                        end = true;
                                        path = null;
                                    }
                                    else
                                    {
                                        clampDir = false;
                                        path.Add(neigh);
                                        next = neigh;
                                        // Debug.Log("add " + neigh);
                                        clampDir = pBoard[next.Row, next.Column] == DirMather.None; // предусмотреть отсутствие направление у ячейки (save pevious dir)
                                    }
                                }
                                else if (neigh.IsDisabled) // passage cell
                                {
                                    next = neigh;
                                    clampDir = true;
                                    //  Debug.Log("disabled " + neigh);
                                }
                                else
                                {
                                    //  Debug.Log("another block " + neigh);
                                    end = true;
                                    path = null;
                                }
                            }
                        }
                        c.fillPathToSpawner = path;
                    }
                });
            }
        }

        /// <summary>
        /// destroy default main grid cells
        /// </summary>
        private void DestroyGrid()
        {
            GridCell[] gcs = gameObject.GetComponentsInChildren<GridCell>();
            for (int i = 0; i < gcs.Length; i++)
            {
                Destroy(gcs[i].gameObject);
            }
        }

        public void ShowGrid(MatchGrid nGrid, float time, Action completeCallBack)
        {
            Debug.Log("show grid");
            if (time == 0)
            {
                CurrentGrid = null;

                foreach (var item in AdditionalGrids)
                {
                    if (item.Value != nGrid)
                    {
                        item.Value.Parent.localPosition = new Vector3(-20, 0, 0);
                    }
                    else
                    {
                        CurrentGrid = item.Value;
                    }
                }

                if (MainGrid != nGrid)
                {
                    MainGrid.Parent.localPosition = new Vector3(-20, 0, 0);
                }
                else
                {
                    CurrentGrid = MainGrid;
                }

                if (CurrentGrid == null)
                {
                    CurrentGrid = MainGrid; // as default
                }

                CurrentGrid.Parent.localPosition = new Vector3(0, 0, 0);
            }
            else
            {
                Vector3 startPos = new Vector3(-20, 0, 0);
                Vector3 endPos = new Vector3(20, 0, 0);
                switch (moveDir)
                {
                    case MoveDir.Left:
                        startPos = new Vector3(20, 0, 0);
                        endPos = new Vector3(-20, 0, 0);
                        break;
                    case MoveDir.Right:
                        startPos = new Vector3(-20, 0, 0);
                        endPos = new Vector3(20, 0, 0);
                        break;
                    case MoveDir.Up:
                        startPos = new Vector3(0, -27, 0);
                        endPos = new Vector3(0, 27, 0);
                        break;
                    case MoveDir.Down:
                        startPos = new Vector3(0, 27, 0);
                        endPos = new Vector3(0, -27, 0);
                        break;
                }
                Transform cGParent = CurrentGrid.Parent;
                SimpleTween.Value(cGParent.gameObject, cGParent.localPosition, endPos, time).SetOnUpdate((Vector3 lPos)=> { cGParent.localPosition = lPos; });
                CurrentGrid = nGrid;
                SimpleTween.Value(nGrid.Parent.gameObject, startPos, new Vector3(0, 0, 0), time).SetOnUpdate((Vector3 lPos) => { nGrid.Parent.localPosition = lPos; }).AddCompleteCallBack(completeCallBack);
            }
            #region matchgroups
            CollectGroups = new MatchGroupsHelper(CurrentGrid);
            CollectGroups.ComboCollect = ComboCollectHandler;
            CollectGroups.Collect = CollectHandler;
            EstimateGroups = new MatchGroupsHelper(CurrentGrid);
            #endregion matchgroups
        }

        public void ShowNextGrid(float time)
        {
            Debug.Log("show next grid");
            SetControlActivity(false, false);
            MbState = MatchBoardState.Waiting;
            ShowGrid(NextGrid, time, ()=> 
            {
                SetControlActivity(true, true);
                MbState = MatchBoardState.Collect;
            });

            foreach (var item in GOSet.TargetObjects)
            {
                if (CurrentGrid.LcSet.levelMission.Targets.ContainObjectID(item.ID) && (CurrentGrid.LcSet.levelMission.Targets.CountByID(item.ID) > 0))
                {
                    if (!CurTargets.ContainsKey(item.ID)) CurTargets.Add(item.ID, new TargetData(item.ID, CurrentGrid.LcSet.levelMission.GetTargetCount(item.ID)));
                    else CurTargets[item.ID].IncNeedCount (CurrentGrid.LcSet.levelMission.GetTargetCount(item.ID));
                }
            }

        }
        #endregion grid construct

        #region bombs
        /// <summary>
        /// async collect matched objects in a group
        /// </summary>
        /// <param name="completeCallBack"></param>
        internal void ExplodeClickBomb(GridCell c)
        {
            if (!c.DynamicClickBomb)
            {
                return;
            }
            MbState = MatchBoardState.Waiting;
            SetControlActivity(false, false);
            c.ExplodeBomb(0.0f, showBombExplode, true, flyCollected, ()=>
            {
                MbState = MatchBoardState.Fill;
            });

        }
        #endregion bombs

        #region states
        private List<FallingObject> GetFalling()
        {
            List<GridCell> botCell = CurrentGrid.GetBottomDynCells();
            List<FallingObject> res = new List<FallingObject>();
            foreach (var item in botCell)
            {
                if (item)
                {
                    FallingObject f = item.Falling;
                    if (f)
                    {
                        res.Add(f);
                    }
                }
            }
            return res;
        }

        private void CollectFalling(Action completeCallBack)
        {
         //   Debug.Log("collect falling " + GetFalling().Count);
            ParallelTween pt = new ParallelTween();
            foreach (var item in GetFalling())
            {
                pt.Add((callBack)=> 
                {
                    item.Collect(0, false, true, callBack);
                });
            }
            pt.Start(completeCallBack);
        }

        private void CollectState()
        {
            MbState = MatchBoardState.Waiting;
            collected = 0;

            if (CurrentGrid.GetFreeCells(true).Count > 0)
            {
                MbState = MatchBoardState.Fill;
                return;
            }

            SetControlActivity(false, false); //Debug.Log("collect matches");

            CollectFalling(()=> { WinContr.CheckResult(); });

            CollectGroups.CancelTweens();
            CollectGroups.CreateGroups(3, false);

            if (CollectGroups.Length == 0) // no matches
            {
                SetControlActivity(true, true);
                WinContr.CheckResult();
                if (CurrentGrid.GetFreeCells(true).Count > 0)
                    MbState = MatchBoardState.Fill;
                else
				{
                    MbState = MatchBoardState.ShowEstimate;
				    if (manualStep)
                    {
                        manualStep = false;
                        AfterStepBoardEvent?.Invoke(this);
                    }
				}
            }
            else
            {
                BeforeCollectBoardEvent?.Invoke(this);
                MatchScore = scoreController.GetScoreForMatches(CollectGroups.Length);
                CollectGroups.CollectMatchGroups(() => { GreatingMessage(); MbState = MatchBoardState.Fill; MatchScore = scoreController.GetScoreForMatches(0); });
            }
        }

        private void FillGridByStep(List<GridCell> freeCells, Action completeCallBack)
        {
            if (freeCells.Count == 0)
            {
                completeCallBack?.Invoke();
                return;
            }

            ParallelTween tp = new ParallelTween();
            foreach (GridCell gc in freeCells)
            {
                tp.Add((callback) =>
                {
                    gc.FillGrab(callback);
                });
            }
            tp.Start(() =>
            {
                completeCallBack?.Invoke();
            });
        }

        private void FillState()
        {
            if (fStarted) return;
            StartCoroutine(FillStateC());
            return;
        }

        bool fStarted = false;
        private IEnumerator FillStateC()
        {
            fStarted = true;
            List<GridCell> gFreeCells = CurrentGrid.GetFreeCells(true); // Debug.Log("fill free count: " + gFreeCells.Count + " to collapse" );
            bool filled = false;
            Debug.Log("gFreeCells : " + gFreeCells.Count);
            if (gFreeCells.Count > 0)
            {
                
                CreateFillPath(CurrentGrid);
                SetControlActivity(false, false);
                Debug.Log("before fill");
                BeforeFillBoardEvent?.Invoke(this);
            }

            while (gFreeCells.Count > 0)
            {
                FillGridByStep(gFreeCells, () => {});
                yield return new WaitForEndOfFrame();
                gFreeCells = CurrentGrid.GetFreeCells(true);
                filled = true;
            }
            while (!CurrentGrid.NoPhys()) yield return new WaitForEndOfFrame();
            if (filled) AfterFillBoardEvent?.Invoke(this);
            MbState = MatchBoardState.Collect;
            fStarted = false;
        }

        private void PlayIddleAnimRandomly()
        {
            if ((Time.time - lastPlayTime) < 5.3f) return;
            int randCell = UnityEngine.Random.Range(0, CurrentGrid.Cells.Count);
            CurrentGrid.Cells[randCell].PlayIddle();
            lastPlayTime = Time.time;
        }

        private void IddleState()
        {
            PlayIddleAnimRandomly();
            if (Time.time - lastiEstimateShowed >= 5f)
            {
                lastiEstimateShowed = Time.time;
                MbState = MatchBoardState.ShowEstimate;
            }
        }

        private void ShowEstimateState()
        {
            if (!CurrentGrid.NoPhys()) return;

            MbState = MatchBoardState.Waiting;
            EstimateGroups.CancelTweens();
            EstimateGroups.CreateGroups(2, true);

            if (EstimateGroups.Length == 0)
            {
                MixGrid(null);
                return;
            }

            if (WinContr.Result != GameResult.WinAuto)
            {
                lastiEstimateShowed = Time.time; //  Debug.Log("show estimate");
                if (HardModeHolder.Mode == HardMode.Easy)
                {
                    EstimateGroups.ShowNextEstimateMatchGroups(() =>
                    {
                        MbState = MatchBoardState.Iddle; // go to iddle state
                    });
                }
                else
                {
                    MbState = MatchBoardState.Iddle; // go to iddle state
                }
            }
            else
            {
                MakeStep(); // make auto step
            }
        }

        public void MixGrid(Action completeCallBack)
        {
            ParallelTween pT0 = new ParallelTween();
            ParallelTween pT1 = new ParallelTween();

            TweenSeq tweenSeq = new TweenSeq();
            List<GridCell> cellList = new List<GridCell>();
            List<GameObject> goList = new List<GameObject>();
            CollectGroups.CancelTweens();
            EstimateGroups.CancelTweens();

            CurrentGrid.Cells.ForEach((c) => { if (c.IsMixable) { cellList.Add(c); goList.Add(c.DynamicObject); } });
            cellList.ForEach((c) => { pT0.Add((callBack) => { c.MixJump(transform.position, callBack); }); });

            cellList.ForEach((c) =>
            {
                int random = UnityEngine.Random.Range(0, goList.Count);
                GameObject m = goList[random];
                pT1.Add((callBack) => { c.GrabDynamicObject(m.gameObject, false, callBack); });
                goList.RemoveAt(random);
            });

            tweenSeq.Add((callBack) =>
            {
                pT0.Start(callBack);
            });

            tweenSeq.Add((callBack) =>
            {
                pT1.Start(() =>
                {
                    MbState = MatchBoardState.Fill;
                    completeCallBack?.Invoke();
                    callBack();
                });
            });
            tweenSeq.Start();
        }

        internal void SetControlActivity(bool activityGrid, bool activityMenu)
        {
            if(WinContr.Result == GameResult.None || WinContr.Result == GameResult.PreLoose) // control touch activity only for live game
            {
                TouchManager.SetTouchActivity(activityGrid);
                HeaderGUIController.Instance.SetControlActivity(activityMenu);
                FooterGUIController.Instance.SetControlActivity(activityMenu);
            }
            else
            {
                TouchManager.SetTouchActivity(false);
                HeaderGUIController.Instance.SetControlActivity(false);
                FooterGUIController.Instance.SetControlActivity(false);
            }
        }
        #endregion states

        #region swap helper handlers
        private void MatchBeginSwapHandler(GridCell source, GridCell target)
        {
            if (GMode == GameMode.Play)
            {
                SetControlActivity(false, false);
            }
        }
  
        private void MatchEndSwapHandler(GridCell source, GridCell target)
        {
            if (GMode == GameMode.Play)
            {
                collected = 0; // reset collected count
                CollectGroups.CreateGroups(3, false);
                if (CollectGroups.Length == 0)   // no matches
                {
                    if (!anySwap)
                        SwapHelper.UndoSwap(() =>
                        {
                            SetControlActivity(true, true);
                            MbState = MatchBoardState.Fill;
                        });

                    else
                    {
                        SetControlActivity(true, true);
                        WinContr.MakeMove();
                        MbState = MatchBoardState.Fill;
                    }
                }
                else
                {
					manualStep = true;
                    BeforeStepBoardEvent?.Invoke(this);
                    SetControlActivity(true, true);
                    WinContr.MakeMove();
                    MbState = MatchBoardState.Fill;     Debug.Log("end swap -> to fill");
                }
            }
        }
        #endregion swap helper handlers

        #region gridcell handlers
        private void MatchPointerDownHandler(GridCell c)
        {
            if (GMode == GameMode.Play)
            {
                if (CurrentGrid.NoPhys())
                {
                    CollectGroups.CancelTweens();
                    EstimateGroups.CancelTweens();
                    ApplyBooster(c);
                    ExplodeClickBomb(c);
                }
            }
            else if (GMode == GameMode.Edit)
            {
#if UNITY_EDITOR
              //  gConstructor.GetComponent<GameConstructor>().selected = c;
#endif
            }
        }

        private void MatchDragEnterHandler(GridCell c)
        {
            if (GMode == GameMode.Play)
            {
                SwapHelper.Swap();
            }
        }

        private void MatchDoubleClickHandler(GridCell c)
        {

        }

        /// <summary>
        /// Raise for each collected matchobject
        /// </summary>
        /// <param name="gCell"></param>
        /// <param name="mData"></param>
        public void TargetCollectEventHandler(int id)
        {
            if (GMode == GameMode.Play)
            {
                if (Targets.ContainsKey(id)) Targets[id].IncCurrCount();
                if (CurTargets.ContainsKey(id)) CurTargets[id].IncCurrCount();
                GameEvents.CollectGridObject?.Invoke(id);
            }
        }

        /// <summary>
        /// Raise for each collected matchobject
        /// </summary>
        /// <param name="gCell"></param>
        /// <param name="mData"></param>
        public void TargetAddEventHandler(int id)
        {
            if (GMode == GameMode.Play)
            {
                if (Targets.ContainsKey(id)) Targets[id].IncNeedCount(1);
				if (CurTargets.ContainsKey(id)) CurTargets[id].IncNeedCount(1);
            }
        }

        public bool IsTarget(int ID)
        {
            return (Targets!=null && Targets.ContainsKey(ID));
        }

        public void MatchScoreCollectHandler()
        {
            //collected += 1;
            //if (collected <= 3) MPlayer.AddScore(10);
            //else MPlayer.AddScore(20);
        }
        #endregion gridcell handlers

        #region match group handlers score counter
        private void ComboCollectHandler(MatchGroupsHelper mgH)
        {
            // combo message
        }

        private void GreatingMessage()
        {
            if (WinContr.Result == GameResult.WinAuto) return; //  EffectsHolder.Instance.InstantiateScoreFlyerAtPosition(s, scoreFlyerPos, f);
            int add = (collected - 3) * 10;
            int score = collected * 10 + Math.Max(0, add);
            if (score > 59 && score < 89)
            {
                Debug.Log("GOOD");
                if(goodPrefab) goodPrefab.CreateWindowAndClose(1);
            }
            else if (score > 89 && score < 119)
            {
                Debug.Log("GREAT");
                if(greatPrefab) greatPrefab.CreateWindowAndClose(1);
            }
            else if (score > 119)
            {
                Debug.Log("EXCELLENT");
                if(excellentPrefab) excellentPrefab.CreateWindowAndClose(1);
            }
        }

        /// <summary>
        /// async collect matched objects in a group
        /// </summary>
        /// <param name="completeCallBack"></param>
        internal void CollectHandler(MatchGroup m, Action completeCallBack)
        {
            float delay = 0;

            SetHidden(m.Cells);

            if (m.Length > 3 && m.BombsCount == 0 && bombCreator) // create bomb
            {
                BombCreator bC = Instantiate(bombCreator);
                bC.Create(bombType, m, showScore, scoreController.GetBombScore(m.Length), completeCallBack);
                ScoreHolder.Add(scoreController.GetBombScore(m.Length));
                return;
            }

            ParallelTween collectTween = new ParallelTween();
            foreach (GridCell c in m.Cells)
            {
                delay += collectDelay;
                float d = delay;
                collectTween.Add((callBack) => { c.CollectMatch(d, true, flyCollected, true, true, showBombExplode, showScore, MatchScore, callBack); });
            }
            ScoreHolder.Add(MatchScore*m.Length);
            collectTween.Start(completeCallBack);
        }

        internal void SetHidden(List<GridCell> cells)
        {
            HiddenObject hO = null;
            foreach (var item in cells)
            {
                hO = item.Hidden;
                if (hO) break;
            }

            if (hO)
            {
                foreach (var item in cells)
                {
                    if (!item.Hidden && !item.Overlay && !item.Underlay)
                    {
                        item.SetObject(GOSet.GetHiddenObject(hO.ID));
                    }
                }
            }
        }
        #endregion match group handlers score counter

        private void MakeStep()
        {
            GridCell bomb = CurrentGrid.GetBomb();
            if (bomb)
            {
                SetControlActivity(false, false);
                MbState = MatchBoardState.Waiting;
                bomb.ExplodeBomb(0, true, false, flyCollected, () =>
                {
                    WinContr.MakeMove();
                    MbState = MatchBoardState.Fill;
                    SetControlActivity(true, true);
                });
                return;
            }

            if (autoWin== AutoWin.Slow)
            {
                if (EstimateGroups.Length > 0)
                {
                    EstimateGroups.SwapEstimate();
                }
            }
            else if(autoWin== AutoWin.Fast)
            {
                SetControlActivity(false, false);
                int moves = WinContr.MovesRest;
                int bombsCount = Mathf.Min(moves, 3);
                List<GridCell> cells = CurrentGrid.GetRandomMatch(bombsCount);

                MbState = MatchBoardState.Waiting;
                if (cells.Count > 2)
                    BombObject.ExplodeArea(cells[2].GColumn.cells, 0, true, true, flyCollected, true, () =>
                    {
                        WinContr.MakeMove();
                    });

                if (cells.Count > 1)
                    BombObject.ExplodeArea(cells[1].GRow.cells, 0.05f, true, true, flyCollected, true, () =>
                   {
                       WinContr.MakeMove();
                   });

                BombObject.ExplodeArea(cells[0].GRow.cells, 0.15f, true, true, flyCollected, true, () =>
                {
                    // set 3 bombs
                    WinContr.MakeMove();
                    MbState = MatchBoardState.Fill; //    Debug.Log("end swap -> to fill");
                    SetControlActivity(true, true);
                });

            }
            else if (autoWin == AutoWin.Bombs)
            {
                Rockets();
            }
        }

        public void Rockets()
        {
            SetControlActivity(false, false);
            int moves = WinContr.MovesRest;
            int bombsCount = Mathf.Min(moves, 10); // ?????
            List<GridCell> cells = CurrentGrid.GetRandomMatch(bombsCount);
            cells.RemoveAll((c)=>{ return c.MatchProtected; });
            cells.Shuffle();
            bombsCount = cells.Count;

            MbState = MatchBoardState.Waiting;

            TweenSeq anim = new TweenSeq();
            ParallelTween pT = new ParallelTween();
            ParallelTween pT1 = new ParallelTween();
            Action<GameObject, float, Action> delayAction = (g, del, callBack) => { SimpleTween.Value(g, 0, 1, del).AddCompleteCallBack(callBack); };

            anim.Add((callBack) =>
            {
                pT1.Start(callBack);
            });

            anim.Add((callBack) => // create rockets
            {
                foreach (var item in cells)
                {
                    BombDir bd = UnityEngine.Random.Range(0, 2) == 0 ? BombDir.Horizontal : BombDir.Vertical;
                    BombObject r = null;
                    switch (bombType)
                    {
                        case BombType.StaticMatch:
                            r = StaticMatchBombObject.Create(item, GOSet.GetStaticMatchBombObject(bd, item.Match.ID), TargetCollectEventHandler);
                            break;
                        case BombType.DynamicMatch:
                            r = DynamicMatchBombObject.Create(item, GOSet.GetDynamicMatchBombObject(bd, item.Match.ID), TargetCollectEventHandler);
                            r.SetToFront(true);
                            break;
                        case BombType.DynamicClick:
                            r = DynamicClickBombObject.Create(item, GOSet.GetDynamicClickBombObject(bd), TargetCollectEventHandler);
                            r.SetToFront(true);
                            break;
                        default:
                            r = DynamicClickBombObject.Create(item, GOSet.GetDynamicClickBombObject(bd), TargetCollectEventHandler);
                            r.SetToFront(true);
                            break;
                    }

                    pT.Add((cB) =>
                    {
                        item.ExplodeBomb(UnityEngine.Random.Range(0,2f), true, true, false, cB);
                    });
                }
                callBack();
            });

            anim.Add((callBack) => // delay
            {
                delayAction(gameObject, 1.0f, callBack);
            });

            anim.Add((callBack) =>
            {
                pT.Start(callBack);
            });

            anim.Add((callBack) =>
            {
                WinContr.MakeMove(bombsCount);
                MbState = MatchBoardState.Fill; //    Debug.Log("end swap -> to fill");
                SetControlActivity(true, true);
                callBack();
            });

            anim.Start();
        }

        public bool NeedAlmostMessage()
        {
            return showAlmostMessage && (almostCoins <= CoinsHolder.Count) && MGui.GetPopUpPrefabByDescription("almost");
        }

        #region undo
        ///// <summary>
        /////  Save Undo state of the match board
        ///// </summary>
        //internal void SaveUndoState()
        //{
        //    if (undoStates == null) undoStates = new List<DataState>();
        //    if (undoStates.Count >= 5)
        //    {
        //        undoStates.RemoveAt(0);
        //    }
        //    DataState ds = new DataState(this, MPlayer);
        //    undoStates.Add(ds);
        //    // Debug.Log("save undo state" + undoStates.Count);
        //    grid.Cells.ForEach((ct) => { ct.SaveUndoState(); });
        //}

        ///// <summary>
        ///// set the prev state on board
        ///// </summary>
        //public void PreviousState()
        //{
        //    if (GMode == GameMode.Edit) return;
        //    if (Time.timeScale == 0) return;
        //    if (undoStates == null || undoStates.Count == 0) return;
        //    currentGrid.Cells.ForEach((ct) => { ct.PreviousUndoState(); });
        //    DataState ds = undoStates[undoStates.Count - 1];
        //    ds.RestoreState(this, MPlayer);
        //    undoStates.RemoveAt(undoStates.Count - 1);
        //    HeaderGUIController.Instance.Refresh();
        //}

        /// <summary>
        /// Set Time.timescale =(Time.timescale!=0)? 0 : 1
        /// </summary>
        public void Pause()
        {
            if (GMode == GameMode.Edit) return;
            if (Time.timeScale == 0.0f)
            {
                Time.timeScale = 1f;
                SetControlActivity(true, true);
            }

            else if (Time.timeScale > 0f)
            {
                Time.timeScale = 0f;
                SetControlActivity(false, true);
            }
        }
        #endregion undo

        #region boosters
        /// <summary>
        /// Aplly active booster to gridcell
        /// </summary>
        /// <param name="gCell"></param>
        private void ApplyBooster(GridCell gCell)
        {
            collected = 0; // reset collected count
            if (Booster.ActiveBooster != null)
            {
                MbState = MatchBoardState.Waiting;
                SetControlActivity(false, false);
                Booster.ActiveBooster.ApplyToGridM(gCell, ()=> { MbState = MatchBoardState.Fill; });
            }
        }
        #endregion boosters

        bool wave = false;
        public void ExplodeWave(float delay, Vector3 center, float radius, Action completeCallBack)
        {
            if (wave) return;
            wave = true;
            AnimationCurve ac = explodeCurve; //sineCurve;//
            ParallelTween pT = new ParallelTween();
            TweenSeq anim = new TweenSeq();
            float maxDist = radius * CurrentGrid.Step.x;
            float maxAmpl = 1.0f;
            float speed = 15f;
            float waveTime = 0.15f;
            // Debug.Log("WAVE");

            anim.Add((callBack) => // delay
            {
                SimpleTween.Value(gameObject, 0, 1, delay).AddCompleteCallBack(callBack);
            });

            CurrentGrid.Cells.ForEach((tc) =>
            {
                if (tc.DynamicObject)
                {
                    Vector3 tcPos = tc.transform.position;
                    Vector3 dir = tcPos - center;
                    float dirM = dir.magnitude;
                    dirM = (dirM < 1) ? 1 : dirM;
                    dirM = (dirM > maxDist) ? maxDist : dirM;
                    Vector3 dirOne = dir.normalized;
                    float b = maxAmpl;
                    float k = -maxAmpl / maxDist;
                    pT.Add((callBack) =>
                    {
                        SimpleTween.Value(gameObject, 0f, 1f, waveTime).SetOnUpdate((float val) =>
                        {
                            float deltaPos = ac.Evaluate(val);
                            if (tc.DynamicObject) tc.DynamicObject.transform.position = tcPos + dirOne * deltaPos * (k * dirM + b);// new Vector3(deltaPos, deltaPos, 0);
                        }).
                                                                AddCompleteCallBack(() =>
                                                                {
                                                                    if (tc.DynamicObject) tc.DynamicObject.transform.localPosition = Vector3.zero;
                                                                    callBack();
                                                                }).SetDelay(dirM / speed);
                    });
                }
            });

            pT.Start(() => { wave = false; completeCallBack?.Invoke(); });
        }
    }

    public class MatchGroupsHelper
    {
        private List<MatchGroup> mgList;
        private MatchGrid grid;
        public Action<MatchGroupsHelper> ComboCollect;
        public Action<MatchGroup, Action> Collect;

        public int Length
        {
            get { return mgList.Count; }
        }

        /// <summary>
        /// Find match croups on grid and estimate match groups
        /// </summary>
        public MatchGroupsHelper(MatchGrid grid )
        {
             mgList = new List<MatchGroup>();
            this.grid = grid;
         
        }

        public void CreateGroups(int minMatches, bool estimate)
        {
            mgList = new List<MatchGroup>();
            if (!estimate)
            {
                grid.Rows.ForEach((br) =>
                {
                    List<MatchGroup> mgList_t = br.GetMatches(minMatches, false);
                    if (mgList_t != null && mgList_t.Count > 0)
                    {
                        AddRange(mgList_t);
                    }
                });

                grid.Columns.ForEach((bc) =>
                {
                    List<MatchGroup> mgList_t = bc.GetMatches(minMatches, false);
                    if (mgList_t != null && mgList_t.Count > 0)
                    {
                        AddRange(mgList_t);
                    }
                });
            }
            else
            {
                List<MatchGroup> mgList_t = new List<MatchGroup>();
                grid.Rows.ForEach((gr) =>
                {
                    mgList_t.AddRange(gr.GetMatches(minMatches, true));
                });
                mgList_t.ForEach((mg) => { if (mg.IsEstimateMatch(mg.Length, true, grid)) { AddEstimate(mg); } });

                mgList_t = new List<MatchGroup>();
                grid.Columns.ForEach((gc) =>
                {
                    mgList_t.AddRange(gc.GetMatches(minMatches, true));
                });
                mgList_t.ForEach((mg) => { if (mg.IsEstimateMatch(mg.Length, false, grid)) { AddEstimate(mg); } });
            }
        }

        /// <summary>
        /// Add new matchgroup and merge all intersections
        /// </summary>
        public void Add(MatchGroup mG)
        {
            List<MatchGroup> intersections = new List<MatchGroup>();

            for (int i = 0; i < mgList.Count; i++)
            {
                if (mgList[i].IsIntersectWithGroup(mG))
                {
                    intersections.Add(mgList[i]);
                }
            }
            // merge intersections
            if (intersections.Count > 0)
            {
                intersections.ForEach((ints) => { mgList.Remove(ints); });
                intersections.Add(mG);
                mgList.Add(Merge(intersections));
            }
            else
            {
                mgList.Add(mG);
            }
        }

        /// <summary>
        /// Add new estimate matchgroup
        /// </summary>
        public void AddEstimate(MatchGroup mGe)
        {
            for (int i = 0; i < mgList.Count; i++)
            {
                if (mgList[i].IsEqual(mGe))
                {
                    return;
                }
            }
            mgList.Add(mGe);
        }

        /// <summary>
        /// Add new matchgroup List and merge all intersections
        /// </summary>
        public void AddRange(List<MatchGroup> mGs)
        {
            for (int i = 0; i < mGs.Count; i++)
            {
                Add(mGs[i]);
            }
        }

        private MatchGroup Merge(List<MatchGroup> intersections)
        {
            MatchGroup mG = new MatchGroup();
            intersections.ForEach((ints) => { mG.Merge(ints); });
            return mG;
        }

        TweenSeq showSequence;
        public void ShowMatchGroupsSeq(Action completeCallBack)
        {
            showSequence = new TweenSeq();
            if (mgList.Count > 0)
            {
                Debug.Log("show match");
                foreach (MatchGroup mG in mgList)
                {
                    showSequence.Add((callBack) =>
                    {
                        mG.Show(callBack);
                    });
                }
            }
            showSequence.Add((callBack) =>
            {
                if (completeCallBack != null) completeCallBack();
                Debug.Log("show match ended");
                callBack();
            });
            showSequence.Start();
        }

        public void ShowMatchGroupsPar(Action completeCallBack)
        {
            showSequence = new TweenSeq();
            ParallelTween showTweenPar = new ParallelTween();

            if (mgList.Count > 0)
            {
                //  Debug.Log("show match");
                foreach (MatchGroup mG in mgList)
                {
                    showTweenPar.Add((callBack) =>
                    {
                        mG.Show(callBack);
                    });
                }
            }

            showSequence.Add((callBack) =>
            {
                showTweenPar.Start(callBack);
            });

            showSequence.Add((callBack) =>
            {
                if (completeCallBack != null) completeCallBack();
                // Debug.Log("show match ended");
                callBack();
            });
            showSequence.Start();
        }

        TweenSeq showEstimateSequence;
        public void ShowEstimateMatchGroupsSeq(Action completeCallBack)
        {
            showEstimateSequence = new TweenSeq();
            if (mgList.Count > 0)
            {
                foreach (MatchGroup mG in mgList)
                {
                    showEstimateSequence.Add((callBack) => { mG.Show(callBack); });
                }
            }
            showEstimateSequence.Add((callBack) =>
            {
                completeCallBack?.Invoke();
                callBack();
            });
            showEstimateSequence.Start();
        }

        static int next = 0;
        public void ShowNextEstimateMatchGroups(Action completeCallBack)
        {
            showEstimateSequence = new TweenSeq();
            next = (next < mgList.Count) ? next : 0;
            int n = next;
            if (mgList.Count > 0)
            {
                showEstimateSequence.Add((callBack) => { mgList[n].Show(callBack); });
            }
            showEstimateSequence.Add((callBack) =>
            {
                completeCallBack?.Invoke();
                callBack();
            });
            showEstimateSequence.Start();
            next++;
        }

        public void CollectMatchGroups (Action completeCallBack)
        {
            ParallelTween pt = new ParallelTween();

            if (mgList.Count == 0)
            {
                completeCallBack?.Invoke();
                return;
            }

            for (int i = 0; i < mgList.Count; i++)
            {
                if (mgList[i] != null)
                {
                    MatchGroup m = mgList[i];
                    pt.Add((callBack) =>
                    {
                        Collect(m, callBack);
                    });
                }
            }
            pt.Start(() =>
            {
                if (mgList.Count > 1) ComboCollect?.Invoke(this);
                completeCallBack?.Invoke();
            });
        }

        public override string ToString()
        {
            string s = "";
            mgList.ForEach((mg) => { s += mg.ToString(); });
            return s;
        }

        public void CancelTweens()
        {
            if (showSequence != null) { showSequence.Break(); showSequence = null; }
            if (showEstimateSequence != null) { showEstimateSequence.Break(); showEstimateSequence = null; }
            mgList.ForEach((mg) => { mg.CancelTween(); });
        }

        public void SwapEstimate()
        {
            mgList[0].SwapEstimate();
        }
    }

    public class MatchGroup : CellsGroup
    {
        private GridCell est1;
        private GridCell est2;

        public bool IsIntersectWithGroup(MatchGroup mGroup)
        {
            if (mGroup == null || mGroup.Length == 0) return false;
            for (int i = 0; i < Cells.Count; i++)
            {
                if (mGroup.Contain(Cells[i])) return true;
            }
            return false;
        }

        public void Merge(MatchGroup mGroup)
        {
            if (mGroup == null || mGroup.Length == 0) return;
            for (int i = 0; i < mGroup.Cells.Count; i++)
            {
                Add(mGroup.Cells[i]);
            }
        }

        public bool IsEqual(MatchGroup mGroup)
        {
            if (Length != mGroup.Length) return false;
            foreach (GridCell c in Cells)
            {
                if (!mGroup.Contain(c)) return false;
            }
            return true;
        }

        public bool IsEstimateMatch(int matchCount, bool horizontal, MatchGrid grid)
        {
            if (Length != matchCount) return false;
            if (horizontal)
            {
                GridCell L = GetLowermostX();
                GridCell T = GetTopmostX();

                // 3 estimate positions for l - cell (astrics)
                //   1 X X
                // 3 0 L T X
                //   2 X X
                int X0 = L.Column - 1; int Y0 = L.Row;
                GridCell c0 = grid[Y0, X0];

                if ((c0 != null) && c0.IsDraggable() && ((T.Column - L.Column) == 1))
                {
                    int X1 = X0; int Y1 = Y0 - 1;
                    GridCell c1 = grid[Y1, X1];
                    if ((c1 != null) && c1.IsMatchObjectEquals(L) && c1.IsDraggable())
                    {
                        Add(c1);
                        est1 = c0;
                        est2 = c1;
                        return true;
                    }

                    int X2 = X0; int Y2 = Y0 + 1;
                    GridCell c2 = grid[Y2, X2];
                    if ((c2 != null) && c2.IsMatchObjectEquals(L) && c2.IsDraggable())
                    {
                        Add(c2);
                        est1 = c0;
                        est2 = c2;
                        return true;
                    }

                    int X3 = X0 - 1; int Y3 = Y0;
                    GridCell c3 = grid[Y3, X3];
                    if ((c3 != null) && c3.IsMatchObjectEquals(L) && c3.IsDraggable())
                    {
                        Add(c3);
                        est1 = c0;
                        est2 = c3;
                        return true;
                    }
                }

                // 3 estimate positions for T - cell (astrics)
                //    X X 4
                //  X L T 0 6
                //    X X 5
                X0 = T.Column + 1; Y0 = T.Row;
                c0 = grid[Y0, X0];
                if ((c0 != null) && c0.IsDraggable() && ((T.Column - L.Column) == 1))
                {
                    int X4 = X0; int Y4 = Y0 - 1;
                    GridCell c4 = grid[Y4, X4];
                    if ((c4 != null) && c4.IsMatchObjectEquals(T) && c4.IsDraggable())
                    {
                        Add(c4);
                        est1 = c0;
                        est2 = c4;
                        return true;
                    }

                    int X5 = X0; int Y5 = Y0 + 1;
                    GridCell c5 = grid[Y5, X5];
                    if ((c5 != null) && c5.IsMatchObjectEquals(T) && c5.IsDraggable())
                    {
                        Add(c5);
                        est1 = c0;
                        est2 = c5;
                        return true;
                    }

                    int X6 = X0 + 1; int Y6 = Y0;
                    GridCell c6 = grid[Y6, X6];
                    if ((c6 != null) && c6.IsMatchObjectEquals(T) && c6.IsDraggable())
                    {
                        Add(c6);
                        est1 = c0;
                        est2 = c6;
                        return true;
                    }
                }

                // 2 estimate positions for L0T - horizontal
                //    X 7 X
                //  X L 0 T X
                //    X 8 X
                X0 = L.Column + 1; Y0 = L.Row;
                c0 = grid[Y0, X0];
                if ((c0 != null) && c0.IsDraggable() && ((T.Column - L.Column) == 2))
                {
                    int X7 = L.Column + 1; int Y7 = L.Row - 1;
                    GridCell c7 = grid[Y7, X7];
                    if (c7 != null && c7.IsMatchObjectEquals(L) && c7.IsDraggable())
                    {
                        Add(c7);
                        est1 = c0;
                        est2 = c7;
                        return true;
                    }

                    int X8 = L.Column + 1; int Y8 = L.Row + 1;
                    GridCell c8 = grid[Y8, X8];
                    if (c8 != null && c8.IsMatchObjectEquals(L) && c8.IsDraggable())
                    {
                        Add(c8);
                        est1 = c0;
                        est2 = c8;
                        return true;
                    }
                }
            }
            else
            {
                GridCell L = GetLowermostY();
                GridCell T = GetTopmostY();
                // 3 estimate positions for L - cell 
                //     
                //     X 
                //   X T X 
                //   X L X
                //   1 0 2
                //     3
                int X0 = L.Column; int Y0 = L.Row + 1;
                GridCell c0 = grid[Y0, X0];
                if ((c0 != null) && c0.IsDraggable() && ((T.Row - L.Row) == -1))
                {
                    int X1 = X0 - 1; int Y1 = Y0;
                    GridCell c1 = grid[Y1, X1];
                    if ((c1 != null) && c1.IsMatchObjectEquals(L) && c1.IsDraggable())
                    {
                        Add(c1);
                        est1 = c0;
                        est2 = c1;
                        return true;
                    }

                    int X2 = X0 + 1; int Y2 = Y0;
                    GridCell c2 = grid[Y2, X2];
                    if ((c2 != null) && c2.IsMatchObjectEquals(L) && c2.IsDraggable())
                    {
                        Add(c2);
                        est1 = c0;
                        est2 = c2;
                        return true;
                    }

                    int X3 = X0; int Y3 = Y0 + 1;
                    GridCell c3 = grid[Y3, X3];
                    if ((c3 != null) && c3.IsMatchObjectEquals(L) && c3.IsDraggable())
                    {
                        Add(c3);
                        est1 = c0;
                        est2 = c3;
                        return true;
                    }
                }

                // 3 estimate positions for T - cell
                //     6
                //   4 0 5
                //   X T X 
                //   X L X
                //     X 
                X0 = L.Column; Y0 = T.Row - 1;
                c0 = grid[Y0, X0];
             //   Debug.Log("c0: " + c0 + " : " + c0.IsDraggable() +" : " + ((T.Row - L.Row)));
                if ((c0 != null) && c0.IsDraggable() && ((T.Row - L.Row) == -1))
                {
                    int X4 = T.Column - 1; int Y4 = T.Row - 1;
                    GridCell c4 = grid[Y4, X4];
                    if ((c4 != null) && c4.IsMatchObjectEquals(L) && c4.IsDraggable())
                    {
                        Add(c4);
                        est1 = c0;
                        est2 = c4;
                        return true;
                    }

                    int X5 = T.Column + 1; int Y5 = T.Row - 1;
                    GridCell c5 = grid[Y5, X5];
                    if ((c5 != null) && c5.IsMatchObjectEquals(L) && c5.IsDraggable())
                    {
                        Add(c5);
                        est1 = c0;
                        est2 = c5;
                        return true;
                    }

                    int X6 = T.Column; int Y6 = T.Row - 2;
                    GridCell c6 = grid[Y6, X6];
                    if ((c6 != null) && c6.IsMatchObjectEquals(L) && c6.IsDraggable())
                    {
                        Add(c6);
                        est1 = c0;
                        est2 = c6;
                        return true;
                    }
                }

                // 2 estimate positions for T0L - vertical
                //      X
                //    X T X
                //    7 0 8 
                //    X L X
                //      X
                X0 = L.Column; Y0 = L.Row - 1;
                c0 = grid[Y0, X0];
                if ((c0 != null) && c0.IsDraggable() && ((T.Row - L.Row) == -2))
                {
                    int X7 = X0 - 1; int Y7 = Y0;
                    GridCell c7 = grid[Y7, X7];
                    if ((c7 != null) && c7.IsMatchObjectEquals(L) && c7.IsDraggable())
                    {
                        Add(c7);
                        est1 = c0;
                        est2 = c7;
                        return true;
                    }

                    int X8 = X0 + 1; int Y8 = Y0;
                    GridCell c8 = grid[Y8, X8];
                    if ((c8 != null) && c8.IsMatchObjectEquals(L) && c8.IsDraggable())
                    {
                        Add(c8);
                        est1 = c0;
                        est2 = c8;
                        return true;
                    }
                }
            }
            return false;
        }

        internal void SwapEstimate()
        {
            if (est1 && est2)
            {
                //Debug.Log("swap estimate");
                //est1.Swap(est2.Match);
                SwapHelper.Swap(est1, est2);
            }
        }

        internal MatchGroupType GetGroupType()
        {
            if (Length == 4 && IsHorizonal()) // hor4
            {
                return MatchGroupType.Hor4;
            }
            else if (Length == 4 && IsVertical()) // vert4
            {
                return MatchGroupType.Vert4;
            }
            else if (Length == 5 && IsVertical()) // vert5
            {
                return MatchGroupType.Vert5;
            }
            else if (Length == 5 && IsHorizonal()) // hor5
            {
                return MatchGroupType.Hor5;
            }
            else if (Length == 5 ) // LT
            {
                return MatchGroupType.LT;
            }
            else if (Length == 6) // MiddleLT
            {
                return MatchGroupType.MiddleLT;
            }
            else if (Length == 7) // BigLT
            {
                return MatchGroupType.BigLT;
            }
            return MatchGroupType.Simple;
        }
    }

    public class CellsGroup
    {
        public List<GridCell> Cells { get; private set; }
        public List<GridCell> Bombs { get; private set; }
        public int lastObjectOrderNumber;
        public int lastMatchedID { get; private set; }
        public GridCell lastAddedCell { get; private set; }
        public GridCell lastMatchedCell { get; private set; }

        public int MinYPos
        {
            get; private set;
        }

        public bool Contain(GridCell mCell)
        {
            return Cells.Contains(mCell);
        }

        public int Length
        {
            get { return Cells.Count; }
        }

        public CellsGroup()
        {
            Cells = new List<GridCell>();
            Bombs = new List<GridCell>();
            MinYPos = -1;
        }

        public void Add(GridCell mCell)
        {
            if (!Cells.Contains(mCell))
            {
                Cells.Add(mCell);
                MinYPos = (mCell.Row < MinYPos) ? mCell.Row : MinYPos;
                lastAddedCell = mCell;
                lastMatchedCell = (lastMatchedCell == null || lastMatchedCell.Match == null) ? mCell : lastMatchedCell;

                if (mCell.Match)
                {
                    lastObjectOrderNumber = mCell.Match.ID;
                    lastMatchedCell = (lastMatchedCell.Match.SwapTime < mCell.Match.SwapTime) ? mCell : lastMatchedCell;
                    lastMatchedID = lastMatchedCell.Match.ID;

                    if (mCell.HasBomb)
                    {
                        { Bombs.Add(mCell); }
                    }
                }
            }
        }

        public void AddRange(IEnumerable <GridCell> mCells)
        {
            if (mCells != null)
            {
                foreach (var item in mCells)
                {
                    Add(item);
                }
            }
        }

        public void CancelTween()
        {
            Cells.ForEach((c) => { c.CancelTween(); });
        }

        public override string ToString()
        {
            string s = "";
            Cells.ForEach((c) => { s += c.ToString(); });
            return s;
        }

        public GridCell GetLowermostX()
        {
            if (Cells.Count == 0) return null;
            GridCell l = Cells[0];
            for (int i = 0; i < Cells.Count; i++)
            {
                if (Cells[i].Column < l.Column) l = Cells[i];
            }
            return l;
        }

        public GridCell GetTopmostX()
        {
            if (Cells.Count == 0) return null;
            GridCell t = Cells[0];
            for (int i = 0; i < Cells.Count; i++)
            {
                if (Cells[i].Column > t.Column) t = Cells[i];
            }
            return t;
        }

        public GridCell GetLowermostY()
        {
            if (Cells.Count == 0) return null;
            GridCell l = Cells[0];
            for (int i = 0; i < Cells.Count; i++)
            {
                if (Cells[i].Row > l.Row) l = Cells[i];
            }
            return l;
        }

        public GridCell GetTopmostY()
        {
            if (Cells.Count == 0) return null;
            GridCell t = Cells[0];
            for (int i = 0; i < Cells.Count; i++)
            {
                if (Cells[i].Row < t.Row) t = Cells[i];
            }
            return t;
        }

        public List<GridCell> GetDynamicArea()
        {
            List<GridCell> res = new List<GridCell>(Length);
            for (int i = 0; i < Length; i++)
            {
                if (Cells[i].DynamicObject)
                {
                    res.Add(Cells[i]);
                }
            }
            return res;
        }

        internal bool IsHorizonal()
        {
            if (Cells.Count < 2) return false;
            int row = Cells[0].Row;
            for (int i = 1; i < Cells.Count; i++)
            {
                if (row != Cells[i].Row) return false;
            }
            return true;
        }

        internal bool IsVertical()
        {
            if (Cells.Count < 2) return false;
            int column = Cells[0].Column;
            for (int i = 1; i < Cells.Count; i++)
            {
                if (column != Cells[i].Column) return false;
            }
            return true;
        }

        /// <summary>
        /// Scaling sequenced group
        /// </summary>
        /// <param name="completeCallBack"></param>
        internal void Show(Action completeCallBack)
        {
            ParallelTween showTween = new ParallelTween();
            foreach (GridCell gc in Cells)
            {
                showTween.Add((callBack) =>
                {
                    gc.ZoomMatch(callBack);
                });
            }
            showTween.Start(completeCallBack);
        }

        internal int BombsCount
        {
            get { return Bombs.Count; }
        }

        internal void Remove(GridCell mCell)
        {
            if (mCell == null) return;
            if (Contain(mCell))
            {
                Cells.Remove(mCell);
            }
        }

        internal void Remove(List<GridCell> mCells)
        {
            if (mCells == null) return;
            for (int i = 0; i < mCells.Count; i++)
            {
                if (Contain(mCells[i]))
                {
                    Cells.Remove(mCells[i]);
                }
            }
        }
    }

    public class Row<T> : CellArray<T> where T : GridCell
    {
        public Row(int size) : base(size) { }

        public void CreateWestWind(GameObject prefab, Vector3 scale, Transform parent, Action completeCallBack)
        {
            GameObject s = UnityEngine.Object.Instantiate(prefab, cells[0].transform.position, Quaternion.identity);
            s.transform.localScale = scale;
            s.transform.parent = parent;
          
            Vector3 dPos = new Vector3((cells[0].transform.localPosition - cells[1].transform.localPosition).x * 3.0f,0, 0);
            s.transform.localPosition += dPos;

            Vector3 endPos = cells[cells.Length - 1].transform.position - dPos * scale.x; 
            Whirlwind w = s.GetComponent<Whirlwind>();
            w.Create(endPos, completeCallBack);
        }

        /// <summary>
        /// Get right cells
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public List<T> GetRightCells(int index)
        {
            List<T> cs = new List<T>();
            if (ok(index))
            {
                int i = Length - 1;
                while (i > index)
                {
                    cs.Add(cells[i]);
                    i--;
                }
            }
            return cs;
        }

        /// <summary>
        /// Get right cell
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public T GetRightCell(int index)
        {
            if (ok(index + 1))
            {
                return cells[index + 1];
            }
            return null;
        }

        /// <summary>
        /// Get left cells
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public List<T> GetLeftCells(int index)
        {
            List<T> cs = new List<T>();
            if (ok(index))
            {
                int i = 0;
                while (i < index)
                {
                    cs.Add(cells[i]);
                    i++;
                }
            }
            return cs;
        }

        /// <summary>
        /// Get left cell
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public T GetLeftCell(int index)
        {
            if (ok(index - 1))
            {
                return cells[index - 1];
            }
            return null;
        }

        public T GetLeftDynamic()
        {
            return GetMinDynamic();
        }

        public T GetRightDynamic()
        {
            return GetMaxDynamic();
        }
    }

    public class Column<T> : CellArray<T> where T : GridCell
    {
        public Spawner Spawn { get; private set; }

        public Column(int size) : base(size) { }

        public void CreateTopSpawner(Spawner prefab, SpawnerStyle sStyle, Vector3 scale, Transform parent)
        {
            switch (sStyle)
            {
                case SpawnerStyle.AllEnabled:
                    GridCell gc = GetTopDynamic();
                    if (gc)
                    {
                        gc.CreateSpawner(prefab, new Vector2(0, -(cells[0].transform.position - cells[1].transform.position).y * 1.3f));
                        Spawn = gc.spawner;
                    }
                    break;
                case SpawnerStyle.AllEnabledAlign:
                    GridCell c = GetTopDynamic();
                    if (c)
                    {
                        c.CreateSpawner(prefab, new Vector2(0, -(cells[0].transform.position - cells[1].transform.position).y * 1.3f));
                        Spawn = c.spawner;
                    }

                    break;
                case SpawnerStyle.DisabledAligned:
                    if (!cells[0].Blocked && !cells[0].IsDisabled)
                    {
                        cells[0].CreateSpawner(prefab, new Vector2(0, -(cells[0].transform.position - cells[1].transform.position).y * 1.3f));
                        Spawn = cells[0].spawner;
                    }
                    break;
            }
           
        }

        public void CreateNordWind(GameObject prefab, Vector3 scale, Transform parent, Action completeCallBack)
        {
            GameObject s = UnityEngine.Object.Instantiate(prefab, cells[0].transform.position, Quaternion.identity);
            s.transform.localScale = scale;
            s.transform.parent = parent;
            s.transform.eulerAngles = new Vector3(0, 0, -90);
            Vector3 dPos = new Vector3( 0, (cells[0].transform.localPosition - cells[1].transform.localPosition).y * 3.0f, 0);
            s.transform.localPosition += dPos;

            Vector3 endPos = cells[cells.Length - 1].transform.position - dPos * scale.x;
            Whirlwind w = s.GetComponent<Whirlwind>();
            w.Create(endPos, completeCallBack);
        }

        public T GetTopDynamic()
        {
            return GetMinDynamic();
        }

        public T GetBottomDynamic()
        {
            return GetMaxDynamic();
        }

        /// <summary>
        /// Get cells at top
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public List<T> GetTopCells(int index)
        {
            List<T> cs = new List<T>();
            if (ok(index))
            {
                int i = 0;
                while (i < index)
                {
                    cs.Add(cells[i]);
                    i++;
                }
            }
            return cs;
        }

        /// <summary>
        /// Get cell at top
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public T GetTopCell(int index)
        {
            if (ok(index - 1))
            {
                return cells[index - 1];
            }
            return null;
        }

        /// <summary>
        /// Get bottom cells
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public List<T> GetBottomCells(int index)
        {
            List<T> cs = new List<T>();
            if (ok(index))
            {
                int i = Length-1;
                while (i > index)
                {
                    cs.Add(cells[i]);
                    i--;
                }
            }
            return cs;
        }

        /// <summary>
        /// Get bottom cell
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public T GetBottomCell(int index)
        {
            if (ok(index + 1))
            {
                return cells[index - 1];
            }
            return null;
        }
    }

    public class CellArray<T> : GenInd<T> where T : GridCell
    {
        public CellArray(int size) : base(size) { }

        public static bool AllMatchObjectsIsEqual(GridCell[] mcs)
        {
            if (mcs == null || !mcs[0] || mcs.Length < 2) return false;
            for (int i = 1; i < mcs.Length; i++)
            {
                if (!mcs[i]) return false;
                if (!mcs[0].IsMatchObjectEquals(mcs[i])) return false;
            }
            return true;
        }

        public List<MatchGroup> GetMatches(int minMatches, bool X0X)
        {
            List<MatchGroup> mgList = new List<MatchGroup>();
            MatchGroup mg = new MatchGroup();
            mg.Add(cells[0]);
            for (int i = 1; i < cells.Length; i++)
            {
                int prev = mg.Length - 1;
                if (cells[i].IsMatchable && cells[i].IsMatchObjectEquals(mg.Cells[prev])  && mg.Cells[prev].IsMatchable)
                {
                    mg.Add(cells[i]);
                    if (i == cells.Length - 1 && mg.Length >= minMatches)
                    {
                        mgList.Add(mg);
                        mg = new MatchGroup();
                    }
                }
                else // start new match group
                {
                    if (mg.Length >= minMatches)
                    {
                        mgList.Add(mg);
                    }
                    mg = new MatchGroup();
                    mg.Add(cells[i]);
                }
            }

            if (X0X) // [i-2, i-1, i]
            {
                mg = new MatchGroup();

                for (int i = 2; i < cells.Length; i++)
                {
                    mg.Add(cells[i - 2]);
                    if (cells[i].IsMatchable && cells[i].IsMatchObjectEquals(mg.Cells[0]) && !cells[i - 1].IsMatchObjectEquals(mg.Cells[0])  && mg.Cells[0].IsMatchable && cells[i - 1].IsDraggable())
                    {
                        mg.Add(cells[i]);
                        mgList.Add(mg);
                    }
                    mg = new MatchGroup();
                }
            } // end X0X
            return mgList;
        }

        public List<T> GetDynamicArea()
        {
            List<T> res = new List<T>(Length);
            for (int i = 0; i < Length; i++)
            {
                if (cells[i].DynamicObject)
                {
                   res.Add(cells[i]);
                }
            }
            return res;
        }

        public T GetMinDynamic()
        {
            for (int i = 0; i < Length; i++)
            {
                if (cells[i].DynamicObject || (!cells[i].Blocked && !cells[i].IsDisabled))
                {
                    return cells[i];
                }
            }
            return null;
        }

        public T GetMaxDynamic()
        {
            for (int i = Length - 1; i >= 0; i--)
            {
                if (cells[i].DynamicObject || (!cells[i].Blocked && !cells[i].IsDisabled))
                {
                    return cells[i];
                }
            }
            return null;
        }

        public Vector3 GetDynamicCenter()
        {
            T l = GetMinDynamic();
            T r = GetMaxDynamic();

            if (l && r) return (l.transform.position + r.transform.position) / 2f;
            else if (l) return l.transform.position;
            else if (r) return r.transform.position;
            else return Vector3.zero;
        }

        public override string ToString()
        {
            string s = "";
            for (int i = 0; i < cells.Length; i++)
            {
                s += cells[i].ToString();
            }
            return s;
        }

    }

    public class GenInd<T> where T : class
    {
        public T[] cells;
        public int Length;

        public GenInd(int size)
        {
            cells = new T[size];
            Length = size;
        }

        public T this[int index]
        {
            get { if (ok(index)) { return cells[index]; } else {  return null; } }
            set { if (ok(index)) { cells[index] = value; } else {  } }
        }

        protected bool ok(int index)
        {
            return (index >= 0 && index < Length);
        }

        public T GetMiddleCell()
        {
            int number = Length / 2;

            return cells[number];
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(GameBoard))]
    public class MatchBoardEditor : Editor
    {
        private bool test = true;
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            //EditorGUILayout.Space();
            //EditorGUILayout.Space();
            //#region test
            //if (EditorApplication.isPlaying)
            //{
            //    MatchBoard tg = (MatchBoard)target;
            //    if (MatchBoard.GMode == GameMode.Play)
            //    {
            //        if (test = EditorGUILayout.Foldout(test, "Test"))
            //        {
            //            #region fill
            //            EditorGUILayout.BeginHorizontal("box");
            //            if (GUILayout.Button("Fill(remove matches)"))
            //            {
            //                tg.grid.FillGrid(true);
            //            }
            //            if (GUILayout.Button("Fill"))
            //            {
            //                tg.grid.FillGrid(false);
            //            }
            //            if (GUILayout.Button("Remove matches"))
            //            {
            //                tg.grid.RemoveMatches();
            //            }
            //            EditorGUILayout.EndHorizontal();
            //            #endregion fill

            //            #region mix
            //            EditorGUILayout.BeginHorizontal("box");
            //            if (GUILayout.Button("Mix"))
            //            {
            //                tg.MixGrid(null); 
            //            }

            //            if (GUILayout.Button("Mix"))
            //            {
            //                tg.MixGrid(null);
            //            }
            //            EditorGUILayout.EndHorizontal();
            //            #endregion mix

            //            #region matches
            //            EditorGUILayout.BeginHorizontal("box");
            //            if (GUILayout.Button("Estimate check"))
            //            {
            //                 tg.EstimateGroups.CreateGroups( 2, true);
            //                Debug.Log("Estimate Length:" + tg.EstimateGroups.Length);
            //            }

            //            if (GUILayout.Button("Get free cells"))
            //            {
            //                Debug.Log("Free cells: " + tg.grid?.GetFreeCells().Count);
            //            }
            //            EditorGUILayout.EndHorizontal();
            //            #endregion matches

            //        }

            //        EditorGUILayout.LabelField("Board state: " + tg.MbState);
            //        EditorGUILayout.LabelField("Estimate groups count: " + ((tg.EstimateGroups!=null)? tg.EstimateGroups.Length.ToString(): "none"));
            //        EditorGUILayout.LabelField("Collect groups count: " + ((tg.CollectGroups != null) ? tg.CollectGroups.Length.ToString() : "none"));
            //        EditorGUILayout.LabelField("Free cells count: " + ((tg.grid!= null) ? tg.grid.GetFreeCells().Count.ToString() : "none"));

            //        return;
            //    }
            //}
            //EditorGUILayout.LabelField("Goto play mode for test");
            //#endregion test
        }
    }
#endif
}
