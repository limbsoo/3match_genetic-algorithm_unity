﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Unity.Burst.CompilerServices;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.VersionControl;
using UnityEditorInternal.VersionControl;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.VirtualTexturing;
using UnityEngine.UI;
using UnityEngine.UIElements;
using static Mkey.Match3Helper;
using static UnityEditor.PlayerSettings;
using static UnityEditor.Progress;
using static UnityEngine.GraphicsBuffer;
using Random = UnityEngine.Random;


namespace Mkey
{
    public class MatchGrid
    {
        private GameObjectsSet goSet;

        public List<Column<GridCell>> Columns { get; private set; }
        public List<GridCell> Cells { get; private set; }

        /// <summary>
        /// ///////////////////////////////////////////////////
        /// </summary>
        public List<GridCell> CellsID { get; private set; }
        /// ///////////////////////////////////////////////////


        public List<Row<GridCell>> Rows { get; private set; }
        public Transform Parent { get; private set; }
        private int sortingOrder;
        private GameMode gMode;
        private int  vertSize;
        private int horSize;
        private GameObject prefab;
        private float yStart; // Camera.main.orthographicSize - radius
        private float yStep;
        private float xStep;
        private int yOffset;
        private Vector2 cellSize;
        private float cOffset;
        public bool haveFillPath = false;
        public Vector2 Step { get { return new Vector2(xStep, yStep); } }
        public LevelConstructSet LcSet { get; private set; }

        public MatchGrid(LevelConstructSet lcSet, GameObjectsSet goSet, Transform parent, int sortingOrder, GameMode gMode)
        {
            this.LcSet = lcSet;
            this.Parent = parent;
            this.gMode = gMode;
            this.sortingOrder = sortingOrder;
            Debug.Log("new grid " + lcSet.name);

            vertSize = lcSet.VertSize;
            horSize = lcSet.HorSize;
            this.goSet = goSet;
            prefab = goSet.gridCellEven;
            cellSize = prefab.GetComponent<BoxCollider2D>().size;

            float deltaX = lcSet.DistX;
            float deltaY = lcSet.DistY;
            float scale = lcSet.Scale;
            parent.localScale = new Vector3(scale, scale, scale);

            Cells = new List<GridCell>(vertSize * horSize);
            Rows = new List<Row<GridCell>>(vertSize);

            yOffset = 0;
            xStep = (cellSize.x + deltaX);
            yStep = (cellSize.y + deltaY);
           
            cOffset = (1 - horSize) * xStep/2.0f; // offset from center by x-axe
            yStart = (vertSize-1)  * yStep / 2.0f;

            //instantiate cells
            for (int i = 0; i < vertSize; i++)
            {
                AddRow();
            }

            //readLevelConstructSet
            SetObjectsData(lcSet, gMode);

            //newLevelConstruct
            //AssignObjectData(lcSet, gMode);


            Debug.Log("create cells: " + Cells.Count);
        }

        public void Rebuild(GameObjectsSet mSet, GameMode gMode)
        {
            Debug.Log("rebuild ");

            this.LcSet = LcSet;
            vertSize = LcSet.VertSize;
            horSize = LcSet.HorSize;

            float deltaX = LcSet.DistX;
            float deltaY = LcSet.DistY;
            float scale = LcSet.Scale;
            Parent.localScale = new Vector3(scale,scale,scale);

            this.goSet = mSet;
            Cells.ForEach((c) => { c.DestroyGridObjects(); });

            List<GridCell> tempCells = Cells;
            Cells = new List<GridCell>(vertSize * horSize + horSize);
            Rows = new List<Row<GridCell>>(vertSize);

            xStep = (cellSize.x + deltaX);
            yStep = (cellSize.y + deltaY);

            cOffset = (1 - horSize) * xStep / 2.0f; // offset from center by x-axe
            yStart = (vertSize - 1) * yStep / 2.0f;

            // create rows 
            GridCell cell;
            Row<GridCell> row;
            int cellCounter = 0;
            int ri = 0;
            Sprite sRE = mSet.gridCellEven.GetComponent<SpriteRenderer>().sprite;
            Sprite sRO = mSet.gridCellOdd.GetComponent<SpriteRenderer>().sprite;

            for (int i = 0; i < vertSize; i++)
            {
                bool isEvenRow = (i % 2 == 0);
                row = new Row<GridCell>(horSize);

                for (int j = 0; j < row.Length; j++)
                {
                    bool isEvenColumn = (j % 2 == 0);
                    Vector3 pos = new Vector3(j * xStep + cOffset, yStart - i * yStep, 0);

                    if (tempCells != null && cellCounter < tempCells.Count)
                    {
                        cell = tempCells[cellCounter];
                        cell.gameObject.SetActive(true);
                        cell.transform.localPosition = pos;
                        cellCounter++;
                        SpriteRenderer sR = cell.GetComponent<SpriteRenderer>();
                        if (sR)
                        {
                            sR.enabled = true;
                            if (isEvenRow) sR.sprite = (!isEvenColumn) ? sRO : sRE;
                            else sR.sprite = (isEvenColumn) ? sRO : sRE;
                        }
                    }
                    else
                    {
                        if (isEvenRow)
                            cell = UnityEngine.Object.Instantiate((!isEvenColumn) ? mSet.gridCellOdd : mSet.gridCellEven).GetComponent<GridCell>();
                        else
                            cell = UnityEngine.Object.Instantiate((isEvenColumn) ? mSet.gridCellOdd : mSet.gridCellEven).GetComponent<GridCell>();
                        cell.transform.parent = Parent;
                        cell.transform.localPosition = pos;
                        cell.transform.localScale = Vector3.one;
                    }


                    Cells.Add(cell);
                    row[j] = cell;
                }
                Rows.Add(row);
                ri++;
            }

            // destroy not used cells
            if (cellCounter < tempCells.Count)
            {
                for (int i = cellCounter; i < tempCells.Count; i++)
                {
                    UnityEngine.Object.Destroy(tempCells[i].gameObject);
                }
            }

            // cache columns
            Columns = new List<Column<GridCell>>(horSize);
            Column<GridCell> column;
            for (int c = 0; c < horSize; c++)
            {
                column = new Column<GridCell>(Rows.Count);
                for (int r = 0; r < Rows.Count; r++)
                {
                    column[r] = Rows[r][c];
                }
                Columns.Add(column);
            }

            for (int r = 0; r < Rows.Count; r++)
            {
                for (int c = 0; c < horSize; c++)
                {
                    Rows[r][c].Init(r, c, Columns[c], Rows[r], gMode);
                }
            }
            SetObjectsData(LcSet, gMode);

            Debug.Log("rebuild cells: " + Cells.Count);
        }

        /// <summary>
        /// set objects data from featured list to grid
        /// </summary>
        /// <param name="featCells"></param>
        /// <param name="gMode"></param>
        private void SetObjectsData(LevelConstructSet lcSet, GameMode gMode)
        {
            //MainLCSet에서 isDisabled 전달

            if (lcSet.cells != null)
                foreach (var c in lcSet.cells)
                {
                    if (c != null && c.gridObjects != null)
                    {
                        foreach (var o in c.gridObjects)
                        {
                            if (c.row < Rows.Count && c.column < Rows[c.row].Length) Rows[c.row][c.column].SetObject(o.id, o.hits);
                        }
                    }
                }
        }


        //private void AssignObjectData(LevelConstructSet lcSet, GameMode gMode)
        //{
        //    //int cellsCnt = 5;

        //    Rows[0][1].SetObject(100, 0);
        //    Rows[0][2].SetObject(500000, 0);
        //    Rows[2][1].SetObject(1, 0);

        //    //Rows[3][0].SetObject(100, 0);
        //    //Rows[3][1].SetObject(100, 0);
        //    //Rows[3][2].SetObject(100, 0);
        //    //Rows[3][3].SetObject(100, 0);
        //}



        /// <summary>
        /// Add row to grid
        /// </summary>
        private void AddRow()
        {
            GridCell cell;
            Row<GridCell> row = new Row<GridCell>(horSize);
            bool isEvenRow = (Rows.Count % 2 == 0);
            for (int j = 0; j < row.Length; j++)
            {
                bool isEvenColumn = (j % 2 == 0);
                // pos를 고정시키면 셀 생성 X 
                Vector3 pos = new Vector3(j * xStep + cOffset, yStart + yOffset * yStep, 0);


                if (isEvenRow)
                    cell = UnityEngine.Object.Instantiate((!isEvenColumn) ? goSet.gridCellOdd : goSet.gridCellEven).GetComponent<GridCell>();
                else
                    cell = UnityEngine.Object.Instantiate((isEvenColumn) ? goSet.gridCellOdd : goSet.gridCellEven).GetComponent<GridCell>();

                cell.transform.parent = Parent;

                cell.transform.localPosition = pos;
                cell.transform.localScale = Vector3.one;
                Cells.Add(cell);
                row[j] = cell;
            }

            Rows.Add(row);

            // cache columns
            Columns = new List<Column<GridCell>>(horSize);
            Column<GridCell> column; 
            for (int c = 0; c < horSize; c++)
            {
                column = new Column<GridCell>(Rows.Count);
                for (int r = 0; r < Rows.Count; r++)
                {
                    column[r] = Rows[r][c];
                }
                Columns.Add(column);
            }

      //      Debug.Log("rows: " + Rows.Count +  " ;columns count: " + columns.Count);
            for (int r = 0; r < Rows.Count; r++)
            {
                for (int c = 0; c < horSize; c++)
                {
                    Rows[r][c].Init(r, c, Columns[c], Rows[r], gMode);
                }
            }

            yOffset--;
        }

        public GridCell this[int index0, int index1]
        {
            get { if (ok(index0, index1)) { return Rows[index0][index1]; } else { return null; } }
            set { if (ok(index0, index1)) { Rows[index0][index1] = value; } else {  } }
        }

