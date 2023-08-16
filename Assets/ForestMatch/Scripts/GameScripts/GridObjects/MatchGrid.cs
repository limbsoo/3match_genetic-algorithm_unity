using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using UnityEngine.UIElements;
using static Mkey.Match_Helper;
using static UnityEditor.PlayerSettings;
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


        private void AssignObjectData(LevelConstructSet lcSet, GameMode gMode)
        {
            //int cellsCnt = 5;

            Rows[0][1].SetObject(100, 0);
            Rows[0][2].SetObject(500000, 0);
            Rows[2][1].SetObject(1, 0);

            //Rows[3][0].SetObject(100, 0);
            //Rows[3][1].SetObject(100, 0);
            //Rows[3][2].SetObject(100, 0);
            //Rows[3][3].SetObject(100, 0);
        }



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

        bool estimateMaxedOut(DNA<char> p, ref int cnt, int max)
        {
            if(max <= cnt) p.maxedOut = true;
            return p.maxedOut;
        }

        public void fillState(DNA<char> p, SpawnController sC)
        {
            List<GridCell> gFreeCells = GetFreeCells(mh.grid, true);
            if (gFreeCells.Count > 0) mh.createFillPath(mh.grid);

            int fill_cnt = 0;

            while (gFreeCells.Count > 0)
            {
                mh.fillGridByStep(gFreeCells, () => { });
                gFreeCells = GetFreeCells(mh.grid, true);

                if (estimateMaxedOut(p,ref fill_cnt, 100)) break;
            }


            foreach (var item in mh.curTargets)
            {
                if (item.Value.Achieved) p.targetClear = true;
                else
                {
                    p.targetClear = false;
                    break;
                }
            }
            if (p.targetClear) return;

            p.curState = 2;

            //NoPhys1(g);
            //while (!NoPhys1(g));
            //if (noMatches) RemoveMatches();
        }

        public void showState(DNA<char> p, SpawnController sC, Transform trans)
        {

            foreach (var item in mh.curTargets)
            {
                if (item.Value.Achieved) p.targetClear = true;
                else
                {
                    p.targetClear = false;
                    break;
                }
            }
            if (p.targetClear) return;

            int mix_cnt = 0;

            while (!estimateMaxedOut(p, ref mix_cnt, 100))
            {
                //mh.cancelTweens(mh.grid);
                mh.createMatchGroups1(2, true, mh.grid, p);

                if (mh.grid.mgList.Count == 0)
                {
                    mh.mixGrid(null, mh.grid, trans);
                    mix_cnt++;
                }
                else break;
            }

            if (p.maxedOut) return;

            else
            {
                List<MatchGroup> matchedTarget = new List<MatchGroup>();

                foreach (var mc in mh.grid.mgList)
                {
                    foreach (var item in mh.curTargets)
                    {
                        List<int> mg_cell = mc.Cells[0].GetGridObjectsIDs();

                        if (mg_cell[0] == item.Key)
                        {
                            matchedTarget.Add(mc);
                            break;
                        }
                    }
                }

                if (matchedTarget.Count == 0) mh.grid.mgList[0].SwapEstimate();

                else
                {
                    //int target_num = 0;
                    //mh.grid.mgList[target_num].SwapEstimate();
                    //mgList에서 랜덤하게 해서 변종확률?

                    int number = Random.Range(0, matchedTarget.Count - 1);

                    for(int i = 0; i < mh.grid.mgList.Count; i++) 
                    {
                        if (matchedTarget[number].Cells[0].DynamicObject == mh.grid.mgList[i].Cells[0].DynamicObject)
                        {
                            mh.grid.mgList[i].SwapEstimate();
                            break;
                        }
                    }

                    //foreach (var mc in mh.grid.mgList)
                    //{
                    //    if (target_match[number] == mc) mc.SwapEstimate();
                    //    break;
                    //}
                }

                p.numMove--;
                p.curState = 2;




                //List<List<int>> targetsInCell = new List<List<int>>();

                //foreach (var currentCellsID in mh.grid.Cells)
                //{
                //    List<int> res = currentCellsID.GetGridObjectsIDs();

                //    //Debug.Log("currentCellsID ======Row" + currentCellsID.Row + "---------------------------Column" + currentCellsID.Column);

                //    foreach (var item in mh.curTargets)
                //    {
                //        if (!item.Value.Achieved)
                //        {
                //            if (item.Value.ID >= 1000 && item.Value.ID <= 1006)
                //            {
                //                if (item.Value.ID == res[0]) targetsInCell.Add(new List<int> { currentCellsID.Row, currentCellsID.Column });
                //            }
                //        }
                //    }
                //}

                //List<int> collectMatch = new List<int>();
                ////셀판별추가
                //for (int i = 0; i < mh.grid.mgList.Count; i++)
                //{
                //    bool isTargetBlock = false;

                //    for (int j = 0; j < targetsInCell.Count; j++)
                //    {
                //        if (mh.grid.mgList[i].Cells[0].Row == targetsInCell[j][0] && mh.grid.mgList[i].Cells[0].Column == targetsInCell[j][1])
                //        {
                //            isTargetBlock = true;
                //            break;
                //        }

                //        if (mh.grid.mgList[i].Cells[1].Row == targetsInCell[j][0] && mh.grid.mgList[i].Cells[1].Column == targetsInCell[j][1])
                //        {
                //            isTargetBlock = true;
                //            break;
                //        }
                //    }

                //    if (isTargetBlock) collectMatch.Add(i);
                //}

                //int target_num = 0;

                //if (collectMatch.Count <= 0)
                //{
                //    mh.grid.mgList[0].SwapEstimate();
                //    target_num = 0;
                //}

                //else
                //{
                //    int number = Random.Range(0, collectMatch.Count - 1);
                //    mh.grid.mgList[collectMatch[number]].SwapEstimate();
                //    target_num = collectMatch[number];
                //}

                p.numMove--;
                p.curState = 2;
            }
        }

        int collect_cnt = 0;
        public void collectState(DNA<char> p)
        {
            if (collect_cnt >= 100)
            {
                p.maxedOut = true;
                return;
            }

            if (mh.grid.GetFreeCells(true).Count > 0)
            {
                p.curState = 0;
                return;
            }

            //mh.collectFalling(mh.grid);
            //mh.cancelTweens(mh.grid);
            mh.createMatchGroups(3, false, mh.grid);

            if (mh.grid.mgList.Count == 0)
            {
                collect_cnt = 0;
                p.curState = 1;
            }

            else
            {
                for (int i = 0; i < mh.grid.mgList.Count; i++)
                {
                    List<int> res = mh.grid.mgList[i].Cells[0].GetGridObjectsIDs();
                    foreach (var item in mh.curTargets) if (item.Value.ID == res[0]) item.Value.IncCurrCount(mh.grid.mgList[i].Cells.Count);
                }

                ////////

                for (int i = 0; i < mh.grid.mgList.Count; i++)
                {
                    if (mh.grid.mgList[i] != null)
                    {
                        foreach (GridCell c in mh.grid.mgList[i].Cells)
                        {
                            c.DestroyGridObjects();
                        }
                    }
                 }


                //MatchGroupsHelper helper = new MatchGroupsHelper(mh.grid);
                //helper.CollectMatchGroups1(mh.grid);

                p.curState = 0;
            }

            collect_cnt++;
        }

        public char GetRandomGene()
        {
            int number = Random.Range(0, 2);
            return (char)(number + '0');
        }

        public void initalizeGrid1(SpawnController sC)
        {
            for (int i = 0; i < mh.grid.Cells.Count; i++)
            {
                mh.grid.Cells[i].DestroyGridObjects();

                MatchObject m = sC.GetRandomObjectPrefab(LcSet, goSet);
                mh.grid.Cells[i].SetObject1(m);

            }
        }


        public void initalizeGrid(DNA<char> p, SpawnController sC)
        {
            for (int i = 0; i < mh.grid.Cells.Count; i++)
            {
                mh.grid.Cells[i].DestroyGridObjects();

                MatchObject m = sC.GetRandomObjectPrefab(LcSet, goSet);
                mh.grid.Cells[i].SetObject1(m);

                //if (i == 11 || i == 14 || i == 16 || i == 19 || i == 21 || i == 24 || i == 26 || i == 29)
                ////if ( i == 9 || i == 13 || i == 14)
                //{
                //    BlockedObject b = sC.GetRandomBlockedObjectPrefab(LcSet, goSet);
                //    mh.grid.Cells[i].SetObject1(b);
                //}

                //if (

                //    i == 11 || i == 14 || 
                //    i == 16 || i == 19 ||
                //    i == 21 || i == 24 ||
                //    i == 26 || i== 29)
                ////if ( i == 9 || i == 13 || i == 14)
                //{
                //    BlockedObject b = sC.GetRandomBlockedObjectPrefab(LcSet, goSet);
                //    mh.grid.Cells[i].SetObject1(b);
                //}

                //if (p.genes[i] == '1')
                //{
                //    BlockedObject b = sC.GetRandomBlockedObjectPrefab(LcSet, goSet);
                //    mh.grid.Cells[i].SetObject1(b);
                //}
            }
        }

        public void initalizeMatchBlock(DNA<char> p, SpawnController sC)
        {
            for (int i = 0; i < mh.grid.Cells.Count; i++)
            {
                if (p.genes[i] != '1')
                {
                    mh.grid.Cells[i].DestroyGridObjects();
                    MatchObject m = sC.GetRandomObjectPrefab(LcSet, goSet);
                    mh.grid.Cells[i].SetObject1(m);
                }
            }
        }

        public bool EstimateFeasible(MatchGrid grid, DNA<char> p)
        {
            int cnt = 0;
            bool isFeasible = mh.haveInfeasibleCell(grid, cnt);
            return false;
        }

        public void estimateFeasible(DNA<char> p)
        {

            p.isFeasible = true;


            //mh.createFillPath(mh.grid);
            //p.infeasibleCellCnt = 0;

            //for (int i = mh.grid.Columns.Count(); i < mh.grid.Cells.Count; i++)
            //{
            //    if (mh.grid.Cells[i].Blocked == null && mh.grid.Cells[i].fillPathToSpawner == null)
            //    {
            //        if (mh.grid.Cells[i].Neighbors.Top.Blocked != null) p.infeasibleCellCnt++;
            //    }
            //}

            //if (p.infeasibleCellCnt == 0) p.isFeasible = true;
            //else p.isFeasible = false;
        }

        public void calculateFitness(DNA<char> p, SpawnController sC, Transform trans)
        {
            if (p.isFeasible)
            {
                makeFeasibleFitness(p, sC, trans);
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

        public bool estimateFloationgPoint(double a, double b)
        {
            //double epsilon = 1e-9;

            if (areEqual(a, b) || a < b) return true;
       
            return false;

        }

        public bool areEqual(double a, double b, double tolerance = 1e-9)
        {
            return Math.Abs(a - b) <= tolerance;
        }


        public void makeFeasibleFitness(DNA<char> p, SpawnController sC, Transform trans)
        {
            List<int> moveContainer = new List<int>();
            List<int> obstructionRateContainer = new List<int>();



            for (int repeatIdx = 0; repeatIdx < ga.repeat; repeatIdx++)
            {
                initalizeGrid(p, sC);
                //initalizeMatchBlock(p, sC);

                //mh.createAvailableMatchGroup(mh.grid);

                //MatchGroup matchGroup = new MatchGroup();
                //for (int i = 0; i < mh.grid.Cells.Count; i++) mh.grid.Cells[i].possibleCnt = 0;
                //for (int i = 0; i < mh.grid.Cells.Count; i++)
                //{
                //    matchGroup.countPossible(mh.grid, i);
                //}

                //int a = 0;

                //for (int i = 0; i < mh.grid.Cells.Count; i++)
                //{
                //    a += mh.grid.Cells[i].possibleCnt;
                //}



                //int tryCnt = 0;
                p.numMove = ga.moveLimit;
                p.targetClear = false;
                p.maxedOut = false;
                p.curState = 0;
                p.obstructionRate = 0;

                foreach (var item in mh.curTargets) item.Value.InitCurCount();

                while (p.numMove > 0 && p.targetClear == false && p.maxedOut == false)
                {
                    switch (p.curState)
                    {
                        case 0:
                            fillState(p, sC);
                            break;
                        case 1:
                            showState(p, sC, trans);
                            break;
                        case 2:
                            collectState(p);
                            break;
                    }

                    //if (tryCnt > 5000) break;
                    //tryCnt++;
                }

                obstructionRateContainer.Add(p.obstructionRate);

                if (p.maxedOut == true || p.targetClear == false)
                {
                    moveContainer.Add(ga.moveLimit);
                    //continue;
                } 

                else moveContainer.Add(p.numMove);

                //mh.CancelTweens(mh.grid);
                //else num_move_container.Add(0);
            }

            p.calculateFeasibleFitness(moveContainer, (double)ga.targetMove, ga.targetStd);

            //double alpha = 0.5;

            ga.repeatMovements.Add(moveContainer);
            ga.repeatMovementsCnt++;

            ga.obstructionRates.Add(obstructionRateContainer);

            if (ga.curGenerationBestFitness < p.fitness)
            {
                ga.curGenerationBestFitness = p.fitness;
                ga.curGenerationBestMean = p.mean;
                ga.curGenerationBestStd = p.stanardDeviation;
                ga.curBestMoves = moveContainer;
            }

            if (ga.bestFitness < p.fitness)
            {
                ga.bestFitness = p.fitness;
                ga.bestMeanMove = p.mean;
                ga.bestStd = p.stanardDeviation;
                ga.bestMoves = moveContainer;
            }



            //if(estimateFloationgPoint(ga.curGenerationBestFitness, p.fitness))
            //{
            //    ga.curGenerationBestFitness = p.fitness;
            //    ga.curGenerationBestMean = p.mean;
            //    ga.curGenerationBestStd = p.stanardDeviation;
            //}

            //if (estimateFloationgPoint(ga.bestFitness, p.fitness))
            //{
            //    ga.bestFitness = p.fitness;
            //    ga.bestMeanMove = p.mean;
            //    ga.bestStd = p.stanardDeviation;
            //}





            //if(alpha * (1.0 / (1.0 + Math.Abs(mean - target_move))) + (1 - alpha) * (1.0 / (1.0 + Math.Abs(standardDeviation - target_std));
            //if (Math.Abs(ga.targetMove - ga.curBestMeanMove) > Math.Abs(ga.targetMove - p.mean))
            //{
            //    ga.curBestMeanMove = p.mean;
            //}



        }

        void getMatch3Level(Transform trans)
        {
            SpawnController sC = SpawnController.Instance;

            CSVFileWriter cs = new CSVFileWriter();

            ga.bestMeanMove = 0;
            ga.bestStd = 0;
            ga.bestFitness = 0;

            ga.repeatMovements = new List<List<int>>();
            ga.obstructionRates = new List<List<int>>();

            //initalizeGrid1(sC);

            //MatchGroup matchGroup = new MatchGroup();
            ////for (int i = 0; i < mh.grid.Cells.Count; i++) mh.grid.Cells[i].possibleCnt = 0;
            //for (int i = 0; i < mh.grid.Cells.Count; i++)
            //{
            //    matchGroup.countPossible(mh.grid, i);
            //}

            //int a = 0;

            //for (int i = 0; i < mh.grid.Cells.Count; i++)
            //{
            //    a += mh.grid.Cells[i].possibleCnt;
            //}

            //List<int> pc = new List<int>();

            //for (int i = 0; i < mh.grid.Cells.Count; i++)
            //{
            //    pc.Add(mh.grid.Cells[i].possibleCnt);
            //}

            while (ga.bestFitness < ga.targetFitness && ga.generation <= ga.generationLimit)
            {
                ga.curGenerationBestMean = 0;
                ga.curGenerationBestStd = 0;
                ga.curGenerationBestFitness = 0;
                ga.repeatMovementsCnt = 0;

                foreach (DNA<char> p in ga.population)
                {
                    if (p.fitness != 0)
                    {
                        if (p.isFeasible) ga.feasiblePopulation.Add(p);
                        else ga.infeasiblePopulation.Add(p);
                    }

                    else
                    {
                        initalizeGrid(p, sC);
                        estimateFeasible(p);
                        calculateFitness(p, sC, trans);
                    }
                }

                //double bestAverageMove = 0;
                //double beststandardDeviation = 0;

                ////for (int i = 0; i < ga.feasible_population.Count; i++)
                ////{
                ////    if(bestAverageMove < ga.feasible_population[i].average_move) bestAverageMove = ga.feasible_population[i].average_move;
                ////    if (beststandardDeviation < ga.feasible_population[i].sd) beststandardDeviation = ga.feasible_population[i].sd;
                ////}


                cs.generation.Add(ga.generation);

                if(ga.infeasiblePopulation == null) cs.infeasiblePopulationCnt.Add(0);
                else cs.infeasiblePopulationCnt.Add(ga.infeasiblePopulation.Count());

                if (ga.feasiblePopulation == null) cs.feasiblePopulationCnt.Add(0);
                else cs.feasiblePopulationCnt.Add(ga.feasiblePopulation.Count());

                //cs.infeasiblePopulationCnt.Add(ga.infeasiblePopulation.Count());
                //cs.feasiblePopulationCnt.Add(ga.feasiblePopulation.Count());

                cs.curGenerationBestMean.Add(ga.curGenerationBestMean);
                cs.curGenerationBestStd.Add(ga.curGenerationBestStd);
                cs.curGenerationBestFitness.Add(ga.curGenerationBestFitness);

                cs.bestMeanMove.Add(ga.bestMeanMove);
                cs.bestStd.Add(ga.bestStd);
                cs.bestFitness.Add(ga.bestFitness);

                cs.repeatMovementsCntContainer.Add(ga.repeatMovementsCnt);

                if (ga.curBestMoves == null) cs.curBestMoves.Add(new List<int>() { -1 });
                else cs.curBestMoves.Add(ga.curBestMoves);

                if (ga.bestMoves == null) cs.bestMoves.Add(new List<int>() { -1 });
                else cs.bestMoves.Add(ga.bestMoves);







                if (ga.bestFitness >= ga.targetFitness) break;
                ga.NewGeneration();


                //if (ga.feasibleParent == null)
                //{
                //    cs.feasibleParent.Add(new List<double>() { -1, -1 });
                //    cs.feasibleParentIdx.Add(new List<int>() { -1, -1 });
                //}

                //else
                //{
                //    cs.feasibleParent.Add(new List<double>(ga.feasibleParent));
                //    cs.feasibleParentIdx.Add(new List<int>(ga.feasibleParentIdx));
                //}


                //if (ga.infeasibleParent == null)
                //{
                //    cs.infeasibleParent.Add(new List<double>() { -1, -1 });
                //    cs.infeasibleParentIdx.Add(new List<int>() { -1, -1 });
                //}

                //else
                //{
                //    cs.infeasibleParent.Add(new List<double>(ga.infeasibleParent));
                //    cs.infeasibleParentIdx.Add(new List<int>(ga.infeasibleParentIdx));
                //}

            }

            cs.mixedList = new List<object>
            {
                cs.generation,
                cs.infeasiblePopulationCnt,
                cs.feasiblePopulationCnt,
                cs.curGenerationBestMean,
                cs.curGenerationBestStd,
                cs.curGenerationBestFitness,
                cs.bestMeanMove,
                cs.bestStd,
                cs.bestFitness,
                cs.feasibleParent,
                cs.feasibleParentIdx,
                cs.infeasibleParent,
                cs.infeasibleParentIdx,
                cs.curBestMoves,
                cs.bestMoves
            };

            cs.write(ga, Cells);
            //cs.write1(ga);

            initalizeGrid(ga.population[0], sC);
        }

        public List<MatchGroup> mgList;
        public Match_Helper mh;
        System.Random randomGa;
        GeneticAlgorithm<char> ga;

        internal void fillGrid(bool noMatches, MatchGrid g, Dictionary<int, TargetData> targets, Spawner spawnerPrefab, SpawnerStyle spawnerStyle, Transform GridContainer, Transform trans, LevelConstructSet IC)
        {
            randomGa = new System.Random();
            ga = new GeneticAlgorithm<char>(Cells.Count, randomGa, GetRandomGene); //유전알고리즘 호출

            mh = new Match_Helper();
            mh.board = new GameBoard();
            mh.board.makeBoard(g, spawnerPrefab, spawnerStyle, GridContainer, trans, IC);
            g.mgList = new List<MatchGroup>();
            mh.grid = g;
            mh.curTargets = targets;


            //List<double> doubles = new List<double>();

            //for (double i = 0; i < 10; i += 0.01)
            //{
            //    string result = string.Format("{0:0.########0}", Math.Exp(-Math.Abs(i)));

            //    doubles.Add((Double.Parse(result)));
            //}

            //List<double> doubles = new List<double>();
            //for (double i = 0; i < 100; i += 0.01)
            //{
            //    doubles.Add(1.0 / (1.0 + Math.Abs(i)));
            //}

            getMatch3Level(trans);
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
            for (int i = 0; i < Cells.Count; i++)
            {
                if (!Cells[i].Blocked && !Cells[i].IsDisabled && !Cells[i].DynamicObject)
                {
                    MatchObject m = sC.GetMainRandomObjectPrefab(LcSet, goSet);
                    Cells[i].SetObject(m);
                }
            }
            if (noMatches) RemoveMatches();
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
    }
}




