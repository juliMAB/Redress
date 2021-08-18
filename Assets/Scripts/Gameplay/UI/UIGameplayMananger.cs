using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
namespace EndlessT4cos.Gameplay.UI
{
    public class UIGameplayMananger : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI scoreText;

        public void ScoreUpdate(int value)
        {
            scoreText.text = "Score: ";
            scoreText.text += value.ToString();
        }
    }
}

