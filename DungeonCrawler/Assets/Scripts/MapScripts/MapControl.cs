using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapControl : MonoBehaviour
{

    public int MapSize;
    string[,] GameMap;
    public GameObject basicField;
    float size;

    // Start is called before the first frame update
    void Start()
    {
        GameMap = new string[MapSize+2, MapSize+2];
        size = basicField.transform.GetChild(0).GetComponent<Renderer>().bounds.size.x;
        GenerateMap();
        DrawMap();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void GenerateMap()
    {
        for(int i=1;i<=MapSize;i++)
        {
            for (int j = 1; j <= MapSize; j++)
            {
                GameMap[i, j] = "";
                if (i == j) GameMap[i,j] = "Empty field";
            }
        }
    }

    void DrawMap()
    {
        for (int i = 0; i <= MapSize; i++)
        {
            for (int j = 0; j <= MapSize; j++)
            {
                if (GameMap[i, j] != "" && GameMap[i, j] != null)
                {
                    Instantiate(Resources.Load(GameMap[i,j]), new Vector3(i * size, 0, j * size), Quaternion.identity);
                }
            }
        }
    }

    void MakeCorridor(int x1, int y1, int x2, int y2)
    {

    }

}