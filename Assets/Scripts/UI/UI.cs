using PixelCrushers.DialogueSystem;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UI : MonoBehaviour
{

    public static UI instance;

    [SerializeField] private GameObject[] uiElements;
    public bool alternativeInput { get; private set; }
    private PlayerInputSet input;
    private bool skillTreeEnabled;
    private bool inventoryEnabled;
    private bool merchantEnabled;
    private bool craftEnabled;
    private bool questEnabled;


    public UI_SkillToolTip skillToolTip { get; private set; }
    public UI_ItemToolTip itemToolTip { get; private set; }
    public UI_StatToolTip statToolTip { get; private set; }
    public UI_SkillTree skillTreeUI { get; private set; }

    public UI_Inventory inventoryUI { get; private set; }

    public UI_Merchant merchantUI { get; private set; }
  
    public UI_Craft craftUI { get; private set; }

    public UI_Storage storageUI { get; private set; }

    public UI_InGame inGameUI { get; private set; } 
    public UI_Options optionsUI { get; private set; }
    public UI_DeathScreen deathScreenUI {  get; private set; }

    public UI_FadeScreen fadeScreenUI { get; private set; }


    public UI_Quest questUI { get; private set; }

    public UI_DungeonSelectUI dungeonSelectUI { get; private set; }
    public ProximitySelector selector {  get; private set; }
    private void Awake()
    {
        skillToolTip = GetComponentInChildren<UI_SkillToolTip>();
        itemToolTip = GetComponentInChildren<UI_ItemToolTip>();
        statToolTip = GetComponentInChildren<UI_StatToolTip>();

        skillTreeUI = GetComponentInChildren<UI_SkillTree>(true);
        inventoryUI = GetComponentInChildren<UI_Inventory>(true);
        merchantUI = GetComponentInChildren<UI_Merchant>(true);
        craftUI = GetComponentInChildren<UI_Craft>(true);
        storageUI = GetComponentInChildren<UI_Storage>(true);
        inGameUI = GetComponentInChildren<UI_InGame>(true);
        optionsUI = GetComponentInChildren<UI_Options>(true);
        fadeScreenUI = GetComponentInChildren<UI_FadeScreen>(true);
        deathScreenUI = GetComponentInChildren<UI_DeathScreen>(true);
        questUI = GetComponentInChildren<UI_Quest>(true);
        dungeonSelectUI = GetComponentInChildren <UI_DungeonSelectUI>(true);
        selector = FindFirstObjectByType<ProximitySelector>();

        instance = this;

        // ×¢²á³¡¾°ÇÐ»»ÊÂ¼þ
        //SceneManager.sceneLoaded += OnSceneLoaded;

        skillTreeEnabled = skillTreeUI.gameObject.activeSelf;
        inventoryEnabled = inventoryUI.gameObject.activeSelf;
    }
    private void Start()
    {
        skillTreeUI.UnlockDefaultSkills();
    }
    public void SetupControlsUI(PlayerInputSet inputSet)
    {
        input = inputSet;

        input.UI.SkillTreeUI.performed += ctx => ToggleSkillTreeUI(); 
        input.UI.InventoryUI.performed += ctx => ToggleInventoryUI();
        input.UI.OptionsUI.performed += ctx =>
        {
            foreach (var element in uiElements)
            {
                if (element.activeSelf)
                {
                    Time.timeScale = 1;
                    SwitchToInGameUI();
                    return;
                }
            }

            Time.timeScale = 0;
            OpenOptionsUI();
        };

        input.UI.QuestUI.performed += ctx => ToggleQuestUI();


    }

    public void OpenDeathScreenUI()
    {

        SwitchTo(deathScreenUI.gameObject);

        input.Disable();

    }
    public void OpenOptionsUI()
    {

        HideAllTooltips();
        StopPlayerControls(true);
        SwitchTo(optionsUI.gameObject);
    }
    private void SwitchTo(GameObject objectToSwitchOn)
    {
        foreach (var element in uiElements)
            element.gameObject.SetActive(false);

        objectToSwitchOn.SetActive(true);

    }

    public void SwitchToInGameUI()
    {

        HideAllTooltips();
        StopPlayerControls(false);
        SwitchTo(inGameUI.gameObject);

        skillTreeEnabled = false;
        inventoryEnabled = false;
        craftEnabled = false;
        merchantEnabled = false;
        questEnabled = false;
    }
    public void HideAllTooltips()
    {
        itemToolTip.ShowToolTip(false, null);
        skillToolTip.ShowToolTip(false, null);
        statToolTip.ShowToolTip(false, null);
    }
    public void ToggleQuestUI()
    {

        questUI.transform.SetAsLastSibling();
        SetToolTipsAsLastSibling();
        questEnabled = !questEnabled;
    
        questUI.gameObject.SetActive(questEnabled);
       if(questEnabled)  questUI.SetupQuestUI(questUI.questManager.availableQuests);

        HideAllTooltips();
        StopPlayerControlsIfNeeded();
    }

    private void SetToolTipsAsLastSibling()
    {
        itemToolTip.transform.SetAsLastSibling();
        skillToolTip.transform.SetAsLastSibling();
        statToolTip.transform.SetAsLastSibling();

    }
    public void ToggleSkillTreeUI()
    {

        skillTreeUI.transform.SetAsLastSibling();
        SetToolTipsAsLastSibling();
        fadeScreenUI.transform.SetAsLastSibling();
        skillTreeEnabled = !skillTreeEnabled;
        skillTreeUI.gameObject.SetActive(skillTreeEnabled);

        HideAllTooltips();
        StopPlayerControlsIfNeeded();
    }

    public void ToggleInventoryUI()
    {

        inventoryUI.transform.SetAsLastSibling();
        SetToolTipsAsLastSibling();
        fadeScreenUI.transform.SetAsLastSibling();
        inventoryEnabled = !inventoryEnabled;
        inventoryUI.gameObject.SetActive(inventoryEnabled);
        HideAllTooltips();
        StopPlayerControlsIfNeeded();
    }
    public void OpenStorageUI(bool openStorageUI)
    {
        storageUI.gameObject.SetActive(openStorageUI);
        StopPlayerControls(openStorageUI);

        if (openStorageUI == false)
        {
            craftUI.gameObject.SetActive(false);
            HideAllTooltips();
        }
    }

    public void talkingOpenMerchantUI()
    {

        if (merchantUI.gameObject.activeSelf) return;
        merchantUI.gameObject.SetActive(true);
        StopPlayerControls(true);
    }

    public void OpenMerchantUI(bool openMerchantUI)
    {
      
        Debug.Log($"OpenMerchantUI: {openMerchantUI}");
        merchantUI.gameObject.SetActive(openMerchantUI);

        StopPlayerControls(openMerchantUI);
        if (openMerchantUI == false)
        {
            HideAllTooltips();
        }
    }
    public void OpenCraftUI(bool openStorageUI)
    {
        craftUI.gameObject.SetActive(openStorageUI);
        StopPlayerControls(openStorageUI);

        if (openStorageUI == false)
        {
            craftUI.gameObject.SetActive(false);
            HideAllTooltips();
        }
    }

    public void OpenDungeonUI()
    {
        dungeonSelectUI.gameObject.SetActive(true);
        StopPlayerControls(true);

        HideAllTooltips();

    }

    private void StopPlayerControlsIfNeeded()
    {
        foreach (var element in uiElements)
        {
            
            if (element.activeSelf)
            {
               
                StopPlayerControls(true);
                return;
            }
        }
        StopPlayerControls(false);
    }

    private void StopPlayerControls(bool stopControls)
    {
        if (stopControls)
        {
            input.Player.Disable();
            input.Player.Interact.Enable();
            selector.enabled= false;
        }
        else
        {
            input.Player.Enable();
            selector.enabled = true;
        }

    }

    //private void OnDestroy()
    //{
    //    SceneManager.sceneLoaded -= OnSceneLoaded;
    //}

    //private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    //{

    //    if (dungeonSelectUI != null)
    //        dungeonSelectUI.gameObject.SetActive (false);

    //}
}
