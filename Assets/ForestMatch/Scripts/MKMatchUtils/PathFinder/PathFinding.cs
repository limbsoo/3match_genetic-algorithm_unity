using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using UnityEngine;
using static UnityEditor.Progress;
using static UnityEngine.RuleTile.TilingRuleOutput;

namespace Mkey
{
    public class PathFinder : IDisposable
    {
        public void Dispose()
        {
            //Dispose(true);
            GC.SuppressFinalize(this);
        }

        //Queue<List<GridCell>> llgs;

        //Queue<List<PFCell>> waveArrays;
        //Queue<List<PFCell>> waveArrayTemps;

 
        public PathFinder()
        {
            //llgs = new Queue<List<GridCell>>();
            //waveArrays = new Queue<List<PFCell>>();
            //waveArrayTemps = new Queue<List<PFCell>>();


            //for (int i = 0; i < 100; i++)
            //{
            //    llgs.Enqueue(new List<GridCell>());
            //    waveArrays.Enqueue(new List<PFCell>());
            //    waveArrayTemps.Enqueue(new List<PFCell>());
            //}



            //for (int i = 0; i < 200; i++)
            //{
            //    llgs.Enqueue(new List<GridCell>());

            //}

            //for (int i = 0; i < 500; i++)
            //{
            //    waveArrays.Enqueue(new List<PFCell>());

            //}


            //for (int i = 0; i < 500; i++)
            //{
            //    waveArrayTemps.Enqueue(new List<PFCell>());
            //}


        }




        private List<PFCell> fullPath = new List<PFCell>();
        //public IList<PFCell> FullPath { get { return fullPath?.AsReadOnly(); } }
        public IList<PFCell> FullPath { get { return (fullPath == null) ? null : fullPath.AsReadOnly(); } }

        List<GridCell> res;

        //List<GridCell> res = new List<GridCell>();


        public List<GridCell> GCPath()
        {
            //llgs.Enqueue(res);
            //res = llgs.Dequeue();
            //res.Clear();



            //List<GridCell> res = new List<GridCell>();
            //res.Clear();


            res = null; 
            res = new List<GridCell>();

            //res = new();
            if (fullPath != null)
            {
                foreach (var item in fullPath)
                {
                    res.Add(item.gCell);
                }
            }


            return res;


            //List<GridCell> res = new List<GridCell>();
            //if (fullPath != null)
            //{
            //    foreach (var item in fullPath)
            //    {
            //        res.Add(item.gCell);
            //    }
            //}
            //return res;
        }

        /// <summary>
        /// Create all possible paths from this position
        /// </summary>
        /// <param name = "A" ></ param >
        private void CreateGlobWayMap(Map WorkMap, PFCell A)
        {
            // UnityEngine.Debug.Log("create path to top ");
            WorkMap.ResetPath();
            List<PFCell> waveArray = new List<PFCell>();
            waveArray.Add(A);
            A.mather = A;

            bool work = true;
            while (work)
            {
                work = false;
                List<PFCell> waveArrayTemp = new List<PFCell>();
                foreach (PFCell mather in waveArray)
                {
                    if (mather.available || (A == mather && !mather.available))
                    {
                        List<PFCell> childrens = mather.Neighbors.GetNeighBorsPF();
                        foreach (PFCell child in childrens)
                        {
                            if (!child.HaveMather() && child.available && child.IsPassabilityFrom(mather)) /// try
                            {
                                child.mather = mather;
                                waveArrayTemp.Add(child);
                                work = true;
                            }
                        }
                    }
                }
                waveArray = waveArrayTemp;
            }
        }

        /// <summary>
        /// Create all possible paths to destination point
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// 

        List<PFCell> waveArrayTemp;
        List<PFCell> waveArray;

        //List<PFCell> waveArrayTemp = new List<PFCell>();
        //List<PFCell> waveArray = new List<PFCell>();

