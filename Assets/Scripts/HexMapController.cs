using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using System;
using System.Collections;
using NUnit.Framework;
using UnityEngine.Tilemaps;


public class HexMapController : MonoBehaviour {

	[Header("Tile size")]
	public float dZ;
	public float dX;
	public float playerOffset;

	private PlayerController player;
	private Dictionary<HexPos, HexTileController> tiles = new Dictionary<HexPos, HexTileController>();

	public IEnumerable<HexTileController> tileControllers { get { return tiles.Values; } }

	// Use this for initialization
	void Start()
	{
		player = FindObjectOfType<PlayerController>();
	}

	public void SetPlayerStart(HexPos pos)
	{
		if (player == null) {
			player = FindObjectOfType<PlayerController>();
		}

		SetPlayerPosition(pos);
	}

	public void SetTiles(Dictionary<HexPos, HexTileController> tiles)
	{
		this.tiles = tiles;
	}

	public void SetPlayerPosition(HexPos pos)
	{
		if (player != null) {
			player.transform.position = GetPositionFor(pos) + Vector3.up * playerOffset;
		}
	}

	public Vector3 GetPositionFor(HexPos hexPos)
	{
		return new Vector3(hexPos.x * dX, 0, hexPos.y * dZ + hexPos.x * dZ / 2f);
	}

	public HexPos GetHexPositionFor(Vector3 position)
	{
		var hexX = (int) Math.Round(position.x / dX);
		var hexY = (int) Math.Round((position.z - hexX * dZ / 2f) / dZ);
		return new HexPos(hexX, hexY);
	}

	HexTileController GetTile(HexPos hexPos)
	{
		HexTileController tileObject;
		if (tiles.TryGetValue(hexPos, out tileObject)) {
			return tileObject;
		}

		return null;
	}

	public void RevealPosition(Vector3 position, int distance)
	{
		RevealPosition(GetHexPositionFor(position), distance);
	}

	public void RevealPosition(HexPos position, int distance)
	{
		foreach (HexTileController tile in GetNeighbours(position, distance)) {
			RevealPosition(tile.pos);
		}
	}

	public List<HexTileController> GetNeighbours(HexPos pos, int distance = 1)
	{
		var neighbours = new List<HexTileController>();

		for (int x = -distance; x <= distance; x++) {
			for (int y = -distance; y <= distance; y++) {
				int dist = HexPos.GetDistance(0, 0, x, y);
				if (dist == 0 || dist > distance) {
					continue;
				}

				var neighbour = GetTile(new HexPos(pos.x + x, pos.y + y));

				if (neighbour != null) {
					neighbours.Add(neighbour);
				}
			}
		}

		return neighbours;
	}

	private void RevealPosition(HexPos hexPos)
	{
		var tile = GetTile(hexPos);

		if (tile != null) {
			tile.FlipTile();
		}
	}

	public void ClearHighlights()
	{
		foreach (HexTileController tile in tiles.Values) {
			tile.SetHighlighted(false);
		}
	}
}