        private bool ok(int index0, int index1)
        {
            return (index0 >= 0 && index0 < vertSize && index1 >= 0 && index1 < horSize);
        }

        /// <summary>
        ///  return true if cells not simulate physics
        /// </summary>
        /// <returns></returns>
        internal bool NoPhys()
        {
            foreach (GridCell c in Cells)
            {
                if (c.PhysStep) return false;
            }
            return true;
        }

        internal bool NoPhys1(MatchGrid g)
        {
            foreach (GridCell c in g.Cells)
            {
                if (c.PhysStep) return false;
            }
            return true;
        }


        #region  get data from grid
        public MatchGroupsHelper GetMatches(int minMatch)
        {
            MatchGroupsHelper mgh = new MatchGroupsHelper(this);
            mgh.CreateGroups(minMatch, false);
            return mgh;
        }

        internal List<GridCell> GetEqualCells(GridCell gCell)
        {
            List<GridCell> gCells = new List<GridCell>();
            for (int i = 0; i < Cells.Count; i++)
            {
                if (Cells[i].IsMatchObjectEquals(gCell))
                {
                    gCells.Add(Cells[i]);
                }
            }
            return gCells;
        }

        internal List<GridCell> GetNeighCells(GridCell gCell, bool useDiagCells)
        {
            List<GridCell> nCells = new List<GridCell>();
            int row = gCell.Row;
            int column = gCell.Column;

            GridCell c = this[row, column - 1]; if (c) nCells.Add(c); // left
            c = this[row - 1, column]; if (c) nCells.Add(c); //  top
            c = this[row, column + 1]; if (c) nCells.Add(c); // right
            c = this[row + 1, column]; if (c) nCells.Add(c); // bot

            if (useDiagCells)
            {
                c = this[row + 1, column - 1]; if (c) nCells.Add(c); // bot - left
                c = this[row - 1, column - 1]; if (c) nCells.Add(c); // top - left
                c = this[row - 1, column + 1]; if (c) nCells.Add(c); // top right
                c = this[row + 1, column + 1]; if (c) nCells.Add(c); // bot- right
            }
            return nCells;
        }



        /// <summary>
        /// Return not blocked, not disabled cells without dynamic object
        /// </summary>
        /// <returns></returns>
        internal List<GridCell> GetFreeCells(MatchGrid g, bool withPath)
        {
            List<GridCell> gcL = new List<GridCell>();
            for (int i = 0; i < g.Cells.Count; i++)
            {
                if (g.Cells[i].IsDynamicFree && !g.Cells[i].Blocked && !g.Cells[i].IsDisabled)
                {
                    //if (withPath && g.Cells[i].HaveFillPath() || !withPath)
                        gcL.Add(g.Cells[i]);
                }
            }
            return gcL;
        }

        /// <summary>
        /// Return not blocked, not disabled cells without dynamic object, with fillPath or with and without
        /// </summary>
        /// <returns></returns>
        internal List<GridCell> GetFreeCells(bool withPath)
        {
            List<GridCell> gcL = new List<GridCell>();
            for (int i = 0; i < Cells.Count; i++)
            {

                if (Cells[i].IsDynamicFree && !Cells[i].Blocked && !Cells[i].IsDisabled)
                {
                    if (withPath && Cells[i].HaveFillPath() || !withPath)
                        gcL.Add(Cells[i]);
                }
            }
            return gcL;
        }

        /// <summary>
        /// Return objects count on grid with selected ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int GetObjectsCountByID(int id)
        {
            int res = 0;
            GridObject[] bds = Parent.GetComponentsInChildren<GridObject>();
            foreach (var item in bds)
            {
                if (item.ID == id) res++;
            }

            return res;
        }

        /// <summary>
        /// Return cells with object ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<GridCell> GetAllByID(int id)
        {
            List<GridCell> res = new List<GridCell>();
            foreach (var item in Cells)
            {
                if (item.HaveObjectWithID(id))
                {
                    res.Add(item);
                }
            }
            return res;
        }

        public void CalcObjects()
        {
            GridObject[] bds = Parent.GetComponentsInChildren<GridObject>();
            Debug.Log("Objects count: " + bds.Length);
        }

        /// <summary>
        /// Get chess distance
        /// </summary>
        /// <returns></returns>
        public static int GetChessDist(GridCell gc1, GridCell gc2)
        {
            return (Mathf.Abs(gc1.Row - gc2.Row) + Mathf.Abs (gc1.Column - gc2.Column));
        }

        /// <summary>
        /// Get chess distance
        /// </summary>
        /// <returns></returns>
        public static GridCell GetChessNear(GridCell gCell, IEnumerable<GridCell> area)
        {
            int dist = Int32.MaxValue;
            GridCell nearItem = null;
            if (gCell && area != null)
            {
                foreach (GridCell c in area)
                {
                    int dist2 = GetChessDist(c, gCell);
                    if (dist2 < dist)
                    {
                        nearItem = c;
                        dist = dist2;
                    }
                }
            }
            return nearItem;
        }

        /// <summary>
        /// Return random match cell list
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public List<GridCell> GetRandomMatch(int count)
        {
            List<GridCell> temp = new List<GridCell>(Cells.Count);
            List<GridCell> res = new List<GridCell>(count);

            foreach (var item in Cells)
            {
                if (item.Match && !item.Overlay && !item.Underlay)
                {
                    temp.Add(item);
                }
            }
            temp.Shuffle();
            count = Mathf.Min(count, temp.Count);

            for (int i = 0; i < count; i++)
            {
                res.Add(temp[i]);
            }
            return res;
        } 

        public GridCell GetBomb()
        {
            foreach (var item in Cells)
            {
                if (item.HasBomb)
                {
                    if(item.Match && item.IsMatchable)
                        return item;
                    if (!item.Match)
                        return item;
                }
            }
            return null;
        }

        public List<GridCell> GetBottomDynCells()
        {
            // get all mather cells
            List<GridCell> mathers = new List<GridCell>();
            Cells.ForEach((c) =>
            {
                if (c.GravityMather && !mathers.Contains(c.GravityMather))
                {
                    mathers.Add(c.GravityMather);
                }
            });

            List<GridCell> res = new List<GridCell>();
            Cells.ForEach((c) =>
            {
                if (c.DynamicObject || (!c.Blocked && !c.IsDisabled))
                {
                    if (!mathers.Contains(c) && !res.Contains(c))
                    {
                        res.Add(c);
                    }
                }
            });

            return res;
        }

        public Row<GridCell> GetRow(int row)
        {
            return (row >= 0 && row < Rows.Count) ? Rows[row] : null;
        }

        public Column<GridCell> GetColumn(int col)
        {
            return (col >= 0 && col < Columns.Count) ? Columns[col] : null;
        }

        public  CellsGroup GetWave(GridCell gCell, int radius)
        {
            radius = Mathf.Max(0,radius);
            CellsGroup res = new CellsGroup();
            int row1 = gCell.Row - radius;
            int row2 = gCell.Row + radius;
            int col1 = gCell.Column - radius;
            int col2 = gCell.Column + radius;
            Row<GridCell> topRow = GetRow(row1);
            Row<GridCell> botRow = GetRow(row2);
            Column<GridCell> leftCol = GetColumn(col1);
            Column<GridCell> rightCol = GetColumn(col2);

            if (topRow != null)
            {
                for (int i  = col1; i <= col2; i++)
                {
                    if (ok(row1, i)) res.Add(topRow[i]);
                }
            }

            if (rightCol != null)
            {
                for (int i = row1; i <= row2; i++)
                {
                    if (ok(i, col2)) res.Add(rightCol[i]);
                }
            }

            if (botRow != null)
            {
                for (int i = col2; i >= col1; i--)
                {
                    if (ok(row2, i)) res.Add(botRow[i]);
                }
            }

            if (leftCol != null)
            {
                for (int i = row2; i >= row1; i--)
                {
                    if (ok(i, col1)) res.Add(leftCol[i]);
                }
            }

            return res;
        }

        public CellsGroup GetAroundArea(GridCell gCell, int radius)
        {
            radius = Mathf.Max(0, radius);
            CellsGroup res = new CellsGroup();
            if (radius > 0)
                for (int i = 1; i <= radius; i++)
                {
                    res.AddRange(GetWave(gCell, i).Cells);
                }
            return res;
        }

        /// <summary>
        /// Return gridcells group with id matched  around gCell
        /// </summary>
        /// <param name="gCell"></param>
        /// <returns></returns>
        public MatchGroup GetMatchIdArea(GridCell gCell)
        {
            MatchGroup res = new MatchGroup();
            if (!gCell.Match || !gCell.IsMatchable) return res;

            MatchGroup equalNeigh = new MatchGroup();
            MatchGroup neighTemp;
            int id = gCell.Match.ID;
            res.Add(gCell);

            equalNeigh.AddRange(gCell.Neighbors.GetMatchIdCells(id, true)); //equalNeigh.AddRange(gCell.EqualNeighBornCells());
            while (equalNeigh.Length > 0)
            {
                res.AddRange(equalNeigh.Cells);
                neighTemp = new MatchGroup();
                foreach (var item in equalNeigh.Cells)
                {
                    neighTemp.AddRange(item.Neighbors.GetMatchIdCells(id, true)); // neighTemp.AddRange(item.EqualNeighBornCells());
                }
                equalNeigh = neighTemp;
                equalNeigh.Remove(res.Cells);
            }
            return res;
        }
        #endregion  get data from grid


