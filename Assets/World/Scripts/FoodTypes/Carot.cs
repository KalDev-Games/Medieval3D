using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Carot : Food
{
    // Start is called before the first frame update
    Carot()
    {
        type = FoodType.carot;
        saturationBonus = 2;
        regenerationBonus = 2;
        healthBonus = 2;
    }
}
