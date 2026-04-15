using System.Collections;
using UnityEngine;

public class UI_CraftAvatar : MonoBehaviour
{
    [SerializeField] private Animator anim;
    [SerializeField] private ItemType type;



    private Coroutine currentAnimCoroutine;
    private void Awake()
    {

    }

    //private void Start()
    //{
    //    ChangeAnim();
    //}
    public void ChangeType(ItemType type)
    {
        this.type = type;
    }

    public void ChangeAnim()
    {
        if (currentAnimCoroutine != null)
        {
            StopCoroutine(currentAnimCoroutine);
            currentAnimCoroutine = null;
        }


        currentAnimCoroutine = StartCoroutine(TriggerAnimationCoroutine());
    }
    private IEnumerator TriggerAnimationCoroutine()
    {
        // 开始动画标记
        anim.SetBool("animOver", true);

        // 重置所有 Trigger，避免冲突
        foreach (ItemType type in System.Enum.GetValues(typeof(ItemType)))
        {
            anim.ResetTrigger(type.ToString());
        }

        // 等待一帧确保 Trigger 被重置
        yield return null;

        // 设置目标 Trigger 播放动画
  
        anim.SetTrigger(type.ToString());

        // 动画播放完成后重置标记
        anim.SetBool("animOver", false);

        // 协程结束后清空引用
        currentAnimCoroutine = null;
    }

    public void StopAnim()
    {
        if (currentAnimCoroutine != null)
        {
            StopCoroutine(currentAnimCoroutine);
            currentAnimCoroutine = null;
            anim.SetBool("animOver", false); // 重置动画状态
        }
    }
}
