using UnityEngine;
using PixelCrushers.DialogueSystem; // Dialogue System 的命名空间

[RequireComponent(typeof(Collider2D))]
public class NPCProximityInteractor : MonoBehaviour
{
    [Header("References")]
    public Rigidbody2D rb;
    public Object_NPC npc;

    private bool isPatroling;

    [Header("Dialogue")]
    [Tooltip("在 Dialogue Database 中的对话标题（Conversation title）")]
    public string conversationTitle;

    [Header("Interaction")]
    public KeyCode interactKey = KeyCode.Space;
    [Tooltip("是否在进入范围时显示提示（可在 Inspector 把 UI 对象拖进来）")]
    public GameObject promptUI;

    private bool playerInRange = false;

    void Start()
    {
        if (npc == null) npc = GetComponent<Object_NPC>();
        if (rb == null) rb = GetComponent<Rigidbody2D>();

        if (npc != null)
        {
            isPatroling = npc.canPatrol;
        }
        else
        {
            Debug.LogWarning($"{name}: 未找到 Object_NPC 组件！");
        }

        if (promptUI) promptUI.SetActive(false);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (npc != null)
            {
                npc.canPatrol = false;
                npc.SetState(NPCState.Idle);
            }

            Debug.Log($"{name}: Player entered NPC range!");
            playerInRange = true;
            if (promptUI) promptUI.SetActive(true);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log($"{name}: Player exited NPC range!");

            if (npc != null && isPatroling)
            {
                npc.canPatrol = true;
            }

            playerInRange = false;
            if (promptUI) promptUI.SetActive(false);
        }
    }

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(interactKey))
        {
            if (!string.IsNullOrEmpty(conversationTitle))
            {
                Debug.Log($"{name}: Starting conversation '{conversationTitle}'");
                DialogueManager.StartConversation(conversationTitle, transform);
            }
            else
            {
                Debug.LogWarning($"{name}: conversationTitle is empty. 请在 Inspector 设置！");
            }
        }
    }

    // 可视化 Trigger 范围
    void OnDrawGizmosSelected()
    {
        var col = GetComponent<Collider2D>();
        Gizmos.color = Color.green;
        if (col is CircleCollider2D cc)
        {
            Gizmos.DrawWireSphere(transform.position + (Vector3)cc.offset, cc.radius);
        }
        else if (col is BoxCollider2D bc)
        {
            var size = bc.size;
            var pos = (Vector3)bc.offset + transform.position;
            Gizmos.DrawWireCube(pos, size);
        }
    }
}
