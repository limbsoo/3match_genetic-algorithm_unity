using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Mkey
{
    public class FooterGUIController : MonoBehaviour
    {
        [SerializeField]
        private RectTransform BoostersParent;
        [SerializeField]
        private GameObject PauseButton;

        #region temp vars
        private GameBoard MBoard => GameBoard.Instance;
        private  GuiController MGui => GuiController.Instance; 
        private GameConstructSet GCSet => GameConstructSet.Instance; 
        private GameObjectsSet GOSet =>(GCSet) ? GCSet.GOSet : null;
        #endregion temp vars

        public static FooterGUIController Instance;

        #region regular
        void Awake()
        {
            if (Instance) Destroy(Instance.gameObject);
            Instance = this;
        }

        private IEnumerator Start()
        {
            while (MBoard == null) yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();

            if (GameBoard.GMode == GameMode.Edit)
            {
                gameObject.SetActive(false);
            }
            else
            {
                //set booster events
                foreach (var item in GOSet.BoosterObjects)
                {
                    item.ChangeUseEvent += ChangeBoosterUseEventHandler;
                }
                CreateBoostersPanel();
            }

            if (MBoard.WinContr != null && MBoard.WinContr.IsTimeLevel && PauseButton) PauseButton.SetActive(false);
        }

        private void OnDestroy()
        {
            // remove boostar events
            if (GOSet && GOSet.BoosterObjects != null)
                foreach (var item in GOSet.BoosterObjects)
                {
                    item.ChangeUseEvent -= ChangeBoosterUseEventHandler;
                }
        }
        #endregion regular

        private void CreateBoostersPanel()
        {
            GuiBoosterHelper[] ms = BoostersParent.GetComponentsInChildren<GuiBoosterHelper>();
            foreach (GuiBoosterHelper item in ms)
            {
                 DestroyImmediate(item.gameObject);
            }

            foreach (var item in GOSet.BoosterObjects)
            {
              if(item.Use)  item.CreateActivateHelper(BoostersParent);
            }
        }

        private void ChangeBoosterUseEventHandler(Booster booster)
        {
            if (booster.Use)
            {
               booster.CreateActivateHelper(BoostersParent);
            }
            else
            {
                //destroy not used booster helpers
                GuiBoosterHelper[] bHs = BoostersParent.GetComponentsInChildren<GuiBoosterHelper>();
                foreach (GuiBoosterHelper item in bHs)
                {
                   if(item.booster == booster)  DestroyImmediate(item.gameObject);
                }
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

        public void Map_Click()
        {
            if (Time.timeScale == 0) return;
            if (GameBoard.GMode == GameMode.Play)
            {
                if (MGui) MGui.ShowPopUpByDescription("quit");
            }
        }

        public void Pause_Click()
        {
            if (MGui)
            {
                PopUpsController puPrefab = MGui.GetPopUpPrefabByDescription("pause");
                if(puPrefab) MGui.ShowPopUp(puPrefab, () => { if (MBoard) MBoard.Pause(); }, null);
            }
        }
    }
}