        private void CreateGlobWayMap(Map WorkMap, PFCell A, PFCell B, int cnt)
        {
            WorkMap.ResetPath();

            waveArray = null;
            waveArray = new List<PFCell>();



            //waveArrays.Enqueue(waveArray);
            //waveArray = waveArrays.Dequeue();
            //waveArray.Clear();
            ////waveArray.TrimExcess();


            waveArray.Add(A);
            A.mather = A;
            bool work = true;


            while (work)
            {
                work = false;

                waveArrayTemp = null;
                waveArrayTemp = new List<PFCell>();


                //waveArrayTemps.Enqueue(waveArrayTemp);
                //waveArrayTemp = waveArrayTemps.Dequeue();
                //waveArrayTemp.Clear();



                foreach (PFCell mather in waveArray)
                {
                    if (mather.available)
                    {
                        List<PFCell> childrens = mather.Neighbors.GetNeighBorsPF();




                        foreach (PFCell child in childrens)
                        {
                            if (!child.HaveMather())
                            {
                                child.mather = mather;
                                waveArrayTemp.Add(child);
                                work = true;
                                if (child == B) return;

                            }
                        }
                    }
                }
                waveArray = null;
                waveArray = waveArrayTemp;
            }
        }


        bool isFirst = true;

        public void new_CreatePath(Map WorkMap, PFCell A, PFCell B, int cnt)
        {
            fullPath = null;
            if (WorkMap == null || A == null || B == null || !A.available || !B.available) return;

            CreateGlobWayMap(WorkMap, A, B, cnt);
            if (IsWayExistTo(B))
            {

                fullPath = null;
                //fullPath = new List<PFCell>();

                //if (cnt > 70)
                //{
                //    fullPath = new List<PFCell>(20);
                //}

                //else if (cnt > 30)
                //{
                //    fullPath = new List<PFCell>(10);
                //}

                //else fullPath = new List<PFCell>();

                fullPath = new List<PFCell>();




                fullPath.Add(B);
                PFCell mather = B.mather;
                while (mather != A.mather)
                {
                    fullPath.Add(mather);
                    mather = mather.mather;
                }
                fullPath.Reverse();
            }

        }




        /// <summary>
        /// Return true if FullPathA contain start point and end point
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <returns></returns>
        private bool IsWayCreated(PFCell A, PFCell B)
        {
            return (fullPath!=null && PathLenght > 0 && fullPath[0] == A && fullPath[PathLenght - 1] == B);
        }





        //waveArrays.Enqueue(waveArray);
        //waveArray = waveArrays.Dequeue();
        //waveArray.Clear();
        ////waveArray.TrimExcess();
        ///

        //waveArrayTemps.Enqueue(waveArrayTemp);
        //waveArrayTemp = waveArrayTemps.Dequeue();
        //waveArrayTemp.Clear();

        private void CreateGlobWayMa11111p(Map WorkMap, PFCell A, PFCell B, List<GridCell> path)
        {
            WorkMap.ResetPath();

            waveArray = null;
            waveArray = new List<PFCell>();

            waveArray.Add(A);
            A.mather = A;
            bool work = true;


            while (work)
            {
                work = false;

                waveArrayTemp = null;
                waveArrayTemp = new List<PFCell>();



                foreach (PFCell mather in waveArray)
                {
                    if (mather.available)
                    {
                        List<PFCell> childrens = mather.Neighbors.GetNeighBorsPF();
                        foreach (PFCell child in childrens)
                        {
                            if (!child.HaveMather())
                            {
                                child.mather = mather;
                                waveArrayTemp.Add(child);
                                work = true;
                                if (child == B) return;

                            }
                        }
                    }
                }
                waveArray = null;
                waveArray = waveArrayTemp;
            }
        }


