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
    public static List<GameObject> sWorldTiles;
    [SerializeField]
    private List<GameObject> treePrefabs;
    private static List<GameObject> sTreePrefabs;
    [SerializeField]
    private static int worldSize = 256;
    public static int worldHeight = 8;
    [SerializeField]

    public static int offsetXZ = 8;
    public static int offsetY = 8;

    //Stufenweise
    private static int viewDistance = 8;

    //The model of the world
    public static Chunk[,] model;

    //current active chunks
    private List<Chunk> seenChunks = new List<Chunk>();

    private static int lastX = 0;
    private static int lastY = 0;

    private enum MyEnum
    {
        offline,
        sd
    }
    

    [Header("Debugging")]
    [SerializeField]
    private float differenceX;
    [SerializeField]
    private float differenceY;
    [SerializeField]
    private int playerX;
    [SerializeField]
    private int playerY;


    public static int GetWorldSizeMedian()
    {
        return worldSize / 2;
    }

    void Start()
    {
        
        sTreePrefabs = treePrefabs;
        sWorldTiles = worldTiles;

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
                        int randomTrees = Random.Range(0, 3);
                        int randomAmount = Random.Range(5, 20);
                        int shouldThereBeTrees = Random.Range(0, 100);

                        if ((random == 0 || random >= 7) && z == 0 && randomTrees <= 3)
                        {
                            model[x + median, y + median].SetTypeOfObject(z,0);
                        }
                        else if (random == 0 && z == 0)
                        {
                            
                            model[x + median, y + median].SetTypeOfObject(z, 0);
                        }
                        else if (random > 1 && random < 7 && z == 1)
                        {
                            model[x + median, y + median].SetTypeOfObject(z, random);
                        }
                        else if (random > 1 && random < 7 && z > 1 && model[x + median, y + median].getAllTypes()[z-1] > 1)
                        {
                            model[x + median, y + median].SetTypeOfObject(z, random);
                        }
                        else if(z== 1 && shouldThereBeTrees % 20 == 0)
                        {
                            for (int i = 0; i < randomAmount; i++)
                            {
                                int xPos = Random.Range(-4, 4);
                                int yPos = Random.Range(-4, 4);
                                model[x + median, y + median].SetUpNewProp(randomTrees, new Vector2(xPos + x, yPos + y), 0);
                            }
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

    

    public static void RefreshChunk(int x, int y)
    {
        Chunk chunk = model[x + GetWorldSizeMedian(), y + GetWorldSizeMedian()];
        Debug.LogWarning(chunk.GetPosOfChunk());

        for (int i = 0; i < chunk.GetAllObjectsOfChunk().Length; i++)
        {
            chunk.GetAllObjectsOfChunk()[i].SetActive(false);
        }

        //Destroy
        int length = chunk.GetAllPropObjects().Count;
        for (int j = 0; j < length; j++)
        {
            Destroy(chunk.GetAllPropObjects()[j].gameObject);
        }

        length = chunk.GetAllObjectsOfChunk().Length;
        for (int j = 0; j < length; j++)
        {
            Destroy(chunk.GetAllObjectsOfChunk()[j].gameObject);
        }

        //Rebuild
        for (int j = 0; j < chunk.GetAllObjectsOfChunk().Length; j++)
        {
            Vector2 pos = chunk.GetCoordinates();
            GameObject go = Instantiate(SpawnObject(chunk.getAllTypes()[j]),
                new Vector3(pos.x * 8, j * 8 - 8, pos.y * 8),
                Quaternion.identity);
            go.isStatic = true;
            chunk.AddTileToChunk(go, j);

        }

        for (int j = 0; j < chunk.GetTypesOfProp().Count; j++)
        {
            Vector2 pos = chunk.GetPositionsOfProps()[j];
            GameObject go = Instantiate(SpawnTree(chunk.GetTypesOfProp()[j]),
                new Vector3(pos.x * 8, 0, pos.y * 8),
                Quaternion.identity);
            go.isStatic = true;
            chunk.AddPropObject(go);
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

                for (int j = 0; j < seenChunks[i].GetAllPropObjects().Count; j++)
                {
                    Destroy(seenChunks[i].GetAllPropObjects()[j].gameObject);
                }

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
            if (!seenChunks.Contains(newChunks[i]))
            {
                for (int j = 0; j < newChunks[i].GetAllObjectsOfChunk().Length; j++)
                {
                    Vector2 pos = newChunks[i].GetCoordinates();
                    GameObject go = Instantiate(SpawnObject(newChunks[i].getAllTypes()[j]), 
                        new Vector3(pos.x * 8, j * 8 - 8, pos.y * 8), 
                        Quaternion.identity);
                    go.isStatic = true;
                    newChunks[i].AddTileToChunk(go, j);

                }

                for (int j = 0; j < newChunks[i].GetTypesOfProp().Count; j++)
                {
                    Vector2 pos = newChunks[i].GetPositionsOfProps()[j];
                    GameObject go = Instantiate(SpawnTree(newChunks[i].GetTypesOfProp()[j]),
                        new Vector3(pos.x * 8, 0, pos.y * 8), 
                        Quaternion.identity);
                    go.isStatic = true;
                    newChunks[i].AddPropObject(go);
                }


            }

        }

        seenChunks = newChunks;

    }    

    private static GameObject SpawnObject(int type)
    {
        GameObject tile;
        switch (type)
        {
            case 0:
                tile = sWorldTiles[0];
                break;
            case 1:
                tile = sWorldTiles[1];
                break;
            case 2:
                tile = sWorldTiles[2];
                break;
            case 3:
                tile = sWorldTiles[3];
                break;
            case 4:
                tile = sWorldTiles[4];
                break;
            case 5:
                tile = sWorldTiles[5];
                break;
            case 6:
                tile = sWorldTiles[6];
                break;
            default:
                return sWorldTiles[0];
        }
        return tile;
    }


    private static GameObject SpawnTree(int id)
    {
        switch (id)
        {
            case 0:
                return sTreePrefabs[0];
            case 1:
                return sTreePrefabs[1];
            case 2:
                return sTreePrefabs[2];
            case 3:
                return sTreePrefabs[3];
            default:
                return sTreePrefabs[0];
        }
    }
    
}
