using System.Collections.Generic;
using UnityEngine;

public static class GridPathfinder
{
    #region A* Pathfinder
    
    private static readonly Vector3[] Directions =
    {
        new Vector3(0, 1, 0),
        new Vector3(0, -1, 0),
        new Vector3(1, 0, 0),
        new Vector3(-1, 0, 0),
    };
    
    private class Node
    {
        public Vector3 Position;
        public Node Parent;
        public float GCost;
        public float HCost;
        public float FCost => GCost + HCost;
    }

    public static List<Vector3> FindPath(GridMap gridMap, Vector3 startWorld, Vector3 targetWorld, GridZone allowedZone)
    {
        Vector3 start = gridMap.GetGridPosition(startWorld);
        Vector3 target = gridMap.GetGridPosition(targetWorld);

        if (gridMap.GetZoneAt(start) != allowedZone || gridMap.GetZoneAt(targetWorld) != allowedZone)
        {
            Debug.LogError("Can't find path Either the start or end is not is same Allowed Zone!");
            return null;
        }
        
        var openSet     = new List<Node>();
        var openLookup  = new HashSet<Vector3>();
        var closeSet    = new HashSet<Vector3>();
        var allNodes    = new Dictionary<Vector3, Node>();
        
        var startNode = new Node
        {
            Position = start,
            GCost = 0,
            HCost = Heuristic(start, target)
        };
        
        openSet.Add(startNode);
        openLookup.Add(start);
        allNodes[start] = startNode;

        while (openSet.Count > 0)
        {
            int bestIndex = GetLowestFCostIndex(openSet);
            var currentNode = openSet[bestIndex];

            if (currentNode.Position == target)
            {
                return RetracePath(currentNode);
            }
            
            openSet.Remove(currentNode);
            openLookup.Remove(currentNode.Position);
            closeSet.Add(currentNode.Position);

            float cellSize = gridMap.GetCellSizeAt(currentNode.Position);
            if (cellSize <= 0)
                continue; 
            
            foreach (var dir in Directions)
            {
                Vector3 rawNeighbor = currentNode.Position + dir * cellSize;
                Vector3 neighborPos = gridMap.GetGridPosition(rawNeighbor);
                
                if (closeSet.Contains(neighborPos))
                    continue;
                
                GridZone neighborZone = gridMap.GetZoneAt(neighborPos);
                if(neighborZone != allowedZone)
                    continue;
                
                float tentativeGCost = currentNode.GCost + 1f;

                if (!allNodes.TryGetValue(neighborPos, out var neighborNode))
                {
                    neighborNode = new Node {Position = neighborPos};
                    allNodes[neighborPos] = neighborNode;
                }
                else if (tentativeGCost > neighborNode.GCost)
                {
                     continue;
                }
                
                neighborNode.Parent = currentNode;
                neighborNode.GCost  = tentativeGCost;
                neighborNode.HCost  = Heuristic(neighborPos, neighborNode.Position);

                if (!openLookup.Contains(neighborPos))
                {
                    openSet.Add(neighborNode);
                    openLookup.Add(neighborPos);
                }
            }
        }

        return null;
    }

    private static int GetLowestFCostIndex(List<Node> openSet)
    {
        int bestIndex = 0;
        for (int i = 0; i < openSet.Count; i++)
        {
            if (openSet[i].FCost < openSet[bestIndex].FCost ||
                (Mathf.Approximately(openSet[i].FCost, openSet[bestIndex].FCost) &&
                 openSet[i].HCost < openSet[bestIndex].HCost))
            {
                bestIndex = i;
            }
        }
        
        return bestIndex;
    }

    private static float Heuristic(Vector3 a, Vector3 b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }

    private static List<Vector3> RetracePath(Node endNode)
    {
        var path = new List<Vector3>();
        var current = endNode;
        while (current != null)
        {
            path.Add(current.Position);
            current = current.Parent;
        }
        path.Reverse();
        return path;
    }
    
    #endregion

    #region Find Closes Path
    
    public static bool TryFindNearPath(GridMap gridMap, Vector3 startPosition, GridZone allowedZone, out Vector3 nearest)
    {
        return gridMap.TryGetNearestZoneCell(startPosition, allowedZone, out nearest);
    }

    #endregion
}
