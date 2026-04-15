using UnityEngine;
using UnityEngine.InputSystem;

public class SkillInputManager : MonoBehaviour
{
    private Player player;
    private Entity_SkillManager skillManager;

    public SkillType[] skills;

    private void Awake()
    {
        player = GetComponent<Player>();
        skillManager = GetComponent<Entity_SkillManager>();
        
       
    }

    private void Start()
    {
        if (player.input != null)
        {
            player.input.Player.Skill.performed += OnSkillPerformed;
            player.input.Enable(); 
        }
    }
    private void OnSkillPerformed(InputAction.CallbackContext ctx)
    {
        skills = skillManager.playerSkillSlots;
        string key = ctx.control.name; // 获取按键名
        int index = KeyToSkillIndex(key); // 自定义方法：按键映射到技能索引
        if (index >= 0 && index < skills.Length)
        {
         
            SkillBase skillbase = skillManager.GetSKillByType(skills[index]);
           
            skillbase.TryUseSkill() ; // 调用技能
        }


    }

    private int KeyToSkillIndex(string key)
    {
        switch (key)
        {
            case "q": return 0;
            case "w": return 1;
            case "e": return 2;
            case "r": return 3;
            case "t": return 4;
            case "y": return 5;
            default: return -1;
        }
    }



   
    private void OnDisable()
    {
        player.input.Player.Skill.performed -= OnSkillPerformed;
    }

}
