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
    [SerializeField] private TextMeshProUGUI curInfoTxt;
    [SerializeField] private TextMeshProUGUI upgradeCostTxt;
    private GoldManager goldManager;
    private TowerMenuController registry;

    public Vector3Int placedCell { get; set; }
    public TowerPlacer ownerPlacer { get; set; }

    [Header("Info")]
    public int costValue;
    public int upgradeCost;
    private int totalCost;
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

        totalCost += costValue;
        curInfoTxt.text = $"Level : {currentLevelIndex+1}\nDamage : {curDamage}\nFire Rate : {curFireRate}\nRange : {curRange}";
        upgradeCostTxt.text = IsMaxLevel ? "-" : $"{towerData.levels[lv+1].cost}";
    }


    public bool IsMaxLevel => currentLevelIndex >= towerData.levels.Length - 1;

    public void Upgrade()
    {
        if (IsMaxLevel) return;
        currentLevelIndex++;
        Init(currentLevelIndex);
        goldManager.TrySpend(costValue);
        ApplyLevel(currentLevelIndex);
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

        //if (menuController != null) return;
        //if (gameObject.activeInHierarchy)
        //{
        //    menuController = GetComponentInParent<TowerMenuController>();
        //    menuController.towers.Add(this);
        //}
    }

    public void ShowInfo()
    {

        var active = curInfoImage.gameObject.activeInHierarchy;
        curInfoImage.gameObject.SetActive(!active);
    }

    public void Sell()
    {
        goldManager.Add(totalCost);

        // 점유 해제 + 그리드 갱신
        if (ownerPlacer != null)
        {
            ownerPlacer.FreeCell(placedCell, GetComponentsInChildren<Collider2D>());
        }

        registry?.Unregister(this);

        Destroy(gameObject);
    }

    private void OnEnable()
    {
        registry = GetComponentInParent<TowerMenuController>();
        registry?.Register(this);
    }

    private void OnDisable()
    {
        // SetActive(false)나 파괴 직전에도 호출됨
        registry?.Unregister(this);
    }

    // 선택: 방어적 중복 제거
    private void OnDestroy()
    {
        registry?.Unregister(this);
    }


}
