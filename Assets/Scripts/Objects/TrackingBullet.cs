using UnityEngine;

public class TrackingBullet : Projectile
{
    public float trackingTime = 1.2f;
    public float raycastDistance = 10f;
    public float LerpTime = 0.02f;
    public bool isMoving = false;
    public float rand = 0;

    private void Start()
    {

        //rand = Random.Range(-30,30);
        
        co = null;
    }

    protected override void Update()
    {
        if (!isMoving) return;
        base.Update();
        if (co == null) co = GetComponent<Collider2D>();
        if (trackingTime > 0)
        {
            if (targetTransform)
            {

                var diff = targetTransform.position - transform.position;
                var nowZ = transform.rotation.eulerAngles.z;
                var toZ = Mathf.Atan2(diff.y, diff.x) * 180 / Mathf.PI;

 
               transform.rotation = Quaternion.Euler(0, 0, Mathf.LerpAngle(nowZ, toZ, LerpTime));

                Debug.Log(targetTransform.position);
            }
            else
            {

                Collider2D hit = Physics2D.OverlapCircle(transform.position, raycastDistance, whatIsPlayer);

                if (hit)
                {
                    targetTransform = hit.transform;
                }
            }
            trackingTime -= Time.deltaTime;

        }


        float degree = transform.rotation.eulerAngles.z + rand;
        float radian = Mathf.PI / 180 * degree;
        float x = Mathf.Cos(radian);
        float y = Mathf.Sin(radian);

        Vector3 movement = new Vector3(x, y, 0) * speed * Time.deltaTime ;
        transform.position += movement;
    }

    public void SetRotation(float angle)
    {
        
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);

        if (((1 << collision.gameObject.layer) & whatIsPlayer) == 0) return;

        ApplyDamageToTarget(collision);



    }

}