        private void CreateGlobWayMap(Map WorkMap, PFCell A, PFCell B)
        {
            WorkMap.ResetPath();

            waveArray = null;
            waveArray = new List<PFCell>();



            waveArray.Add(A);
            A.mather = A;
            bool work = true;


            while (work)
            {
                work = false;

                waveArrayTemp = null;
                waveArrayTemp = new List<PFCell>();



                foreach (PFCell mather in waveArray)
                {
                    if (mather.available)
                    {
                        List<PFCell> childrens = mather.Neighbors.GetNeighBorsPF();
                        foreach (PFCell child in childrens)
                        {
                            if (!child.HaveMather())
                            {
                                child.mather = mather;
                                waveArrayTemp.Add(child);
                                work = true;
                                if (child == B) return;

                            }
                        }
                    }
                }
                waveArray = null;
                waveArray = waveArrayTemp;
            }
        }


        public void finding(Map WorkMap, PFCell A, PFCell current, Stack<GridCell> currentPath)
        {
            if (current.mather == null)
            {
                current.mather = current;

                if (!current.gCell.Blocked && !current.gCell.IsDisabled && !current.gCell.MovementBlocked)
                {
                    if (current.gCell.spawner)
                    {
                        if (A.gCell.fillPathToSpawner != null)
                        {
                            if (current.gCell.fillPathToSpawner == null)
                            {
                                A.gCell.fillPathToSpawner = new List<GridCell>();
                                currentPath.Push(current.gCell);

                                foreach (var c in currentPath)
                                {
                                    c.pfCell.mather = c.pfCell;
                                    A.gCell.fillPathToSpawner.Add(c);
                                }
                                currentPath.Pop();

                                //if (currentPath.Count < A.gCell.fillPathToSpawner.Count)
                                //{
                                //    A.gCell.fillPathToSpawner = new List<GridCell>();
                                //    currentPath.Push(current.gCell);
                                //    foreach (var c in currentPath)
                                //    {
                                //        c.pfCell.mather = c.pfCell;
                                //        A.gCell.fillPathToSpawner.Add(c);
                                //    }
                                //    currentPath.Pop();
                                //}
                            }

                            else
                            {
                                if (currentPath.Count + current.gCell.fillPathToSpawner.Count < A.gCell.fillPathToSpawner.Count)
                                {
                                    A.gCell.fillPathToSpawner = new List<GridCell>();
                                    currentPath.Push(current.gCell);
                                    foreach (var c in currentPath)
                                    {
                                        c.pfCell.mather = c.pfCell;
                                        A.gCell.fillPathToSpawner.Add(c);
                                    }
                                    currentPath.Pop();
                                }
                            }


                        }

                        else
                        {
                            A.gCell.fillPathToSpawner = new List<GridCell>();
                            currentPath.Push(current.gCell);
                            foreach (var c in currentPath)
                            {
                                c.pfCell.mather = c.pfCell;
                                A.gCell.fillPathToSpawner.Add(c);
                            }

                            currentPath.Pop();
                        }


                    }

                    else
                    {
                        if (current.gCell.fillPathToSpawner != null)
                        {
                            if (current.gCell.fillPathToSpawner[current.gCell.fillPathToSpawner.Count - 1].spawner)
                            {
                                if (A.gCell.fillPathToSpawner != null)
                                {
                                    if (currentPath.Count + current.gCell.fillPathToSpawner.Count + 1 < A.gCell.fillPathToSpawner.Count)
                                    {
                                        A.gCell.fillPathToSpawner = new List<GridCell>();
                                        currentPath.Push(current.gCell);

                                        foreach (var c in currentPath)
                                        {
                                            c.pfCell.mather = c.pfCell;
                                            A.gCell.fillPathToSpawner.Add(c);
                                        }

                                        foreach (var c in current.gCell.fillPathToSpawner)
                                        {
                                            c.pfCell.mather = c.pfCell;
                                            A.gCell.fillPathToSpawner.Add(c);
                                        }
                                        currentPath.Pop();
                                    }
                                }

                                else
                                {

                                    A.gCell.fillPathToSpawner = new List<GridCell>();
                                    currentPath.Push(current.gCell);

                                    foreach (var c in currentPath)
                                    {
                                        c.pfCell.mather = c.pfCell;
                                        A.gCell.fillPathToSpawner.Add(c);
                                    }

                                    foreach (var c in current.gCell.fillPathToSpawner)
                                    {
                                        c.pfCell.mather = c.pfCell;
                                        A.gCell.fillPathToSpawner.Add(c);
                                    }

                                    currentPath.Pop();
                                }
                            }
                        }

                        else
                        {
                            //currentPath.Push(current.gCell);
                            current.mather = null;
                            CreatePathes(WorkMap, A, current, currentPath);

                            //currentPath.Pop();

                            //if (current.gCell.fillPathToSpawner != null)
                            //{
                            //    foreach (var c in currentPath)
                            //    {
                            //        current.gCell.fillPathToSpawner.Add(c);
                            //    }
                            //}

                            //currentPath.Pop();
                        }

                    }
                }
            }
        }


