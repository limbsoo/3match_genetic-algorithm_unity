﻿using System;
using UnityEngine;

namespace Mkey
{
    public class MatchObject : GridObject, IEquatable<MatchObject>
    {
        [Header("Addit properties")]
        [Space(8)]
        public GameObject iddleAnimPrefab;
        public GameObject collectAnimPrefab;

        [Header("Dynamic Match Bombs")]
        [Space(8)]
        public DynamicMatchBombObject dynamicMatchBombObjectVertical;
        public DynamicMatchBombObject dynamicMatchBombObjectHorizontal;
        public DynamicMatchBombObject dynamicMatchBombObjectRadial;

        //next features
        private DynamicMatchBombObject dynamicMatchBombObjectColor;
        private StaticMatchBombObject staticMatchBombObjectColor;
        private DynamicClickBombObject dynamicClickBombObjectColor;

        [SerializeField]
        private GUIFlyer scoreFlyerPrefab;

        #region properties
        public bool IsExploidable
        {
            get; internal set;
        }

        public float SwapTime { get; set; } // save last swap
        #endregion properties

        #region events
        private Action ScoreCollectEvent;
        #endregion events

        #region private
        private TweenSeq zoomSequence;
        private TweenSeq explodeSequence;
        private GridCell gCell;
        #endregion private

        #region regular

        #endregion regular

        /// <summary>
        /// Return true if object IDs is Equal
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(MatchObject other)
        {
            if (other == null) return false;
            return ((ID > 0) && (ID == other.ID));
        }

        /// <summary>
        /// Reset localscale, reset alpha
        /// </summary>
        public void ResetTween()
        {
            transform.localScale = transform.parent.localScale;
            SRenderer.color = new Color(1, 1, 1, 1);
        }

        /// <summary>
        /// Show simple zoom sequence on main object
        /// </summary>
        /// <param name="completeCallBack"></param>
        internal void Zoom(Action completeCallBack)
        {
            if (zoomSequence != null)
            {
                zoomSequence.Break();
            }

            zoomSequence = new TweenSeq();
            for (int i = 0; i < 2; i++)
            {
                zoomSequence.Add((callBack) =>
                {
                    SimpleTween.Value(gameObject, 1.0f, 1.2f, 0.07f).SetOnUpdate((float val) =>
                    {
                        SetLocalScale(val);
                    }).AddCompleteCallBack(() =>
                    {
                        callBack();
                    });
                });
                zoomSequence.Add((callBack) =>
                {
                    SimpleTween.Value(gameObject, 1.2f, 1.0f, 0.09f).SetOnUpdate((float val) =>
                    {
                       SetLocalScale(val);

                    }).AddCompleteCallBack(() =>
                    {
                        callBack();
                    });
                });
            }

            zoomSequence.Add((callBack) => { completeCallBack?.Invoke(); callBack(); });
            zoomSequence.Start();
        }

        /// <summary>
        /// Collect match object, hit overlays, hit underlays
        /// </summary>
        /// <param name="completeCallBack"></param>
        internal void Collect(GridCell gCell, float delay, bool showPrefab, bool fly, bool hitProtection, bool sideHitProtection, bool showScore, int score, Action completeCallBack)
        {
            this.gCell = gCell;
            transform.parent = null;

            collectSequence = new TweenSeq();

            collectSequence.Add((callBack) =>
            {
                delayAction(gameObject, delay, callBack);
            });

            if(showScore && score > 0) InstantiateScoreFlyer(scoreFlyerPrefab, MBoard.MatchScore);

            // sprite seq animation
            if (showPrefab)
                collectSequence.Add((callBack) =>
                {
                    if (this && !fly) GetComponent<SpriteRenderer>().enabled = false;
                    Creator.InstantiateAnimPrefab(collectAnimPrefab, transform, transform.position, SortingOrder.MainExplode);
                    delayAction (gameObject, 0.30f,  () =>
                       {
                           if (this && fly) SetToFront(true);
                           callBack();
                       });
                });

            // hit protection
            collectSequence.Add((callBack) =>
            {
                if (hitProtection)
                {
                    gCell.DirectHit(null);
                }
                if (sideHitProtection)
                {
                    gCell.Neighbors.Cells.ForEach((GridCell c) => { c.SideHit(null); });
                }
                callBack();
            });

            //fly
            if (fly)
            {
                collectSequence.Add((callBack) =>
                {
                    SimpleTween.Move(gameObject, transform.position, GameBoard.Instance.FlyTarget, 0.4f).AddCompleteCallBack(() =>
                    {
                    //  callBack();
                    });
                    callBack(); // not wait
            });

            collectSequence.Add((callBack) =>
            {
                delayAction(gameObject, 0.15f, callBack);
            });
            }
            
            // finish
            collectSequence.Add((callBack) =>
            {
                ScoreCollectEvent?.Invoke();
                TargetCollectEvent?.Invoke(ID);
                completeCallBack?.Invoke();
                Destroy(gameObject,  (fly) ? 0.4f: 0);
            });

            collectSequence.Start();
        }

