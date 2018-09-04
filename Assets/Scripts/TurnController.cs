using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnController : MonoBehaviour {

	private HexMapBuilder mapBuilder;
	private NavigationController navigationController;
	private PlayerController player;
	private Turn turn;

	void OnGUI()
	{
		if (!player.HasMovesLeft() && GUI.Button(new Rect(Screen.width / 2 - 50, 5, 100, 30), "EndTurn")) {
			StartPlayerTurn();
		}
	}

	void Start()
	{
		mapBuilder = GetComponent<HexMapBuilder>();
		navigationController = GetComponent<NavigationController>();
		player = FindObjectOfType<PlayerController>();
		turn = Turn.INITIALIZE_GAME;
	}

	// Update is called once per frame
	void Update()
	{
		switch (turn) {
			case Turn.INITIALIZE_GAME:
				mapBuilder.CreateMap();
				navigationController.BuildNavMap();
				StartPlayerTurn();
				break;
				
			case Turn.PLAYER:
				player.UpdateTurn();
				break;
		}
	}

	private void StartPlayerTurn()
	{
		player.StartTurn();
		turn = Turn.PLAYER;
	}

	enum Turn
	{
		INITIALIZE_GAME,
		PLAYER
	}
}
