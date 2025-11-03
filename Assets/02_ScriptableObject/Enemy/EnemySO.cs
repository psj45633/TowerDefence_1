using UnityEngine;


[CreateAssetMenu(fileName ="EnemySO",menuName ="EnemySO")]
public class EnemySO : ScriptableObject
{
    public int stage;

    public int MaxHp;
    public float baseMoveSpeed = 1f;

    public int rewardGold;

    public Sprite sprite;
    public Color tintColor = Color.white;
}
