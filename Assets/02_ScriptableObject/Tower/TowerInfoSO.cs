using UnityEngine;


[CreateAssetMenu(menuName ="TowerInfoSO", fileName ="TowerInfoSO")]
public class TowerInfoSO : ScriptableObject
{
    public TowerLevel[] levels;
}

[System.Serializable]
public class TowerLevel
{
    [Header("Stats")]
    public int cost;
    public int damage;
    public float fireRateCoefficient;
    public float range;

    [Header("Visual/Prefab")]
    public Sprite bodyImage;
    public Sprite headImage;

    [Header("Projectile")]
    public GameObject projectilePrefab;

    [Header("SFX")]
    public AudioClip shootSfx;

    [HideInInspector]
    public float fireRate() { return 1 / fireRateCoefficient; }



}
