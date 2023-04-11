using System;
using System.Collections.Generic;
using UnityEngine;

namespace Mkey
{
    public class DynamicClickBombObject : BombObject
    {
        #region properties
        #endregion properties

        #region regular
 
        #endregion regular

        #region create
        /// <summary>
        /// Create new DynamicClickBombObject for gridcell
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="oData"></param>
        /// <param name="addCollider"></param>
        /// <param name="radius"></param>
        /// <param name="isTrigger"></param>
        /// <returns></returns>
        public static DynamicClickBombObject Create(GridCell parent, DynamicClickBombObject prefab,  Action<int> TargetCollectEvent)
        {
            if (!parent || !prefab) return null;
            GridObject b = prefab.Create(parent, TargetCollectEvent);
            return (b) ? b.GetComponent<DynamicClickBombObject>() : null;
        }

        /// <summary>
        /// Create new DynamicClickBombObject over board, without parent
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="oData"></param>
        /// <param name="addCollider"></param>
        /// <param name="radius"></param>
        /// <param name="isTrigger"></param>
        /// <returns></returns>
        public static DynamicClickBombObject CreateOverBoard(DynamicClickBombObject prefab, Vector3 position, Vector3 localScale, Action<int> TargetCollectEvent)
        {
            if (!prefab) return null;

            DynamicClickBombObject gridObject = Instantiate(prefab);
            if (!gridObject) return null;
            gridObject.transform.localScale = localScale;
            gridObject.transform.localPosition = position;
#if UNITY_EDITOR
            gridObject.name = "DynamicClickBomb: " + gridObject.ID;
#endif

            return gridObject;
        }
        #endregion create

        #region override
        public override void SetToFront(bool set)
        {
            if (!SRenderer) SRenderer = GetComponent<SpriteRenderer>();
            if (!SRenderer) return;
            if (set)
                SRenderer.sortingOrder = SortingOrder.DynamicClickBombToFront;
            else
                SRenderer.sortingOrder = SortingOrder.DynamicClickBomb;
        }

        public override GridObject Create(GridCell parent, Action<int> TargetCollectEvent)
        {
            if (!parent) return null;
            if (parent.IsDisabled || parent.Blocked) { return null; }
            if (parent.DynamicObject)
            {
                GameObject old = parent.DynamicObject;
                DestroyImmediate(old);
            }

            DynamicClickBombObject gridObject = Instantiate(this, parent.transform);
            if (!gridObject) return null;
            gridObject.transform.localScale = Vector3.one;
            gridObject.transform.localPosition = Vector3.zero;
            gridObject.SRenderer = gridObject.GetComponent<SpriteRenderer>();
#if UNITY_EDITOR
            gridObject.name = "DynamicClickBomb: " + gridObject.ID;
#endif
            gridObject.TargetCollectEvent = TargetCollectEvent;
            gridObject.SetToFront(false);
            return gridObject;
        }
        #endregion override
        }
}
