using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

namespace Mkey
{
    public class SpawnController : MonoBehaviour
    {

        public static SpawnController Instance;

        private void Awake()
        {
            if (Instance) Destroy(gameObject);
            else
            {
                Instance = this;
            }
        }

       

        /// <summary>
        /// spawn new MO or return previous spawned
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public GridObject Get(GridCell gridCell, LevelConstructSet lCSet, GameObjectsSet gOSet, GameBoard mBoard, Vector3 position)
        {
            if (!gridCell) return null;
            MatchObject prefab = GetMainRandomObjectPrefab(lCSet, gOSet);
            if (!prefab) return null;

            GridObject match = prefab.Create(gridCell, mBoard.TargetCollectEventHandler);
            if (match)
            {
                match.transform.position = position;
            }
            return match;
        }

        public MatchObject GetMainRandomObjectPrefab(LevelConstructSet lCSet, GameObjectsSet gOSet)
        {

            List<MatchObject> gridObjects = lCSet.GetMatchObjects(gOSet);
            gridObjects.Shuffle();
            return gridObjects.Count > 0 ? gridObjects[0] : null;
        }

        //get picked object block
        public MatchObject GetPickedObjectBlock(LevelConstructSet lCSet, GameObjectsSet gOSet)
        {
            List<MatchObject> gridObjects = lCSet.GetMatchObjects(gOSet);
            //gridObjects.Shuffle();
            return gridObjects.Count > 0 ? gridObjects[0] : null;
        }

        ////////////////////////////////////////////////////////////////////////

        public MatchObject GetPickMatchObject(LevelConstructSet lCSet, GameObjectsSet gOSet,int idx)
        {
            List<MatchObject> gridObjects = lCSet.GetMatchObjects(gOSet);


            return gridObjects[idx];

            //gridObjects.Shuffle();
            //return gridObjects.Count > 0 ? gridObjects[0] : null;
        }




        public MatchObject GetRandomObjectPrefab(LevelConstructSet lCSet, GameObjectsSet gOSet)
        {
            List<MatchObject> gridObjects = lCSet.GetMatchObjects(gOSet);

            //List<MatchObject> gridObjects = lCSet.GetAllObjects(gOSet);


            gridObjects.Shuffle();
            return gridObjects.Count > 0 ? gridObjects[0] : null;
        }


        public BlockedObject GetRandomBlockedObjectPrefab(LevelConstructSet lCSet, GameObjectsSet gOSet)
        {
            //List<MatchObject> gridObjects = lCSet.GetMatchObjects(gOSet);

            List<BlockedObject> gridObjects = lCSet.GetBlockedObjects(gOSet);

            //gridObjects.Shuffle();
            return gridObjects.Count > 0 ? gridObjects[0] : null;
        }

        public OverlayObject GetRandomOverlayObjectPrefab(LevelConstructSet lCSet, GameObjectsSet gOSet)
        {
            //List<MatchObject> gridObjects = lCSet.GetMatchObjects(gOSet);

            List<OverlayObject> gridObjects = lCSet.GetOverlayObjects(gOSet);

            gridObjects.Shuffle();

            return gridObjects.Count > 0 ? gridObjects[0] : null;
        }

        public UnderlayObject GetRandomUnderlayObjectPrefab(LevelConstructSet lCSet, GameObjectsSet gOSet)
        {
            //List<MatchObject> gridObjects = lCSet.GetMatchObjects(gOSet);

            List<UnderlayObject> gridObjects = lCSet.GetUnderlayObjects(gOSet);

            gridObjects.Shuffle();
            return gridObjects.Count > 0 ? gridObjects[0] : null;
        }

        public FallingObject GetRandomUgetFallingObjectPrefab(LevelConstructSet lCSet, GameObjectsSet gOSet)
        {
            //List<MatchObject> gridObjects = lCSet.GetMatchObjects(gOSet);

            List<FallingObject> gridObjects = lCSet.GetFallingObjects(gOSet);

            gridObjects.Shuffle();
            return gridObjects.Count > 0 ? gridObjects[0] : null;
        }

        public HiddenObject GetRandomHiddenObjectPrefab(LevelConstructSet lCSet, GameObjectsSet gOSet)
        {
            //List<MatchObject> gridObjects = lCSet.GetMatchObjects(gOSet);

            List<HiddenObject> gridObjects = lCSet.GetHiddenObjects(gOSet);

            gridObjects.Shuffle();
            return gridObjects.Count > 0 ? gridObjects[0] : null;
        }


        public MatchObject GetMainObjectPrefab(GameObjectsSet gOSet, int id)
        {
           return  gOSet.GetMainObject(id);
        }

        public List<MatchObject> GetMainRandomObjectPrefabs( int count, LevelConstructSet lCSet, GameObjectsSet gOSet)
        {
            List<MatchObject> gridObjects = lCSet.GetMatchObjects(gOSet);
            gridObjects.Shuffle();

            return gridObjects.Count >= count ? gridObjects.GetRange(0, count) : gridObjects;
        }

        public MatchObject GetMainRandomObjectPrefab(LevelConstructSet lCSet, GameObjectsSet gOSet, List<GridObject> exclude)
        {
            List<MatchObject> gridObjects = lCSet.GetMatchObjects(gOSet);
            gridObjects.Shuffle();
            if (exclude != null)
                gridObjects.RemoveAll((gO) => { return exclude.Contains(gO); });

            return (gridObjects.Count > 0) ? gridObjects[0] : null;
        }















    }
}