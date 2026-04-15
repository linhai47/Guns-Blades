using UnityEngine;

public class BarkBillboard : MonoBehaviour
{
    void LateUpdate()
    {
        if (Camera.main != null)
        {
            // 保持朝向相机，防止文字翻转
            transform.forward = Camera.main.transform.forward;
        }
    }
}