using PixelCrushers.DialogueSystem.PlayMaker;
using UnityEngine;
using UnityEngine.Playables;

[RequireComponent(typeof(Collider2D))] // 2D 场景用 Collider2D
public class TimelineTrigger : MonoBehaviour
{
    public PlayableDirector timeline;      // 需要播放的 Timeline
    public bool playOnEnter = true;        // 是否进入触发自动播放
    public bool onlyOnce = true;           // 是否只触发一次

    private bool triggered = false;

    private void Reset()
    {
        // 确保 Collider2D 是 Trigger
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            col.isTrigger = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {

        if (onlyOnce && triggered) return;

        if (other.CompareTag("Player"))
        {
            triggered = true;

            if (playOnEnter && timeline != null)
            {
                timeline.Play();
            }
        }
    }

    // 可选：手动触发
    public void PlayTimeline()
    {
        if (timeline != null)
        {
            timeline.Play();
        }
    }
}
