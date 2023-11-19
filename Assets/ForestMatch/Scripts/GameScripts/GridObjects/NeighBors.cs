using System;
using System.Collections.Generic;
using UnityEngine;

namespace Mkey
{
    [Serializable]
    /// <summary>
    /// Get neighborns for gridcell 
    /// </summary>
    public class NeighBors
    {
        public GridCell Main { get; private set; }
        public GridCell Left { get; private set; }
        public GridCell Right { get; private set; }
        public GridCell Top { get; private set; }
        public GridCell Bottom { get; private set; }

        public GridCell TopLeft { get; private set; }
        public GridCell BottomLeft { get; private set; }
        public GridCell TopRight { get; private set; }
        public GridCell BottomRight { get; private set; }

        public List<GridCell> Cells { get; private set; }

        /// <summary>
        /// Create NeighBorns  cells
        /// </summary>
        /// <param name="main"></param>
        /// <param name="id"></param>
        public NeighBors(GridCell main, bool addDiag)
        {
            Main = main;
            Left = main.GRow[main.Column - 1];
            Right = main.GRow[main.Column + 1];
            Top = main.GColumn[main.Row - 1];
            Bottom = main.GColumn[main.Row + 1];

            Cells = new List<GridCell>();
            if (Top) Cells.Add(Top);
            if (Bottom) Cells.Add(Bottom);
            if (Left) Cells.Add(Left);
            if (Right) Cells.Add(Right);

            if (addDiag)
            {
                TopLeft = (Top) ? Top.GRow[Top.Column - 1] : null;
                BottomLeft = (Bottom) ? Bottom.GRow[Bottom.Column - 1] : null; 
                TopRight = (Top) ? Top.GRow[Top.Column + 1] : null;
                BottomRight = (Bottom) ? Bottom.GRow[Bottom.Column + 1] : null;

                Cells = new List<GridCell>();
                if (Top) Cells.Add(Top);
                if (TopLeft) Cells.Add(TopLeft);
                if (Left) Cells.Add(Left);
                if (BottomLeft) Cells.Add(BottomLeft);

                if (Bottom) Cells.Add(Bottom);
                if (BottomRight) Cells.Add(BottomRight);
                if (Right) Cells.Add(Right);
                if (TopRight) Cells.Add(TopRight);
            }
        }

        public bool Contain(GridCell gCell)
        {
            return Cells.Contains(gCell);
        }

        public override string ToString()
        {
            return ("All cells : " + ToString(Cells));
        }

        public static string ToString(List<GridCell> list)
        {
            string res = "";
            foreach (var item in list)
            {
                res += item.ToString();
            }
            return res;
        }

        List<PFCell> results = new List<PFCell>();

        public List<PFCell> GetNeighBorsPF()
        {
            results = new List<PFCell>();

            foreach (var item in Cells)
            {
                results.Add(item.pfCell);
            }

            return results;


            //List<PFCell> res = new List<PFCell>();
            //foreach (var item in Cells)
            //{
            //    res.Add(item.pfCell);
            //}

            //return res;
        }

        public List<GridCell> GetMatchIdCells(int id, bool matchable)
        {
            List<GridCell> res = new List<GridCell>();
            foreach (var item in Cells)
            {
                MatchObject m = item.Match;
                if(m &&(m.ID == id) && (item.IsMatchable == matchable))
                {
                    res.Add(item);
                }
            }
            return res;
        }


        public List<GridCell> HaveFillPath()
        {
            List<GridCell> res = new List<GridCell>();

            int maxSize = 100;

            //if (Left != null)
            //{
            //    if (!Left.Blocked && !Left.IsDisabled && !Left.MovementBlocked)
            //    {
            //        if (Left.fillPathToSpawner != null)
            //        {
            //            Left.fillPathToSpawner = new List<GridCell>();
            //            Bottom.fillPathToSpawner.Add(gc);

            //            if (gc.fillPathToSpawner != null)
            //            {
            //                foreach (var v in gc.fillPathToSpawner) Bottom.fillPathToSpawner.Add(v);
            //            }

            //            Bottom.isVisit = true;

            //            Bottom.Neighbors.bottomFill(Bottom);
            //        }
            //    }
            //}





            if (Left != null)
            {
                if (!Left.isVisit && Left.fillPathToSpawner != null)
                {

                }


                if (Left.isVisit && Left.fillPathToSpawner != null)
                {
                    if (maxSize > Left.fillPathToSpawner.Count + 1)
                    {
                        maxSize = Left.fillPathToSpawner.Count + 1;
                        res = Left.fillPathToSpawner;
                        res.Add(Left);
                    }
                }
            }

            if (Right != null)
            {
                if (Right.isVisit && Right.fillPathToSpawner != null)
                {
                    if (maxSize > Right.fillPathToSpawner.Count + 1)
                    {
                        maxSize = Right.fillPathToSpawner.Count + 1;
                        res = Right.fillPathToSpawner;
                        res.Add(Right);
                    }
                }

            }

            if (Bottom != null)
            {
                if (Bottom.isVisit && Bottom.fillPathToSpawner != null)
                {
                    if (maxSize > Bottom.fillPathToSpawner.Count + 1)
                    {
                        maxSize = Bottom.fillPathToSpawner.Count + 1;
                        res = Bottom.fillPathToSpawner;
                        res.Add(Bottom);
                    }
                }
            }

            if (Top != null)
            {
                if (Top.isVisit && Top.fillPathToSpawner != null)
                {
                    if (maxSize > Top.fillPathToSpawner.Count + 1)
                    {
                        maxSize = Top.fillPathToSpawner.Count + 1;
                        res = Top.fillPathToSpawner;
                        res.Add(Top);
                    }
                }
            }



            if (res.Count == 0) res = null;

            //if(Left.fillPathToSpawner != null && Left.fillPathToSpawner.Count < maxSize)
            //{
            //    maxSize = Left.fillPathToSpawner.Count;
            //}

                //if

                //Left {
                //    get;
                //    Right {
                //        get
                //    Top {
                //            get;
                //            Bottom {
                //                ge

                //            TopLeft {
                //                    g
                //            BottomLeft
                //            TopRight {
                //                        BottomRight


                //foreach (var item in Cells)
                //{
                //    MatchObject m = item.Match;
                //    if (m && (m.ID == id) && (item.IsMatchable == matchable))
                //    {
                //        res.Add(item);
                //    }
                //}
            return res;
        }

