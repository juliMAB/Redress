using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public float slowdownFactor = 0.05f;
    public float slowdownLength = 2f;
    //private void Update()
    //{
    //    Time.timeScale += (1f / slowdownLength) * Time.unscaledDeltaTime;
    //    Time.timeScale = Mathf.Clamp(Time.timeScale, 0f, 1f);
    //}
    public void DoSlowMontion()
    {
        Time.timeScale = slowdownFactor;
        Time.fixedDeltaTime = Time.deltaTime * .02f;
    }

    public IEnumerator timeSlow(float duration)
    {
        
        float elapsed = 0.0f;

        Time.timeScale = slowdownFactor;
        Time.fixedDeltaTime = Time.deltaTime * .02f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }
        Time.timeScale = 1;
    }
}
