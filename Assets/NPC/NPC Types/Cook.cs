using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cook : NPC
{
    Cook()
    {
        job = NPCJobs.npcJobs.cook;
    }

    public override void Trade(Player player)
    {
        Debug.Log("Hello I'm a cook. I want your meat!");
    }
}
