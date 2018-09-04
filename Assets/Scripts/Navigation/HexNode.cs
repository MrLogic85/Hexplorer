using System.Collections.Generic;
using NUnit.Framework;
using System;

public class HexNode {
	
	public HexPos pos;
	public int enterCost;
	public bool stopOnEnter;
	public bool isRevealed;

	private List<HexPath> neighbours = new List<HexPath>();

	public List<HexPath> Neighbours { get { return neighbours; } }

	// Navigation parameters
	public int straightLineDistanceToEnd;
	public Int32? minCostToStart;
	public HexNode nearestToStart;
	public bool visited;

	public HexNode(HexPos pos, int enterCost = 1, bool stopOnEnter = false, bool isRevealed = false)
	{
		this.pos = pos;
		this.enterCost = enterCost;
		this.stopOnEnter = stopOnEnter;
		this.isRevealed = isRevealed;

		HexTileController.OnTileFlip += (tilePos) => {
			if (tilePos == this.pos) {
				Reveal();
			}
		};
	}

	public bool IsAt(HexPos pos)
	{
		return this.pos.Equals(pos);
	}

	public void Reveal()
	{
		isRevealed = true;
	}

	public bool AddNeighbour(HexNode neighbour)
	{
		foreach (HexPath path in neighbours) {
			if (path.toNode.IsAt(neighbour.pos))
				return false;
		}

		var outPath = new HexPath(this, neighbour);
		neighbours.Add(outPath);

		return true;
	}

	public bool HasNeighbour(HexNode neighbour)
	{
		return HasNeighbour(neighbour.pos);
	}

	public bool HasNeighbour(HexPos pos)
	{
		foreach (HexPath path in neighbours) {
			if (path.toNode.IsAt(pos)) {
				return true;
			}
		}

		return false;
	}

	public bool CanReach(HexNode node, int steps)
	{
		return CanReach(node.pos, steps);
	}

	public bool CanReach(HexPos pos, int steps)
	{
		if (this.IsAt(pos)) {
			return false;
		}

		foreach (HexPath path in neighbours) {
			if (path.toNode.isRevealed && path.toNode.TryMoveTo(pos, steps)) {
				return true;
			}
		}

		return false;
	}

	private bool TryMoveTo(HexPos pos, int steps)
	{
		if (steps < enterCost) {
			return false;
		}

		steps -= enterCost;

		if (this.IsAt(pos)) {
			return true;
		}

		if (stopOnEnter) {
			return false;
		}

		foreach (HexPath path in neighbours) {
			if (path.toNode.isRevealed && path.toNode.TryMoveTo(pos, steps)) {
				return true;
			}
		}

		return false;
	}
}
