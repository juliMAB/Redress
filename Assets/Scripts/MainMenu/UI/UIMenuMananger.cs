using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Redress.Management;

namespace Redress.Menu.UI
{
    public class UIMenuMananger : MonoBehaviour
    {
        private bool panel1LighstActivated = true;

        [Header("Entities")]
        [SerializeField] private GameObject menu = null;
        [SerializeField] private GameObject options = null;
        [SerializeField] private GameObject credits = null;
        [SerializeField] private GameObject[] creditsPanels = null;
        //[SerializeField] private GameObject[] creditsPanelsButtons = null;
        [SerializeField] private Color buttonColor = Color.cyan;
        [SerializeField] private GameObject[] lightsPanel1 = null;
        [SerializeField] private GameObject[] lightsPanel2 = null;

        [Header("UI Animation Configuration")]
        [SerializeField] private float changeEffectTime = 0.5f;
        [SerializeField] private float time = 0f;

        private enum Menu { Main, Options, Credits}
        private enum CreditsPanel { Names, Assets}

        private void Update()
        {
            time += Time.deltaTime;

            if (time > changeEffectTime)
            {
                ChangeLights();

                time = 0f;
            }
        }

        private void ChangeLights()
        {
            panel1LighstActivated = !panel1LighstActivated;

            for (int i = 0; i < lightsPanel1.Length; i++)
            {
                lightsPanel1[i].SetActive(panel1LighstActivated);
            }

            for (int i = 0; i < lightsPanel2.Length; i++)
            {
                lightsPanel2[i].SetActive(!panel1LighstActivated);
            }
        }

        public void GoToScene(int scene)
        {
            GameManager.Instance.GoToScene((GameManager.Scene)scene);
        }

        public void SwitchToCreditsPanel(int panel)
        {
            CreditsPanel creditsPanel = (CreditsPanel)panel;

            //creditsPanelsButtons[0].SetActive(false);
            //creditsPanelsButtons[1].SetActive(false);
            //creditsPanels[0].SetActive(false);
            //creditsPanels[1].SetActive(false);

           // switch (creditsPanel)
           // {
           //     case CreditsPanel.Names:
           //         creditsPanelsButtons[1].SetActive(true);
           //         break;
           //     case CreditsPanel.Assets:
           //         creditsPanelsButtons[0].SetActive(true);                    
           //         break;
           //     default:
           //         break;
           // }

            creditsPanels[panel].SetActive(true);
        }

        public void SwitchToPanel(int panel)
        {
            Menu menuPanel = (Menu)panel;

            menu.SetActive(false);
            credits.SetActive(false);
            options.SetActive(false);

            switch (menuPanel)
            {
                case Menu.Main:
                    menu.SetActive(true);
                    break;
                case Menu.Options:
                    options.SetActive(true);
                    break;
                case Menu.Credits:
                    credits.SetActive(true);
                    SwitchToCreditsPanel(0);
                    break;
                default:
                    break;
            }
        }

        public void SwitchButtonColorActive(GameObject button, bool active)
        {
            if (active)
            {
                button.GetComponent<Text>().color = buttonColor;
            }
            else
            {
                button.GetComponent<Text>().color = Color.white;
            }
        }

        public void ExitGame()
        {
            GameManager.Instance.CloseGame();
        }
    }
}
