using UnityEngine;


[CreateAssetMenu(fileName ="EnemySO",menuName ="SO")]
public class EnemySO : ScriptableObject
{
    public int stageIndex;

    public int MaxHp;
    public float baseMoveSpeed = 1f;

    public int rewardGold;

    public Sprite sprite;
    public Color tintColor = Color.white;
}
