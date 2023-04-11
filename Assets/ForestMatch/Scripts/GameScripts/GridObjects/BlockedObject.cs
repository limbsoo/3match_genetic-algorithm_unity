using System;
using UnityEngine;

namespace Mkey
{
    public class BlockedObject : GridObject
    {
        [SerializeField]
        private bool destroyable;
        public Sprite[] protectionStateImages;
        public GameObject hitAnimPrefab;

        #region properties
        public bool Destroyable { get { return destroyable; } }
        public int Protection
        {
            get { return protectionStateImages.Length + 1 - Hits; }
        }
        #endregion properties

        #region override
        public override void Hit(GridCell gCell, Action completeCallBack)
        {
            if (Protection <= 0)
            {
                completeCallBack?.Invoke();
                return;
            }

            Hits++;
            if (protectionStateImages.Length > 0)
            {
                int i = Mathf.Min(Hits - 1, protectionStateImages.Length - 1);
                SRenderer.sprite = protectionStateImages[i];
            }

            if (hitAnimPrefab)
            {
                Creator.InstantiateAnimPrefab(hitAnimPrefab, transform.parent, transform.position, SortingOrder.MainExplode);
            }

            if (Protection <= 0)
            {
                transform.parent = null;
                Debug.Log("destroyed " + ToString());
                hitDestroySeq = new TweenSeq();

                SetToFront(true);

                hitDestroySeq.Add((callBack) =>
                {
                    delayAction(gameObject, 0.05f, callBack);
                });

                hitDestroySeq.Add((callBack) =>
                {
                    TargetCollectEvent?.Invoke(ID);
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
            if (!SRenderer) SRenderer = GetComponent<SpriteRenderer>();
            if (!SRenderer) return;
            if (set)
                SRenderer.sortingOrder = SortingOrder.Blocked;
            else
                SRenderer.sortingOrder = SortingOrder.Blocked;
        }

        public override string ToString()
        {
            return "Blocked: " + ID;
        }

        public override GridObject Create(GridCell parent, Action<int> TargetCollectEvent)
        {
            if (!parent) return null;
            parent.DestroyGridObjects(); // new

            if (Hits > protectionStateImages.Length) return null;

            BlockedObject gridObject = Instantiate(this, parent.transform);
            if (!gridObject) return null;
            gridObject.transform.localScale = Vector3.one;
            gridObject.transform.localPosition = Vector3.zero;
            gridObject.SRenderer = gridObject.GetComponent<SpriteRenderer>();
#if UNITY_EDITOR
            gridObject.name = "blocked " + parent.ToString();
#endif
            gridObject.TargetCollectEvent = TargetCollectEvent;
            gridObject.SetToFront(false);
            gridObject.Hits = Mathf.Clamp(Hits, 0, protectionStateImages.Length);
            if (protectionStateImages.Length > 0 && gridObject.Hits > 0)
            {
                int i = Mathf.Min(gridObject.Hits - 1, protectionStateImages.Length - 1);
                gridObject.SRenderer.sprite = protectionStateImages[i];
            }

            return gridObject;
        }

        public override Sprite[] GetProtectionStateImages()
        {
            return protectionStateImages;
        }
        #endregion override
    }
}
