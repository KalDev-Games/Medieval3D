using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGenerator : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    private Transform player;
    [SerializeField]
    private List<GameObject> worldTiles;
    [SerializeField]
    private int worldSize = 256;
    [SerializeField]
    private int worldHeight = 16;
    [SerializeField]
    private int viewDistance = 8;

    //The model of the world
    public static GameObject[,,] world;

    //current active chunks
    private List<GameObject> seenTiles = new List<GameObject>();

    private int lastX = 0;
    private int lastY = 0;

    public float differenceX;
    public float differenceY;


    void Start()
    {
        int median = worldSize / 2;
        world = new GameObject[worldSize,worldHeight,worldSize];
        for (int x = -median; x < median; x++)
        {
            for (int y = -median; y < median; y++)
            {
                for (int z = 0; z < worldHeight; z++)
                {
                    if (z > 0)
                    {
                       
                        GameObject go = new GameObject();
                        go.transform.position = new Vector3(x * 8, 8 * z - 8, y * 8);
                        go.transform.SetParent(transform);

                        go.isStatic = true;
                        world[x + median, z, y + median] = go;
                    } else
                    {
                        GameObject go = Instantiate(worldTiles[0], new Vector3(x * 8, -8, y * 8), Quaternion.identity, transform);
                        go.isStatic = true;
                        world[x + median, z, y + median] = go;
                    }
                    
                }



                
              
                
                
            }
        }
        UpdateChunks(0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        int playerX = (int)player.position.x;
        int playerY = (int)player.position.z;

        differenceX = Mathf.Abs(lastX - playerX);
        differenceY = Mathf.Abs(lastY - playerY);
        if (Mathf.Abs(lastX - playerX)>=8 || Mathf.Abs(lastY - playerY) >= 8)
        {
            UpdateChunks(playerX, playerY);
            lastX = playerX;
            lastY = playerY;
        }
        
        
    }

    private void UpdateChunks(int playerX, int playerY)
    {
        int median = worldSize / 2;
        List<GameObject> newTiles = new List<GameObject>();
        try
        {
            for (int i = -viewDistance; i < viewDistance; i++)
            {
                for (int j = -viewDistance; j < viewDistance; j++)
                {
                    for (int k = 0; k < worldHeight; k++)
                    {
                        int x = playerX / 8 + i;
                        int y = playerY / 8 + j;

                        GameObject tile = world[x + median, k, y + median];
                        newTiles.Add(tile);
                    }
                    
                }
            }
        }
        catch (System.IndexOutOfRangeException)
        {
            Debug.Log("No field to find");
        }

        

        foreach (var item in seenTiles)
        {
            if (!newTiles.Contains(item))
            {
                item.SetActive(false);

            }
        }

        seenTiles = newTiles;

        foreach (var item in seenTiles)
        {
            item.SetActive(true);
        }
    }
}
