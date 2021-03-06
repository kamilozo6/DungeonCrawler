using UnityEngine;
using System.Collections;

[System.Serializable]
public class Tile {
	// Tile Types
	public const int TILE_EMPTY = 0;
	public const int TILE_ROOM = 1;
	public const int TILE_WALL = 2;
	public const int TILE_CORRIDOR = 3;
	
	// Tile ID
	public int id;
    public int texture;
    
	
	public Tile ( int _id, int _texture )
	{
		this.id = _id;
        this.texture = _texture;
	}
}
