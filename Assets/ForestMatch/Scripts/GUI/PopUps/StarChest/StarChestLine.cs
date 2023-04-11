using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace Mkey
{
	public class StarChestLine : MonoBehaviour
	{
        #region temp vars
        [SerializeField]
        private Image rewardImage;
        [SerializeField]
        private AudioClip clip;

        [SerializeField]
        private UnityEvent ApplyRewardEvent;

        #endregion temp vars
        private SoundMaster MSound => SoundMaster.Instance;
        #region regular
        private void Start()
        {

            if(rewardImage) { rewardImage.enabled = false; }
        }

        internal void Show(Action completeCallBack)
        {
            ApplyRewardEvent?.Invoke();

            if(!rewardImage)
            {
                completeCallBack?.Invoke();
                return;
            }

            if (rewardImage)
            {
                if (clip) MSound.PlayClip(0, clip);
                rewardImage.enabled = true;
                RectTransform rT = rewardImage.GetComponent<RectTransform>();
                SimpleTween.Value(gameObject, Vector3.zero, Vector3.one*1.5f, 0.5f).SetOnUpdate((Vector3 t)=> { if (rT) rT.localScale = t; }).AddCompleteCallBack(completeCallBack).SetEase(EaseAnim.EaseOutBounce);
            }
        }
        #endregion regular
    }
}
