using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

/*
    02.12.2019 - first
    13.12.2019 - add spawner brush, path brush
    18.12.2019 - additional level 
    21.12.2019 - fix asset creating methods (utils)
    24.12.2019 - fix continuos dirty scene (construct panel set active false)
    01.02.2020 - improved buttons creating
    03.02.2021 - update 
*/

namespace Mkey
{
    public class GameConstructor : MonoBehaviour
    {
#if UNITY_EDITOR
        private List<RectTransform> openedPanels;

        [SerializeField]
        private Text editModeText;

        #region selected brush
        [Space(8)]
        //[SerializeField]
        //private Image disabledBrushImage;
        [SerializeField]
        private Image selectedDisabledBrushImage;

        [Space(8)]
        [SerializeField]
        private Image pathBrushImage;
        [SerializeField]
        private Image selectedPathBrushImage;

        [Space(8)]
        [SerializeField]
        private Image spawnBrushImage;
        [SerializeField]
        private Image selectedSpawnBrushImage;
        #endregion selected brush

        [SerializeField]
        private GridObject currentBrush;

        [SerializeField]
        private IncDecInputPanel IncDecPanelPrefab;

        [SerializeField]
        private PanelContainerController brushPanelContainerPrerfab;
        [SerializeField]
        private Transform brushContainersParent;

        #region mission
        [Space(8, order = 0)]
        [Header("Mission", order = 1)]
        [SerializeField]
        private PanelContainerController MissionPanelContainer;
        [SerializeField]
        private IncDecInputPanel InputTextPanelMissionPrefab;
        [SerializeField]
        private IncDecInputPanel IncDecTogglePanelMissionPrefab;
        [SerializeField]
        private IncDecInputPanel TogglePanelMissionPrefab;
        #endregion mission

        #region grid construct
        [Space(8, order = 0)]
        [Header("Grid", order = 1)]
        [SerializeField]
        private PanelContainerController GridPanelContainer;
        [SerializeField]
        private IncDecInputPanel IncDecGridPrefab;
        #endregion grid construct

        #region game construct
        [Space(8, order = 0)]
        [Header("Game construct", order = 0)]
        [SerializeField]
        private Button levelButtonPrefab;
        [SerializeField]
        private Button levelButtonBigPrefab;
        [SerializeField]
        private Button smallButtonPrefab;
        [SerializeField]
        private GameObject constructPanel;
        [SerializeField]
        private Button openConstructButton;
        [SerializeField]
        private ScrollRect LevelButtonsContainer;
        [SerializeField]
        private ScrollRect AddLevelButtonsContainer;
        [SerializeField]
        private Button newAddButton;
        [SerializeField]
        private Button removeAddButton;
        #endregion game construct

        #region temp vars
        private MissionConstruct levelMission;
        private Dictionary<int, TargetData> targets;
        private GameBoard MBoard { get { return GameBoard.Instance; } }
        private MatchGrid MGrid { get { return MBoard.CurrentGrid; } }
        private GameConstructSet GCSet { get { return GameConstructSet.Instance; } }
        private GameObjectsSet GOSet { get { return GCSet.GOSet; } }
        private LevelConstructSet MainLCSet { get { return GCSet.GetLevelConstructSet(GameLevelHolder.CurrentLevel); } } 
        private List<LevelConstructSet> LCSetAdds { get { return GCSet.GetAddLevelConstructSets(GameLevelHolder.CurrentLevel); } }
        #endregion temp vars

        #region properties
        public int ScoreTarget { get { return (MainLCSet) ? MainLCSet.levelMission.ScoreTarget : 0; } }
        #endregion properties

        #region default data
        private string levelConstructSetSubFolder = "LevelConstructSets";  //resource folders
        private string pathToSets = "Assets/ForestMatch/Resources/";
        private int minVertSize = 5;
        private int maxVertSize = 15;
        private int minHorSize = 5;
        private int maxHorSize = 15;
        #endregion default data

        #region events
        public Action<int> ChangeTargetScoreEvent;
        #endregion events

