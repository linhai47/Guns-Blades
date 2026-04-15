using System.Collections.Generic;
using UnityEngine;

public class SkillObject_TornadoSword :SkillObject_Base
{
    [Header("Tornado Settings")]
    [SerializeField] private float pullForce = 5f;       // 吸附力度
    [SerializeField] private float pullRadius = 5f;      // 吸附范围
    [SerializeField] private float lifeTime = 3f;        // 龙卷剑持续时间
    [SerializeField] private float damageInterval = 1f;  // 伤害间隔
    [SerializeField] private float damageRadius = .8f;
    
    private bool enemyDetected = false;

    public float timer;
    public float damageTimer;

    private List<Collider2D> enemiesInRange = new List<Collider2D>();
    protected override void start()
    {
        base.start();
     
       
       
    }
    public override void Setup(SkillBase skill,bool isFlying = true)
    {
        base.Setup(skill);
        Destroy(gameObject,lifeTime);
        SetDirection();
    }
    private void Update()
    {
        timer += Time.deltaTime;
       if(enemyDetected)damageTimer += Time.deltaTime;

        // 定时对范围内敌人造成伤害
        if (damageTimer >= damageInterval)
        {
            Circle_AOE_Damage(damageRadius,transform);
            damageTimer = 0f;
        }

       


    }

    private void FixedUpdate()
    {
        

    }


    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (((1 << collision.gameObject.layer) & whatIsEnemy) == 0) return;
        Circle_AOE_Damage(damageRadius, transform);
        damageTimer = 0f;
        enemyDetected = true;


      

    }



    protected void OnTriggerExit2D(Collider2D collision)
    {
        if (((1 << collision.gameObject.layer) & whatIsEnemy) == 0) return; 
        enemyDetected = false;
    }
}
