using Unity.Cinemachine;
using UnityEngine;

public class DoorCameraBoundary : MonoBehaviour
{
    [Header("边界")]


    public CinemachineConfiner2D confiner; // 虚拟摄像机上的 Confiner
    public PolygonCollider2D boundaryCollider;
    public Transform ParentOffset;
    public Transform door;
    public GameObject doorGate;
    public GameObject preDoorGate;
    public float openOffsetX = 3f;               // 门打开后右边顶点偏移量

    public Vector2[] originalPoints;            // 原始顶点
    public UI_Boss uiBoss;
    public bool ifTriggerBossFight;
    private void Start()
    {
        // 获取 Collider 当前的顶点
        originalPoints = boundaryCollider.GetPath(0);
        uiBoss = FindFirstObjectByType<UI_Boss>(FindObjectsInactive.Include);
    }

    // 假设玩家进入触发点调用
    public void OnTriggerDoorOpen()
    {
        Debug.Log("Open Trigger");
        Vector2[] newPoints = new Vector2[originalPoints.Length];

        for (int i = 0; i < originalPoints.Length; i++)
        {
            Vector2 point = originalPoints[i];

            Debug.Log(point.x);
            Debug.Log(door.position.x);
            if (point.x > door.position.x - ParentOffset.position.x)
                point.x += openOffsetX;


            newPoints[i] = point;

        }
        originalPoints = newPoints;
        // 更新 Collider 顶点，Confiner 会自动应用
        boundaryCollider.SetPath(0, originalPoints);
        confiner.InvalidateBoundingShapeCache();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {

            doorGate.SetActive(true);
            if (preDoorGate != null) preDoorGate.SetActive(true);
            if (ifTriggerBossFight)
            {
                uiBoss.gameObject.SetActive(true);
            }
        }
    }

}
