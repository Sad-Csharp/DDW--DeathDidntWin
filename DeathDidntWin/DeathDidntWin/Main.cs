using System;
using BepInEx;
using Death.App.UserInterface.Cursors;
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
    private Rect itemWindowRect_ = new(10, 10, 10, 10);
    private Rect windowRect_ = new (0,0,350,350);
    private bool isWindowToggled_;
    
    #endregion
    
    private void Start()
    {
        Debug.LogWarning("Death Didnt Win has been loaded!");
        Patcher.Init();
        Patcher.TryPatch(typeof(ShardPullRangePatch));
        Skin.TryLoadSkin();
        windowRect_ = Utils.CenterWindow(windowRect_);
        itemWindowRect_ = Utils.CenterWindow(itemWindowRect_);
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
                Debug.LogError("Unable to find player object! Health adder disabled");
                return;
            }
            player.Entity.GainLife(Lifegain.Heal(1000f), true);
            Debug.LogWarning($"Life added to {player.gameObject.name}!");
        }
    }

    private void OnGUI()
    {
        if (!isWindowToggled_)
            return;
        
        if (Skin.WhiteSkin != null)
            GUI.skin = Skin.WhiteSkin;
        
        CursorManager.ForceOsCursor = isWindowToggled_;
        
        windowRect_ = GUILayout.Window(GetHashCode(), windowRect_, DDWGui.Draw, "<b>Death Didnt Win</b>", GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
        
        try
        {
            Tabshow(windowRect_);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    private void Tabshow(Rect mainWindow)
    {
        if (!isWindowToggled_)
        {
            DDWGui.IsShowingItemWindow = false;
            return;
        }
        
        if (!DDWGui.IsShowingItemWindow)
            return;

        // REQUIRED, we're no longer Overriding so we need to detect the Repaint so the window can be moved
        if (Event.current.type == EventType.Repaint)
        {
            itemWindowRect_.x = windowRect_.x + windowRect_.width + 10;
            itemWindowRect_.y = windowRect_.y;
        }

        itemWindowRect_ = GUILayout.Window(GetHashCode() + 1, itemWindowRect_, DDWGui.DrawItems, "<b>Item Spawner</b>", GUILayout.MinWidth(300), GUILayout.MinHeight(10));
    }
}