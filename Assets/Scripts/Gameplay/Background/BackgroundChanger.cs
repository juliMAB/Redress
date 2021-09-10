using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EndlessT4cos.Gameplay.Background
{
    
    public class BackgroundChanger : MonoBehaviour
    {
        [System.Serializable]
        public class LvlSprite
        {
            public Sprite[] sp;
        };
        public LvlSprite[] lvlSprites;
        [SerializeField] private BackgroundsManager backgroundsManager;
        public void UpdateSprite(int lvl)
        {
            foreach (var item in backgroundsManager.Objects)
            {
                //les tiro las corrutins para actualizar el fondo.
                if (!item.activeSelf)
                {
                    StartCoroutine(ActivateWhenNotActiveFirst(item, lvlSprites[lvl].sp[0], lvlSprites[lvl].sp[1]));
                }
            }
            foreach (var item in backgroundsManager.Objects)
            {
                //les tiro las corrutins para actualizar el fondo.
                if (item.activeSelf)
                {
                    StartCoroutine(ActivateWhenNotActive(item, lvlSprites[lvl].sp[1]));
                }
            }

        }

        //public void UpdateSprite(int value)
        //{
        //    if (isAlreadyRunning)
        //    {
        //        return;
        //    }

        //    if (value >= sprite2.Length)
        //    {
        //        value = sprite2.Length - 1;
        //    }

        //    for (int i = 0; i < backgroundsManager.Objects.Length; i++)
        //    {
        //        StartCoroutine(ActivateWhenNotActive(backgroundsManager.Objects[i], sprite2[value], i == backgroundsManager.Objects.Length - 1));
        //    }

        //    isAlreadyRunning = true;
        //}
        private IEnumerator ActivateWhenNotActive(GameObject go, Sprite sprite)
        {
            while (go.activeSelf)
            {
                yield return null;
            }
            go.GetComponent<SpriteRenderer>().sprite = sprite;
            Debug.Log("Se cargo el 2 sprite");
        }
        private IEnumerator ActivateWhenNotActiveFirst(GameObject go, Sprite sprite,Sprite sprite2)
        {
            while (go.activeSelf)
            {
                yield return null;
            }
            go.GetComponent<SpriteRenderer>().sprite = sprite;
            Debug.Log("Se cargo el sprite changer");
            while (!go.activeSelf)
            {
                yield return null;
            }
            StartCoroutine(ActivateWhenNotActive(go, sprite2));
        }
        //private IEnumerator ActivateWhenNotActive(GameObject go, Sprite sprite, bool isLast)
        //{
        //    while (go.activeSelf)
        //    {
        //        yield return null;
        //    }

        //    go.GetComponent<SpriteRenderer>().sprite = sprite;

        //    if (isLast)
        //    {
        //        isAlreadyRunning = false;
        //    }
        //}
    }
}
