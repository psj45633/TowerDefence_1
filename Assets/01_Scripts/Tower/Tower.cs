using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Tower : MonoBehaviour
{
    public TowerInfoSO towerData;
    public int currentLevelIndex = 0;

    ITowerAttack attack;
    public TowerTargeter targeter;

    private int wallLayer;

    [SerializeField] private Image curInfoImage;
    [SerializeField] private TextMeshProUGUI curInfotxt;
    private GoldManager goldManager;

    [Header("Info")]
    public int costValue;
    public int curDamage;
    public float curFireRate;
    public float curRange;
    public Sprite curBodyImage;
    public Sprite curHeadImage;
    public GameObject curProjectile;
    public AudioClip curAudioClip;

    

    void Awake()
    {
        ApplyLevel(0);

        attack = GetComponentInChildren<ITowerAttack>();
        attack.Init(this);
        attack.Apply(towerData);
        targeter = GetComponentInChildren<TowerTargeter>();
        Init(currentLevelIndex);

        var gm = Object.FindAnyObjectByType<GoldManager>();
        goldManager = gm;
    }

    private void Init(int lv)
    {
        costValue = towerData.levels[lv].cost;
        curDamage = towerData.levels[lv].damage;
        curFireRate = towerData.levels[lv].fireRate;
        curRange = towerData.levels[lv].range;
        curBodyImage = towerData.levels[lv].bodyImage;
        curHeadImage = towerData.levels[lv].headImage;
        curProjectile = towerData.levels[lv].projectilePrefab;
        curAudioClip = towerData.levels[lv].shootSfx;
        
        curInfotxt.text = $"Level : {currentLevelIndex+1}\nDamage : {curDamage}\nFire Rate : {curFireRate}\nRange : {curRange}";
    }


    public bool IsMaxLevel => currentLevelIndex >= towerData.levels.Length - 1;

    public void Upgrade()
    {
        if (IsMaxLevel) return;

        goldManager.TrySpend(costValue);
        currentLevelIndex++;
        ApplyLevel(currentLevelIndex);

        Init(currentLevelIndex);
    }

    private void ApplyLevel(int levelIndex)
    {
        currentLevelIndex = Mathf.Clamp(levelIndex, 0, towerData.levels.Length - 1);
    }


    float timer;
    void Update()
    {
        var target = targeter.currentTarget;

        timer += Time.deltaTime;
        //if (target != null && timer > fireCooldown && attack.CanFire(target))
        if (target != null && timer > curFireRate)
        {
            attack.Attack(target);
            timer = 0;
        }

    }

    public void ShowInfo()
    {
        var active = curInfoImage.gameObject.activeInHierarchy;
        curInfoImage.gameObject.SetActive(!active);
    }






}
