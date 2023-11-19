using System;
using System.Collections.Generic;
using UnityEngine;

namespace Mkey
{
    public class BlockedObject : GridObject
    {
        [SerializeField]
        private bool destroyable;
        public Sprite[] protectionStateImages;
        public GameObject hitAnimPrefab;

        public int hitCnt = 0;

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


        public Queue<GridObject> poolingObjectQueue;


        public override void returnObject(GridObject go)
        {
            go.gameObject.SetActive(false);
            go.transform.SetParent(this.transform);
            this.poolingObjectQueue.Enqueue(go);
        }



        //public override void initailze(GridObject go)
        //{
        //    poolingObjectQueue = new Queue<GridObject>();


        //    for(int i=0;i<100;i++)
        //    {
        //        poolingObjectQueue.Enqueue(CreateNewObject(go));
        //    }


        //    //if (!parent) return null;
        //    //parent.DestroyGridObjects(); // new


        //    //BlockedObject gridObject = Instantiate(this, parent.transform);


        //    //if (!gridObject) return null;
        //    //gridObject.transform.localScale = Vector3.one;
        //    //gridObject.transform.localPosition = Vector3.zero;
        //    //gridObject.name = "blocked " + parent.ToString();

        //    //return gridObject;
        //}


        //public override GridObject CreateNewObject(GridObject go)
        //{

        //    BlockedObject gridObject = Instantiate(this);

        //    gridObject.gameObject.SetActive(false);
        //    gridObject.transform.SetParent(this.transform);

        //    return gridObject;
        //}


        public override GridObject new_create(GridCell parent)
        {
            if (!parent) return null;
            parent.DestroyGridObjects(); // new


            BlockedObject gridObject = Instantiate(this, parent.transform);



            gridObject.transform.localScale = Vector3.one;
            gridObject.transform.localPosition = Vector3.zero;

#if UNITY_EDITOR
            gridObject.name = "blocked " + parent.ToString();
#endif
            gridObject.TargetCollectEvent = TargetCollectEvent;
            SetToFront(false);
            gridObject.Hits = Mathf.Clamp(Hits, 0, protectionStateImages.Length);
            if (protectionStateImages.Length > 0 && Hits > 0)
            {
                int i = Mathf.Min(Hits - 1, protectionStateImages.Length - 1);
            }

            gridObject.gameObject.SetActive(false);


            return gridObject;



            //if (this.blokedPoolingObjectQueue.Count > 0)
            //{
            //    var obj = this.blokedPoolingObjectQueue.Dequeue();
            //    obj.transform.SetParent(null);
            //    obj.gameObject.SetActive(true);
            //    return obj;
            //}

            //else
            //{
            //    var newObj = this.CreateNewObject();
            //    newObj.gameObject.SetActive(true);
            //    newObj.transform.SetParent(null);
            //    return newObj;
            //}

            //BlockedObject gridObject = Instantiate(this, parent.transform);
            //if (!gridObject) return null;
            //gridObject.transform.localScale = Vector3.one;
            //gridObject.transform.localPosition = Vector3.zero;
            //gridObject.name = "blocked " + parent.ToString();

        }


        public override Sprite[] GetProtectionStateImages()
        {
            return protectionStateImages;
        }
        #endregion override
    }
}
