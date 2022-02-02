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

        if (player.Stone >= 10)
        {
            player.Stone -= 10;
            player.Gold += 7;
            Debug.Log("Thanks!");
        }
    }
}
