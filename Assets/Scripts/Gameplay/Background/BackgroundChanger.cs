using System.Collections;
using UnityEngine;

namespace EndlessT4cos.Gameplay.Background
{
    public class BackgroundChanger : MonoBehaviour
    {
        [SerializeField] private Sprite[] sprite2 = null;
        [SerializeField] private BackgroundsManager backgroundsManager;
        private bool isAlreadyRunning = false;

        public void UpdateSprite(int value)
        {
            if (isAlreadyRunning)
            {
                return;
            }

            if (value >= sprite2.Length)
            {
                value = sprite2.Length - 1;
            }

            for (int i = 0; i < backgroundsManager.Objects.Length; i++)
            {
                StartCoroutine(ActivateWhenNotActive(backgroundsManager.Objects[i], sprite2[value], i == backgroundsManager.Objects.Length - 1));
            }

            isAlreadyRunning = true;
        }

        private IEnumerator ActivateWhenNotActive(GameObject go, Sprite sprite, bool isLast)
        {
            while (go.activeSelf)
            {
                yield return null;
            }

            go.GetComponent<SpriteRenderer>().sprite = sprite;

            if (isLast)
            {
                isAlreadyRunning = false;
            }
        }
    }
}
