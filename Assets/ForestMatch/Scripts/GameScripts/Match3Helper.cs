using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unity.Mathematics;
//using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.RuleTile.TilingRuleOutput;
using Random = UnityEngine.Random;
using static UnityEditor.Progress;
using Unity.VisualScripting;
using System.Linq.Expressions;
using Transform = UnityEngine.Transform;
using System.Drawing;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.VersionControl;
using System.Security.Cryptography.X509Certificates;
using static Mkey.GridCell;
using static UnityEngine.GraphicsBuffer;
using System.IO;
using System.Security.Cryptography;

//231018 sundry

namespace Mkey
{
    public class Difficult
    {
        public char[] map;
        public char[] protections;
        public int pottential;
        public int obstacle;
        public int blockedPottential;
        public int blocked1;
        public int blocked2;
        public int blocked3;
        public int overlayPottential;
        public int overlay1;
        public int overlay2;
        public int overlay3;

        public Difficult()
        {
            pottential = 0;
            obstacle = 0;
            blockedPottential = 0;
            blocked1 = 0;
            blocked2 = 0;
            blocked3 = 0;
            overlayPottential = 0;
            overlay1 = 0;
            overlay2 = 0;
            overlay3 = 0;
        }

    }




    public class Match3Helper
    {
        public MatchGrid grid;
        public Dictionary<int, TargetData> curTargets;
        public GameBoard board;
        public Limit limits;
        public PlayHelper plays;

        public int csvCnt;
        public int match3Cycle;

        public int csvFolder;

        //public bool isActualMeasureSwap;
        //public bool onlySpawnBlockedObject;
        //public bool onlySpawnOverlayObject;
        //public bool onlySpawnObstacleObject;


        //public bool randomSpawnObstacleObject;

        public int blockedObjectHitCnt;
        public int overlayObjectHitCnt;

        public int wantDifficulty;
        public int difficultyTolerance;

        public int gridSize;
        public int rowSize;
        public int colSize;

        public int numOfMatchBlock;
        public int blockProtection;

        //public List<int> protections;

        public bool spawnObstacleObject;
        public bool spawnBlockedObject;
        public bool spawnOverlayObject;
        public bool haveRandomProtection;
        public bool getSetGenes;
        public bool isOnce;

        public bool divideSpecificBlock;

        public int minusRange;
        public int originPoten;


        public List<MatchGrid> mgs;


        public List<Queue<GridObject>> poolingObjectQueue;


        //public Queue<List<GridCell>> llg;


        public BlockedObject obsatcle;
        public BlockedObject blocked;
        public OverlayObject overlay;
        public List<MatchObject> match;
        public List<GridCell> tmpCells;
        public List<GridCell> tmpMatchCells;


        public List<int> spawnRootSize;
        public bool knwoSpawnRootSize;

        public int fCnt;
        public int sCnt;
        public int dCnt;


        public Queue<PathFinder> pFs;


        public List<BlockedObject> blockeds;
        public List<OverlayObject> overlays;

        public Difficult[] difficults;
        public Match3Helper(MatchGrid g, Dictionary<int, TargetData> targets)
        {
            grid = g;
            curTargets = targets;
            gridSize = g.Cells.Count;
            rowSize = g.Rows.Count;
            colSize = g.Columns.Count;

            board = new GameBoard();
            limits = new Limit();

            csvCnt = 5;
            match3Cycle = 0;

            csvFolder = 0;

            isOnce = false;
            getSetGenes = false;

            divideSpecificBlock = false;

            if (isOnce && getSetGenes)
            {
                limits.match3Cycle = 1;
                limits.generation = 1;
                limits.csvCnt = 0;
            }

            else if (!isOnce && getSetGenes)
            {
                limits.match3Cycle = 15;
                limits.generation = 1;
                limits.csvCnt = 9;
            }

            else
            {
                //limits.match3Cycle = 1;
                //limits.generation = 1;
                //limits.csvCnt = 0;


                limits.match3Cycle = 121;
                limits.generation = 100;
                limits.csvCnt = 121;


                //limits.match3Cycle = 50;
                //limits.generation = 100;
                //limits.csvCnt = 29;
                ////limits.csvCnt = 9;
            }


            limits.geneticGeneration = 500;
            limits.move = 300;
            limits.repeat = 20;
            limits.find = 2000;
            limits.mix = 200;


            spawnObstacleObject = false;
            spawnBlockedObject = true;
            spawnOverlayObject = false;


            haveRandomProtection = false;
            blockProtection = 1;

            ////size 99
            //wantDifficulty = 888;
            //difficultyTolerance = 30;
            //minusRange = 80;
            //originPoten = 888;


            //size 1010

            wantDifficulty = 594;
            //wantDifficulty = 110;
            difficultyTolerance = 55;
            minusRange = 110;
            originPoten = 1144;

            ////size 1111
            //wantDifficulty = 1432;
            //difficultyTolerance = 30;
            //minusRange = 140;
            //originPoten = 1432;



            obsatcle = null;
            blocked = null;
            overlay = null;
            match = null;

            tmpCells = new List<GridCell>();
            tmpMatchCells = new List<GridCell>();

            spawnRootSize = new List<int>(gridSize);


            numOfMatchBlock = 7; //GetRandomObjectPrefab 이거 지워서 가지수바꾸려면 바꿔야함


            // pFs = new Queue<PathFinder> ();

            //for (int i = 0; i < 100; i++)
            //{
            //    pFs.Enqueue(new PathFinder());
            //}


            //llg = new Queue<List<GridCell>> ();

            //for(int i = 0; i < 100; i++) 
            //{
            //    llg.Enqueue(new List<GridCell> ());
            //}

            knwoSpawnRootSize = false;
        }



        public void makeBlocks(SpawnController sC, LevelConstructSet LcSet, GameObjectsSet goSet)
        {
            blockeds = new List<BlockedObject>();

            for (int i = 0; i < 5; i++)
            {
                BlockedObject blocked = sC.getSelectedBlockedObject(LcSet, goSet, i);
                blockeds.Add(blocked);
            }

            overlays = new List<OverlayObject>();

            for (int i = 0; i < 3; i++)
            {
                OverlayObject overlay = sC.GetSelectOverlayObject(LcSet, goSet, i);
                overlays.Add(overlay);
            }

            match = new List<MatchObject>();

            for (int i = 0; i < 7; i++)
            {
                MatchObject m = sC.GetPickMatchObject(LcSet, goSet, i);
                match.Add(m);
            }




            //obsatcle = sC.GetObstacleObjectPrefab(LcSet, goSet);
            //blocked = sC.GetSelectBreakableBlockedObjectPrefab(LcSet, goSet);
            //overlay = sC.GetSelectOverlayObjectPrefab(LcSet, goSet);

            //match = new List<MatchObject>();

            //for (int i = 0; i < 7; i++)
            //{
            //    MatchObject m = sC.GetPickMatchObject(LcSet, goSet, i);
            //    match.Add(m);
            //}


            //List<GridObject> list = new List<GridObject> ();

            //Queue<GridObject> poolingObjectQueue = new Queue<GridObject>();

            //for (int i = 0; i < 10; i++)
            //{
            //    poolingObjectQueue.Enqueue(CreateNewObject());
            //}



            //tmpCells = new List<GridCell>();



            //GridCell g = new GridCell();

            //g.Create(this, MBoard.TargetCollectEventHandler);



            //tmpCells.Add(g);

        }




