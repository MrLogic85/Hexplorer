using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System;
using UnityEngine.AI;
using System.Diagnostics;
using System.Text;
using UnityEngine.Tilemaps;
using NUnit.Framework.Constraints;

[CustomEditor(typeof (HexMapBuilder))]
public class HexMapControllerEditor : Editor {

	public override void OnInspectorGUI()
	{
		HexMapBuilder map = (HexMapBuilder) target;

		EditorGUI.BeginChangeCheck();
		DrawDefaultInspector();

		if (EditorGUI.EndChangeCheck()) {
			map.CreateMap();
		}

		EditorGUI.BeginChangeCheck();
		EditorGUILayout.Separator();
		EditorGUILayout.LabelField("Map Generator", EditorStyles.boldLabel);

		if (GUILayout.Button("GenerateSeed")) {
			map.seed = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
		}

		map.seed = EditorGUILayout.IntField("Seed", map.seed);
		map.size = EditorGUILayout.IntSlider("Size", map.size, 1, 10);

		EditorGUILayout.LabelField("Noise Parameters");
		map.amp1Hz = EditorGUILayout.Slider("1Hz", map.amp1Hz, 0f, 1f);
		map.amp10Hz = EditorGUILayout.Slider("10Hz", map.amp10Hz, 0f, 1f);
		map.amp100Hz = EditorGUILayout.Slider("100Hz", map.amp100Hz, 0f, 1f);
		map.amp1000Hz = EditorGUILayout.Slider("1000Hz", map.amp1000Hz, 0f, 1f);
		EditorGUILayout.MinMaxSlider("Water", ref map.waterMin, ref map.waterMax, 0f, 100f);
		EditorGUILayout.MinMaxSlider("Grass", ref map.waterMax, ref map.grassMax, 0, 100);
		EditorGUILayout.MinMaxSlider("Hill", ref map.grassMax, ref map.hillMax, 0, 100);
		EditorGUILayout.MinMaxSlider("Mountain", ref map.hillMax, ref map.mountainMax, 0, 100);

		if (!EditorGUI.EndChangeCheck()) {
			return;
		}

		var random = new System.Random(map.seed);
		var tiles = new StringBuilder();
		var offset1Hz = (float) random.NextDouble();
		var offset10Hz = (float) random.NextDouble();
		var offset100Hz = (float) random.NextDouble();
		var offset1000Hz = (float) random.NextDouble();
		var variancey1Hz = (float) random.NextDouble();
		var variancex1Hz = (float) random.NextDouble();
		var variancey10Hz = (float) random.NextDouble() * 10f + 5f;
		var variancex10Hz = (float) random.NextDouble() * 10f + 5f;
		var variancey100Hz = (float) random.NextDouble() * 100f + 50f;
		var variancex100Hz = (float) random.NextDouble() * 100f + 50f;
		var variancey1000Hz = (float) random.NextDouble() * 1000f + 500f;
		var variancex1000Hz = (float) random.NextDouble() * 1000f + 500f;

		Vector3 center = new Vector3(map.size - 1, map.size - 1, 2 - map.size * 2);

		for (int y = (map.size - 1) * 2; y >= 0; y--) {
			for (int x = 0; x < (map.size - 1) * 2 + 1; x++) {
				var pos = new Vector3(y, x, -y - x);
				var dir = (pos - center);
				var distance = (Math.Abs(dir.x) + Math.Abs(dir.y) + Math.Abs(dir.z)) / 2.0;
				if (distance < map.size) {
					var value = map.amp1Hz * Math.Sin(y * variancey1Hz + x * variancex1Hz + offset1Hz)
					            + map.amp10Hz * Math.Sin(y * variancey10Hz + x * variancex10Hz + offset10Hz)
					            + map.amp100Hz * Math.Sin(y * variancey100Hz + x * variancex100Hz + offset100Hz)
					            + map.amp1000Hz * Math.Sin(y * variancey1000Hz + x * variancex1000Hz + offset1000Hz);
					var amp = map.amp1Hz + map.amp10Hz + map.amp100Hz + map.amp1000Hz;
					value = (value / amp + 1f) * 50f;
					int tileType = GetTileType(value);
					tiles.Append(tileType).Append(' ');
				} else {
					tiles.Append("0 ");
				}
			}

			tiles.Append("\n");
		}

		map.setMapData(tiles.ToString());
	}

	int GetTileType(double value)
	{
		HexMapBuilder map = (HexMapBuilder) target;

		if (value >= map.waterMin && value < map.waterMax) {
			return 1;
		} else if (value < map.grassMax) {
			return 3;
		} else if (value < map.hillMax) {
			return 4;
		} else if (value < map.mountainMax) {
			return 5;
		}

		return 0;
	}
}
