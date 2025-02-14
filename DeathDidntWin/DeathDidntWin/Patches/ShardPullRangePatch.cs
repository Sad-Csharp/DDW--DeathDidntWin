using Death.Run.Core;
using Death.Utils;
using HarmonyLib;
using UnityEngine;

namespace DeathDidntWin.Patches;


[HarmonyPatch]
public class ShardPullRangePatch
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
}