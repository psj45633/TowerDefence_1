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
    public float fireRate;
    public float range;

    [Header("Visual/Prefab")]
    public GameObject modealPrefab;

    [Header("Projectile")]
    public GameObject projectilePrefab;

    [Header("SFX")]
    public AudioClip shootSfx;





}
