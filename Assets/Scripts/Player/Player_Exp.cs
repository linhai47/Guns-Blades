using System;
using Language.Lua;
using UnityEngine;

public class Player_Exp : MonoBehaviour , ISaveable
{
    [Header("玩家等级")]
    public int playerLevel = 1;

    [Header("经验")]
    public float exp;
    public float nextLevelNeedExp;
    public float baseExp = 100f;
    public float growthRate = 1.5f;
    public event Action OnExpChange;

    private void Start()
    {
        OnExpChange?.Invoke();
    }

    public void AddExp(float addExp)
    {

        exp += addExp;
        OnExpChange?.Invoke();
        LevelUp();
    }

    private void LevelUp()
    {
        while (exp >= nextLevelNeedExp)
        {
            exp -= nextLevelNeedExp;
            playerLevel = playerLevel + 1;
            SetNextLevelNeedExp();
            OnExpChange?.Invoke();
        }
    }
    public float getExpPercent()
    {
        return exp / nextLevelNeedExp;
    }
    public void SetNextLevelNeedExp()
    {
        nextLevelNeedExp = baseExp * Mathf.Pow(growthRate, playerLevel - 1);


    }

    public void LoadData(GameData data)
    {
        exp = data.exp;
        playerLevel = data.playerLevel;
        nextLevelNeedExp = data.nextLevelNeedExp;
        OnExpChange?.Invoke();
    }

    public void SaveData(ref GameData data)
    {

        data.nextLevelNeedExp = nextLevelNeedExp;   
        data.playerLevel = playerLevel;
        data.exp = exp;
    }

   
}
