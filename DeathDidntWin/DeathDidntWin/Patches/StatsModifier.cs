using Death.Run.Core;
using Death.Utils;
using HarmonyLib;
using UnityEngine;

namespace DeathDidntWin.Patches;


[HarmonyPatch]
public class StatsModifier
{
    [HarmonyPatch(typeof(IStatsEx), "GetAsRadius")]
    [HarmonyPrefix]
    private static bool PatchPullRadius(IStats stats, StatId stat, ref float __result)
    {
        if (stat != (StatId) 8)
            return true;
        
        __result = MathUtils.CalcRadiusFromArea(100000f);
        return false;
    }
    
    //[HarmonyPatch(typeof(Entity), nameof(Entity.MaxHealth), MethodType.Getter)]
    //[HarmonyPrefix]
    //private static bool PatchMaxHealth(Entity __instance, ref float __result)
    //{
    //    if (!Mathf.Approximately(__instance.MaxHealth, float.MaxValue))
    //        return true;
    //    __result = float.MaxValue;
    //    return false;
    //}
}