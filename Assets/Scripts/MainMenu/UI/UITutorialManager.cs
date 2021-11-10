using UnityEngine;

namespace Redress.Menu.UI
{
    public class UITutorialManager : MonoBehaviour
    {
        [SerializeField] private GameObject[] tutorialScreens = null;
        [SerializeField] private GameObject nextButton = null;
        [SerializeField] private GameObject lastButton = null;
        [SerializeField] private GameObject skipTutorialButton = null;

        private int enabledPanelIndex = 0;

        public void ActivateNextPanel()
        {
            if (enabledPanelIndex < tutorialScreens.Length - 1)
            {
                tutorialScreens[enabledPanelIndex].SetActive(false);

                enabledPanelIndex++;

                tutorialScreens[enabledPanelIndex].SetActive(true);
            }

            UpdateButtons();
        }

        public void ActivateLastPanel()
        {
            if (enabledPanelIndex > 0)
            {
                tutorialScreens[enabledPanelIndex].SetActive(false);

                enabledPanelIndex--;

                tutorialScreens[enabledPanelIndex].SetActive(true);
            }

            UpdateButtons();
        }

        private void UpdateButtons()
        {
            nextButton.SetActive(enabledPanelIndex != tutorialScreens.Length - 1);
            skipTutorialButton.SetActive(enabledPanelIndex != tutorialScreens.Length - 1);
            lastButton.SetActive(enabledPanelIndex != 0);
        }
    }
}