using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager instance;

    private FileDataHandler dataHandler;

    private GameData gameData;
    private List<ISaveable> allsaveables;

    [SerializeField] private string fileName = "tothesky.json";
    [SerializeField] private bool encryptData = true;

    private void Awake()
    {
        instance = this;
    }
    private IEnumerator Start()
    {

        Debug.Log(Application.persistentDataPath);
        dataHandler = new FileDataHandler(Application.persistentDataPath, fileName, encryptData);
        allsaveables = FindIsaveables();

        yield return null;
        LoadGame();
    }

    private void LoadGame()
    {
        gameData = dataHandler.LoadData();
        if (gameData == null)
        {
            Debug.Log("No save data found ,creating new save");
            gameData = new GameData();
            return;
        }

        foreach (var saveable in allsaveables)
        {
            saveable.LoadData(gameData);
        }
    }
    public void SaveGame()
    {
        foreach (var saveable in allsaveables)
        {
            saveable.SaveData(ref gameData);
        }

        dataHandler.SaveData(gameData);
    }

    public GameData GetGameData() => gameData;

    [ContextMenu("*** Delete save data***")]
    public void DeleteSaveData()
    {
        dataHandler = new FileDataHandler(Application.persistentDataPath, fileName, encryptData);

        dataHandler.Delete();

        LoadGame();

    }
    private void OnApplicationQuit()
    {
        SaveGame();
    }


    private List<ISaveable> FindIsaveables()
    {
        return FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None).OfType<ISaveable>()
            .ToList();

    }
}
