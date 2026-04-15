using System.Collections.Generic;
using UnityEngine;

public class GameData 
{
    public int gold;

    public List<Inventory_Item> itemList;
    public SerializableDictionary<string, int> inventory; // itemSaveId -> stackSize
    public SerializableDictionary<string, int> storageItems;
    public SerializableDictionary<string, int> storageMaterials;

    public SerializableDictionary<string, ItemType> equipedItems; // itemSaveId -> slotType;

    public int skillPoints;
    public SerializableDictionary<string, bool> skillTreeUI; // skill name -> unlock status
    public SerializableDictionary<string, SkillType> playerSkills;
    //public SerializableDictionary<SkillType, SkillUpgradeType> skillUpgrades; // skill type -> upgrade type

    public SerializableDictionary<string, bool> unlockedCheckpoints;
    public SerializableDictionary<string, Vector3> inScenePortals;

    public SerializableDictionary<string, bool> completedQuests;
    public SerializableDictionary<string, int> activeQuests;

    public int chapterIndex;

    public string portalDestinationSceneName;
    public bool returningFromTown;

    public string lastScenePlayed;
    public Vector3 lastPlayerPosition;

    public int playerLevel = 1;
    public float exp =0;
    public float nextLevelNeedExp;
    public GameData()
    {
        inventory = new SerializableDictionary<string, int>();
        storageItems = new SerializableDictionary<string, int>();
        storageMaterials = new SerializableDictionary<string, int>();

        equipedItems = new SerializableDictionary<string, ItemType>();

        skillTreeUI = new SerializableDictionary<string, bool>();
        playerSkills = new SerializableDictionary<string, SkillType>();


        unlockedCheckpoints = new SerializableDictionary<string, bool>();

        inScenePortals = new SerializableDictionary<string, Vector3>();

        completedQuests = new SerializableDictionary<string, bool>();
        activeQuests = new SerializableDictionary<string, int>();

    }
}
