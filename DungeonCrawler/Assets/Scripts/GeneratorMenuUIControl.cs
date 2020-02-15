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

    public InputField seedInput;
    public Slider mapWidth;
    public Slider mapHeight;
    public Slider maxRoomSize;
    public Slider minRoomSize;
    public Slider roomWallBorder;
    public Slider corridorWidth;
    public Slider maxDepth;

    public void GenerateDungeon(string dungeonName)
    {
        seed = int.Parse(seedInput.text);
        MAP_WIDTH = (int)mapWidth.value;
        MAP_HEIGHT = (int)mapHeight.value;
        ROOM_MAX_SIZE = (int)maxRoomSize.value;
        ROOM_MIN_SIZE = (int)minRoomSize.value;
        ROOM_WALL_BORDER = (int)roomWallBorder.value;
        CORRIDOR_WIDTH = (int)corridorWidth.value;
        MAX_DEPTH = (int)maxDepth.value;
        SceneManager.LoadScene(dungeonName);
    }
    public void Awake()
    {
        seedInput.text = (System.DateTime.Now.Millisecond * 1000 + System.DateTime.Now.Minute * 100).ToString();
    }
}
