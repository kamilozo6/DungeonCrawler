﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

// DungeonGenerator class. Singleton.
public class DungeonGenerator : MonoSingleton <DungeonGenerator> {
	
	// Dungeon Parameters
	public int MAP_WIDTH = 128;
	public int MAP_HEIGHT = 128;
	
	// Room Parameters
	public int ROOM_MAX_SIZE = 24;
	public int ROOM_MIN_SIZE = 4;
	public int ROOM_WALL_BORDER = 1;
	public bool ROOM_UGLY_ENABLED = true;
	public float ROOM_MAX_RATIO = 5.0f;
	
	// Generation Parameters
	public int MAX_DEPTH = 10;
	public int CHANCE_STOP = 5;
	public int SLICE_TRIES = 10;
	public int CORRIDOR_WIDTH = 2;
	
	// Tilemap
	public Tile[,] tiles;
	
	// Prefabs and Instance Management
	public GameObject containerRooms;
	public GameObject prefabWall01; 
	public GameObject prefabFloor01;
    public GameObject[] prefabPola;
    public Material[] roomTexture;
    public Material corridorTexture;
    public GameObject meshCombiner;
	
	// Player	
	public GameObject player;
	
	// The Random Seed
	public int seed = -1;
	
	// QuadTree for dungeon distribution
	public QuadTree quadTree;
	
	// List of rooms
	public List<Room> rooms;

	// Test
	public GameObject FPScontr;
	private bool spawnedFPS;
	
	// On Awake
	public override void Init()
	{
        MAP_WIDTH = GeneratorMenuUIControl.MAP_WIDTH;
        MAP_HEIGHT = GeneratorMenuUIControl.MAP_HEIGHT;

        // Room Parameters
        ROOM_MAX_SIZE = GeneratorMenuUIControl.ROOM_MAX_SIZE;
        ROOM_MIN_SIZE = GeneratorMenuUIControl.ROOM_MIN_SIZE;
        ROOM_WALL_BORDER = GeneratorMenuUIControl.ROOM_WALL_BORDER;
        MAX_DEPTH = GeneratorMenuUIControl.MAX_DEPTH;
        CORRIDOR_WIDTH = GeneratorMenuUIControl.CORRIDOR_WIDTH;
        seed = GeneratorMenuUIControl.seed;
    // Initialize the tilemap
        tiles = new Tile[MAP_HEIGHT,MAP_WIDTH];
		for (int i = 0; i < MAP_HEIGHT; i++) 
			for (int j = 0; j < MAP_WIDTH; j++) 
				tiles[i,j] = new Tile(Tile.TILE_EMPTY, 0);
		
		// Init QuadTree
		quadTree = new QuadTree(new AABB(new XY(MAP_WIDTH/2.0f,MAP_HEIGHT/2.0f),new XY(MAP_WIDTH/2.0f, MAP_HEIGHT/2.0f)));

		// List of rooms
		rooms = new List<Room>();

        //Camera.main.transform.position = new Vector3(MAP_WIDTH / 2, 100, MAP_HEIGHT / 2);
        Random.InitState(seed);
        GenerateDungeon(seed);

    }
	
	// On Start
	void Start () 
	{

	}
	
	// Each frame
	void Update () {
		// Generate a new Test Dungeon
		//if (Input.GetButtonDown("Jump"))
		//{
		//	// Generate a new Seed
		//	seed = System.DateTime.Now.Millisecond*1000 + System.DateTime.Now.Minute*100;
			
		//	// Camera on middle and looking down
		//	Camera.main.transform.position = new Vector3(MAP_WIDTH/2,100,MAP_HEIGHT/2);
			
		//	// Set the randome seed
		//	Random.seed = seed;
			
		//	// Generate Dungeon
		//	Debug.Log ("Dungeon Generation Started");
			
		//	GenerateDungeon(seed);
			
		//}
	}
	