        public char getRandomGene()
        {
            int number = Random.Range(0, 2);
            return (char)(number + '0');
        }
        public char[] getGenes()
        {
            char[] genes = new char[Cells.Count];

            string s = m3h.setMaps(m3h.csvCnt, true);

            for (int i = 0; i < Cells.Count; i++)
            {
                genes[i] = s[i];
            }


            return genes;




            //return m3h.difficults[m3h.csvCnt].map;

            //char[] genes = new char[Cells.Count];

            //string[] map = {

            //    "" ,
            //    "" ,
            //    "" ,
            //    "" ,
            //    "" ,
            //    "" ,
            //    "" ,
            //    "" ,
            //    "" ,
            //    ""
            //};

            //int[] difficult =
            //{
            //   561 ,
            //   501 ,
            //   456 ,
            //   406 ,
            //   350 ,
            //   329 ,
            //   268 ,
            //   246 ,
            //   165 ,
            //   134
            //};

            //for (int i = 0; i < map[m3h.csvCnt].Length; i++) genes[i] = map[m3h.csvCnt][i];

            //m3h.wantDifficulty = difficult[m3h.csvCnt];
            //return genes;
        }




        public void destoryAllCellInGrid()
        {
            for (int i = 0; i < m3h.grid.Cells.Count; i++)
            {
                m3h.grid.Cells[i].DestroyGridObjects();
            }
        }

        //public void setGridPerProtection(DNA<char> p, SpawnController sC)
        //{
        //    BlockedObject b = sC.GetObstacleObjectPrefab(LcSet, goSet);
        //    BlockedObject b0 = sC.GetSelectBreakableBlockedObjectPrefab(LcSet, goSet);
        //    OverlayObject o = sC.GetSelectOverlayObjectPrefab(LcSet, goSet);

        //    for(int i = 0; i < 6; i++)
        //    {
        //        destoryAllCellInGrid();

        //        for (int j = 0; j < m3h.grid.Cells.Count; j++)
        //        {
        //            if (p.cellsID[j] == p.blocks[i].kind && m3h.protections[j] == p.blocks[i].protection)
        //            {
        //                if(p.blocks[i].kind == 1)
        //                {
        //                    m3h.grid.Cells[j].SetObject1(b0);
        //                    p.blocks[i].cnt++;
        //                }

        //                else if (p.blocks[i].kind == 2)
        //                {
        //                    MatchObject m = sC.GetRandomObjectPrefab(LcSet, goSet);
        //                    m3h.grid.Cells[j].SetObject1(m);
        //                    m3h.grid.Cells[j].SetObject1(o);
        //                    p.blocks[i].cnt++;
        //                }

        //            }

        //            else
        //            {
        //                if (p.cellsID[j] == 0)
        //                {
        //                    m3h.grid.Cells[j].SetObject1(b);
        //                }

        //                else
        //                {
        //                    MatchObject m = sC.GetRandomObjectPrefab(LcSet, goSet);
        //                    m3h.grid.Cells[j].SetObject1(m);
        //                }
        //            }
        //        }

        //        if (p.blocks[i].cnt != 0) p.blocks[i].pottential = m3h.cntPerPottential(p);
        //        else p.blocks[i].pottential = 0;
        //    }

        //}


        public void setGridEachKind(DNA<char> p, SpawnController sC, bool isSpawnObstacle, bool isSpawnBlocked, bool isSpawnOverlay, bool isEachProtection, int protection)
        {
            BlockedObject b = sC.GetObstacleObjectPrefab(LcSet, goSet);
            BlockedObject b0 = sC.GetSelectBreakableBlockedObjectPrefab(LcSet, goSet);
            OverlayObject o = sC.GetSelectOverlayObjectPrefab(LcSet, goSet);

            destoryAllCellInGrid();

            if (!isEachProtection)
            {
                for (int i = 0; i < m3h.grid.Cells.Count; i++)
                {
                    if (isSpawnObstacle && p.gridObjects[i] == 0) m3h.grid.Cells[i].SetObject1(b);

                    else if (isSpawnBlocked && p.gridObjects[i] == 1) m3h.grid.Cells[i].SetObject1(b0);

                    else if (isSpawnOverlay && p.gridObjects[i] == 2)
                    {
                        MatchObject m = sC.GetRandomObjectPrefab(LcSet, goSet);
                        m3h.grid.Cells[i].SetObject1(m);
                        m3h.grid.Cells[i].SetObject1(o);
                    }

                    else
                    {
                        MatchObject m = sC.GetRandomObjectPrefab(LcSet, goSet);
                        m3h.grid.Cells[i].SetObject1(m);
                    }
                }
            }

            else
            {
                {
                    for (int i = 0; i < m3h.grid.Cells.Count; i++)
                    {
                        if (isSpawnObstacle && p.gridObjects[i] == 0) m3h.grid.Cells[i].SetObject1(b);

                        else if (isSpawnBlocked && p.gridObjects[i] == 1 && p.objectProtection[i] == protection) m3h.grid.Cells[i].SetObject1(b0);

                        else if (isSpawnOverlay && p.gridObjects[i] == 2 && p.objectProtection[i] == protection)
                        {
                            MatchObject m = sC.GetRandomObjectPrefab(LcSet, goSet);
                            m3h.grid.Cells[i].SetObject1(m);
                            m3h.grid.Cells[i].SetObject1(o);
                        }

                        else
                        {
                            MatchObject m = sC.GetRandomObjectPrefab(LcSet, goSet);
                            m3h.grid.Cells[i].SetObject1(m);
                        }
                    }
                }
            }
        }



        //public void setGridOnlyOne(DNA<char> p, SpawnController sC, int idx)
        //{
        //    BlockedObject b = sC.GetObstacleObjectPrefab(LcSet, goSet);
        //    BlockedObject b0 = sC.GetSelectBreakableBlockedObjectPrefab(LcSet, goSet);
        //    OverlayObject o = sC.GetSelectOverlayObjectPrefab(LcSet, goSet);

        //    for (int i = 0; i < m3h.grid.Cells.Count; i++)
        //    {
        //        if (p.cellsID[i] == idx)
        //        {
        //            switch (idx)
        //            {
        //                case 0:
        //                    m3h.grid.Cells[i].SetObject1(b);
        //                    break;
        //                case 1:
        //                    m3h.grid.Cells[i].SetObject1(b0);
        //                    m3h.grid.Cells[i].setProtection = m3h.protections[i];
        //                    break;
        //                case 2:
        //                    MatchObject m = sC.GetRandomObjectPrefab(LcSet, goSet);
        //                    m3h.grid.Cells[i].SetObject1(m);
        //                    m3h.grid.Cells[i].SetObject1(o);
        //                    m3h.grid.Cells[i].setProtection = m3h.protections[i];
        //                    break;
        //            }
        //        }

        //        else
        //        {
        //            /////////////////////////////////////////////
        //            if (p.cellsID[i] == 0)
        //            {
        //                m3h.grid.Cells[i].SetObject1(b);
        //            }

        //            else
        //            {
        //                MatchObject m = sC.GetRandomObjectPrefab(LcSet, goSet);
        //                m3h.grid.Cells[i].SetObject1(m);
        //            }
        //        }
        //    }

        //}


        public void setGridRepeat(DNA<char> p, SpawnController sC)
        {
            BlockedObject b = sC.GetObstacleObjectPrefab(LcSet, goSet);
            BlockedObject b0 = sC.GetSelectBreakableBlockedObjectPrefab(LcSet, goSet);
            OverlayObject o = sC.GetSelectOverlayObjectPrefab(LcSet, goSet);
            //b0.hitCnt = m3h.blockedObjectHitCnt;
            //o.hitCnt = m3h.overlayObjectHitCnt;

            for (int i = 0; i < m3h.grid.Cells.Count; i++)
            {
                if (p.gridObjects[i] == 0)
                {
                    m3h.grid.Cells[i].SetObject1(b);
                }

                else if (p.gridObjects[i] == 1)
                {
                    m3h.grid.Cells[i].SetObject1(b0);
                    m3h.grid.Cells[i].setProtection = p.objectProtection[i];
                }

                else if (p.gridObjects[i] == 2)
                {
                    MatchObject m = sC.GetRandomObjectPrefab(LcSet, goSet);
                    m3h.grid.Cells[i].SetObject1(m);
                    m3h.grid.Cells[i].SetObject1(o);
                    m3h.grid.Cells[i].setProtection = p.objectProtection[i];
                }

                else
                {
                    MatchObject m = sC.GetRandomObjectPrefab(LcSet, goSet);
                    m3h.grid.Cells[i].SetObject1(m);
                }
            }

        }


        //public void setGridRepeat(DNA<char> p, SpawnController sC)
        //{
        //    BlockedObject b = sC.GetObstacleObjectPrefab(LcSet, goSet);
        //    BlockedObject b0 = sC.GetSelectBreakableBlockedObjectPrefab(LcSet, goSet);
        //    OverlayObject o = sC.GetSelectOverlayObjectPrefab(LcSet, goSet);
        //    //b0.hitCnt = m3h.blockedObjectHitCnt;
        //    //o.hitCnt = m3h.overlayObjectHitCnt;

        //    for (int i = 0; i < m3h.grid.Cells.Count; i++)
        //    {
        //        if (p.cellsID[i] == 0)
        //        {
        //            m3h.grid.Cells[i].SetObject1(b);
        //        }

        //        else if (p.cellsID[i] == 1)
        //        {
        //            m3h.grid.Cells[i].SetObject1(b0);
        //            m3h.grid.Cells[i].setProtection = m3h.protections[i];
        //        } 

        //        else if(p.cellsID[i] == 2)
        //        {
        //            MatchObject m = sC.GetRandomObjectPrefab(LcSet, goSet);
        //            m3h.grid.Cells[i].SetObject1(m);
        //            m3h.grid.Cells[i].SetObject1(o);
        //            m3h.grid.Cells[i].setProtection = m3h.protections[i];
        //        }

        //        else
        //        {
        //            MatchObject m = sC.GetRandomObjectPrefab(LcSet, goSet);
        //            m3h.grid.Cells[i].SetObject1(m);
        //        }
        //    }

        //}

