using UnityEngine;
using UnityEngine.UI;

namespace Mkey
{
    public class AlmostWindowController : PopUpsController
    {
        [SerializeField]
        private Text coinsText;
        private GameBoard MBoard => GameBoard.Instance;

        private int  Coins{ get; set; }

        public void SetCoins(int coins)
        {
            Coins = coins;
            if (coinsText) coinsText.text = Coins.ToString();
        }

        public void Close_Click()
        {
            CloseWindow();
            MBoard.showAlmostMessage = false;
            MBoard.WinContr.CheckResult();
        }

        public void Play_Click()
        {
            CloseWindow();
            CoinsHolder.Add(-Coins);
            MBoard.WinContr.AddMoves(5);
        }
    }
}
