using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;

namespace TAStar
{
    public class AStar
    {
        private Node[,] nodes;
        private List<Node> openList;
        private List<Node> closedList;

        public AStar(Node[,] nodes)
        {
            this.nodes = nodes;
        }

        public List<Node> FindPath(Node startNode, Node endNode)
        {
            openList = new List<Node>();
            closedList = new List<Node>();

            openList.Add(startNode);

            while (openList.Count > 0)
            {
                Node currentNode = openList[0];

                for (int i = 1; i < openList.Count; i++)
                {
                    if (openList[i].FCost < currentNode.FCost || openList[i].FCost == currentNode.FCost && openList[i].HCost < currentNode.HCost)
                    {
                        currentNode = openList[i];
                    }
                }

                openList.Remove(currentNode);
                closedList.Add(currentNode);

                if (currentNode == endNode)
                {
                    return GetPath(startNode, endNode);
                }

                foreach (Node neighborNode in GetNeighborNodes(currentNode))
                {
                    if (!neighborNode.Walkable || closedList.Contains(neighborNode))
                    {
                        continue;
                    }

                    int newGCost = currentNode.GCost + GetDistance(currentNode, neighborNode);

                    if (newGCost < neighborNode.GCost || !openList.Contains(neighborNode))
                    {
                        neighborNode.GCost = newGCost;
                        neighborNode.HCost = GetDistance(neighborNode, endNode);
                        neighborNode.Parent = currentNode;

                        if (!openList.Contains(neighborNode))
                        {
                            openList.Add(neighborNode);
                        }
                    }
                }
            }

            return null;
        }

        private List<Node> GetPath(Node startNode, Node endNode)
        {
            List<Node> path = new List<Node>();
            Node currentNode = endNode;

            while (currentNode != startNode)
            {
                path.Add(currentNode);
                currentNode = currentNode.Parent;
            }

            path.Reverse();

            return path;
        }

        private List<Node> GetNeighborNodes(Node node)
        {
            List<Node> neighborNodes = new List<Node>();

            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    if (x == 0 && y == 0)
                    {
                        continue;
                    }

                    int checkX = node.X + x;
                    int checkY = node.Y + y;

                    if (checkX >= 0 && checkX < nodes.GetLength(0) && checkY >= 0 && checkY < nodes.GetLength(1))
                    {
                        neighborNodes.Add(nodes[checkX, checkY]);
                    }
                }
            }

            return neighborNodes;
        }

        private int GetDistance(Node nodeA, Node nodeB)
        {
            int distanceX = Mathf.Abs(nodeA.X - nodeB.X);
            int distanceY = Mathf.Abs(nodeA.Y - nodeB.Y);

            if (distanceX > distanceY)
            {
                return 14 * distanceY + 10 * (distanceX - distanceY);
            }
            else
            {
                return 14 * distanceX + 10 * (distanceY - distanceX);
            }
        }
    }
}
