using System.Collections.Generic;

namespace Graph
{
    public class Vertex
    {
        public VertexStruct Data;
        public bool isVisited;
        public List<Vertex> neighbors;
        public Vertex()
        {
            neighbors = new List<Vertex>();
        }

        public void AddNeighbor(Vertex item)
        {
            if (!HasNeighbor(item))
            {
                neighbors.Add(item);
            }
        }

        public bool HasNeighbor(Vertex item)
        {
            return neighbors.Contains(item);
        }

        public List<Vertex> GetNeighbors()
        {
            return neighbors;
        }
    }
}