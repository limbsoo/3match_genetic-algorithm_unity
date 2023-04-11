using UnityEngine;
using System;
using System.Collections.Generic;

/*
    02.12.2019 - first
    30.12.2019
    17.03.2020
    15.07.2020
 */
namespace Mkey
{
   // [CreateAssetMenu]
    public class LevelConstructSet : BaseScriptable
    {
        [SerializeField]
        private PopUpsController levelStartStoryPage;
        [SerializeField]
        private PopUpsController levelWinStoryPage;

        [Space(16)]
        [SerializeField]
        private int vertSize = 8;
        [SerializeField]
        private int horSize = 8;
        [SerializeField]
        private float distX = 0.0f;
        [SerializeField]
        private float distY = 0.0f;
        [SerializeField]
        private float scale = 0.9f;
        [SerializeField]
        private int backGroundNumber = 0;

        [Space(16)]
        [HideInInspector]
        [SerializeField]
        public List<CellData> spawnCells;
        [HideInInspector]
        [SerializeField]
        public List<Vector2> spawnOffsets;
        [SerializeField]
        public List<GCellObects> cells;
        public MissionConstruct levelMission;
        [HideInInspector]
        [SerializeField]
        public string boardJS;
        [Header ("Set min 5 matchObjects")]
        [SerializeField]
        public List<MatchObject> usedMatchObjects;

        #region properties
        public PopUpsController LevelWinStoryPage { get { return levelWinStoryPage; } }

        public PopUpsController LevelStartStoryPage { get { return levelStartStoryPage; } }

        public int VertSize
        {
            get { return vertSize; }
            set
            {
                if (value < 1) value = 1;
                vertSize = value;
                SetAsDirty();
            }
        }

        public int HorSize
        {
            get { return horSize; }
            set
            {
                if (value < 1) value = 1;
                horSize = value;
                SetAsDirty();
            }
        }

        public float DistX
        {
            get { return distX; }
            set
            {
                distX = RoundToFloat(value, 0.05f);
                SetAsDirty();
            }
        }

        public float DistY
        {
            get { return distY; }
            set
            {
                distY = RoundToFloat(value, 0.05f);
                SetAsDirty();
            }
        }

        public float Scale
        {
            get { return scale; }
            set
            {
                if (value < 0) value = 0;
                scale = RoundToFloat(value, 0.05f);
                SetAsDirty();
            }
        }
        #endregion properties

        public int BackGround
        {
            get { return backGroundNumber; }
        }

        #region regular
        void OnEnable()
        {
            if (levelMission == null) levelMission = new MissionConstruct();
            levelMission.SaveEvent = SetAsDirty;
        }
        #endregion regular

        #region add object
        public void AddSpawnCell(CellData cd)
        {
            if (spawnCells == null) spawnCells = new List<CellData>();
            if (ContainCellData(spawnCells ,cd))
            {
                RemoveCellData(spawnCells, cd);
            }
            else
            {
                spawnCells.Add(cd);
            }
          
            SetAsDirty();
        }
        #endregion add object

        public void SaveSpawnOfsets(MatchGrid mGrid)
        {
            spawnOffsets = new List<Vector2>();
            if (spawnCells != null)
            {
                foreach (var item in spawnCells)
                {
                    GridCell gC = mGrid.Rows[item.Row].cells[item.Column];
                    if (gC && gC.spawner)
                    {
                        spawnOffsets.Add(gC.transform.InverseTransformPoint(gC.spawner.transform.position));
                    }
                }
            }
            SetAsDirty();
        }

        /// <summary>
        /// Remove all non-existent cells data from board
        /// </summary>
        /// <param name="gOS"></param>
        public void Clean(GameObjectsSet gOS)
        {
            if (spawnCells != null)
            {
                spawnCells.RemoveAll((c) =>
                {
                    return ((c.Column >= horSize) || (c.Row >= vertSize));
                });
            }
            if (cells == null) cells = new List<GCellObects>();
            cells.RemoveAll((c)=> { return ((c.column >= horSize) || (c.row >= vertSize)); });
            if (gOS)
            {
                foreach (var item in cells)
                {
                    if (item.gridObjects != null)
                    {
                        item.gridObjects.RemoveAll((o)=> { return !gOS.ContainID(o.id); });
                    }
                }
            }

            // clean fill path
            PBoard pBoard = JsonUtility.FromJson<PBoard>(boardJS);
            if (pBoard != null && pBoard.cells != null && pBoard.rows > 0 && pBoard.columns > 0)
            {
                int rT = pBoard.rows;
                int cT = pBoard.columns;
                if (rT != vertSize || cT != horSize)
                {
                    PBoard nPBoard = new PBoard(vertSize, horSize);
                    for (int i = 0; i < rT; i++)
                    {
                        for (int j = 0; j < cT; j++)
                        {
                            nPBoard[i, j] = pBoard[i, j];
                        }
                    }
                    boardJS = JsonUtility.ToJson(nPBoard);
                }
            }
            SetAsDirty();
        }