        //public void countOneKindObstacle(DNA<char> p, SpawnController sC)
        //{
        //    for (int i = 0; i < m3h.grid.Cells.Count; i++) m3h.grid.Cells[i].DestroyGridObjects();

        //    for (int i = 0; i < m3h.grid.Cells.Count; i++)
        //    {
        //        MatchObject m = sC.GetRandomObjectPrefab(LcSet, goSet);
        //        m3h.grid.Cells[i].SetObject1(m);
        //    }

        //    p.notObstacleCnt = m3h.cntPerPottential(p);

        //    for (int i = 0; i < m3h.grid.Cells.Count; i++) m3h.grid.Cells[i].DestroyGridObjects();
        //    setGridRepeat(p, sC);

        //    if (m3h.onlySpawnObstacleObject) p.obstaclePottentialCnt = m3h.cntPerPottential(p);
        //    else p.obstaclePottentialCnt = p.notObstacleCnt;

        //    if (m3h.onlySpawnBlockedObject) p.blockedPottentialCnt = m3h.cntPerPottential(p);
        //    else p.blockedPottentialCnt = p.notObstacleCnt;

        //    if (m3h.onlySpawnOverlayObject) p.overlayPottentialCnt = m3h.cntPerPottential(p);
        //    else p.overlayPottentialCnt = p.notObstacleCnt;
        //}


        //public void countPerObstacle(DNA<char> p, SpawnController sC)
        //{

        //    BlockedObject b = sC.GetObstacleObjectPrefab(LcSet, goSet);
        //    BlockedObject b0 = sC.GetSelectBreakableBlockedObjectPrefab(LcSet, goSet);
        //    OverlayObject o = sC.GetSelectOverlayObjectPrefab(LcSet, goSet);


        //    for (int i = 0; i < m3h.grid.Cells.Count; i++) m3h.grid.Cells[i].DestroyGridObjects();

        //    for (int i = 0; i < m3h.grid.Cells.Count; i++)
        //    {
        //        MatchObject m = sC.GetRandomObjectPrefab(LcSet, goSet);
        //        m3h.grid.Cells[i].SetObject1(m);
        //    }
        //    p.notObstacleCnt = m3h.cntPerPottential(p);

        //    for (int i = 0; i < m3h.grid.Cells.Count; i++) m3h.grid.Cells[i].DestroyGridObjects();

        //    for (int i = 0; i < m3h.grid.Cells.Count; i++)
        //    {
        //        if (p.cellsID[i] == 0) m3h.grid.Cells[i].SetObject1(b);
        //        //else if (p.gl[i] == 1) m3h.grid.Cells[i].SetObject1(b0);
        //        //else if (p.gl[i] == 2) m3h.grid.Cells[i].SetObject1(o);
        //        else
        //        {
        //            MatchObject m = sC.GetRandomObjectPrefab(LcSet, goSet);
        //            m3h.grid.Cells[i].SetObject1(m);
        //        }
        //    }
        //    p.obstaclePottentialCnt = m3h.cntPerPottential(p);

        //    for (int i = 0; i < m3h.grid.Cells.Count; i++) m3h.grid.Cells[i].DestroyGridObjects();

        //    for (int i = 0; i < m3h.grid.Cells.Count; i++)
        //    {
        //        if (p.cellsID[i] == 1) m3h.grid.Cells[i].SetObject1(b0);
        //        else
        //        {
        //            MatchObject m = sC.GetRandomObjectPrefab(LcSet, goSet);
        //            m3h.grid.Cells[i].SetObject1(m);
        //        }
        //    }
        //    p.blockedPottentialCnt = m3h.cntPerPottential(p);

        //    for (int i = 0; i < m3h.grid.Cells.Count; i++) m3h.grid.Cells[i].DestroyGridObjects();
        //    for (int i = 0; i < m3h.grid.Cells.Count; i++)
        //    {
        //        MatchObject m = sC.GetRandomObjectPrefab(LcSet, goSet);
        //        m3h.grid.Cells[i].SetObject1(m);

        //        if (p.cellsID[i] == 0) m3h.grid.Cells[i].SetObject1(o);
        //    }
        //    p.overlayPottentialCnt = m3h.cntPerPottential(p);


        //    for (int i = 0; i < m3h.grid.Cells.Count; i++) m3h.grid.Cells[i].DestroyGridObjects();
        //    setGridRepeat(p, sC);
        //}


        public void setGrid(DNA<char> p, SpawnController sC)
        {
            BlockedObject obstacle = sC.GetObstacleObjectPrefab(LcSet, goSet);
            BlockedObject blocked = sC.GetSelectBreakableBlockedObjectPrefab(LcSet, goSet);
            OverlayObject overlay = sC.GetSelectOverlayObjectPrefab(LcSet, goSet);

            ////////////////////////
            //bool isRandomObstacle = true;
            //bool isRandomProtection = true;
            //List<int> protections = new List<int>();

            List<int> kindsOfObstacle = new List<int>();

            if (m3h.spawnObstacleObject) kindsOfObstacle.Add(0);
            if (m3h.spawnBlockedObject) kindsOfObstacle.Add(1);
            if (m3h.spawnOverlayObject) kindsOfObstacle.Add(2);

            for (int i = 0; i < m3h.grid.Cells.Count; i++)
            {
                if (p.genes[i] == '1')
                {
                    int randomIndex = Random.Range(0, kindsOfObstacle.Count);
                    int randomProtection = 0;

                    if (kindsOfObstacle[randomIndex] == 0)
                    {
                        m3h.grid.Cells[i].SetObject1(obstacle);
                        p.gridObjects.Add(0);
                        p.objectProtection.Add(0);
                    } 

                    else if (kindsOfObstacle[randomIndex] == 1 || kindsOfObstacle[randomIndex] == 2)
                    {
                        if (kindsOfObstacle[randomIndex] == 1)
                        {
                            m3h.grid.Cells[i].SetObject1(blocked);
                            p.gridObjects.Add(1);
                        } 

                        else
                        {
                            MatchObject m = sC.GetRandomObjectPrefab(LcSet, goSet);
                            m3h.grid.Cells[i].SetObject1(m);
                            m3h.grid.Cells[i].SetObject1(overlay);
                            p.gridObjects.Add(2);
                        }

                        if (m3h.haveRandomProtection)
                        {
                            randomProtection = Random.Range(1, m3h.blockProtection + 1);
                            p.objectProtection.Add(randomProtection);
                        }

                        else
                        {
                            p.objectProtection.Add(m3h.blockProtection);
                        }
                        


                        //p.gridObjects.Add(kindsOfObstacle[randomIndex]);
                        //p.objectProtection.Add(randomProtection);

                    }


                    else
                    {
                        int a = 0;
                    }

                }

                else
                {
                    MatchObject m = sC.GetRandomObjectPrefab(LcSet, goSet);
                    m3h.grid.Cells[i].SetObject1(m);
                    p.gridObjects.Add(3);
                    p.objectProtection.Add(0);
                }
            }
            

            //if (isRandomObstacle)
            //{
            //    List<int> kindsOfObstacle = new List<int>();

            //    for (int i = 0; i < m3h.grid.Cells.Count; i++)
            //    {
            //        if (p.genes[i] == '1')
            //        {
            //            int randomIndex = Random.Range(0, kindsOfObstacle.Count + 1);

            //            if (kindsOfObstacle[randomIndex] == 0) m3h.grid.Cells[i].SetObject1(obstacle);

            //            else if (kindsOfObstacle[randomIndex] == 1 || kindsOfObstacle[randomIndex] == 2)
            //            {
            //                if (kindsOfObstacle[randomIndex] == 1) m3h.grid.Cells[i].SetObject1(blocked);

            //                else
            //                {
            //                    MatchObject m = sC.GetRandomObjectPrefab(LcSet, goSet);
            //                    m3h.grid.Cells[i].SetObject1(m);
            //                    m3h.grid.Cells[i].SetObject1(overlay);
            //                }

            //                if (m3h.haveRandomProtection)
            //                {
            //                    int randomProtection = Random.Range(1, m3h.blockProtection + 1);
            //                    protections.Add(randomProtection);
            //                }

            //                else protections.Add(m3h.blockProtection);

            //            }

            //            p.gridObjects.Add(kindsOfObstacle[randomIndex]);
            //        }

            //        else
            //        {
            //            MatchObject m = sC.GetRandomObjectPrefab(LcSet, goSet);
            //            m3h.grid.Cells[i].SetObject1(m);
            //            p.gridObjects.Add(3);
            //        }
            //    }
            //}

            //else
            //{
            //    for (int i = 0; i < m3h.grid.Cells.Count; i++)
            //    {
            //        if (m3h.difficults[m3h.csvCnt].map[i] == '0')
            //        {
            //            m3h.grid.Cells[i].SetObject1(obstacle);
            //            p.gridObjects.Add(0);
            //        }

            //        else if (m3h.difficults[m3h.csvCnt].map[i] == '1' || m3h.difficults[m3h.csvCnt].map[i] == '2')
            //        {
            //            if (m3h.difficults[m3h.csvCnt].map[i] == '1') m3h.grid.Cells[i].SetObject1(blocked);

            //            else
            //            {
            //                MatchObject m = sC.GetRandomObjectPrefab(LcSet, goSet);
            //                m3h.grid.Cells[i].SetObject1(m);
            //                m3h.grid.Cells[i].SetObject1(overlay);
            //            }

            //            protections.Add(m3h.blockProtection);
            //        }

            //        else if (m3h.difficults[m3h.csvCnt].map[i] == '3')
            //        {
            //            MatchObject m = sC.GetRandomObjectPrefab(LcSet, goSet);
            //            m3h.grid.Cells[i].SetObject1(m);
            //            p.cellsID.Add(3);
            //        }
            //    }
            //}


        }