        public string setMaps(int idx, bool isMap)
        {
            string[] map =
            {
                "311333030011333333133303333333333331333333313333333333333333333333333333133333113" ,
                "333330303301333333333333333333310333333333333333333333331033333301033333333333330" ,
                "331333133133333330133300133330333331333303333333333333333333333333303333333333330" ,
                "033103030133333333331333333313033313333333333333333333333333330331310331131301333" ,
                "331113331313330333133333333333333331333130333333133333133333333030313333130313103" ,
                "313033103111333113333333311100333311130333333333333311113333333033303333103011333" ,
                "130303011333333333313303333333333333333133333330013013333313133030013333330101330" ,
                "000331101303333333333333003133330331133333313133331333333003333300333310331103000" ,
                "033310030030100333001111310033333333010333300133333103333333133030033333101003030" ,
                "333133133300333331103333101313333330303333331333301333131000300010333300010300311"
            };

            string[] protections =
            {
                "011000000032000000100000000000000001000000020000000000000000000000000000200000210" ,
                "000000000002000000000000000000030000000000000000000000003000000003000000000000000" ,
                "003000300200000000100000300000000002000000000000000000000000000000000000000000000" ,
                "000100000100000000001000000020000030000000000000000000000000000002030002203002000" ,
                "003130002020000000300000000000000003000300000000200000100000000000020000300010200" ,
                "010000200211000210000000011200000012100000000000000011310000000000000000300013000" ,
                "200000032000000000030000000000000000000100000000030030000010200000030000000102000" ,
                "000002102000000000000000000200000002300000020100003000000000000000000010001100000" ,
                "000030000000200000001332030000000000010000000200000100000000300000000000102000000" ,
                "000100100000000002300000303020000000000000003000003000201000000010000000020000022"
            };

            if (isMap) return map[idx];

            else return protections[idx];
        }



        List<GridCell> gcL = new List<GridCell>();

        internal List<GridCell> GetFreeCells(MatchGrid g, bool withPath)
        {
            //gcL.Clear();
            gcL = new List<GridCell>();
            for (int i = 0; i < g.Cells.Count; i++)
            {
                if (g.Cells[i].IsDynamicFree && !g.Cells[i].Blocked && !g.Cells[i].IsDisabled)
                {
                    gcL.Add(g.Cells[i]);
                }
            }
            return gcL;
        }

        List<GridCell> gFreeCells = new List<GridCell>();


        //-- fillFreeCells ------------------------------------------------------------------------//
        public void fillFreeCells()
        {
            //gFreeCells.Clear();

            //List<GridCell> gFreeCells = new List<GridCell>();

            gFreeCells = new List<GridCell>();
            gFreeCells = GetFreeCells(grid, true);

            //int min = 0;

            if (gFreeCells.Count > 0)
            {
                //for (int i = 0; i < gFreeCells.Count; i++)
                //{
                //    if (min < gFreeCells[i].Column)
                //    {
                //        min = gFreeCells[i].Column;
                //    }
                //}

                CreateFillPath(grid);
                //new_createFillPath111111111();
                //new_createFillPathTOFill(min);

            }


            //gFreeCells = GetFreeCells(grid, true);


            while (gFreeCells.Count > 0)
            {
                new_fillGridByStep(gFreeCells);
                gFreeCells = GetFreeCells(grid, true);
                plays.fillGridCnt++;

                if (estimateMax(plays.fillGridCnt, limits.find)) return;
            }

            estimateClear();

            fCnt++;
            plays.fillGridCnt = 0;
            plays.curState = 2;
        }


        List<int> predictMatch = new List<int>();
        List<int> predictObstacle = new List<int>();

        //-- swapCells ------------------------------------------------------------------------//
        public void swapCells(DNA<char> p, Transform trans)
        {
            while (!estimateMax(plays.mixGridCnt, limits.mix))
            {
                createMatchGroups(2, true, grid);

                if (grid.mgList.Count == 0)
                {
                    mixGrid(null, grid, trans);
                    plays.mixGridCnt++;
                }

                else break;
            }

            if (plays.isError) return;

            if (grid.mgList.Count > 1)
            {
                predictMatch = null;
                predictMatch = new List<int>();
                predictObstacle = null;
                predictObstacle = new List<int>();

                for (int i = 0; i < grid.mgList.Count; i++)
                {
                    int predictCnt = 0;
                    //predictCnt += estimateIncludeTarget(grid.mgList[i]);
                    predictCnt += estimateIncludingTarget(grid.mgList[i]);

                    predictMatch.Add(predictCnt);

                    int predictObstacleCnt = 0;
                    predictObstacleCnt += estimateObstacle(grid.mgList[i]);
                    predictObstacle.Add(predictCnt);
                }

                int max = 0;
                int maxIdx = 0;

                for (int i = 0; i < predictMatch.Count; i++)
                {
                    if (max < predictMatch[i])
                    {
                        max = predictMatch[i];
                        maxIdx = i;
                    }
                }

                if (max == 0)
                {
                    int obstacleMax = 0;
                    int obstacleMaxIdx = 0;

                    for (int j = 0; j < predictObstacle.Count; j++)
                    {
                        if (obstacleMax < predictObstacle[j])
                        {
                            obstacleMax = predictObstacle[j];
                            obstacleMaxIdx = j;
                        }
                    }

                    if (obstacleMax == 0)
                    {
                        int randNum = Random.Range(0, predictMatch.Count);
                        //int randNum = Random.Range(0, predictMatch.Count - 1);
                        grid.mgList[randNum].new_SwapEstimate();
                    }

                    else grid.mgList[obstacleMaxIdx].new_SwapEstimate();

                }

                else grid.mgList[maxIdx].new_SwapEstimate();

            }

            else grid.mgList[0].new_SwapEstimate();


            sCnt++;
            p.swapCnt++;
            plays.curState = 2;
        }

