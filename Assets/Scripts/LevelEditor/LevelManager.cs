using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;
using System.IO;

public class LevelManager : MonoBehaviour
{
	#region Singleton + more
	public static LevelManager instance;

	private void Awake()
	{
		if (instance == null)
		{
			instance = this;
		} else
		{
			Destroy(this);
		}

		foreach(Tilemap tilemap in tilemaps) 
		{
			foreach (Tilemaps num in System.Enum.GetValues(typeof(Tilemaps)))
			{
				if (tilemap.name == num.ToString())
				{
					if (!layers.ContainsKey((int)num)) layers.Add((int)num, tilemap);
				}
			}
		}
	}
	#endregion

	public List<CustomTile> tiles = new List<CustomTile>();
	[SerializeField] List<Tilemap> tilemaps = new List<Tilemap>();
	public Dictionary<int, Tilemap> layers = new Dictionary<int, Tilemap>();

	public enum Tilemaps
	{
		Misc1 = 10,
		AirDown = 20,
		AirUp = 30,
		Water = 40,
		Platforms = 50,
		Ground = 60,
		Misc2 = 70,
		Preview = 100
	}

	private void Update()
	{
		if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.A)) SaveLevel();
		if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.L)) LoadLevel();

	}

	void SaveLevel()
	{
		//create a new leveldata
		LevelData levelData = new LevelData();

		//set up the layers in the leveldata
		foreach (var item in layers.Keys)
		{
			levelData.layers.Add(new LayerData(item));
		}

		foreach (var layerData in levelData.layers)
		{
			if (!layers.TryGetValue(layerData.layerID, out Tilemap tilemap)) break;

			//get the bounds of the tilemap
			BoundsInt bounds = tilemap.cellBounds;

			//loop trougth the bounds of the tilemap
			for (int x = bounds.min.x; x < bounds.max.x; x++)
			{
				for (int y = bounds.min.y; y < bounds.max.y; y++)
				{
					//get the tile on the position
					TileBase temp = tilemap.GetTile(new Vector3Int(x, y, 0));
					//find the temp tile in the custom tiles list
					CustomTile temptile = tiles.Find(t => t.tile == temp);

					//if there's a customtile associated with the tile
					if (temptile != null)
					{
						//add the values to the leveldata
						layerData.tiles.Add(temptile.id);
						layerData.posesX.Add(x);
						layerData.posesY.Add(y);
					}
				}
			}

		}

		//save the data as a json
		string json = JsonUtility.ToJson(levelData, true);
		File.WriteAllText(Application.dataPath + "/testLevel.json", json);

		//debug
		Debug.Log("Level was saved");
	}

	private void LoadLevel()
	{
		string json = File.ReadAllText(Application.dataPath + "/testLevel.json");
		LevelData levelData = JsonUtility.FromJson<LevelData>(json);

		foreach (var data in levelData.layers)
		{
			if (!layers.TryGetValue(data.layerID, out Tilemap tilemap)) break;
			
			tilemap.ClearAllTiles();

			for (int i = 0; i < data.tiles.Count; i++)
			{
				TileBase tile = tiles.Find(t => t.id == data.tiles[i]).tile;
				if (tile) tilemap.SetTile(new Vector3Int(data.posesX[i], data.posesY[i], 0), tile);
			}

		}

		
	}
}

[System.Serializable]
public class LevelData
{
	public List<LayerData> layers = new List<LayerData>();
}

[System.Serializable]
public class LayerData
{
	public int layerID;
	public List<string> tiles = new List<string>();
	public List<int> posesX = new List<int>();
	public List<int> posesY = new List<int>();

	public LayerData(int id)
	{
		layerID = id;
	}
}
