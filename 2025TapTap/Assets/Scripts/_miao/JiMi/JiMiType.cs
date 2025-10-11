// JiMi 类型定义
namespace miao
{
    public enum JiMiType
    {
        Normal,         // 普通哈基米
        ZhiShengJi,     // 直升基
        DingDongJi,     // 盯洞基
        HaNiuMo,        // 哈牛魔
    }
}

/*
 * 新增基米方式：
 * 1、在JiMiType（本脚本）加入类型
 * 2、在Type目录下新建   xxx_JiMiAbility脚本，复制Normal_JiMiAbility内容并替换
 * 3、在Player脚本的 SetJiMiType() 方法中增加Switch的case
 * 4、在 Resources/JiMiData 下新建对应基米的SO
 * 
 * 5、如有JiMiDataSO改动需求自行更改
 * 6、在 JiMi/SO/class/UnlockCondition 下 新建 xxx_UnlockCondition脚本，复制Normal_UnlockCondition内容并替换
 * 7、在 Resources/UnlockCondition 下新建对应条件的SO
 * 8、将第（7）步的SO拖给第（4）步的SO作为条件
 */