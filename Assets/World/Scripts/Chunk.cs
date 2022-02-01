using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk
{
    // Start is called before the first frame update
    private GameObject[] chunkObjects;
    private int[] typesOfObjects;
    private Quaternion[] rotationOfLayers;
    private Vector2 position;

    //Props like trees, rocks,...
    private List<int> props = new List<int>();
    private List<Vector2> propPos = new List<Vector2>();
    private List<float> propRotZ = new List<float>();
    private List<GameObject> propObjects = new List<GameObject>();

    public List<GameObject> PropObjects { get => propObjects; set => propObjects = value; }

    public Chunk()
    {
        chunkObjects = new GameObject[WorldGenerator.worldHeight];
        typesOfObjects = new int[WorldGenerator.worldHeight];
        rotationOfLayers = new Quaternion[WorldGenerator.worldHeight];
    }

    public Quaternion[] GetAllRotations()
    {
        return rotationOfLayers;
    }

    public void SetRotationOfLayer(Quaternion rotation, int level)
    {
        rotationOfLayers[level] = rotation;
    }

    public void SetUpNewProp(int type, Vector2 position, float rotation)
    {
        props.Add(type);
        propPos.Add(position);
        propRotZ.Add(rotation);
    }

    public void AddTileToChunk(GameObject tile, int heightLevel)
    {
        chunkObjects[heightLevel] = tile;
    }

    public void SetCoordinates(int x, int y)
    {
        position = new Vector2(x, y);
    }


    public Vector2 GetCoordinates()
    {
        return position;
    }

    public GameObject[] GetAllObjectsOfChunk()
    {
        return chunkObjects;
    }

    public int[] getAllTypes()
    {
        return typesOfObjects;
    }

    public void SetTypeOfObject(int level, int type)
    {
        typesOfObjects[level] = type;
    }

    public Vector2 GetPosOfChunk()
    {
        return position;
    }

    public List<GameObject> GetAllPropObjects()
    {
        return PropObjects;
    }

    public void AddPropObject(GameObject gameObject)
    {
        PropObjects.Add(gameObject);
    }


    public List<int> GetTypesOfProp()
    {
        return props;
    }

    public List<Vector2> GetPositionsOfProps()
    {
        return propPos;
    }

    public List<float> GetRotationOfProps()
    {
        return propRotZ;
    }
}
