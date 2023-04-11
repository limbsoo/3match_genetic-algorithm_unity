using System.Collections.Generic;
using UnityEngine;
using System;

namespace Mkey
{
    public class CombinedBomb : MonoBehaviour
    {
        protected GameBoard MBoard { get { return GameBoard.Instance; } }
        protected GuiController MGui { get { return GuiController.Instance; } }
        protected SoundMaster MSound { get { return SoundMaster.Instance; } }
        protected GameConstructSet GCSet { get { return GameConstructSet.Instance; } }
        protected LevelConstructSet LCSet { get { return GCSet.GetLevelConstructSet(GameLevelHolder.CurrentLevel); } }
        protected GameObjectsSet GOSet { get { return GCSet.GOSet; } }
        protected MatchGrid MGrid { get { return MBoard.CurrentGrid; } }

        protected Action<GameObject, float, Action> delayAction = (g, del, callBack) => { SimpleTween.Value(g, 0, 1, del).AddCompleteCallBack(callBack); };

        [SerializeField]
        protected AudioClip explodeClip;
        [SerializeField]
        protected GameObject explodePrefab;

        #region virtual
        internal virtual void PlayExplodeAnimation(GridCell gCell, float delay, Action completeCallBack)
        {
            completeCallBack?.Invoke();
        }

        public virtual void ApplyToGrid(GridCell gCell, float delay,  Action completeCallBack)
        {
            completeCallBack?.Invoke();

        }

        public virtual void ExplodeArea(GridCell gCell, float delay, bool sequenced, bool showPrefab, bool fly, bool hitProtection, Action completeCallBack)
        {
            completeCallBack?.Invoke();
        }

        public virtual CellsGroup GetArea(GridCell gCell)
        {
            CellsGroup cG = new CellsGroup();
            return cG;
        }
        #endregion virtual
    }
}

