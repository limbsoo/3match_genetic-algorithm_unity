using System;
using System.Collections.Generic;
using UnityEngine;

namespace Mkey
{
    public class BombCombiner : MonoBehaviour
    {
        [SerializeField]
        private CombinedRadBombAndRadBomb radBombAndRadBombPrefab;
        [SerializeField]
        private CombinedRadBombAndLineBomb radBombAndLineBombPrefab;
        [SerializeField]
        private CombinedLineBombAndLineBomb lineBombAndLineBombPrefab;
        [SerializeField]
        private CombinedColorBombAndLineBomb colorBombAndLineBombPrefab;
        [SerializeField]
        private CombinedColorBombAndRadBomb colorBombAndRadBombPrefab;
        [SerializeField]
        private CombinedColorBombAndColorBomb colorBombAndColorBombPrefab;

        #region events
        public static Action CombineCompleteEvent;
        #endregion events

        [SerializeField]
        private Sprite glow;

        #region temp vars
        [SerializeField]
        private bool dLog = false;
        private SoundMaster MSound { get { return SoundMaster.Instance; } }
        private GameBoard MBoard { get { return GameBoard.Instance; } }
        private GameConstructSet GCSet { get { return GameConstructSet.Instance; } }
        private LevelConstructSet LCSet { get { return GCSet.GetLevelConstructSet(GameLevelHolder.CurrentLevel); } }
        private GameObjectsSet GOSet { get { return GCSet.GOSet; } }
        private ParallelTween collectTween;
        private DynamicMatchBombObject sourceColorBomb; // need only for color bomb
        #endregion temp vars 

        private void Start()
        {
            SwapHelper.BombsCombinedEvent = CombineBombsEventHandler;
        }

