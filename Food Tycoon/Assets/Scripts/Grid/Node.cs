using UnityEngine;
using System.Collections;

public class Node : IHeapItem<Node>
{
    public Vector3 WorldPosition;
    public NodeGridPosition GridPos;

    public bool Walkable;
    public bool Occupied;

    public int gCost;
    public int hCost;
    public int fCost { get { return gCost + hCost; } }

    private int heapIndex;
    public int HeapIndex
    {
        get { return heapIndex; }
        set { heapIndex = value; }
    }
    

    public Node Parent;

    /// <summary>
    /// Constructor, It checks automatically if it is walkable or not
    /// </summary>
    /// <param name="_WorldPosition"></param>
    /// <param name="_gridX"></param>
    /// <param name="_gridY"></param>
    public Node(Vector3 _WorldPosition, int _gridX, int _gridY)
    {        
        WorldPosition = _WorldPosition;
        GridPos = new NodeGridPosition(_gridX, _gridY);

        //This makes it a bit more secure
        Walkable = CheckIfWalkable();
    }

    /// <summary>
    /// Constructor, It checks automatically if it is walkable or not
    /// </summary>
    /// <param name="_WorldPosition"></param>
    /// <param name="_GridPos"></param>
    public Node(Vector3 _WorldPosition, NodeGridPosition _GridPos)
    {        
        WorldPosition = _WorldPosition;
        GridPos = new NodeGridPosition(_GridPos);

        //This makes it a bit more secure
        Walkable = CheckIfWalkable();
    }


    /// <summary>
    /// Sets This node's Walkable to the result of CheckIfWalkable
    /// </summary>
    public void UpdateWalkable()
    {
        Walkable = CheckIfWalkable();
    }
    
    private bool CheckIfWalkable()
    {
        return (!Physics.CheckSphere(WorldPosition, Grid.Instance.nodeRadius, Grid.Instance.UnwalkableMask));
    }

    public int CompareTo(Node nodeToCompareTo)
    {
        int compare = fCost.CompareTo(nodeToCompareTo.fCost);
        if(compare == 0)
        {
            compare = hCost.CompareTo(nodeToCompareTo.hCost);
        }

        return -compare;
    }
}


[System.Serializable]
public struct NodeGridPosition
{
    public int x;
    public int y;

    public static NodeGridPosition Null { get {return new NodeGridPosition(-1,-1); }}

    public NodeGridPosition(int _x, int _y)
    {
        x = _x;
        y = _y;
    }

    public NodeGridPosition(NodeGridPosition _GridPos)
    {
        x = _GridPos.x;
        y = _GridPos.y;
    }


    public static bool operator ==  (NodeGridPosition GridPos1, NodeGridPosition GridPos2)
    {
        return (GridPos1.x == GridPos2.x && GridPos1.y == GridPos2.y);
    }

    public static bool operator != (NodeGridPosition GridPos1, NodeGridPosition GridPos2)
    {
        return !(GridPos1.x == GridPos2.x && GridPos1.y == GridPos2.y);
    }

    

}
