using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk
{
    // Start is called before the first frame update
    private GameObject[] chunkObjects;
    private int[] typesOfObjects;
    private Vector2 position;
    
    public Chunk()
    {
        chunkObjects = new GameObject[WorldGenerator.worldHeight];
        typesOfObjects = new int[WorldGenerator.worldHeight];
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
}
