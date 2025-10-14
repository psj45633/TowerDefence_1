using UnityEngine;

public class ProjectileAttack : MonoBehaviour, ITowerAttack
{
    private Tower owner;







    public void Init(Tower owner)
    {
        
    }

    public void Apply(TowerInfoSO data)
    {

    }
    public bool CanFire(Enemy target)
    {


        return target != null;
    }

    public void Attack(Enemy target)
    {
        Debug.Log("Projectile Attack");
    }

    public void Tick(float dt)
    {

    }
}
