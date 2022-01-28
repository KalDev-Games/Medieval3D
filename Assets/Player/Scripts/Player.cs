using UnityEngine;

public class Player : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    private float health;
    [SerializeField]
    private float hunger;


    void Start()
    {
        health = 100;
        hunger = 100;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
