using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour,ISaveable
{
    public static GameManager instance;
    private Vector3 lastPlayerPosition;

    private string lastScenePlayed;
    private bool dataLoaded;
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }
    public void ContinuePlay()
    {
        ChangeScene(lastScenePlayed, RespawnType.NonSpecific);
    }

    public void RestartScene()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        ChangeScene(sceneName, RespawnType.NonSpecific);
    }


    public void ChangeScene(string sceneName, RespawnType respwanType)
    {
        SaveManager.instance.SaveGame();

        Time.timeScale = 1;
        StartCoroutine(ChangeSceneCo(sceneName, respwanType));
    }
    private IEnumerator ChangeSceneCo(string sceneName, RespawnType respawnType)
    {
        // 1. 获取当前场景的 FadeScreen

        if (UI_FadeScreen.instance != null)
            yield return StartCoroutine(UI_FadeScreen.instance.FadeOutCoroutine());
        else
        {
            Debug.LogWarning("No UI_FadeScreen found before scene load!");
        }

        // 2. 切换场景
        SceneManager.LoadScene(sceneName);

        // 3. 重置数据加载标志
        dataLoaded = false;
        yield return null;
        // 4. 加载存档（如果存在）

        while (dataLoaded == false)
        {
            Debug.Log("data Loading");
            yield return null;
        }
        // 5. 如果是主菜单，不需要处理 Player
        if (SceneManager.GetActiveScene().name == "MainMenu")
            yield break;

        // 6. 获取 Player 实例
        Player player = Player.instance;
        if (player == null)
        {
            Debug.LogWarning("Player instance not found after scene load!");
            yield break;
        }

        // 7. 获取玩家新的位置
        Vector3 position = GetNewPlayerPosition(respawnType);
        if (position != Vector3.zero)
        {
            Debug.Log("TP");
            player.TeleportPlayer(position);
        }
        yield return null;
        // 8. 获取新场景的 FadeScreen
        // 等待淡入
        if (UI_FadeScreen.instance != null)
            yield return StartCoroutine(UI_FadeScreen.instance.FadeInCoroutine());
        else
        {
            Debug.LogWarning("No UI_FadeScreen found after scene load!");
        }
  
    
       
    }

    private UI_FadeScreen FindFadeScreenUI()
    {
        if (UI.instance != null)
            return UI.instance.fadeScreenUI;
        else
            return FindFirstObjectByType<UI_FadeScreen>();
    }
    private Vector3 GetNewPlayerPosition(RespawnType type)
    {
        if (type == RespawnType.Portal)
        {
            Object_Portal portal = Object_Portal.instance;

            Vector3 position = portal.GetPosition();

            portal.SetTrigger(false);
            portal.DisableIfNeeded();

            return position;
        }


        if (type == RespawnType.NonSpecific)
        {
            var data = SaveManager.instance.GetGameData();
            var checkpoints = FindObjectsByType<Object_Checkpoint>(FindObjectsSortMode.None);
            var unlockedCheckpoints = checkpoints
                .Where(cp => data.unlockedCheckpoints.TryGetValue(cp.GetCheckpointId(), out bool unlocked) && unlocked)
                .Select(cp => cp.GetPosition())
                .ToList();

            var enterWaypoints = FindObjectsByType<Object_Waypoint>(FindObjectsSortMode.None)
                .Where(wp => wp.GetWaypointType() == RespawnType.Enter)
                .Select(wp => wp.GetPositionAndSetTriggerFalse())
                .ToList();

            var selectedPositions = unlockedCheckpoints.Concat(enterWaypoints).ToList(); // combine two lists into one

            if (selectedPositions.Count == 0)
                return Vector3.zero;

            return selectedPositions.
                OrderBy(position => Vector3.Distance(position, lastPlayerPosition)) // arrange form lowest to highest by comparing distance
                .First();
        }

        return GetWaypointPosition(type);
    }
    private Vector3 GetWaypointPosition(RespawnType type)
    {
        var waypoints = FindObjectsByType<Object_Waypoint>(FindObjectsSortMode.None);

        foreach (var point in waypoints)
        {
            if (point.GetWaypointType() == type)
                return point.GetPositionAndSetTriggerFalse();
        }

        return Vector3.zero;
    }

    public void LoadData(GameData data)
    {

        lastScenePlayed = data.lastScenePlayed;
        lastPlayerPosition = data.lastPlayerPosition;

        if (string.IsNullOrEmpty(lastScenePlayed))
            lastScenePlayed = "Level_0";
        Debug.Log("LoadData");
        dataLoaded = true;
    }

    public void SaveData(ref GameData data)
    {
        string currentScene = SceneManager.GetActiveScene().name;

        if (currentScene == "MainMenu")
            return;

        data.lastPlayerPosition = Player.instance.transform.position;
        data.lastScenePlayed = currentScene;
        dataLoaded = false;
    }
}
