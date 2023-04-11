using System.Collections.Generic;
using UnityEngine;
using System;

namespace Mkey
{
    public class CombinedRadBombAndLineBomb : CombinedBomb
    {
        public SpriteRenderer sR;
        public List<DynamicMatchBombObject> rockets;
        private ParallelTween pT;

        [SerializeField]
        private DynamicMatchBombObject bombLineVertPrefab;
        [SerializeField]
        private DynamicMatchBombObject bombLineHorPrefab;

        #region override
        internal override void PlayExplodeAnimation(GridCell gCell, float delay, Action completeCallBack)
        {
            if (!gCell || explodePrefab == null || !bombLineVertPrefab || !bombLineHorPrefab) completeCallBack?.Invoke();
           // Debug.Log(name + "play explode animation");
            TweenSeq anim = new TweenSeq();
            pT = new ParallelTween();
            rockets = new List<DynamicMatchBombObject>();

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

            anim.Add((callBack) => // create rockets
            {
                NeighBors nB = gCell.Neighbors;
                if (nB.Left)
                {
                    GridCell c = nB.Left;
                    DynamicMatchBombObject rL =   DynamicMatchBombObject.CreateOverBoard(bombLineVertPrefab, c.transform.position, c.transform.lossyScale,  null);
                    rL.SetToFront(true);
                    pT.Add((cB) =>
                    {
                        ExplodeRocket(rL, c, 0, cB);
                    });
                    rockets.Add(rL);
                }
                if (nB.Right)
                {
                    GridCell c = nB.Right;
                    DynamicMatchBombObject rR = DynamicMatchBombObject.CreateOverBoard(bombLineVertPrefab, c.transform.position, c.transform.lossyScale, null);
                    rR.SetToFront(true);
                    pT.Add((cB) =>
                    {
                        ExplodeRocket(rR, c, 0, cB);
                    });
                    rockets.Add(rR);
                }
                if (nB.Top)
                {
                    GridCell c = nB.Top;
                    DynamicMatchBombObject rT = DynamicMatchBombObject.CreateOverBoard(bombLineHorPrefab, c.transform.position, c.transform.lossyScale, null);
                    rT.SetToFront(true);
                    pT.Add((cB) =>
                    {
                        ExplodeRocket(rT, c, 0, cB);
                    });
                    rockets.Add(rT);
                }
                if (nB.Bottom)
                {
                    GridCell c = nB.Bottom;
                    DynamicMatchBombObject rB = DynamicMatchBombObject.CreateOverBoard(bombLineHorPrefab, c.transform.position, c.transform.lossyScale, null);
                    rB.SetToFront(true);
                    pT.Add((cB) =>
                    {
                        ExplodeRocket(rB, c, 0, cB);
                    });
                    rockets.Add(rB);
                }

                DynamicMatchBombObject r1 = DynamicMatchBombObject.CreateOverBoard(bombLineHorPrefab, gCell.transform.position, gCell.transform.lossyScale, null);
                r1.SetToFront(true);
                pT.Add((cB) =>
                {
                    ExplodeRocket(r1, gCell, 0, cB);
                });
                rockets.Add(r1);
                DynamicMatchBombObject r2 = DynamicMatchBombObject.CreateOverBoard(bombLineVertPrefab, gCell.transform.position, gCell.transform.lossyScale, null);
                r2.SetToFront(true);
                pT.Add((cB) =>
                {
                    ExplodeRocket(r2, gCell, 0, cB);
                });
                rockets.Add(r2);

                callBack();
            });

            anim.Add((callBack) => // explode wave
            {
                MBoard.ExplodeWave(0, transform.position, 5, null);
                callBack();
            });

            anim.Add((callBack) => // explode sound
            {
                MSound.PlayClip(0, explodeClip);
                callBack();
            });

            anim.Add((callBack) => // delay
            { 
                delayAction(gameObject, 0, callBack);
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

        private void ExplodeRocket(BombObject bomb, GridCell gCell, float delay, Action completeCallBack)
        {
            bomb.PlayExplodeAnimation(gCell, delay, () =>
            {
                bomb.ExplodeArea(gCell, 0, true, false, true, completeCallBack);
            });
        }
       
    }
}

