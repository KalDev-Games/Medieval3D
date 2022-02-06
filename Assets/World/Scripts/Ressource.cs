using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ressource 
{
    // Start is called before the first frame update
    public enum ressourceType
    {
        wood,
        rock,
        gold
    }

    public IdInfo id;
    protected int stackLimit;
    protected ressourceType ressourceTypeOfObject;

    public ressourceType RessourceTypeOfObject { get => ressourceTypeOfObject; set => ressourceTypeOfObject = value; }

    public Ressource DeepCopy()
    {
        Ressource other = (Ressource)this.MemberwiseClone();
        other.id = new IdInfo(id.Id);
        return other;
    }



}

public class IdInfo
{
    public int Id;

    public IdInfo(int id)
    {
        Id = id;
    }
}


