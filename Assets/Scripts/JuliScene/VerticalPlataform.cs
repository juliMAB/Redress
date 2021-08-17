using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VerticalPlataform : MonoBehaviour
{
    private Collider2D col;
    private void Start()
    {
        col = GetComponent<Collider2D>();
    }
    private void Update()
    {
        if (Input.GetButton("Crouch"))
        {
            col.isTrigger = true;
        }
        else
        {
            col.isTrigger = false;
        }
    }
}
