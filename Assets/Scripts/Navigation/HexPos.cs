using System;
using System.Xml.Linq;

[System.Serializable]
public class HexPos {
	
	public int x;
	public int y;

	public int z { get { return -x - y; } }

	public HexPos(int x, int y)
	{
		this.x = x;
		this.y = y;
	}

	public override bool Equals(object obj)
	{
		var item = obj as HexPos;

		if (item == null) {
			return false;
		}

		return this.x == item.x && this.y == item.y;
	}

	public override int GetHashCode()
	{
		return this.x.GetHashCode() + this.y.GetHashCode();
	}

	public int DistanceTo(HexPos pos)
	{
		return HexPos.GetDistance(this, pos);
	}

	public static int GetDistance(HexPos from, HexPos to)
	{
		return HexPos.GetDistance(from.x, from.y, to.x, to.y);
	}

	public static int GetDistance(int fromX, int fromY, int toX, int toY)
	{
		return (Math.Abs(fromX - toX) + Math.Abs(fromY - toY) + Math.Abs(toX + toY - fromX - fromY)) / 2;
	}
}