	// Clean everything
	public void ResetDungeon()
	{
		// Disable player
		//player.SetActive(false);
		
		// Reset tilemap
		for (int i = 0; i < MAP_HEIGHT; i++) 
			for (int j = 0; j < MAP_WIDTH; j++) 
				tiles[i,j] = new Tile(Tile.TILE_EMPTY, 0);
		
		// Reset QuadTree
		quadTree = new QuadTree(new AABB(new XY(MAP_WIDTH/2.0f,MAP_HEIGHT/2.0f),new XY(MAP_WIDTH/2.0f, MAP_HEIGHT/2.0f)));
		
		// Reset rooms
		rooms.Clear();
		
		// Destroy tile GameObjects
		foreach (Transform t in containerRooms.transform) GameObject.Destroy(t.gameObject);
	}
	
	// Generate a new dungeon with the given seed
	public void GenerateDungeon(int seed)
	{
		Debug.Log ("Generating QuadTree");
			
		// Clean
		ResetDungeon ();
		
		// Place a temporary floor to see progress
//		floor = GameObject.Instantiate(prefabFloor01,new Vector3(MAP_WIDTH/2,-0.5f,MAP_HEIGHT/2), Quaternion.identity) as GameObject;
//		floor.transform.localScale = new Vector3(MAP_WIDTH,1,MAP_HEIGHT);
		
		// Generate QuadTree
		GenerateQuadTree (ref quadTree);

		Debug.Log ("Generating Rooms");		
		// Generate Rooms
		GenerateRooms (ref rooms, quadTree);		
		
		Debug.Log ("Generating Corridors");		
		// Generate Corridors
		GenerateCorridors ();		
		
		Debug.Log ("Generating Walls");
		
		GenerateWalls();
		
		Debug.Log ("Generating GameObjects, this may take a while..");
		
		// Instantiate prefabs
		GenerateGameObjects(quadTree);
			
		// Place Player
		//int r = Random.Range(0,rooms.Count-1);
		//Room room = rooms[r];
		//player.SetActive(true);
		//player.transform.position = new Vector3(room.boundary.center.x,1.0f,room.boundary.center.y);
		
//		GameObject.DestroyImmediate(floor);
		
	}
	
	// Generate the quadtree system
	void GenerateQuadTree(ref QuadTree _quadTree)
	{
		_quadTree.GenerateQuadTree(seed);
	}
	
	// Generate the list of rooms and dig them
	public void GenerateRooms(ref List<Room> _rooms, QuadTree _quadTree)
	{
		// Childless node
		if (_quadTree.northWest == null && _quadTree.northEast == null && _quadTree.southWest == null && _quadTree.southEast == null)
		{
			_rooms.Add(GenerateRoom(_quadTree));
			return;
		}
		
		// Recursive call
		if (_quadTree.northWest != null) GenerateRooms (ref _rooms,_quadTree.northWest);
		if (_quadTree.northEast != null) GenerateRooms (ref _rooms,_quadTree.northEast);
		if (_quadTree.southWest != null) GenerateRooms (ref _rooms,_quadTree.southWest);
		if (_quadTree.southEast != null) GenerateRooms (ref _rooms,_quadTree.southEast);
	}
	
