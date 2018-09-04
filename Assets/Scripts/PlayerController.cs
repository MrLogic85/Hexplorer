using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof (Rigidbody))]
[RequireComponent(typeof (NavMeshAgent))]
public class PlayerController : MonoBehaviour {

	private HexMapController mapController;
	private NavigationController navigationController;

	public int moveDistance;
	public int viewDistance;

	private int movesLeft;

	[HideInInspector]
	public NavMeshAgent navMeshAgent;

	// Use this for initialization
	void Start()
	{
		mapController = FindObjectOfType<HexMapController>();
		navigationController = FindObjectOfType<NavigationController>();
		navMeshAgent = GetComponent<NavMeshAgent>();
		navMeshAgent.updateRotation = false;
	}

	// Update is called once per frame
	void Update()
	{
		if (navMeshAgent.desiredVelocity.IsNotZero()) {
			transform.forward = navMeshAgent.desiredVelocity;
		}
	}

	public void StartTurn()
	{
		movesLeft = moveDistance;
		var playerPos = mapController.GetHexPositionFor(transform.position);
		navigationController.SetPossibleMoveDistance(playerPos, moveDistance);
	}

	public void UpdateTurn()
	{
		if (navigationController.IsPlayerNavigating()) {
			return;
		}

		if (Input.GetMouseButtonDown(0)) {
			RaycastHit hit;
			if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100)) {
				var playerPos = mapController.GetHexPositionFor(transform.position);
				var toPos = hit.transform.position;
				var toHexPos = mapController.GetHexPositionFor(toPos);

				var movesRequired = navigationController.GetMoveDistance(playerPos, toHexPos);

				if (movesRequired > 0 && movesRequired <= movesLeft) {
					movesLeft -= movesRequired;
					navigationController.SetPossibleMoveDistance(toHexPos, movesLeft);
					navigationController.NavigateTo(toHexPos);
				}
			}
		}
	}

	public bool HasMovesLeft()
	{
		return movesLeft > 0;
	}
}