        public void IncBackGround(int length)
        {
            backGroundNumber++;
            backGroundNumber = (int)Mathf.Repeat(backGroundNumber, length);
            Save();
        }

        public void DecBackGround(int length)
        {
            backGroundNumber--;
            backGroundNumber = (int)Mathf.Repeat(backGroundNumber, length);
            Save();
        }

        public bool HaveFillPath(MatchGrid mGrid)
        {
            PBoard pB = GetBoard(mGrid);
            return (pB != null && pB.HaveFillPath());
        }

        public void SetMatherDir(MatchGrid mGrid, int row, int column, DirMather dir)
        {
            PBoard pBoard = GetBoard(mGrid);
            pBoard[row, column] = dir;
            boardJS = JsonUtility.ToJson(pBoard);
            Debug.Log("serialize : " + boardJS);
            SetAsDirty();
        }

        public DirMather GetMatherDir(MatchGrid mGrid, int row, int column)
        {
            PBoard pBoard = GetBoard(mGrid);
            return pBoard[row, column];
        }

        public void RemoveMatherDir(MatchGrid mGrid, int row, int column)
        {
            PBoard pBoard = GetBoard(mGrid);
            pBoard[row, column] = 0;
            boardJS = JsonUtility.ToJson(pBoard);
            SetAsDirty();
        }

        public bool HaveSpawners()
        {
            return spawnCells != null && spawnCells.Count > 0;
        }

        public  PBoard GetBoard(MatchGrid mGrid)
        {
            DirMather[] cells = null;
            int r = mGrid.Rows.Count;
            int c = mGrid.Columns.Count;
            cells = new DirMather[r * c];
            PBoard pBoard = null;

            if (!string.IsNullOrEmpty(boardJS))
            {
                pBoard = JsonUtility.FromJson<PBoard>(boardJS);
                if (pBoard != null && pBoard.cells != null && pBoard.rows > 0 && pBoard.columns > 0 && pBoard.rows == r && pBoard.columns == c && pBoard.cells.Length == r * c)
                {
                    int rT = pBoard.rows;
                    int cT = pBoard.columns;

                    for (int i = 0; i < r; i++) // copy
                    {
                        for (int j = 0; j < c; j++)
                        {
                            cells[i * c + j] = (i < rT && j < cT) ? pBoard[i, j] : DirMather.None;
                        }
                    }
                    pBoard.cells = cells;
                }
                else
                {
                    pBoard = null;
                }
            }

            if (pBoard == null)
            {
                pBoard = new PBoard(r, c);
            }

            return pBoard;
        }

        private float RoundToFloat(float val, float delta)
        {
            int vi = Mathf.RoundToInt(val / delta);
            return (float)vi * delta;
        }

        private void RemoveCellData(List<CellData> cdl, CellData cd)
        {
            if (cdl != null) cdl.RemoveAll((c) => { return ((cd.Column == c.Column) && (cd.Row == c.Row)); });
        }

        private bool ContainCellData(List<CellData> lcd, CellData cd)
        {
            if (lcd == null || cd == null) return false;
            foreach (var item in lcd)
            {
                if ((item.Row == cd.Row) && (item.Column == cd.Column)) return true;
            }
            return false;
        }

        private bool ContainEqualCellData(List<CellData> lcd, CellData cd)
        {
            if (lcd == null || cd == null) return false;
            foreach (var item in lcd)
            {
                if ((item.Row == cd.Row) && (item.Column == cd.Column) && (cd.ID == item.ID)) return true;
            }
            return false;
        }