        public void findFillPath(GridCell gc)
        {
            gc.fillPathToSpawner = new List<GridCell>();

            if (Left != null)
            {
                gc.calculate(Left);

                //if (!Left.Blocked && !Left.IsDisabled && !Left.MovementBlocked)
                //{
                //    if(Left.fillPathToSpawner == null && !Left.isVisit)
                //    {
                //        Left.Neighbors.findFillPath(Left);
                //    }

                //    if (Bottom.fillPathToSpawner != null)
                //    {
                //        if(gc.fillPathToSpawner.Count > 0)
                //        {
                //            if(gc.fillPathToSpawner.Count < Left.fillPathToSpawner.Count + 1)
                //            {
                //                gc.fillPathToSpawner = new List<GridCell>();
                //                gc.fillPathToSpawner.Add(Left);

                //                if (Left.fillPathToSpawner != null)
                //                {
                //                    foreach (var v in Left.fillPathToSpawner) gc.fillPathToSpawner.Add(v);
                //                }
                //            }
                //        }
                //        gc.isVisit = true;
                //    }

                //    else
                //    {
                //        Bottom.isVisit = true;
                //    }
                //}
            }

            if (Right != null) gc.calculate(Right);

            if (Bottom != null) gc.calculate(Bottom);

            if (Top != null) gc.calculate(Top);

        }



        public void bottomFill(GridCell gc)
        {
            if(Bottom != null)
            {
                if (!Bottom.Blocked && !Bottom.IsDisabled && !Bottom.MovementBlocked)
                {
                    if(Bottom.fillPathToSpawner == null)
                    {
                        Bottom.fillPathToSpawner = new List<GridCell>();
                        Bottom.fillPathToSpawner.Add(gc);

                        if (gc.fillPathToSpawner != null)
                        {
                            foreach (var v in gc.fillPathToSpawner) Bottom.fillPathToSpawner.Add(v);
                        }

                        Bottom.isVisit = true;

                        Bottom.Neighbors.bottomFill(Bottom);
                    }
                }
            }
        }
        public List<GridCell> findPath()
        {
            List<GridCell> res = new List<GridCell>();
            int maxSize = 100;

            if(Left != null)
            {
                if(Left.Blocked == null && !Left.isVisit)
                {
                    List<GridCell> lgs = new List<GridCell>();
                    lgs = Left.Neighbors.findPath();

                    if (lgs != null)
                    {
                        if (maxSize > lgs.Count + 1)
                        {
                            maxSize = lgs.Count + 1;
                            res = lgs;
                            res.Add(Left);
                        }
                    }
                }
            }

            if (Right != null)
            {
                if (Right.Blocked == null && !Right.isVisit)
                {
                    List<GridCell> lgs = new List<GridCell>();
                    lgs = Right.Neighbors.findPath();

                    if (lgs != null)
                    {
                        if (maxSize > lgs.Count + 1)
                        {
                            maxSize = lgs.Count + 1;
                            res = lgs;
                            res.Add(Right);
                        }
                    }
                }
            }

            if (Top != null)
            {
                if (Top.Blocked == null && !Top.isVisit)
                {
                    List<GridCell> lgs = new List<GridCell>();
                    lgs = Top.Neighbors.findPath();

                    if (lgs != null)
                    {
                        if (maxSize > lgs.Count + 1)
                        {
                            maxSize = lgs.Count + 1;
                            res = lgs;
                            res.Add(Top);
                        }
                    }
                }
            }


            if (Bottom != null)
            {
                if (Bottom.Blocked == null && !Bottom.isVisit)
                {
                    List<GridCell> lgs = new List<GridCell>();
                    lgs = Bottom.Neighbors.findPath();

                    if (lgs != null)
                    {
                        if (maxSize > lgs.Count + 1)
                        {
                            maxSize = lgs.Count + 1;
                            res = lgs;
                            res.Add(Bottom);
                        }
                    }
                }
            }

            if (res.Count == 0) res = null;
            return res;
        }

    }
}