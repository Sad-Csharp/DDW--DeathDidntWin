using System;
using System.Linq;
using Death;
using Death.App;
using Death.App.UserInterface.Cursors;
using Death.Data;
using Death.Dialogues;
using Death.Dialogues.Core;
using Death.Dialogues.Presentation.BubbleDialogue;
using Death.Items;
using Death.Run.Behaviours;
using Death.Run.Behaviours.Entities;
using Death.Run.Core;
using Death.Unlockables;
using UnityEngine;
using DeathDidntWin.Miscellanious;
using HarmonyLib;

namespace DeathDidntWin;

public static class DDWGui
{
    #region Fields

    public static bool IsShowingItemWindow;
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

            var prevColor = GUI.backgroundColor;
            GUI.backgroundColor = CursorManager.ForceOsCursor ? Color.green : Color.red;
            if (GUILayout.Button("Force Os Cursor"))
            {
                var flag = !CursorManager.ForceOsCursor;
                CursorManager.ForceOsCursor = flag;
            }
            GUI.backgroundColor = prevColor;
            
            if (GUILayout.Button("Close Game"))
            {
                Application.Quit();
            }

            if (GUILayout.Button("Uncap FPS"))
            {
                Application.targetFrameRate = 1000;
            }

            if (GUILayout.Button("Debug Chat Bubble"))
            {
                PopMessage("Debug Chat Bubble");
            }
        }

        using (new GUILayout.VerticalScope("box"))
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Cheats");
            GUILayout.EndHorizontal();
            var player = Player.Instance;
            
            IsShowingItemWindow = GUILayout.Toggle(IsShowingItemWindow, "Show Item Spawner");

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

            if (GUILayout.Button("Add 1.25k Exp"))
            {
                if (References.TryGetGameManager() == null)
                    return;

                var xpTrackerComponent = Player.Instance.gameObject.GetComponent<Behaviour_XpTracker>();
                if (xpTrackerComponent == null)
                    return;
                
                xpTrackerComponent.GainXp(1250F);
                Debug.LogWarning("Added 1.25k Exp!");
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
            GUILayout.Label("<b>Stats Modifier</b>");
            try
            {
                var player = Player.Instance;
                scrollPosition_2 = GUILayout.BeginScrollView(scrollPosition_2, GUILayout.MinHeight(120));
                if (player == null)
                    return;

                var stats = player.Entity.Stats;
                if (stats == null)
                    return;
                
                foreach (StatId stat in Enum.GetValues(typeof(StatId)))
                {
                    if (GUILayout.Button($"Add 5 Flat To {stat.ToString()}"))
                    {
                        stats.Modifier.AddFlat(stat, 5);
                    }
                }
                GUILayout.EndScrollView();
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error in stats modifer scrollview: {ex.Message}");
            }
        }
        
        GUI.DragWindow();
    }

    public static void DrawItems(int windowID)
    {
        GUILayout.Label("<b>Item's Will Go In Inventory!</b>");
        using (new GUILayout.VerticalScope("box"))
        {
            scrollPosition_ = GUILayout.BeginScrollView(scrollPosition_, GUILayout.MinHeight(240));
            try
            {
                var uniqueItems = References.TryGetUniqueItems().OrderBy(item => item.Item.Rarity).ThenBy(item => item.Item.Code);

                foreach (var item in uniqueItems)
                {
                    var prevColor = GUI.backgroundColor;
                    switch (item.Item.Rarity)
                    {
                        case ItemRarity.Common:
                            GUI.backgroundColor = Color.white;
                            break;
                        case ItemRarity.Rare:
                            GUI.backgroundColor = Color.blue;
                            break;
                        case ItemRarity.Epic:
                            GUI.backgroundColor = Color.magenta;
                            break;
                        case ItemRarity.Mythic:
                            GUI.backgroundColor = Color.red;
                            break;
                        case ItemRarity.Immortal:
                            GUI.backgroundColor = Color.yellow;
                            break;
                    }
                        
                    if (GUILayout.Button($"Add {item.Item.Code}"))
                    {
                        var backpack = References.TryGetGameManager().ProfileManager.Active.Backpack;
                        backpack.TryAdd(item.Item.Clone(), true);
                    }
                    GUI.backgroundColor = prevColor;
                }
            }
            catch(Exception ex)
            {
                Debug.LogError($"Error in item scrollview: {ex.Message}");
                throw;
            }
            GUILayout.EndScrollView();
        }
    }


    #region Methods

    private static void PopMessage(string message)
    {
        string[] messages = [message];
        DialogueSpeaker speaker = Player.Instance.Speaker;
        Dialogue dialogue = new Dialogue(Dialogue.Types.NonInterrupting);
        dialogue.Lines.Add(Line.Say(speaker.SpeakerId, messages, null));
        BubbleDialogueContext context = DialogueSpeakerManager.GenerateContext();
        context.AddInitialSpeaker(speaker);
    }
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