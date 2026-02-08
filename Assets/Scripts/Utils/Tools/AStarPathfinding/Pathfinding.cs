using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BullBrukBruker
{
    public static class Pathfinding
    {
        private const int MOVE_STRAIGHT_COST = 10;
        private const int MOVE_DIAGONAL_COST = 14;

        private static Dictionary<Vector3Int, PathNode> nodes;

        private static void ResetData()
        {
            nodes ??= new();
            nodes.Clear();
        }

        public static List<PathNode> FindPath(Vector3Int startIndex, Vector3Int endIndex, params KeyValuePair<Vector3Int, int>[] nodeCosts)
        {
            ResetData();

            InitializeNodes(startIndex, endIndex);

            SetNodeCosts(nodeCosts);

            return GeneratePath(startIndex, endIndex);
        }

        private static void InitializeNodes(Vector3Int startIndex, Vector3Int endIndex)
        {
            for (int x = startIndex.x; x <= endIndex.x; x++)
            {
                for (int y = startIndex.y; y <= endIndex.y; y++)
                {
                    var currentIndex = new Vector3Int(x, y, 0);

                    PathNode pathNode = new(currentIndex);

                    nodes[currentIndex] = pathNode;
                }
            }
        }

        private static void SetNodeCosts(params KeyValuePair<Vector3Int, int>[] nodeCosts)
        {
            foreach (var nodeCost in nodeCosts)
                if (nodes.ContainsKey(nodeCost.Key))
                    nodes[nodeCost.Key].BiasCost = nodeCost.Value;
        }

        public static List<PathNode> FindRandomizedPath(Vector3Int startIndex, Vector3Int endIndex)
        {
            ResetData();

            InitializeRandomBiasedNodes(startIndex, endIndex);

            return GeneratePath(startIndex, endIndex);
        }

        private static void InitializeRandomBiasedNodes(Vector3Int startIndex, Vector3Int endIndex)
        {
            for (int x = startIndex.x; x <= endIndex.x; x++)
            {
                for (int y = startIndex.y; y <= endIndex.y; y++)
                {
                    var currentIndex = new Vector3Int(x, y, 0);

                    PathNode pathNode = new RandomPathNode(currentIndex);

                    nodes[currentIndex] = pathNode;
                }
            }
        }

        private static List<PathNode> GeneratePath(Vector3Int startIndex, Vector3Int endIndex)
        {
            var openList = new List<PathNode> { nodes[startIndex] };
            var closedList = new List<PathNode>();

            PathNode startNode = nodes[startIndex];
            PathNode endNode = nodes[endIndex];

            startNode.GCost = startNode.BiasCost;
            startNode.HCost = CalculateDistanceCost(startNode, endNode);
            startNode.CalculateFCost();

            while (openList.Count > 0)
            {
                PathNode currentNode = GetLowestFCostNode(openList);
                if (currentNode == endNode)
                    return CalculatePath(endNode);

                openList.Remove(currentNode);
                closedList.Add(currentNode);

                foreach (PathNode neighbourNode in GetNeighbourList(currentNode))
                {
                    if (closedList.Contains(neighbourNode)) continue;

                    float tentativeGCost = currentNode.GCost + CalculateDistanceCost(currentNode, neighbourNode) + neighbourNode.BiasCost;
                    if (tentativeGCost < neighbourNode.GCost)
                    {
                        neighbourNode.PreviousNode = currentNode;

                        neighbourNode.GCost = tentativeGCost;
                        neighbourNode.HCost = CalculateDistanceCost(neighbourNode, endNode);
                        neighbourNode.CalculateFCost();

                        if (!openList.Contains(neighbourNode))
                            openList.Add(neighbourNode);
                    }
                }
            }

            return null;
        }

        private static List<PathNode> GetNeighbourList(PathNode currentNode)
        {
            List<PathNode> neighbourList = new();

            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    if (i == 0 && j == 0)
                        continue;

                    var index = currentNode.Index + new Vector3Int(i, j);
                    if (TryGetNode(index, out var neighborNode))
                        neighbourList.Add(neighborNode);
                }
            }

            return neighbourList;
        }

        private static bool TryGetNode(Vector3Int index, out PathNode node) {
            if (nodes.TryGetValue(index, out node))
                return true;

            return false;
        }

        private static List<PathNode> CalculatePath(PathNode endNode)
        {
            List<PathNode> path = new()
            {
                endNode
            };

            PathNode currentNode = endNode;

            while (currentNode.PreviousNode != null)
            {
                path.Add(currentNode.PreviousNode);
                currentNode = currentNode.PreviousNode;
            }

            path.Reverse();
            return path;
        }

        private static int CalculateDistanceCost(PathNode a, PathNode b)
        {
            int xDistance = Mathf.Abs(a.Index.x - b.Index.x);
            int yDistance = Mathf.Abs(a.Index.y - b.Index.y);
            int remaining = Mathf.Abs(xDistance - yDistance);

            return MOVE_DIAGONAL_COST * Mathf.Min(xDistance, yDistance) + MOVE_STRAIGHT_COST * remaining;
        }

        private static PathNode GetLowestFCostNode(List<PathNode> pathNodeList)
        {
            PathNode lowestFCostNode = pathNodeList[0];

            for (int i = 1; i < pathNodeList.Count; i++)
                if (pathNodeList[i].FCost < lowestFCostNode.FCost)
                    lowestFCostNode = pathNodeList[i];

            return lowestFCostNode;
        }

    }
}