        public int estimateObstacle(MatchGroup mg)
        {
            int result = 0;
            for (int i = 0; i < mg.Cells.Count; i++)
            {
                if (mg.Cells[i].Neighbors.Top != null)
                {
                    if (estimateNeighborObstacle(mg.Cells[i].Neighbors.Top)) result++;
                }

                if (mg.Cells[i].Neighbors.Left != null)
                {
                    if (estimateNeighborObstacle(mg.Cells[i].Neighbors.Left)) result++;
                }

                if (mg.Cells[i].Neighbors.Right != null)
                {
                    if (estimateNeighborObstacle(mg.Cells[i].Neighbors.Right)) result++;
                }

                if (mg.Cells[i].Neighbors.Bottom != null)
                {
                    if (estimateNeighborObstacle(mg.Cells[i].Neighbors.Bottom)) result++;
                }
            }
            return result;
        }


        public bool estimateNeighborObstacle(GridCell g)
        {
            //if (g != null) if (g.MovementBlocked) return true;

            if (g.Blocked != null)
            {
                if (!g.Blocked.Destroyable) return false;

                else return true;
            }

            else if (g.Overlay != null) return true;

            else return false;

        }




        //-- swapCells --////////////////////////////////////////////////////////////////////////


        public int estimateIncludingTarget(MatchGroup mg)
        {

            int includeTargetCnt = 0;

            foreach (var item in curTargets)
            {
                if (!item.Value.Achieved)
                {
                    List<int> mg_cell = mg.Cells[0].GetGridObjectsIDs();
                    if (mg_cell[0] == item.Value.ID) includeTargetCnt += mg.Length;
                }
            }

            return includeTargetCnt;
        }

















        public int estimateIncludeTarget(MatchGroup mg)
        {
            int includeTargetCnt = 0;

            foreach (var item in curTargets)
            {
                if (!item.Value.Achieved)
                {
                    switch (item.Value.ID)
                    {
                        case int n when (n >= 1000 && n <= 1006):
                            includeTargetCnt = estimateTargetIsMatch(mg, n);
                            break;
                        case 200001:
                            includeTargetCnt = estimateTargetIsUnderlay(mg, 200001);
                            break;
                        case 100004:
                            includeTargetCnt = estimateTargetIsOverlay(mg, 100004);
                            break;
                        case 101:
                            includeTargetCnt = estimateTargetIsBlocked(mg, 101);
                            break;
                    }
                }
            }
            return includeTargetCnt;
        }

        public int estimateTargetIsMatch(MatchGroup mg, int targetID)
        {
            int result = 0;
            List<int> mg_cell = mg.Cells[0].GetGridObjectsIDs();
            if (mg_cell[0] == targetID) result = mg.Length;
            return result;
        }

        public int estimateTargetIsUnderlay(MatchGroup mg, int targetID)
        {
            int result = 0;
            for (int i = 0; i < mg.Length; i++) if (mg.Cells[i].Underlay != null) result++;
            return result;
        }
        public int estimateTargetIsOverlay(MatchGroup mg, int targetID)
        {
            int result = 0;
            for (int i = 0; i < mg.Length; i++) if (mg.Cells[i].Overlay != null) result++;
            return result;
        }

        public bool estimateNeighborBlocked(GridCell g)
        {
            if (g != null)
            {
                if (g.Blocked != null && g.Blocked.Destroyable) return true;
            }
            return false;
        }

        public int estimateTargetIsBlocked(MatchGroup mg, int targetID)
        {
            int result = 0;
            for (int i = 0; i < mg.Cells.Count; i++)
            {
                if (mg.Cells[i].Neighbors.Top != null)
                {
                    if (estimateNeighborBlocked(mg.Cells[i].Neighbors.Top)) result++;
                }

                if (mg.Cells[i].Neighbors.Left != null)
                {
                    if (estimateNeighborBlocked(mg.Cells[i].Neighbors.Left)) result++;
                }

                if (mg.Cells[i].Neighbors.Right != null)
                {
                    if (estimateNeighborBlocked(mg.Cells[i].Neighbors.Right)) result++;
                }

                if (mg.Cells[i].Neighbors.Bottom != null)
                {
                    if (estimateNeighborBlocked(mg.Cells[i].Neighbors.Bottom)) result++;
                }
            }
            return result;
        }

        //-- matchAndDestory ------------------------------------------------------------------------//


        List<int> mgCells;

        public void matchAndDestory(DNA<char> p)
        {
            if (estimateMax(plays.findMatchCnt, limits.find)) return;

            if (grid.GetFreeCells(true).Count > 0)
            {
                plays.curState = 0;
                return;
            }

            createMatchGroups(3, false, grid);

            if (grid.mgList.Count == 0)
            {
                plays.findMatchCnt = 0;
                plays.curState = 1;
            }

            else
            {
                for (int i = 0; i < grid.mgList.Count; i++)
                {
                    foreach (var item in curTargets)
                    {
                        if (!item.Value.Achieved)
                        {
                            for (int j = 0; j < grid.mgList[i].Cells.Count; j++)
                            {
                                mgCells = null;
                                mgCells = grid.mgList[i].Cells[j].GetGridObjectsIDs();
                                //List<int> mgCells = grid.mgList[i].Cells[j].GetGridObjectsIDs();

                                if (mgCells[0] == item.Value.ID) item.Value.IncCurrCount(1);


                                if (spawnBlockedObject)
                                {
                                    destoryNeigborObstacle(grid.mgList[i].Cells[j].Neighbors.Top);
                                    destoryNeigborObstacle(grid.mgList[i].Cells[j].Neighbors.Left);
                                    destoryNeigborObstacle(grid.mgList[i].Cells[j].Neighbors.Right);
                                    destoryNeigborObstacle(grid.mgList[i].Cells[j].Neighbors.Bottom);
                                }

                                if (spawnOverlayObject)
                                {
                                    if (grid.mgList[i].Cells[j].Overlay != null)
                                    {
                                        grid.mgList[i].Cells[j].Overlay.hitCnt++;

                                        if (grid.mgList[i].Cells[j].Overlay.Protection <= grid.mgList[i].Cells[j].Overlay.hitCnt)
                                        {
                                            grid.mgList[i].Cells[j].DestroyObjects();
                                        }
                                    }
                                }


                                grid.mgList[i].Cells[j].DestroyObjects();
                            }
                        }
                    }
                }

                p.matchCnt++;
                plays.curState = 0;

                dCnt++;
            }
        }


        public void destoryNeigborObstacle(GridCell gc)
        {


            if (gc != null && gc.Blocked != null)
            {
                if (gc.Blocked.Destroyable)
                {
                    gc.Blocked.hitCnt++;

                    if (gc.Blocked.Protection <= gc.Blocked.hitCnt)
                    {
                        gc.DestroyObjects();
                    }

                    //if (gc.Blocked.Protection <= gc.Blocked.hitCnt) gc.DestroyGridObjects();
                }
            }
        }

        public bool estimateMax(int cnt, int max)
        {
            if (max < cnt)
            {
                plays.isError = true;
                return true;
            }

            return false;
        }