        public void CreatePathes(Map WorkMap, PFCell A, PFCell current, Stack<GridCell> currentPath)
        {
            if (current.mather == null)
            {
                current.mather = current;

                if (current.Neighbors.Top != null)
                {
                    //currentPath.Push(current.Neighbors.Top);
                    finding(WorkMap, A, current.Neighbors.Top.pfCell, currentPath);
                    //currentPath.Pop();
                }

                if (current.Neighbors.Left != null)
                {
                    //currentPath.Push(current.Neighbors.Left);
                    finding(WorkMap, A, current.Neighbors.Left.pfCell, currentPath);
                    //currentPath.Pop();
                }

                if (current.Neighbors.Right != null)
                {
                    //currentPath.Push(current.Neighbors.Right);
                    finding(WorkMap, A, current.Neighbors.Right.pfCell, currentPath);
                    //currentPath.Pop();
                }

                //if (current.Neighbors.Bottom != null)
                //{
                //    //currentPath.Push(current.Neighbors.Bottom);
                //    finding(WorkMap, A, current.Neighbors.Bottom.pfCell, currentPath);
                //    //currentPath.Pop();
                //}

            }

        }


        public void CreatePathes(Map WorkMap, PFCell A, PFCell B, int length)
        {























            ////spawn cell과 같은 경우를 찾기 위해
            ////랭스가 이미찾은거보다 크면 리턴
            //if(length > fullPath.Count) return;

            ////





            //if(A.Neighbors.Top != null)
            //{
            //    if (!A.Neighbors.Top.Blocked && !A.Neighbors.Top.IsDisabled && !A.Neighbors.Top.MovementBlocked)
            //    {
            //        if(A.mather == null)
            //        {
            //            if (A.Neighbors.Top.spawner)
            //            {
            //                A.mather = A.Neighbors.Top.pfCell;
            //                fullPath.Add(A.Neighbors.Top.pfCell);
            //                return;
            //            }

            //            else if (A.Neighbors.Top.fillPathToSpawner[A.Neighbors.Top.fillPathToSpawner.Count - 1] == B.gCell)
            //            {
            //                A.mather = A.Neighbors.Top.pfCell;
            //                fullPath.Add(A.Neighbors.Top.pfCell);

            //                foreach(var c in A.Neighbors.Top.fillPathToSpawner) fullPath.Add(c.pfCell);
            //                return;
            //            }

            //            else
            //            {
            //                CreatePathes(WorkMap, A.Neighbors.Top.pfCell, B, length + 1);
            //            }
            //        }

            //        else
            //        {
            //            if (A.Neighbors.Top.spawner)
            //            {
            //                if(length < A.Neighbors.Top.fillPathToSpawner.Count)
            //                {
            //                    A.mather = A.Neighbors.Top.pfCell;
            //                    fullPath.Add(A.Neighbors.Top.pfCell);
            //                }
            //            }

            //            else if (A.Neighbors.Top.fillPathToSpawner[A.Neighbors.Top.fillPathToSpawner.Count - 1] == B.gCell)
            //            {

            //                if (length < A.Neighbors.Top.fillPathToSpawner.Count)
            //                {
            //                    A.mather = A.Neighbors.Top.pfCell;
            //                    fullPath.Add(A.Neighbors.Top.pfCell);

            //                    foreach (var c in A.Neighbors.Top.fillPathToSpawner) fullPath.Add(c.pfCell);
            //                }
            //            }

            //            else
            //            {
            //                CreatePathes(WorkMap, A.Neighbors.Top.pfCell, B, length + 1);
            //            }
            //        }
            //    }





            //}

            ////if (A.Neighbors.Left != null)
            ////{
            ////    if (A.Neighbors.Left.fillPathToSpawner[A.Neighbors.Left.fillPathToSpawner.Count - 1] == B.gCell)
            ////    {
            ////        fullPath.Add(A.Neighbors.Left.pfCell);
            ////        return;
            ////    }

            ////    else CreatePathes(WorkMap, A.Neighbors.Left.pfCell, B);
            ////}

            ////if (A.Neighbors.Right != null)
            ////{
            ////    if (A.Neighbors.Right.fillPathToSpawner[A.Neighbors.Right.fillPathToSpawner.Count - 1] == B.gCell)
            ////    {
            ////        fullPath.Add(A.Neighbors.Right.pfCell);
            ////        return;
            ////    }

            ////    else CreatePathes(WorkMap, A.Neighbors.Right.pfCell, B);
            ////}

            ////if (A.Neighbors.Bottom != null)
            ////{
            ////    if (A.Neighbors.Bottom.fillPathToSpawner[A.Neighbors.Bottom.fillPathToSpawner.Count - 1] == B.gCell)
            ////    {
            ////        fullPath.Add(A.Neighbors.Bottom.pfCell);
            ////        return;
            ////    }

            ////    else CreatePathes(WorkMap, A.Neighbors.Bottom.pfCell, B);
            ////}




            ////fullPath = null;
            ////if (WorkMap == null || A == null || B == null || !A.available || !B.available) return;


            //////if (!IsWayCreated(A, B))
            ////if (!IsWayCreated(A, B))
            ////{
            ////    CreateGlobWayMa11111p(WorkMap, A, B);
            ////    if (IsWayExistTo(B))
            ////    {
            ////        fullPath = new List<PFCell>();
            ////        fullPath.Add(B);
            ////        PFCell mather = B.mather;
            ////        while (mather != A.mather)
            ////        {
            ////            fullPath.Add(mather);
            ////            mather = mather.mather;
            ////        }
            ////        fullPath.Reverse();
            ////    }

            ////}

        }



