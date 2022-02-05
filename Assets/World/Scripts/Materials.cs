using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Materials : Ressource
{
    
    protected int hitsTaken = 0;
    protected int hitsNecessary;
    protected int amountOfRessources;
    protected ressourceType ressourceTyp;

    public int HitsNecessary { get => hitsNecessary; }
    public int AmountOfRessources { get => amountOfRessources;}
    public string RessourceTyp { get => ressourceTyp.ToString();}
    public int HitsTaken { get => hitsTaken; set => hitsTaken = value; }
}
