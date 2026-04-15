using UnityEngine;

public class Enemy_ObjectAnimationTriggers : Entity_AnimationTriggers
{
    public Enemy enemy;

    public GameObject enemy_objects;
    private EnemyObjects casting_Objects;
    public Transform objectTransform;
    public float duration;




    protected override void Awake()
    {
        base.Awake();
        enemy = GetComponentInParent<Enemy>();
        
    }

    public void Create_SingleHitVfx()
    {
        GameObject singleHitVfx = Instantiate(enemy_objects, objectTransform.position, Quaternion.identity);

        EnemyObjects_SingleHitVfx casting_Objects = singleHitVfx.GetComponentInChildren<EnemyObjects_SingleHitVfx>();
        casting_Objects.duration = duration;
        casting_Objects.Setup(enemy,false);

    }

  
}
