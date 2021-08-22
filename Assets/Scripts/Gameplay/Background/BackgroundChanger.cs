using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace EndlessT4cos.Gameplay.Background
{
    public class BackgroundChanger : MonoBehaviour
    {
        [SerializeField] Sprite[] sprite2 = null;
        [SerializeField] BackgroundsManager backgroundsManager;
        
        public void updateSprite(int value)
        {
            if (value >= sprite2.Length)
            {
                value = sprite2.Length - 1;
            }
            foreach (var item in backgroundsManager.Objects)
            {
                StartCoroutine(activateWhenNotActive(item, sprite2[value]));
            }
        }
        private IEnumerator activateWhenNotActive(GameObject go, Sprite sprite)
        {
            while (go.activeSelf)
            {
                yield return null;
            }
            go.GetComponent<SpriteRenderer>().sprite = sprite;
        }
    }
}
