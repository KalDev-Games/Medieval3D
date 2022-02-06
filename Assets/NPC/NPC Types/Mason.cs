using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mason : NPC
{
    Mason()
    {
        job = NPCJobs.npcJobs.mason;
    }

    public override void Trade(Player player)
    {
        Debug.Log("Hello I'm a mason. I want your rocks!");
        int needsAmount = 10;
        int reward = 7;
        Debug.Log(player.Inventory.InventoryHelper.Count);

        if (player.HowManyRessources<Rock>() >= needsAmount)
        {
            List<Ressource> ressources = new List<Ressource>();

            foreach (var item in player.Inventory.InventoryHelper)
            {
                if (item.GetType() == typeof(Rock) && needsAmount > 0)
                {
                    needsAmount--;
                }
                else
                {
                    ressources.Add(item);
                }
            }

            player.Inventory.InventoryHelper = ressources;
            
            for (int i = 0; i < reward; i++)
            {
                player.Inventory.InventoryHelper.Add(new Gold());
            }

        }
        Debug.Log(player.Inventory.InventoryHelper.Count);



    }
}
