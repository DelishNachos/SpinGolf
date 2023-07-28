using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LevelEditor : MonoBehaviour
{
	[SerializeField] Tilemap defaultTilemap;
	private Vector3Int prevPos;

	Tilemap previewTilemap
	{
		get
		{
			if (LevelManager.instance.layers.TryGetValue((int)LevelManager.Tilemaps.Preview, out Tilemap tilemap)) {
				return tilemap;
			} else
			{
				return defaultTilemap;
			}
		}
	}
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

		if (prevPos != pos)
		{
			UpdatePreviewTile(pos, prevPos);
			prevPos = pos;
		}

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
			UpdatePreviewTile(pos, prevPos);
		}
		if (Input.GetKeyDown(KeyCode.Q))
		{
			_selectedTileIndex--;
			if (_selectedTileIndex < 0) _selectedTileIndex = LevelManager.instance.tiles.Count - 1;
			UpdatePreviewTile(pos, prevPos);
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

	private void UpdatePreviewTile(Vector3Int _pos, Vector3Int _prevPos)
	{
		ResetPreviewTile(_prevPos);
		PreviewTile(_pos);
	}

	private void PreviewTile(Vector3Int pos)
	{
		previewTilemap.SetTile(pos, currentTile);
	}
	private void ResetPreviewTile(Vector3Int pos)
	{
		previewTilemap.SetTile(pos, null);
	}
}
