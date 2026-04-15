using UnityEngine;

public class WaterTriggerHandler : MonoBehaviour
{
    [SerializeField] private LayerMask waterMask;
    [SerializeField] private GameObject splashParticles;

    private EdgeCollider2D edgeColl;
    private InteractableWater water;

    private void Awake()
    {
        edgeColl = GetComponent<EdgeCollider2D>();
        water = GetComponent<InteractableWater>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
       


        if ((waterMask.value & (1 << collision.gameObject.layer)) > 0)
        {
            Debug.Log("Layer Detected");
            Rigidbody2D rb = collision.GetComponentInParent<Rigidbody2D>();
            if (rb != null)
            {
                Vector2 localPos = gameObject.transform.localPosition;
                Vector2 hitObjectPos = collision.transform.position;
                Bounds hitObjectBounds = collision.bounds;

                Vector3 spawnPos = Vector3.zero;
                if (collision.transform.position.y >= edgeColl.points[1].y + edgeColl.offset.y + localPos.y)
                {
                    spawnPos = hitObjectPos - new Vector2(0f, hitObjectBounds.extents.y);

                }
                else
                {
                    spawnPos = hitObjectPos + new Vector2(0f, hitObjectBounds.extents.y);

                }

                GameObject waterwave= Instantiate(splashParticles, spawnPos, Quaternion.identity);
                Destroy(waterwave,1f);
                int multiplier = 1;

                if (rb.linearVelocity.y < 0)
                {
                    multiplier = -1;
                }
                else
                {
                    multiplier = 1;
                }
                float vel = rb.linearVelocity.y * water.ForceMultiplier;
                vel = Mathf.Clamp(Mathf.Abs(vel), 0f, water.MaxForce);

                vel *= multiplier;

                water.Splash(collision, vel);
            }



        }
        else
        {
            Debug.Log("No collision");
        }
    }

}
