using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class HexTileController : MonoBehaviour {

	public delegate void OnTileFlipEvent(HexPos pos);

	public static event OnTileFlipEvent OnTileFlip;

	public SpriteRenderer highlight;
	public TileEnum tileType;

	[Header("Navigation")]
	public int enterCost;
	public int leaveCost;

	[HideInInspector]
	public HexPos pos;

	private bool flippedValue = false;

	public bool flipped { 
		get { 
			return flippedValue;
		}

		set {
			if (!flippedValue && value == true) {
				GetComponent<Animator>().SetBool("Flipped", value);
			}
			flippedValue = value;
		}
	}

	public void Start()
	{
		highlight = GetComponentInChildren<SpriteRenderer>();
	}

	public void FlipTile()
	{
		GetComponent<Animator>().SetTrigger("Flip");
	}

	public void OnTileFlipped()
	{
		flipped = true;
		if (OnTileFlip != null) {
			OnTileFlip(pos);
		}
	}

	public void SetHighlighted(bool highlighted)
	{
		highlight.enabled = highlighted;
	}
}
