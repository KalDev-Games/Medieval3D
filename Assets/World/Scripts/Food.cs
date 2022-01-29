using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : Ressource
{
    protected enum FoodType
    {
        carot,
        corn
    }
    [SerializeField]
    protected FoodType type;
    [SerializeField]
    protected float regenerationBonus;
    [SerializeField]
    protected float healthBonus;
    [SerializeField]
    protected float saturationBonus;

    public float RegenerationBonus { get => regenerationBonus;}
    public float HealthBonus { get => healthBonus;}
    public float SaturationBonus { get => saturationBonus;}
    public string GetTypeOfFood()
    {
        return type.ToString();
    }
}