        public void CreatePath(Map WorkMap, PFCell A, PFCell B)
        {
            //   UnityEngine.Debug.Log(A + " : " + B);
            fullPath = null;
            if (WorkMap == null || A == null || B == null || !A.available || !B.available) return;

            //if (!IsWayCreated(A, B))
            if (!IsWayCreated(A, B))
            {
                CreateGlobWayMap(WorkMap, A, B);
                if (IsWayExistTo(B))
                {
                    fullPath = new List<PFCell>();
                    fullPath.Add(B);
                    PFCell mather = B.mather;
                    while (mather != A.mather)
                    {
                        fullPath.Add(mather);
                        mather = mather.mather;
                    }
                    fullPath.Reverse();
                }
                //else
                //{
                //    fullPath.Add(A);
                //}
            }




            ////   UnityEngine.Debug.Log(A + " : " + B);
            //fullPath = null;
            //if (WorkMap == null || A == null || B == null || !A.available || !B.available) return;

            ////if (!IsWayCreated(A, B))
            //if (!IsWayCreated(A, B))
            //{
            //    CreateGlobWayMap(WorkMap, A, B);
            //    if (IsWayExistTo(B))
            //    {
            //        fullPath = new List<PFCell>();
            //        fullPath.Add(B);
            //        PFCell mather = B.mather;
            //        while (mather != A.mather)
            //        {
            //            fullPath.Add(mather);
            //            mather = mather.mather;
            //        }
            //        fullPath.Reverse();
            //    }
            //    //else
            //    //{
            //    //    fullPath.Add(A);
            //    //}
            //}
        }

