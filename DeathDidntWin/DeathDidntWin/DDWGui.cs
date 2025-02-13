using System;
using System.Net.Mime;
using Death;
using Death.App;
using Death.Data;
using Death.Items;
using Death.Run.Behaviours;
using Death.Run.Behaviours.Abilities;
using Death.Run.Behaviours.Entities;
using Death.Run.Core;
using Death.Unlockables;
using UnityEngine;
using DeathDidntWin.Miscellanious;
using DeathDidntWin.Patches;
using HarmonyLib;

namespace DeathDidntWin;

public static class DDWGui
{
    #region Fields

    private static bool showUniqueItems_;
    private static Vector2 scrollPosition_;
    private static Vector2 scrollPosition_2;
    
    #endregion
    
    public static void Draw(int windowID)
    {
        using (new GUILayout.VerticalScope("box"))
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Debug Options");
            GUILayout.EndHorizontal();
            
            if (GUILayout.Button("Close Game"))
            {
                Application.Quit();
            }

            if (GUILayout.Button("Set 1000FPS Limit"))
            {
                Application.targetFrameRate = 1000;
            }
        }

        using (new GUILayout.VerticalScope("box"))
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Cheats");
            GUILayout.EndHorizontal();
            var player = Player.Instance;
            
            showUniqueItems_ = GUILayout.Toggle(showUniqueItems_, "Show Unique Items List");

            if (GUILayout.Button("Enable GodMode"))
            {
                if (player == null)
                {
                    Debug.LogError("Unable to find player object! God mode disabled");
                    return;
                }
                player.Entity.Invulnerable.AddStack();
            }

            if (GUILayout.Button("Disable GodMode"))
            {
                if (player == null)
                {
                    Debug.LogError("Unable to find player object! God mode disabled");
                    return;
                }
                player.Entity.Invulnerable.RemoveStack();
            }
            
            if (GUILayout.Button("Add 10k Gold"))
            {
                if (References.TryGetGameManager() == null)
                    return;
                
                var profile = References.TryGetGameManager().ProfileManager.Active;
            
                if (profile == null)
                    return;
                
                profile.Gold += 10000;
                Debug.LogWarning("Added 10k Gold!");
            }

            if (GUILayout.Button("Add 25k Exp"))
            {
                if (References.TryGetGameManager() == null)
                    return;

                var xpTrackerComponent = Player.Instance.gameObject.GetComponent<Behaviour_XpTracker>();
                if (xpTrackerComponent == null)
                    return;
                
                xpTrackerComponent.GainXp(25000f);
                Debug.LogWarning("Added 25k Exp!");
            }

            if (GUILayout.Button("Unlock All"))
            {
                UnlockAll();
            }

            if (GUILayout.Button("Reset Vendor Items"))
            {
                if (References.TryGetGameManager() == null)
                    return;
                
                var profile = References.TryGetGameManager().ProfileManager.Active;
                
                if (profile == null)
                    return;
                
                profile.ReGenerateShop();
                Debug.LogWarning("Vendor items have been changed!");
            }
        }


        using (new GUILayout.VerticalScope("box"))
        {
            try
            {
                var player = Player.Instance;
                scrollPosition_2 = GUILayout.BeginScrollView(scrollPosition_2, GUILayout.MinHeight(120));
                if (player == null)
                    return;

                var stats = player.Entity.Stats;
                if (stats == null)
                    return;

                var intial = 0f;
                var negativeInitial = -intial;

                if (GUILayout.Button("Reset Stats"))
                {
                    if (intial != 0f)
                        return;
                    
                    foreach (StatId stat in Enum.GetValues(typeof(StatId)))
                    {
                        stats.Modifier.SetFlat(stat, negativeInitial);
                        intial = 0f;
                    }
                }
                
                foreach (StatId stat in Enum.GetValues(typeof(StatId)))
                {
                    if (GUILayout.Button($"Add 5 Flat To {stat.ToString()}"))
                    {
                        stats.Modifier.SetFlat(stat, 5);
                        intial += 5f;
                    }
                }

                GUILayout.EndScrollView();
            }
            catch
            {
                // ignored
            }
        }


        using (new GUILayout.VerticalScope("box"))
        {
            scrollPosition_ = GUILayout.BeginScrollView(scrollPosition_, GUILayout.MinHeight(240));
            if (showUniqueItems_)
            {
                try
                {
                    foreach (var item in References.TryGetUniqueItems())
                    {
                        if (item == null)
                            continue;
                        var prevColor = GUI.backgroundColor;

                        if (item.Item.Rarity == ItemRarity.Immortal)
                            GUI.backgroundColor = new Color(1f, .45f,0f,1f);
                        
                        if (item.Item.Rarity == ItemRarity.Mythic)
                            GUI.backgroundColor = Color.red;

                        if (item.Item.Rarity == ItemRarity.Rare)
                            GUI.backgroundColor = Color.blue;
                        
                        if (item.Item.Rarity == ItemRarity.Common)
                            GUI.backgroundColor = new Color(.36f, .25f, 0f, 1f);

                        if (item.Item.Rarity == ItemRarity.Epic)
                            GUI.backgroundColor = new Color(0.4f, 0.0f, 0.4f, 1f);

                        if (GUILayout.Button($"Add {item.Item.Code}"))
                        {
                            var backpack = References.TryGetGameManager().ProfileManager.Active.Backpack;
                            backpack.TryAdd(item.Item.Clone(), true);
                        }
                        
                        GUI.backgroundColor = prevColor;
                    }
                }
                catch
                {
                    // ignored
                }
            }
            GUILayout.EndScrollView();
        }
        GUI.DragWindow();
    }


    #region Methods
    private static void UnlockAll()
    {
        var progression = References.TryGetGameManager().ProfileManager.Active.Progression;
        
        if (progression == null)
            return;
        
        foreach (var unlockable in Database.Furniture.All)
            Game.Unlocks.Unlock(unlockable);
        
        foreach (var unlockable in Database.Characters.All)
        {
            progression.MarkCharacterRevealed(unlockable.Code.ToString());
            if (!unlockable.IsUnlockedByDefault)
                Game.Unlocks.Unlock(unlockable);
        }

        foreach (var readOnlyGod in Database.Gods.All)
        {
            if (!readOnlyGod.IsUnlockedByDefault)
                Game.Unlocks.Unlock(readOnlyGod);
        }
        
        Debug.LogWarning("All unlocks have been unlocked!");
    }

    
    /*
     * if (GUILayout.Button("Debug Items List"))
            {
                foreach (var item in Database.ItemUniques.All)
                    Debug.LogWarning(item.Item.Code + ": " + item.Item.SubtypeCode + " - " + item.Item.Class + " - " + item.Item.Rarity);
            }

            if (GUILayout.Button("Give All Immortal Items"))
            {
                foreach (UniqueItemTemplate item in Database.ItemUniques.All)
                {
                    if (item.Item.Rarity == ItemRarity.Immortal)
                        References.TryGetGameManager().ProfileManager.Active.Backpack.TryAdd(item.Item.Clone(), true);
                }
            }
            
            if (GUILayout.Button("Give All Unique Items"))
            {
                foreach (UniqueItemTemplate item in Database.ItemUniques.All)
                {
                    References.TryGetGameManager().ProfileManager.Active.Stashes._stashes[0]._pages.ForEach(slot  => slot.TryAdd(item.Item.Clone()));
                }
            }
     */
    #endregion
}