        //public void setObstacleFromGene(DNA<char> p, SpawnController sC)
        //{
        //    BlockedObject b = sC.GetObstacleObjectPrefab(LcSet, goSet);
        //    BlockedObject b0 = sC.GetSelectBreakableBlockedObjectPrefab(LcSet, goSet);
        //    OverlayObject o = sC.GetSelectOverlayObjectPrefab(LcSet, goSet);

        //    p.kindsOfObstacle = new List<int>();

        //    if (m3h.spawnObstacleObject) p.kindsOfObstacle.Add(0);
        //    if (m3h.spawnBlockedObject) p.kindsOfObstacle.Add(1);
        //    if (m3h.spawnOverlayObject) p.kindsOfObstacle.Add(2);

        //    for (int i = 0; i < m3h.grid.Cells.Count; i++)
        //    {
        //        if (m3h.difficults[m3h.csvCnt].map[i] == '0')
        //        {
        //            m3h.grid.Cells[i].SetObject1(b);
        //            p.cellsID.Add(0);
        //        }

        //        else if (m3h.difficults[m3h.csvCnt].map[i] == '1')
        //        {
        //            m3h.grid.Cells[i].SetObject1(b0);
        //            p.cellsID.Add(1);
        //        }

        //        else if (m3h.difficults[m3h.csvCnt].map[i] == '2')
        //        {
        //            MatchObject m = sC.GetRandomObjectPrefab(LcSet, goSet);
        //            m3h.grid.Cells[i].SetObject1(m);
        //            m3h.grid.Cells[i].SetObject1(o);
        //            p.cellsID.Add(2);
        //        }

        //        else if (m3h.difficults[m3h.csvCnt].map[i] == '3')
        //        {
        //            MatchObject m = sC.GetRandomObjectPrefab(LcSet, goSet);
        //            m3h.grid.Cells[i].SetObject1(m);
        //            p.cellsID.Add(3);
        //        }
        //    }
           



        //    ////b0.hitCnt = m3h.blockedObjectHitCnt;
        //    ////o.hitCnt = m3h.overlayObjectHitCnt;

        //    //for (int i = 0; i < m3h.grid.Cells.Count; i++)
        //    //{
        //    //    if (p.genes[i] == '1')
        //    //    {
        //    //        if(m3h.onlySpawnObstacleObject)
        //    //        {
        //    //            m3h.grid.Cells[i].SetObject1(b);
        //    //            p.cellsID.Add(0);
        //    //        }

        //    //        else if(m3h.onlySpawnBlockedObject)
        //    //        {
        //    //            m3h.grid.Cells[i].SetObject1(b0);
        //    //            p.cellsID.Add(1);
        //    //        }

        //    //        else if (m3h.onlySpawnOverlayObject) 
        //    //        {
        //    //            MatchObject m = sC.GetRandomObjectPrefab(LcSet, goSet);
        //    //            m3h.grid.Cells[i].SetObject1(m);
        //    //            m3h.grid.Cells[i].SetObject1(o);
        //    //            p.cellsID.Add(2);
        //    //        }
        //    //    }

        //    //    else
        //    //    {
        //    //        MatchObject m = sC.GetRandomObjectPrefab(LcSet, goSet);
        //    //        m3h.grid.Cells[i].SetObject1(m);
        //    //        p.cellsID.Add(3);
        //    //    }

        //    //}
        //}

        public bool estimateObstacleTarget(int targetID)
        {
            //target: overlay (Lianna), blocked (Root)
            if (targetID == 100004 || targetID == 101) return true;

            return false;
        }

        public void estimateIsFeasible(DNA<char> p)
        {
            m3h.new_createFillPath();

            p.infeasibleCellCnt = 0;

            for (int i = m3h.grid.Columns.Count(); i < m3h.grid.Cells.Count; i++)
            {
                //if (m3h.grid.Cells[i].Neighbors.Top.Overlay != null &&
                //   m3h.grid.Cells[i].Neighbors.Left.Overlay != null &&
                //   m3h.grid.Cells[i].Neighbors.Right.Overlay != null &&
                //   m3h.grid.Cells[i].Neighbors.Bottom.Overlay != null)
                ////if (m3h.grid.Cells[i].Neighbors.Top.Overlay != null)
                //{
                //    p.infeasibleCellCnt++;
                //}


                if (m3h.grid.Cells[i].Blocked == null && m3h.grid.Cells[i].fillPathToSpawner == null /*&& m3h.grid.Cells[i].Overlay == null*/)
                {
                    if (m3h.grid.Cells[i].Neighbors.Top.Blocked != null /*|| m3h.grid.Cells[i].Neighbors.Top.Overlay != null*/)
                    {
                        p.infeasibleCellCnt++;
                    }
                    
                }
            }

            if (p.infeasibleCellCnt == 0) p.isFeasible = true;
            else p.isFeasible = false;
        }



        public void calculateFitness(DNA<char> p, SpawnController sC, Transform trans)
        {
            if (p.isFeasible)
            {
                p.calculateFeasibleFitness(m3h.wantDifficulty, m3h.difficultyTolerance);
                ga.feasiblePopulation.Add(p);
                ga.feasibleFitnessSum += p.fitness;
            }

            else
            {
                p.calculateInfeasibleFitness();
                ga.infeasiblePopulation.Add(p);
                ga.infeasibleFitnessSum += p.fitness;
            }
        }

        public void calculateNewFitness(DNA<char> p, SpawnController sC, Transform trans)
        {
            if (p.isFeasible)
            {
                int cnt = 0;

                if (p.mapMatchPotential == m3h.difficults[m3h.csvCnt].pottential) cnt += Math.Abs(p.mapMatchPotential - m3h.difficults[m3h.csvCnt].pottential);
                if (p.obstaclePottentialCnt == m3h.difficults[m3h.csvCnt].obstacle) cnt += Math.Abs(p.obstaclePottentialCnt - m3h.difficults[m3h.csvCnt].obstacle);

                if (p.blockedPottentialCnt == m3h.difficults[m3h.csvCnt].blockedPottential) cnt += Math.Abs(p.blockedPottentialCnt - m3h.difficults[m3h.csvCnt].blockedPottential);
                if (p.blocks[0].pottential == m3h.difficults[m3h.csvCnt].blocked1) cnt += Math.Abs(p.blocks[0].pottential - m3h.difficults[m3h.csvCnt].blocked1);
                if (p.blocks[1].pottential == m3h.difficults[m3h.csvCnt].blocked2) cnt += Math.Abs(p.blocks[1].pottential - m3h.difficults[m3h.csvCnt].blocked2);
                if (p.blocks[2].pottential == m3h.difficults[m3h.csvCnt].blocked3) cnt += Math.Abs(p.blocks[2].pottential - m3h.difficults[m3h.csvCnt].blocked3);

                if (p.overlayPottentialCnt == m3h.difficults[m3h.csvCnt].overlayPottential) cnt += Math.Abs(p.overlayPottentialCnt - m3h.difficults[m3h.csvCnt].overlayPottential);
                if (p.blocks[3].pottential == m3h.difficults[m3h.csvCnt].overlay1) cnt += Math.Abs(p.blocks[3].pottential - m3h.difficults[m3h.csvCnt].overlay1);
                if (p.blocks[4].pottential == m3h.difficults[m3h.csvCnt].overlay2) cnt += Math.Abs(p.blocks[4].pottential - m3h.difficults[m3h.csvCnt].overlay2);
                if (p.blocks[5].pottential == m3h.difficults[m3h.csvCnt].overlay3) cnt += Math.Abs(p.blocks[5].pottential - m3h.difficults[m3h.csvCnt].overlay3);

                p.calculateFitennnsssssss(cnt);
                ga.feasiblePopulation.Add(p);
                ga.feasibleFitnessSum += p.fitness;
            }

            else
            {
                p.calculateInfeasibleFitness();
                ga.infeasiblePopulation.Add(p);
                ga.infeasibleFitnessSum += p.fitness;
            }
        }



        public void calculateFitnessThroughPlay(DNA<char> p, SpawnController sC, Transform trans, bool isObstacleTarget, CSVFileWriter cs)
        {
            List<int> bestSwapCntContainer = new List<int>();
            List<int> swapCntContainer = new List<int>();
            List<int> matchCntContainer = new List<int>();

            for (int repeatIdx = 0; repeatIdx < m3h.limits.repeat; repeatIdx++)
            {
                destoryAllCellInGrid();
                setGridRepeat(p, sC);

                foreach (var item in m3h.curTargets) item.Value.InitCurCount();
                m3h.new_createFillPath();
                m3h.grid.RemoveMatches1();

                p.swapCnt = 0;
                p.matchCnt = 0;

                m3h.plays = new PlayHelper();

                while (m3h.plays.isClear == false && m3h.plays.isError == false && m3h.plays.playCnt < m3h.limits.find)
                {
                    switch (m3h.plays.curState)
                    {
                        case 0: m3h.fillFreeCells();
                            break;
                        case 1: m3h.swapCells(p,trans);
                            break;
                        case 2: m3h.matchAndDestory(p);
                            break;
                    }
                    m3h.plays.playCnt++;
                }
                Debug.Log("endPlay");

                if (m3h.plays.isError == true || m3h.plays.isClear == false)
                {
                    swapCntContainer.Add(999999);
                    matchCntContainer.Add(999999);
                    bestSwapCntContainer.Add(999999);
                }

                else
                {
                    swapCntContainer.Add(p.swapCnt);
                    matchCntContainer.Add(p.matchCnt);
                    bestSwapCntContainer.Add(p.swapCnt);
                }

                Debug.Log("count++");
            }
            cs.swapContainer.Add(swapCntContainer);
            cs.matchContainer.Add(matchCntContainer);
            cs.bestSwapContainer.Add(bestSwapCntContainer);

            ga.generation++;
        }

