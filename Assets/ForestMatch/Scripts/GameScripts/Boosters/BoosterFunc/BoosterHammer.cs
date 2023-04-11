using System.Collections.Generic;
using UnityEngine;
using System;

namespace Mkey
{
    public class BoosterHammer: BoosterFunc
    {
        [SerializeField]
        private float speed = 20f;
        [SerializeField]
        private GameObject animPrefab;

        private ScoreController SC => MBoard.GetComponent<ScoreController>();

        #region override
        public override CellsGroup GetArea(GridCell hitGridCell)
        {
            CellsGroup cG = new CellsGroup();
            List<GridCell> area = new NeighBors(hitGridCell, true).Cells;
            cG.Add(hitGridCell);
            foreach (var item in area)
            {
              if(hitGridCell.IsMatchObjectEquals(item) && item.IsMatchable) cG.Add(item);
            }

            return cG;
        }

        public override void Apply(GridCell gCell, Action completeCallBack)
        {
            ParallelTween par0 = new ParallelTween();
            TweenSeq bTS = new TweenSeq();
            CellsGroup area = GetArea(gCell);

            //move activeBooster
            Vector3 pos = transform.position;
            float dist = Vector2.Distance(pos, gCell.transform.position);

            Vector3 rotPivot = Vector3.zero;
            float rotRad = 6f;
            bTS.Add((callBack) =>
            {
                SimpleTween.Move(gameObject, pos, gCell.transform.position, dist / speed).AddCompleteCallBack(() =>
                {
                    rotPivot = transform.position - new Vector3(0, rotRad, 0);
                    callBack();
                }).SetEase(EaseAnim.EaseInSine);
            });


            // back move
            bTS.Add((callBack) =>
            {
                SimpleTween.Value(gameObject, Mathf.Deg2Rad * 90f, Mathf.Deg2Rad * 180f, 0.25f).SetEase(EaseAnim.EaseInCubic). //
                SetOnUpdate((float val) => { transform.position = new Vector3(rotRad * Mathf.Cos(val), rotRad * Mathf.Sin(val), 0) + rotPivot; }).
                AddCompleteCallBack(() => { callBack(); });
            });
            //forward move
            bTS.Add((callBack) =>
            {

                SimpleTween.Value(gameObject, Mathf.Deg2Rad * 180f, Mathf.Deg2Rad * 100f, 0.2f).SetEase(EaseAnim.EaseOutBounce).
                    SetOnUpdate((float val) =>
                    {
                        transform.position = new Vector3(rotRad * Mathf.Cos(val), rotRad * Mathf.Sin(val), 0) + rotPivot;
                    }).
                    AddCompleteCallBack(() =>
                    {
                        SetActive(gameObject, false, 0.25f);
                        GameObject g = Creator.InstantiateAnimPrefab(animPrefab, gCell.transform, gCell.transform.position, SortingOrder.Booster + 1);
                        if (g) Destroy(g, 2);
                        callBack?.Invoke();
                    });

            });

            if (gCell.IsMatchable)
            {
                bTS.Add((callBack) =>
                {
                    ScoreHolder.Add(SC.BaseMatchScore);
                    gCell.CollectMatch(0, true, false, true, true, false, MBoard.showScore, MBoard.MatchScore, callBack);
                });
            }
            else
            {
                bTS.Add((callBack) =>
                {
                    gCell.SideHit(null);
                    callBack();
                });
            }

            bTS.Add((callback) =>
            {
                if (gameObject) Destroy(gameObject);
                completeCallBack?.Invoke();
                callback();
            });

            bTS.Start();
        }
        #endregion override
    }
}