        internal void SideHitCells(GridCell gCell, Action completeCallBack)
        {
            this.gCell = gCell;
            gCell.Neighbors.Cells.ForEach((GridCell c) => { c.SideHit(null); });
            completeCallBack?.Invoke();
        }

        /// <summary>
        /// show explode effect and collect match
        /// </summary>
        /// <param name="completeCallBack"></param>
        /// <param name="bomb"></param>
        /// <param name="bombType"></param>
        internal void Explode(GridCell gCell, bool showPrefab, bool fly, bool hitProtection, bool sideHitProtection, float delay, Action completeCallBack)
        {
            explodeSequence = new TweenSeq();
            transform.parent = null;
            if (delay > 0)
            {
                explodeSequence.Add((callBack) => {
                    SimpleTween.Value(gameObject, 0, 1, delay).AddCompleteCallBack(callBack);
                });
            }

            explodeSequence.Add((callBack) => 
            {
                Collect(gCell, 0, showPrefab, fly, hitProtection, sideHitProtection, MBoard.showScore, MBoard.MatchScore, callBack);
                ScoreHolder.Add(MBoard.MatchScore);
            });
            explodeSequence.Add((callBack) => { completeCallBack?.Invoke(); callBack(); });
            explodeSequence.Start();
        }

        /// <summary>
        /// If matched > = 4 cretate bomb from items
        /// </summary>
        /// <param name="bombCell"></param>
        /// <param name="completeCallBack"></param>
        internal void MoveMatchToBomb(GridCell fromCell, GridCell toCell, float delay, bool hitProtection, Action completeCallBack)
        {
            if (hitProtection)
            {
                fromCell.DirectHit(null);
                fromCell.Neighbors.Cells.ForEach((GridCell c) => { c.SideHit(null); });
            }

            SimpleTween.Move(gameObject, transform.position, toCell.transform.position, 0.15f).AddCompleteCallBack(completeCallBack).SetEase(EaseAnim.EaseInCirc).SetDelay(delay);
        }

        #region override
        public override void CancellTweensAndSequences()
        {
            SimpleTween.Cancel(gameObject, false);
            zoomSequence?.Break();
            explodeSequence?.Break();
            base.CancellTweensAndSequences();
        }

        public override void SetToFront(bool set)
        {
            if (!SRenderer) SRenderer = GetComponent<SpriteRenderer>();
            if (!SRenderer) return;
            if (set)
                SRenderer.sortingOrder = SortingOrder.MainToFront;
            else
                SRenderer.sortingOrder = SortingOrder.Main;
        }

        public override string ToString()
        {
            return "MainObject: " + ID;
        }

        public override GridObject Create(GridCell parent, Action<int> TargetCollectEvent)
        {
            if (!parent) return null;
            if (parent.IsDisabled || parent.Blocked) { return null; }
            if (parent.DynamicObject)
            {
                GameObject old = parent.DynamicObject;
                DestroyImmediate(old);
            }

            MatchObject gridObject = Instantiate(this, parent.transform);
            if (!gridObject) return null;
            gridObject.transform.localScale = Vector3.one;
            gridObject.transform.localPosition = Vector3.zero;
            gridObject.SRenderer = gridObject.GetComponent<SpriteRenderer>();
#if UNITY_EDITOR
            gridObject.name = "match id: " + gridObject.ID + "(" + gridObject.SRenderer.sprite + ")";
#endif
            gridObject.TargetCollectEvent = TargetCollectEvent;
            gridObject.ScoreCollectEvent = MBoard.MatchScoreCollectHandler;
            gridObject.SetToFront(false);
            return gridObject;
        }
        #endregion override
        }
}


