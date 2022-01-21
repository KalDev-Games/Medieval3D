using System.Collections;
using System.Collections.Generic;
using System.Threading;
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
    public static int worldHeight = 8;
    [SerializeField]
    //Stufenweise
    private static int viewDistance = 8;

    //The model of the world
    public static GameObject[,,] world;
    public static Chunk[,] model;

    //current active chunks
    private List<Chunk> seenChunks = new List<Chunk>();

    private static int lastX = 0;
    private static int lastY = 0;

    

    [Header("Debugging")]
    [SerializeField]
    private List<string> updatedTiles = new List<string>();
    [SerializeField]
    private float differenceX;
    [SerializeField]
    private float differenceY;
    [SerializeField]
    private int playerX;
    [SerializeField]
    private int playerY;



    private Thread thread;

    void Start()
    {
        

        int median = worldSize / 2;
       
        model = new Chunk[worldSize, worldSize];

        for (int x = 0; x < worldSize; x++)
        {
            for (int y = 0; y < worldSize; y++)
            {
                model[x, y] = new Chunk();
                model[x, y].SetCoordinates(x, y);
            }  
        }



        for (int x = -median; x < median; x++)
        {
            for (int y = -median; y < median; y++)
            { 
                model[x + median, y + median].SetCoordinates(x, y);
                for (int z = 0; z < worldHeight; z++)
                {
                   
                    if (z > 0)
                    {
                        int random = Random.Range(0, 50);
                        if (random == 0)
                        {
                            model[x + median, y + median].SetTypeOfObject(z,0);
                        }
                        else if (random > 1 && random < 7 && z == 1)
                        {
                            model[x + median, y + median].SetTypeOfObject(z, random);
                        }
                        else if (random > 1 && random < 7 && z > 1 && model[x + median, y + median].getAllTypes()[z-1] > 1)
                        {
                            model[x + median, y + median].SetTypeOfObject(z, random);
                        }


                        
                    }
                    else
                    {
                        model[x + median, y + median].SetTypeOfObject(z, 1);

                    }
                }
            }
        }

        UpdateChunks(0, 0);
    }

    public Thread StartTheThread(int param1, int param2)
    {
        var t = new Thread(() => UpdateChunks(param1, param2));
        t.Start();
        return t;
    }

  



    // Update is called once per frame
    void Update()
    {
        int playerX = (int)player.position.x;
        int playerY = (int)player.position.z; 

        if (Mathf.Abs(lastX - playerX)>=8 || Mathf.Abs(lastY - playerY) >= 8)
        {
            StartCoroutine(RunUpdate(playerX, playerY));
            lastX = playerX;
            lastY = playerY;
        }
        
    }


    IEnumerator RunUpdate(int x, int y)
    {
        UpdateChunks (x,y);
        yield return null;
    }

    private void UpdateChunks(int playerX, int playerY)
    {
        int median = worldSize / 2;
        List<Chunk> newChunks = new List<Chunk>();
        int viewDistance = WorldGenerator.viewDistance * 2;

        int chunkX;
        int chunkY;

        //Check which chunks are need to be seen
        try
        {
            for (int x = -viewDistance; x <= viewDistance; x++)
            {
                for (int y = -viewDistance; y <= viewDistance; y++)
                {
                    chunkX = playerX / 8 + x;
                    chunkY = playerY / 8 + y;
                    newChunks.Add(model[chunkX + median, chunkY + median]); //all visbible chunks are in this list
                    
                }
            }
        }
        catch (System.IndexOutOfRangeException)
        {
            throw;
        }

        //Destroy chunks which are not visible anymore
        int length = seenChunks.Count;
        for (int i = 0; i < length; i++)
        {
            if (!newChunks.Contains(seenChunks[i]))
            {
                for (int j = 0; j < seenChunks[i].GetAllObjectsOfChunk().Length; j++)
                {
                    Destroy(seenChunks[i].GetAllObjectsOfChunk()[j].gameObject);
                }
            }
        }

        //Spawn new chunks
        length = newChunks.Count;
        for (int i = 0; i < length; i++)
        {
            for (int j = 0; j < newChunks[i].GetAllObjectsOfChunk().Length; j++)
            {
                if (!seenChunks.Contains(newChunks[i]))
                {
                    Vector2 pos = newChunks[i].GetCoordinates();
                    GameObject go = Instantiate(SpawnObject(newChunks[i].getAllTypes()[j]), new Vector3(pos.x * 8, j * 8 - 8, pos.y * 8), Quaternion.identity);
                    newChunks[i].AddTileToChunk(go, j);
                }
                
            }
        }

        seenChunks = newChunks;

    }    

    private GameObject SpawnObject(int type)
    {
        GameObject tile;
        switch (type)
        {
            case 0:
                tile = worldTiles[0];
                break;
            case 1:
                tile = worldTiles[1];
                break;
            case 2:
                tile = worldTiles[2];
                break;
            case 3:
                tile = worldTiles[3];
                break;
            case 4:
                tile = worldTiles[4];
                break;
            case 5:
                tile = worldTiles[5];
                break;
            case 6:
                tile = worldTiles[6];
                break;
            default:
                return worldTiles[0];
        }
        return tile;
    }

    
}
