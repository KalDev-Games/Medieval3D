using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Corn : Food
{
    public Corn()
    {
        type = FoodType.corn;
        saturationBonus = 5;
        regenerationBonus = 5;
        healthBonus = 5;
    }

}
