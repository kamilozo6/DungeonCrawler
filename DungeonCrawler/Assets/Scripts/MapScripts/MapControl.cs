using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapControl : MonoBehaviour
{

    public int MapSize;
    BitArray GameMap;
    public GameObject pole;

    // Start is called before the first frame update
    void Start()
    {
        GameMap = new BitArray(MapSize * MapSize);
        GenerateMap();
        DrawMap();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void GenerateMap()
    {
        for(int i=0;i<MapSize;i++)
        {
            for (int j = 0; j < MapSize; j++)
            {
                if (i == j) GameMap[i + (j * MapSize)] = true;
            }
        }
    }

    void DrawMap()
    {
        for (int i = 0; i < GameMap.Length; i++)
        {
            if (GameMap[i]) Instantiate(pole, new Vector3((i % MapSize)*pole.transform.GetChild(0).GetComponent<Renderer>().bounds.size.x, 0, (i / MapSize) * pole.transform.GetChild(0).GetComponent<Renderer>().bounds.size.z), Quaternion.identity);
        }
    }

    void MakeCorridor(int x1, int y1, int x2, int y2)
    {

    }

}