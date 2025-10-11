using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Rigidbody rb;
    public static Player Instance;//����


    [Header("��������")]
    public float _PlayerVelocity = 0.00f;//��ҵ�ǰ�ٶ�
    public float _PlayerMaxVelocity = 5.00f;//�������ٶ�
    public int _PlayerHealth = 100;//��ҵ�ǰѪ��
    [SerializeField]
    private int _PlayerMaxHealth = 100;//������Ѫ��


    [Header("��ǰ����������")]
    public JiMiType currentJiMiType = JiMiType.Normal;

    [Header("��ǰ����������")]
    public JiMiAbility currentAbility;

    [Header("���ֹ�������")]
    public int haQiCount = 0;



    private void Awake()
    {
        Instance = this;
        haQiCount = 0;//���ù�������
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError(gameObject.name + " û�л�ȡ�� Rigidbody");
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
        // �ж��Ƿ����
        string typeName = newType.ToString(); // ʹ��ö������Ϊ JiMiDataSO ����
        if (!JiMiUnlockManager.Instance.IsUnlocked(typeName))
        {
            Debug.LogWarning($"������ {typeName} δ�������޷��л���");
            return; // δ������ֱ�ӷ��أ����л�
        }
        // ���������
        if (currentAbility != null)
        {
            currentAbility.OnDeactivate(this);
            Destroy(currentAbility);
        }


        currentJiMiType = newType;

        // �����������������
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

        // ����������
        if (currentAbility != null)
            currentAbility.OnActivate(this);

        Debug.Log("��ǰ�������л�Ϊ��" + currentJiMiType);
    }
}