        public void estimateClear()
        {
            foreach (var item in curTargets)
            {
                if (item.Value.Achieved) plays.isClear = true;

                else
                {
                    plays.isClear = false;
                    break;
                }
            }
        }



        /// //////////////////////////////////////////////////////////////////////////////////////////////////
        public bool checkNeigborIsBreakableObstacle(GridCell gc)
        {
            if (gc != null)
            {
                if (gc.Blocked != null && gc.Blocked.Destroyable) return true;
            }
            return false;
        }
        public bool haveNeigborBreakableObstacle(GridCell gc)
        {
            if (gc.Neighbors.Top != null)
            {
                if (checkNeigborIsBreakableObstacle(gc.Neighbors.Top)) return true;
            }

            if (gc.Neighbors.Left != null)
            {
                if (checkNeigborIsBreakableObstacle(gc.Neighbors.Left)) return true;
            }

            if (gc.Neighbors.Right != null)
            {
                if (checkNeigborIsBreakableObstacle(gc.Neighbors.Right)) return true;
            }

            if (gc.Neighbors.Bottom != null)
            {
                if (checkNeigborIsBreakableObstacle(gc.Neighbors.Bottom)) return true;
            }

            return false;
        }

        public void cntMatchPottentials(DNA<char> p)
        {
            int minMatches = 3;
            grid.mgList = new List<MatchGroup>();

            grid.Rows.ForEach((br) =>
            {
                List<MatchGroup> mgList_t = br.getMatchableGroup(minMatches);

                for (int i = 0; i < mgList_t.Count; i++)
                {
                    grid.mgList.Add(mgList_t[i]);
                }
            });

            grid.Columns.ForEach((bc) =>
            {
                List<MatchGroup> mgList_t = bc.getMatchableGroup(minMatches);

                for (int i = 0; i < mgList_t.Count; i++)
                {
                    grid.mgList.Add(mgList_t[i]);
                }

            });

            p.matchFromMap = grid.mgList.Count;

            for (int i = 0; i < grid.mgList.Count; i++)
            {
                if (haveNeigborBreakableObstacle(grid.mgList[i].Cells[0]))
                {
                    p.nearBreakableObstacles++;
                    continue;
                }

                if (haveNeigborBreakableObstacle(grid.mgList[i].Cells[1]))
                {
                    p.nearBreakableObstacles++;
                    continue;
                }

                if (haveNeigborBreakableObstacle(grid.mgList[i].Cells[2]))
                {
                    p.nearBreakableObstacles++;
                }
            }

            for (int i = 0; i < grid.mgList.Count; i++)
            {
                if (grid.mgList[i].Cells[0].Overlay != null)
                {
                    p.includeMatchObstacles++;
                    continue;
                }

                if (haveNeigborBreakableObstacle(grid.mgList[i].Cells[1]))
                {
                    p.includeMatchObstacles++;
                    continue;
                }

                if (haveNeigborBreakableObstacle(grid.mgList[i].Cells[2]))
                {
                    p.includeMatchObstacles++;
                }
            }
        }







        public void cntPerPottentials(DNA<char> p)
        {
            MatchGroup mg = new MatchGroup();

            for (int i = 0; i < grid.Cells.Count; i++)
            {
                grid.Cells[i].cellPottential = new GridCell.CellPottential();
            }

            for (int i = 0; i < grid.Cells.Count; i++)
            {
                mg.countPottential(grid, i);
            }

            p.allPottential = new AllPottentials();

            for (int i = 0; i < grid.Cells.Count; i++)
            {
                p.allPottential.map += grid.Cells[i].cellPottential.map;
                p.allPottential.obstacle += grid.Cells[i].cellPottential.obstacle;
                p.allPottential.blocked1 += grid.Cells[i].cellPottential.blocked1;
                p.allPottential.blocked2 += grid.Cells[i].cellPottential.blocked2;
                p.allPottential.blocked3 += grid.Cells[i].cellPottential.blocked3;
                p.allPottential.blocked4 += grid.Cells[i].cellPottential.blocked4;
                p.allPottential.overlay1 += grid.Cells[i].cellPottential.overlay1;
                p.allPottential.overlay2 += grid.Cells[i].cellPottential.overlay2;
                p.allPottential.overlay3 += grid.Cells[i].cellPottential.overlay3;
                p.allPottential.overlay4 += grid.Cells[i].cellPottential.overlay4;
                p.allPottential.somethingWrong += grid.Cells[i].cellPottential.somethingWrong;
            }


            p.potttnennsss = 0;

            p.potttnennsss += p.allPottential.obstacle;
            p.potttnennsss += p.allPottential.blocked1;
            p.potttnennsss += p.allPottential.blocked2;
            p.potttnennsss += p.allPottential.blocked3;
            p.potttnennsss += p.allPottential.blocked4;
            p.potttnennsss += p.allPottential.overlay1;
            p.potttnennsss += p.allPottential.overlay2;
            p.potttnennsss += p.allPottential.overlay3;
            p.potttnennsss += p.allPottential.overlay4;

        }


        public void cntFinalPottential(DNA<char> p)
        {
            p.finalPottential = 0;

            //p.potttnennsss = 0;

            double map = 0;
            double obstacle = 0;
            double blocked1 = 0;
            double blocked2 = 0;
            double blocked3 = 0;
            double blocked4 = 0;
            double overlay1 = 0;
            double overlay2 = 0;
            double overlay3 = 0;
            double overlay4 = 0;
            double somethingWrong = 0;

            MatchGroup mg = new MatchGroup();

            for (int i = 0; i < grid.Cells.Count; i++)
            {
                grid.Cells[i].cellPottential = new GridCell.CellPottential();
            }

            for (int i = 0; i < grid.Cells.Count; i++)
            {
                mg.countPottential(grid, i);
            }

            for (int i = 0; i < grid.Cells.Count; i++)
            {
                map += grid.Cells[i].cellPottential.map;
                obstacle += grid.Cells[i].cellPottential.obstacle;
                blocked1 += grid.Cells[i].cellPottential.blocked1;
                blocked2 += grid.Cells[i].cellPottential.blocked2;
                blocked3 += grid.Cells[i].cellPottential.blocked3;
                blocked4 += grid.Cells[i].cellPottential.blocked4;
                overlay1 += grid.Cells[i].cellPottential.overlay1;
                overlay2 += grid.Cells[i].cellPottential.overlay2;
                overlay3 += grid.Cells[i].cellPottential.overlay3;
                overlay4 += grid.Cells[i].cellPottential.overlay4;
                somethingWrong += grid.Cells[i].cellPottential.somethingWrong;
            }


            //p.potttnennsss = 0;

            //p.potttnennsss += p.allPottential.obstacle;
            //p.potttnennsss += p.allPottential.blocked1;
            //p.potttnennsss += p.allPottential.blocked2;
            //p.potttnennsss += p.allPottential.blocked3;
            //p.potttnennsss += p.allPottential.blocked4;
            //p.potttnennsss += p.allPottential.overlay1;
            //p.potttnennsss += p.allPottential.overlay2;
            //p.potttnennsss += p.allPottential.overlay3;
            //p.potttnennsss += p.allPottential.overlay4;


            //p.finalPottential = p.potttnennsss;

            if (csvFolder <= 2) p.finalPottential = blocked1;

            else if (csvFolder <= 5) p.finalPottential = blocked2;

            else if (csvFolder <= 8) p.finalPottential = blocked3;

            else p.finalPottential = blocked4;
        }