        public void InitStart()
        {
            if (GameBoard.GMode == GameMode.Edit)
            {
                if (!MBoard) return;

                Debug.Log("gc init");
                if (!GCSet)
                {
                    Debug.Log("Game construct set not found!!!");
                    return;
                }
                if (!GOSet)
                {
                    Debug.Log("GameObjectSet not found!!! - ");
                    return;
                }

                currentBrush = null;

                // create brush panels
                CreateBrushContainer(brushContainersParent, brushPanelContainerPrerfab, "Blocked brush panel", new List<GridObject>(GOSet.BlockedObjects));
                CreateBrushContainer(brushContainersParent, brushPanelContainerPrerfab, "Falling brush panel", new List<GridObject>(GOSet.FallingObjects));
              //  CreateBrushContainer(brushContainersParent, brushPanelContainerPrerfab, "Main brush panel", new List<GridObject>(GOSet.MatchObjects));
                CreateBrushContainer(brushContainersParent, brushPanelContainerPrerfab, "Overlay brush panel", new List<GridObject>(GOSet.OverlayObjects));
                CreateBrushContainer(brushContainersParent, brushPanelContainerPrerfab, "Underlay brush panel", new List<GridObject>(GOSet.UnderlayObjects));
                CreateBrushContainer(brushContainersParent, brushPanelContainerPrerfab, "Hidden brush panel", new List<GridObject>(GOSet.HiddenObjects));
                CreateBrushContainer(brushContainersParent, brushPanelContainerPrerfab, "Treasure brush panel", new List<GridObject>(GOSet.TreasureObjects));

                if (editModeText) editModeText.text = "EDIT MODE" + '\n' + "Level " + (GameLevelHolder.CurrentLevel + 1);
                ShowLevelData(false);

                DeselectAllBrushes();
                CreateLevelButtons();
                ShowConstructMenu(true);
            }
        }

        #region show board
        private void ShowLevelData()
        {
            ShowLevelData(true);
        }

        private void ShowLevelData(bool rebuild)
        {
            GCSet.Clean();
            MainLCSet.Clean(GOSet);
            if (LCSetAdds != null)
            {
                foreach (var item in LCSetAdds)
                {
                    if (item) item.Clean(GOSet);
                }
            }
            Debug.Log("Show level data: " + (GameLevelHolder.CurrentLevel));
            if (rebuild) MBoard.CreateGameBoard();

            levelMission = MBoard.FullLevelMission;
            targets = MBoard.Targets;
            foreach (var item in targets)
            {
                item.Value.SetCurrCount(0);
                int iCount = levelMission.Targets.CountByID(item.Key);
                if (iCount > 0)
                    item.Value.SetNeedCount(levelMission.Targets.CountByID(item.Key));
                else
                    item.Value.SetNeedCount(0);
            }

            LevelButtonsRefresh();
            if (editModeText) editModeText.text = "EDIT MODE" + '\n' + "Level " + (GameLevelHolder.CurrentLevel + 1);

            ChangeTargetScoreEvent?.Invoke(levelMission.ScoreTarget);

            if (HeaderGUIController.Instance)
            {
                HeaderGUIController.Instance.RefreshTimeMoves();
                HeaderGUIController.Instance.RefreshLevel();
            }
          
            AddLevelButtonsBeh();
            ShowFillPath(MBoard.CurrentGrid);
            CreateAddLevelButtons();
            AddLevelButtonsRefresh();
        }

