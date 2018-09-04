public class HexPath {

	private HexNode fromNodeVal;
	private HexNode toNodeVal;

	public HexNode fromNode { get { return fromNodeVal; } }

	public HexNode toNode { get { return toNodeVal; } }

	public int Cost { get { return toNodeVal.enterCost; } }

	public HexPath(HexNode fromNode, HexNode toNode)
	{
		fromNodeVal = fromNode;
		toNodeVal = toNode;
	}
}