        List<MatchGroup> mgList_t;

        public void createMatchGroups(int minMatches, bool estimate, MatchGrid grid)
        {
            //l_mgList = new List<MatchGroup>();
            grid.mgList = null;
            grid.mgList = new List<MatchGroup>();
            if (!estimate)
            {
                foreach (var br in grid.Rows)
                {
                    mgList_t = null;
                    mgList_t = br.GetMatches(minMatches, false);

                    //List<MatchGroup> mgList_t = br.GetMatches(minMatches, false);
                    if (mgList_t != null && mgList_t.Count > 0)
                    {
                        addRange(mgList_t, grid);
                    }
                }

                foreach (var bc in grid.Columns)
                {
                    mgList_t = null;
                    mgList_t = bc.GetMatches(minMatches, false);
                    //List<MatchGroup> mgList_t = bc.GetMatches(minMatches, false);
                    if (mgList_t != null && mgList_t.Count > 0)
                    {
                        addRange(mgList_t, grid);
                    }
                }
            }
            else
            {
                mgList_t = null;
                mgList_t = new List<MatchGroup>();
                //List<MatchGroup> mgList_t = new List<MatchGroup>();
                foreach (var gr in grid.Rows)
                {
                    mgList_t.AddRange(gr.GetMatches(minMatches, true));
                }
                foreach (var mg in mgList_t)
                {
                    if (mg.IsEstimateMatch(mg.Length, true, grid))
                    {
                        addEstimate(mg, grid);
                    }
                }

                mgList_t = null;
                mgList_t = new List<MatchGroup>();
                foreach (var gc in grid.Columns)
                {
                    mgList_t.AddRange(gc.GetMatches(minMatches, true));
                }
                foreach (var mg in mgList_t)
                {
                    if (mg.IsEstimateMatch(mg.Length, false, grid))
                    {
                        addEstimate(mg, grid);
                    }
                }
            }





            ////l_mgList = new List<MatchGroup>();
            //grid.mgList = new List<MatchGroup>();
            //if (!estimate)
            //{
            //    grid.Rows.ForEach((br) =>
            //    {
            //        List<MatchGroup> mgList_t = br.GetMatches(minMatches, false);
            //        if (mgList_t != null && mgList_t.Count > 0)
            //        {
            //            addRange(mgList_t, grid);
            //        }
            //    });

            //    grid.Columns.ForEach((bc) =>
            //    {
            //        List<MatchGroup> mgList_t = bc.GetMatches(minMatches, false);
            //        if (mgList_t != null && mgList_t.Count > 0)
            //        {
            //            addRange(mgList_t, grid);
            //        }
            //    });
            //}
            //else
            //{
            //    List<MatchGroup> mgList_t = new List<MatchGroup>();
            //    grid.Rows.ForEach((gr) =>
            //    {
            //        mgList_t.AddRange(gr.GetMatches(minMatches, true));
            //    });
            //    mgList_t.ForEach((mg) => { if (mg.IsEstimateMatch(mg.Length, true, grid)) { addEstimate(mg, grid); } });

            //    mgList_t = new List<MatchGroup>();
            //    grid.Columns.ForEach((gc) =>
            //    {
            //        mgList_t.AddRange(gc.GetMatches(minMatches, true));
            //    });
            //    mgList_t.ForEach((mg) => { if (mg.IsEstimateMatch(mg.Length, false, grid)) { addEstimate(mg, grid); } });
            //}
        }

        private MatchGroup merge(List<MatchGroup> intersections)
        {

            MatchGroup mG = new MatchGroup();
            intersections.ForEach((ints) => { mG.Merge(ints); });
            return mG;
        }

        public void add(MatchGroup mG, MatchGrid grid)
        {
            List<MatchGroup> intersections = new List<MatchGroup>();

            for (int i = 0; i < grid.mgList.Count; i++)
            {
                if (grid.mgList[i].IsIntersectWithGroup(mG))
                {
                    intersections.Add(grid.mgList[i]);
                }
            }
            // merge intersections
            if (intersections.Count > 0)
            {
                intersections.ForEach((ints) => { grid.mgList.Remove(ints); });
                intersections.Add(mG);
                grid.mgList.Add(merge(intersections));
            }
            else
            {
                grid.mgList.Add(mG);
            }
        }
        public void addRange(List<MatchGroup> mGs, MatchGrid grid)
        {
            for (int i = 0; i < mGs.Count; i++)
            {
                add(mGs[i], grid);
            }
        }
        public void new_addEstimate(MatchGroup mGe, List<MatchGroup> lmgList)
        {
            for (int i = 0; i < lmgList.Count; i++)
            {
                if (lmgList[i].IsEqual(mGe))
                {
                    return;
                }
            }
            lmgList.Add(mGe);
        }

        public void addEstimate(MatchGroup mGe, MatchGrid grid)
        {
            for (int i = 0; i < grid.mgList.Count; i++)
            {
                if (grid.mgList[i].IsEqual(mGe))
                {
                    return;
                }
            }
            grid.mgList.Add(mGe);
        }

        public void cancelTweens(MatchGrid g)
        {
            g.mgList.ForEach((mg) => { cancelTween1(g); });
        }

        public void cancelTween1(MatchGrid g)
        {
            g.Cells.ForEach((c) => { c.CancelTween(); });
        }

        //Map map1;
        //PathFinder pF1;
        //List<GridCell> path1;

        //Map map;
        //PathFinder pF;
        //List<GridCell> path;






        // pF = new PathFinder();


        Map map;
        PathFinder pF;




