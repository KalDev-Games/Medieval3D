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
    private int maxTrees = 30;
    [SerializeField]
    private int minTrees = 10;
    [SerializeField]
    private int treeOrNatureElement = 90; //The higher the more and trees and less rocks

    // Start is called before the first frame update
    [Header ("Player & World")]
    [SerializeField]
    private Transform player;
    [SerializeField]
    private List<GameObject> worldTiles;
    public static List<GameObject> sWorldTiles;
    [SerializeField]
    private List<GameObject> worldTilesWater;
    public static List<GameObject> sWorldTilesWater;
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
    [Header("Castle")]
    [SerializeField]
    private int maxSizeOfCastle = 3; // a castle has the size of this (value * 2 + 1)^2
    [SerializeField]
    private int rarityOfCastle = 16; //the higher this value, the less castles spawn

    [Header("Water")]
    [SerializeField]
    private int maxSizeOfLake = 3; // a castle has the size of this (value * 2 + 1)^2
    [SerializeField]
    private int rarityOfLake = 16; //the higher this value, the less castles spawn

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
    private Slider progressBar;
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
        //StartCoroutine(RunGeneratingWorld());
        Setup();
        //StartCoroutine(RunLakeGeneration());
        CreateSeas();
        whatAmIDoing.text = "Generating castles";
        //StartCoroutine(RunGeneration());
        CalculateCastlePositions();
        //CalculateCastlePositions();
        isLoading = false;
        panelGameUI.SetActive(true);
        Player.LastUI = 1;
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
        Setup();
        yield return null;
    }

    private void Setup()
    {
        rarityOfCastle = worldSize / rarityOfCastle;

        sPropPrefabs = propPrefabs;
        sWorldTiles = worldTiles;
        sNatureElementsPrefabs = natureElementsPrefabs;
        sWorldTilesWater = worldTilesWater;

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
                        int randomRot = Random.Range(0, 360);

                        int decideBetweenNatureAndRessource = Random.Range(0, 100) + 1;


                        if (decideBetweenNatureAndRessource <= treeOrNatureElement)
                        {
                            for (int i = 0; i < randomAmount; i++)
                            {
                                int xPos = Random.Range(-4, 4);

                                int yPos = Random.Range(-4, 4);

                                model[x + median, y + median].SetUpNewProp(randomTrees, new Vector2(xPos + x * offsetXZ, yPos + y * offsetXZ), randomRot);
                            }
                            model[x + median, y + median].SetTypeOfObject(z, 0);
                            break;
                        }
                        else
                        {
                            int xPos = Random.Range(-4, 4);

                            int yPos = Random.Range(-4, 4);

                            model[x + median, y + median].SetUpNewProp(randomTrees + 1000, new Vector2(xPos + x * offsetXZ, yPos + y * offsetXZ), randomRot);
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
        progressBar.value = 50;
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
            if (chunk.GetTypesOfProp()[j] < 1000)
            {
                go.AddComponent<Loot>();
                go.GetComponent<Loot>().ressource = AddRessource(chunk.GetTypesOfProp()[j]);
            }
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

    IEnumerator RunLakeGeneration()
    {
        CreateSeas();
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

                for (int i = 1; i < height; i++)
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
        progressBar.value = 100;

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
                    ClearTileOfProps(castleArea[x + maxSizeOfCastle, y + maxSizeOfCastle]);
                    
                }
            }
            //Debug.LogWarning(castleArea[0, 0].GetCoordinates());
            GenerateCastle(castleArea);
        }
    }

    #endregion



    private void ClearTileOfProps(Chunk chunk)
    {
        try
        {
            chunk.GetTypesOfProp().Clear();
            chunk.GetPositionsOfProps().Clear();
            chunk.GetRotationOfProps().Clear();
            chunk.PropObjects.Clear();
        }
        catch (System.Exception)
        {


        }
    }
    //p = stretch *(-apex)^2+100

    private void CreateSeas()
    {
        int amountLakes = worldSize / rarityOfLake;
        List<Chunk> seaParts = new List<Chunk>();

        for (int i = 0; i < amountLakes; i++)
        {
            //random Coordinates
            seaParts.Clear();
            int xPos = Random.Range(0, worldSize - 50);
            int yPos = Random.Range(0,worldSize - 50);

            while (xPos + maxSizeOfLake > worldSize && yPos + maxSizeOfLake > worldSize)
            {
                xPos = Random.Range(0, worldSize);
                yPos = Random.Range(0, worldSize);

            }

            int isThereWater = Random.Range(0, 20);
            Debug.LogError("Go to " + model[xPos,yPos].GetPosOfChunk().x*8 + " and " 
                + model[xPos, yPos].GetPosOfChunk().y * 8);

            for (int x = 0; x < maxSizeOfLake; x++)
            {
                for (int y = 0; y < maxSizeOfLake; y++)
                {
                    isThereWater = Random.Range(0, 20);
                    //Debug.LogWarning("Probability for " + y + " is " + probabilityForLake(maxSizeOfLake / 2, y) + "\n" + isThereWater);
                    if (probabilityForLake(maxSizeOfLake/2,y) >= isThereWater && probabilityForLake(maxSizeOfLake / 2, x) >= isThereWater) 
                    {
                        try
                        {
                            //Debug.LogWarning(model[xPos + x, yPos + y].GetCoordinates() + "\n" + model[xPos + x, yPos + y].GetAllObjectsOfChunk()[0]);
                            model[xPos + x, yPos + y].SetTypeOfObject(0,2000);
                            ClearTileOfProps(model[xPos + x, yPos + y]);
                            seaParts.Add(model[xPos + x, yPos + y]);
                            //Debug.LogWarning(model[xPos + x, yPos + y].GetCoordinates() + "\n" + model[xPos + x, yPos + y].GetAllObjectsOfChunk()[0]);
                        }
                        catch (System.Exception)
                        {
                            var xPosition = xPos + x;
                            var yPosition = yPos + y;
                            Debug.LogWarning("Chunk not possible for " + xPosition + "|" + yPosition);
                            throw;
                        }
                        
                    }
                }
            }

            foreach (var seaTile in seaParts)
            {
                Vector2 pos = seaTile.GetPosOfChunk();

                bool[] directions = new bool[4];



                int neighborCounter = 0;

                foreach (var item in seaParts) 
                {

                    Vector2 comp = item.GetPosOfChunk();
                    if (Vector2.Distance(pos, comp) == 1)
                    {
                        neighborCounter++;

                        if (pos.x - comp.x == 0 && pos.y - comp.y == 1)
                        {
                            directions[0] = true;
                        }
                        else if (pos.x - comp.x == 0 && pos.y - comp.y == -1)
                        {
                            directions[2] = true;
                        } 
                        else if (pos.x - comp.x == 1 && pos.y - comp.y == 0)
                        {
                            directions[1] = true;
                        }
                        else if (pos.x - comp.x == -1 && pos.y - comp.y == 0)
                        {
                            directions[3] = true;
                        }





                    }
                }
                if (neighborCounter > 1 && neighborCounter < 4)
                {
                    if (neighborCounter == 2)
                    {
                        seaTile.SetTypeOfObject(0, 2001);

                        if (directions[0] && directions [1])
                        {
                            seaTile.SetRotationOfLayer(Quaternion.AngleAxis(270, Vector3.up),0);
                        }
                        else if (directions[1] && directions[2])
                        {
                            seaTile.SetRotationOfLayer(Quaternion.AngleAxis(90, Vector3.up), 0);
                        }
                        else if (directions[2] && directions[3])
                        {
                            seaTile.SetRotationOfLayer(Quaternion.AngleAxis(0, Vector3.up), 0);
                        } 
                        else if (directions[3] && directions[0])
                        {
                            seaTile.SetRotationOfLayer(Quaternion.AngleAxis(180,Vector3.up), 0);
                        }

                    }
                    else if (neighborCounter == 3)
                    {
                        seaTile.SetTypeOfObject(0, 2002);

                        if (directions[0] && directions[1] && directions[2])
                        {
                            seaTile.SetRotationOfLayer(Quaternion.AngleAxis(270, Vector3.up), 0);
                        } 
                        else if (directions[1] && directions[2] && directions[3])
                        {
                            seaTile.SetRotationOfLayer(Quaternion.AngleAxis(180, Vector3.up), 0);
                        }
                        else if (directions[2] && directions[3] && directions[0])
                        {
                            seaTile.SetRotationOfLayer(Quaternion.AngleAxis(0, Vector3.up), 0);
                        }
                        else if (directions[3] && directions[0] && directions[1])
                        {
                            seaTile.SetRotationOfLayer(Quaternion.AngleAxis(90, Vector3.up), 0);
                        }


                    }

                }
                
            }
        }



    }

    private float probabilityForLake(int apex, int xValue)
    {
        int maxProbability = 20;
        int startAlititude = 1;
        float stretch = 1;
        if (apex == xValue)
        {
            stretch = startAlititude - maxProbability;
        }
        else
        {
            stretch = startAlititude - maxProbability / Mathf.Pow(xValue - apex, 2);
        }
        
        return stretch * Mathf.Pow(xValue - apex, 2) + maxProbability;
    }





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
                        if (newChunksBuildings[i].GetTypesOfProp()[j] < 1000)
                        {
                            go.AddComponent<Loot>();
                            go.GetComponent<Loot>().ressource = AddRessource(newChunksBuildings[i].GetTypesOfProp()[j]);
                        }
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
            case 2000:
                tile = sWorldTilesWater[0];
                break;
            case 2001:
                tile = sWorldTilesWater[1];
                break;
            case 2002:
                tile = sWorldTilesWater[2];
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
                return AddLooting(new Wood(), sPropPrefabs[0]);
            case 1: 
                return AddLooting(new Wood(), sPropPrefabs[1]);
            case 2:
                return AddLooting(new Wood(), sPropPrefabs[2]);
            case 3:
                return AddLooting(new Wood(), sPropPrefabs[3]);
            case 4:
                return AddLooting(new Corn(), sPropPrefabs[4]);
            case 5:
                return AddLooting(new Carot(), sPropPrefabs[5]);
            case 6:
                return AddLooting(new Rock(), sPropPrefabs[6]);
            case 7:
                return AddLooting(new Rock(), sPropPrefabs[7]);
            case 8:
                return AddLooting(new Rock(), sPropPrefabs[8]);
            case 9:
                return AddLooting(new Rock(), sPropPrefabs[9]);
                
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
                return AddLooting(new Wood(), sPropPrefabs[0]);
        }
    }

    private static GameObject AddLooting(Ressource rsc, GameObject gObj)
    {
        GameObject newGObj = gObj;
        newGObj.GetComponent<Loot>().ressource = rsc;
        if (newGObj.GetComponent<Loot>().ressource != null)
        {
            newGObj.GetComponent<Loot>().display = true;
        }
                
        return newGObj;
    }

    private static Ressource AddRessource(int index)
    {
        int index2 = index;
        if (index >= 0 && index < 4)
        {
            return new Wood();
        }
        else if (index == 4)
        {
            return new Corn();
        }
        else if (index == 5)
        {
            return new Carot();
        }
        else if (index > 5 && index < 10)
        {
            return new Rock();
        }
        else
        {
            return new Rock();
        }
    }


    
}
