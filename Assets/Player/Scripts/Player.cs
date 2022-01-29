using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    private float health;
    [SerializeField]
    private float hunger;
    [SerializeField]
    private float endurance;

    [SerializeField]
    private float starvationRate = 1f;
    [SerializeField]
    private float hungerDamage = 0.5f;
    [SerializeField]
    private float enduranceMinus = 0.5f;
    [SerializeField]
    private AnimationCurve regeneration;

    [SerializeField]
    private float rangeForRay = 2;

    public float Endurance { get => endurance; set => endurance = value; }
    public float EnduranceMinus { get => enduranceMinus;}
    public AnimationCurve Regeneration { get => regeneration; }
    public float Hunger { get => hunger;}

    private NonMovementControls controls;

    [SerializeField]
    private Food food;
    private GameObject obj;

    private void Awake()
    {
        controls = new NonMovementControls();
        controls.NotMovementActions.Eat.performed += Eat;

    }

    void OnEnable()
    {
        controls.NotMovementActions.Eat.Enable();
    }

    void Start()
    {
        health = 100;
        hunger = 100;
        Endurance = 100;
    }



    // Update is called once per frame
    void Update()
    {
        hunger -= starvationRate * Time.deltaTime;

        if (Hunger <= 0)
        {
            hunger = 0;
            health -= hungerDamage * Time.deltaTime;
        }


        RaycastHit hit;
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.rotation * Vector3.forward * rangeForRay);
        Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.rotation * Vector3.forward * rangeForRay, Color.black);
        Debug.Log("Ray");
     
        //Debug.DrawRay(ray,Color.red);

        if (Physics.Raycast(ray, out hit))
        {
            
            Debug.Log(hit.transform.name);

            if (hit.transform.GetComponent<Food>())
            {
                Debug.Log(hit.transform.GetComponent<Food>().GetTypeOfFood());
                food = hit.transform.GetComponent<Food>();
                obj = hit.transform.gameObject;
            }
            else
            {
                food = null;
                obj = null;
            }
            
          

            // Do something with the object that was hit by the raycast.
        }

    }

    private void Eat(InputAction.CallbackContext ctx)
    {
        if (food != null)
        {
            hunger += food.SaturationBonus;
            health += food.HealthBonus;
            endurance += food.RegenerationBonus;

            Destroy(obj);
            food = null;
        }
    }
}
