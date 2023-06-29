using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static Mkey.FitnessHelper;
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


        /// //////////
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
        internal List<GridCell> GetFreeCells(MatchGrid g)
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

        /// <summary>
        /// Return not blocked, not disabled cells without dynamic object, with fillPath or with and without
        /// </summary>
        /// <returns></returns>
        internal List<GridCell> GetFreeCells(bool withPath)
        {
            List<GridCell> gcL = new List<GridCell>();
            for (int i = 0; i < Cells.Count; i++)
            {

                //Dictionary <string,int> a = new Dictionary<string,int>();
                //a = Cells[2].DynamicObject;

                //int a = Cells[2].DynamicObject.base

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



        //유전알고리즘
        //////////////////////////////////////////////////////////////////


        //System.Random random_ga;
        //GeneticAlgorithm<char> ga;
        //string targetString = "000001111100000111110000011111000001111100000111110000011111";
        //string validCharacters = "000001111100000111110000011111000001111100000111110000011111";
        //int populationSize = 5;
        //float mutationRate = 5;
        //int elitism = 5;
        //int targetInt = 5;

        //int numCharsPerText = 15000;
        //string targetText;
        //string bestText;
        //string bestFitnessText;
        //string numGenerationsText;
        //Transform populationTextParent;
        //string textPrefab;

        //string gridsizeString;
        //int howManyCorrect = 5;


        //private char GetRandomCharacter()
        //{
        //    int i = random_ga.Next(validCharacters.Length);
        //    return validCharacters[i];
        //}

        //private float FitnessFunction(int index)
        //{
        //    float score = 0;
        //    DNA<char> dna = ga.Population[index];

        //    //for (int i = 0; i < dna.Genes.Length; i++)
        //    //{
        //    //    if (dna.Genes[i] == targetString[i])
        //    //    {
        //    //        score += 1;
        //    //    }
        //    //}

        //    //score /= targetString.Length;

        //    //score = (Mathf.Pow(2, score) - 1) / (2 - 1);


        //    for (int i = 0; i < dna.Genes.Length; i++)
        //    {
        //        if (dna.Genes[i] == '1')
        //        {
        //            score += 1;
        //        }
        //    }

        //    //score = score / howManyCorrect;


        //    return score;
        //}



        //호출 넘버 부여



        public void FillStateGrid(MatchGrid g, SpawnController sC)
        {
            //List<GridCell> gFreeCells = g.GetFreeCells(); // Debug.Log("fill free count: " + gFreeCells.Count + " to collapse" );

            if (cellContainer.freeCellContainer.Count <= 0)
            {
                cur_state = 2;
            }

            else
            {
                ////bool filled = false;
                ////cellContainer.CreateFillPath(g);
                ////while (cellContainer.freeCellContainer.Count > 0)
                ////{
                ////cellContainer.FillGridByStep(cellContainer.freeCellContainer, () => { });
                ////cellContainer.freeCellContainer = g.GetFreeCells();
                ////}

                //for (int i = 0; i < cellContainer.freeCellContainer.Count;i++) 
                //{
                //    //cellContainer.freeCellContainer.Add(g.Cells[cellContainer.l_mgList[i].Cells[j].Row * Rows.Count() + cellContainer.l_mgList[i].Cells[j].Column]);
                //    MatchObject m = sC.GetRandomObjectPrefab(LcSet, goSet);
                //    Cells[cellContainer.freeCellContainer[i].Column + cellContainer.freeCellContainer[i].Row * Rows.Count()].SetObject(m);

                //    //Cells[j].SetObject(m);

                //}

                cellContainer.CreateFillPath(g);
                cellContainer.FillGridByStep(cellContainer.freeCellContainer, () => { });
                cellContainer.freeCellContainer.RemoveAll(x => x.IsMatchable);

                cur_state = 2;
            }



            //if (noMatches)
            //{
            //    RemoveMatches();
            //}

        }

        int mix_limit = 10;
        int mix_cnt = 0;

        public void ShowStateGrid(MatchGrid g, Dictionary<int, TargetData> targets, SpawnController sC)
        {
            cellContainer.l_mgList = new List<MatchGroup>();

            while (mix_cnt <= mix_limit)
            {
                cellContainer.CancelTweens();
                cellContainer.CreateMatchGroups(2, true, g);

                if (cellContainer.l_mgList.Count == 0)
                {
                    cellContainer.MixGrid(g);
                    mix_cnt++;
                }

                else break;
            }

            if (mix_cnt >= mix_limit)
            {
                mix_cnt = 0;
                notClear = true;
                return;
            }

            else
            {
                List<List<int>> targetsInCell = new List<List<int>>();

                foreach (var currentCellsID in g.Cells)
                {
                    List<int> res = currentCellsID.GetGridObjectsIDs();

                    foreach (var item in targets)
                    {
                        if (!item.Value.Achieved)
                        {
                            if (item.Value.ID >= 1000 && item.Value.ID <= 1006)
                            {
                                if (item.Value.ID == res[0])
                                {
                                    targetsInCell.Add(new List<int> { currentCellsID.Row, currentCellsID.Column });
                                }
                            }
                        }
                    }
                }

                //cellContainer.TargetSwap(targetsInCell);

                List<int> collectMatch = new List<int>();

                for (int i = 0; i < cellContainer.l_mgList.Count; i++)
                {
                    bool isInsert = false;

                    for (int j = 0; j < targetsInCell.Count; j++)
                    {
                        if (cellContainer.l_mgList[i].Cells[0].Row == targetsInCell[j][0] && cellContainer.l_mgList[i].Cells[0].Column == targetsInCell[j][1])
                        {
                            isInsert = true;
                            break;
                        }

                        if (cellContainer.l_mgList[i].Cells[1].Row == targetsInCell[j][0] && cellContainer.l_mgList[i].Cells[1].Column == targetsInCell[j][1])
                        {
                            isInsert = true;
                            break;
                        }
                    }

                    if (isInsert) collectMatch.Add(i);
                }

                int target_num = 0;

                if (collectMatch.Count <= 0)
                {
                    cellContainer.l_mgList[0].SwapEstimate();
                    target_num = 0;
                } 

                else
                {
                    int number = Random.Range(0, collectMatch.Count - 1);
                    cellContainer.l_mgList[collectMatch[number]].SwapEstimate();
                    target_num = collectMatch[number];
                }

                List<int> find = new List<int>();
                find = cellContainer.l_mgList[target_num].GetEst();

                GridCell tmp_Cell = new GridCell();

                tmp_Cell = Cells[cellContainer.l_mgList[target_num].Cells[0].Column + cellContainer.l_mgList[target_num].Cells[0].Row * Rows.Count];

                tmp_Cell = Cells[find[0] + find[1] * Rows.Count];
                Cells[find[0] + find[1] * Rows.Count] = Cells[find[2] + find[3] * Rows.Count];
                Cells[find[2] + find[3] * Rows.Count] = tmp_Cell;

                //Cells[cellContainer.freeCellContainer[i].Column + cellContainer.freeCellContainer[i].Row * Rows.Count()].SetObject(m);

                //for (int i=0;i< cellContainer.l_mgList.Count; i++)
                //{
                //    //Cells[cellContainer.l_mgList[target_num].Cells[0].Column + cellContainer.l_mgList[target_num].Cells[0].Row * Rows.Count] = 

                //    //MatchObject m = sC.GetRandomObjectPrefab(LcSet, goSet);
                //    //Cells[i].SetObject(m);
                //}


                move--;
                cur_state = 2;
            }

        }

        int collect_limit = 100;
        int collect_cnt = 0;

        public void CollectStateGrid(MatchGrid g)
        {

            if (collect_cnt >= collect_limit)
            {
                collect_cnt = 0;
                notClear = true;
                return;
            }

            if (cellContainer.freeCellContainer.Count > 0)
            {
                cur_state = 0;
                return;
            }

            //if (g.GetFreeCells(true).Count > 0)
            //{
            //    cur_state = 0;
            //    return;
            //}


            cellContainer.l_mgList = new List<MatchGroup>();
            //cellContainer.CollectFalling(g);
            cellContainer.CancelTweens();
            cellContainer.CreateMatchGroups(3, false, g);
            g.mgList = cellContainer.l_mgList;

            cellContainer.CollectFalling(g);

            //if(GetFreeCells(g).Count > 0)
            //{
            //    collect_cnt = 0;
            //    cur_state = 0;
            //}

            //매치블록 지우기 얘내를 프리셀로 만들어야됨
            cellContainer.freeCellContainer = new List<GridCell>();

            for (int i = 0; i < cellContainer.l_mgList.Count; i++)
            {

                for(int j = 0; j < cellContainer.l_mgList[i].Length; j++)
                {
                    cellContainer.freeCellContainer.Add(g.Cells[cellContainer.l_mgList[i].Cells[j].Row * Rows.Count() + cellContainer.l_mgList[i].Cells[j].Column]);
                }
                
                //4개 일땐?
            }

            if (cellContainer.freeCellContainer.Count > 0)
            {
                collect_cnt = 0;
                cur_state = 0;
            }

            //int collected = 0;
            //List<GridCell> gcL = new List<GridCell>();
            //for (int i = 0; i < Cells.Count; i++)
            //{

            //    //Dictionary <string,int> a = new Dictionary<string,int>();
            //    //a = Cells[2].DynamicObject;

            //    //int a = Cells[2].DynamicObject.base

            //    if (Cells[i].IsDynamicFree && !Cells[i].Blocked && !Cells[i].IsDisabled)
            //    {
            //        if (withPath && Cells[i].HaveFillPath() || !withPath)
            //            gcL.Add(Cells[i]);
            //    }
            //}
            //return gcL;


            else cur_state = 1;
        }

        public char GetRandomGene()
        {
            int number = Random.Range(0, 2);
            return (char)(number + '0');
        }

        public float FitnessFunction(int idx)
        {
            float fitness = 0.0f;

            return fitness;
        }

        public GameBoard gameBoard;

        //////////////////////////////////////////////////////////////////

        System.Random random_ga;
        GeneticAlgorithm<char> ga;
        public MatchGrid grid;
        public List<MatchGroup> mgList;
        int move;
        int cur_state;
        bool targetClear = false;
        bool notClear = false;

        public FitnessHelper cellContainer { get; private set; }


        #region fill grid
        /// <summary>
        /// Fill grid with random regular objects, preserve existing dynamic objects (match, click bomb, falling)
        /// </summary>
        /// <param name="noMatches"></param>
        /// <param name="goSet"></param>
        internal void FillGrid(bool noMatches, MatchGrid g, Dictionary<int, TargetData> targets, Spawner spawnerPrefab, SpawnerStyle spawnerStyle, Transform GridContainer, Transform trans)
        {
            SpawnController sC = SpawnController.Instance;
            ////일단 Grid 채우기
            //Debug.Log("fill grid, remove matches: " + noMatches);
            //for (int i = 0; i < Cells.Count; i++)
            //{
            //    MatchObject m = sC.GetRandomObjectPrefab(LcSet, goSet);
            //    Cells[i].SetObject(m);
            //}


            int populationSize = 3;
            int elitism = 1;
            float mutationRate = 0.01f;

            //유전알고리즘 호출
            random_ga = new System.Random();
            ga = new GeneticAlgorithm<char>(populationSize, Cells.Count, random_ga, GetRandomGene, FitnessFunction, elitism, mutationRate);
            
            int repetition_limit = 10;
            int generation_limit = 10;
            int move_limit = 20;
            int repetition_cnt = 0;
            int generation_cnt = 0;

            int collect_Fit_Size = 0;

            // 세대 반복 횟수
            //for (int idx = 0; idx < 10; idx++)
            {
                List<MatchObject> MatchObjectCollect = new List<MatchObject>(Cells.Count);
                for (int j = 0; j < Cells.Count; j++)
                {
                    MatchObject m = sC.GetRandomObjectPrefab(LcSet, goSet);

                    if (j == 13 || j == 12 || j == 11)
                    {
                        BlockedObject m1 = sC.GetRandomBlockedObjectPrefab(LcSet, goSet);
                        Cells[j].SetObject(m1);
                        //MatchObjectCollect.Add(m1);
                    }

                    else
                    {
                        Cells[j].SetObject(m);
                        //MatchObjectCollect.Add(m);
                    }

                }

                gameBoard = new GameBoard();

                gameBoard.test(g, spawnerPrefab, spawnerStyle, GridContainer, trans);

                //if (g.haveFillPath)
                //{
                //    foreach (var item in lC.spawnCells)
                //    {
                //        if (g[item.Row, item.Column]) g[item.Row, item.Column].CreateSpawner(spawnerPrefab, Vector2.zero);
                //    }

                //}



                // 그리드의 개수
                for (int i = 0; i < populationSize; i++)
                {
                    List<int> collect_Fitness = new List<int>();

                    // 평균 계산 반복 횟수
                    for (int j = 0; j < repetition_limit; j++)
                    {
                        //for (int k = 0; k < Cells.Count; k++)
                        //{
                        //    Cells[k].SetObject(MatchObjectCollect[k]);

                        //    if (ga.Population[i].Genes[k] == '1')
                        //    {
                        //        BlockedObject m = sC.GetRandomBlockedObjectPrefab(LcSet, goSet);
                        //        Cells[k].SetObject(m);
                        //    }
                        //}

                        cellContainer = new FitnessHelper();
                        move = move_limit;
                        move += 5;
                        cur_state = 0;
                        targetClear = false;
                        notClear = false;

                        while (move > 0 && targetClear == false && notClear == false)
                        {
                            switch (cur_state)
                            {
                                case 0:
                                    FillStateGrid(g, sC);
                                    break;
                                case 1:
                                    ShowStateGrid(g, targets, sC);
                                    break;
                                case 2:
                                    CollectStateGrid(g);
                                    break;
                            }
                        }

                        if (targetClear == true)
                        {
                            //move 가공 필요
                            collect_Fitness.Add(move);
                            collect_Fit_Size = collect_Fitness.Count;
                        }
                    }
                }

                //ga.NewGeneration();
            }


            //float4 searchBoundary(float2 uv)
            //{
            //    const int constant = 3;

            //    float2 mean = g_motionVectorMipMap.SampleLevel(g_samLinear, uv, MIPMAP_LEVEL).xy;
            //    float2 squaMean = g_motionVectorMipMap.SampleLevel(g_samLinear, uv, MIPMAP_LEVEL).zw;

            //    float2 standardDeviation = sqrt(squaMean - mean * mean);
            //    float2 Max = mean + standardDeviation * constant;
            //    float2 Min = mean - standardDeviation * constant;

            //    float2 leftTopCorner = max(uv - Max, float2(0.0f, 0.0f));
            //    float2 rightBottomCorner = min(uv - Min, float2(1.0f, 1.0f));

            //    return float4(leftTopCorner, rightBottomCorner);
            //}

            //float constant = 3;
            //float mean;
            //float squareMean;
            //float standardDevitaion = (float)Math.Sqrt(squareMean - mean * mean);


            //평균 이동횟수가 이동한계횟수에 가깝고 표준편차가 원하는 이동한계 표준편차에 가까워야한다.

            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            ////while (1 - Math.Abs(ga.BestFitness) > 0.1f && generation_cnt < generation_limit)
            //{
            //    List<MatchObject> MatchObjectCollect = new List<MatchObject>(Cells.Count);
            //    for (int j = 0; j < Cells.Count; j++)
            //    {
            //        MatchObject m = sC.GetRandomObjectPrefab(LcSet, goSet);
            //        Cells[j].SetObject(m);
            //        MatchObjectCollect.Add(m);
            //    }

            //    for (int i = 0; i < populationSize; i++)
            //    {
            //        List<int> collect_Fitness = new List<int>();

            //        for (int j = 0; j < repetition_limit; j++)
            //        {
            //            //for (int k = 0; k < Cells.Count; k++)
            //            //{
            //            //    Cells[k].SetObject(MatchObjectCollect[k]);

            //            //    //if (ga.Population[i].Genes[k] == '1')
            //            //    //{
            //            //    //    BlockedObject m = sC.GetRandomBlockedObjectPrefab(LcSet, goSet);
            //            //    //    Cells[k].SetObject(m);
            //            //    //}
            //            //}

            //            cellContainer = new FitnessHelper();
            //            move = move_limit;
            //            move += 5;
            //            cur_state = 0;
            //            targetClear = false;

            //            while (move > 0 || targetClear == false)
            //            {
            //                switch (cur_state)
            //                {
            //                    case 0:
            //                        FillStateGrid(g);
            //                        break;
            //                    case 1:
            //                        ShowStateGrid(g, targets);
            //                        break;
            //                    case 2:
            //                        CollectStateGrid(g);
            //                        break;
            //                }
            //            }

            //            if (targetClear == true)
            //            {
            //                //move 가공 필요
            //                collect_Fitness.Add(move);
            //            }
            //        }

            //        ////fitness 입력
            //        //ga.Population[i].Fitness = 1;
            //    }

            //    //ga.NewGeneration();
            //    //generation_cnt++;
            //    //break;
            //}









            //while (1 - Math.Abs(ga.BestFitness) > 0.1f)
            //{
            //    for (int i = 0; i < populationSize; i++)
            //    {
            //        move = 25;
            //        cur_state = 0;
            //        ////gene으로  cell 채우기
            //        //List<int> collectID = new List<int>();
            //        for (int j = 0; j < Cells.Count; j++)
            //        {
            //            MatchObject m = sC.GetRandomObjectPrefab(LcSet, goSet);
            //            Cells[j].SetObject(m);
            //            //collectID.Add(m.ID);
            //        }

            //        for (int j = 0; j < populationSize; j++)
            //        {
            //            if (ga.Population[i].Genes[j] == '1')
            //            {
            //                BlockedObject m = sC.GetRandomBlockedObjectPrefab(LcSet, goSet);
            //                Cells[j].SetObject(m);
            //            }

            //        }

            //        //cellContainer = new FitnessHelper(Cells, collectID);
            //        cellContainer = new FitnessHelper();

            //        while (move > 0)
            //        {
            //            if (cur_state == 0) FillStateGrid(g);
            //            else if (cur_state == 1) ShowStateGrid(g, targets);
            //            else if (cur_state == 2) CollectStateGrid(g);
            //        }
            //    }

            //    //if (cnt > 100)
            //    //{
            //    //    Debug.Log("Can't find BestFitness : " + ga.BestFitness);
            //    //    break;
            //    //}

            //    ga.NewGeneration();

            //    //UpdateText(ga.BestGenes, ga.BestFitness, ga.Generation, ga.Population.Count, (j) => ga.Population[j].Genes);
            //    //cnt++;
            //}

            ////if (cnt < 100) Debug.Log("find BestFitness : " + ga.BestFitness);




            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////







            ////FillState-------------------------------------------------------------------------------------//

            //List<GridCell> gFreeCells = g.GetFreeCells(true); // Debug.Log("fill free count: " + gFreeCells.Count + " to collapse" );
            //bool filled = false;
            //if (gFreeCells.Count > 0)
            //{
            //    cellContainer.CreateFillPath(g);
            //}

            //while (gFreeCells.Count > 0)
            //{
            //    cellContainer.FillGridByStep(gFreeCells, () => { });
            //    gFreeCells = g.GetFreeCells(true);
            //    filled = true;
            //}

            ////ShowEstimateState-------------------------------------------------------------------------------------//

            //////getset을 써야하나
            ////Dictionary<int, TargetData> copied_targets = new Dictionary<int, TargetData>();
            ////board.Copy_target(copied_targets);

            //cellContainer.CancelTweens();
            //cellContainer.CreateMatchGroups(2, true, g);

            ////MixGrid

            //List<List<int>> targetsInCell = new List<List<int>>();

            //foreach (var currentCellsID in g.Cells)
            //{
            //    List<int> res = currentCellsID.GetGridObjectsIDs();

            //    foreach (var item in targets)
            //    {
            //        if (!item.Value.Achieved)
            //        {
            //            if (item.Value.ID >= 1000 && item.Value.ID <= 1006)
            //            {
            //                if (item.Value.ID == res[0])
            //                {
            //                    targetsInCell.Add(new List<int> { currentCellsID.Row, currentCellsID.Column });
            //                }
            //            }
            //        }
            //    }
            //}

            ////cellContainer.TargetSwap(targetsInCell);


            ////CollectState-------------------------------------------------------------------------------------//
            //int collected = 0;

            //if (g.GetFreeCells(true).Count > 0)
            //{
            //    return;
            //}

            //cellContainer.CollectFalling(g);
            //cellContainer.CancelTweens();
            //cellContainer.CreateMatchGroups(3, false, g);
            //if(cellContainer.l_mgList.Count==0)
            //{

            //}


            //if (CollectGroups.Length == 0) // no matches
            //{
            //    if (g.GetFreeCells(true).Count > 0)
            //        MbState = MatchBoardState.Fill;
            //    else
            //    {
            //        MbState = MatchBoardState.ShowEstimate;
            //    }
            //}

            //else
            //{
            //    BeforeCollectBoardEvent?.Invoke(this);
            //    MatchScore = scoreController.GetScoreForMatches(CollectGroups.Length);
            //    CollectGroups.CollectMatchGroups(() => { GreatingMessage(); MbState = MatchBoardState.Fill; MatchScore = scoreController.GetScoreForMatches(0); });
            //}







            ////first grid fill 시 3match 줄여줌
            //if (noMatches)
            //{
            //    RemoveMatches();
            //}












            //CreateGroups11(2, true);

            ////if (EstimateGroups.Length > 0)
            //{
            //    //EstimateGroups.SwapEstimate();

            //    List<List<int>> targetsInCell = new List<List<int>>();

            //    foreach (var currentCellsID in CurrentGrid.Cells)
            //    {
            //        List<int> res = currentCellsID.GetGridObjectsIDs();

            //        foreach (var item in CurTargets)
            //        {
            //            if (!item.Value.Achieved)
            //            {
            //                if (item.Value.ID >= 1000 && item.Value.ID <= 1006)
            //                {
            //                    if (item.Value.ID == res[0])
            //                    {
            //                        targetsInCell.Add(new List<int> { currentCellsID.Row, currentCellsID.Column });
            //                    }
            //                }
            //            }
            //        }
            //    }


            //    EstimateGroups.FOA_TargetSwap(targetsInCell);



            //}




            //CollectState();
            //FillState();










            //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////










            //for (int i = 0; i < Cells.Count; i++)
            //{
            //    MatchObject m = sC.GetRandomObjectPrefab(LcSet, goSet);
            //    Cells[i].SetObject(m);
            //}


            //for (int i = 0; i < gridsizeString.Length; i++)
            //{
            //    if (ga.BestGenes[i] == '1')
            //    {
            //        BlockedObject m = sC.GetRandomBlockedObjectPrefab(LcSet, goSet);
            //        Cells[i].SetObject(m);
            //    }

            //}








            //for (int i = 0; i < 3; i++)
            //{
            //    OverlayObject m = sC.GetRandomOverlayObjectPrefab(LcSet, goSet);

            //    //m.BlockMovement = true;

            //    Cells[i].SetObject(m);
            //}

            //// 됨 target count 체크 필요
            //for (int i = 0; i < 3; i++)
            //{
            //    UnderlayObject m = sC.GetRandomUnderlayObjectPrefab(LcSet, goSet);

            //    Cells[i].SetObject(m);
            //}

            //// 됨 target count 체크 필요
            //for (int i = 0; i < 3; i++)
            //{
            //    FallingObject m = sC.GetRandomUgetFallingObjectPrefab(LcSet, goSet);

            //    Cells[i].SetObject(m);
            //}

            //// 됨 target count 체크 필요
            //for (int i = 0; i < 3; i++)
            //{
            //    HiddenObject m = sC.GetRandomHiddenObjectPrefab(LcSet, goSet);

            //    Cells[i].SetObject(m);
            //}





            ////getPickedObjectBlock
            //for (int i = 0; i < Cells.Count; i++)
            //{
            //    if (!Cells[i].Blocked && !Cells[i].IsDisabled && !Cells[i].DynamicObject)
            //    {
            //        MatchObject m = sC.GetPickedObjectBlock(LcSet, goSet);
            //        Cells[i].SetObject(m);
            //    }

            //}




            ////fill control test
            //MatchObject m = sC.GetMainRandomObjectPrefab(LcSet, goSet);
            //for (int i = 0; i < Cells.Count; i++)
            //{
            //    Cells[i].SetObject(m);
            //}

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




