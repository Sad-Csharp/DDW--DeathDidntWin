using System;
using HarmonyLib;
using UnityEngine;

namespace DeathDidntWin.Patches;

public static class Patcher
{
    private static Harmony harmony_;
    
    public static void Init()
    {
        if (harmony_ != null)
            return;
        
        harmony_ = new Harmony("DeathDidntWin");
    }

    public static void TryPatch(Type type)
    {
        try
        {
            harmony_.PatchAll(type);
            Debug.Log("Applied patche for " + type.Name);
        }
        catch (Exception ex)
        {
            Debug.LogError("Unable to apply patches for " + type.Name + ", error: " + ex.Message);
        }
    }
}