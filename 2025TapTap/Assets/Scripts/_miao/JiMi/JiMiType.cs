// JiMi ���Ͷ���
namespace miao
{
    public enum JiMiType
    {
        Normal,         // ��ͨ������
        ZhiShengJi,     // ֱ����
        DingDongJi,     // ������
        HaNiuMo,        // ��ţħ
    }
}

/*
 * �������׷�ʽ��
 * 1����JiMiType�����ű�����������
 * 2����TypeĿ¼���½�   xxx_JiMiAbility�ű�������Normal_JiMiAbility���ݲ��滻
 * 3����Player�ű��� SetJiMiType() ����������Switch��case
 * 4���� Resources/JiMiData ���½���Ӧ���׵�SO
 * 
 * 5������JiMiDataSO�Ķ��������и���
 * 6���� JiMi/SO/class/UnlockCondition �� �½� xxx_UnlockCondition�ű�������Normal_UnlockCondition���ݲ��滻
 * 7���� Resources/UnlockCondition ���½���Ӧ������SO
 * 8�����ڣ�7������SO�ϸ��ڣ�4������SO��Ϊ����
 */