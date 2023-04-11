using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

namespace Mkey
{
    public class HeaderGUIController : MonoBehaviour
    {
        [Header("Stars")]
        [SerializeField]
        private Image star1;
        [SerializeField]
        private Image star2;
        [SerializeField]
        private Image star3;
        [SerializeField]
        private GameObject starPrefab;


        [Space(8)]
        [Header("Moves and Time")]
        [SerializeField]
        private GameObject MovesBlock;
        [SerializeField]
        private Text MovesCountText;
        [SerializeField]
        private GameObject TimerBlock;
        [SerializeField]
        private Text TimerText;
        [SerializeField]
        private Text levelNumber;

        [Space(8)]
        [Header("Targets")]
        [SerializeField]
        private GameObject targetPrefab;
        [SerializeField]
        private RectTransform targetsContainer;

        [Space(8)]
        [Header("Score strip")]
        [SerializeField]
        private Image ScoreStrip;

        #region temp vars
        private GameObject fullStar_1;
        private GameObject fullStar_2;
        private GameObject fullStar_3;
        private int oldCount;

        private GameBoard MBoard => GameBoard.Instance; 
        private SoundMaster MSound => SoundMaster.Instance; 
        private GameConstructSet GCSet => GameConstructSet.Instance;
        private LevelConstructSet LCSet => GCSet.GetLevelConstructSet(GameLevelHolder.CurrentLevel);
        private GameObjectsSet GOSet => GCSet.GOSet;

        private StarsHolder MStars => StarsHolder.Instance;
        private ScoreHolder MScore => ScoreHolder.Instance;
        #endregion temp vars

        public static HeaderGUIController Instance { get; private set; }

        #region regular
        private void Awake()
        {
            if (Instance) Destroy(Instance.gameObject);
            Instance = this;
        }

        private void Start()
        {
            CreateTargets();

            // set event handlers 
            if (GameBoard.GMode == GameMode.Play)
            {
                Refresh();
                MScore.ChangeEvent.AddListener(RefreshScoreStrip);
                MStars.ChangeEvent.AddListener(RefreshStars);

                WinController winContr = MBoard.WinContr;
                if (MovesBlock) MovesBlock.SetActive(!winContr.IsTimeLevel);
                if (TimerBlock) TimerBlock.SetActive(winContr.IsTimeLevel);

                if (!winContr.IsTimeLevel)
                {
                    winContr.ChangeMovesEvent += (int m) => { if (MovesCountText) MovesCountText.text = m.ToString(); };
                }
                else
                {
                    winContr.TimerTickEvent += (float t) => { if (TimerText) TimerText.text = t.ToString(); };
                }
            }
        }

        private void OnDestroy()
        {
            MScore.ChangeEvent.RemoveListener(RefreshScoreStrip);
            MStars.ChangeEvent.RemoveListener(RefreshStars);
            if (ScoreStrip) SimpleTween.Cancel(ScoreStrip.gameObject, false);
        }
        #endregion regular

        public void Refresh()
        {
            RefreshLevel();
            RefreshTimeMoves();
            RefreshStars(StarsHolder.Count);
            RefreshScoreStrip(ScoreHolder.Count);
        }

        public void RefreshLevel()
        {
            if (levelNumber) levelNumber.text = (GameLevelHolder.CurrentLevel + 1).ToString();
        }

        public void RefreshTimeMoves()
        {
            if (GameBoard.GMode == GameMode.Edit)
            {
                MissionConstruct mc = MBoard.FullLevelMission;
                if (MovesCountText) MovesCountText.text = mc.MovesConstrain.ToString();
                if (TimerText) TimerText.text = mc.TimeConstrain.ToString();
                if (MovesBlock) MovesBlock.SetActive(!mc.IsTimeLevel);
                if (TimerBlock) TimerBlock.SetActive(mc.IsTimeLevel);
            }
            else
            {
                WinController winContr = MBoard.WinContr;
                if (!winContr.IsTimeLevel)
                {
                    if (MovesCountText) MovesCountText.text = winContr.MovesRest.ToString();
                }
                else
                {
                    if (TimerText) TimerText.text = winContr.TimeRest.ToString();
                }
            }
        }

