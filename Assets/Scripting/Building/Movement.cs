using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Movement : MonoBehaviour
{
    // Start is called before the first frame update
    private float sensitivityOfMovement;
    private float sensitivityOfLooking;
    public GameObject prefabToPlace;
    private GameObject prefabInHand = null;
    public bool buildingMode = false;

    // Bit shift the index of the layer (8) to get a bit mask
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Mouse.current.rightButton.isPressed)
        {
            buildingMode = !buildingMode;
            if (!buildingMode)
            {
                prefabInHand = null;
            }
        }

        if(buildingMode && prefabInHand != null)
        {
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (Physics.Raycast(ray, out RaycastHit hit, 100) && Mouse.current.leftButton.isPressed)
            {
                Instantiate(prefabInHand, hit.transform.position, Quaternion.identity);
            }
        }
        else if (buildingMode && prefabInHand == null)
        {
            prefabInHand = prefabToPlace;
        }

       
    }
}
