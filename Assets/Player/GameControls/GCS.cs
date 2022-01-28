using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GCS : MonoBehaviour
{
    //Game Control System
    // Start is called before the first frame update

    NonMovementControls controller;
    [Header("Building Mode")]
    [SerializeField]
    private bool enableBuilding;
    [SerializeField]
    private List<GameObject> avaliableObjectsToBuild = new List<GameObject>();
    [SerializeField]
    private int index;

    [Header("Debugging")]
    [SerializeField]
    private float playerRotation;

    private GameObject localObject;
    private Chunk chunk;
    private Quaternion rotation;  
    private int height;
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Debug.Log(Cursor.lockState.ToString());

        
    }

    private void OnEnable()
    {
        controller.NotMovementActions.EnableBuildingMode.Enable();
        controller.NotMovementActions.Switch.Enable();
        controller.NotMovementActions.PlaceObject.Enable();
        controller.NotMovementActions.RotateObject.Enable();
    }

    private void Awake()
    {
        controller = new NonMovementControls();
        controller.NotMovementActions.EnableBuildingMode.performed += EnableBuildingMode;
        controller.NotMovementActions.Switch.performed += ChangeIndex;
        controller.NotMovementActions.PlaceObject.performed += Build;
        controller.NotMovementActions.RotateObject.performed += RotateObject;
    }


    private void RotateObject(InputAction.CallbackContext ctx)
    {
        rotation = Quaternion.AngleAxis(90, Vector3.up);
    }

    private void Build(InputAction.CallbackContext ctx)
    {
        if (enableBuilding && height <= WorldGenerator.worldHeight)
        {
            Destroy(localObject);
            localObject = null;
            
            chunk.SetTypeOfObject(height,index + 2);
            chunk.SetRotationOfLayer(rotation, height);
            Debug.LogWarning(chunk.GetPosOfChunk());
            WorldGenerator.RefreshChunk((int)chunk.GetPosOfChunk().x, (int)chunk.GetPosOfChunk().y);
        }
    }

    private void ChangeIndex(InputAction.CallbackContext ctx)
    {
        float z = controller.NotMovementActions.Switch.ReadValue<float>();
        if (z > 0)
        {
            index++;

            if (index >= avaliableObjectsToBuild.Count)
            {
                index = 0;
            }
        }
        else if (z < 0)
        {
            index--;

            if (index <= 0)
            {
                index = avaliableObjectsToBuild.Count - 1;
            }
        }
            
    }

    private void EnableBuildingMode(InputAction.CallbackContext ctx)
    {
        enableBuilding = !enableBuilding;

        avaliableObjectsToBuild.Clear();
        for (int i = 2; i < WorldGenerator.sWorldTiles.Count; i++)
        {
            avaliableObjectsToBuild.Add(WorldGenerator.sWorldTiles[i]);
        }

        if (localObject != null)
        {
            Destroy(localObject);
            localObject = null;
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        if (enableBuilding)
        {
            //Calculate Chunk
            int playerXPos = (int)(transform.position.x / WorldGenerator.offsetXZ);
            int playerYPos = (int)(transform.position.z / WorldGenerator.offsetXZ);

            playerRotation = Mathf.Abs(transform.rotation.eulerAngles.y) % 360;
            int median = WorldGenerator.GetWorldSizeMedian();
            try
            {
                if ((playerRotation < 45 && playerRotation > 0) || playerRotation >= 315)
                {
                    Debug.LogWarning("Schaue nach vorn");
                    chunk = WorldGenerator.model[playerXPos + median, playerYPos + median + 1];
                    MoveChunks(chunk);
                }
                else if (playerRotation >= 45 && playerRotation < 135)
                {
                    Debug.LogWarning("Schaue nach links");
                    chunk = WorldGenerator.model[playerXPos + median + 1, playerYPos + median];
                    MoveChunks(chunk);
                }
                else if (playerRotation >= 135 && playerRotation < 225)
                {
                    Debug.LogWarning("Schaue nach hinten");
                    chunk = WorldGenerator.model[playerXPos + median, playerYPos + median - 1];
                    MoveChunks(chunk);
                }
                else if (playerRotation >= 225 && playerRotation < 315)
                {
                    Debug.LogWarning("Schaue nach rechts");
                    chunk = WorldGenerator.model[playerXPos + median - 1, playerYPos + median];
                    MoveChunks(chunk);
                }
                //lastPos = new Vector2(playerXPos, playerYPos);
            }
            catch (System.IndexOutOfRangeException)
            {
                throw;
            }

           

            

        }
    }

    private void MoveChunks(Chunk chunk)
    {
        
        for (height = 0; height < chunk.getAllTypes().Length; height++)
        {
            if (chunk.getAllTypes()[height] == 0)
            {
                if (localObject == null)
                {
                    localObject = Instantiate(avaliableObjectsToBuild[index],
                    new Vector3(chunk.GetPosOfChunk().x, height * WorldGenerator.offsetY - WorldGenerator.offsetY, chunk.GetPosOfChunk().y),
                    Quaternion.identity);
                }
                else
                {
                    localObject.transform.position = new Vector3(chunk.GetPosOfChunk().x, height * WorldGenerator.offsetY - WorldGenerator.offsetY, chunk.GetPosOfChunk().y);
                }
                break;
            }
        }
    }
}