        public void CreateTargets()
        {
            if (!targetsContainer) return;
            if (!targetPrefab) return;

            GUIObjectTargetHelper[] ts = targetsContainer.GetComponentsInChildren<GUIObjectTargetHelper>();
            foreach (var item in ts)
            {
                Destroy(item.gameObject);
            }

            foreach (var item in MBoard.Targets)
            {
                RectTransform t = Instantiate(targetPrefab, targetsContainer).GetComponent<RectTransform>();
                GUIObjectTargetHelper th = t.GetComponent<GUIObjectTargetHelper>();
                th.SetData(item.Value, true);
                th.SetIcon(GOSet.GetObject(item.Value.ID).GuiImage);
                item.Value.ChangeCountEvent += (td) => { if (this) { th.SetData(td, true); th.gameObject.SetActive(td.NeedCount > 0); } };
                th.gameObject.SetActive(item.Value.NeedCount > 0);
            }
        }

        public void SetControlActivity(bool activity)
        {
            Button[] buttons = GetComponentsInChildren<Button>();
            for (int i = 0; i < buttons.Length; i++)
            {
                buttons[i].interactable = activity;
            }
        }

        #region stars
        internal void RefreshStars(int count)
        {
                SetStars(count, null);
        }

        /// <summary>
        /// Instantiate stars objects with any animation
        /// </summary>
        /// <param name="starCount"></param>
        /// <param name="completeCallBack"></param>
        private void SetStars(int starCount, Action completeCallBack)
        {
            if (fullStar_1 && starCount < 1) Destroy(fullStar_1);
            if (fullStar_2 && starCount < 2) Destroy(fullStar_2);
            if (fullStar_3 && starCount < 3) Destroy(fullStar_3);

            TweenSeq ts = new TweenSeq();

            if (!fullStar_1 && starCount > 0)
            {
                ts.Add((callBack) =>
                {
                    fullStar_1 = InstantiateNewStar(starPrefab, star1.transform);
                    AnimateNewStar(fullStar_1, callBack);
                });

            }

            if (!fullStar_2 && starCount > 1)
            {
                ts.Add((callBack) =>
                {
                    fullStar_2 = InstantiateNewStar(starPrefab, star2.transform);
                    AnimateNewStar(fullStar_2, callBack);
                });
            }

            if (!fullStar_3 && starCount > 2)
            {
                ts.Add((callBack) =>
                {
                    fullStar_3 = InstantiateNewStar(starPrefab, star3.transform);
                    AnimateNewStar(fullStar_3, callBack);
                });
            }

            ts.Add((callBack) =>
            {
                 completeCallBack?.Invoke();
                callBack();
            });

            ts.Start();
        }

        private GameObject InstantiateNewStar(GameObject prefab, Transform target)
        {
            GameObject star = Instantiate(prefab, target.position, Quaternion.identity);
            star.transform.localScale = target.lossyScale;
            star.transform.parent = target;
            return star;
        }

        private void AnimateNewStar(GameObject star, Action completeCallBack)
        {
            SimpleTween.Value(star, 0, 1, 0.5f).SetOnUpdate((val) =>
            {
                if (star) star.transform.localScale = new Vector3(val, val, val);
            }).AddCompleteCallBack(() =>
            {
               completeCallBack?.Invoke();
            }).SetEase(EaseAnim.EaseOutBounce);
        }
        #endregion stars

        #region score strip
        internal void RefreshScoreStrip(int score)
        {
            int averageScore = ScoreHolder.AverageScore;
            float amount = (averageScore > 0) ? (float)score / (float)(averageScore * 2.0f) : 0;
            if (ScoreStrip)
                SimpleTween.Value(ScoreStrip.gameObject, ScoreStrip.fillAmount, amount, 0.3f).SetOnUpdate((float val) =>
                {
                    if (ScoreStrip) ScoreStrip.fillAmount = val;
                }).SetEase(EaseAnim.EaseOutCubic);
        }
        #endregion score strip
    }
}