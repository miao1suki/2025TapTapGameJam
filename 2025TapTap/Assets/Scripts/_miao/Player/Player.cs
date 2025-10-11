using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Rigidbody rb;
    public static Player Instance;//单例


    [Header("基础属性")]
    public float _PlayerVelocity = 0.00f;//玩家当前速度
    public float _PlayerMaxVelocity = 5.00f;//玩家最大速度
    public int _PlayerHealth = 100;//玩家当前血量
    [SerializeField]
    private int _PlayerMaxHealth = 100;//玩家最大血量


    [Header("当前哈基米类型")]
    public JiMiType currentJiMiType = JiMiType.Normal;

    [Header("当前哈基米能力")]
    public JiMiAbility currentAbility;

    [Header("本局哈气次数")]
    public int haQiCount = 0;



    private void Awake()
    {
        Instance = this;
        haQiCount = 0;//重置哈气次数
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError(gameObject.name + " 没有获取到 Rigidbody");
        }
    }
    void Start()
    {
        
    }


    void Update()
    {
 
    }

    public void ResetPlayerHealth()
    {
        _PlayerHealth = _PlayerMaxHealth;
    }

    public void ChangePlayerHealth(int health)
    {
        _PlayerHealth += health;
    }

    public void SetPlayerHealth(int health)
    {
        _PlayerHealth += health;
    }

    public void SetJiMiType(JiMiType newType)
    {
        // 判断是否解锁
        string typeName = newType.ToString(); // 使用枚举名作为 JiMiDataSO 名称
        if (!JiMiUnlockManager.Instance.IsUnlocked(typeName))
        {
            Debug.LogWarning($"哈基米 {typeName} 未解锁，无法切换！");
            return; // 未解锁，直接返回，不切换
        }
        // 清除旧能力
        if (currentAbility != null)
        {
            currentAbility.OnDeactivate(this);
            Destroy(currentAbility);
        }


        currentJiMiType = newType;

        // 根据类型添加新能力
        switch (newType)
        {
            case JiMiType.Normal:
                currentAbility = gameObject.AddComponent<Normal_JiMiAbility>();
                break;
            case JiMiType.ZhiShengJi:
                currentAbility = gameObject.AddComponent<ZhiShengJi_JiMiAbility>();
                break;
            case JiMiType.DingDongJi:
                currentAbility = gameObject.AddComponent<DingDongJi_JiMiAbility>();
                break;
            case JiMiType.HaNiuMo:
                currentAbility = gameObject.AddComponent<HaNiuMo_JiMiAbility>();
                break;
            default:
                currentAbility = null;
                break;
        }

        // 激活新能力
        if (currentAbility != null)
            currentAbility.OnActivate(this);

        Debug.Log("当前哈基米切换为：" + currentJiMiType);
    }
}
