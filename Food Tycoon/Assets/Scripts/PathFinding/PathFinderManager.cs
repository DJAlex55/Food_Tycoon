using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[RequireComponent(typeof(Grid))]
[RequireComponent(typeof(PathRequestManager))]
public class PathFinderManager : MonoBehaviour
{
    private Grid grid;
    private PathRequestManager requestManager;

    void Awake()
    {
        grid = GetComponent<Grid>();
        requestManager = GetComponent<PathRequestManager>();
    }    

    public void StartFindPath(Vector3 PathStart, Vector3 PathEnd, Action<Vector3[], bool> callBack)
    {
        StartCoroutine(FindPath(PathStart, PathEnd));
    }


    IEnumerator FindPath(Vector3 StartPosition, Vector3 TargetPosition)
    {        
        Node StartNode = Grid.WorldPointToNode(StartPosition);
        Node TargetNode = Grid.WorldPointToNode(TargetPosition);


        Vector3[] waypoints = new Vector3[0];
        bool pathsuccess = false;

        if (StartNode.Walkable && TargetNode.Walkable)
        {

            Heap<Node> openSet = new Heap<Node>(grid.MaxSize);
            HashSet<Node> closedSet = new HashSet<Node>();

            openSet.Add(StartNode);


            while (openSet.Count > 0)
            {
                Node currentNode = openSet.RemoveFirst();

                closedSet.Add(currentNode);

                if (currentNode == TargetNode)
                {
                    //path found
                    pathsuccess = true;

                    break;
                }


                foreach (Node neighbour in grid.GetNeighbours(currentNode))
                {
                    if (!neighbour.Walkable || closedSet.Contains(neighbour))
                        continue;

                    int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);
                    if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                    {
                        neighbour.gCost = newMovementCostToNeighbour;
                        neighbour.hCost = GetDistance(neighbour, TargetNode);
                        neighbour.Parent = currentNode;

                        if (!openSet.Contains(neighbour))
                            openSet.Add(neighbour);
                        else
                            openSet.UpdateItem(neighbour);
                    }

                }

            } 
        }

        yield return null;

        if (pathsuccess)
        {
            waypoints = RetracePath(StartNode, TargetNode);
        }
        requestManager.FinisedProcessingPath(waypoints, pathsuccess);

    }

    
    

    int GetDistance(Node A, Node B)
    {
        int DstX = Mathf.Abs(A.GridPos.x - B.GridPos.x);
        int DstY= Mathf.Abs(A.GridPos.y - B.GridPos.y);

        if (DstX > DstY)
            return 14 * DstY + 10 * (DstX - DstY);
        return 14 * DstX + 10 * (DstY - DstX);
    }

    Vector3[] RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        while(currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.Parent;
        }

        Vector3[] waypoints = SimplifyPath(path);
        Array.Reverse(waypoints);

        return waypoints;

    }

    Vector3[] SimplifyPath(List<Node> path)
    {
        List<Vector3> waypoints = new List<Vector3>();

        Vector2 DirectionOld = Vector2.zero;

        for (int i = 1; i < path.Count; i++)
        {
            Vector2 DirectionNew = new Vector2(path[i - 1].GridPos.x - path[i].GridPos.x, path[i - 1].GridPos.y - path[i].GridPos.y);
            if(DirectionNew != DirectionOld)
            {
                waypoints.Add(path[i].WorldPosition);
                DirectionOld = DirectionNew;
            }
        }
        return waypoints.ToArray();

    }


}
