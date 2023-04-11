using System;
using UnityEngine;

namespace Mkey
{
    public class GridObject : MonoBehaviour
    {
        [Tooltip("Picture that is used in GUI")]
        [SerializeField]
        private Sprite GuiObjectImage;
        [HideInInspector]
        [SerializeField]
        private int id = Int32.MinValue;

        #region construct object properties
        public bool canUseAsTarget;
        #endregion construct object properties

        #region properties
        public int ID { get { return id; } private set { id = value; } }
        public string Name { get { return name; } }
        protected SpriteRenderer SRenderer { get; set; }
        protected SoundMaster MSound { get { return SoundMaster.Instance; } }
        protected GameBoard MBoard { get { return GameBoard.Instance; } }
        protected MatchGrid MGrid { get { return MBoard.CurrentGrid; } }
        public Sprite ObjectImage { get { SpriteRenderer sr = GetComponent<SpriteRenderer>(); return (sr) ? sr.sprite : null; } }
        public Sprite GuiImage { get { return (GuiObjectImage) ? GuiObjectImage : ObjectImage; } }
        public Sprite GuiImageHover { get { return GuiImage; } }
        public int Hits { get; set; }
        #endregion properties

        #region events
        public Action<int> TargetCollectEvent;
        #endregion events

        #region protected temp vars
        protected Action<GameObject, float, Action> delayAction = (g, del, callBack) => { SimpleTween.Value(g, 0, 1, del).AddCompleteCallBack(callBack); };
        protected TweenSeq collectSequence;
        protected TweenSeq hitDestroySeq;
        private static Canvas parentCanvas;
        #endregion protected temp vars

        #region regular
        void OnDestroy()
        {
            CancellTweensAndSequences();
        }
        #endregion regular

        #region common
        /// <summary>
        /// Return true if is the same object (the same reference)
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        internal bool ReferenceEquals(GridObject other)
        {
            return System.Object.ReferenceEquals(this, other);//Determines whether the specified Object instances are the same instance.
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="scale"></param>
        internal void SetLocalScale(float scale)
        {
            transform.localScale = (transform.parent) ? transform.parent.localScale * scale : new Vector3(scale, scale,scale);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="scale"></param>
        internal void SetLocalScaleX(float scale)
        {
            Vector3 parLS = (transform.parent) ? transform.parent.localScale : Vector3.one;
            float ns = parLS.x * scale ;
            transform.localScale = new Vector3(ns, parLS.y, parLS.z);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="scale"></param>
        internal void SetLocalScaleY(float scale)
        {
            Vector3 parLS = (transform.parent) ? transform.parent.localScale : Vector3.one;
            float ns = parLS.y * scale;
            transform.localScale = new Vector3(parLS.x, ns, parLS.z);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="alpha"></param>
        internal void SetAlpha(float alpha)
        {
            if (!SRenderer) GetComponent<SpriteRenderer>();
            if (SRenderer)
            {
                Color c = SRenderer.color;
                Color newColor = new Color(c.r, c.g, c.b, alpha);
                SRenderer.color = newColor;
            }
        }

        internal void InstantiateScoreFlyer(GUIFlyer scoreFlyerPrefab, int score)
        {
            if (!scoreFlyerPrefab) return;
            if (!parentCanvas)
            {
                Debug.Log("no canvas");
                GameObject gC = GameObject.Find("CanvasMain");
                if (gC) parentCanvas = gC.GetComponent<Canvas>();
                if (!parentCanvas) parentCanvas = FindObjectOfType<Canvas>();
            }

            GUIFlyer flyer = scoreFlyerPrefab.CreateFlyer(parentCanvas, score.ToString());
            if (flyer)
            {
                flyer.transform.localScale = transform.lossyScale;
                flyer.transform.position = transform.position;
            }
        }
        #endregion common

        #region virtual
        /// <summary>
        /// Hit object from collected
        /// </summary>
        /// <param name="completeCallBack"></param>
        public virtual void Hit(GridCell gCell,  Action completeCallBack)
        {
            completeCallBack?.Invoke();
        }

        /// <summary>
        /// Cancel all tweens and sequences
        /// </summary>
        public virtual void CancellTweensAndSequences()
        {
            collectSequence?.Break();
            hitDestroySeq?.Break();
            SimpleTween.Cancel(gameObject, false);
        }

        public virtual void SetToFront(bool set)
        {

        }

        public virtual GridObject Create(GridCell parent, Action<int> TargetCollectEvent)
        {
            return parent? Instantiate(this, parent.transform) : Instantiate(this);
        }

        public virtual GridObject Create(GridCell parent, int hitsCount, Action<int> TargetCollectEvent)
        {
            return parent ? Instantiate(this, parent.transform) : Instantiate(this);
        }

        public virtual Sprite[] GetProtectionStateImages()
        {
            return null;
        }
        #endregion virtual

        public void Enumerate(int id)
        {
            this.id = id;
        }
    }

    [Serializable]
    public class GridObjectState
    {
        public int id;
        public int hits;

        public GridObjectState(int id, int hits)
        {
            this.id = id;
            this.hits = hits;
        }
    }
}

