using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine;

public class GeneratorMenuUIControl : MonoBehaviour
{
    static public int MAP_WIDTH = 128;
    static public int MAP_HEIGHT = 128;

    // Room Parameters
    static public int ROOM_MAX_SIZE = 24;
    static public int ROOM_MIN_SIZE = 4;
    static public int ROOM_WALL_BORDER = 1;
    static public bool ROOM_UGLY_ENABLED = true;
    static public float ROOM_MAX_RATIO = 5.0f;

    // Generation Parameters
    static public int MAX_DEPTH = 10;
    static public int CHANCE_STOP = 5;
    static public int SLICE_TRIES = 10;
    static public int CORRIDOR_WIDTH = 2;


    static public int seed = -1;

    public void GenerateDungeon(string dungeonName)
    {
        SceneManager.LoadScene(dungeonName);
    }
}