        private void ShowFillPath(MatchGrid mGrid)
        {
            LevelConstructSet lCSet = mGrid.LcSet;

            foreach (var item in mGrid.Cells)
            {
                if (item.spawner)
                {
                    GameObject old = item.spawner.gameObject;
                    item.spawner = null;
                    DestroyImmediate(old);
                }
            }

            if (lCSet.spawnCells != null)
            {
                int i = 0;
                foreach (var item in lCSet.spawnCells)
                {
                    GridCell gC = mGrid.Rows[item.Row].cells[item.Column];
                    if (lCSet.spawnOffsets != null && lCSet.spawnOffsets.Count == lCSet.spawnCells.Count)
                    {
                        gC.CreateSpawner(MBoard.spawnerPrefab, lCSet.spawnOffsets[i]);
                    }
                    else
                    {
                        gC.CreateSpawner(MBoard.spawnerPrefab, Vector2.zero);
                    }
                    i++;
                    gC.spawner.Show(true);
                }
            }
            SaveSpawnOffsets();

            Debug.Log("show fill path");
            if (selectedPathBrushImage.enabled)
            {
                foreach (var item in mGrid.Cells)
                {
                    BoxCollider2D bc = item.GetComponent<BoxCollider2D>();
                    if (bc) bc.enabled = false;
                    int row = item.Row;
                    int column = item.Column;
                    if (item.IsDisabled || item.spawner)
                    {
                        lCSet.SetMatherDir(mGrid, row, column, DirMather.None);
                        continue;
                    }

                    MatherButton[] mBs = item.GetComponentsInChildren<MatherButton>(true);
                    foreach (var mB in mBs)
                    {
                        DirMather dir = DirMather.None;
                        mB.gameObject.SetActive(false);
                        GridCell neigh = null;

                        switch (mB.dirMather)
                        {
                            case DirMather.Top:
                                neigh = item.Neighbors.Top;
                                dir = DirMather.Top;
                                break;
                            case DirMather.Right:
                                neigh = item.Neighbors.Right;
                                dir = DirMather.Right;
                                break;
                            case DirMather.Bottom:
                                neigh = item.Neighbors.Bottom;
                                dir = DirMather.Bottom;
                                break;
                            case DirMather.Left:
                                neigh = item.Neighbors.Left;
                                dir = DirMather.Left;
                                break;
                        }

                        if (neigh) // && !neigh.Blocked && !neigh.IsDisabled && !neigh.MovementBlocked)
                        {
                            mB.gameObject.SetActive(true);
                            DirMather sDir = lCSet.GetMatherDir(mGrid, row, column);
                            DirMather dM = mB.dirMather;
                            mB.SetActive((sDir == DirMather.Top && dM == DirMather.Top) || (sDir == DirMather.Left && dM == DirMather.Left) || (sDir == DirMather.Bottom && dM == DirMather.Bottom) || (sDir == DirMather.Right && dM == DirMather.Right));
                            mB.clickEventAction = (m) =>
                            {
                                Debug.Log(row + " : " + column + " : " + (!m.IsActive ? DirMather.None : dir));
                                lCSet.SetMatherDir(mGrid, row, column, !m.IsActive ? DirMather.None : dir);
                            };
                        }
                    }
                }
            }
            else
            {
                foreach (var item in mGrid.Cells)
                {

                    BoxCollider2D bc = item.GetComponent<BoxCollider2D>();
                    if (bc) bc.enabled = true;

                    MatherButton [] mBs =  item.GetComponentsInChildren<MatherButton>(true);
                    foreach (var mB in mBs)
                    {
                        mB.gameObject.SetActive(false);
                    }
                }
            }
        }

        public void SaveSpawnOffsets()
        {
            MainLCSet.SaveSpawnOfsets(MGrid);
        } 
        #endregion show board

        #region construct menus +
        bool openedConstr = false;

        public void OpenConstructPanel()
        {
            SetConstructControlActivity(false);
            constructPanel.SetActive(true);

            RectTransform rt = constructPanel.GetComponent<RectTransform>();//Debug.Log(rt.offsetMin + " : " + rt.offsetMax);
            float startX = (!openedConstr) ? 0 : 1f;
            float endX = (!openedConstr) ? 1f : 0;

            SimpleTween.Value(constructPanel, startX, endX, 0.2f).SetEase(EaseAnim.EaseInCubic).
                               SetOnUpdate((float val) =>
                               {
                                   rt.transform.localScale = new Vector3(val, 1, 1);
                                   // rt.offsetMax = new Vector2(val, rt.offsetMax.y);
                               }).AddCompleteCallBack(() =>
                               {
                                   SetConstructControlActivity(true);
                                   openedConstr = !openedConstr;
                                   LevelButtonsRefresh();
                               });


        }

        private void SetConstructControlActivity(bool activity)
        {
            Button[] buttons = constructPanel.GetComponentsInChildren<Button>();
            for (int i = 0; i < buttons.Length; i++)
            {
                buttons[i].interactable = activity;
            }
        }

        private void ShowConstructMenu(bool show)
        {
            constructPanel.SetActive(show);
            openConstructButton.gameObject.SetActive(show);
        }

        public void CreateLevelButtons()
        {
            GCSet.Clean();

            Transform parent = LevelButtonsContainer.content.transform;
            DestroyGOInChildrenWithComponent<Button>(parent);

            for (int i = 0; i < GCSet.levelSets.Count; i++)
            {
                int level = i + 1;
                Button button = CreateButton(levelButtonPrefab, parent, null, "" + level.ToString(), () =>
                {
                    GameLevelHolder.CurrentLevel = level - 1;
                    MBoard.ShowGrid(MBoard.MainGrid, 0, null);
                    CloseOpenedPanels();
                    ShowLevelData();
                });
            }
        }

        public void RemoveLevel()
        {
            Debug.Log("Click on Button <Remove level...> ");
            if (GCSet.LevelCount < 2)
            {
                Debug.Log("Can't remove the last level> ");
                return;
            }
            GCSet.RemoveLevel(GameLevelHolder.CurrentLevel);
            CreateLevelButtons();
            GameLevelHolder.CurrentLevel = (GameLevelHolder.CurrentLevel <= GCSet.LevelCount - 1) ? GameLevelHolder.CurrentLevel : GameLevelHolder.CurrentLevel - 1;
            ShowLevelData();
        }

