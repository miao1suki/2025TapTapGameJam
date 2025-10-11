using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JiMiUnlockManager : MonoBehaviour
{
    public static JiMiUnlockManager Instance;  //����

    public List<JiMiDataSO> allJiMis;
    private void Awake()
    {
        Instance = this;

        // ��̬�������� JiMiDataSO
        JiMiDataSO[] loadedJiMis = Resources.LoadAll<JiMiDataSO>("JiMiData");
        allJiMis = new List<JiMiDataSO>(loadedJiMis);
    }
    public void OnPlayerEvent(Player player)
    {
        //�¼��仯ʱ�����
        foreach (var jimi in allJiMis)
        {
            if (jimi.UnlockCondition != null)
                jimi.UnlockCondition.OnEventTriggered(player);
        }
    }

    public void CheckUnlocks(Player player)
    {
        //����Ƿ�����������������ڼ���Ƿ�Ӧ���л���������
        foreach (var jimi in allJiMis)
        {
            if (!jimi.IsUnlocked && jimi.UnlockCondition != null)
            {
                if (jimi.UnlockCondition.IsSatisfied(player))
                {
                    Unlocke(jimi);
                    Debug.Log($"{jimi.DisplayName} �ѽ�����");
                    // ���� UI��������ȡ�����״̬��
                }
            }
        }
    }

    public bool IsUnlocked(string jimiName)
    {
        //��ȡIsUnlocked��״̬���޷����ڽ�����ֻ�ǲ鿴��ǰ״̬��
        var jimi = allJiMis.Find(j => j.name == jimiName);
        return jimi != null && jimi.IsUnlocked;
    }

    private void Unlocke(JiMiDataSO jimi)
    {
        jimi.IsUnlocked = true;
    }
}
