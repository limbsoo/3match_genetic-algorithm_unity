using System.Collections.Generic;
using UnityEngine;
using System;

namespace Mkey
{
    public class CombinedColorBombAndLineBomb : CombinedBomb
    {
        [SerializeField]
        public List<BombObject> rockets;
        private ParallelTween pT;
        private CellsGroup eArea;
        public BombObject source { get; set; }
        [SerializeField]
        private BombObject bombLineVertPrefab;
        [SerializeField]
        private BombObject bombLineHorPrefab;

        #region override
        internal override void PlayExplodeAnimation(GridCell gCell, float delay, Action completeCallBack)
        {
            if (!gCell || !explodePrefab || !source || !bombLineVertPrefab || !bombLineHorPrefab) completeCallBack?.Invoke();

            TweenSeq anim = new TweenSeq();
            pT = new ParallelTween();
            rockets = new List<BombObject>();

            anim.Add((callBack) => // delay
            {
                delayAction(gameObject, delay, callBack);
            });

            anim.Add((callBack) => // scale out
            {
                SimpleTween.Value(gameObject, 1, 1.5f, 0.2f).SetOnUpdate((float val)=> { transform.localScale = gCell.transform.lossyScale * val; }).AddCompleteCallBack(callBack);
            });

            anim.Add((callBack) => // scale in and explode prefab
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

            anim.Add((callBack) => // create line bombs
            {
                foreach (var item in eArea.Cells)
                {
                    BombObject r = UnityEngine.Random.Range(0, 2) == 0 ? Instantiate(bombLineVertPrefab) : Instantiate(bombLineHorPrefab);
                    pT.Add((cB) =>
                    {
                        ExplodeBomb(r, item, 0.05f, cB);
                    });
                    rockets.Add(r);
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

        public override CellsGroup GetArea(GridCell gCell)
        {
            CellsGroup cG = new CellsGroup();
            if (!gCell) return cG;

            cG.AddRange(MGrid.GetAllByID(source.matchID).SortByDistanceTo(gCell));
            return cG;
        }
        #endregion override

        private void ExplodeBomb(BombObject bomb, GridCell gCell, float delay, Action completeCallBack)
        {
            bomb.PlayExplodeAnimation(gCell, delay, () =>
            {
                bomb.ExplodeArea(gCell, 0, true, false, true, completeCallBack);
            });
        }
    }
}

