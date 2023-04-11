﻿using System;
using UnityEngine;

namespace Mkey
{
    public class TargetData
    {
        public int ID { get; private set; }
        public int NeedCount { get; private set; }
        public int CurrCount { get; private set; }

        public Action<TargetData> ChangeCountEvent; // <currcount, needcount>

        public bool Achieved { get { return CurrCount >= NeedCount; } }

        public TargetData(int id, int needCount)
        {
            ID = id;
            NeedCount = needCount;
            CurrCount = 0;
        }

        public Sprite GetImage(GameObjectsSet mSet)
        {
            GridObject bD = mSet.GetObject(ID);
            return (bD != null) ? bD.GuiImage : null;
        }

        public void IncCurrCount()
        {
            SetCurrCount(CurrCount + 1);
        }

        public void IncCurrCount(int inccount)
        {
            SetCurrCount(CurrCount + inccount);
        }

        public void SetCurrCount(int newCount)
        {
            newCount = Mathf.Max(0, newCount);
            bool changed = (CurrCount != newCount);
            CurrCount = newCount;
            if (changed) ChangeCountEvent?.Invoke(this);
        }

        public void IncNeedCount(int incCount)
        {
            SetNeedCount(NeedCount + incCount);
        }

        public void SetNeedCount(int newCount)
        {
            newCount = Mathf.Max(0, newCount);
            bool changed = (NeedCount != newCount);
            NeedCount = newCount;
            if (changed) ChangeCountEvent?.Invoke(this);
        }

        public TargetData Duplicate()
        {
            return new TargetData(ID, NeedCount);
        }
    }
}