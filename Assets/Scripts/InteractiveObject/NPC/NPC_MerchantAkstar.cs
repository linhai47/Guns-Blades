using UnityEngine;

public class NPC_MerchantAkstar : Object_NPC, IInteractable
{

    [SerializeField] private UI_ShopAvatar shopAvatar;
    [SerializeField] private UIShopAnimation[] shopAnim;

  
    protected override void Awake()
    {
        base.Awake();
        merchant = GetComponent<Inventory_Merchant>();
        ChangeIdle(); 
       
    }
    
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);
        //Debug.Log("Enter");

        
    }
    protected override void OnTriggerExit2D(Collider2D collision)
    {
        base.OnTriggerExit2D(collision);

        //ui.OpenMerchantUI(false);
    }

    
    protected override void Update()
    {
        base.Update();
        
        
        
    }
    public override void Interact()
    {
        base.Interact();
        isOpen = !isOpen;
       

        if (!ui.merchantUI.gameObject.activeSelf)
        {
            
            isOpen = true;
        }
        Debug.Log("Interact");
        foreach (var anim in shopAnim)
        {
            anim.ResetToHidden();
            anim.Toggle(isOpen);
        }

   
       
        ui.OpenMerchantUI(isOpen);
        ui.merchantUI.SetupMerchantUI(merchant, inventory);
        merchant.FillShopList(merchant.type);
        if(ui.merchantUI.isActiveAndEnabled) shopAvatar.ChangeAnim();
      
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