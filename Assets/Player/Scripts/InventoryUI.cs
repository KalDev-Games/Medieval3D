using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private Text displayAmountWood;
    [SerializeField] private Text displayAmountRocks;
    [SerializeField] private Text displayAmountGold;

    public Text DisplayAmountWood { get => displayAmountWood; set => displayAmountWood = value; }
    public Text DisplayAmountRocks { get => displayAmountRocks; set => displayAmountRocks = value; }
    public Text DisplayAmountGold { get => displayAmountGold; set => displayAmountGold = value; }

}
