using System.Collections.Generic;
using UnityEngine;
using System;

namespace Mkey
{
    public class CombinedLineBombAndLineBomb : CombinedBomb
    {
        public SpriteRenderer sR;
        private ParallelTween pT;
        [SerializeField]
        private BombObject bombLineVertPrefab;
        [SerializeField]
        private BombObject bombLineHorPrefab;

        #region override
        internal override void PlayExplodeAnimation(GridCell gCell, float delay, Action completeCallBack)
        {
            if (!gCell || explodePrefab == null) completeCallBack?.Invoke();
            TweenSeq anim = new TweenSeq();
            pT = new ParallelTween();
 
            anim.Add((callBack) => // delay
            {
                delayAction(gameObject, delay, callBack);
            });

            anim.Add((callBack) =>
            {
                SimpleTween.Value(gameObject, 1, 1.5f, 0.2f).SetOnUpdate((float val)=> { transform.localScale = gCell.transform.lossyScale * val; }).AddCompleteCallBack(callBack);
            });

            anim.Add((callBack) => // scale in and explode prefab
            {
                SimpleTween.Value(gameObject, 1.5f, 1.0f, 0.15f).SetOnUpdate((float val) => { transform.localScale = gCell.transform.lossyScale * val; }).AddCompleteCallBack(callBack);
                GameObject g = Instantiate(explodePrefab);
                if (g)
                {
                    g.transform.position = transform.position;
                    g.transform.localScale = transform.localScale * .50f;
                }
            });

            anim.Add((callBack) => // create line bombs
            {
                BombObject r1 = (BombObject)Instantiate(bombLineHorPrefab);
                pT.Add((cB) => 
                {
                    ExplodeBomb(r1, gCell, 0, cB);
                });

                BombObject r2 = (BombObject)Instantiate(bombLineVertPrefab);
                pT.Add((cB) =>
                {
                    ExplodeBomb(r2, gCell, 0, cB);
                });
                callBack();
            });

            anim.Add((callBack) => // explode wave
            {
                MBoard.ExplodeWave(0, transform.position, 5, null);
                callBack();
            });

            anim.Add((callBack) =>
            {
                MSound.PlayClip(0, explodeClip);
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

        private void ExplodeBomb(BombObject bomb, GridCell gCell, float delay, Action completeCallBack)
        {
            bomb.PlayExplodeAnimation(gCell, delay, () =>
            {
                bomb.ExplodeArea(gCell, 0, true, false, true, completeCallBack);
            });
        }
       
    }
}