	// Generate a single room
	public Room GenerateRoom(QuadTree _quadTree)
	{
        //Material tekstura = roomTexture[Random.Range(0, roomTexture.Length - 1)];
		// Center of the room
		XY roomCenter = new XY();
		roomCenter.x = Random.Range(ROOM_WALL_BORDER + _quadTree.boundary.Left() + ROOM_MIN_SIZE/2.0f, _quadTree.boundary.Right() - ROOM_MIN_SIZE/2.0f - ROOM_WALL_BORDER);
		roomCenter.y = Random.Range(ROOM_WALL_BORDER + _quadTree.boundary.Bottom() + ROOM_MIN_SIZE/2.0f, _quadTree.boundary.Top() - ROOM_MIN_SIZE/2.0f - ROOM_WALL_BORDER);		

		if(!spawnedFPS)
		{
			spawnedFPS = true;
			Instantiate(FPScontr, new Vector3(roomCenter.x, 0.5f, roomCenter.y), Quaternion.Euler(0, 0, 0));
		}
		else
		{
			MonsterBasic.SpawnRandomMonster(new Vector3(roomCenter.x + 0.5f, 0.0f, roomCenter.y + 0.5f));
		}
		
		// Half size of the room
		XY roomHalf = new XY();
		
		float halfX = (_quadTree.boundary.Right() - roomCenter.x - ROOM_WALL_BORDER);
		float halfX2 =(roomCenter.x - _quadTree.boundary.Left() - ROOM_WALL_BORDER);
		if (halfX2 < halfX) halfX = halfX2;
		if (halfX > ROOM_MAX_SIZE/2.0f) halfX = ROOM_MAX_SIZE/2.0f;
		
		float halfY = (_quadTree.boundary.Top() - roomCenter.y - ROOM_WALL_BORDER);
		float halfY2 =(roomCenter.y - _quadTree.boundary.Bottom() - ROOM_WALL_BORDER);
		if (halfY2 < halfY) halfY = halfY2;
		if (halfY > ROOM_MAX_SIZE/2.0f) halfY = ROOM_MAX_SIZE/2.0f;
		
		roomHalf.x = Random.Range((float)ROOM_MIN_SIZE/2.0f,halfX);
		roomHalf.y = Random.Range((float)ROOM_MIN_SIZE/2.0f,halfY);

		// Eliminate ugly zones
		if (ROOM_UGLY_ENABLED == false) 
		{
			float aspect_ratio = roomHalf.x / roomHalf.y;
			if (aspect_ratio > ROOM_MAX_RATIO || aspect_ratio < 1.0f/ROOM_MAX_RATIO) return GenerateRoom(_quadTree); 
		}
		
		// Create AABB
		AABB randomAABB = new AABB(roomCenter, roomHalf);
		
		// Dig the room in our tilemap
		DigRoom (randomAABB.BottomTile(), randomAABB.LeftTile(), randomAABB.TopTile()-1, randomAABB.RightTile()-1);
		
		// Return the room
		return new Room(randomAABB,_quadTree);
	}
	
	void GenerateCorridors()
	{
		quadTree.GenerateCorridors();
	}

	// Generate walls when there's something near
	public void GenerateWalls()
	{
		// Place walls
		for (int i = 0; i < MAP_HEIGHT; i++)
		{
			for (int j = 0; j < MAP_WIDTH; j++)
			{
				bool room_near = false;
				if (IsPassable(i,j)) continue;
				if (i > 0) if (IsPassable(i-1,j)) room_near = true;
				if (i < MAP_HEIGHT-1) if (IsPassable(i+1,j)) room_near = true;
				if (j > 0) if (IsPassable(i,j-1)) room_near = true;
				if (j < MAP_WIDTH-1) if (IsPassable(i,j+1)) room_near = true;
				if (room_near) SetWall(i,j);
			}
		}
	}
	
	// Read tilemap and instantiate GameObjects
	void GenerateGameObjects(QuadTree _quadtree)
	{
		// If it's an end quadtree, read every pos and make a chunk of combined meshes
		if (_quadtree.HasChildren() == false)
		{
			GameObject container = GameObject.Instantiate(meshCombiner) as GameObject;
			for (int row = _quadtree.boundary.BottomTile(); row <= _quadtree.boundary.TopTile()-1; row++)
			{
				for (int col = _quadtree.boundary.LeftTile(); col <= _quadtree.boundary.RightTile()-1; col++)
				{
					int id = tiles[row,col].id;
					if (id == Tile.TILE_ROOM) //Pole w pomieszczeniu
					{
                        var selectedTile = Random.Range(0, 100);
                        if(selectedTile>80)
                        {
                            selectedTile = Random.Range(0, prefabPola.Length);
                        }
                        else
                        {
                            selectedTile = 0;
                        }
						GameObject floor = GameObject.Instantiate(prefabPola[selectedTile],new Vector3(col+0.5f,0.0f,row + 0.5f),Quaternion.identity) as GameObject;
						floor.transform.parent = container.transform;
                        floor.GetComponent<Renderer>().material = roomTexture[tiles[row,col].texture];
					}
                    else if (id == Tile.TILE_CORRIDOR) //pole w korytarzu
                    {
                        GameObject floor = GameObject.Instantiate(prefabFloor01, new Vector3(col + 0.5f, 0.0f, row + 0.5f), Quaternion.identity) as GameObject;
                        floor.transform.parent = container.transform;
                        floor.GetComponent<Renderer>().material = corridorTexture;
                    }
                    else if (id == Tile.TILE_WALL) // pole na ścianę
					{
						GameObject wall = GameObject.Instantiate(prefabWall01,new Vector3(col + 0.5f, 0.0f,row + 0.5f),Quaternion.identity) as GameObject;
						wall.transform.parent = container.transform;
                        wall.GetComponent<Renderer>().material = corridorTexture;
                    }
				}
			}
			container.transform.parent = containerRooms.transform;
		}
		else 
		{
				GenerateGameObjects(_quadtree.northWest);
				GenerateGameObjects(_quadtree.northEast);
				GenerateGameObjects(_quadtree.southWest);
				GenerateGameObjects(_quadtree.southEast);
		}

	}
	// Helper Methods
	public bool IsEmpty(int row, int col) { return tiles[row,col].id == Tile.TILE_EMPTY; }
	
