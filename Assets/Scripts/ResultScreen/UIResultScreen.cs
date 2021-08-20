using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using EndlessT4cos.Gameplay.User;
using EndlessT4cos.Management;

namespace EndlessT4cos.ResultScreen
{
    public class UIResultScreen : MonoBehaviour
    {
        [SerializeField] private Text info = null;

        private string FormatDistance(float _distance)
        {
            string text; 

            if (_distance < 100)
            {
                int dis = (int)_distance;

                text = "00.";

                if (dis < 10)
                {
                    text += "0";
                }

                text += dis.ToString();
            }
            else
            {
                int num1 = (int)_distance / 1000;
                int num2 = (int)(_distance - num1 * 1000) / 100;
                int num3 = (int)(_distance - num1 * 1000 - num2 * 100) / 10;
                int num4 = (int)(_distance - num1 * 1000 - num2 * 100 - num3 * 10);

                text = num1.ToString() + num2.ToString() + "." + num3.ToString() + num4.ToString();
            }

            text += "km";

            return text;
        }

        void Start()
        {
            PlayerData playerData = GameManager.Instance.PlayerData;

            info.text = "Points\n" + playerData.Score +
                        "\nMax velocity\n" + playerData.Velocity +
                        "\nTraveled distance\n" + FormatDistance(playerData.TraveledDitance);
        }
    }
}

