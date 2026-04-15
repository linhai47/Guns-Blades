using UnityEngine;

public class RoomGate : MonoBehaviour
{
    [Header("Gate as a blocker object")]
    public GameObject gateObject; // 要显示/隐藏的门模型或碰撞体
    public bool startClosed = true;

    [Header("Optional: slide settings")]
    public bool useSlide = false;
    public Vector3 closedLocalPos;
    public Vector3 openLocalPos;
    public float slideSpeed = 4f;

    bool isOpen = false;

    private void Start()
    {
        if (gateObject == null) gateObject = this.gameObject;
        if (useSlide)
        {
            gateObject.transform.localPosition = startClosed ? closedLocalPos : openLocalPos;
            isOpen = !startClosed;
            if (startClosed) Close(); else Open();
        }
        else
        {
            gateObject.SetActive(!startClosed ? false : true); // active when closed (blocks)
            isOpen = !startClosed;
        }
    }

    public void Open()
    {
        if (useSlide)
        {
            StopAllCoroutines();
            StartCoroutine(SlideTo(openLocalPos));
        }
        else
        {
            // 关闭阻挡（隐藏/禁用碰撞）
            gateObject.SetActive(false);
        }
        isOpen = true;
    }

    public void Close()
    {
        if (useSlide)
        {
            StopAllCoroutines();
            StartCoroutine(SlideTo(closedLocalPos));
        }
        else
        {
            gateObject.SetActive(true);
        }
        isOpen = false;
    }

    System.Collections.IEnumerator SlideTo(Vector3 targetLocal)
    {
        while (Vector3.Distance(gateObject.transform.localPosition, targetLocal) > 0.01f)
        {
            gateObject.transform.localPosition = Vector3.MoveTowards(gateObject.transform.localPosition, targetLocal, slideSpeed * Time.deltaTime);
            yield return null;
        }
        gateObject.transform.localPosition = targetLocal;
    }
}