        public void InsertBefore()
        {
            Debug.Log("Click on Button <Insert level before...> ");
            LevelConstructSet lcs = ScriptableObjectUtility.CreateResourceAsset<LevelConstructSet>(pathToSets, levelConstructSetSubFolder, "", " " + 1.ToString());
            GCSet.InsertBeforeLevel(GameLevelHolder.CurrentLevel, lcs);
            CreateLevelButtons();
            ShowLevelData();
        }

        public void InsertAfter()
        {
            Debug.Log("Click on Button <Insert level after...> ");
            LevelConstructSet lcs = ScriptableObjectUtility.CreateResourceAsset<LevelConstructSet>(pathToSets, levelConstructSetSubFolder, "", " " + 1.ToString());
            GCSet.InsertAfterLevel(GameLevelHolder.CurrentLevel, lcs);
            CreateLevelButtons();
            GameLevelHolder.CurrentLevel += 1;
            ShowLevelData();
        }

        private void LevelButtonsRefresh()
        {
            Button[] levelButtons = LevelButtonsContainer.content.gameObject.GetComponentsInChildren<Button>();
            for (int i = 0; i < levelButtons.Length; i++)
            {
                SelectButton(levelButtons[i], (i == GameLevelHolder.CurrentLevel));
            }
        }

        private void SelectButton(Button b, bool select)
        {
            b.GetComponent<Image>().color = (select) ? new Color(0.5f, 0.5f, 0.5f, 1) : new Color(1, 1, 1, 1);
        }

        private void AddLevelButtonsBeh()
        {
            removeAddButton.gameObject.SetActive(!MBoard.IsMainGridActive);
        }

        public void CreateAddLevel()
        {
            Debug.Log("Click on Button <Create additional level for...> " + MainLCSet.name);
            LevelConstructSet lcs = ScriptableObjectUtility.CreateResourceAsset<LevelConstructSet>(levelConstructSetSubFolder, "", MainLCSet.name, "_add");
            Debug.Log("new asset created: " + lcs);
            GCSet.AddAdditionalLevel(lcs);
            ShowLevelData();
        }

        public void RemoveAddLevel()
        {
            Debug.Log("Click on Button <Remove additional level...> ");
            GCSet.RemoveAddLevel(MGrid.LcSet);
            ShowLevelData();
        }

        public void CreateAddLevelButtons()
        {
            Transform parent = AddLevelButtonsContainer.content;
            DestroyGOInChildrenWithComponent<Button>(parent);

            List<MatchGrid> aL = new List<MatchGrid>( MBoard.AdditionalGrids.Values);
            for (int i = 0; i < aL.Count; i++)
            {
                int level = i + 1;
                Button button = CreateButton(levelButtonBigPrefab, parent, null, "AddLevel " + level.ToString(), ()=>
                {
                    MBoard.ShowGrid(aL[level - 1], 0 , null);
                    CloseOpenedPanels();
                    ShowLevelData();
                });
            }
        }

        private void AddLevelButtonsRefresh()
        {
            Button[] levelButtons = AddLevelButtonsContainer.content.GetComponentsInChildren<Button>();
            for (int i = 0; i < levelButtons.Length; i++)
            {
                SelectButton(levelButtons[i], (i == MBoard.AddGridIndex));
            }
        }
        #endregion construct menus

        #region grid settings
        private void ShowLevelSettingsMenu(bool show)
        {
            constructPanel.SetActive(show);
            openConstructButton.gameObject.SetActive(show);
        }