        /// <summary>
        /// Create the shortest path if exist, else fullPath set to null
        /// </summary>
        /// <param name="WorkMap"></param>
        /// <param name="A"></param>
        /// <param name="B"></param>
        public void CreatePath(Map WorkMap, PFCell A, List<PFCell> B)
        {
            fullPath = null;
            if (WorkMap == null || A == null || B == null || B.Count == 0 || !A.available) return;

            List<PFCell> tempPath;
            CreateGlobWayMap(WorkMap, A);
            foreach (var item in B)
            {
                if (item.available)
                {
                    if (IsWayExistTo(item))
                    {
                        tempPath = new List<PFCell>();
                        tempPath.Add(item);

                        PFCell mather = item.mather;
                        while (mather != A.mather)
                        {
                            tempPath.Add(mather);
                            mather = mather.mather;
                        }
                        tempPath.Reverse();
                        if (fullPath == null || fullPath.Count > tempPath.Count) fullPath = tempPath;
                    }
                }
            }
        }

        /// <summary>
        /// Create the shortest path if exist, else fullPath set to null
        /// </summary>
        /// <param name="WorkMap"></param>
        /// <param name="A"></param>
        /// <param name="B"></param>
        public void CreatePathToTop(Map WorkMap, PFCell A)
        {
            fullPath = null;
            if (WorkMap == null || A == null) return;

            List<PFCell> tempPath;
            CreateGlobWayMap(WorkMap, A);
            PFCell mather;
            List<PFCell> topAvailable = new List<PFCell>();
            int minRow = int.MaxValue;

            // get top available cells
            foreach (var item in WorkMap.PFCells)
            {
                if (IsWayExistTo(item))
                {
                    if (minRow >= item.row)
                    {
                        minRow = item.row;
                        topAvailable.Add(item);
                    }
                    else
                    {
                        break;
                    }
                }
            }
            // UnityEngine.Debug.Log("min row :" + minRow);

            // create shortest path to top available cells
            foreach (var item in topAvailable)
            {
                if (item.row == minRow)
                {
                    tempPath = new List<PFCell>(topAvailable.Count);
                    tempPath.Add(item);

                    mather = item.mather;
                    while (mather != A.mather)
                    {
                        tempPath.Add(mather);
                        mather = mather.mather;
                    }
                    tempPath.Reverse();
                    if (fullPath == null || fullPath.Count > tempPath.Count) fullPath = tempPath;
                }
            }
            //  UnityEngine.Debug.Log("Path to top created " + DebugPath());
        }

        //private void CreatePathThread(Map WorkMap, PFCell A, PFCell B)
        //{
        //    ThreadPool.QueueUserWorkItem(m => CreatePath(WorkMap, A, B));
        //}

        private bool IsWayExistTo(PFCell B)
        {
            return (B.HaveMather() && B.available); 
        }

        public int PathLenght { get { return (fullPath == null)? int.MaxValue : fullPath.Count; } }

        public List<PFCell> GetAvailablePFPositionAround(Map WorkMap, PFCell A, int distance)
        {
            List<PFCell> lPos = new List<PFCell>();
            CreateGlobWayMap(WorkMap, A);
            foreach (var item in WorkMap.PFCells)
            {
                if (IsWayExistTo(item) && item.GetDistanceTo(A) == distance)
                {
                    lPos.Add(item);
                }
            }
            return lPos;
        }

        public string DebugPath()
        {
            string debugString = "";
            if (fullPath != null)
            {
                foreach (var item in fullPath)
                {
                    if (item != null)
                    {
                        debugString += item.ToString();
                    }
                    else
                    {
                        debugString += "null";
                    }
                }
            }
            return debugString;
        }


    }
}
