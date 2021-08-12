using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VerticalPlataform : MonoBehaviour
{
    private PlatformEffector2D effector;
    [SerializeField]float waitTime;
    private void Start()
    {
        effector = GetComponent<PlatformEffector2D>();
    }
    private void Update()
    {
        if (Input.GetButtonUp("Crouch"))
        {
            waitTime = 0.5f;
        }
        if (Input.GetButton("Crouch"))
        {
            if (waitTime<=0)
            {
                effector.rotationalOffset = 180f;
                waitTime = 0.5f;
            }
            else
            {
                waitTime -= Time.deltaTime;
            }
        }

        if (Input.GetButton("Jump"))
        {
            effector.rotationalOffset = 0;
        }
    }
}