	public bool IsPassable(int row, int col) { return tiles[row,col].id == Tile.TILE_ROOM || tiles[row,col].id == Tile.TILE_CORRIDOR; }
	
	public bool IsPassable(XY xy) { return IsPassable((int) xy.y, (int) xy.x);}
	
	public void SetWall(int row, int col)
	{
		tiles[row,col].id = Tile.TILE_WALL;
	}

	// Dig a room, placing floor tiles
	public void DigRoom(int row_bottom, int col_left, int row_top, int col_right)
	{
		// Out of range
		if ( row_top < row_bottom ) 
		{
		    int tmp = row_top;
		    row_top = row_bottom;
		    row_bottom = tmp;
		}
		
		// Out of range
		if ( col_right < col_left ) 
		{
		    int tmp = col_right;
		    col_right = col_left;
		    col_left = tmp;
		}
		
		if (row_top > MAP_HEIGHT-1) return;
		if (row_bottom < 0) return;
		if (col_right > MAP_WIDTH-1) return;
		if (col_left < 0) return;

        // Dig floor
        var texNum = Random.Range(0, roomTexture.Length);
        for (int row = row_bottom; row <= row_top; row++) 
	        for (int col = col_left; col <= col_right; col++) 
	            DigRoom (row,col, texNum);
	}
	
	public void DigRoom(int row, int col, int texNum)
	{
        //todo dodać losowanie typu pola i tekstury
        tiles[row,col].id = Tile.TILE_ROOM;
        tiles[row, col].texture = texNum;
	}
	
	public void DigCorridor(int row, int col)
	{
        if (tiles[row, col].id == Tile.TILE_EMPTY)
        {
            tiles[row, col].id = Tile.TILE_CORRIDOR;
        }
	}
	
	public void DigCorridor(XY p1, XY p2)
	{
		int row1 = Mathf.RoundToInt(p1.y);
		int row2 = Mathf.RoundToInt(p2.y);
		int col1 = Mathf.RoundToInt(p1.x);
		int col2 = Mathf.RoundToInt(p2.x);
		
		DigCorridor(row1,col1,row2,col2);
	}
	
	public void DigCorridor(int row1, int col1, int row2, int col2)
	{		
		if (row1 <= row2)
		{
			for (int col = col1; col < col1 + CORRIDOR_WIDTH; col++)
				for (int row = row1; row <= row2; row++)
					DigCorridor(row,col);
		}
		else 
		{
			for (int col = col1; col < col1 + CORRIDOR_WIDTH; col++)
				for (int row = row2; row <= row1; row++)
					DigCorridor(row,col);
		}
		
		if (col1 <= col2)
		{
			for (int row = row2; row < row2 + CORRIDOR_WIDTH; row++)
				for (int col = col1; col <= col2; col++)
					DigCorridor(row,col);
		}
		else 
		{
			for (int row = row2; row < row2 + CORRIDOR_WIDTH; row++)
				for (int col = col2; col <= col1; col++)
					DigCorridor(row2,col);
		}
	}
	
	
}
