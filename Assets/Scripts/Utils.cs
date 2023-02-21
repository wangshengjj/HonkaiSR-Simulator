using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utils
{
    public static ObjectPool<Buff> valueBuffPool = new ObjectPool<Buff>(100);
    public static double Lerp(double min, double max, float scale, float value){
        return min + (max - min) * scale / value;
    }

    public static double Lerp(double min, double max, int scale, int value) 
    {
        return min + (max - min) * (float)scale / (float)value;
    }
    public static float Lerp(float min, float max, float scale, float value)
    {
        return min + (max - min) * scale / value;
    }
    public static float Lerp(float min, float max, int scale, int value)
    {
        return min + (max - min) * (float)scale / (float)value;
    }

    public static string[] attributeNames = new string[(int)CommonAttribute.Count]
    {
        "攻击力",
        "防御力",
        "生命上限",
        "速度",
        "暴击率",
        "暴击伤害",
        "治疗加成",
        "受治疗加成",
        "充能效率",
        "嘲讽权重",
        "效果命中",
        "效果抵抗",
        "全属性伤害加成",
        "物理伤害加成",
        "火属性伤害加成",
        "冰属性伤害加成",
        "雷属性伤害加成",
        "风属性伤害加成",
        "量子伤害加成",
        "虚数伤害加成",
        "全属性抗性",
        "物理伤害抗性",
        "火属性伤害抗性",
        "冰属性伤害抗性",
        "雷属性伤害抗性",
        "风属性伤害抗性",
        "量子伤害抗性",
        "虚数伤害抗性"
    };

}
