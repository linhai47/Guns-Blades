using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class NPC_BlackSmithRuggles : Object_NPC , IInteractable
{
    private Inventory_Storage storage;

    protected override void Awake()
    {
        base.Awake();
        storage = GetComponent<Inventory_Storage>();
       
    }
    private void Start()
    {
    
    }
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);
        storage.SetInventory(inventory);
    }


    public override void Interact()
    {
        base.Interact();
        ui.craftUI.SetupCraftUI(storage);
        ui.storageUI.SetupStorageUI(storage);
        isOpen = !isOpen;
         ui.OpenCraftUI(isOpen);
        //ui.OpenStorageUI(true);
    }
    protected override void Enter()
    {
        base.Enter();
        currentState = NPCState.Idle;
    }
    protected override void ChangeIdle()
    {
        base.ChangeIdle();


    }
    protected override void ChangePatrol()
    {
        base.ChangePatrol();


    }
    protected override void ChangeTalk()
    {
        base.ChangeTalk();
        rb.linearVelocity = Vector2.zero;
    }

    protected override void ChangeTaskOver()
    {
        base.ChangeTaskOver();
    }
}
