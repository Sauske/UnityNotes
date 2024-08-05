/**
 * File: graph_dfs.cs
 * Created Time: 2023-03-08
 * Author: hpstory (hpstory1024@163.com)
 */
using UnityEngine;
using hello_algo.utils;
using System.Collections.Generic;

namespace hello_algo.chapter_graph
{
    public class graph_dfs
    {
        /* 深度优先遍历辅助函数 */
        void DFS(GraphAdjList graph, HashSet<Vertex> visited, List<Vertex> res, Vertex vet)
        {
            res.Add(vet);     // 记录访问顶点
            visited.Add(vet); // 标记该顶点已被访问
                              // 遍历该顶点的所有邻接顶点
            foreach (Vertex adjVet in graph.adjList[vet])
            {
                if (visited.Contains(adjVet))
                {
                    continue; // 跳过已被访问的顶点                             
                }
                // 递归访问邻接顶点
                DFS(graph, visited, res, adjVet);
            }
        }

        /* 深度优先遍历 */
        // 使用邻接表来表示图，以便获取指定顶点的所有邻接顶点
        List<Vertex> GraphDFS(GraphAdjList graph, Vertex startVet)
        {
            // 顶点遍历序列
            List<Vertex> res = new List<Vertex>();
            // 哈希集合，用于记录已被访问过的顶点
            HashSet<Vertex> visited = new HashSet<Vertex>();
            DFS(graph, visited, res, startVet);
            return res;
        }

        [TestAttibute]
        public void Test()
        { 
            /* 初始化无向图 */
            Vertex[] v = Vertex.ValsToVets(new int[] { 0, 1, 2, 3, 4, 5, 6 });
            Vertex[][] edges = new Vertex[][] { };
            edges[0] = new Vertex[] { v[0], v[1] };
            edges[1] = new Vertex[] { v[0], v[3] };
            edges[2] = new Vertex[] { v[1], v[2] };
            edges[3] = new Vertex[] { v[2], v[5] };
            edges[4] = new Vertex[] { v[4], v[5] };
            edges[5] = new Vertex[] { v[5], v[6] };       

            GraphAdjList graph = new GraphAdjList(edges);
            Debug.Log("\n初始化后，图为");
            graph.Print();

            /* 深度优先遍历 */
            List<Vertex> res = GraphDFS(graph, v[0]);
            Debug.Log("\n深度优先遍历（DFS）顶点序列为");
            Debug.Log(string.Join(" ", Vertex.VetsToVals(res)));
        }
    }
}
