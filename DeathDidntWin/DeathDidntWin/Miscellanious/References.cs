using System.Collections.Generic;
using System.Linq;
using Death.App;
using Death.Data;
using Death.Items;
using UnityEngine;
using Object = UnityEngine.Object;

namespace DeathDidntWin.Miscellanious;

public static class References
{
    private static readonly string[] characters_ = ["Assassin_Fab(Clone)", "Ranger_Fab(Clone)", "Knight_Fab(Clone)", "Barbarian_Fab(Clone)", "Sorceress_Fab(Clone)", "Warrior_Fab(Clone)"];
    private static GameManager gameManager_;
    private static GameObject player_;
    private static UniqueItemTemplate[] uniqueItems_;

    public static GameManager TryGetGameManager()
    { 
        if (gameManager_ != null)
            return gameManager_;

        gameManager_ = Object.FindObjectOfType<GameManager>();
    
        if (gameManager_ == null)
        {
            Debug.LogWarning("Unable to find game manager!");
            return gameManager_ = null;
        }
        Debug.LogWarning("Game manager found: " + gameManager_.name);
        return gameManager_;
    }

    public static GameObject TryGetPlayerGameObject()
    {
        if (player_ != null)
            return player_;

        foreach (var playerName in characters_)
        {
            player_ = GameObject.Find(playerName);
            if (playerName == null) 
                continue;
                
            Debug.LogWarning($"Player object found: {player_.name}");
            return player_;
        }
                
        Debug.LogWarning("Unable to find player object!");
        return player_ = null;
    }

    public static UniqueItemTemplate[] TryGetUniqueItems()
    {
        if (uniqueItems_ != null)
            return uniqueItems_;
            
        uniqueItems_ = Database.ItemUniques.All.ToArray();
        if (uniqueItems_ == null)
        {
            Debug.LogWarning("Unable to get unique items!");
            return uniqueItems_ = null;
        }
        Debug.LogWarning("Unique items found: " + uniqueItems_.Length);
        return uniqueItems_;
    }
}