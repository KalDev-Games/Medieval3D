using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class WorldGenerator : MonoBehaviour
{
    //Change this for changing tree occurences
    [Header("Trees and Ressources")]
    [SerializeField]
    private const int maxTrees = 10;
    [SerializeField]
    private const int minTrees = 3;
    [SerializeField]
    private const int treeOrNatureElement = 90; //The higher the more and trees and less rocks

    // Start is called before the first frame update
    [SerializeField]
    private Transform player;
    [SerializeField]
    private List<GameObject> worldTiles;
    public static List<GameObject> sWorldTiles;
    [SerializeField]
    private List<GameObject> propPrefabs;
    private static List<GameObject> sPropPrefabs;
    [SerializeField]
    private List<GameObject> natureElementsPrefabs;
    private static List<GameObject> sNatureElementsPrefabs;
    [SerializeField]
    private static int worldSize = 256;
    public static int worldHeight = 8;
    [SerializeField]

    public static int offsetXZ = 8;
    public static int offsetY = 8;

    //Stufenweise
    private static int viewDistance = 8;
    private static int viewDistanceProps = 4;

    //The model of the world
    public static Chunk[,] model;

    //current active chunks
    private List<Chunk> seenChunksBuildings = new List<Chunk>();
    private List<Chunk> seenChunksProps = new List<Chunk>();

    private static int lastX = 0;
    private static int lastY = 0;

    //Castle System
    private int maxSizeOfCastle = 3; // a castle has the size of this (value * 2 + 1)^2
    private int rarityOfCastle = 16; //the higher this value, the less castles spawn

    [Header("Debugging")]
    [SerializeField]
    private float differenceX;
    [SerializeField]
    private float differenceY;
    [SerializeField]
    private int playerX;
    [SerializeField]
    private int playerY;
    [SerializeField]
    private List<Chunk> newChunksBuildings;
    [SerializeField]
    private List<Chunk> newChunksProps;

    [Header("Loading Screen")]
    [SerializeField]
    private GameObject panelLoadingScreen;
    [SerializeField]
    private UnityEngine.UI.Slider progressBar;
    [SerializeField]
    private Text whatAmIDoing;
    [SerializeField]
    private GameObject panelGameUI;
    [SerializeField]
    private GameObject loadingCircle;
    private bool isLoading;

    public static int GetWorldSizeMedian()
    {
        return worldSize / 2;
    }

    void Start()
    {
        isLoading = true;
        progressBar.value = 0;
        whatAmIDoing.text = "Generating World";
        StartCoroutine(RunGeneratingWorld());
        whatAmIDoing.text = "Generating castles";
        StartCoroutine(RunGeneration());
        //CalculateCastlePositions();
        isLoading = false;
        panelGameUI.SetActive(true);
        panelLoadingScreen.SetActive(false);
    }


    public Thread StartTheThread(int param1, int param2)
    {
        var t = new Thread(() => UpdateChunks(param1, param2));
        t.Start();
        return t;
    }

    private IEnumerator RotateLoadingCircle()
    {
        float rotatingSpeed = 10;
        while (isLoading)
        {
            loadingCircle.transform.Rotate(new Vector3(rotatingSpeed * Time.deltaTime,0,0));
        }
        yield return null;
    }


    private IEnumerator RunGeneratingWorld()
    {
        rarityOfCastle = worldSize / rarityOfCastle;

        sPropPrefabs = propPrefabs;
        sWorldTiles = worldTiles;
        sNatureElementsPrefabs = natureElementsPrefabs;

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
                        int randomTrees = Random.Range(0, sPropPrefabs.Count);
                        int randomAmount = Random.Range(minTrees, maxTrees);
                        int shouldThereBeTrees = Random.Range(0, 20);
                        int randomRot = Random.Range(0, 4);

                        int decideBetweenNatureAndRessource = Random.Range(0, 100) + 1;


                        if (decideBetweenNatureAndRessource <= treeOrNatureElement)
                        {
                            for (int i = 0; i < randomAmount; i++)
                            {
                                int xPos = Random.Range(-4, 4);

                                int yPos = Random.Range(-4, 4);

                                model[x + median, y + median].SetUpNewProp(randomTrees, new Vector2(xPos + x * offsetXZ, yPos + y * offsetXZ), 0);
                            }
                            model[x + median, y + median].SetTypeOfObject(z, 0);
                            break;
                        }
                        else
                        {
                            int xPos = Random.Range(-4, 4);

                            int yPos = Random.Range(-4, 4);

                            model[x + median, y + median].SetUpNewProp(randomTrees + 1000, new Vector2(xPos + x * offsetXZ, yPos + y * offsetXZ), 0);
                            model[x + median, y + median].SetTypeOfObject(z, 0);
                            break;
                        }

                        

                        /*
                        if ((random == 0 || random >= 7) && z == 0 && randomTrees <= 3)
                        {
                            model[x + median, y + median].SetTypeOfObject(z,0);
                            model[x + median, y + median].SetRotationOfLayer(Quaternion.AngleAxis(randomRot * 90,Vector3.up),z);
                        }
                        else if (random == 0 && z == 0)
                        {
                            
                            
                        }
                        else if (random > 1 && random < 7 && z == 1)
                        {
                            model[x + median, y + median].SetTypeOfObject(z, random);
                        }
                        else if (random > 1 && random < 7 && z > 1 && model[x + median, y + median].getAllTypes()[z-1] > 1)
                        {
                            model[x + median, y + median].SetTypeOfObject(z, random);
                        }
                        else if(z== 1)
                        {
                            
                        }
                        
                        */

                    }
                    else
                    {
                        model[x + median, y + median].SetTypeOfObject(z, 1);

                    }
                }
            }
        }

        UpdateChunks(0, 0);
        yield return null;
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
        //Debug.LogWarning(chunk.GetPosOfChunk());

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
                new Vector3(pos.x, 0, pos.y),
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

    IEnumerator RunGeneration()
    {
        CalculateCastlePositions();
        yield return null;
    }

    #region Castle
    private void GenerateCastle(Chunk[,]chunksForCastle)
    {
        
        List<Chunk>[] chunkRings = new List<Chunk>[maxSizeOfCastle + 1];
        List<Chunk> chunkList = new List<Chunk>();


        //Calculate center of castle
        //int center = (maxSizeOfCastle - 1) / 2;
        //Chunk centerChunk = chunksForCastle[center,center];
        //Vector2 positionOfCenterChunk = centerChunk.GetPosOfChunk();

        /*
        for (int i = 0; i < maxSizeOfCastle + 1; i++)
        {
            chunkRings[i] = new List<Chunk>();
        }

        chunkList.Add(centerChunk);
        chunkRings[0] = chunkList;
        */


        for (int x = 0; x < chunksForCastle.GetLength(0); x++)
        {
            for (int y = 0; y < chunksForCastle.GetLength(0); y++)
            {
                int height = Random.Range(0, worldHeight);

                for (int i = 0; i < height; i++)
                {

                    int randomTile = Random.Range(2, sWorldTiles.Count);
                    int randomRot = Random.Range(0, 4);

                    chunksForCastle[x,y].SetTypeOfObject(i, randomTile);
                    Quaternion rot = Quaternion.AngleAxis(randomRot * 90, Vector3.up);
                    chunksForCastle[x, y].SetRotationOfLayer(rot, i);


                }
            }
        }




        /*
        //Calculate rings for castle
        for (int i = 1; i < maxSizeOfCastle; i++)
        {

            for (int x = 0; x < chunksForCastle.GetLength(0); x++)
            {
                
                for (int y = 0; y < chunksForCastle.GetLength(1); y++)
                {

                    try
                    {
                        Vector2 pos = chunksForCastle[x, y].GetPosOfChunk();

                        float distance = Vector2.Distance(positionOfCenterChunk, pos);

                        if (distance <= Mathf.Sqrt((i + i) * i))
                        {
                            for (int j = 0; j < chunkRings.Length; j++)
                            {

                                if (chunkRings[j].Contains(chunksForCastle[x, y]))
                                {
                                    break;
                                }
                                chunkRings[i].Add(chunksForCastle[x, y]);
                            }

                        }
                    }
                    catch (System.Exception)
                    {

                        throw;
                    }
                    
                }
            }
            
        }

        //Generate castle

        for (int i = 0; i < chunkRings.Length; i++) //for every castle ring
        {
            for (int j = 0; j < chunkRings[i].Count; j++) //for every chunk in this castle ring
            {
                int probabilityForTile = Random.Range(0, 100) + 1;
                int rate = 10;
                int maxValueForSpawning = rate * i;

                List<Chunk> chunkForSpawning = chunkRings[i];

                if (maxValueForSpawning < probabilityForTile)
                {

                    for (int height = 1; height < chunkForSpawning[j].getAllTypes().Length; height++)
                    {
                        int probabilityForTileHeight = Random.Range(0, 100) + 1;
                        int rateHeight = 20;
                        int maxValueForSpawningHeight = (rateHeight + i * 2) * height;

                        if (maxValueForSpawningHeight < probabilityForTileHeight)
                        {
                            int randomTile = Random.Range(2, sWorldTiles.Count);
                            int randomRot = Random.Range(0, 4);

                            chunkForSpawning[j].SetTypeOfObject(height, randomTile);
                            Quaternion rot = Quaternion.AngleAxis(randomRot * 90, Vector3.up);
                            chunkForSpawning[j].SetRotationOfLayer(rot, height);
                        }

                    }

                }
            }

        }
        */

    }

    private void CalculateCastlePositions()
    {
        for (int i = 0; i < rarityOfCastle; i++)
        {
            int xPos = Random.Range(0, worldSize);
            int yPos = Random.Range(0, worldSize);

            //Debug.LogWarning(xPos + " " + yPos);

            Chunk[,] castleArea = new Chunk[maxSizeOfCastle * 2 + 1, maxSizeOfCastle * 2 + 1];
            for (int x = -maxSizeOfCastle; x <= maxSizeOfCastle; x++)
            {
                for (int y = -maxSizeOfCastle; y <= maxSizeOfCastle; y++)
                {
                    var xVal = x + maxSizeOfCastle;
                    var yVal = y + maxSizeOfCastle;
                    //Debug.Log(xVal + " " + yVal + "\n" + castleArea.GetLength(0));

                    try
                    {
                        castleArea[x + maxSizeOfCastle, y + maxSizeOfCastle] = model[xPos + x, yPos + y];
                        castleArea[x + maxSizeOfCastle, y + maxSizeOfCastle].GetTypesOfProp().Clear();
                        castleArea[x + maxSizeOfCastle, y + maxSizeOfCastle].GetPositionsOfProps().Clear();
                        castleArea[x + maxSizeOfCastle, y + maxSizeOfCastle].GetRotationOfProps().Clear();
                        castleArea[x + maxSizeOfCastle, y + maxSizeOfCastle].PropObjects.Clear();
                    }
                    catch (System.Exception)
                    {
                        
                       
                    }

                    
                }
            }
            Debug.LogWarning(castleArea[0, 0].GetCoordinates());
            GenerateCastle(castleArea);
        }
    }

    #endregion

    private void UpdateChunks(int playerX, int playerY)
    {
        int median = worldSize / 2;
        newChunksBuildings = new List<Chunk>();
        newChunksProps = new List<Chunk>();
        int viewDistance = WorldGenerator.viewDistance * 2;

        int chunkX;
        int chunkY;

        //Check which chunks are need to be seen - Buildings
        try
        {
            for (int x = -viewDistance; x <= viewDistance; x++)
            {
                for (int y = -viewDistance; y <= viewDistance; y++)
                {
                    chunkX = playerX / 8 + x;
                    chunkY = playerY / 8 + y;

                    if (Mathf.Abs(x) <= viewDistanceProps && Mathf.Abs(y) <= viewDistanceProps)
                    {
                        newChunksProps.Add(model[chunkX + median, chunkY + median]); //all visbible chunks are in this list
                    } 
                    newChunksBuildings.Add(model[chunkX + median, chunkY + median]); //all visbible chunks are in this list
                    
                }
            }

        }
        catch (System.IndexOutOfRangeException)
        {
            throw;
        }

        //Destroy chunks which are not visible anymore
        int length = seenChunksBuildings.Count;
        for (int i = 0; i < length; i++)
        {
            if (!newChunksBuildings.Contains(seenChunksBuildings[i]))
            {


                for (int j = 0; j < seenChunksBuildings[i].GetAllPropObjects().Count; j++)
                {
                    Destroy(seenChunksBuildings[i].GetAllPropObjects()[j].gameObject);
                }

                for (int j = 0; j < seenChunksBuildings[i].GetAllObjectsOfChunk().Length; j++)
                {
                    Destroy(seenChunksBuildings[i].GetAllObjectsOfChunk()[j].gameObject);
                }
            }
        }


        length = newChunksBuildings.Count;
        for (int i = 0; i < length; i++)
        {
            if (!seenChunksBuildings.Contains(newChunksBuildings[i]))
            {
                //Generate new tiles
                for (int j = 0; j < newChunksBuildings[i].GetAllObjectsOfChunk().Length; j++)
                {
                    Vector2 pos = newChunksBuildings[i].GetCoordinates();
                    GameObject go = Instantiate(SpawnObject(newChunksBuildings[i].getAllTypes()[j]),
                        new Vector3(pos.x * 8, j * 8 - 8, pos.y * 8),
                        newChunksBuildings[i].GetAllRotations()[j]);
                    go.isStatic = true;
                    newChunksBuildings[i].AddTileToChunk(go, j);

                }

                //Generate props
                if (!seenChunksProps.Contains(newChunksBuildings[i]))
                {
                    for (int j = 0; j < newChunksBuildings[i].GetTypesOfProp().Count; j++)
                    {
                        Vector2 pos = newChunksBuildings[i].GetPositionsOfProps()[j];
                        GameObject go = Instantiate(SpawnTree(newChunksBuildings[i].GetTypesOfProp()[j]),
                            new Vector3(pos.x, 0, pos.y),
                            Quaternion.identity);
                        go.isStatic = true;
                        newChunksBuildings[i].AddPropObject(go);
                    }
                }


            }

        }


        seenChunksBuildings = newChunksBuildings;
        seenChunksProps = newChunksProps;

    }    

    private static GameObject SpawnObject(int type)
    {
        GameObject tile;
        switch (type)
        {
            case 0:
                tile = sWorldTiles[0]; //Air
                break;
            case 1:
                tile = sWorldTiles[1]; //Dirt
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
                return sWorldTiles[1];
        }
        return tile;
    }


    private static GameObject SpawnTree(int id)
    {
        switch (id)
        {
            case 0:
                return sPropPrefabs[0];
            case 1:
                return sPropPrefabs[1];
            case 2:
                return sPropPrefabs[2];
            case 3:
                return sPropPrefabs[3];
            case 4:
                return sPropPrefabs[4];
            case 5:
                return sPropPrefabs[5];
            case 6:
                return sPropPrefabs[6];

                //Nature Elements
            case 1001:
                return sNatureElementsPrefabs[0];
            case 1002:
                return sNatureElementsPrefabs[1];
            case 1003:
                return sNatureElementsPrefabs[2];
            case 1004:
                return sNatureElementsPrefabs[3];
            case 1005:
                return sNatureElementsPrefabs[4];
            case 1006:
                return sNatureElementsPrefabs[5];
            case 1007:
                return sNatureElementsPrefabs[6];
            case 1008:
                return sNatureElementsPrefabs[7];
            case 1009:
                return sNatureElementsPrefabs[8];
            case 1010:
                return sNatureElementsPrefabs[9];
            default:
                return sPropPrefabs[0];
        }
    }


    
}