        public void new_createFillPath111111111()
        {
            map = new Map(grid);
            pF = new PathFinder();


            for (int i = 0; i < grid.Cells.Count; i++)
            {
                if (!grid.Cells[i].Blocked && !grid.Cells[i].IsDisabled && !grid.Cells[i].MovementBlocked)
                {
                    grid.Cells[i].isVisit = false;
                }

                else grid.Cells[i].isVisit = true;
            }

            for (int i = 0; i < grid.Rows[0].Length; i++)
            {
                grid.Rows[0].cells[i].fillPathToSpawner = null;
                grid.Rows[0].cells[i].isVisit = true;
            }


            for (int i = 0; i < grid.Rows[0].Length; i++)
            {
                for (int j = 1; j < grid.Rows.Count; j++)
                {
                    if (!grid.Cells[i].Blocked && !grid.Cells[i].IsDisabled && !grid.Cells[i].MovementBlocked)
                    {
                        grid.Rows[j].cells[i].fillPathToSpawner = new List<GridCell>();

                        grid.Rows[j].cells[i].fillPathToSpawner.Add(grid.Rows[j - 1].cells[i]);

                        if (grid.Rows[j - 1].cells[i].fillPathToSpawner != null)
                        {
                            foreach (var v in grid.Rows[j - 1].cells[i].fillPathToSpawner)
                            {
                                grid.Rows[j].cells[i].fillPathToSpawner.Add(v);
                            }
                        }
                        grid.Rows[j].cells[i].isVisit = true;
                    }

                    else break;
                }
            }

            for (int i = 0; i < grid.Rows[0].Length; i++)
            {
                for (int j = 1; j < grid.Rows.Count; j++)
                {
                    if (!grid.Rows[j].cells[i].isVisit)
                    {
                        int leftSize = 0;
                        int RightSize = 0;

                        if (grid.Rows[j].cells[i].Neighbors.Left != null)
                        {
                            if (grid.Rows[j].cells[i].Neighbors.Left.fillPathToSpawner != null)
                            {
                                leftSize = grid.Rows[j].cells[i].Neighbors.Left.fillPathToSpawner.Count();
                            }
                        }

                        if (grid.Rows[j].cells[i].Neighbors.Right != null)
                        {
                            if (grid.Rows[j].cells[i].Neighbors.Right.fillPathToSpawner != null)
                            {
                                RightSize = grid.Rows[j].cells[i].Neighbors.Right.fillPathToSpawner.Count();
                            }
                        }

                        if (leftSize == 0 && RightSize == 0)
                        {
                        }

                        else
                        {
                            grid.Rows[j].cells[i].fillPathToSpawner = new List<GridCell>();

                            if (leftSize > RightSize)
                            {
                                grid.Rows[j].cells[i].fillPathToSpawner.Add(grid.Rows[j].cells[i].Neighbors.Left);

                                if (grid.Rows[j].cells[i].Neighbors.Left.fillPathToSpawner != null)
                                {
                                    foreach (var v in grid.Rows[j].cells[i].Neighbors.Left.fillPathToSpawner)
                                    {
                                        grid.Rows[j].cells[i].fillPathToSpawner.Add(v);
                                    }
                                }
                                grid.Rows[j].cells[i].isVisit = true;
                            }

                            else if (leftSize < RightSize)
                            {
                                grid.Rows[j].cells[i].fillPathToSpawner.Add(grid.Rows[j].cells[i].Neighbors.Right);

                                if (grid.Rows[j].cells[i].Neighbors.Right.fillPathToSpawner != null)
                                {
                                    foreach (var v in grid.Rows[j].cells[i].Neighbors.Right.fillPathToSpawner)
                                    {
                                        grid.Rows[j].cells[i].fillPathToSpawner.Add(v);
                                    }
                                }
                                grid.Rows[j].cells[i].isVisit = true;
                            }

                            else
                            {
                                if(i < grid.Rows.Count - i)
                                {
                                    grid.Rows[j].cells[i].fillPathToSpawner.Add(grid.Rows[j].cells[i].Neighbors.Right);

                                    if (grid.Rows[j].cells[i].Neighbors.Right.fillPathToSpawner != null)
                                    {
                                        foreach (var v in grid.Rows[j].cells[i].Neighbors.Right.fillPathToSpawner)
                                        {
                                            grid.Rows[j].cells[i].fillPathToSpawner.Add(v);
                                        }
                                    }
                                    grid.Rows[j].cells[i].isVisit = true;
                                }


                                else
                                {
                                    grid.Rows[j].cells[i].fillPathToSpawner.Add(grid.Rows[j].cells[i].Neighbors.Left);

                                    if (grid.Rows[j].cells[i].Neighbors.Left.fillPathToSpawner != null)
                                    {
                                        foreach (var v in grid.Rows[j].cells[i].Neighbors.Left.fillPathToSpawner)
                                        {
                                            grid.Rows[j].cells[i].fillPathToSpawner.Add(v);
                                        }
                                    }
                                    grid.Rows[j].cells[i].isVisit = true;
                                }
                            }
                            grid.Rows[j].cells[i].Neighbors.bottomFill(grid.Rows[j].cells[i]);
                        }
                    }
                }
            }

            bool allVisit = false;
            while (allVisit)
            {
                allVisit = true;
                foreach (var c in grid.Cells)
                {
                    if (!c.isVisit)
                    {
                        c.findingPath();

                        //c.Neighbors.findFillPath(c);
                        allVisit = false;
                    }
                }
            }

            //foreach (var c in grid.Cells)
            //{
            //    if(c.fillPathToSpawner != null)
            //    {
            //        c.fillPathToSpawner.Reverse();
            //    }
            //}
        }




        //foreach (var c in grid.Cells)
        //{
        //    if (!c.isVisit)
        //    {
        //        List<GridCell> res = c.Neighbors.HaveFillPath();

        //        if (res != null)
        //        {
        //            c.fillPathToSpawner = res;
        //            c.isVisit = true;
        //        }
        //    }
        //}

        //int count = 0;

        //foreach (var c in grid.Cells)
        //{
        //    if (!c.isVisit) count++;
        //}

        //for (int i = 0; i < grid.Columns[0].Length; i++)
        //{
        //    for (int j = 1; j < grid.Columns.Count; j++)
        //    {
        //        if()


        //        if(i != 0)
        //        {
        //            grid.Columns[j].cells[i].Neighbors.Left.isVisit = false;
        //        }
        //    }
        //}


        //        for (int i = 0; i < grid.Cells.Count; i++)
        //{
        //    if(!grid.Cells[i].isVisit)
        //    {
        //        grid.Cells[i].Neighbors.HaveFillPath();
        //    }



        //}






          bool isFirst = true;


        public int maxLength = 0;
        public int cnt = 0;



        //public void new_createFillPath()
        //{

        //    //Map map = new Map(grid);
        //    //PathFinder pF = new PathFinder();

        //    //map = null;
        //    map = new Map(grid);
        //    //pF = null;
        //    pF = new PathFinder();



        //    //if (isFirst)
        //    //{
        //    //    map = new Map(grid);
        //    //    pF = new PathFinder();
        //    //    isFirst = false;
        //    //}

        //    //else
        //    //{
        //    //    map.Dispose();
        //    //    map = new Map(grid);

