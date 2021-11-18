using UnityEngine;
using UnityEngine.UI;

namespace Redress.Gameplay.Data
{
    public class HighscoreManager : MonoBehaviour
    {
        [SerializeField] private Text text;

        private int highscore = 0;

        public int Highscore { get => highscore; set => highscore = value; }

        private void Start()
        {
            highscore = PlayerPrefs.GetInt("highscore", highscore);
            text.text = highscore.ToString();
        }

        public void SetScore(int score)
        {
            if (score > highscore)
            {
                highscore = score;
                text.text = score.ToString();

                PlayerPrefs.SetInt("highscore", highscore);
            }
        }
    }
}