using UnityEngine;
using UnityEngine.Playables;
using Unity.Cinemachine;

public class TimelineCameraFixer : MonoBehaviour
{
    public PlayableDirector timeline;
    public CinemachineCamera lastVCam;

    void OnEnable()
    {
        timeline.stopped += OnTimelineStopped;
    }

    void OnDisable()
    {
        timeline.stopped -= OnTimelineStopped;
    }

     public void OnTimelineStopped(PlayableDirector pd)
    {
        // 获取主相机
        Camera mainCam = Camera.main;

        if (lastVCam != null && mainCam != null)
        {
            // 将主相机同步到最后虚拟镜头的位置和旋转
            mainCam.transform.position = lastVCam.transform.position;
            mainCam.transform.rotation = lastVCam.transform.rotation;
        }
    }
}