        public void OpenSettingsPanel_Click()
        {
            Debug.Log("open grid settings click");

            ScrollPanelController sRC = GridPanelContainer.ScrollPanel;
            if (sRC) // 
            {
                if (sRC) sRC.CloseScrollPanel(true, null);
            }
            else
            {
                CloseOpenedPanels();
                //instantiate ScrollRectController
                sRC = GridPanelContainer.InstantiateScrollPanel();
                sRC.textCaption.text = "Grid panel";

                //create  vert size block
                IncDecInputPanel.Create(sRC.scrollContent, IncDecGridPrefab, "VertSize", MGrid.LcSet.VertSize.ToString(),
                    () => { IncVertSize(); },
                    () => { DecVertSize(); },
                    (val) => { },
                    () => { return MGrid.LcSet.VertSize.ToString(); },
                    null);

                //create hor size block
                IncDecInputPanel.Create(sRC.scrollContent, IncDecGridPrefab, "HorSize", MGrid.LcSet.HorSize.ToString(),
                    () => { IncHorSize(); },
                    () => { DecHorSize(); },
                    (val) => { },
                    () => { return MGrid.LcSet.HorSize.ToString(); },
                    null);

                //create background block
                if(MBoard.IsMainGridActive)
                IncDecInputPanel.Create(sRC.scrollContent, IncDecGridPrefab, "BackGrounds", MGrid.LcSet.BackGround.ToString(),
                    () => { IncBackGround(); },
                    () => { DecBackGround(); },
                    (val) => { },
                    () => { return MGrid.LcSet.BackGround.ToString(); },
                    null);

                //create dist X block
                IncDecInputPanel.Create(sRC.scrollContent, IncDecGridPrefab, "Dist X", MGrid.LcSet.DistX.ToString(),
                    () => { IncDistX(); },
                    () => { DecDistX(); },
                    (val) => { },
                    () => { return MGrid.LcSet.DistX.ToString(); },
                    null);

                //create dist Y block
                IncDecInputPanel.Create(sRC.scrollContent, IncDecGridPrefab, "Dist Y", MGrid.LcSet.DistY.ToString(),
                    () => { IncDistY(); },
                    () => { DecDistY(); },
                    (val) => { },
                    () => { return MGrid.LcSet.DistY.ToString(); },
                    null);

                //create scale block
                IncDecInputPanel.Create(sRC.scrollContent, IncDecGridPrefab, "Scale", MGrid.LcSet.Scale.ToString(),
                    () => { IncScale(); },
                    () => { DecScale(); },
                    (val) => { },
                    () => { return MGrid.LcSet.Scale.ToString(); },
                    null);

                sRC.OpenScrollPanel(null);
            }
        }

        public void IncVertSize()
        {
            Debug.Log("Click on Button <VerticalSize...> ");
            int vertSize = MBoard.CurrentGrid.LcSet.VertSize;
            vertSize = (vertSize < maxVertSize) ? ++vertSize : maxVertSize;
            MBoard.CurrentGrid.LcSet.VertSize = vertSize;
            ShowLevelData();
        }

        public void DecVertSize()
        {
            Debug.Log("Click on Button <VerticalSize...> ");
            int vertSize = MGrid.LcSet.VertSize;
            vertSize = (vertSize > minVertSize) ? --vertSize : minVertSize;
            MGrid.LcSet.VertSize = vertSize;
            ShowLevelData();
        }

        public void IncHorSize()
        {
            Debug.Log("Click on Button <HorizontalSize...> ");
            int horSize = MGrid.LcSet.HorSize;
            horSize = (horSize < maxHorSize) ? ++horSize : maxHorSize;
            MGrid.LcSet.HorSize = horSize;
            ShowLevelData();
        }

        public void DecHorSize()
        {
            Debug.Log("Click on Button <HorizontalSize...> ");
            int horSize = MGrid.LcSet.HorSize;
            horSize = (horSize > minHorSize) ? --horSize : minHorSize;
            MGrid.LcSet.HorSize = horSize;
            ShowLevelData();
        }

        public void IncDistX()
        {
            Debug.Log("Click on Button <DistanceX...> ");
            int dist = Mathf.RoundToInt(MGrid.LcSet.DistX * 100f);
            dist += 5;
            MGrid.LcSet.DistX = (dist > 100) ? 1f : dist / 100f;
            ShowLevelData();
        }

        public void DecDistX()
        {
            Debug.Log("Click on Button <DistanceX...> ");
            int dist = Mathf.RoundToInt(MGrid.LcSet.DistX * 100f);
            dist -= 5;
            MGrid.LcSet.DistX = (dist > 0f) ? dist / 100f : 0f;
            ShowLevelData();
        }

        public void IncDistY()
        {
            Debug.Log("Click on Button <DistanceY...> ");
            int dist = Mathf.RoundToInt(MGrid.LcSet.DistY * 100f);
            dist += 5;
            MGrid.LcSet.DistY = (dist > 100) ? 1f : dist / 100f;
            ShowLevelData();
        }

        public void DecDistY()
        {
            Debug.Log("Click on Button <DistanceY...> ");
            int dist = Mathf.RoundToInt(MGrid.LcSet.DistY * 100f);
            dist -= 5;
            MGrid.LcSet.DistY = (dist > 0f) ? dist / 100f : 0f;
            ShowLevelData();
        }

        public void IncScale()
        {
            Debug.Log("Click on Button <Scale...> ");
            int scale = Mathf.RoundToInt(MGrid.LcSet.Scale * 100f);
            scale += 5;
            MGrid.LcSet.Scale = scale / 100f;
            ShowLevelData();
        }

