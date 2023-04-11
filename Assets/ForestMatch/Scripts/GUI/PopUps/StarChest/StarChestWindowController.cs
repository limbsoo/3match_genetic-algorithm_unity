using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Mkey
{
	public class StarChestWindowController : PopUpsController
	{
        [SerializeField]
        private RectTransform chest;
        [SerializeField]
        private Sprite openedChest;
        [SerializeField]
        private Sprite closedChest;
        [SerializeField]
        private List<StarChestLine> chestLines;
        [SerializeField]
        private Image chestLight;
        [SerializeField]
        private GameObject button;

        #region temp vars
        private StarChestController SCC { get { return StarChestController.Instance; } }
        private int rIndex = 0;
        [SerializeField]
        private bool move = false;
        #endregion temp vars

        public Action OpenChestEvent;

        #region regular
        public override void RefreshWindow()
        {
            rIndex = UnityEngine.Random.Range(0, chestLines.Count);
            base.RefreshWindow();
        }

        private void OnDestroy()
        {
			
        }
		#endregion regular

        public void Open_Click()
        {
            Open(CloseWindow);
            SCC.ResetData();
            SetControlActivity(false);
            if (button) button.SetActive(false);
            OpenChestEvent?.Invoke();
        }

        public void ScaleOut(Action completeCallBack, float delay)
        {
            if (chest) chest.localScale = Vector3.zero;
            SimpleTween.Value(gameObject, Vector3.zero, Vector3.one, 0.5f).SetOnUpdate((Vector3 val) =>
            {
                if (chest) chest.localScale = val;
            })
            .SetDelay(delay)
            .SetEase(EaseAnim.EaseOutBounce)
            .AddCompleteCallBack(completeCallBack);
        }

        public void Open(Action completeCallBack)
        {
            if(!chest)
            {
                completeCallBack?.Invoke();
                return;
            }

            TweenSeq ts = new TweenSeq();

            Image im = chest.GetComponent<Image>();

            chest.localScale = Vector3.zero;

            ts.Add((callBack) =>
            {
                SimpleTween.Value(gameObject, Vector3.one, new Vector3(1.5f, 0.5f, 1f), 0.1f).SetOnUpdate((Vector3 val) =>
                {
                    if (chest) chest.localScale = val;
                })
          .AddCompleteCallBack(callBack);
            });

            ts.Add((callBack) =>
            {
                if (move)
                    SimpleTween.Value(gameObject, 0, 5, 0.15f).SetOnUpdate((float val) =>
                    {
                        if (chest) { chest.anchoredPosition -= new Vector2(0, val); }
                    });
                SimpleTween.Value(gameObject, new Vector3(1.55f, 0.5f, 1f), new Vector3(1.00f, 1.00f, 1.00f), 0.25f).SetOnUpdate((Vector3 val) =>
                {
                    if (chest) chest.localScale = val;
                })
          .SetEase(EaseAnim.EaseOutBounce)
          .AddCompleteCallBack(callBack);
            });

            ts.Add((callBack) =>
            {
                if (openedChest && im)
                {
                    im.sprite = openedChest;
                }
                if (chestLight)
                {
                    chestLight.gameObject.SetActive(true);
                    SimpleTween.Value(gameObject, -Mathf.PI / 4f, Mathf.PI / 4f, 1f).SetOnUpdate((float val) =>
                    {
                        if (chestLight) chestLight.color = new Color(1, 1, 1, Mathf.Cos(val));
                    }).SetCycled();
                }
                //if (coinsFountain)
                //{
                //    if (coinsFountainPosition) coinsFountain.transform.position = coinsFountainPosition.position;
                //    coinsFountain.Jump();
                //}
                //if (coinsText)
                //{
                //    coinsText.gameObject.SetActive(true);
                //    coinsText.text = Coins.ToString("# ### ### ### ###");
                //}

                
                callBack?.Invoke();
            });

            ts.Add((callBack) =>
            {
                chestLines[rIndex].Show(callBack);
            });

            ts.Add((callBack) =>
            {
                SimpleTween.Value(gameObject, 0, 1, 2).AddCompleteCallBack(callBack);
            });

            ts.Add((callBack) =>
            {
                completeCallBack?.Invoke();
            });
            ts.Start();
        }
    }
}
