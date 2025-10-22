using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Tower owner;
    private ObjectPool pool;



    private void Start()
    {
        owner = GetComponentInParent<Tower>();
        pool = owner.GetComponentInChildren<ObjectPool>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Enemy") || collision.CompareTag("Wall"))
        {
            pool.ReturnToPool(gameObject);
        }
    }

}