        public void DecScale()
        {
            Debug.Log("Click on Button <Scale...> ");
            int scale = Mathf.RoundToInt(MGrid.LcSet.Scale * 100f);
            scale -= 5;
            MGrid.LcSet.Scale = (scale > 0f) ? scale / 100f : 0f;
            ShowLevelData();
        }

        public void IncBackGround()
        {
            Debug.Log("Click on Button <BackGround...> ");
            MGrid.LcSet.IncBackGround(GOSet.BackGroundsCount);
            ShowLevelData();
        }

        public void DecBackGround()
        {
            Debug.Log("Click on Button <BackGround...> ");
            MGrid.LcSet.DecBackGround(GOSet.BackGroundsCount);
            ShowLevelData();
        }
        #endregion grid settings

        #region grid brushes
        public void Cell_Click(GridCell cell)
        {
            Debug.Log("Click on cell <" + cell.ToString() + "...> ");
            LevelConstructSet lCSet = MGrid.LcSet;

            if (selectedPathBrushImage.enabled)
            {
                Debug.Log("path brush enabled");
                ShowLevelData();

            }
            else if (selectedSpawnBrushImage.enabled)
            {
                Debug.Log("spawn brush enabled");
                lCSet.AddSpawnCell(new CellData(-100, cell.Row, cell.Column));
                ShowLevelData();

            }
            else if (currentBrush)
            {
                if (cell.HaveObjectWithID(currentBrush.ID))
                {
                    cell.RemoveObject(currentBrush.ID);
                }
                else
                {
                    cell.SetObject(currentBrush.ID, currentBrush.Hits);
                }

                lCSet.SaveObjects(cell);
            }

            CloseOpenedPanels();
        }

        private void CloseOpenedPanels()
        {
            ScrollPanelController[] sRCs = GetComponentsInChildren<ScrollPanelController>();
            foreach (var item in sRCs)
            {
                item.CloseScrollPanel(true, null);
            }
        }

        private void SetSpriteControlActivity(RectTransform panel, bool activity)
        {
            Button[] buttons = panel.GetComponentsInChildren<Button>();
            for (int i = 0; i < buttons.Length; i++)
            {
                buttons[i].interactable = activity;
            }
        }

        public void SelectDisabledBrush()
        {
            DeselectAllBrushes();
            selectedDisabledBrushImage.enabled = true;
            currentBrush = GOSet.Disabled;
        }


        public void SelectPathBrush()
        {
            DeselectAllBrushes();
            selectedPathBrushImage.enabled = true;
            ShowFillPath(MBoard.CurrentGrid);
        }

        public void SelectSpawnBrush()
        {
            DeselectAllBrushes();
            selectedSpawnBrushImage.enabled = true;
            ShowFillPath(MBoard.CurrentGrid);
        }

        private void DeselectAllBrushes()
        {
            currentBrush = null;
            PanelContainerController[] panelContainerControllers = brushContainersParent.GetComponentsInChildren<PanelContainerController>();

            foreach (var item in panelContainerControllers)
            {
              if(item)  item.selector.enabled = false;
            }

            selectedPathBrushImage.enabled = false;
            selectedSpawnBrushImage.enabled = false;
            ShowFillPath(MBoard.CurrentGrid);
        }
        #endregion grid brushes

