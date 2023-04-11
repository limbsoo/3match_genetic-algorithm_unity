using System;
using UnityEngine;

namespace Mkey
{
    public class DynamicBlockerObject : GridObject
    {
        [Header("Addit properties")]
        [Space(8)]
        public AudioClip privateClip;

        public Sprite[] protectionStateImages;

        public GameObject hitAnimPrefab;
        public bool sideHit;
        public bool directHit;

        #region properties
        public int Protection
        {
            get { return  protectionStateImages.Length + 1 - Hits; }
        }
        #endregion properties

        #region create
        internal virtual void SetData()
        {
            SetToFront(false);
        }

        /// <summary>
        /// Create new DynamicBlockerObject for gridcell
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="prefab"></param>
        /// <param name="addCollider"></param>
        /// <param name="radius"></param>
        /// <param name="isTrigger"></param>
        /// <returns></returns>
        public static DynamicBlockerObject Create(GridCell parent, DynamicBlockerObject prefab,  Action<int> TargetCollectEvent)
        {
            //            if (!parent || prefab == null) return null;
            //            GameObject gO = null;
            //            SpriteRenderer sR = null;
            //            DynamicBlockerObject gridObject = null;

            //            sR = Creator.CreateSprite(parent.transform, prefab.ObjectImage, parent.transform.position);
            //            gO = sR.gameObject;

            //            gridObject = gO.GetOrAddComponent<DynamicBlockerObject>();

            //#if UNITY_EDITOR
            //            gO.name = "dynamic blocker " + parent.ToString();
            //#endif
            //            gridObject.SetData(prefab);
            //            gridObject.SRenderer = sR;
            //            gridObject.TargetCollectEvent = TargetCollectEvent;
            //            return gridObject;
            return null;
        }

        public void SetProtection(int protection)
        {
           // Protection = protection;
        }
        #endregion create

        #region override
        public override void Hit(GridCell gCell, Action completeCallBack)
        {
            Hits++;
            int protection = Protection;

            if (protectionStateImages.Length > 0)
            {
                int i = Mathf.Min(Hits - 1, protectionStateImages.Length - 1);
                GetComponent<SpriteRenderer>().sprite = protectionStateImages[i];
            }

            if (hitAnimPrefab)
            {
                Creator.InstantiateAnimPrefab(hitAnimPrefab, transform.parent, transform.position, SortingOrder.MainExplode);
            }

            MSound.PlayClip(0, privateClip);

            if (protection == 0)
            {
                hitDestroySeq = new TweenSeq();

                SetToFront(true);

                hitDestroySeq.Add((callBack) => // play preexplode animation
                {
                    SimpleTween.Value(gameObject, 0, 1, 0.050f).AddCompleteCallBack(() =>
                    {
                        callBack();
                    });
                });

                hitDestroySeq.Add((callBack) =>
                {
                    TargetCollectEvent?.Invoke(ID);
                    MSound.PlayClip(0, privateClip, transform.position, null);
                    callBack();
                });

                hitDestroySeq.Add((callBack) =>
                {
                    completeCallBack?.Invoke();
                    Destroy(gameObject);
                    callBack();
                });

                hitDestroySeq.Start();
            }
            else
            {
                completeCallBack?.Invoke();
            }
        }

        public override void CancellTweensAndSequences()
        {
            base.CancellTweensAndSequences();
        }

        public override void SetToFront(bool set)
        {
            GridCell gC = GetComponentInParent<GridCell>();
            int addOrder = (gC) ? gC.AddRenderOrder : 0;

            if (!SRenderer) SRenderer = GetComponent<SpriteRenderer>();
            if (!SRenderer) return;
            if (set)
                SRenderer.sortingOrder = SortingOrder.MainToFront + addOrder;
            else
                SRenderer.sortingOrder = SortingOrder.Main + addOrder;
        }

        public override string ToString()
        {
            return "DynamicBlocker: " + ID;
        }
        #endregion override
    }
}
