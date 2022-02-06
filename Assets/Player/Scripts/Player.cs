using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
    public static int LastUI { get => lastUI; set => lastUI = value; }
    public List<Ressource> Inventory { get => inventory; set => inventory = value; }

    private NonMovementControls controls;

    [Header("Food & Ressource")]
    [SerializeField]
    private Food food;
    [SerializeField]
    private Materials material;
    [SerializeField]
    private GameObject obj;

    [Header("Inventory")]
    [SerializeField]
    private List<Ressource> inventory;

    [Header("GUI")]
    [SerializeField]
    private Slider sliderHealth;
    [SerializeField]
    private Slider sliderHunger;
    [SerializeField]
    private Slider sliderEndurance;

    [Header("Position")]
    [SerializeField]
    private Vector2 currentPosition;
    [SerializeField]
    private Vector2 currentChunk;

    [Header("UI")]
    private static int lastUI;
    [SerializeField]
    private List<GameObject> otherUI;
    [SerializeField]
    private GameObject pauseUI;
    
    [Header("NPC")]
    [SerializeField]
    private NPC npc;

    private void Awake()
    {
        controls = new NonMovementControls();
        controls.NotMovementActions.Eat.performed += Eat;
        controls.WorldInteraction.GetRessource.performed += Interact;

        controls.Misc.PauseGame.performed += PauseGame;

    }

    void OnEnable()
    {
        controls.NotMovementActions.Eat.Enable();
        controls.WorldInteraction.GetRessource.Enable();
        controls.Misc.PauseGame.Enable();
    }

    void Start()
    {
        health = 100;
        hunger = 100;
        Endurance = 100;
    }



    // Update is called once per frame
    void FixedUpdate()
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
     
        //Debug.DrawRay(ray,Color.red);

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.transform.GetComponent<Loot>())
            {
                try
                {
                    Debug.Log(hit.transform.name);
                    Debug.Log(hit.transform.GetComponent<Loot>().ressource.GetType());
                }
                catch (Exception)
                {
                    hit.transform.name += " error";                    
                }
                
                if (hit.transform.GetComponent<Loot>().ressource.GetType().Equals(typeof(Food)))
                {
                    food = (Food)hit.transform.GetComponent<Loot>().ressource;
                    obj = hit.transform.gameObject;
                }
                else if (hit.transform.GetComponent<Loot>().ressource.GetType().Equals(typeof(Materials)))
                {
                    material = (Materials)hit.transform.GetComponent<Loot>().ressource;
                    obj = hit.transform.gameObject;
                }
                else if (hit.transform.GetComponent<NPC>())
                {
                    npc = hit.transform.GetComponent<NPC>();
                }
                else
                {
                    food = null;
                    material = null;
                    npc = null;
                    obj = null;
                }
            }

            // Do something with the object that was hit by the raycast.
        }

    }

    private void LateUpdate()
    {
        sliderHealth.value = health;
        sliderEndurance.value = endurance;
        sliderHunger.value = hunger;

        currentPosition = new Vector2(transform.position.x, transform.position.z);
        currentChunk = new Vector2(transform.position.x / 8, transform.position.z/ 8);
    }


    private void PauseGame(InputAction.CallbackContext ctx)
    {
        Time.timeScale = 0;
        foreach (var item in otherUI)
        {
            item.SetActive(false);
        }
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.lockState = CursorLockMode.None;
        pauseUI.SetActive(true);
    }

    public void EndGame()
    {
        SceneManager.LoadScene(0);
    }

    public void ResumeGame()
    {
        Cursor.lockState= CursorLockMode.Locked;
        Time.timeScale = 1;
        pauseUI.SetActive(false);
        foreach (var item in otherUI)
        {
            item.SetActive(false);
        }
        otherUI[lastUI].SetActive(true);
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

    private void Interact(InputAction.CallbackContext ctx)
    {
        #region ressources
        if (material != null && material.HitsTaken < material.HitsNecessary)
        {
            material.HitsTaken++;
            Debug.Log("Hit");

        } else if (material != null && material.HitsTaken == material.HitsNecessary)
        {
            material.id = new IdInfo(0);

            Materials newMat = (Materials)Activator.CreateInstance(material.GetType());

            //Ressource rsc = material.DeepCopy();

            //Debug.Log(rsc.GetType());
            inventory.Add(newMat);

            Destroy(obj);
            material = null;
        }
        #endregion

        if (npc != null)
        {
            npc.Trade(this);
        }
    }


    public int HowManyRessources(Materials.ressourceType whatKind)
    {
        int counter = 0;
        

        for (int i = 0; i < inventory.Count; i++)
        {
            if (inventory[i].Equals(whatKind))
            {
                counter++;
            }
        }

        return counter;
    }
}
