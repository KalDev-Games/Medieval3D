using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory
{
    // Start is called before the first frame update
    private List<Ressource> inventory = new List<Ressource> ();

    public List<Ressource> InventoryHelper { get => inventory; set => inventory = value; }

    public Inventory()
    {
        inventory = new List<Ressource> ();
    }
}
