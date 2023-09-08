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

namespace Mkey
{
    public class Match_Helper
    {
        public GameBoard board;
        public MatchGrid grid;
        public Dictionary<int, TargetData> curTargets;

        public List<int> CellsContainer;

        public int[] cellCnts;

        //public Match_Helper()
        //{
        //}

        //public FitnessHelper(List<GridCell> cells, List<int> collectID)
        //{
        //    fixGrid = new List<CellInfo>(cells.Count);
        //    l_mgList = new List<MatchGroup>();

        //    for (int i = 0; i < 30; i++) fixGrid.Add(new CellInfo(cells[i].Row, cells[i].Column, collectID[i]));
        //}
        //public class CellInfo
        //{
        //    public int row;
        //    public int col;
        //    public int objectID;

        //    public CellInfo(int c_row, int c_col, int c_ID)
        //    {
        //        row = c_row;
        //        col = c_col;
        //        objectID = c_ID;
        //    }
        //}

        //GetFreeCells


        public void createMatchGroups1(int minMatches, bool estimate, MatchGrid grid, DNA<char> p)
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
                    mgList_t.AddRange(gr.GetMatches1(minMatches, true));
                });
                mgList_t.ForEach((mg) => { if (mg.IsEstimateMatch1(mg.Length, true, grid, p)) { addEstimate(mg, grid); } });

                mgList_t = new List<MatchGroup>();
                grid.Columns.ForEach((gc) =>
                {
                    mgList_t.AddRange(gc.GetMatches1(minMatches, true));
                });
                mgList_t.ForEach((mg) => { if (mg.IsEstimateMatch1(mg.Length, false, grid, p)) { addEstimate(mg, grid); } });
            }
        }


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



        public void createFillPath(MatchGrid g)
        {
            if (!g.haveFillPath)
            {
                Debug.Log("Make gravity fill path");
                Map map = new Map(g);
                PathFinder pF = new PathFinder();

                g.Cells.ForEach((c) =>
                {
                    if (!c.Blocked && !c.IsDisabled && !c.MovementBlocked)
                    {
                        int length = int.MaxValue;
                        List<GridCell> path = null;
                        g.Columns.ForEach((col) =>
                        {
                            if (col.Spawn)
                            {
                                if (col.Spawn.gridCell != c)
                                {
                                    pF.CreatePath(map, c.pfCell, col.Spawn.gridCell.pfCell);
                                    if (pF.FullPath != null && pF.PathLenght < length) { path = pF.GCPath(); length = pF.PathLenght; }
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
                PBoard pBoard = g.LcSet.GetBoard(g);
                g.Cells.ForEach((c) =>
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
                                        clampDir = pBoard[next.Row, next.Column] == DirMather.None; // ¬á¬â¬Ö¬Õ¬å¬ã¬Þ¬à¬ä¬â¬Ö¬ä¬î ¬à¬ä¬ã¬å¬ä¬ã¬ä¬Ó¬Ú¬Ö ¬ß¬Ñ¬á¬â¬Ñ¬Ó¬Ý¬Ö¬ß¬Ú¬Ö ¬å ¬ñ¬é¬Ö¬Û¬Ü¬Ú (save pevious dir)
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

        public void fillGridByStep(List<GridCell> freeCells, Action completeCallBack)
        {
            if (freeCells.Count == 0)
            {
                //completeCallBack?.Invoke();
                return;
            }

            foreach (GridCell gc in freeCells)
            {
                gc.fillGrab1(completeCallBack);
            }


            //ParallelTween tp = new ParallelTween();
            //foreach (GridCell gc in freeCells)
            //{
            //    tp.Add((callback) =>
            //    {
            //        gc.FillGrab1(callback);
            //    });
            //}
            //tp.Start1(() =>
            //{
            //    //completeCallBack?.Invoke();
            //});
        }

        public void collectFalling(MatchGrid grid)
        {
            //   Debug.Log("collect falling " + GetFalling().Count);
            ParallelTween pt = new ParallelTween();
            foreach (var item in getFalling(grid))
            {
                pt.Add((callBack) =>
                {
                    item.Collect(0, false, true, callBack);
                });
            }
            pt.Start1();
        }

        public List<FallingObject> getFalling(MatchGrid grid)
        {
            List<GridCell> botCell = grid.GetBottomDynCells();
            List<FallingObject> res = new List<FallingObject>();
            foreach (var item in botCell)
            {
                if (item)
                {
                    FallingObject f = item.Falling;
                    if (f)
                    {
                        res.Add(f);
                    }
                }
            }
            return res;
        }

        public void collectMatchGroups(MatchGrid grid)
        {
            ParallelTween pt = new ParallelTween();

            if (grid.mgList.Count == 0) return;


            for (int i = 0; i < grid.mgList.Count; i++)
            {
                if (grid.mgList[i] != null)
                {
                    MatchGroup m = grid.mgList[i];
                    pt.Add((callBack) =>
                    {
                        //Collect(m, callBack);
                    });
                }
            }
            pt.Start1(() =>
            {
                
            });
        }

        

        public void mixGrid(Action completeCallBack, MatchGrid grid, Transform trans)
        {
            ParallelTween pT0 = new ParallelTween();
            ParallelTween pT1 = new ParallelTween();

            TweenSeq tweenSeq = new TweenSeq();
            List<GridCell> cellList = new List<GridCell>();
            List<GameObject> goList = new List<GameObject>();
            //CollectGroups.CancelTweens();
            //EstimateGroups.CancelTweens();

            
            cancelTweens(grid);


            grid.Cells.ForEach((c) => { if (c.IsMixable) { cellList.Add(c); goList.Add(c.DynamicObject); } });
            //cellList.ForEach((c) => { pT0.Add((callBack) => { c.MixJump1(trans.position, callBack); }); });

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
                    //MbState = MatchBoardState.Fill;
                    //completeCallBack?.Invoke();
                    callBack();
                });
            });
            tweenSeq.Start();
        }

        public bool[,] isVisited;
        public int[,] connected;
        public int[] possibleArea;

        public bool isConnected(MatchGrid grid, int cnt)
        {
            GridCell[,] curCell = new GridCell[grid.Columns.Count, grid.Rows.Count];

            for (int i = 0; i < grid.Columns.Count; i++)
            {
                for (int j = 0; j < grid.Rows.Count; j++)
                {
                    curCell[i, j] = grid.Cells[i * grid.Rows.Count + j];
                }
            }

            isVisited = new bool[grid.Columns.Count, grid.Rows.Count];
            connected = new int[grid.Columns.Count, grid.Rows.Count];

            //for (int i = 0; i < grid.Columns.Count; i++)
            //{
            //    for (int j = 0; j < grid.Rows.Count; j++)
            //    {
            //        isVisited[i,j] = true;

            //        if(curCell[i, j].DynamicObject)
            //        {
            //            bool isReachSpawn = false;
            //            visitConnectCell(curCell, i, j, isReachSpawn);
            //            break;
            //        }


            //        //if (!isVisited[i, j] && curCell[i, j].DynamicObject)
            //        //{
            //        //    bool isReachSpawn = false;
            //        //    visitConnectCell(curCell, i, j, isReachSpawn);
            //        //    if (!isReachSpawn) return false;
            //        //}
            //    }

            //    break;
            //}

            for (int row = 0; row < grid.Rows.Count; row++)
            {
                for (int col = 0; col < grid.Columns.Count; col++)
                {
                    if(!curCell[col, row].DynamicObject)
                    {
                        isVisited[col, row] = true;
                        connected[col, row] = grid.Columns.Count + 1;
                    }
                }
            }

            int area = 1;

            for (int row = 0; row < grid.Rows.Count; row++)
            {
                int col = 0;

                if (!isVisited[col, row])
                {
                    isVisited[col, row] = true;
                    bool isReachSpawn = false;
                    visitConnectCell(curCell, col, row, isReachSpawn);

                    for (int i = 0; i < grid.Columns.Count; i++)
                    {
                        for (int j = 0; j < grid.Rows.Count; j++)
                        {
                            if (isVisited[i,j] && connected[i, j] == 0) connected[i, j] = area;
                        }
                    }

                    area++;
                }
            }

            possibleArea = new int[grid.Columns.Count + 2];

            for (int i = 0; i < grid.Columns.Count; i++)
            {
                for (int j = 0; j < grid.Rows.Count; j++)
                {
                    possibleArea[connected[i, j]] += grid.Cells[i * grid.Rows.Count + j].possibleCnt;
                }
            }


            //int[,] possibleCnts = new int[grid.Columns.Count, grid.Rows.Count];
            //int[] cnts = new int[16];

            //for (int i = 0; i < grid.Columns.Count; i++)
            //{
            //    for (int j = 0; j < grid.Rows.Count; j++)
            //    {
            //        possibleCnts[i, j] = grid.Cells[i * grid.Rows.Count + j].possibleCnt;
            //        cnts[grid.Cells[i * grid.Rows.Count + j].possibleCnt]++;
            //    }
            //}



            return true;
        }

        void visitConnectCell(GridCell[,] curCell, int col, int row, bool isReachSpawn)
        {
            if (canVisit(curCell, col, row - 1)) // Top
            {
                if (row == 0) isReachSpawn = true;
                isVisited[col,row - 1] = true;
                visitConnectCell(curCell, col, row - 1, isReachSpawn);
            }

            if (canVisit(curCell, col, row + 1)) // Bottom
            {
                if (row == 0) isReachSpawn = true;
                isVisited[col, row + 1] = true;
                visitConnectCell(curCell, col, row + 1, isReachSpawn);
            }

            if (canVisit(curCell, col - 1, row)) // Left
            {
                if (row == 0) isReachSpawn = true;
                isVisited[col - 1,row] = true;
                visitConnectCell(curCell, col - 1, row, isReachSpawn);
            }

            if (canVisit(curCell, col + 1, row)) // Right
            {
                if (row == 0) isReachSpawn = true;
                isVisited[col + 1,row] = true;
                visitConnectCell(curCell, col + 1, row, isReachSpawn);
            }

            //if (canVisit(curCell, col - 1, row - 1)) // TopLeft
            //{
            //    if (row == 0) isReachSpawn = true;
            //    isVisited[col - 1,row - 1] = true;
            //    visitConnectCell(curCell, col - 1, row - 1, isReachSpawn);
            //}

            //if (canVisit(curCell, col - 1, row + 1)) // BottomLeft
            //{
            //    if (row == 0) isReachSpawn = true;
            //    isVisited[col - 1, row + 1] = true;
            //    visitConnectCell(curCell, col - 1, row + 1, isReachSpawn);
            //}

            //if (canVisit(curCell, col + 1, row - 1)) // TopRight
            //{
            //    if (row == 0) isReachSpawn = true;
            //    isVisited[col + 1, row - 1] = true;
            //    visitConnectCell(curCell, col + 1, row - 1, isReachSpawn);
            //}

            //if (canVisit(curCell, col + 1, row + 1)) // BottomRight
            //{
            //    if (row == 0) isReachSpawn = true;
            //    isVisited[col + 1, row + 1] = true;
            //    visitConnectCell(curCell, col + 1, row + 1, isReachSpawn);
            //}
        }

        bool canVisit(GridCell[,] curCell, int col, int row)
        {
            if( col >= 0 && row >= 0 && col < grid.Columns.Count && row < grid.Rows.Count)
            {
                if(!isVisited[col, row] && curCell[col, row].DynamicObject) return true;
                else return false;

                //if (col >= 0 && row >= 0 && col <= grid.Columns.Count && row <= grid.Rows.Count)
                //{
                //    if (curCell[col, row].DynamicObject) return true;
                //        else return false;
                //}
            }


            //if (col >= 0 && col < curCell[col,row].GColumn.Length && row >= 0 && row < curCell[col, row].GRow.Length)
            //{
            //    if (curCell[col, row].DynamicObject) return true;
            //    else return false;
            //}

            else return false;
        }





        public void createAvailableMatchGroup(MatchGrid grid)
        {
            for(int i= 0; i < grid.Cells.Count;i++) grid.Cells[i].possibleCnt = 0;

            //grid.mgList = new List<MatchGroup>();

            //List<MatchGroup> mgList_t = new List<MatchGroup>();
            //grid.Rows.ForEach((gr) =>
            //{
            //    mgList_t.AddRange(gr.isContinuousRow());
            //});
            //mgList_t.ForEach((mg) => { mg.countPossible(mg.Length, true, grid);});

            //mgList_t = new List<MatchGroup>();
            //grid.Columns.ForEach((gc) =>
            //{
            //    mgList_t.AddRange(gc.isContinuousColumn());
            //});

            //mgList_t.ForEach((mg) => { mg.countPossible(mg.Length, false, grid); });

            List<int> pc = new List<int>();

            for (int i = 0; i < grid.Cells.Count; i++)
            {
                pc.Add(grid.Cells[i].possibleCnt);
            }


            int a = 0;

        }





        //public GridCell GetLowermostX(MatchGrid grid)
        //{
        //    if (grid.Cells.Count == 0) return null;
        //    GridCell l = grid.Cells[0];
        //    for (int i = 0; i < grid.Cells.Count; i++) if (grid.Cells[i].Column < l.Column) l = grid.Cells[i];
        //    return l;
        //}

        //public GridCell GetTopmostX(MatchGrid grid)
        //{
        //    if (grid.Cells.Count == 0) return null;
        //    GridCell t = grid.Cells[0];
        //    for (int i = 0; i < grid.Cells.Count; i++) if (grid.Cells[i].Column > t.Column) t = grid.Cells[i];
        //    return t;
        //}

        //public GridCell GetLowermostY(MatchGrid grid)
        //{
        //    if (grid.Cells.Count == 0) return null;
        //    GridCell l = grid.Cells[0];
        //    for (int i = 0; i < grid.Cells.Count; i++) if (grid.Cells[i].Row > l.Row) l = grid.Cells[i];
        //    return l;
        //}

        //public GridCell GetTopmostY(MatchGrid grid)
        //{
        //    if (grid.Cells.Count == 0) return null;
        //    GridCell t = grid.Cells[0];
        //    for (int i = 0; i < grid.Cells.Count; i++) if (grid.Cells[i].Row < t.Row) t = grid.Cells[i];
        //    return t;
        //}












    }








}