        public void CombineAndExplode(GridCell gCell, DynamicMatchBombObject bomb, DynamicMatchBombObject bomb2, Action completeCallBack)
        {
            if(!gCell || ! bomb)
            {
                completeCallBack?.Invoke();
                return;
            }

            NeighBors nG = gCell.Neighbors;
            BombDir bd1 = bomb.GetBombDir();
            BombCombine bC = BombCombine.None;
            List<DynamicMatchBombObject> nBs = new List<DynamicMatchBombObject>();

            BombDir bd2 = bomb2.GetBombDir();

            //if(bd1 == BombDir.Color)
            //{
            //    sourceColorBomb = GOSet.GetDynamicMatchBombObject(bomb.ID);
            //}
            //else if(bd2 == BombDir.Color)
            //{
            //    sourceColorBomb = GOSet.GetDynamicMatchBombObject(bomb2.ID);
            //}
            Debug.Log(sourceColorBomb);

            bC = GetCombineType(bd1, bd2);
            nBs.Add(bomb);
            nBs.Add(bomb2);

            switch (bC)
            {
                case BombCombine.ColorBombAndColorBomb:     // clean full board
                    collectTween = new ParallelTween();
                    nBs.Add(bomb);
                    foreach (var item in nBs)
                    {
                        MatchObject m = item.GetMatch();
                        GameObject mg = (m) ? m.gameObject : null;
                        item.transform.parent = null;
                        if (mg) Destroy(mg);
                        item.SetToFront(true);
                        Creator.CreateSpriteAtPosition(item.transform, glow, item.transform.position, SortingOrder.BombCreator - 1);
                        collectTween.Add((callBack) =>
                        {
                            item.MoveToBomb(gCell, 0, () => {Destroy(item.gameObject); callBack(); });
                        });
                    }
                    collectTween.Start(() =>
                    {
                        CombinedColorBombAndColorBomb bigBomb = Instantiate(colorBombAndColorBombPrefab);
                        bigBomb.transform.localScale = gCell.transform.lossyScale;
                        bigBomb.transform.position = gCell.transform.position;
                        bigBomb.ApplyToGrid(gCell, 0.2f, completeCallBack);
                    });

                    break;

                case BombCombine.RadBombAndRadBomb:               // big bomb explode
                    collectTween = new ParallelTween();
                    foreach ( var item in nBs)
                    {
                        MatchObject m = item.GetMatch();
                        GameObject mg = (m) ? m.gameObject : null;
                        item.transform.parent = null;
                        if (mg) Destroy(mg);
                        item.SetToFront(true);
                        Creator.CreateSpriteAtPosition(item.transform, glow, item.transform.position, SortingOrder.BombCreator - 1);
                        collectTween.Add((callBack) => 
                        {
                            item.MoveToBomb(gCell, 0, ()=> { Destroy(item.gameObject); callBack(); });
                        });
                    }
                    collectTween.Start(() =>
                    {
                        CombinedRadBombAndRadBomb bigBomb =  Instantiate(radBombAndRadBombPrefab);
                        bigBomb.transform.localScale = gCell.transform.lossyScale; 
                        bigBomb.transform.position = gCell.transform.position;
                        bigBomb.ApplyToGrid(gCell, 0.2f, completeCallBack);
                    });
                    break;
                case BombCombine.HV:           // 2 rows or 2 columns
                    collectTween = new ParallelTween();
                    lineBombAndLineBombPrefab.sR.sprite = bomb2.ObjectImage;
                    foreach (var item in nBs)
                    {
                        MatchObject m = item.GetMatch();
                        GameObject mg =(m)? m.gameObject : null;
                        item.transform.parent = null;
                        if (mg) Destroy(mg);
                        item.SetToFront(true);
                        Creator.CreateSpriteAtPosition(item.transform, glow, item.transform.position, SortingOrder.BombCreator - 1);
                        collectTween.Add((callBack) =>
                        {
                            item.MoveToBomb(gCell, 0, () => { Destroy(item.gameObject, 0.2f); callBack(); });
                        });
                    }
                    collectTween.Start(() =>
                    {
                        CombinedLineBombAndLineBomb bigBomb = Instantiate(lineBombAndLineBombPrefab);
                        bigBomb.transform.localScale = gCell.transform.lossyScale;
                        bigBomb.transform.position = gCell.transform.position;
                        bigBomb.ApplyToGrid(gCell, 0.2f, completeCallBack);
                    });
                    break;
                case BombCombine.ColorBombAndRadBomb:          // replace color match with bomb
                    collectTween = new ParallelTween();

                    foreach (var item in nBs)
                    {
                        MatchObject m = item.GetMatch();
                        GameObject mg = (m) ? m.gameObject : null;
                        item.transform.parent = null;
                        if (mg) Destroy(mg);
                        item.SetToFront(true);
                        Creator.CreateSpriteAtPosition(item.transform, glow, item.transform.position, SortingOrder.BombCreator - 1);
                        collectTween.Add((callBack) =>
                        {
                            item.MoveToBomb(gCell, 0, () => { Destroy(item.gameObject); callBack(); });
                        });
                    }
                    collectTween.Start(() =>
                    {
                        CombinedColorBombAndRadBomb colorBombAndBomb = Instantiate(colorBombAndRadBombPrefab);
                        colorBombAndBomb.transform.localScale = gCell.transform.lossyScale;
                        colorBombAndBomb.transform.position = gCell.transform.position;
                        colorBombAndBomb.source = sourceColorBomb;
                        colorBombAndBomb.ApplyToGrid(gCell, 0.2f, completeCallBack);
                        colorBombAndBomb.GetComponent<SpriteRenderer>().sprite = sourceColorBomb.ObjectImage;
                    });

                    break;

                case BombCombine.BombAndHV:             // 3 rows and 3 columns
                    collectTween = new ParallelTween();
                    foreach (var item in nBs)
                    {
                        MatchObject m = item.GetMatch();
                        GameObject mg = (m) ? m.gameObject : null;
                        item.transform.parent = null;
                        if (mg) Destroy(mg);
                        item.SetToFront(true);
                        Creator.CreateSpriteAtPosition(item.transform, glow, item.transform.position, SortingOrder.BombCreator - 1);
                        collectTween.Add((callBack) =>
                        {
                            item.MoveToBomb(gCell, 0, () => { Destroy(item.gameObject); callBack(); });
                        });
                    }
                    collectTween.Start(() =>
                    {
                        CombinedRadBombAndLineBomb bombAndRocket = Instantiate(radBombAndLineBombPrefab);
                        bombAndRocket.transform.localScale = gCell.transform.lossyScale;
                        bombAndRocket.transform.position = gCell.transform.position;
                        bombAndRocket.ApplyToGrid(gCell, 0.2f, completeCallBack);
                    });
                    break;
                case BombCombine.ColorBombAndHV:        // replace color bomb with rockets
                    collectTween = new ParallelTween();
                    nBs.Add(bomb);
                   
                    foreach (var item in nBs)
                    {
                        MatchObject m = item.GetMatch();
                        GameObject mg = (m) ? m.gameObject : null;
                        item.transform.parent = null;
                        if (mg) Destroy(mg);
                        item.SetToFront(true);
                        Creator.CreateSpriteAtPosition(item.transform, glow, item.transform.position, SortingOrder.BombCreator - 1);
                        collectTween.Add((callBack) =>
                        {
                            item.MoveToBomb(gCell, 0, () => { Destroy(item.gameObject); callBack(); });
                        });
                    }
                    collectTween.Start(() =>
                    {
                        CombinedColorBombAndLineBomb colorBombAndRocket = Instantiate(colorBombAndLineBombPrefab);
                        colorBombAndRocket.transform.localScale = gCell.transform.lossyScale;
                        colorBombAndRocket.transform.position = gCell.transform.position;
                        colorBombAndRocket.source = sourceColorBomb;
                        colorBombAndRocket.ApplyToGrid(gCell, 0.2f, completeCallBack);
                        colorBombAndRocket.GetComponent<SpriteRenderer>().sprite = sourceColorBomb.ObjectImage;
                    });
                    break;

                //case BombCombine.None:                      // simple explode
                //    gCell.ExplodeBomb(0.0f, true, true, bd1 == BombDir.Color, false, () =>
                //    {
                //        completeCallBack?.Invoke();
                //    });
                //    break;
                default:
                    completeCallBack?.Invoke();
                    break;
            }
        }

