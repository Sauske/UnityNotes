namespace TAStar
{
    public class Node
    {
        public int X { get; set; }
        public int Y { get; set; }
        public bool Walkable { get; set; }
        public int GCost { get; set; }
        public int HCost { get; set; }
        public int FCost { get { return GCost + HCost; } }
        public Node Parent { get; set; }

        public Node(int x, int y, bool walkable)
        {
            X = x;
            Y = y;
            Walkable = walkable;
        }
    }
}