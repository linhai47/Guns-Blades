using UnityEngine;

public class Object_ArielStatue : MonoBehaviour
{
    [Header("存档点粒子特效")]
    public ParticleSystem activateEffect;

    private void Awake()
    {
       activateEffect = GetComponentInChildren<ParticleSystem>();   
    }

    [Header("是否已经激活")]
    public bool isActivated = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 检查是不是玩家触发的
        if (other.CompareTag("Player") && !isActivated)
        {
            Debug.Log("触发存档点：" + gameObject.name);

            ActivateStatue();
        }
    }

    private void ActivateStatue()
    {
        isActivated = true;

        // 播放粒子特效
        if (activateEffect != null)
        {
            activateEffect.Play();
        }

       



        Debug.Log("存档点已激活：" + gameObject.name);
    }
}
