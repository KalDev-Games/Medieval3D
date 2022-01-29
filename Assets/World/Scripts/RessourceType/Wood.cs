using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wood : Materials
{
    Wood()
    {
        ressourceTyp = ressourceType.wood;
        hitsNecessary = Random.Range(7, 10);
        amountOfRessources = Random.Range(1, 5);
    }
}
