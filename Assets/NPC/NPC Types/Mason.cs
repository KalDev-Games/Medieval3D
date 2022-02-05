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

        if (player.HowManyRessources(Ressource.ressourceType.rock) >= needsAmount)
        {
            foreach (var item in player.Inventory)
            {
                if (item.RessourceTypeOfObject == Ressource.ressourceType.rock)
                {
                    player.Inventory.Remove(item);
                    needsAmount--;
                }

                if (needsAmount == 0)
                {
                    break;
                }
            }
            for (int i = 0; i < reward; i++)
            {
                player.Inventory.Add(new Gold());
            }

        }

        

        
    }
}
