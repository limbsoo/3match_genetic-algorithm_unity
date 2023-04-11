using System;
using UnityEngine;

namespace Mkey
{
    public class SwapHelper 
    {
        public static GridCell Source;
        public static GridCell Target;

        public static Action<GridCell, GridCell> SwapBeginEvent;
        public static Func<GridCell, GridCell, bool> BombsCombinedEvent;
        public static Action<GridCell, GridCell> SwapEndEvent;
        private static TouchManager Touch { get { return TouchManager.Instance; } }

        public static void Swap()
        {
            Source = (Touch.Draggable) ? Touch.Source : null;
            Target = Touch.Target;

            Swap(Source, Target);
            Touch.SetDraggable(null, null);
            Touch.SetTarget(null);
        }

        public static void Swap(GridCell gc1, GridCell gc2)
        {
            Source = gc1;
            Target = gc2;

            if (Source && Source.CanSwap(Target))
            {
                if (Source && Source.CanCombined(Target))
                {
                    if (BombsCombinedEvent != null && BombsCombinedEvent(Source, Target)) return;
                }
                SwapBeginEvent?.Invoke(Source, Target);
                MatchObject dM = Source.Match;
                MatchObject tM = Target.Match;
                if (dM) dM.SwapTime = Time.time;
                if (tM) tM.SwapTime = Time.time;
                GameObject tDO = Target.DynamicObject;
                GameObject sDO = Source.DynamicObject;
                Source.GrabDynamicObject(tDO, false, null);
                Target.GrabDynamicObject(sDO, false, () =>
                {
                    SwapEndEvent?.Invoke(Source, Target);
                });
            }
            else if (Source)
            {
                Touch.ResetDrag(null);
            }
        }

        public static void UndoSwap(Action callBack)
        {
            GameObject tDO = Target.DynamicObject;
            GameObject sDO = Source.DynamicObject;

            Source.GrabDynamicObject(tDO, false, null);
            Target.GrabDynamicObject(sDO, false, () => { callBack?.Invoke(); });
        }
    }
}