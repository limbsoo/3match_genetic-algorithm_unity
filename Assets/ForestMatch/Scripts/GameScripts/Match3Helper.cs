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

            csvCnt = 0;
            match3Cycle = 0;

            isOnce = false;
            getSetGenes = false;

            divideSpecificBlock = false;

            if (isOnce && getSetGenes)
            {
                limits.match3Cycle = 1;
                limits.generation = 1;
                limits.csvCnt = 0;
            }

            else if(!isOnce && getSetGenes)
            {
                limits.match3Cycle = 15;
                limits.generation = 1;
                limits.csvCnt = 9;
            }

            else
            {
                limits.match3Cycle = 50;
                limits.generation = 100;
                limits.csvCnt = 29;


                ////limits.match3Cycle = 1;
                ////limits.generation = 1;
                ////limits.csvCnt = 0;
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
            wantDifficulty = 1144;
            difficultyTolerance = 50;
            minusRange = 110;
            originPoten = 1144;

            ////size 1111
            //wantDifficulty = 1432;
            //difficultyTolerance = 30;
            //minusRange = 140;
            //originPoten = 1432;

















            //if (getSetGenes) setPottentials();

            //blockedObjectHitCnt = 0;
            //overlayObjectHitCnt = 0;

            //isActualMeasureSwap = false;

            //spawnObstacleObject = true;
            //spawnBlockedObject = true;
            //spawnOverlayObject = false;
            //haveRandomProtection = true;
            //blockProtection = 3;






            //spawnObstacleObject = false;
            //spawnBlockedObject = false;
            //spawnOverlayObject = true;
            //haveRandomProtection = false;
            //blockProtection = 3;

            //wantDifficulty = 1300;
            //difficultyTolerance = 50;
            //minusRange = 120;
            //originPoten = 1300;

            //wantDifficulty = 800;
            //difficultyTolerance = 50;
            //minusRange = 70;
            //originPoten = 800;

            numOfMatchBlock = 7; //GetRandomObjectPrefab 이거 지워서 가지수바꾸려면 바꿔야함


            // 0 600
            //1 550
            //2 500
            //3 450
            //4 400
            //5 350
            //6 300
            //7 250
            //8 200
            //9 150
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


        //public void setPottentials()
        //{
        //    difficults = new Difficult[10];

        //    string[] map =
        //    {
        //        "311333030011333333133303333333333331333333313333333333333333333333333333133333113" ,
        //        "333330303301333333333333333333310333333333333333333333331033333301033333333333330" ,
        //        "331333133133333330133300133330333331333303333333333333333333333333303333333333330" ,
        //        "033103030133333333331333333313033313333333333333333333333333330331310331131301333" ,
        //        "331113331313330333133333333333333331333130333333133333133333333030313333130313103" ,
        //        "313033103111333113333333311100333311130333333333333311113333333033303333103011333" ,
        //        "130303011333333333313303333333333333333133333330013013333313133030013333330101330" ,
        //        "000331101303333333333333003133330331133333313133331333333003333300333310331103000" ,
        //        "033310030030100333001111310033333333010333300133333103333333133030033333101003030" ,
        //        "333133133300333331103333101313333330303333331333301333131000300010333300010300311"
        //    };

        //    string[] masp =
        //        {
        //        "556	426	239	350	290	0	0	0	0	0" ,
        //        "519	586	323	406	476	0	0	0	0	0" ,
        //        "451	334	190	246	257	0	0	0	0	0" ,
        //        "404	385	115	237	238	0	0	0	0	0" ,
        //        "364	453	153	271	246	0	0	0	0	0" ,
        //        "307	368	156	196	289	0	0	0	0	0" ,
        //        "256	487	194	305	315	0	0	0	0	0" ,
        //        "202	412	189	269	305	0	0	0	0	0" ,
        //        "170	404	167	302	254	0	0	0	0	0" ,
        //        "103	402	118	250	235	0	0	0	0	0"
        //    };

        //    string[]protections =
        //    {
        //        "011000000032000000100000000000000001000000020000000000000000000000000000200000210" ,
        //        "000000000002000000000000000000030000000000000000000000003000000003000000000000000" ,
        //        "003000300200000000100000300000000002000000000000000000000000000000000000000000000" ,
        //        "000100000100000000001000000020000030000000000000000000000000000002030002203002000" ,
        //        "003130002020000000300000000000000003000300000000200000100000000000020000300010200" ,
        //        "010000200211000210000000011200000012100000000000000011310000000000000000300013000" ,
        //        "200000032000000000030000000000000000000100000000030030000010200000030000000102000" ,
        //        "000002102000000000000000000200000002300000020100003000000000000000000010001100000" ,
        //        "000030000000200000001332030000000000010000000200000100000000300000000000102000000" ,
        //        "000100100000000002300000303020000000000000003000003000201000000010000000020000022"
        //    };


        //    for (int i = 0; i < 10; i++)
        //    {
        //        Difficult difficlut = new Difficult();
        //        string s = "";
        //        int cnt = 0;

        //        for (int j = 0; j < masp[i].Length; j++)
        //        {
        //            if (masp[i][j] == '\t')
        //            {
        //                if (s != "")
        //                {
        //                    if (cnt == 0) difficlut.pottential = Convert.ToInt32(s);
        //                    else if (cnt == 1) difficlut.obstacle = Convert.ToInt32(s);

        //                    else if (cnt == 2) difficlut.blockedPottential = Convert.ToInt32(s);
        //                    else if (cnt == 3) difficlut.blocked1 = Convert.ToInt32(s);
        //                    else if (cnt == 4) difficlut.blocked2 = Convert.ToInt32(s);
        //                    else if (cnt == 5) difficlut.blocked3 = Convert.ToInt32(s);

        //                    else if (cnt == 6) difficlut.overlayPottential = Convert.ToInt32(s);
        //                    else if (cnt == 7) difficlut.overlay1 = Convert.ToInt32(s);
        //                    else if (cnt == 8) difficlut.overlay2 = Convert.ToInt32(s);
        //                    else if (cnt == 9) difficlut.overlay3 = Convert.ToInt32(s);

        //                    cnt++;
        //                }

        //                s = "";
        //            }

        //            else s += masp[i][j];
        //        }

        //        difficults[i] = difficlut;
        //    }


            

        //    for (int i = 0; i < map.Length; i++)
        //    {
        //        char[] genes = new char[gridSize];

        //        for (int j = 0; j < gridSize; j++)
        //        {
        //            genes[j] = map[i][j+81];
        //        }
  
        //        difficults[i].map = genes;
        //    }

        //    for (int i = 0; i < map.Length; i++)
        //    {
        //        char[] genes = new char[gridSize];

        //        for (int j = 0; j < gridSize; j++)
        //        {
        //            genes[j] = protections[i][j + 81];
        //        }

        //        difficults[i].protections = genes;
        //    }


        //}




        //public void setProtection(DNA<char> p)
        //{
        //    protections = new List<int>();

        //    for (int i = 0; i < p.cellsID.Count; i++)
        //    {
        //        if (difficults[csvCnt].protections[i] == '1')
        //        {
        //            protections.Add(1);
        //        }

        //        else if(difficults[csvCnt].protections[i] == '2')
        //        {
        //            protections.Add(2);
        //        }

        //        else if (difficults[csvCnt].protections[i] == '3')
        //        {
        //            protections.Add(3);
        //        }

        //        else if (difficults[csvCnt].protections[i] == '0')
        //        {
        //            protections.Add(0);
        //        }


        //        //if (p.cellsID[i] == 0 || p.cellsID[i] == 3) protections.Add(0);

        //        //else
        //        //{
        //        //    if(haveRandomProtection)
        //        //    {
        //        //        int randomProtection = Random.Range(1, blockProtection + 1);
        //        //        protections.Add(randomProtection);
        //        //    }

        //        //   else protections.Add(blockProtection);

        //        //}

        //    }
           

        //}



        internal List<GridCell> GetFreeCells(MatchGrid g, bool withPath)
        {
            List<GridCell> gcL = new List<GridCell>();
            for (int i = 0; i < g.Cells.Count; i++)
            {
                if (g.Cells[i].IsDynamicFree && !g.Cells[i].Blocked && !g.Cells[i].IsDisabled)
                {
                    gcL.Add(g.Cells[i]);
                }
            }
            return gcL;
        }


        //-- fillFreeCells ------------------------------------------------------------------------//
        public void fillFreeCells()
        {
            List<GridCell> gFreeCells = new List<GridCell>();
            gFreeCells = GetFreeCells(grid, true);
            //if (gFreeCells.Count > 0) createFillPath(grid);
            if (gFreeCells.Count > 0)
            {
                new_createFillPath();
            }
            

            while (gFreeCells.Count > 0)
            {
                new_fillGridByStep(gFreeCells);
                gFreeCells = GetFreeCells(grid, true);
                plays.fillGridCnt++;

                if (estimateMax(plays.fillGridCnt, limits.find)) return;
            }

            estimateClear();

            plays.fillGridCnt = 0;
            plays.curState = 2;
        }

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
                List<int> predictMatch = new List<int>();
                List<int> predictObstacle = new List<int>();


                for (int i = 0; i < grid.mgList.Count; i++)
                {
                    int predictCnt = 0;
                    predictCnt += estimateIncludeTarget(grid.mgList[i]);
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

                    if(obstacleMax == 0)
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

            if (g != null)
            {
                if (g.Blocked != null)
                {
                    if (!g.Blocked.Destroyable) return false;

                    else return true;
                }
                 
                else if (g.Overlay != null) return true;

                else return false;
            }

            return false;
        }




        //-- swapCells --////////////////////////////////////////////////////////////////////////
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
        public void matchAndDestory(DNA<char> p)
        {
            if (estimateMax(plays.findMatchCnt, limits.find)) return;

            //if (grid.GetFreeCells(true).Count > 0)
            //{
            //    plays.curState = 0;
            //    return;
            //}

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
                                List<int> mgCells = grid.mgList[i].Cells[j].GetGridObjectsIDs();

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
                                    if(grid.mgList[i].Cells[j].Overlay != null)
                                    {
                                        grid.mgList[i].Cells[j].Overlay.hitCnt++;

                                        if (grid.mgList[i].Cells[j].setProtection <= grid.mgList[i].Cells[j].Overlay.hitCnt)
                                        {
                                            grid.mgList[i].Cells[j].DestroyGridObjects();
                                        }
                                    }
                                }


                                grid.mgList[i].Cells[j].DestroyGridObjects();
                            }
                        }
                    }
                }

                p.matchCnt++;
                plays.curState = 0;
            }
        }


        public void destoryNeigborObstacle(GridCell gc)
        {
            if (gc != null && gc.Blocked != null)
            {
                if (gc.Blocked.Destroyable)
                {
                    gc.Blocked.hitCnt++;
                    if (gc.setProtection <= gc.Blocked.hitCnt) gc.DestroyGridObjects();
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
                if(checkNeigborIsBreakableObstacle(gc.Neighbors.Top)) return true;
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
        }



        //public void cntMapPottentials(DNA<char> p)
        //{
        //    MatchGroup mg = new MatchGroup();

        //    for (int i = 0; i < grid.Cells.Count; i++)
        //    {
        //        grid.Cells[i].matchFromSwapPotential = 0;
        //        grid.Cells[i].obstacle = 0;
        //        grid.Cells[i].blocked1 = 0;
        //        grid.Cells[i].blocked2 = 0;
        //        grid.Cells[i].blocked3 = 0;
        //        grid.Cells[i].overlay1 = 0;
        //        grid.Cells[i].overlay2 = 0;
        //        grid.Cells[i].overlay3 = 0;
        //        grid.Cells[i].somethingWrong = 0;
        //    }

        //    for (int i = 0; i < grid.Cells.Count; i++)
        //    {
        //        mg.cntMapPerPottentials(grid, i);
        //    }

        //    int mapMatchPotential = 0;
        //    int obstacle = 0;
        //    int blocked1 = 0;
        //    int blocked2 = 0;
        //    int blocked3 = 0;
        //    int overlay1 = 0;
        //    int overlay2 = 0;
        //    int overlay3 = 0;
        //    int somethingWrong = 0;


        //    for (int i = 0; i < grid.Cells.Count; i++)
        //    {
        //        mapMatchPotential += grid.Cells[i].matchFromSwapPotential;

        //        obstacle += grid.Cells[i].obstacle;
        //        blocked1 += grid.Cells[i].blocked1;
        //        blocked2 += grid.Cells[i].blocked2;
        //        blocked3 += grid.Cells[i].blocked3;
        //        overlay1 += grid.Cells[i].overlay1;
        //        overlay2 += grid.Cells[i].overlay2;
        //        overlay3 += grid.Cells[i].overlay3;
        //        somethingWrong += grid.Cells[i].somethingWrong;
        //    }

        //    //p.mapObstacle = obstacle;
        //    //p.mapBlocked1 = blocked1;
        //    //p.mapBlocked2 = blocked2;
        //    //p.mapBlocked3 = blocked3;
        //    //p.mapOverlay1 = overlay1;
        //    //p.mapOverlay2 = overlay2;
        //    //p.mapOverlay3 = overlay3;
        //    //p.mapSomethingWrong = somethingWrong;
        //}





















































        public void cntMapmatchPottential(DNA<char> p)
        {
            MatchGroup mg = new MatchGroup();

            for (int i = 0; i < grid.Cells.Count; i++)
            {
                grid.Cells[i].matchFromSwapPotential = 0;
                grid.Cells[i].obstacle = 0;
                grid.Cells[i].blocked1 = 0;
                grid.Cells[i].blocked2 = 0;
                grid.Cells[i].blocked3 = 0;
                grid.Cells[i].overlay1 = 0;
                grid.Cells[i].overlay2 = 0;
                grid.Cells[i].overlay3 = 0;
                grid.Cells[i].somethingWrong = 0;
            }


            for (int i = 0; i < grid.Cells.Count; i++)
            {
                mg.cntPerPottentials(grid, i);
            }

            int mapMatchPotential = 0;
            List<int> mapMatchPotentialList = new List<int>();
            int obstacleCnt = 0;

            for (int i = 0; i < grid.Cells.Count; i++)
            {
                mapMatchPotentialList.Add(grid.Cells[i].matchFromSwapPotential);
                mapMatchPotential += grid.Cells[i].matchFromSwapPotential;

                if (grid.Cells[i].Blocked != null || grid.Cells[i].Overlay != null) obstacleCnt++;
            }

            //p.pottential = new Pottential();
            //p.pottential.map = mapMatchPotential;
            //p.mapMatchPotentialList = mapMatchPotentialList;
            //p.obstacleCnt = obstacleCnt;


            //p.originMapPottential = new OriginMapPottential();

            //MatchGroup mg = new MatchGroup();

            //for (int i = 0; i < grid.Cells.Count; i++)
            //{
            //    grid.Cells[i].matchFromSwapPotential = 0;
            //    mg.cntPottential(grid, i);
            //}

            //int mapMatchPotential = 0;
            //List<int> mapMatchPotentialList = new List<int>();
            //int obstacleCnt = 0;

            //for (int i = 0; i < grid.Cells.Count; i++)
            //{
            //    mapMatchPotentialList.Add(grid.Cells[i].matchFromSwapPotential);
            //    mapMatchPotential += grid.Cells[i].matchFromSwapPotential;

            //    if (grid.Cells[i].Blocked != null || grid.Cells[i].Overlay != null) obstacleCnt++;
            //}

            //p.pottential = new Pottential();

            //p.pottential.map = mapMatchPotential;
            //p.mapMatchPotentialList = mapMatchPotentialList;
            //p.obstacleCnt = obstacleCnt;
        }

        //public int cntPottentials(DNA<char> p)
        //{
        //    MatchGroup mg = new MatchGroup();

        //    for (int i = 0; i < grid.Cells.Count; i++)
        //    {
        //        grid.Cells[i].matchFromSwapPotential = 0;
        //    }

        //    for (int i = 0; i < grid.Cells.Count; i++)
        //    {
        //        mg.cntPottential(grid, i);
        //    }

        //    int mapMatchPotential = 0;

        //    for (int i = 0; i < grid.Cells.Count; i++)
        //    {
        //        mapMatchPotential += grid.Cells[i].matchFromSwapPotential;
        //    }

        //    return mapMatchPotential;
        //}

        //public void cntPerPottentials(DNA<char> p)
        //{
        //    MatchGroup mg = new MatchGroup();

        //    for (int i = 0; i < grid.Cells.Count; i++)
        //    {
        //        grid.Cells[i].matchFromSwapPotential = 0;

        //        grid.Cells[i].obstacle = 0;
        //        grid.Cells[i].blocked1 = 0;
        //        grid.Cells[i].blocked2 = 0;
        //        grid.Cells[i].blocked3 = 0;
        //        grid.Cells[i].overlay1 = 0;
        //        grid.Cells[i].overlay2 = 0;
        //        grid.Cells[i].overlay3 = 0;
        //        grid.Cells[i].somethingWrong = 0;
        //    }

        //    for (int i = 0; i < grid.Cells.Count; i++)
        //    {
        //        mg.cntPerPottentials(grid, i);
        //    }

        //    int mapMatchPotential = 0;
        //    int obstacle = 0;
        //    int blocked1 = 0;
        //    int blocked2 = 0;
        //    int blocked3 = 0;
        //    int overlay1 = 0;
        //    int overlay2 = 0;
        //    int overlay3 = 0;
        //    int somethingWrong = 0;


        //    for (int i = 0; i < grid.Cells.Count; i++)
        //    {
        //        mapMatchPotential += grid.Cells[i].matchFromSwapPotential;

        //        obstacle += grid.Cells[i].obstacle;
        //        blocked1 += grid.Cells[i].blocked1;
        //        blocked2 += grid.Cells[i].blocked2;
        //        blocked3 += grid.Cells[i].blocked3;
        //        overlay1 += grid.Cells[i].overlay1;
        //        overlay2 += grid.Cells[i].overlay2;
        //        overlay3 += grid.Cells[i].overlay3;
        //        somethingWrong += grid.Cells[i].somethingWrong;
        //    }

        //    p.obstacle = obstacle;
        //    p.blocked1 = blocked1;
        //    p.blocked2 = blocked2;
        //    p.blocked3 = blocked3;
        //    p.overlay1 = overlay1;
        //    p.overlay2 = overlay2;
        //    p.overlay3 = overlay3;
        //    p.somethingWrong = somethingWrong;
        //}

        //public void cntMapPottentials(DNA<char> p)
        //{
        //    MatchGroup mg = new MatchGroup();

        //    for (int i = 0; i < grid.Cells.Count; i++)
        //    {
        //        grid.Cells[i].matchFromSwapPotential = 0;
        //        grid.Cells[i].obstacle = 0;
        //        grid.Cells[i].blocked1 = 0;
        //        grid.Cells[i].blocked2 = 0;
        //        grid.Cells[i].blocked3 = 0;
        //        grid.Cells[i].overlay1 = 0;
        //        grid.Cells[i].overlay2 = 0;
        //        grid.Cells[i].overlay3 = 0;
        //        grid.Cells[i].somethingWrong = 0;
        //    }

        //    for (int i = 0; i < grid.Cells.Count; i++)
        //    {
        //        mg.cntMapPerPottentials(grid, i);
        //    }

        //    int mapMatchPotential = 0;
        //    int obstacle = 0;
        //    int blocked1 = 0;
        //    int blocked2 = 0;
        //    int blocked3 = 0;
        //    int overlay1 = 0;
        //    int overlay2 = 0;
        //    int overlay3 = 0;
        //    int somethingWrong = 0;


        //    for (int i = 0; i < grid.Cells.Count; i++)
        //    {
        //        mapMatchPotential += grid.Cells[i].matchFromSwapPotential;

        //        obstacle += grid.Cells[i].obstacle;
        //        blocked1 += grid.Cells[i].blocked1;
        //        blocked2 += grid.Cells[i].blocked2;
        //        blocked3 += grid.Cells[i].blocked3;
        //        overlay1 += grid.Cells[i].overlay1;
        //        overlay2 += grid.Cells[i].overlay2;
        //        overlay3 += grid.Cells[i].overlay3;
        //        somethingWrong += grid.Cells[i].somethingWrong;
        //    }



        //    p.mapObstacle = obstacle;
        //    p.mapBlocked1 = blocked1;
        //    p.mapBlocked2 = blocked2;
        //    p.mapBlocked3 = blocked3;
        //    p.mapOverlay1 = overlay1;
        //    p.mapOverlay2 = overlay2;
        //    p.mapOverlay3 = overlay3;
        //    p.mapSomethingWrong = somethingWrong;
        //}




        //public void countObstacle(DNA<char> p)
        //{
        //    MatchGroup mg = new MatchGroup();

        //    for (int i = 0; i < grid.Cells.Count; i++)
        //    {
        //        grid.Cells[i].matchFromSwapPotential = 0;
        //        grid.Cells[i].nearBreakableObstacle = 0;
        //        grid.Cells[i].includeMatchObstacle = 0;
        //    }


        //    for (int i = 0; i < grid.Cells.Count; i++) mg.cntMatchPottential(grid, i);

        //    int mapMatchPotential = 0;
        //    List<int> mapMatchPotentialList = new List<int>();
        //    int breakableObstacle = 0;
        //    int obstacleCnt = 0;
        //    int includeMatchObstacle = 0;

        //    for (int i = 0; i < grid.Cells.Count; i++)
        //    {
        //        mapMatchPotentialList.Add(grid.Cells[i].matchFromSwapPotential);
        //        mapMatchPotential += grid.Cells[i].matchFromSwapPotential;              
        //        breakableObstacle += grid.Cells[i].nearBreakableObstacle;
        //        includeMatchObstacle += grid.Cells[i].includeMatchObstacle;

        //        //if (grid.Cells[i].DynamicObject == null) obstacleCnt++;

        //        if (grid.Cells[i].Blocked != null || grid.Cells[i].Overlay != null) obstacleCnt++;

        //    }

        //    p.mapMatchPotential = mapMatchPotential;
        //    p.mapMatchPotentialList = mapMatchPotentialList;
        //    p.breakableObstacle = breakableObstacle;
        //    p.obstacleCnt = obstacleCnt;
        //    p.includeMatchObstacle = includeMatchObstacle;
        //}




        //public int cntPerPottential(DNA<char> p)
        //{
        //    for (int i = 0; i < grid.Cells.Count; i++)
        //    {
        //        grid.Cells[i].matchFromSwapPotential = 0;
        //        grid.Cells[i].nearBreakableObstacle = 0;
        //        grid.Cells[i].includeMatchObstacle = 0;
        //    }

        //    int cnt = 0;

        //    MatchGroup mg = new MatchGroup();
        //    for (int i = 0; i < grid.Cells.Count; i++) mg.cntMatchPottential(grid, i);

        //    for (int i = 0; i < grid.Cells.Count; i++)
        //    {
        //        cnt += grid.Cells[i].matchFromSwapPotential;
        //    }

        //    return cnt;
        //}


        public void createMatchGroups(int minMatches, bool estimate, MatchGrid grid)
        {
            //l_mgList = new List<MatchGroup>();
            grid.mgList = new List<MatchGroup>();
            if (!estimate)
            {
                grid.Rows.ForEach((br) =>
                {
                    List<MatchGroup> mgList_t = br.GetMatches(minMatches, false);
                    if (mgList_t != null && mgList_t.Count > 0)
                    {
                        addRange(mgList_t, grid);
                    }
                });

                grid.Columns.ForEach((bc) =>
                {
                    List<MatchGroup> mgList_t = bc.GetMatches(minMatches, false);
                    if (mgList_t != null && mgList_t.Count > 0)
                    {
                        addRange(mgList_t, grid);
                    }
                });
            }
            else
            {
                List<MatchGroup> mgList_t = new List<MatchGroup>();
                grid.Rows.ForEach((gr) =>
                {
                    mgList_t.AddRange(gr.GetMatches(minMatches, true));
                });
                mgList_t.ForEach((mg) => { if (mg.IsEstimateMatch(mg.Length, true, grid)) { addEstimate(mg, grid); } });

                mgList_t = new List<MatchGroup>();
                grid.Columns.ForEach((gc) =>
                {
                    mgList_t.AddRange(gc.GetMatches(minMatches, true));
                });
                mgList_t.ForEach((mg) => { if (mg.IsEstimateMatch(mg.Length, false, grid)) { addEstimate(mg, grid); } });
            }
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

        public void new_createFillPath()
        {
            if (!grid.haveFillPath)
            {
                Debug.Log("mh Make gravity fill path");
                Map map = new Map(grid);
                PathFinder pF = new PathFinder();

                grid.Cells.ForEach((c) =>
                {
                    //if (!c.Blocked && !c.IsDisabled && !c.MovementBlocked && !c.Overlay)
                    if (!c.Blocked && !c.IsDisabled && !c.MovementBlocked)
                    {
                        int length = int.MaxValue;
                        List<GridCell> path = null;
                        grid.Columns.ForEach((col) =>
                        {
                            if (col.Spawn)
                            {
                                if (col.Spawn.gridCell != c)
                                {
                                    pF.new_CreatePath(map, c.pfCell, col.Spawn.gridCell.pfCell);

                                    if (pF.FullPath != null && pF.PathLenght < length) 
                                    {
                                        path = pF.GCPath(); length = pF.PathLenght; 
                                    }
                                }
                                else
                                {
                                    length = 0;
                                    path = new List<GridCell>();
                                }
                            }
                        });
                        c.fillPathToSpawner = path;
                    }
                });
            }
            else
            {
                Debug.Log("Have predefined fill path");
                PBoard pBoard = grid.LcSet.GetBoard(grid);
                grid.Cells.ForEach((c) =>
                {
                    if (!c.Blocked && !c.IsDisabled && !c.MovementBlocked && !c.spawner)
                    {
                        //   Debug.Log("path for " + c);
                        GridCell next = c;
                        List<GridCell> path = new List<GridCell>();
                        GridCell mather = null;
                        GridCell neigh = null;
                        bool end = false;
                        DirMather dir = DirMather.None;
                        bool clampDir = false;
                        while (!end)
                        {
                            dir = (!clampDir) ? pBoard[next.Row, next.Column] : dir;
                            NeighBors nS = next.Neighbors;
                            //     Debug.Log(dir);
                            switch (dir)
                            {
                                case DirMather.None:
                                    neigh = null;
                                    break;
                                case DirMather.Top:
                                    neigh = nS.Top;
                                    break;
                                case DirMather.Right:
                                    neigh = nS.Right;
                                    break;
                                case DirMather.Bottom:
                                    neigh = nS.Bottom;
                                    break;
                                case DirMather.Left:
                                    neigh = nS.Left;
                                    break;
                            }

                            if (neigh && neigh.spawner)
                            {
                                //  Debug.Log("spawner neigh " + neigh);
                                path.Add(neigh);
                                if (mather) mather = neigh;
                                end = true;
                            }
                            else if (!neigh)
                            {
                                //  Debug.Log("none neigh ");
                                end = true;
                                path = null;
                            }
                            else if (neigh)
                            {
                                if (!neigh.Blocked && !neigh.IsDisabled && !neigh.MovementBlocked)
                                {
                                    if (path.Contains(neigh)) // corrupted path
                                    {
                                        // Debug.Log("corruptred neigh " + neigh);
                                        end = true;
                                        path = null;
                                    }
                                    else
                                    {
                                        clampDir = false;
                                        path.Add(neigh);
                                        next = neigh;
                                        // Debug.Log("add " + neigh);
                                        clampDir = pBoard[next.Row, next.Column] == DirMather.None; // предусмотреть отсутствие направление у ячейки (save pevious dir)
                                    }
                                }
                                else if (neigh.IsDisabled) // passage cell
                                {
                                    next = neigh;
                                    clampDir = true;
                                    //  Debug.Log("disabled " + neigh);
                                }
                                else
                                {
                                    //  Debug.Log("another block " + neigh);
                                    end = true;
                                    path = null;
                                }
                            }
                        }
                        c.fillPathToSpawner = path;
                    }
                });
            }
        }



        public void new_fillGridByStep(List<GridCell> freeCells)
        {
            if (freeCells.Count == 0) return;

            foreach (GridCell gc in freeCells) gc.new_fillGrab();
        }


        public void mixGrid(Action completeCallBack, MatchGrid grid, Transform trans)
        {
            ParallelTween pT0 = new ParallelTween();
            ParallelTween pT1 = new ParallelTween();

            TweenSeq tweenSeq = new TweenSeq();
            List<GridCell> cellList = new List<GridCell>();
            List<GameObject> goList = new List<GameObject>();

            cancelTweens(grid);


            grid.Cells.ForEach((c) => { if (c.IsMixable) { cellList.Add(c); goList.Add(c.DynamicObject); } });

            cellList.ForEach((c) =>
            {
                int random = UnityEngine.Random.Range(0, goList.Count);
                GameObject m = goList[random];
                pT1.Add((callBack) => { c.GrabDynamicObject1(m.gameObject, false, callBack); });
                goList.RemoveAt(random);
            });

            tweenSeq.Add((callBack) =>
            {
                pT0.Start(callBack);
            });

            tweenSeq.Add((callBack) =>
            {
                pT1.Start(() =>
                {
                    callBack();
                });
            });
            tweenSeq.Start();
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
