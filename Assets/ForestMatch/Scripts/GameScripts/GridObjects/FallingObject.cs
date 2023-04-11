using System;
using UnityEngine;

namespace Mkey
{
    public class FallingObject : GridObject
    {
        [Header("Addit properties")]
        [Space(8)]
        public AudioClip privateClip;
        public GameObject collectAnimPrefab;
        public bool canSwap = false;
        #region properties
        #endregion properties

        /// <summary>
        /// Collect falling object
        /// </summary>
        /// <param name="completeCallBack"></param>
        internal void Collect( float delay, bool showPrefab, bool fly,  Action completeCallBack)
        {
            transform.parent = null;
            TweenSeq  cSequence = new TweenSeq();
            if (delay > 0)
            {
                cSequence.Add((callBack) =>
                {
                    SimpleTween.Value(gameObject, 0, 1, delay).AddCompleteCallBack(callBack);
                });
            }

            cSequence.Add((callBack) =>
            {
                MSound.PlayClip(0, privateClip);
                callBack();
            });

            // sprite seq animation
            if (showPrefab)
            {
                cSequence.Add((callBack) =>
                {
                    Creator.InstantiateAnimPrefab(collectAnimPrefab, transform, transform.position, SortingOrder.MainExplode);
                    delayAction(gameObject, 1.0f, () =>
                            {
                                if (this) SetToFront(true);
                                callBack();
                            });
                });
            }
            //fly
            if (fly)
            {
                cSequence.Add((callBack) =>
                {
                    SimpleTween.Move(gameObject, transform.position, GameBoard.Instance.FlyTarget, 0.4f).AddCompleteCallBack(() =>
                    {
                        //  callBack();
                    });
                    callBack(); // not wait
                });
                cSequence.Add((callBack) =>
                {
                    SimpleTween.Value(gameObject, 0, 1, 0.15f).AddCompleteCallBack(callBack);
                });
            }
            //finish
            cSequence.Add((callBack) =>
            {
                TargetCollectEvent?.Invoke(ID);
                completeCallBack?.Invoke();
                Destroy(gameObject, (fly) ? 0.6f : 0);
                callBack();
            });

            cSequence.Start();
        }

        #region override
        public override void CancellTweensAndSequences()
        {
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
            return "FallingObject: " + ID;
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
            FallingObject gridObject = Instantiate(this, parent.transform);
            if (!gridObject) return null;
            gridObject.transform.localScale = Vector3.one;
            gridObject.transform.localPosition = Vector3.zero;
            gridObject.SRenderer = gridObject.GetComponent<SpriteRenderer>();
#if UNITY_EDITOR
            gridObject.name = "Falling: " + ID;
#endif
            gridObject.TargetCollectEvent = TargetCollectEvent;
            gridObject.SetToFront(false);
            return gridObject;
        }
        #endregion override
        }
    }