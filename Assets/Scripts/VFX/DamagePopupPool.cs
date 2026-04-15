using System.Collections.Generic;
using UnityEngine;

public class DamagePopupPool : MonoBehaviour
{
    public static DamagePopupPool Instance;

    [Header("DamagePopup Prefab (World Space TMP)")]
    public DamagePopup popupPrefab;
    public int poolSize = 20;
    //public Vector2 PrefabOffset = Vector2.zero;
    private Queue<DamagePopup> pool = new Queue<DamagePopup>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        // 初始化对象池
        for (int i = 0; i < poolSize; i++)
        {
            DamagePopup popup = Instantiate(popupPrefab, transform);
            popup.gameObject.SetActive(false);
            pool.Enqueue(popup);
        }
    }

    public DamagePopup Spawn(Vector3 worldPos, int damage, ElementType element, bool isCrit)
    {
        DamagePopup popup;
        if (pool.Count > 0)
        {
            popup = pool.Dequeue();
        }
        else
        {
            popup = Instantiate(popupPrefab, transform);
        }

        popup.gameObject.SetActive(true);
        popup.Play(worldPos, damage, element, isCrit);

        // 自动回收
        StartCoroutine(ReturnToPoolNextFrame(popup));

        return popup;
    }

    private System.Collections.IEnumerator ReturnToPoolNextFrame(DamagePopup popup)
    {
        // 等待动画完成后再回收
        yield return new WaitForSeconds(popup.duration + 0.5f);
        popup.gameObject.SetActive(false);
        pool.Enqueue(popup);
    }
}
