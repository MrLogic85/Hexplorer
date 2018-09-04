using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Configuration;
using System.Linq;
using UnityEngine.Networking.Types;

public class NavigationController : MonoBehaviour {

	public NavMeshSurface navMesh;

	private PlayerController player;
	private HexMapController mapController;

	private Dictionary<HexPos, HexNode> navigationNodes;

	private bool isPlayerNavigating;
	private List<HexNode> moveNodes;

	// Use this for initialization
	void Start()
	{
		player = FindObjectOfType<PlayerController>();
		mapController = GetComponent<HexMapController>();
	}

	void Update()
	{
		if (isPlayerNavigating) {
			if (player.navMeshAgent.hasPath &&
			    player.navMeshAgent.remainingDistance <= player.navMeshAgent.stoppingDistance * 4) {
				moveNodes.RemoveAt(0);

				if (moveNodes.Count > 0) {
					MoveTo(moveNodes[0], moveNodes.Count == 1);
				} else {
					isPlayerNavigating = false;
				}
			}
		}
	}

	public void BuildNavMap()
	{
		navigationNodes = new Dictionary<HexPos, HexNode>();

		foreach (HexTileController controller in mapController.tileControllers) {
			navigationNodes.Add(controller.pos, new HexNode(
				pos: controller.pos,
				enterCost: controller.enterCost,
				isRevealed: controller.flipped));
		}

		foreach (HexNode node in navigationNodes.Values) {
			List<HexTileController> neighbours = mapController.GetNeighbours(node.pos);

			foreach (HexTileController neighbour in neighbours) {
				HexNode toNode;
				if (IsTraversible(neighbour.tileType) && navigationNodes.TryGetValue(neighbour.pos, out toNode)) {
					node.AddNeighbour(toNode);
				}
			}
		}

		if (navMesh) {
			navMesh.BuildNavMesh();
		}
	}

	public int GetMoveDistance(HexPos fromHexPos, HexPos toHexPos, bool forcedReveal = false)
	{
		HexNode fromNode, toNode;

		if (navigationNodes.TryGetValue(fromHexPos, out fromNode) &&
		    navigationNodes.TryGetValue(toHexPos, out toNode)) {

			if (toNode.enterCost >= 0 && (toNode.isRevealed || forcedReveal)) {
				var path = GetShortestPathAstar(fromNode, toNode);

				if (path.Count > 1) {
					path.RemoveAt(0);
					return path.Aggregate(0, (sum, node) => sum + node.enterCost);
				}
			}
		}

		return -1;
	}

	public void NavigateTo(HexPos toHexPos)
	{
		HexPos fromHexPos = mapController.GetHexPositionFor(player.transform.position);
		HexNode fromNode, toNode;

		if (navigationNodes.TryGetValue(fromHexPos, out fromNode) &&
		    navigationNodes.TryGetValue(toHexPos, out toNode)) {

			if (fromNode.CanReach(toHexPos, player.moveDistance)) {
				isPlayerNavigating = true;

				moveNodes = GetShortestPathAstar(fromNode, toNode);
				if (moveNodes.Count > 1) {
					// Remove start node
					moveNodes.RemoveAt(0);
					MoveTo(moveNodes[0], moveNodes.Count == 1);
				}
			}
		}
	}

	private void MoveTo(HexNode node, bool isEndPoint = false)
	{
		var navMeshAgent = player.navMeshAgent;
		var pos = mapController.GetPositionFor(node.pos);

		if (navMeshAgent.CalculatePath(pos, new NavMeshPath())) {
			navMeshAgent.destination = pos;
			navMeshAgent.autoBraking = isEndPoint;
		}
		mapController.RevealPosition(node.pos, player.viewDistance);
	}

	public bool IsPlayerNavigating()
	{
		return isPlayerNavigating;
	}

	public void SetPossibleMoveDistance(HexPos pos, int distance)
	{
		var tiles = mapController.GetNeighbours(pos, distance);
		mapController.ClearHighlights();

		foreach (HexTileController tile in tiles) {
			var moveDist = GetMoveDistance(pos, tile.pos, forcedReveal: true);
			if (moveDist > 0 && moveDist <= distance) {
				tile.SetHighlighted(true);
			}
		}
	}

	private bool IsTraversible(TileEnum tileType)
	{
		switch (tileType) {
			case TileEnum.GRASS:
			case TileEnum.HILL:
				return true;
			default:
				return false;
		}
	}

	public List<HexNode> GetShortestPathAstar(HexNode start, HexNode end)
	{
		foreach (HexNode node in navigationNodes.Values) {
			node.straightLineDistanceToEnd = node.pos.DistanceTo(start.pos);
			node.minCostToStart = null;
			node.nearestToStart = null;
			node.visited = false;
		}

		AstarSearch(start, end);
		var shortestPath = new List<HexNode>();
		shortestPath.Add(end);
		BuildShortestPath(shortestPath, end);
		shortestPath.Reverse();
		return shortestPath;
	}

	private void BuildShortestPath(List<HexNode> list, HexNode node)
	{
		if (node.nearestToStart == null)
			return;
		list.Add(node.nearestToStart);
		BuildShortestPath(list, node.nearestToStart);
	}

	private void AstarSearch(HexNode start, HexNode end)
	{
		start.minCostToStart = 0;
		var prioQueue = new List<HexNode>();
		prioQueue.Add(start);
		do {
			prioQueue = prioQueue.OrderBy(x => x.minCostToStart + x.straightLineDistanceToEnd).ToList();
			var node = prioQueue.First();
			prioQueue.Remove(node);
			//nodeVisits++;

			foreach (var pathToNeighbour in node.Neighbours.OrderBy(x => x.Cost)) {
				var neighbourNode = pathToNeighbour.toNode;

				if (neighbourNode.visited || !(neighbourNode.isRevealed || neighbourNode.pos == end.pos)) {
					continue;
				} else if (neighbourNode.minCostToStart == null || node.minCostToStart + pathToNeighbour.Cost < neighbourNode.minCostToStart) {
					neighbourNode.minCostToStart = node.minCostToStart + pathToNeighbour.Cost;
					neighbourNode.nearestToStart = node;

					if (!prioQueue.Contains(neighbourNode)) {
						prioQueue.Add(neighbourNode);
					}
				}
			}

			node.visited = true;

			if (node == end) {
				return;
			}
		} while (prioQueue.Any());
	}
}