        private BombCombine GetCombineType(BombDir bd1, BombDir bd2)
        {
            //if(bd1== BombDir.Color )
            //{
            //  if (bd2 == BombDir.Color) {return BombCombine.ColorBombAndColorBomb; }
            //  if (bd2 == BombDir.Radial) return BombCombine.ColorBombAndRadBomb;
            //  if (bd2 == BombDir.Horizontal || bd2 == BombDir.Vertical) return BombCombine.ColorBombAndHV;
            //}
            if (bd1 == BombDir.Radial)
            {
              //  if (bd2 == BombDir.Color) return BombCombine.ColorBombAndRadBomb;
                if (bd2 == BombDir.Radial) return BombCombine.RadBombAndRadBomb;
                if (bd2 == BombDir.Horizontal || bd2 == BombDir.Vertical) return BombCombine.BombAndHV;
            }
            if (bd1 == BombDir.Horizontal || bd1 == BombDir.Vertical)
            {
              //  if (bd2 == BombDir.Color) return BombCombine.ColorBombAndHV;
                if (bd2 == BombDir.Radial) return BombCombine.BombAndHV;
                if (bd2 == BombDir.Horizontal || bd2 == BombDir.Vertical) return BombCombine.HV;
            }
            return BombCombine.HV;
        }

        private List <DynamicClickBombObject> GetNeighBombs(GridCell gCell)
        {
            List<DynamicClickBombObject> res = new List<DynamicClickBombObject>();
            NeighBors nG = gCell.Neighbors;
            foreach (var item in nG.Cells) // search color bomb
            {
                if (item.DynamicClickBomb)
                {
                    res.Add(item.DynamicClickBomb);
                }
            }
            return res;
        }

        private bool CombineBombsEventHandler(GridCell source, GridCell target)
        {
            if(dLog)  Debug.Log(source.GetBomb().ToString() + " / " + target.GetBomb().ToString());
            BombCombiner bC = GetComponent<BombCombiner>();
            BombObject b1 = source.GetBomb();
            BombObject b2 = target.GetBomb();

            DynamicMatchBombObject bm1 = (b1) ? b1.GetComponent<DynamicMatchBombObject>() : null;
            DynamicMatchBombObject bm2 = (b2) ? b2.GetComponent<DynamicMatchBombObject>() : null;

            if (enabled && bm1 && bm2)
            {
                bC.CombineAndExplode(target, bm1, bm2, () =>
                {
                   Debug.Log("end combined swap -> to fill");
                   CombineCompleteEvent?.Invoke();
                });
                return true;
            }
            return false;
        }
    }
}