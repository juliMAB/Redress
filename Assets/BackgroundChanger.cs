using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EndlessT4cos.Gameplay.Background;

public class BackgroundChanger : MonoBehaviour
{
    [SerializeField] Sprite[] sprite2 = null;
    [SerializeField] Sprite actualSprite = null;
    [SerializeField] BackgroundsManager backgroundsManager;

    public void updateSprite(int value)
    {
        if (value>=sprite2.Length)
        {
            value = sprite2.Length - 1;
        }
        Debug.Log("LLEGA AL CHANGER");
        foreach (var item in backgroundsManager.Objects)
        {
             StartCoroutine(activateWhenNotActive(item, sprite2[value]));
        }
        //actualSprite = sprite2[value];
    }
    private IEnumerator activateWhenNotActive(GameObject go,Sprite sprite)
    {
        if (!go.activeSelf)
        {
            go.GetComponent<SpriteRenderer>().sprite = sprite;
            yield return null;
        }
    }
}
