using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LevelEditor : MonoBehaviour
{
	[SerializeField] Tilemap defaultTilemap;

    Tilemap currentTilemap
	{
		get
		{
			if (LevelManager.instance.layers.TryGetValue((int)LevelManager.instance.tiles[_selectedTileIndex].tilemap, out Tilemap tilemap))
			{
				return tilemap;
			} else
			{
				return defaultTilemap;
			}
		}
	}
    TileBase currentTile
	{
		get
		{
			return LevelManager.instance.tiles[_selectedTileIndex].tile;
		}
	}

    private Camera cam;

	private int _selectedTileIndex;

	private void Start()
	{
		cam = Camera.main;
	}

	private void Update()
	{
		Vector3Int pos = currentTilemap.WorldToCell(cam.ScreenToWorldPoint(Input.mousePosition));

		if (Input.GetMouseButton(0))
		{
			PlaceTile(pos);
		}

		if (Input.GetMouseButton(1))
		{
			DeleteTile(pos);
		}

		if (Input.GetKeyDown(KeyCode.E))
		{
			_selectedTileIndex++;
			if (_selectedTileIndex >= LevelManager.instance.tiles.Count) _selectedTileIndex = 0;
		}
		if (Input.GetKeyDown(KeyCode.Q))
		{
			_selectedTileIndex--;
			if (_selectedTileIndex < 0) _selectedTileIndex = LevelManager.instance.tiles.Count - 1;
		}
	}

	private void PlaceTile(Vector3Int pos)
	{
        currentTilemap.SetTile(pos, currentTile);
	}

	private void DeleteTile(Vector3Int pos)
	{
		currentTilemap.SetTile(pos, null);
	}
}
