using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class Magia1 : MonoBehaviour
{
    [SerializeField] Button ejemplo;

    Button[] todos;

    private void OnEnable()
    {
        todos = FindObjectsOfType<Button>();

        foreach (var item in todos)
        {
            item.colors = ejemplo.colors;
            item.GetComponent<Image>().color = ejemplo.GetComponent<Image>().color;
        }
        
    }
}
