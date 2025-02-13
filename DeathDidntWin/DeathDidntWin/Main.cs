using BepInEx;
using Death.Run.Behaviours;
using Death.Run.Core;
using DeathDidntWin.Patches;
using DeathDidntWin.Miscellanious;
using UnityEngine;
using Utils = DeathDidntWin.Miscellanious.Utils;

namespace DeathDidntWin;

[BepInPlugin("com.DeathDidntWin.mod", "Death Lost :)", "1.0.0")]
public class Main : BaseUnityPlugin
{
    #region Fields

    // GUI Window properties
    private Rect windowRect_ = new (0,0,350,350);
    private bool isWindowToggled_;
    #endregion
    
    private void Start()
    {
        Debug.LogWarning("Death Didnt Win has been loaded!");
        Patcher.Init();
        Patcher.TryPatch(typeof(StatsModifier));
        Skin.TryLoadSkin();
        windowRect_ = Utils.CenterWindow(windowRect_);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Backslash))
            isWindowToggled_ = !isWindowToggled_;

        var player = Player.Instance;
        if (player == null)
            return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (player == null)
            {
                Debug.LogError("Unable to find player object! Health added disabled");
                return;
            }
            player.Entity.GainLife(Lifegain.Heal(1000f), true);
        }
    }

    private void OnGUI()
    {
        if (!isWindowToggled_)
            return;
        
        if (Skin.WhiteSkin != null)
            GUI.skin = Skin.WhiteSkin;
        
        Cursor.visible = isWindowToggled_;
        
        windowRect_ = GUILayout.Window(GetHashCode(), windowRect_, DDWGui.Draw, "Death Didnt Win", GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
    }
}