        #region mission
        public void OpenMissionPanel_Click()
        {
            Debug.Log("open mission click");

            MissionConstruct currMiss = MGrid.LcSet.levelMission;

            ScrollPanelController sRC = MissionPanelContainer.ScrollPanel;
            if (sRC) // 
            {
                sRC.CloseScrollPanel(true, null);
            }
            else
            {
                CloseOpenedPanels();
                //instantiate ScrollRectController
                sRC = MissionPanelContainer.InstantiateScrollPanel();
                sRC.textCaption.text = MBoard.IsMainGridActive ?  "Mission panel" : "Mission panel - additional targets";


                IncDecInputPanel movesPanel = null;

                //create time constrain
                if (MBoard.IsMainGridActive)
                {
                    IncDecInputPanel.Create(sRC.scrollContent, IncDecPanelPrefab, "Time", currMiss.TimeConstrain.ToString(),
                    () => { currMiss.AddTime(1); HeaderGUIController.Instance.RefreshTimeMoves(); },
                    () => { currMiss.AddTime(-1); HeaderGUIController.Instance.RefreshTimeMoves(); },
                    (val) => { int res; bool good = int.TryParse(val, out res); if (good) { currMiss.SetTime(res); HeaderGUIController.Instance.RefreshTimeMoves(); } },
                    () => { movesPanel?.gameObject.SetActive(!currMiss.IsTimeLevel); return currMiss.TimeConstrain.ToString(); },
                    null);
                }
                //create mission moves constrain
                if (MBoard.IsMainGridActive)
                {
                    movesPanel = IncDecInputPanel.Create(sRC.scrollContent, IncDecPanelPrefab, "Moves", currMiss.MovesConstrain.ToString(),
                    () => { currMiss.AddMoves(1); HeaderGUIController.Instance.RefreshTimeMoves(); },
                    () => { currMiss.AddMoves(-1); HeaderGUIController.Instance.RefreshTimeMoves(); },
                    (val) => { int res; bool good = int.TryParse(val, out res); if (good) { currMiss.SetMovesCount(res); HeaderGUIController.Instance.RefreshTimeMoves(); } },
                    () => { return currMiss.MovesConstrain.ToString(); },
                    null);
                    movesPanel.gameObject.SetActive(!currMiss.IsTimeLevel);
                }

                //description input field
                if (MBoard.IsMainGridActive)
                {
                    IncDecInputPanel.Create(sRC.scrollContent, InputTextPanelMissionPrefab, "Description", currMiss.Description,
                null,
                null,
                (val) => { currMiss.SetDescription(val); },
                () => { return currMiss.Description; },
                null);
                }

                //create score target
                if (MBoard.IsMainGridActive)
                {
                    IncDecInputPanel.Create(sRC.scrollContent, IncDecPanelPrefab, "Score", currMiss.ScoreTarget.ToString(),
                () => { currMiss.AddScoreTarget(1); ChangeTargetScoreEvent?.Invoke(currMiss.ScoreTarget); },
                () => { currMiss.AddScoreTarget(-1); ChangeTargetScoreEvent?.Invoke(currMiss.ScoreTarget); },
                (val) => { int res; bool good = int.TryParse(val, out res); if (good) { currMiss.SetScoreTargetCount(res); ChangeTargetScoreEvent?.Invoke(currMiss.ScoreTarget); } },
                () => { return currMiss.ScoreTarget.ToString(); },
                null);
                }

                //create object targets
                foreach (var item in targets)
                {
                    int id = item.Key;
                    IncDecInputPanel.Create(sRC.scrollContent, IncDecPanelPrefab, "Target", currMiss.GetTargetCount(id).ToString(),
                    false,
                    () => { currMiss.AddTarget(id, 1); item.Value?.IncNeedCount(1); },
                    () => { currMiss.RemoveTarget(id, 1); item.Value?.IncNeedCount(-1); },
                    (val) => { int res; bool good = int.TryParse(val, out res); if (good) { currMiss.SetTargetCount(id, res); item.Value?.SetNeedCount(res); } },
                    null,
                    () => { return currMiss.GetTargetCount(id).ToString(); }, // grid.GetObjectsCountByID(id).ToString()); },
                    item.Value.GetImage(GOSet));
                }

                sRC.OpenScrollPanel(null);
            }
        }
        #endregion mission

        #region load assets
        T[] LoadResourceAssets<T>(string subFolder) where T : BaseScriptable
        {
            T[] t = Resources.LoadAll<T>(subFolder);
            if (t != null && t.Length > 0)
            {
                string s = "";
                foreach (var m in t)
                {
                    s += m.ToString() + "; ";
                }
                Debug.Log("Scriptable assets loaded," + typeof(T).ToString() + ", count: " + t.Length + "; sets : " + s);
            }
            else
            {
                Debug.Log("Scriptable assets " + typeof(T).ToString() + " not found!!!");
            }
            return t;
        }
        #endregion load assets

        #region utils
        private void DestroyGOInChildrenWithComponent<T>(Transform parent) where T : Component
        {
            if (!parent) return;
            T[] existComp = parent.GetComponentsInChildren<T>();
            for (int i = 0; i < existComp.Length; i++)
            {
                if (parent.gameObject != existComp[i].gameObject) DestroyImmediate(existComp[i].gameObject);
            }
        }

