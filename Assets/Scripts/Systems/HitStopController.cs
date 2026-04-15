// HitStopController.cs
using UnityEngine;
using System.Collections;

public class HitStopController : MonoBehaviour
{
    public static HitStopController Instance;

    private void Awake()
    {
        Instance = this;
    }

    public void StopTime(float duration, float slowScale = 0.0f)
    {
        //Debug.Log("Stop Time ");
        //StartCoroutine(DoHitStop(duration, slowScale));
    }

    private IEnumerator DoHitStop(float duration, float slowScale)
    {
        Time.timeScale = slowScale;
        
        yield return new WaitForSecondsRealtime(duration);
        Time.timeScale = 1f;
    }
}
