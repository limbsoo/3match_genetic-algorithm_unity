using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Mkey
{
    public class StoryPage : PopUpsController
    {
        [Space (8)]
        [SerializeField]
        private StoryTab tabPrefab;
        [SerializeField]
        private RectTransform tabsParent;

        [Space(8)]
        [SerializeField]
        private View[] views;
        private int next;
        private StoryTab current;

        public override void RefreshWindow()
        {
            if (!tabsParent || !tabPrefab) return;
            if (views == null || views.Length == 0) return;

            foreach (var item in tabsParent.GetComponentsInChildren<StoryTab>())
            {
                DestroyImmediate(item.gameObject);
            }

            ShowNext();
            base.RefreshWindow();
        }

        public void ShowNext()
        {
            if(views != null && views.Length > next)
            {
                if(tabPrefab.slide && views[next].image)
                {
                   tabPrefab.slide.sprite = views[next].image;
                }
                if (tabPrefab.MessageText)
                {
                    tabPrefab.MessageText.text = views[next].message;
                }
                if (current)
                {
                    if (views[next].fade)
                    {
                        SetControlActivity(false);
                        StoryTab old = current;
                        old.GetComponent<GuiFader_v2>().FadeOut(0, null);
                        current = Instantiate(tabPrefab, tabsParent);
                        SetControlActivity(false);
                        current.GetComponent<GuiFader_v2>().FadeIn(0, () => { SetControlActivity(true); });
                    }
                    else
                    {
                        StoryTab old = current;
                        DestroyImmediate(old); 
                        current = Instantiate(tabPrefab, tabsParent);
                    }
                }
                else
                {
                    current = Instantiate(tabPrefab, tabsParent);
                }
           
            }
            else
            {
                if (next != 0)
                {
                    CloseWindow();
                    return;
                }
            }
            next++;
        }

        public void Show()
        {
            if (!this) return;
            GuiController.Instance.ShowPopUp(this);
        }
    }

    [Serializable]
    public class View
    {
        public bool fade = true;
        public string message;
        public Sprite image;
    }
}