        private void CreateBrushContainer(Transform parent, PanelContainerController containerPrefab, string capital, List<GridObject> gridObjects)
        {
            if(gridObjects==null || gridObjects.Count == 0)
            {
                Debug.Log("Can't create: " + capital);
                return;
            } 
            PanelContainerController c =  Instantiate(containerPrefab, parent);
            c.capital = capital;
            c.gridObjects = gridObjects;
            c.OpenCloseButton.onClick.RemoveAllListeners();
            c.OpenCloseButton.onClick.AddListener(()=> { CreateBrushPanel(c); });
            c.BrushSelectButton.onClick.RemoveAllListeners();
            c.BrushSelectButton.onClick.AddListener(()=> 
            {
                GridObject gO = c.GetOrAddComponent<GridObject>();
                DeselectAllBrushes();
                currentBrush = GOSet.GetObject(gO.ID); // (gO.ID > 0) ? GOSet.GetObject(gO.ID) : GOSet.Disabled;
                currentBrush.Hits = gO.Hits;
                c.selector.enabled = true;
                //Debug.Log("current brush: " + currentBrush.ID + " ;hits: " + currentBrush.Hits);
            });
            c.brushImage.sprite = gridObjects[0].ObjectImage;
            c.GetOrAddComponent<GridObject>().Enumerate(gridObjects[0].ID);
            if(!string.IsNullOrEmpty(capital)) c.BrushName.text = capital[0].ToString();
        }

        private void CreateBrushPanel(PanelContainerController container)
        {
            ScrollPanelController sRC = container.ScrollPanel;
            if (sRC)
            {
                sRC.CloseScrollPanel(true, null);
            }
            else
            {
                CloseOpenedPanels();

                sRC = container.InstantiateScrollPanel();
                sRC.textCaption.text = container.capital;

                List<GridObject> mData = new List<GridObject>();
                if (container.gridObjects != null) mData.AddRange(container.gridObjects);
                CreateBrushButtons(mData, smallButtonPrefab, container, sRC.scrollContent, container.brushImage, container.selector);
                sRC.OpenScrollPanel(null);
            }
        }

        private void CreateBrushButtons(List<GridObject> mData, Button prefab, PanelContainerController container, Transform parent, Image objectImage, Image selector)
        {
            //create brushes
            if (mData == null || mData.Count == 0) return;

            for (int i = 0; i < mData.Count; i++)
            {
                GridObject mD = mData[i];
                Sprite[] protectionStateImages = mD.GetProtectionStateImages();

                CreateButton(smallButtonPrefab, parent, mD.ObjectImage, () =>
                {
                    Debug.Log("Click on Button <" + mD.ID + "...> ");
                    DeselectAllBrushes();
                    currentBrush = GOSet.GetObject(mD.ID);
                    objectImage.sprite = currentBrush.ObjectImage;
                    GridObject cGO = container.GetOrAddComponent<GridObject>();
                    cGO.Enumerate(currentBrush.ID);
                    cGO.Hits = 0;
                    currentBrush.Hits = 0;
                    selector.enabled = true;
                });

                if (protectionStateImages != null)
                {
                    int hits = 0;
                    foreach (var item in protectionStateImages)
                    {
                        hits += 1;
                        var tHits = hits;
                        CreateButton(smallButtonPrefab, parent, item, () =>
                        {
                            Debug.Log("Click on Button <" + mD.ID +" ;hits: "+ tHits +  "...> " );
                            DeselectAllBrushes();
                            currentBrush = GOSet.GetObject(mD.ID);
                            objectImage.sprite = item;
                            GridObject cGO = container.GetOrAddComponent<GridObject>();
                            cGO.Enumerate(currentBrush.ID);
                            cGO.Hits = tHits;
                            currentBrush.Hits = tHits;
                            selector.enabled = true;
                        });
                    }
                }
            }
        }

        private Button CreateButton(Button prefab, Transform parent, Sprite sprite, System.Action listener)
        {
            Button button = Instantiate(prefab, Vector3.zero, Quaternion.identity);
            button.transform.SetParent(parent);
            button.transform.localScale = Vector3.one;
            button.onClick.RemoveAllListeners();
            if(sprite) button.GetComponent<Image>().sprite = sprite;
            if (listener != null) button.onClick.AddListener(() =>
            {
                listener();
            });

            return button;
        }

        private Button CreateButton(Button prefab, Transform parent, Sprite sprite, string text, System.Action listener)
        {
            Button button = CreateButton(prefab, parent, sprite, listener);
            Text t = button.GetComponentInChildren<Text>();
            if (t && text != null) t.text = text;
            return button;
        }

        private void SelectButton(Button b)
        {
            Text t = b.GetComponentInChildren<Text>();
            if (!t) return;
            t.enabled = true;
            t.gameObject.SetActive(true);
            t.text = "selected";
            t.color = Color.black;
        }

        private void DeselectButton(Button b)
        {
            Text t = b.GetComponentInChildren<Text>();
            if (!t) return;
            t.enabled = true;
            t.gameObject.SetActive(true);
            t.text = "";
        }
        #endregion utils

        public void Scan()
        {
            MGrid.LcSet.Scan(MGrid);
        }
#endif
    }
}