        void getMatch3Level(Transform trans)
        {
            SpawnController sC = SpawnController.Instance;
            sC.numOfMatchBlock = m3h.numOfMatchBlock;
            CSVFileWriter cs = new CSVFileWriter(m3h);
            bool isObstacleTarget = false;
            Debug.Log("beforefitness");

            if(m3h.csvCnt ==10)
            {
                m3h.wantDifficulty = 600;
            }

            if (m3h.getSetGenes)
            {
                destoryAllCellInGrid();

                BlockedObject obstacle = sC.GetObstacleObjectPrefab(LcSet, goSet);
                BlockedObject blocked = sC.GetSelectBreakableBlockedObjectPrefab(LcSet, goSet);
                OverlayObject overlay = sC.GetSelectOverlayObjectPrefab(LcSet, goSet);

                for (int i = 0; i < m3h.grid.Cells.Count; i++)
                {
                    if(m3h.divideSpecificBlock)
                    {
                        if(ga.population[0].genes[i] == '1' && m3h.spawnObstacleObject)
                        {
                            m3h.grid.Cells[i].SetObject1(obstacle);
                            ga.population[0].gridObjects.Add(0);
                            ga.population[0].objectProtection.Add(0);
                        }

                        else if (ga.population[0].genes[i] == '1' && m3h.spawnBlockedObject)
                        {
                            m3h.grid.Cells[i].SetObject1(blocked);
                            ga.population[0].gridObjects.Add(1);
                            ga.population[0].objectProtection.Add(m3h.blockProtection);
                        }

                        else if (ga.population[0].genes[i] == '1' && m3h.spawnOverlayObject)
                        {
                            MatchObject m = sC.GetRandomObjectPrefab(LcSet, goSet);
                            m3h.grid.Cells[i].SetObject1(m);
                            m3h.grid.Cells[i].SetObject1(overlay);
                            ga.population[0].gridObjects.Add(2);
                            ga.population[0].objectProtection.Add(m3h.blockProtection);
                        }

                        else
                        {
                            MatchObject m = sC.GetRandomObjectPrefab(LcSet, goSet);
                            m3h.grid.Cells[i].SetObject1(m);
                            ga.population[0].gridObjects.Add(3);
                            ga.population[0].objectProtection.Add(0);
                        }
                    }

                    else
                    {
                        string protection = m3h.setMaps(m3h.csvCnt, false);

                        if (ga.population[0].genes[i] == '0')
                        {
                            m3h.grid.Cells[i].SetObject1(obstacle);
                            ga.population[0].gridObjects.Add(0);
                            ga.population[0].objectProtection.Add(0);
                        }

                        else if (ga.population[0].genes[i] == '1')
                        {
                            m3h.grid.Cells[i].SetObject1(blocked);
                            ga.population[0].gridObjects.Add(1);

                            if (protection[i] == '1') ga.population[0].objectProtection.Add(1);
                            else if (protection[i] == '2') ga.population[0].objectProtection.Add(2);
                            else if (protection[i] == '3') ga.population[0].objectProtection.Add(3);

                            else
                            {
                                Debug.Log("can'tFindBlockedProtection");
                                ga.population[555].fitness = 33333;
                            }

                        }

                        else if (ga.population[0].genes[i] == '2')
                        {
                            MatchObject m = sC.GetRandomObjectPrefab(LcSet, goSet);
                            m3h.grid.Cells[i].SetObject1(m);
                            m3h.grid.Cells[i].SetObject1(overlay);
                            ga.population[0].gridObjects.Add(2);

                            if (protection[i] == '1') ga.population[0].objectProtection.Add(1);
                            else if (protection[i] == '2') ga.population[0].objectProtection.Add(2);
                            else if (protection[i] == '3') ga.population[0].objectProtection.Add(3);

                            else
                            {
                                Debug.Log("can'tFindOverlayProtection");
                                ga.population[555].fitness = 33333;
                            }
                        }

                        else if (ga.population[0].genes[i] == '3')
                        {
                            MatchObject m = sC.GetRandomObjectPrefab(LcSet, goSet);
                            m3h.grid.Cells[i].SetObject1(m);
                            ga.population[0].gridObjects.Add(3);
                            ga.population[0].objectProtection.Add(0);
                        }

                        else
                        {
                            Debug.Log("can'tFindObject");
                            ga.population[555].fitness = 33333;
                        }
                    }


                }

                estimateIsFeasible(ga.population[0]);

                if (ga.population[0].isFeasible)
                {
                    setGridRepeat(ga.population[0], sC);
                    m3h.cntPerPottentials(ga.population[0]);
                }
                


                //if (ga.population[0].isFeasible) m3h.cntMapmatchPottential(ga.population[0]);
                calculateFitness(ga.population[0], sC, trans);

                /////////////////////////////////////////
                //setGridEachKind(ga.population[0], sC, false, false, false, false, 0);
                //ga.population[0].swapablePottential.notObstacle = m3h.cntPottentials(ga.population[0]);


                //setGridRepeat(ga.population[0], sC);
                //m3h.cntPerPottentials(ga.population[0]);
                //m3h.cntMapPottentials(ga.population[0]);

                calculateFitnessThroughPlay(ga.population[0], sC, trans, isObstacleTarget, cs);
                
            }

            else
            {
                for (int i = 0; i < m3h.limits.geneticGeneration; i++)
                {
                    for (int j = 0; j < ga.population.Count; j++)
                    {
                        if (ga.population[j].fitness == 0)
                        {
                            destoryAllCellInGrid();
                            setGrid(ga.population[j], sC);
                            estimateIsFeasible(ga.population[j]);

                            if (ga.population[j].isFeasible)
                            {
                                setGridRepeat(ga.population[j], sC);
                                m3h.cntPerPottentials(ga.population[j]);
                                //m3h.cntMapmatchPottential(ga.population[j]);
                            }

                            calculateFitness(ga.population[j], sC, trans);

                            if (ga.bestFitness < ga.population[j].fitness && ga.population[j].isFeasible)
                            {
                                ga.bestFitness = ga.population[j].fitness;
                                ga.bestPottential = (int)ga.population[j].allPottential.map;
                            }

                            if (ga.bestFitness >= ga.targetFitness)
                            {
                                ga.population[0] = ga.population[j];
                                break;
                            }
                        }

                        else
                        {
                            if (ga.population[j].isFeasible)
                            {
                                ga.feasiblePopulation.Add(ga.population[j]);
                                ga.feasibleFitnessSum += ga.population[j].fitness;
                            }

                            else
                            {
                                ga.infeasiblePopulation.Add(ga.population[j]);
                                ga.infeasibleFitnessSum += ga.population[j].fitness;
                            }
                        }
                    }

                    if (ga.bestFitness >= ga.targetFitness) break;
                    else ga.newGeneration();

                }

                if (ga.bestFitness >= ga.targetFitness)
                {
                    //setGridEachKind(ga.population[0], sC, isSpawnObstacle,isSpawnBlocked,isSpawnOverlay,isEachProtection,protection)

                    //setGridEachKind(ga.population[0], sC, false, false, false, false, 0);
                    //ga.population[0].pottential.notObstacle = m3h.cntPottentials(ga.population[0]);

                    //if (m3h.spawnObstacleObject)
                    //{
                    //    setGridEachKind(ga.population[0], sC, true, false, false, false, 0);
                    //    ga.population[0].pottential.onlyObstacle = m3h.cntPottentials(ga.population[0]);
                    //}

                    //if (m3h.spawnBlockedObject)
                    //{
                    //    setGridEachKind(ga.population[0], sC, false, true, false, false, 0);
                    //    ga.population[0].pottential.onlyAllBlocked = m3h.cntPottentials(ga.population[0]);

                    //    setGridEachKind(ga.population[0], sC, false, true, false, true, 1);
                    //    ga.population[0].pottential.onlyBlocked1 = m3h.cntPottentials(ga.population[0]);

                    //    setGridEachKind(ga.population[0], sC, false, true, false, true, 2);
                    //    ga.population[0].pottential.onlyBlocked2 = m3h.cntPottentials(ga.population[0]);

                    //    setGridEachKind(ga.population[0], sC, false, true, false, true, 3);
                    //    ga.population[0].pottential.onlyBlocked3 = m3h.cntPottentials(ga.population[0]);
                    //}

                    //if (m3h.spawnOverlayObject)
                    //{
                    //    setGridEachKind(ga.population[0], sC, false, false, true, false, 0);
                    //    ga.population[0].pottential.onlyAllOverlay = m3h.cntPottentials(ga.population[0]);

                    //    setGridEachKind(ga.population[0], sC, false, false, true, true, 1);
                    //    ga.population[0].pottential.onlyOverlay1 = m3h.cntPottentials(ga.population[0]);

                    //    setGridEachKind(ga.population[0], sC, false, false, true, true, 2);
                    //    ga.population[0].pottential.onlyOverlay2 = m3h.cntPottentials(ga.population[0]);

                    //    setGridEachKind(ga.population[0], sC, false, false, true, true, 3);
                    //    ga.population[0].pottential.onlyOverlay3 = m3h.cntPottentials(ga.population[0]);
                    //}

                    //if (m3h.spawnObstacleObject && m3h.spawnBlockedObject)
                    //{
                    //    setGridEachKind(ga.population[0], sC, true, true, false, false, 0);
                    //    ga.population[0].pottential.obstacleAndAllBlocked = m3h.cntPottentials(ga.population[0]);

                    //    setGridEachKind(ga.population[0], sC, true, true, false, true, 1);
                    //    ga.population[0].pottential.obstacleAndBlocked1 = m3h.cntPottentials(ga.population[0]);

                    //    setGridEachKind(ga.population[0], sC, true, true, false, true, 2);
                    //    ga.population[0].pottential.obstacleAndBlocked2 = m3h.cntPottentials(ga.population[0]);

                    //    setGridEachKind(ga.population[0], sC, true, true, false, true, 3);
                    //    ga.population[0].pottential.obstacleAndBlocked3 = m3h.cntPottentials(ga.population[0]);
                    //}

                    //if (m3h.spawnObstacleObject && m3h.spawnOverlayObject)
                    //{
                    //    setGridEachKind(ga.population[0], sC, true, false, true, false, 0);
                    //    ga.population[0].pottential.obstacleAndAllOverlay = m3h.cntPottentials(ga.population[0]);

                    //    setGridEachKind(ga.population[0], sC, true, false, true, true, 1);
                    //    ga.population[0].pottential.obstacleAndOverlay1 = m3h.cntPottentials(ga.population[0]);

                    //    setGridEachKind(ga.population[0], sC, true, false, true, true, 2);
                    //    ga.population[0].pottential.obstacleAndOverlay2 = m3h.cntPottentials(ga.population[0]);

                    //    setGridEachKind(ga.population[0], sC, true, false, true, true, 3);
                    //    ga.population[0].pottential.obstacleAndOverlay3 = m3h.cntPottentials(ga.population[0]);
                    //}


                    //setGridRepeat(ga.population[0], sC);
                    //m3h.cntPerPottentials(ga.population[0]);
                    //m3h.cntMapPottentials(ga.population[0]);
                    

                    for (int i = 0; i < m3h.limits.generation; i++)
                    {
                        calculateFitnessThroughPlay(ga.population[0], sC, trans, isObstacleTarget, cs);
                    }
                }
            }

            

            //else
            //{
            //    for (int i = 0; i < m3h.limits.generation; i++) //유전알고리즘을 통해 예측한 swap횟수에 따른 맵 생성
            //    {
            //        for (int j = 0; j < ga.population.Count; j++)
            //        {
            //            destoryAllCellInGrid(); // destory all cells info
            //            setObstacleFromGene(ga.population[j], sC);
            //            estimateIsFeasible(ga.population[j]);
            //            calculateFitness(ga.population[j], sC, trans);

            //            if (ga.bestFitness < ga.population[j].fitness) ga.bestFitness = ga.population[j].fitness;

            //            if (ga.bestFitness >= ga.targetFitness) break;
            //        }

            //        if (ga.bestFitness >= ga.targetFitness) break;
            //        else ga.newGeneration();
            //    }
            //}


            //if (m3h.getSetGenes) ga.population[555].fitness = 33333;

            Debug.Log("ENDcalculate");

            if (m3h.csvCnt == m3h.limits.csvCnt)
            {
                destoryAllCellInGrid();
                setGridRepeat(ga.population[0], sC);
            }

            if (ga.population[0].isFeasible)
            {
                cs.write(ga, m3h);
                Debug.Log("writedCSV");
                m3h.wantDifficulty -= 50;
            }

            else Debug.Log("isNotFeasible");

        }

