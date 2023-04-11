using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Mkey
{
    public class PanelContainerController : MonoBehaviour
    {
        public Button OpenCloseButton;
        public Button BrushSelectButton;
        public Image selector;
        public Image brushImage;
        public Text BrushName;
        public string capital;
        public List<GridObject> gridObjects;

        [SerializeField]
        private ScrollPanelController ScrollPanelPrefab;

        internal ScrollPanelController ScrollPanel;

        public ScrollPanelController InstantiateScrollPanel()
        {
            if (!ScrollPanelPrefab) return null;

            if (ScrollPanel) DestroyImmediate(ScrollPanel.gameObject);

            RectTransform panel = Instantiate(ScrollPanelPrefab).GetComponent<RectTransform>();
            panel.SetParent(GetComponent<RectTransform>());
            panel.anchoredPosition = new Vector2(0, 0);
            ScrollPanel = panel.GetComponent<ScrollPanelController>();
            return ScrollPanel;
        }
    }
}