        //    //    pF.Dispose();
        //    //    pF = new PathFinder();
        //    //}
        //    //pF = null;
        //    //map = null;


        //    cnt = 0;
        //    maxLength = 0;

        //    int size = 0;



        //    foreach (var c in grid.Cells)
        //    {
        //        if (knwoSpawnRootSize) size = spawnRootSize[cnt];


        //        if (!c.Blocked && !c.IsDisabled && !c.MovementBlocked)
        //        {
        //            int length = int.MaxValue;
        //            List<GridCell> path = null;

        //            foreach (var col in grid.Columns)
        //            {
        //                if (col.Spawn)
        //                {
        //                    if (col.Spawn.gridCell != c)
        //                    {
        //                        pF.new_CreatePath(map, c.pfCell, col.Spawn.gridCell.pfCell, size);


        //                        if (pF.PathLenght != int.MaxValue && pF.PathLenght > maxLength)
        //                        {
        //                            if(pF.PathLenght != int.MaxValue && pF.PathLenght > 2000)
        //                            {
        //                                maxLength = pF.PathLenght;
        //                            }
        //                        }


        //                        if (pF.FullPath != null && pF.PathLenght < length)
        //                        {
        //                            path = null;
        //                            path = pF.GCPath();
        //                            length = pF.PathLenght;
        //                        }
        //                    }

        //                    else
        //                    {
        //                        length = 0;
        //                        //path = new List<GridCell>();

        //                        path = null;


        //                        //int count = (int)c.cellPottential.map;

        //                        path = new List<GridCell>(50);

        //                        //if (cnt > 70)
        //                        //{
        //                        //    path = new List<GridCell>(20);
        //                        //}

        //                        //else if (cnt > 30)
        //                        //{
        //                        //    path = new List<GridCell>(10);
        //                        //}

        //                        //else path = new List<GridCell>();
        //                    }  
        //                }
        //            }

        //            c.fillPathToSpawner = null;
        //            c.fillPathToSpawner = path;

        //        }

        //        cnt++;
        //    }


        //    //map.Dispose();
        //    //pF.Dispose();






        //    //using (pF = new PathFinder())
        //    //{
        //    //    // myObject를 사용한 후, using 블록을 빠져나가면 Dispose가 호출됨
        //    //}

        //    //map1 = new Map(grid);
        //    //pF1 = new PathFinder();

        //    //foreach (var c in grid.Cells)
        //    //{
        //    //    if (!c.Blocked && !c.IsDisabled && !c.MovementBlocked)
        //    //    {
        //    //        int length = int.MaxValue;
        //    //        path1 = null;

        //    //        foreach (var col in grid.Columns)
        //    //        {
        //    //            if (col.Spawn)
        //    //            {
        //    //                if (col.Spawn.gridCell != c)
        //    //                {
        //    //                    pF1.new_CreatePath(map1, c.pfCell, col.Spawn.gridCell.pfCell);

        //    //                    //pF.CreatePathToTop(map, c.pfCell);

        //    //                    if (pF1.FullPath != null && pF1.PathLenght < length)
        //    //                    {
        //    //                        path1 = pF1.GCPath();
        //    //                        length = pF1.PathLenght;
        //    //                    }
        //    //                }
        //    //                else
        //    //                {
        //    //                    length = 0;
        //    //                    path1 = new List<GridCell>();
        //    //                }
        //    //            }
        //    //        }

        //    //        c.fillPathToSpawner = path1;
        //    //    }
        //    //}


        //    ////Debug.Log("mh Make gravity fill path");
        //    //Map map = new Map(grid);
        //    //PathFinder pF = new PathFinder();

        //    //grid.Cells.ForEach((c) =>
        //    //{
        //    //    //if (!c.Blocked && !c.IsDisabled && !c.MovementBlocked && !c.Overlay)
        //    //    if (!c.Blocked && !c.IsDisabled && !c.MovementBlocked)
        //    //    {
        //    //        int length = int.MaxValue;
        //    //        List<GridCell> path = null;
        //    //        grid.Columns.ForEach((col) =>
        //    //        {
        //    //            if (col.Spawn)
        //    //            {
        //    //                if (col.Spawn.gridCell != c)
        //    //                {
        //    //                    pF.new_CreatePath(map, c.pfCell, col.Spawn.gridCell.pfCell);

        //    //                    //pF.CreatePathToTop(map, c.pfCell);

        //    //                    if (pF.FullPath != null && pF.PathLenght < length)
        //    //                    {
        //    //                        path = pF.GCPath(); 
        //    //                        length = pF.PathLenght;
        //    //                    }
        //    //                }
        //    //                else
        //    //                {
        //    //                    length = 0;
        //    //                    path = new List<GridCell>();
        //    //                }
        //    //            }
        //    //        });
        //    //        c.fillPathToSpawner = path;
        //    //    }
        //    //});

        //}

        //Map map;
        //PathFinder pF ;
        //List<GridCell> path;


        //public void new_createFillPathTOFill(int min)
        //{
        //    map = new Map(grid);
        //    pF = new PathFinder();
        //    path = null;

        //    foreach (var c in grid.Cells)
        //    {
        //        if (!c.Blocked && !c.IsDisabled && !c.MovementBlocked)
        //        {
        //            int length = int.MaxValue;
        //            path = null;

        //            foreach (var col in grid.Columns)
        //            {

        //                if (col.Spawn)
        //                {
        //                    if (col.Spawn.gridCell != c)
        //                    {
        //                        pF.new_CreatePath(map, c.pfCell, col.Spawn.gridCell.pfCell);

        //                        if (pF.FullPath != null && pF.PathLenght < length)
        //                        {
        //                            path = pF.GCPath();
        //                            length = pF.PathLenght;
        //                        }
        //                    }
        //                    else
        //                    {
        //                        length = 0;
        //                        //path = new List<GridCell>();
        //                        path = null;
        //                    }
        //                }
        //            }

        //            c.fillPathToSpawner = path;
        //        }
        //    }


        //    //    grid.Cells.ForEach((c) =>
        //    //{
        //    //    //if (!c.Blocked && !c.IsDisabled && !c.MovementBlocked && !c.Overlay)
        //    //    if (!c.Blocked && !c.IsDisabled && !c.MovementBlocked)
        //    //    {
        //    //        int length = int.MaxValue;
        //    //        List<GridCell> path = null;





        //    //        grid.Columns.ForEach((col) =>
        //    //        {
        //    //            if (col.Spawn || col.Column)
        //    //            {
        //    //                if (col.Spawn.gridCell != c)
        //    //                {
        //    //                    pF.new_CreatePath(map, c.pfCell, col.Spawn.gridCell.pfCell);

        //    //                    //pF.CreatePathToTop(map, c.pfCell);