        //void getMatch3Level(Transform trans)
        //{
        //    SpawnController sC = SpawnController.Instance;
        //    sC.numOfMatchBlock = m3h.numOfMatchBlock;
        //    CSVFileWriter cs = new CSVFileWriter(m3h);

        //    Debug.Log("beforefitness");

        //    bool isObstacleTarget = false;

        //    //foreach (var item in m3h.curTargets)
        //    //{
        //    //    isObstacleTarget = estimateObstacleTarget(item.Value.ID);
        //    //    if (isObstacleTarget) break;
        //    //}

        //    if(m3h.isActualMeasureSwap) 
        //    {
        //        for (int i = 0; i < m3h.limits.geneticGeneration; i++)
        //        {
        //            for (int j = 0; j < ga.population.Count; j++)
        //            {
        //                destoryAllCellInGrid();
        //                setObstacleFromGene(ga.population[j], sC);
        //                estimateIsFeasible(ga.population[j]);

        //                if (ga.population[j].isFeasible) m3h.countObstacle(ga.population[j]);

        //                calculateFitness(ga.population[j], sC, trans);

        //                if (ga.bestFitness < ga.population[j].fitness && ga.population[j].isFeasible)
        //                {
        //                    ga.bestFitness = ga.population[j].fitness;
        //                }

        //                if (ga.bestFitness >= ga.targetFitness)
        //                {
        //                    ga.population[0] = ga.population[j];
        //                    break;
        //                }

        //            }

        //            if (ga.bestFitness >= ga.targetFitness) break;
        //            else ga.newGeneration();
        //        }

        //        if (ga.bestFitness >= ga.targetFitness)
        //        {
        //            m3h.setProtection(ga.population[0]);

        //            //흐음
        //            m3h.cntMatchPottentials(ga.population[0]);

        //            destoryAllCellInGrid();
        //            for (int i = 0; i < m3h.grid.Cells.Count; i++)
        //            {
        //                MatchObject m = sC.GetRandomObjectPrefab(LcSet, goSet);
        //                m3h.grid.Cells[i].SetObject1(m);
        //            }
        //            //필요없는게 있긴한데 뭐 크게 지장없는듯
        //            ga.population[0].notObstacleCnt = m3h.cntPerPottential(ga.population[0]);

        //            //destoryAllCellInGrid();
        //            //setGridRepeat(ga.population[0], sC);

        //            for(int i = 0;i< ga.population[0].kindsOfObstacle.Count;i++)
        //            {
        //                destoryAllCellInGrid();
        //                setGridOnlyOne(ga.population[0], sC, ga.population[0].kindsOfObstacle[i]);

        //                switch (ga.population[0].kindsOfObstacle[i])
        //                {
        //                    case 0:
        //                        ga.population[0].obstaclePottentialCnt = m3h.cntPerPottential(ga.population[0]);
        //                        break;
        //                    case 1:
        //                        ga.population[0].blockedPottentialCnt = m3h.cntPerPottential(ga.population[0]);
        //                        break;
        //                    case 2:
        //                        ga.population[0].overlayPottentialCnt = m3h.cntPerPottential(ga.population[0]);
        //                        break;
        //                }
        //            }

        //            setGridPerProtection(ga.population[0], sC);

        //            for (int i = 0; i < m3h.limits.generation; i++)
        //            {
        //                calculateFitnessThroughPlay(ga.population[0], sC, trans, isObstacleTarget, cs);
        //            }

        //            //if (m3h.onlySpawnObstacleObject || m3h.onlySpawnBlockedObject || m3h.onlySpawnOverlayObject)
        //            //{
        //            //    countOneKindObstacle(ga.population[0], sC);
        //            //    m3h.cntMatchPottentials(ga.population[0]);
        //            //} 
        //            //else countPerObstacle(ga.population[0], sC);
        //        }


        //    }

        //    else if(m3h.getSetGenes)
        //    {

        //        for (int i = 0; i < ga.population.Count; i++)
        //        {
        //            destoryAllCellInGrid();
        //            setObstacleFromGene(ga.population[i], sC);
        //        }

        //        for (int i = 0; i < m3h.limits.geneticGeneration; i++)
        //        {
        //            for (int j = 0; j < ga.population.Count; j++)
        //            {
        //                destoryAllCellInGrid();

        //                estimateIsFeasible(ga.population[j]);

        //                if (ga.population[j].isFeasible)
        //                {
        //                    m3h.countObstacle(ga.population[j]);
        //                    m3h.setProtection(ga.population[j]);
        //                    m3h.cntMatchPottentials(ga.population[j]);

        //                    destoryAllCellInGrid();

        //                    for (int k = 0; k < m3h.grid.Cells.Count; k++)
        //                    {
        //                        MatchObject m = sC.GetRandomObjectPrefab(LcSet, goSet);
        //                        m3h.grid.Cells[k].SetObject1(m);
        //                    }

        //                    ga.population[j].notObstacleCnt = m3h.cntPerPottential(ga.population[0]);

        //                    for (int k = 0; k < ga.population[j].kindsOfObstacle.Count; k++)
        //                    {
        //                        destoryAllCellInGrid();
        //                        setGridOnlyOne(ga.population[j], sC, ga.population[j].kindsOfObstacle[k]);

        //                        switch (ga.population[j].kindsOfObstacle[k])
        //                        {
        //                            case 0:
        //                                ga.population[j].obstaclePottentialCnt = m3h.cntPerPottential(ga.population[k]);
        //                                break;
        //                            case 1:
        //                                ga.population[j].blockedPottentialCnt = m3h.cntPerPottential(ga.population[k]);
        //                                break;
        //                            case 2:
        //                                ga.population[j].overlayPottentialCnt = m3h.cntPerPottential(ga.population[k]);
        //                                break;
        //                        }
        //                    }

        //                    setGridPerProtection(ga.population[j], sC);
        //                }

        //                calculateNewFitness(ga.population[j], sC, trans);

        //                if (ga.bestFitness < ga.population[j].fitness && ga.population[j].isFeasible)
        //                {
        //                    ga.bestFitness = ga.population[j].fitness;
        //                }

        //                if (ga.bestFitness >= ga.targetFitness)
        //                {
        //                    ga.population[0] = ga.population[j];
        //                    break;
        //                }

        //            }

        //            if (ga.bestFitness >= ga.targetFitness) break;
        //            else ga.newNEWEWEWEWEGeneration();
        //        }

        //        calculateFitnessThroughPlay(ga.population[0], sC, trans, isObstacleTarget, cs);
        //    }

        //    else
        //    {
        //        for (int i = 0; i < m3h.limits.generation; i++) //유전알고리즘을 통해 예측한 swap횟수에 따른 맵 생성
        //        {
        //            for (int j = 0; j < ga.population.Count; j++)
        //            {
        //                destoryAllCellInGrid(); // destory all cells info
        //                setObstacleFromGene(ga.population[j], sC);
        //                estimateIsFeasible(ga.population[j]);
        //                calculateFitness(ga.population[j], sC, trans);

        //                if (ga.bestFitness < ga.population[j].fitness) ga.bestFitness = ga.population[j].fitness;

        //                if (ga.bestFitness >= ga.targetFitness) break;
        //            }