        internal void SaveObjects(GridCell gC)
        {
            cells.RemoveAll((c)=> { return ((c.row == gC.Row) && (c.column == gC.Column)); });
            List<GridObjectState> gOSs = gC.GetGridObjectsStates();
            if (gOSs.Count > 0) cells.Add(new GCellObects(gC.Row, gC.Column, gOSs));

            SetAsDirty();
        }

        internal void Scan(MatchGrid  mGrid)
        {
            foreach (var item in mGrid.Cells)
            {
                SaveObjects(item);
            }
        }

        internal List<MatchObject> GetMatchObjects(GameObjectsSet goSet)
        {
            List<MatchObject> res = new List<MatchObject>();
            if (usedMatchObjects == null || usedMatchObjects.Count < 5) return new List<MatchObject>(goSet.MatchObjects);
            foreach (var item in usedMatchObjects)
            {
                if (item && !res.Contains(item) && goSet.MatchObjects.Contains(item)) res.Add(item);
            }
            return (res.Count >= 5) ? res : new List<MatchObject>(goSet.MatchObjects);
        }
    }

    [Serializable]
    public class GCellObects
    {
        public int row;
        public int column;
        public List<GridObjectState> gridObjects;

        public GCellObects(int row, int column, List<GridObjectState> gridObjects)
        {
            this.row = row;
            this.column = column;
            this.gridObjects = new List<GridObjectState>(gridObjects);
        }
    }

    [Serializable]
    public class PCell
    {
        public int matherRow;
        public int matherColumn;
        public bool haveMather;

        public PCell( int matherRow, int matherColumn, bool haveMather)
        {
            this.matherRow = matherRow;
            this.matherColumn = matherColumn;
            this.haveMather = haveMather;
        }

        public PCell(PCell pCell)
        {
            matherRow = pCell.matherRow;
            matherColumn = pCell.matherColumn;
            haveMather = pCell.haveMather;
        }
    }

    [Serializable]
    public class PBoard
    {
        public DirMather[] cells;

        public int rows;
        public int columns;

        public PBoard(int rows, int columns)
        {
            this.rows = rows;
            this.columns = columns;

            cells = new DirMather[rows * columns];
        }

        public DirMather this[int index0, int index1]
        {
            get
            {
                if (cells == null) return DirMather.None;
                if (rows > index0 && columns > index1 && rows * columns == cells.Length)
                {
                    return cells[index0 * columns + index1];
                }
                else
                {
                    return DirMather.None;
                }
            }

            set
            {
                if (cells != null && rows > index0 && columns > index1 && rows * columns == cells.Length)
                {
                    cells[index0 * columns + index1] = value;
                }
                else { }
            }
        }


        public bool HaveFillPath()
        {
            int count = 0;
            foreach (var item in cells)
            {
                if (item != 0) count++;
            }
            return count > ((rows * columns) / 3);
        }

        private Vector2Int GetMatherPos(int index0, int index1)
        {
            Vector2Int res = new Vector2Int(-1, -1);
            DirMather dir = this[index0, index1];
            Vector2Int dPos = Vector2Int.zero;

            switch (dir)
            {
                case DirMather.None:
                    break;
                case DirMather.Top:
                    dPos = new Vector2Int(-1, 0);
                    break;
                case DirMather.Right:
                    dPos = new Vector2Int(0, 1);
                    break;
                case DirMather.Bottom:
                    dPos = new Vector2Int(1, 0);
                    break;
                case DirMather.Left:
                    dPos = new Vector2Int(0, -1);
                    break;
            }
            res = new Vector2Int(index0, index1) + dPos;

            if (dPos != Vector2Int.zero)
            {
                while (IndexOk(res.x, res.y) && this[res.x, res.y] == DirMather.None) // search mather in direction
                {
                    res += dPos;
                }
            }
            return res;
        }

        public bool HaveChilds(int index0, int index1)
        {
            Vector2Int[,] mathers = new Vector2Int[rows, columns];

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    Vector2Int m = GetMatherPos(i, j);
                    mathers[i, j] = m;
                }
            }

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    if (mathers[i, j].x == index0 && mathers[i, j].y == index1) return true;
                }
            }
            return false;
        }

        private bool IndexOk(int index0, int index1)
        {
            return (index0 >= 0 && index0 < rows && index1 >= 0 && index1 < columns);
        }
    }
}



