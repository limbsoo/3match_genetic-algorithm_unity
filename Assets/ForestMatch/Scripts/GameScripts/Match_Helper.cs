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

namespace Mkey
{
    public class Match_Helper
    {
        public GameBoard board;
        public MatchGrid grid;
        public Dictionary<int, TargetData> curTargets;

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





        public void CreateMatchGroups(int minMatches, bool estimate, MatchGrid grid)
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
                        AddRange(mgList_t, grid);
                    }
                });

                grid.Columns.ForEach((bc) =>
                {
                    List<MatchGroup> mgList_t = bc.GetMatches(minMatches, false);
                    if (mgList_t != null && mgList_t.Count > 0)
                    {
                        AddRange(mgList_t, grid);
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
                mgList_t.ForEach((mg) => { if (mg.IsEstimateMatch(mg.Length, true, grid)) { AddEstimate(mg, grid); } });

                mgList_t = new List<MatchGroup>();
                grid.Columns.ForEach((gc) =>
                {
                    mgList_t.AddRange(gc.GetMatches(minMatches, true));
                });
                mgList_t.ForEach((mg) => { if (mg.IsEstimateMatch(mg.Length, false, grid)) { AddEstimate(mg, grid); } });
            }
        }

        private MatchGroup Merge(List<MatchGroup> intersections)
        {

            MatchGroup mG = new MatchGroup();
            intersections.ForEach((ints) => { mG.Merge(ints); });
            return mG;
        }

        public void Add(MatchGroup mG, MatchGrid grid)
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
                grid.mgList.Add(Merge(intersections));
            }
            else
            {
                grid.mgList.Add(mG);
            }
        }
        public void AddRange(List<MatchGroup> mGs, MatchGrid grid)
        {
            for (int i = 0; i < mGs.Count; i++)
            {
                Add(mGs[i], grid);
            }
        }
        public void AddEstimate(MatchGroup mGe, MatchGrid grid)
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

        public void CancelTweens(MatchGrid g)
        {
            g.mgList.ForEach((mg) => { CancelTween1(g); });
        }

        public void CancelTween1(MatchGrid g)
        {
            g.Cells.ForEach((c) => { c.CancelTween(); });
        }



        public void CreateFillPath(MatchGrid g)
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
                                        clampDir = pBoard[next.Row, next.Column] == DirMather.None; // ���֬լ��ެ���֬�� ��������Ӭڬ� �߬Ѭ��ѬӬݬ֬߬ڬ� �� ���֬۬ܬ� (save pevious dir)
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

        public void FillGridByStep(List<GridCell> freeCells, Action completeCallBack)
        {
            if (freeCells.Count == 0)
            {
                //completeCallBack?.Invoke();
                return;
            }

            ParallelTween tp = new ParallelTween();
            foreach (GridCell gc in freeCells)
            {
                tp.Add((callback) =>
                {
                    gc.FillGrab1(callback);
                });
            }
            tp.Start1(() =>
            {
                //completeCallBack?.Invoke();
            });
        }

        public void CollectFalling(MatchGrid grid)
        {
            //   Debug.Log("collect falling " + GetFalling().Count);
            ParallelTween pt = new ParallelTween();
            foreach (var item in GetFalling(grid))
            {
                pt.Add((callBack) =>
                {
                    item.Collect(0, false, true, callBack);
                });
            }
            pt.Start1();
        }

        public List<FallingObject> GetFalling(MatchGrid grid)
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

        public void CollectMatchGroups(MatchGrid grid)
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

        

        public void MixGrid(Action completeCallBack, MatchGrid grid, Transform trans)
        {
            ParallelTween pT0 = new ParallelTween();
            ParallelTween pT1 = new ParallelTween();

            TweenSeq tweenSeq = new TweenSeq();
            List<GridCell> cellList = new List<GridCell>();
            List<GameObject> goList = new List<GameObject>();
            //CollectGroups.CancelTweens();
            //EstimateGroups.CancelTweens();

            
            CancelTweens(grid);


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



    }


}