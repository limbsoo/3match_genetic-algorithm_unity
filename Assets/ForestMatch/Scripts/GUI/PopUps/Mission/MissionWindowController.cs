using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Mkey
{
    public class MissionWindowController : PopUpsController
    {
        [SerializeField]
        private Text levelText;

        [Space(8)]       
        [Header ("Targets")]
        [SerializeField]
        private Text scoreText;
        [SerializeField]
        private Text getScoreText;
        [SerializeField]
        private RectTransform targetsContainer;
        [SerializeField]
        private MissionTarget targetPrefab;

        [Space(8)]
        [Header("Boosters")]
        [SerializeField]
        private RectTransform BoostersParent;
        [SerializeField]
        private int boostersCount = 3;

        #region temp wars
        private GameBoard MBoard => GameBoard.Instance;
        private GuiController MGui => GuiController.Instance;
        private WinController WController => MBoard ? MBoard.WinContr : null;
        private GameConstructSet GCSet => GameConstructSet.Instance;
        private GameObjectsSet GOSet => (GCSet) ? GCSet.GOSet : null;
        private LevelConstructSet LCSet => (GCSet) ? GCSet.GetLevelConstructSet(GameLevelHolder.CurrentLevel) : null; 
        #endregion temp wars

        public override void RefreshWindow()
        {
            levelText.text = " Level #" + (GameLevelHolder.CurrentLevel +1).ToString();
            getScoreText.gameObject.SetActive(MBoard.WinContr.HasScoreTarget);
            scoreText.text = MBoard.WinContr.ScoreTarget.ToString();
            CreateTargets();
            CreateBoostersPanel();
            base.RefreshWindow();
        }

        public void CreateTargets()
        {
            if (!targetsContainer) return;
            if (!targetPrefab) return;

            MissionTarget[] ts = targetsContainer.GetComponentsInChildren<MissionTarget>();
            foreach (var item in ts)
            {
                DestroyImmediate(item.gameObject);
            }

            foreach (var item in MBoard.Targets)
            {
                targetPrefab.SetIcon(GOSet.GetObject(item.Value.ID).GuiImage); // unity 2019 fix
                
                RectTransform t = Instantiate(targetPrefab, targetsContainer).GetComponent<RectTransform>();
                MissionTarget th = t.GetComponent<MissionTarget>();
                th.SetData(item.Value, true);
                th.SetIcon(GOSet.GetObject(item.Value.ID).GuiImage);
                th.gameObject.SetActive(item.Value.NeedCount > 0);
            }
        }

        public void Play_Click()
        {
            CloseWindow();
        }

        private void CreateBoostersPanel()
        {
            GuiBoosterHelper[] ms = BoostersParent.GetComponentsInChildren<GuiBoosterHelper>();
            foreach (var item in ms)
            {
                DestroyImmediate(item.gameObject);
            }
            List<Booster> bList = new List<Booster>();
            List<Booster> bListToShop = new List<Booster>();

            bool selectFromAll = true;

            if (!selectFromAll)
            {
                foreach (var b in GOSet.BoosterObjects)
                {
                    if (b.Count > 0) bList.Add(b);
                    else bListToShop.Add(b);
                }

                bList.Shuffle();
                int bCount = Mathf.Min(bList.Count, boostersCount);
                for (int i = 0; i < bCount; i++)
                {
                    Booster b = bList[i];
                    GuiBoosterHelper bM = b.CreateGuiHelper(BoostersParent, "mission");
                }

                int shopCount = boostersCount - bList.Count;
                if (shopCount > 0)
                {
                    shopCount = Mathf.Min(shopCount, bListToShop.Count);
                    bListToShop.Shuffle();

                    for (int i = 0; i < shopCount; i++)
                    {
                        Booster b = bListToShop[i];
                        GuiBoosterHelper bM = b.CreateGuiHelper(BoostersParent, "mission");
                    }
                }
            }
            else
            {
                foreach (var b in GOSet.BoosterObjects)
                {
                    bList.Add(b);
                }

                bList.Shuffle();
                int bCount = Mathf.Min(bList.Count, boostersCount);
                for (int i = 0; i < bCount; i++)
                {
                    Booster b = bList[i];
                    GuiBoosterHelper bM = b.CreateGuiHelper(BoostersParent, "mission");
                }
            }
        }

        public void ToMap_Click()
        {
            CloseWindow();
            SceneLoader.Instance.LoadScene(1);
        }
    }
}