        //            if (ga.bestFitness >= ga.targetFitness) break;
        //            else ga.newGeneration();
        //        }
        //    }


        //    //if (m3h.getSetGenes) ga.population[555].fitness = 33333;

        //    Debug.Log("ENDcalculate");

        //    if(m3h.csvCnt >= 9)
        //    {
        //        destoryAllCellInGrid();
        //        setGridRepeat(ga.population[0], sC);
        //    }

        //    if (ga.population[0].isFeasible)
        //    {
        //        cs.write(ga, m3h);
        //        Debug.Log("writedCSV");
        //        m3h.wantDifficulty -= 50;
        //    }

        //    else Debug.Log("isNotFeasible");

        //}

        public List<MatchGroup> mgList;
        public Match3Helper m3h;
        System.Random randomGa;
        GeneticAlgorithm<char> ga;
        internal void fillGrid(bool noMatches, MatchGrid g, Dictionary<int, TargetData> targets, Spawner spawnerPrefab, SpawnerStyle spawnerStyle, Transform GridContainer, Transform trans, LevelConstructSet IC)
        {
            m3h = new Match3Helper(g, targets);
            m3h.board.makeBoard(g, spawnerPrefab, spawnerStyle, GridContainer, trans, IC);

            for (; m3h.match3Cycle < m3h.limits.match3Cycle; )
            {
                Debug.Log("startNewCycle");
                randomGa = new System.Random();
                ga = new GeneticAlgorithm<char>(Cells.Count, randomGa, getRandomGene, getGenes, m3h); //유전알고리즘 호출
                getMatch3Level(trans);
                m3h.match3Cycle += 1;

                if (m3h.csvCnt > m3h.limits.csvCnt) break;
            }

        }


        #region fill grid
        /// <summary>
        /// Fill grid with random regular objects, preserve existing dynamic objects (match, click bomb, falling)
        /// </summary>
        /// <param name="noMatches"></param>
        /// <param name="goSet"></param>
        /// 
        internal void FillGrid(bool noMatches)
        {

            SpawnController sC = SpawnController.Instance;
            Debug.Log("fill grid, remove matches: " + noMatches);
            sC.numOfMatchBlock = 7;

            BlockedObject b = sC.GetObstacleObjectPrefab(LcSet, goSet);
            BlockedObject b0 = sC.GetSelectBreakableBlockedObjectPrefab(LcSet, goSet);
            OverlayObject o = sC.GetSelectOverlayObjectPrefab(LcSet, goSet);

            b0.hitCnt = 1;
            o.hitCnt = 1;


            for (int i = 0; i < Cells.Count; i++)
            {
                if (i == 5 || i == 10 || i == 15 || i == 20 || i == 25 || i == 30)
                {
                    Cells[i].SetObject1(b0);
                }

                else
                {
                    MatchObject m = sC.GetMainRandomObjectPrefab(LcSet, goSet);
                    Cells[i].SetObject(m);
                }


            }


            if (noMatches) RemoveMatches();



            //SpawnController sC = SpawnController.Instance;

            //Debug.Log("fill grid, remove matches: " + noMatches);
            //for (int i = 0; i < Cells.Count; i++)
            //{
            //    //if (!Cells[i].Blocked && !Cells[i].IsDisabled && !Cells[i].DynamicObject)
            //    {
            //        MatchObject m = sC.GetMainRandomObjectPrefab(LcSet, goSet);
            //        Cells[i].SetObject(m);
            //    }
            //}
            //if (noMatches) RemoveMatches();
        }
        internal void RemoveMatches()
        {
            SpawnController sC = SpawnController.Instance;
            int minMatch = 3;
            GridCell[] gc_row = new GridCell[minMatch];
            GridCell[] gc_col = new GridCell[minMatch];
            System.Func<GridCell[], bool> isEqual = (gcl) =>
            {
                if (gcl == null || gcl.Length == 0) return false;
                foreach (var item in gcl)
                    if (!item || !item.Match) return false;

                int id = gcl[0].Match.ID;

                foreach (var item in gcl)
                    if (item.Match.ID != id) return false;
                return true;
            };
            List<GridObject> mod_list;
            for (int i = 0; i < vertSize; i++)
            {
                for (int j = 0; j < horSize; j++)
                {
                    if (Rows[i][j].Blocked || Rows[i][j].IsDisabled) continue;
                    for (int m = 0; m < minMatch; m++)
                    {
                        gc_row[m] = this[i, j - m];
                        gc_col[m] = this[i - m, j];
                    }
                    mod_list = new List<GridObject>();
                    bool rowHasMatches = false;
                    bool colHasMatches = false;

                    if (isEqual(gc_row)) rowHasMatches = true;
                    if (isEqual(gc_col)) colHasMatches = true;

                    if (rowHasMatches || colHasMatches)
                    {
                        if (gc_col[1] && gc_col[1].Match) mod_list.Add(sC.GetMainObjectPrefab(goSet, gc_col[1].Match.ID));
                        if (gc_row[1] && gc_row[1].Match) mod_list.Add(sC.GetMainObjectPrefab(goSet, gc_row[1].Match.ID));
                    }
                    if (mod_list.Count > 0) Rows[i][j].GetComponent<GridCell>().SetObject((sC.GetMainRandomObjectPrefab(LcSet, goSet, mod_list)));
                }
            }
#if UNITY_EDITOR
            // double test
            for (int i = 0; i < vertSize; i++)
            {
                for (int j = 0; j < horSize; j++)
                {
                    if (Rows[i][j].Blocked || Rows[i][j].IsDisabled) continue;
                    for (int m = 0; m < minMatch; m++)
                    {
                        gc_row[m] = this[i, j - m];
                        gc_col[m] = this[i - m, j];
                    }
                    mod_list = new List<GridObject>();
                    bool rowHasMatches = false;
                    bool colHasMatches = false;

                    if (isEqual(gc_row)) rowHasMatches = true;
                    if (isEqual(gc_col)) colHasMatches = true;

                    if (rowHasMatches || colHasMatches)
                    {
                        if (gc_col[1] && gc_col[1].Match) mod_list.Add(sC.GetMainObjectPrefab(goSet, gc_col[1].Match.ID));
                        if (gc_row[1] && gc_row[1].Match) mod_list.Add(sC.GetMainObjectPrefab(goSet, gc_row[1].Match.ID));
                        Debug.Log("----------------------------Found matches--------------------------------------");
                    }
                    if (mod_list.Count > 0) Rows[i][j].GetComponent<GridCell>().SetObject((sC.GetMainRandomObjectPrefab(LcSet, goSet, mod_list)));
                }
            }
#endif
        }
        #endregion fill grid


        internal void RemoveMatches1()
        {
            SpawnController sC = SpawnController.Instance;
            int minMatch = 3;
            GridCell[] gc_row = new GridCell[minMatch];
            GridCell[] gc_col = new GridCell[minMatch];
            System.Func<GridCell[], bool> isEqual = (gcl) =>
            {
                if (gcl == null || gcl.Length == 0) return false;
                foreach (var item in gcl)
                    if (!item || !item.Match) return false;

                int id = gcl[0].Match.ID;

                foreach (var item in gcl)
                    if (item.Match.ID != id) return false;
                return true;
            };
            List<GridObject> mod_list;
            for (int i = 0; i < vertSize; i++)
            {
                for (int j = 0; j < horSize; j++)
                {
                    if (Rows[i][j].Blocked || Rows[i][j].IsDisabled) continue;
                    for (int m = 0; m < minMatch; m++)
                    {
                        gc_row[m] = this[i, j - m];
                        gc_col[m] = this[i - m, j];
                    }
                    mod_list = new List<GridObject>();
                    bool rowHasMatches = false;
                    bool colHasMatches = false;

                    if (isEqual(gc_row)) rowHasMatches = true;
                    if (isEqual(gc_col)) colHasMatches = true;

                    if (rowHasMatches || colHasMatches)
                    {
                        if (gc_col[1] && gc_col[1].Match) mod_list.Add(sC.GetMainObjectPrefab(goSet, gc_col[1].Match.ID));
                        if (gc_row[1] && gc_row[1].Match) mod_list.Add(sC.GetMainObjectPrefab(goSet, gc_row[1].Match.ID));
                    }
                    if (mod_list.Count > 0)
                    {
                        Rows[i][j].GetComponent<GridCell>().SetObject((sC.GetMainRandomObjectPrefab(LcSet, goSet, mod_list)));
                    }
                    
                }
            }
#if UNITY_EDITOR
            // double test
            for (int i = 0; i < vertSize; i++)
            {
                for (int j = 0; j < horSize; j++)
                {
                    if (Rows[i][j].Blocked || Rows[i][j].IsDisabled) continue;
                    for (int m = 0; m < minMatch; m++)
                    {
                        gc_row[m] = this[i, j - m];
                        gc_col[m] = this[i - m, j];
                    }
                    mod_list = new List<GridObject>();
                    bool rowHasMatches = false;
                    bool colHasMatches = false;

                    if (isEqual(gc_row)) rowHasMatches = true;
                    if (isEqual(gc_col)) colHasMatches = true;

                    if (rowHasMatches || colHasMatches)
                    {
                        if (gc_col[1] && gc_col[1].Match) mod_list.Add(sC.GetMainObjectPrefab(goSet, gc_col[1].Match.ID));
                        if (gc_row[1] && gc_row[1].Match) mod_list.Add(sC.GetMainObjectPrefab(goSet, gc_row[1].Match.ID));
                        Debug.Log("----------------------------Found matches--------------------------------------");
                    }
                    if (mod_list.Count > 0)
                    {
                        Rows[i][j].GetComponent<GridCell>().SetObject((sC.GetMainRandomObjectPrefab(LcSet, goSet, mod_list)));
                    }
                        

                }
            }
#endif
        }
    }
}




