using UnityEngine;
using UnityEngine.UI;

namespace Mkey
{
	public class ModeButton : MonoBehaviour
	{
        [SerializeField]
        private Button modeButton;
        [SerializeField]
        private Text modeText;
        #region temp vars

        #endregion temp vars


        #region regular
		
		private void Start()
		{
#if UNITY_EDITOR
            if (modeButton)
            {
                modeButton.gameObject.SetActive(true);
                if(modeText)  modeText.text = (GameBoard.GMode == GameMode.Edit) ? "GoTo" + System.Environment.NewLine + "PLAY" : "GoTo" + System.Environment.NewLine + "EDIT";
                modeButton.onClick.AddListener(() =>
                {
                    if (GameBoard.GMode == GameMode.Edit)
                    {
                        GameBoard.GMode = GameMode.Play;
                        if (modeText) modeText.text = "GoTo" + System.Environment.NewLine + "EDIT";
                    }
                    else
                    {
                        SimpleTween.ForceCancelAll();
                        GameBoard.GMode = GameMode.Edit;
                        if (modeText) modeText.text = "GoTo" + System.Environment.NewLine + "PLAY";
                    }
                    SceneLoader.Instance.ReLoadCurrentScene();
                });
            }
#else
           if (modeButton) modeButton.gameObject.SetActive(false); 
#endif
        }

		#endregion regular
	}
}
