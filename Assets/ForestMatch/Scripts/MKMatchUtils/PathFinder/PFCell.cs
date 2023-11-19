﻿using Mkey;
using Mono.Collections.Generic;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Mkey
{
    [System.Serializable]


    //public class PFCell
    public class PFCell : IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        //// you can use ConcurrentQueue if you need thread-safe solution
        //private readonly Queue<IDisposable> _disposables = new();

        //public T Using<T>(T disposable) where T : IDisposable
        //{
        //    _disposables.Enqueue(disposable);
        //    return disposable;
        //}

        //public void Dispose()
        //{
        //    foreach (var item in _disposables)
        //    {
        //        item.Dispose();
        //        GC.SuppressFinalize(item);
        //    }

        //}





        public bool available; 
        public PFCell mather;  //public byte openClose = 0; public int fCost = 0;public int gCost = 0; public int hCost = 0;public int stepNumber = 0;
        public int row;
        public int col;
        public NeighBors Neighbors { get; private set; }

        //public GridCell gCell { get; private set; }

        public GridCell gCell;

        public PFCell()
        {
        }

        public PFCell(GridCell gCell)
        {
            this.gCell = gCell;
            mather = null;
            row = gCell.Row;
            col = gCell.Column;
        }

        public void CreateNeighBorns()
        {
            if (gCell == null) return;
            //NeighBors nBs = gCell.Neighbors;// new NeighBorns(gCell); // gridcell neighborns
            //Neighbors = new List<PFCell>(nBs.Cells.Count);

            //foreach (var n in nBs.Cells)
            //{
            //    Neighbors.Add(n.pfCell);
            //}
            Neighbors = gCell.Neighbors;
        }

        public bool IsPassabilityFrom(PFCell a) 
        {




            ////// min 2 neighborns isavailabe
            ////List<PFCell> availableNeighBorns = GetAvailableNeighBorns();
            ////if (availableNeighBorns.Count == 6) return true;

            ////foreach (var item in availableNeighBorns)
            ////{
            ////    if (item.IsNeighBorn(a)) return true;
            ////}
            ////return false;
            return true;
        }

        public List<PFCell> GetAvailableNeighBorns()
        {
            List<PFCell> availableNeighBorns = new List<PFCell>(Neighbors.Cells.Count);
            foreach (var item in Neighbors.Cells)
            {
                if (item.pfCell.available)
                    availableNeighBorns.Add(item.pfCell);
            }
            return availableNeighBorns;
        }

        /// <summary>
        /// Return mather != null
        /// </summary>
        /// <returns></returns>
        public bool HaveMather()
        {
            return mather != null;
        }

        public int GetDistanceTo(PFCell other)
        {
            return Mathf.Abs(other.row - row) + Mathf.Abs(other.col - col);
        }

        public override string ToString()
        {
            string res = (available) ? "available; " : "not available; ";
            res += ((HaveMather()) ? "have mather; " : "not mather; ");
            if (gCell)
                res += gCell.ToString();

            else res+=" gcell null ";
            return res;
        }

        public bool IsNeighBorn(PFCell a)
        {
            foreach (var item in Neighbors.Cells)
            {
                if (item.pfCell == a) return true;
            }
            return false;
        }
    }

}


