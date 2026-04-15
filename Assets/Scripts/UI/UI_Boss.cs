using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

using DG.Tweening;
using System.Collections;

public class UI_Boss : MonoBehaviour
{
    private Boss boss;
    private Inventory_Player inventory;
    private UI_SkillSlot[] skillSlots;

    [Header("HP")]
    [SerializeField] private Slider frontBar; // 立即显示的血条（Instant）
    [SerializeField] private Slider backBar;  // 延迟减少的血条（Delay）
    [SerializeField] private RectTransform healthRect;
    [SerializeField] private Slider healthSlider;
    private float lastHealth;
    [SerializeField] private float delayThreshold = 0.8f; // 延迟开始前的等待时间
    [SerializeField] private float decreaseSpeed = 0.5f;  // 延迟血条缩减速度

    private float currentTargetPercent; // 最新的血量百分比
    private float delayTimer;           // 计时器：距离上次受伤的时间


    public Image bossImage;
    public TextMeshProUGUI SpellCard;
    public Image SpellCardImage;
    public Image imageLTR;
    public Image imageRTL;
    private void Start()
    {


        boss = FindFirstObjectByType<Boss>(FindObjectsInactive.Include);

        boss.health.OnHealthUpdate += OnHealthChanged;
        //boss.health.OnHealthUpdate += UpdateHealthBar;
        lastHealth = boss.health.maxHealth;
        currentTargetPercent = boss.health.GetHealthPercent();
        frontBar.value = backBar.value = currentTargetPercent;


    }



    private void Update()
    {
        if (backBar.value > frontBar.value)
        {
            delayTimer += Time.deltaTime;

            if (delayTimer >= delayThreshold)
            {
                Debug.Log("decrease");
                // 平滑递减
                backBar.value = Mathf.Lerp(backBar.value, frontBar.value, decreaseSpeed * Time.deltaTime);
            }
        }
        else
        {
            // 同步数值（比如回血时）
            backBar.value = frontBar.value;
            delayTimer = 0f;
        }
    }




    private void OnHealthChanged()
    {
        // 更新目标血量百分比
        currentTargetPercent = boss.health.GetHealthPercent();

        // 立即更新前层血条
        frontBar.value = currentTargetPercent;

        // 重置延迟计时
        delayTimer = 0f;
    }







    private void UpdateHealthBar()
    {
        float currentHealth = Mathf.RoundToInt(boss.health.GetcurrentHealth());
        float maxHealth = boss.stats.GetMaxHealth();
        //float sizeDifference = Mathf.Abs(maxHealth - healthRect.sizeDelta.x);

        //if (sizeDifference > .1f)
        //    healthRect.sizeDelta = new Vector2(maxHealth * .2f, healthRect.sizeDelta.y);




        healthSlider.value = boss.health.GetHealthPercent();


    }

    public void SpellCardSlideIn()
    {
        RectTransform rect = SpellCardImage.rectTransform;



        SpellCardImage.gameObject.SetActive(true);

        rect.localScale = Vector3.one * 10f;

        Sequence seq = DOTween.Sequence();

        seq.Append(rect.DOScale(Vector3.one, 1f)).SetEase(Ease.OutBack);
        seq.Join(rect.DOAnchorPos(new Vector2(200f, -850f), 1f).SetEase(Ease.InOutCubic));
        seq.Join(bossImage.rectTransform.DOAnchorPos(new Vector2(220, -515f), 1.5f)).SetEase(Ease.OutQuad);
        seq.Join(imageLTR.DOFade(0.1f, 1f));
        seq.Join(imageRTL.DOFade(0.1f, 1f));

        seq.AppendInterval(1.0f);
        seq.Append(bossImage.DOFade(0f, 1.5f));

        // Step3：缩小一半并移到上方
        seq.Join(rect.DOAnchorPos(new Vector2(200, -100f), 1f).SetEase(Ease.InOutCubic));
        seq.Join(rect.DOScale(Vector3.one * 0.9f, 1f).SetEase(Ease.InOutCubic));

        seq.OnComplete(() =>
        {
            Debug.Log("SpellCard 动画结束");
        });
    } 
}
