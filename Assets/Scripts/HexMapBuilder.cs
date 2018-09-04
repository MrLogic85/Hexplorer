using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;

[RequireComponent(typeof (HexMapController))]
public class HexMapBuilder : MonoBehaviour {
	private static String TILE_HOLDER_NAME = "Tiles";

	[Header("Hex Tiles")]
	public GameObject hexTileGrass;
	public GameObject hexTileHill;
	public GameObject hexTileMountain;
	public GameObject hexTileRiver;
	public GameObject hexTileWater;

	[Header("Map")]
	[TextArea]
	public String mapString = "1";
	public HexPos startPos;
	public bool showTiles;

	[Header("Sizes")]
	public float scale;
	public float scaleY;

	[HideInInspector]
	public int seed;
	[HideInInspector]
	public int size;

	[HideInInspector]
	public float waterMin;
	[HideInInspector]
	public float waterMax;
	[HideInInspector]
	public float grassMax;
	[HideInInspector]
	public float hillMax;
	[HideInInspector]
	public float mountainMax;

	[HideInInspector]
	public float amp1Hz;
	[HideInInspector]
	public float amp10Hz;
	[HideInInspector]
	public float amp100Hz;
	[HideInInspector]
	public float amp1000Hz;

	private HexMapController map;
	private PlayerController player;

	public void setMapData(String map)
	{
		mapString = map;
		CreateMap();
	}

	public void CreateMap()
	{
		map = GetComponent<HexMapController>();
		player = FindObjectOfType<PlayerController>();

		if (transform.Find(TILE_HOLDER_NAME)) {
			DestroyImmediate(transform.Find(TILE_HOLDER_NAME).gameObject);
		}

		var tiles = new Dictionary<HexPos, HexTileController>();

		Transform tileHolder = new GameObject(TILE_HOLDER_NAME).transform;
		Transform movableHolder = new GameObject("Movable").transform;
		Transform immovableHolder = new GameObject("Immovable").transform;
		tileHolder.parent = this.transform;
		movableHolder.parent = tileHolder;
		immovableHolder.parent = tileHolder;

		if (mapString != null) {
			String[] lines = mapString.Split(new String[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
			for (int y = 0; y < lines.Length; y++) {
				String[] tileChars = lines[lines.Length - 1 - y].Split(new String[] { " " }, StringSplitOptions.RemoveEmptyEntries);
				for (int x = 0; x < tileChars.Length; x++) {
					TileEnum tileType = (TileEnum) int.Parse(tileChars[x]);
					GameObject tile = GetTilePrefab(tileType);

					if (tile != null) {
						HexPos hexPos = new HexPos(x, y);
						GameObject tileObject = Instantiate(GetTilePrefab(tileType), map.GetPositionFor(hexPos), Quaternion.identity);
						HexTileController hexTile = tileObject.GetComponentInChildren<HexTileController>();
						hexTile.pos = hexPos;
						var hexOrigScale = hexTile.transform.localScale;
						hexTile.transform.localScale = new Vector3(scale * hexOrigScale.x, scaleY * hexOrigScale.y, scale * hexOrigScale.z);

						switch (tileType) {
							case TileEnum.GRASS:
							case TileEnum.HILL:
							case TileEnum.RIVER:
								tileObject.transform.parent = movableHolder;
								break;

							case TileEnum.MOUNTAIN:
							case TileEnum.WATER:
								tileObject.transform.parent = immovableHolder;
								break;
						}

						if (!showTiles) {
							hexTile.flipped = startPos.DistanceTo(hexPos) <= player.viewDistance;
						}

						tiles.Add(hexPos, hexTile);
					}
				}
			}
		}

		map.SetTiles(tiles);
		map.SetPlayerStart(startPos);
	}

	private GameObject GetTilePrefab(TileEnum type)
	{
		switch (type) {
			case TileEnum.GRASS:
				return hexTileGrass;
			case TileEnum.HILL:
				return hexTileHill;
			case TileEnum.MOUNTAIN:
				return hexTileMountain;
			case TileEnum.RIVER:
				return hexTileRiver;
			case TileEnum.WATER:
				return hexTileWater;
			default:
				return null;
		}
	}
}
