using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mkey
{
    public class GridCell : TouchPadMessageTarget
    {
        #region debug
        private bool debug = false;
        #endregion debug

        #region stroke
        [SerializeField]
        private Transform LeftTopCorner;
        [SerializeField]
        private Transform RightTopCorner;
        [SerializeField]
        private Transform LeftBotCorner;
        [SerializeField]
        private Transform RightBotCorner;
        [SerializeField]
        private Sprite Left;
        [SerializeField]
        private Sprite Right;
        [SerializeField]
        private Sprite Top;
        [SerializeField]
        private Sprite Bottom;
        [SerializeField]
        private Sprite OutTopLeft;
        [SerializeField]
        private Sprite OutBotLeft;
        [SerializeField]
        private Sprite OutTopRight;
        [SerializeField]
        private Sprite OutBotRight;

        [SerializeField]
        private Sprite InTopLeft;
        [SerializeField]
        private Sprite InBotLeft;
        [SerializeField]
        private Sprite InTopRight;
        [SerializeField]
        private Sprite InBotRight;

        #endregion stroke

        #region row column
        public Column<GridCell> GColumn { get; private set; }

      

        public Row<GridCell> GRow { get; private set; }
        public int Row { get; private set; }
        public int Column { get; private set; }
        public List<Row<GridCell>> Rows { get; private set; }
        #endregion row column

        #region objects
        public GameObject DynamicObject
        {
            get
            {
                if (Match) return Match.gameObject;
                if (DynamicClickBomb) return DynamicClickBomb.gameObject;
                if (Falling) return Falling.gameObject;
                if (Treasure) return Treasure.gameObject;
                return null;
            }
        }
        public MatchObject Match { get { return   GetComponentInChildren<MatchObject>(); } }
        public FallingObject Falling { get { return GetComponentInChildren<FallingObject>(); } }
        public DynamicBlockerObject DynamicBlocker { get { return GetComponentInChildren<DynamicBlockerObject>(); } }
        public OverlayObject Overlay { get { return GetComponentInChildren<OverlayObject>(); } }
        public BlockedObject Blocked { get { return GetComponentInChildren<BlockedObject>(); } }
        public UnderlayObject Underlay { get { return GetComponentInChildren<UnderlayObject>(); } }
        public HiddenObject Hidden { get { return GetComponentInChildren<HiddenObject>(); } }
        public DisabledObject Disabled { get { return GetComponentInChildren<DisabledObject>(); } }
        public TreasureObject Treasure{ get { return GetComponentInChildren<TreasureObject>(); } }
        public StaticMatchBombObject StaticMatchBomb { get { return GetComponentInChildren<StaticMatchBombObject>(); } }
        public DynamicMatchBombObject DynamicMatchBomb { get { if (Match) return Match.GetComponentInChildren<DynamicMatchBombObject>(); return null; } }
        public DynamicClickBombObject DynamicClickBomb { get { return GetComponentInChildren<DynamicClickBombObject>(); } }
        #endregion objects

        #region cache fields
        private BoxCollider2D coll2D;
        private SpriteRenderer sRenderer;
        #endregion cache fields

        #region events
        public Action<GridCell> GCPointerDownEvent;
        public Action<GridCell> GCDoubleClickEvent;
        public Action<GridCell> GCDragEnterEvent;
        #endregion events

        #region properties 
        public bool MovementBlocked
        {
            get { return Overlay && Overlay.BlockMovement; }
        }

        /// <summary>
        /// Return true if mainobject and mainobject IsMatchedById || IsMatchedWithAny
        /// </summary>
        /// <returns></returns>
        public bool IsMatchable
        {
            get
            {
                if (!Overlay) return Match;
                return (Match && !Overlay.BlockMatch);
            }
        }

        /// <summary>
        /// Return true if gcell protected with Overlay && Overlay.BlockMatch
        /// </summary>
        public bool MatchProtected
        {
            get
            {
                if (Overlay && Overlay.BlockMatch && Overlay.Protection > 0) return true;
                return false;
            }
        }

        public bool IsMixable
        {
            get
            {
                if((Match || DynamicClickBomb) && !MovementBlocked) return true ;
                return false;
            }
        }

        public bool IsTopCell { get { return Row == 0; } }

        /// <summary>
        /// Return true if gridcell has no dynamic object
        /// </summary>
        public bool IsDynamicFree
        {
            get { return !DynamicObject; }
        }

        public bool IsDisabled
        {
            get { return Disabled; }
        }

        /// <summary>
        /// Return true if mainobject protected with overlay or underlay or self protected
        /// </summary>
        public bool Protected
        {
            get
            {
                if (Overlay && Overlay.Protection > 0) return true;
                if (Underlay && Underlay.Protection > 0) return true;
                return false;
            }
        }

        public bool HasBomb
        {
            get { return (StaticMatchBomb || DynamicMatchBomb || DynamicClickBomb); }
        }

        public bool PhysStep { get; private set; }

        public NeighBors Neighbors { get; private set; }

        public int AddRenderOrder { get { return (GRow != null && GColumn != null) ? (GColumn.Length - Row) * 2 : 0; } }

        private SoundMaster MSound { get { return SoundMaster.Instance; } }
        private GameBoard MBoard { get { return GameBoard.Instance; } }
        private GameConstructSet GCSet { get { return GameConstructSet.Instance; } }
        private LevelConstructSet LCSet { get { return GCSet.GetLevelConstructSet(GameLevelHolder.CurrentLevel); } }
        private GameObjectsSet GOSet { get { return GCSet.GOSet; } }
        private TouchManager Touch { get { return TouchManager.Instance; } }
        #endregion properties 

        #region temp
        private List<GridCellState> undoStates;
        private TweenSeq collectSequence;
        private MatchObject mObjectOld;
        private TweenSeq tS;

        private GameMode gMode;

        public PFCell pfCell;
        [Header("Fill Path")]
        public List<GridCell> fillPathToSpawner;
        public GridCell GravityMather { get { return (fillPathToSpawner != null && fillPathToSpawner.Count > 0) ? fillPathToSpawner[0] : null; } }

        public Spawner spawner;

        private static Action<GameObject, float, Action> delayAction = (g, del, callBack) => { SimpleTween.Value(g, 0, 1, del).AddCompleteCallBack(callBack); };
        #endregion temp

        #region touchbehavior
        private void PointerDownEventHandler(TouchPadEventArgs tpea)
        {
            if (GameBoard.GMode == GameMode.Play)
            {
                GCPointerDownEvent?.Invoke(this);
                Touch.SetTarget(null);
                if (IsDraggable()) Touch.SetDraggable(this, (cBack) =>
                {
                    if (Match)
                    {
                        GrabDynamicObject(Match.gameObject, false, cBack);
                        Touch.SetTarget(null);
                        Touch.SetDraggable(null, null);
                    }
                });
                else
                {
                    Touch.SetDraggable(null, null);
                }
            }
            else
            {
                GCPointerDownEvent?.Invoke(this);
            }
        }

        private void DragEventHandler(TouchPadEventArgs tpea)
        {
            if (GameBoard.GMode == GameMode.Play)
            {
            }
        }

        private void DragBeginEventHandler(TouchPadEventArgs tpea)
        {
            if (GameBoard.GMode == GameMode.Play)
            {
                Touch.SetTarget(null);
                if (Touch.Draggable)
                {
                    StartCoroutine(WaitTarget(0.5f));
                }
            }
        }

        private void DragDropEventHandler(TouchPadEventArgs tpea)
        {
            if (GameBoard.GMode == GameMode.Play)
            {
            }
        }

        public void DragEnterEventHandler(TouchPadEventArgs tpea)
        {
            if (GameBoard.GMode == GameMode.Play)
            {
                Debug.Log("drag enter: " + this + " ;touch target: " + Touch.Target);
                if (Touch.Target) return;
                if (IsDraggable() && Neighbors.Contain(Touch.Source))
                {
                    Touch.SetTarget(this);
                    GCDragEnterEvent?.Invoke(this);
                }
            }
        }

        private void DragExitEventHandler(TouchPadEventArgs tpea)
        {
            if (GameBoard.GMode == GameMode.Play)
            {
                //Touch.SetTarget(null);
                //if (Touch.Draggable)
                //{
                //    StartCoroutine(WaitTarget(0.05f));
                //}
            }
        }

        private void PointerUpEventHandler(TouchPadEventArgs tpea)
        {
            if (GameBoard.GMode == GameMode.Play)
            {
                if (Touch.Draggable)
                {
                    Touch.ResetDrag(null);
                }
            }
        }

        public bool CanSwap(GridCell gCellOther)
        {
            if (!gCellOther) return false;
            if (!gCellOther.IsDraggable()) return false;
            if (IsDraggable() && Neighbors.Contain(gCellOther)) return true;
            return false;
        }

        internal bool CanCombined(GridCell gCellOther)
        {
           return (CanSwap(gCellOther) && HasBomb && gCellOther.HasBomb);
        }

        private IEnumerator WaitTarget(float Waittime)
        {
            yield return new WaitForSeconds(Waittime);
            if (!Touch.Target && Touch.Draggable)
                Touch.ResetDrag(null);
        }

        public bool IsDraggable()
        {
            if ((Match && !Overlay) ||(Falling && Falling.canSwap && !Overlay))  return true;
            return false;
        }

        public GridCell LeftDraggable()
        {
            return CanSwap(Neighbors.Left) ? Neighbors.Left : null;// return IsDraggable() && Neighbors.Left && Neighbors.Left && Neighbors.Left.IsMatchable;
        }

        public GridCell RightDraggable()
        {
            return CanSwap(Neighbors.Right) ? Neighbors.Right: null;
        }

        public GridCell TopDraggable()
        {
            return CanSwap(Neighbors.Top) ? Neighbors.Top : null;
        }

        public GridCell BottomDraggable()
        {
            return CanSwap(Neighbors.Bottom) ? Neighbors.Bottom: null;
        }
        #endregion touchbehavior

        #region set mix grab clean
        internal void SetObject(int ID)
        {
            GridObject prefab = GOSet.GetObject(ID);
            if (prefab) prefab.Hits = 0;
            SetObject(prefab);
        }

        internal void SetObject(int ID, int hits)
        {
            GridObject prefab = GOSet.GetObject(ID);
            if (prefab) prefab.Hits = hits;
            SetObject(prefab);
        }

        internal GridObject SetObject(GridObject prefab)
        {
            if (!prefab) return null;
            GridObject gO = prefab.Create(this, MBoard.TargetCollectEventHandler);
            if (gO && !GameObjectsSet.IsDisabledObject(prefab.ID)) sRenderer.enabled = true;
            return gO;
        }

        internal void MixJump(Vector3 pos, Action completeCallBack)
        {
            PhysStep = true;
            SimpleTween.Move(DynamicObject, transform.position, pos, 0.5f).AddCompleteCallBack(() =>
            {
                PhysStep = false;
                completeCallBack?.Invoke();
            }).SetEase(EaseAnim.EaseInSine);
        }

        internal void GrabDynamicObject(GameObject dObject, bool fast, Action completeCallBack)
        {
            if (dObject)
            {
                dObject.transform.parent = transform;
                if (!fast)
                    MoveTween(dObject, completeCallBack);
                else
                    FastMoveTween(dObject, completeCallBack);
            }
            else
            {
                completeCallBack?.Invoke();
            }
        }

        /// <summary>
        /// Try to grab match object from fill path
        /// </summary>
        /// <param name="completeCallBack"></param>
        internal void FillGrab(Action completeCallBack)
        {
            GameObject mObject = null;
            GridCell gCell = null;

            if (spawner)
            {
                GridObject mo = spawner.Get();
                mObject =(mo) ? mo.gameObject : null;
            }
            else
            {
                gCell = (fillPathToSpawner!=null && fillPathToSpawner.Count>0) ? fillPathToSpawner[0] : GravityMather;
                mObject = gCell.DynamicObject; 
            }
            if (mObject && gCell && (gCell.PhysStep)) return;

            GrabDynamicObject(mObject, (MBoard.fillType == FillType.Fast), completeCallBack);
        }

        /// <summary>
        ///  mainObject = null;
        /// </summary>
        public void UnparentDynamicObject()
        {
           if(DynamicObject)  DynamicObject.transform.parent = null;
        }

        public GridObject [] GetGridObjects()
        {
            return GetComponentsInChildren<GridObject>();
        }

        public List<GridObjectState> GetGridObjectsStates()
        {
            GridObject[] gOs = GetGridObjects();
            List<GridObjectState> res = new List<GridObjectState>();
            foreach (var item in gOs)
            {
                res.Add(new GridObjectState( item.ID, item.Hits));
            }
            return res;
        }

        public List<int> GetGridObjectsIDs()
        {
            List<int> res = new List<int>();
            foreach (var item in GetGridObjects())
            {
                res.Add(item.ID);
            }
            return res;
        }

        public void RemoveObject(int id)
        {
            sRenderer.enabled = true;
            GridObject[] gOs = GetComponentsInChildren<GridObject>();
            foreach (var gO in gOs)
            {
                if (gO && gO.ID == id) { gO.transform.parent = null;  DestroyImmediate(gO.gameObject); }
            }
        }

        public T GetObject<T>() where T: GridObject
        {
            return GetComponentInChildren<T>();
        }
        #endregion set mix grab

        #region grid objects behavior
        private void FastMoveTween(GameObject mObject, Action completeCallBack)
        {
            PhysStep = true;
            tS = new TweenSeq();
            Vector3 scale = transform.localScale;
            float tweenTime = 0.07f;
            float distK = Vector3.Distance(mObject.transform.position, transform.position) / GameBoard.MaxDragDistance;

            //move
            tS.Add((callBack) =>
            {
                SimpleTween.Move(mObject, mObject.transform.position, transform.position, tweenTime * distK).AddCompleteCallBack(() =>
                {
                    mObject.transform.position = transform.position;
                    PhysStep = false;
                    completeCallBack?.Invoke();
                    callBack();
                });
            });
            tS.Start();
        }

        private void MoveTween(GameObject mObject, Action completeCallBack)
        {
            PhysStep = true;
            tS = new TweenSeq();
            Vector3 scale = transform.localScale;
            float tweenTime = 0.07f;
            float distK = Vector3.Distance(mObject.transform.position, transform.position) / GameBoard.MaxDragDistance;
            AnimationCurve scaleCurve = GameBoard.Instance.arcCurve;

            Vector2 dPos = mObject.transform.position - transform.position;
            bool isVert = (Mathf.Abs(dPos.y) > Mathf.Abs(dPos.x));

            //move
            tS.Add((callBack) =>
            {
                SimpleTween.Move(mObject.gameObject, mObject.gameObject.transform.position, transform.position, tweenTime * distK).AddCompleteCallBack(() =>
                {
                    mObject.transform.position = transform.position;
                    callBack();
                }).SetEase(EaseAnim.EaseInSine);
            });

            //curve deform
            tS.Add((callBack) =>
            {
                SimpleTween.Value(mObject, 0.0f, 1f, 0.1f).SetEase(EaseAnim.EaseInSine).SetOnUpdate((float val) =>
                {
                    float t_scale = 1.0f + scaleCurve.Evaluate(val) * 0.1f;
                    mObject.transform.localScale = (isVert) ? new Vector3(t_scale, 2.0f - t_scale, 1) : new Vector3(2.0f - t_scale, t_scale, 1) ; //  mObject.SetLocalScaleX(t_scale); //  mObject.SetLocalScaleY(2.0f - t_scale);

                }).AddCompleteCallBack(() =>
                {
                    PhysStep = false;
                    completeCallBack?.Invoke();
                    callBack();
                });
            });

            tS.Start();
        }

        /// <summary>
        /// Show simple zoom sequence of main object
        /// </summary>
        /// <param name="completeCallBack"></param>
        internal void ZoomMatch(Action completeCallBack)
        {
            if (!Match)
            {
                completeCallBack?.Invoke();
                return;
            }

            Match.Zoom(completeCallBack);
        }

        /// <summary>
        /// Colect match object
        /// </summary>
        /// <param name="completeCallBack"></param>
        internal void CollectMatch(float delay, bool showPrefab, bool fly, bool hitProtection, bool sideHitProtection, bool showBombExplode, bool showScore, int score, Action completeCallBack)
        {
            if (!Match)
            {
                completeCallBack?.Invoke();
                return;
            }

            if (HasBomb)
            {
                ExplodeBomb(delay, showBombExplode, true, false, completeCallBack);
            }
            else
            {
                Match.Collect(this, delay, showPrefab, fly, hitProtection, sideHitProtection, showScore, score, completeCallBack);
            }
        }

        /// <summary>
        /// Play explode animation and explode area
        /// </summary>
        /// <param name="completeCallBack"></param>
        internal void ExplodeBomb(float delay,
            bool playExplodeAnimation,
            bool showCollectPrefab, 
            bool collectFly,
            Action completeCallBack)
            {
            BombObject bomb = GetBomb();
            if (!bomb)
            {
                completeCallBack?.Invoke();
                return;
            }

            bomb.transform.parent = null;

            if(playExplodeAnimation)
            bomb.PlayExplodeAnimation( this, delay, () =>
                {
                    bomb.ExplodeArea(this, 0, showCollectPrefab, collectFly, true, completeCallBack);
                });
            else
            {
               bomb.ExplodeArea(this, 0, showCollectPrefab, collectFly, true, completeCallBack);
            }
        }

        /// <summary>
        /// Move match to gridcell and collect
        /// </summary>
        /// <param name="bombCell"></param>
        /// <param name="completeCallBack"></param>
        internal void MoveMatchAndCollect(GridCell toCell, float delay, bool showPrefab, bool fly, bool hitProtection, bool showBombExplode, Action completeCallBack)
        {
            if (!Match)
            {
                completeCallBack?.Invoke();
                return;
            }

          Match.MoveMatchToBomb(this, toCell, delay, hitProtection , ()=> {
                  CollectMatch( delay, showPrefab, fly, false, false,  showBombExplode, false, 0, completeCallBack);
          });
        }

        public static void ExplodeCell(GridCell gCell, float delay, bool showPrefab, bool fly, bool hitProtection, Action completeCallBack)
        {
            if (gCell.GetBomb() && !gCell.MatchProtected)
            {
                gCell.ExplodeBomb(delay, true, true, false, completeCallBack);
                return;
            }

            if (gCell.Overlay && gCell.Overlay.BlockMatch)
            {
                delayAction(gCell.gameObject, delay, () => { gCell.DirectHit(null); completeCallBack?.Invoke(); });
                return;
            }

            if (gCell.Match && gCell.IsMatchable)
            {
                gCell.Match.Explode(gCell, showPrefab, fly, hitProtection, hitProtection, delay, completeCallBack);
                return;
            }

            completeCallBack?.Invoke();
        }

        /// <summary>
        /// play iddle animation
        /// </summary>
        /// <param name="completeCallBack"></param>
        internal void PlayIddle()
        {
            if (!Match)
            {
                return;
            }
            Creator.InstantiateAnimPrefab(Match.iddleAnimPrefab, Match.transform, Match.transform.position,  SortingOrder.MainIddle);
        }

        /// <summary>
        /// Side hit cell protection
        /// </summary>
        internal void DirectHit(Action completeCallBack)
        {
            if (Overlay)
            {
                Overlay.Hit(this, completeCallBack);
                return;
            }
            if (Underlay)
            {
                Underlay.Hit(this, completeCallBack);
                return;
            }
            if (Hidden)
            {
                Hidden.Hit(this, completeCallBack);
                return;
            }
            completeCallBack?.Invoke();
        }

        /// <summary>
        /// Side hit neighbourn from collected
        /// </summary>
        internal void SideHit(Action completeCallBack)
        {
            if ((Overlay && Overlay.BlockMatch))
            {
                Overlay.Hit(this, completeCallBack);
                return;
            }
            else if (Blocked && Blocked.Destroyable)
            {
                Blocked.Hit(this, completeCallBack);
                return;
            }
            completeCallBack?.Invoke();
        }
        #endregion matchobject behavior

        /// <summary>
        ///  used by instancing for cache data
        /// </summary>
        internal void Init(int cellRow, int cellColumn, Column<GridCell> column, Row<GridCell> row, GameMode gMode)
        {
            Row = cellRow;
            Column = cellColumn;
            GColumn = column;
            GRow = row;
            this.gMode = gMode;
            undoStates = new List<GridCellState>();
#if UNITY_EDITOR
            name = ToString();
#endif
            sRenderer = GetComponent<SpriteRenderer>();
            sRenderer.sortingOrder = SortingOrder.Base;
            Neighbors = new NeighBors(this, false);

            PointerDownEvent = PointerDownEventHandler;
            DragBeginEvent = DragBeginEventHandler;
            DragEnterEvent = DragEnterEventHandler;
            DragExitEvent = DragExitEventHandler;
            DragDropEvent = DragDropEventHandler;
            PointerUpEvent = PointerUpEventHandler;
            DragEvent = DragEventHandler;
        }

        /// <summary>
        ///  return true if  match objects of two cells are equal
        /// </summary>
        internal bool IsMatchObjectEquals(GridCell other)
        {
            if (other == null) return false; 
            if (Match == null) return false;
            return Match.Equals(other.Match);
        }

        /// <summary>
        ///  cancel any tween on main MainObject object
        /// </summary>
        internal void CancelTween()
        {
         //   Debug.Log("Cancel tween");
            if (DynamicObject)
            {
                SimpleTween.Cancel(DynamicObject, false);
                DynamicObject.transform.localScale = Vector3.one;
                DynamicObject.transform.position = transform.position;
            }
            if (Match)
            {
                Match.CancellTweensAndSequences();
                Match.ResetTween();
            }
            if (mObjectOld)
            {
                mObjectOld.CancellTweensAndSequences();
            }
        }

        /// <summary>
        ///  return true if Gridcell not blocked and not disabled
        /// </summary>
        internal bool CanFill()
        {
            if (IsDisabled || Blocked || DynamicObject || Treasure) return false;

            return true;
        }

        /// <summary>
        /// Save current grid cell state to undo List
        /// </summary>
        internal void SaveUndoState()
        {
            if (undoStates.Count >= 5)
            {
                undoStates.RemoveAt(0);
            }
            GridCellState s = new GridCellState(this);
            undoStates.Add(s);
        }

        /// <summary>
        /// Return grid cell to previous grid cell state
        /// </summary>
        internal void PreviousUndoState()
        {
            if (undoStates == null || undoStates.Count == 0) return;
            GridCellState gCS = undoStates[undoStates.Count - 1];
            gCS.RestoreState(this);
            undoStates.RemoveAt(undoStates.Count - 1);
        }

        /// <summary>
        /// DestroyImeediate MainObject, OverlayProtector, UnderlayProtector
        /// </summary>
        internal void DestroyGridObjects()
        {
            GridObject[] gridObjects = GetComponentsInChildren<GridObject>();
            foreach (var item in gridObjects)
            {
                DestroyImmediate(item.gameObject);
            }
        }

        public BombObject GetBomb()
        {
            if (StaticMatchBomb) return StaticMatchBomb;
            if (DynamicMatchBomb) return DynamicMatchBomb;
            if (DynamicClickBomb) return DynamicClickBomb;
            return null;
        } 

        public override string ToString()
        {
            return "cell : [ row: " + Row + " , col: " + Column + "]";
        }

        public bool HaveObjectWithID(int id)
        {
            GridObject [] gOs = GetComponentsInChildren<GridObject>();
            foreach (var gO in gOs)
            {
                if (gO && gO.ID == id) return true;
            }
            return false;
        }

        public void CreateBorder()
        {
            if(Left && LeftBotCorner)
            {
                if (!Neighbors.Left || Neighbors.Left.IsDisabled)
                {
                    SpriteRenderer srL = Creator.CreateSpriteAtPosition(transform, Left, new Vector3(LeftBotCorner.position.x, transform.position.y, transform.position.z), 1);
                    srL.name = "Left border: " + ToString();
                }
            }

            if (Right && RightBotCorner)
            {
                if (!Neighbors.Right || Neighbors.Right.IsDisabled)
                {
                    SpriteRenderer srR = Creator.CreateSpriteAtPosition(transform, Right, new Vector3(RightBotCorner.position.x, transform.position.y, transform.position.z), 1);
                    srR.name = "Right border: " + ToString();
                }
            }

            if (Top && RightTopCorner)
            {
                if (!Neighbors.Top || Neighbors.Top.IsDisabled)
                {
                    SpriteRenderer srT = Creator.CreateSpriteAtPosition(transform, Top, new Vector3(transform.position.x, RightTopCorner.position.y, transform.position.z), 1);
                    srT.name = "Top border: " + ToString();
                }
            }

            if (Bottom && RightBotCorner)
            {
                if (!Neighbors.Bottom || Neighbors.Bottom.IsDisabled)
                {
                    SpriteRenderer srB = Creator.CreateSpriteAtPosition(transform, Bottom, new Vector3(transform.position.x, RightBotCorner.position.y, transform.position.z), 1);
                    srB.name = "Bottom border: " + ToString();
                }
            }

            if(OutTopLeft && LeftTopCorner)
            {
                if ((!Neighbors.Left || Neighbors.Left.IsDisabled) && (!Neighbors.Top || Neighbors.Top.IsDisabled))
                {
                    SpriteRenderer srTL = Creator.CreateSpriteAtPosition(transform, OutTopLeft, LeftTopCorner.position, 1);
                    srTL.name = "OutTopLeft border: " + ToString(); 
                }
            }

            if (OutBotLeft && LeftBotCorner)
            {
                if ((!Neighbors.Left || Neighbors.Left.IsDisabled) && (!Neighbors.Bottom || Neighbors.Bottom.IsDisabled))
                {
                    SpriteRenderer sr = Creator.CreateSpriteAtPosition(transform, OutBotLeft, LeftBotCorner.position, 1);
                    sr.name = "OutBotLeft border: " + ToString();
                }
            }

            if (OutBotRight && RightBotCorner)
            {
                if ((!Neighbors.Right || Neighbors.Right.IsDisabled) && (!Neighbors.Bottom || Neighbors.Bottom.IsDisabled))
                {
                    SpriteRenderer sr = Creator.CreateSpriteAtPosition(transform, OutBotRight, RightBotCorner.position, 1);
                    sr.name = "OutBotLeft border: " + ToString();
                }
            }

            if (OutTopRight && RightTopCorner)
            {
                if ((!Neighbors.Right || Neighbors.Right.IsDisabled) && (!Neighbors.Top || Neighbors.Top.IsDisabled))
                {
                    SpriteRenderer sr = Creator.CreateSpriteAtPosition(transform, OutTopRight, RightTopCorner.position, 1);
                    sr.name = "OutBotLeft border: " + ToString();
                }
            }

            NeighBors n = new NeighBors(this, true);
            if (InTopLeft && LeftTopCorner)
            {
                if ((!Neighbors.Left || Neighbors.Left.IsDisabled) && n.TopLeft && !n.TopLeft.IsDisabled)
                {
                    //SpriteRenderer sr = Creator.CreateSprite(transform, InTopLeft, LeftTopCorner.position, 2);
                    //sr.name = "InTopLeft border: " + ToString();
                }
            }

            if (InBotLeft && LeftBotCorner)
            {
                if ((!Neighbors.Left || Neighbors.Left.IsDisabled) && n.BottomLeft && !n.BottomLeft.IsDisabled)
                {
                    //SpriteRenderer sr = Creator.CreateSprite(transform, InBotLeft, LeftBotCorner.position, 2);
                    //sr.name = "InBotLeft border: " + ToString();
                }
            }

            if (InTopRight && RightTopCorner)
            {
                if ((!Neighbors.Right || Neighbors.Right.IsDisabled) && n.TopRight && !n.TopRight.IsDisabled)
                {
                    //SpriteRenderer sr = Creator.CreateSprite(transform, InTopRight, RightTopCorner.position, 2);
                    //sr.name = "InTopRight border: " + ToString();
                }
            }

            if (InBotRight && RightBotCorner)
            {
                if ((!Neighbors.Right || Neighbors.Right.IsDisabled) && n.BottomRight && !n.BottomRight.IsDisabled)
                {
                    //SpriteRenderer sr = Creator.CreateSprite(transform, InBotRight, RightBotCorner.position, 2);
                    //sr.name = "InBotRight border: " + ToString();
                }
            }

        }

        /// <summary>
        /// return true if have fillpath to spawner or (if fillpath == null) have dynamic object at top 
        /// </summary>
        /// <returns></returns>
        public bool HaveFillPath()
        {
            return ((!Blocked && !IsDisabled && spawner)
                || (!Blocked && !IsDisabled  && !MovementBlocked  && fillPathToSpawner != null && fillPathToSpawner.Count > 0 ))
                || (!Blocked && !IsDisabled && GravityMather && !GravityMather.Blocked &&  !GravityMather.IsDisabled  && GravityMather.DynamicObject && !MovementBlocked && !GravityMather.MovementBlocked);
        }

        private bool PathIsFull()
        {
            if (fillPathToSpawner == null) return false;
            foreach (var item in fillPathToSpawner)
            {
                if (!item.DynamicObject) return false;
            }
            return true;
        }

        public void CreateSpawner(Spawner prefab, Vector2 offset)
        {
            Vector3 pos = transform.position;
            spawner = Instantiate(prefab, transform.position, Quaternion.identity);
            spawner.transform.localScale = Vector3.one;
            spawner.transform.parent = transform;
            spawner.transform.localPosition -= (Vector3)offset;
            spawner.gridCell = this;
        }
    }

    [Serializable]
    public class GridCellState
    {
        public GridCellState(GridCell c)
        {

        }

        public void RestoreState(GridCell c)
        {

        }
    }

    public static class GCListExtension
    {
        public static List<GridCell> SortByDistanceTo(this List<GridCell> list, GridCell gC)
        {
            list.Sort(delegate (GridCell x, GridCell y) // x==y ->0; x>y ->1; x<y -1
            {
                if (x == null)
                {
                    if (y == null)
                    {
                        return 0;// If x is null and y is null, they're equal. 
                }
                    else
                    {
                        return -1;// If x is null and y is not null, yis greater. 
                }
                }
                else
                {
                    float xDist = Vector2.Distance(x.transform.position, gC.transform.position);
                    float yDist = Vector2.Distance(y.transform.position, gC.transform.position);
                    if (xDist == yDist) return 0;
                    if (xDist > yDist) return -1;
                    return 1;
                }
            });
            return list;
        }
    }
}