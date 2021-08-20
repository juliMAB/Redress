using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using EndlessT4cos.Management;

namespace EndlessT4cos.Menu.UI
{
    public class UIMenuMananger : MonoBehaviour
    {
        [SerializeField] private GameObject menu = null;
        [SerializeField] private GameObject options = null;
        [SerializeField] private GameObject credits = null;

        [SerializeField] private GameObject[] creditsPanels = null;
        [SerializeField] private GameObject[] creditsPanelsButtons = null;

        private enum Menu { Main, Options, Credits}
        private enum CreditsPanel { Names, Assets}
        
        public void GoToScene(int scene)
        {
            GameManager.Instance.GoToScene((GameManager.Scene)scene);
        }

        public void SwitchToCreditsPanel(int panel)
        {
            CreditsPanel creditsPanel = (CreditsPanel)panel;

            creditsPanelsButtons[0].SetActive(false);
            creditsPanelsButtons[1].SetActive(false);
            creditsPanels[0].SetActive(false);
            creditsPanels[1].SetActive(false);

            switch (creditsPanel)
            {
                case CreditsPanel.Names:
                    creditsPanelsButtons[1].SetActive(true);
                    break;
                case CreditsPanel.Assets:
                    creditsPanelsButtons[0].SetActive(true);                    
                    break;
                default:
                    break;
            }

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

        public void ExitGame()
        {
            GameManager.Instance.CloseGame();
        }
    }
}
