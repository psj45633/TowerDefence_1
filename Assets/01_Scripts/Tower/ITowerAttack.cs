public interface ITowerAttack
{
    void Init(Tower owner);
    void Apply(TowerInfoSO data);
    bool CanFire(Enemy target);
    void Attack(Enemy target);
    void Tick(float dt);
}
