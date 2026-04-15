using UnityEngine;

public class UI_MerchantButtons : MonoBehaviour
{
   [SerializeField]  private Inventory_Merchant merchant;

    public ItemType shopType;  // 在 Inspector 里设置按钮对应的类型
  
    private void Awake()
    {
      
        
    }

    // 只写一个方法，所有按钮都能用
    public void ChangeCategory()
    {
     
        if (merchant == null)
        {
            Debug.LogError("找不到 Inventory_Merchant 脚本！");
            return;
        }
  
        merchant.FillShopList(shopType);
    }
}