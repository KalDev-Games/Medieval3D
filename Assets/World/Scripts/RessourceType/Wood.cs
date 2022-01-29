using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wood : Materials
{
    Wood()
    {
        System.Random r = new System.Random();
        ressourceTyp = ressourceType.wood;
        hitsNecessary = r.Next(3,5);
        amountOfRessources = r.Next(3, 5);
    }
}