        //    //                    if (pF.FullPath != null && pF.PathLenght < length)
        //    //                    {
        //    //                        path = pF.GCPath();
        //    //                        length = pF.PathLenght;
        //    //                    }
        //    //                }
        //    //                else
        //    //                {
        //    //                    length = 0;
        //    //                    path = new List<GridCell>();
        //    //                }
        //    //            }
        //    //        });
        //    //        c.fillPathToSpawner = path;
        //    //    }
        //    //});

        //}


        public void CreateFillPath(MatchGrid g)
        {
            Map map = new Map(g);
            PathFinder pF = new PathFinder();

            int cnt = 0;

            foreach (var c in g.Cells)
            {
                c.fillPathToSpawner = null;
            }


            foreach (var c in g.Cells)
            {

                if (!c.spawner)
                {
                    if (!c.Blocked && !c.IsDisabled && !c.MovementBlocked)
                    {
                        //Debug.Log("Col" + c.Column + "Row" + c.Row);

                        Stack<GridCell> path = new Stack<GridCell>();

                        map.ResetPath();
                        pF.CreatePathes(map, c.pfCell, c.pfCell, path);

                        //if (c.fillPathToSpawner != null) cnt++;

                    }
                }



                //if (c.fillPathToSpawner != null) c.fillPathToSpawner.Reverse();
                //c.fillPathToSpawner = c.fillPathToSpawner;

            }




            //Map map = new Map(g);
            //PathFinder pF = new PathFinder();


            //foreach (var c in g.Cells)
            //{
            //    if (!c.Blocked && !c.IsDisabled && !c.MovementBlocked)
            //    {
            //        int length = int.MaxValue;
            //        List<GridCell> path = null;


            //        foreach (var col in g.Columns)
            //        {
            //            if (col.Spawn)
            //            {
            //                if (col.Spawn.gridCell != c)
            //                {
            //                    pF.CreatePath(map, c.pfCell, col.Spawn.gridCell.pfCell);

            //                    if (pF.FullPath != null && pF.PathLenght < length)
            //                    {
            //                        path = pF.GCPath();
            //                        length = pF.PathLenght;
            //                    }
            //                }
            //                else
            //                {
            //                    length = 0;
            //                    //path = new List<GridCell>();
            //                    path = null;
            //                }
            //            }
            //        }

            //        c.fillPathToSpawner = path;
            //    }
            //}
        }






        public void new_fillGridByStep(List<GridCell> freeCells)
        {
            if (freeCells.Count == 0) return;

            foreach (GridCell gc in freeCells) gc.new_fillGrab();
        }


        public void mixGrid(Action completeCallBack, MatchGrid grid, Transform trans)
        {
            List<GridCell> cellList = new List<GridCell>();
            List<GridCell> cellList2 = new List<GridCell>();

            for(int i =0;i < grid.Cells.Count;i++) 
            {
                if (grid.Cells[i].IsMixable)
                {
                    cellList.Add(grid.Cells[i]);
                    cellList2.Add(grid.Cells[i]);
                }
            }

            for (int i = 0; i < cellList.Count; i++)
            {
                int random = UnityEngine.Random.Range(0, cellList2.Count);

                List<int> id = cellList[i].GetGridObjectsIDs();
                List<int> id2 = cellList2[random].GetGridObjectsIDs();

                cellList[i].DestroyObjects();
                cellList2[random].DestroyObjects();

                switch (id2[0])
                {
                    case 1000:
                        cellList[i].poolingmatchObjects[0].gameObject.SetActive(true);
                        break;
                    case 1001:
                        cellList[i].poolingmatchObjects[1].gameObject.SetActive(true);
                        break;
                    case 1002:
                        cellList[i].poolingmatchObjects[2].gameObject.SetActive(true);
                        break;
                    case 1003:
                        cellList[i].poolingmatchObjects[3].gameObject.SetActive(true);
                        break;
                    case 1004:
                        cellList[i].poolingmatchObjects[4].gameObject.SetActive(true);
                        break;
                    case 1005:
                        cellList[i].poolingmatchObjects[5].gameObject.SetActive(true);
                        break;
                    case 1006:
                        cellList[i].poolingmatchObjects[6].gameObject.SetActive(true);
                        break;
                }
 
                switch (id[0])
                {
                    case 1000:
                        cellList2[random].poolingmatchObjects[0].gameObject.SetActive(true);
                        break;
                    case 1001:
                        cellList2[random].poolingmatchObjects[1].gameObject.SetActive(true);
                        break;
                    case 1002:
                        cellList2[random].poolingmatchObjects[2].gameObject.SetActive(true);
                        break;
                    case 1003:
                        cellList2[random].poolingmatchObjects[3].gameObject.SetActive(true);
                        break;
                    case 1004:
                        cellList2[random].poolingmatchObjects[4].gameObject.SetActive(true);
                        break;
                    case 1005:
                        cellList2[random].poolingmatchObjects[5].gameObject.SetActive(true);
                        break;
                    case 1006:
                        cellList2[random].poolingmatchObjects[6].gameObject.SetActive(true);
                        break;
                }

                cellList2.RemoveAt(random);
            }




            //ParallelTween pT0 = new ParallelTween();
            //ParallelTween pT1 = new ParallelTween();

                //TweenSeq tweenSeq = new TweenSeq();
                //List<GridCell> cellList = new List<GridCell>();
                //List<GameObject> goList = new List<GameObject>();

                //cancelTweens(grid);


                //grid.Cells.ForEach((c) => { if (c.IsMixable) { cellList.Add(c); goList.Add(c.DynamicObject); } });

                //cellList.ForEach((c) =>
                //{
                //    int random = UnityEngine.Random.Range(0, goList.Count);
                //    GameObject m = goList[random];
                //    pT1.Add((callBack) => { c.GrabDynamicObject1(m.gameObject, false, callBack); });
                //    goList.RemoveAt(random);
                //});

                //tweenSeq.Add((callBack) =>
                //{
                //    pT0.Start(callBack);
                //});

                //tweenSeq.Add((callBack) =>
                //{
                //    pT1.Start(() =>
                //    {
                //        callBack();
                //    });
                //});
                //tweenSeq.Start();
        }

    }


    public class Limit
    {
        public int geneticGeneration;
        public int generation;
        public int find;
        public int repeat;
        public int move;
        public int mix;
        public int match3Cycle;
        public int csvCnt;
    }

    public class PlayHelper
    {
        public bool isClear;
        public bool isError;
        public int curState;
        public int playCnt;
        public int fillGridCnt;
        public int mixGridCnt;
        public int findMatchCnt;

        public PlayHelper()
        {
            isClear = false;
            isError = false;
            curState = 0;
            playCnt = 0;

            fillGridCnt = 0;
            mixGridCnt = 0;
            findMatchCnt = 0;
        }

    }
}
