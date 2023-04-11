using System.Collections.Generic;
using UnityEngine;
using System;

namespace Mkey
{
    public class CombinedColorBombAndRadBomb : CombinedBomb
    {
        private ParallelTween pT;
        private CellsGroup eArea;
        public DynamicMatchBombObject source { get; set; }
        [SerializeField]
        private DynamicMatchBombRadObject bombRadPrefab;

        #region override
        internal override void PlayExplodeAnimation(GridCell gCell, float delay, Action completeCallBack)
        {
            if (!gCell || !explodePrefab || !source || !bombRadPrefab) completeCallBack?.Invoke();
            TweenSeq anim = new TweenSeq();
            pT = new ParallelTween();

            anim.Add((callBack) => // delay
            {
                delayAction(gameObject, delay, callBack);
            });

            anim.Add((callBack) => // scale out
            {
                SimpleTween.Value(gameObject, 1, 1.5f, 0.2f).SetOnUpdate((float val)=> { transform.localScale = gCell.transform.lossyScale * val; }).AddCompleteCallBack(callBack);
            });

            anim.Add((callBack) => // scale in explode prefab
            {
                SimpleTween.Value(gameObject, 1.5f, 1.0f, 0.15f).SetOnUpdate((float val) => { transform.localScale = gCell.transform.lossyScale * val; }).AddCompleteCallBack(callBack);
                GameObject g = Instantiate(explodePrefab);
                g.transform.position = transform.position;
                g.transform.localScale = transform.localScale * .50f;
            });

            anim.Add((callBack) => // explode wave
            {
                MBoard.ExplodeWave(0, transform.position, 5, null);
                callBack();
            });

            anim.Add((callBack) => // sound
            {
                MSound.PlayClip(0, explodeClip);
                callBack();
            });

            eArea = GetArea(gCell);
            ParallelTween pT1 = new ParallelTween();

            anim.Add((callBack) =>
            {
                pT1.Start(callBack);
            });

            anim.Add((callBack) => // create bombs
            {
                foreach (var item in eArea.Cells)
                {
                    DynamicMatchBombObject r = (DynamicMatchBombObject)Instantiate(bombRadPrefab); //BombDir bd = BombDir.RadiaDynamicClickBombObject r = DynamicClickBombObject.Create(item, GoSet.GetDynamicClickBombObject(bd, 0), false, false, MBoard.TargetCollectEventHandler);
                    pT.Add((cB) =>
                    {
                        ExplodeBomb(r, item, 0.5f, cB);
                    });
                }
                callBack();
            });

            anim.Add((callBack) =>
            {
                completeCallBack?.Invoke();
                callBack();
            });

            anim.Start();
        }

        public override void ApplyToGrid(GridCell gCell, float delay,  Action completeCallBack)
        {
            if (gCell.Blocked || gCell.IsDisabled)
            {
                completeCallBack?.Invoke();
                return;
            }
            
            PlayExplodeAnimation(gCell, delay, () =>
            {
                Destroy(gameObject);
                pT.Start(completeCallBack);
            });
           
        }
        #endregion override

        private void ExplodeBomb(DynamicMatchBombObject bomb, GridCell gCell, float delay, Action completeCallBack)
        {
            bomb.PlayExplodeAnimation(gCell, delay, () =>
            {
                bomb.ExplodeArea(gCell, 0,  true, false, true, completeCallBack);
            });
        }

        public override CellsGroup GetArea(GridCell gCell)
        {
            CellsGroup cG = new CellsGroup();
            if (!gCell) return cG;

            cG.AddRange(MGrid.GetAllByID(source.matchID).SortByDistanceTo(gCell));
            return cG;
        }
    }
}

