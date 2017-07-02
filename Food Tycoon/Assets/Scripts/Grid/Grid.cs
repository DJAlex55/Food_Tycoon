using UnityEngine;
using System.Collections.Generic;

public class Grid: MonoBehaviour
{
    public static Grid Instance { get; private set; }

    public Node[,] grid { get; private set; }
    [SerializeField] private Vector2 GridWorldSize;
    [SerializeField] private Vector3 OffSet;
    public LayerMask UnwalkableMask;
    public float nodeRadius;

    public GridCollider GridCollider;

    public bool DrawGridInEditMode;

    private float nodeDiameter;

    private NodeGridPosition _gridSize;
    public NodeGridPosition GridSize { get { return _gridSize; } }

    public int MaxSize { get { return GridSize.x * GridSize.y; } }


    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
        {
            Debug.LogError(gameObject + " was a second Instance! and was destroyed for it!");
            Destroy(gameObject);
        }



        nodeDiameter = nodeRadius * 2;
        _gridSize.x = Mathf.RoundToInt(GridWorldSize.x / nodeDiameter);
        _gridSize.y = Mathf.RoundToInt(GridWorldSize.y / nodeDiameter);

        CreateGrid();        
    }

    private void Start()
    {
        if (GridCollider != null)
        {
            GridCollider.Set(transform.position + OffSet, new Vector3(GridWorldSize.x, 0.5f, GridWorldSize.y));
        }
    }
    

    private void OnDrawGizmos()
    {
        if(DrawGridInEditMode)
            DrawGrid();
        
        Gizmos.DrawWireCube(transform.position+OffSet, new Vector3(GridWorldSize.x, 0, GridWorldSize.y));        
    }



    private void DrawGrid()
    {
        Vector3 WorldBottomLeft = OffSet + transform.position - Vector3.right * (GridWorldSize.x / 2 - nodeRadius) - Vector3.forward * (GridWorldSize.y / 2 - nodeRadius);
        Vector3 WorldBottomRight = WorldBottomLeft + Vector3.right * (GridWorldSize.x + nodeRadius);
        Vector3 WorldTopLeft = WorldBottomLeft + Vector3.forward * (GridWorldSize.y - nodeRadius);

        //draws the vertical lines
        for (int x = 0; x <= GridSize.x; x++)
        { 
            Vector3 Start = WorldBottomLeft + Vector3.right * x * nodeDiameter;
            Vector3 End = WorldTopLeft + Vector3.right * x * nodeDiameter;

            Gizmos.color = Color.white;
            Gizmos.DrawLine(Start, End);
        }

        //draws the orizontal lines
        for (int y = 0; y < GridSize.y; y++)
        {
            Vector3 Start = WorldBottomLeft + Vector3.forward * y * nodeDiameter;
            Vector3 End = WorldBottomRight + Vector3.forward * y * nodeDiameter;

            Gizmos.color = Color.white;
            Gizmos.DrawLine(Start, End);
        }

    }



    private void CreateGrid()
    {
        grid = new Node[GridSize.x, GridSize.y];

        Vector3 WorldBottomLeft = OffSet + transform.position -  Vector3.right * GridWorldSize.x / 2- Vector3.forward * GridWorldSize.y / 2;

        for(int y = 0; y < GridSize.x; y++)
        {
            for (int x = 0; x < GridSize.y; x++)
            {
                Vector3 worldPoint = WorldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);
                grid[x, y] = new Node(worldPoint,x,y);
            }
        }
    }

    #region UpdateGrid

    public void UpdateGrid()
    {
        for (int x = 0; x < GridSize.x; x++)
        {
            for (int y = 0; y < GridSize.y; y++)
            {
                grid[x, y].UpdateWalkable();        
            }
        }
    }


    public void UpdateGrid(NodeGridPosition from, NodeGridPosition to)
    {
        if (CheckIfOutOfBounds(from) || CheckIfOutOfBounds(to))
            return;

        int x = from.x;

        while (x != to.x)
        {
            int y = from.y;

            while (y != to.y)
            {
                grid[x, y].UpdateWalkable();

                if (from.y < to.y)
                    y++;
                else
                    y--;
            }

            if (from.x < to.x)
                x++;
            else
                x--;

        }
    }

    #endregion

    public static Node WorldPointToNode(Vector3 WorldPoint)
    {
        float percentX = (WorldPoint.x - Instance.OffSet.x + (Instance.GridWorldSize.x / 2)) / Instance.GridWorldSize.x;
        float percentY = (WorldPoint.z - Instance.OffSet.z + (Instance.GridWorldSize.y  / 2)) / Instance.GridWorldSize.y;
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);
        int x = Mathf.RoundToInt((Instance.GridSize.x - 1) * percentX);
        int y = Mathf.RoundToInt((Instance.GridSize.y - 1) * percentY);
        return Instance.grid[x, y];
    }

    public static NodeGridPosition WorldPointToNodeGridPosition(Vector3 WorldPoint)
    {
        float percentX = (WorldPoint.x - Instance.OffSet.x + (Instance.GridWorldSize.x / 2)) / Instance.GridWorldSize.x;
        float percentY = (WorldPoint.z - Instance.OffSet.z + (Instance.GridWorldSize.y / 2)) / Instance.GridWorldSize.y;
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);
        int x = Mathf.RoundToInt((Instance.GridSize.x - 1) * percentX);
        int y = Mathf.RoundToInt((Instance.GridSize.y - 1) * percentY);
        return new NodeGridPosition( x, y );
    }

    #region GetWorldPointFromNodeGridPosition

    public static Vector3 GetWorldPointFromNodeGridPosition(NodeGridPosition GridPos )
    {
        if (!Instance.CheckIfOutOfBounds(GridPos))
        {
            Node node = Instance.grid[GridPos.x, GridPos.y];
            return node.WorldPosition;
        }
        return Vector3.zero;
    }

    public static Vector3 GetWorldPointFromNodeGridPosition(NodeGridPosition GridPos, out bool Success)
    {
        if (!Instance.CheckIfOutOfBounds(GridPos))
        {
            Node node = Instance.grid[GridPos.x, GridPos.y];
            Success = true;
            return node.WorldPosition;
        }
        Success = false;
        return Vector3.zero;
    }

    public static Vector3 GetWorldPointFromNodeGridPosition(int x, int y)
    {
        if (!Instance.CheckIfOutOfBounds(x,y))
        {
            Node node = Instance.grid[x, y];
            return node.WorldPosition;
        }
        return Vector3.zero;
    }

    public static Vector3 GetWorldPointFromNodeGridPosition(int x, int y, out bool Success)
    {
        if (!Instance.CheckIfOutOfBounds(x,y))
        {
            Node node = Instance.grid[x, y];
            Success = true;
            return node.WorldPosition;
        }
        Success = false;
        return Vector3.zero;
    }


    #endregion

    #region GetNehigbours

    public List<Node> GetNeighbours(Node node)
    {
        List<Node> Neighbours = new List<Node>();


        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)
                    continue;

                int checkX = node.GridPos.x + x;
                int checkY = node.GridPos.y + y;

                if (!CheckIfOutOfBounds(checkX, checkY))
                    Neighbours.Add( grid[checkX, checkY]);

            }
        }

        return Neighbours;

    }

    public List<Node> GetNeighbours(NodeGridPosition GridPos)
    {
        List<Node> Neighbours = new List<Node>();
        

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)
                    continue;

                int checkX = GridPos.x + x;
                int checkY = GridPos.y + y;

                if (!CheckIfOutOfBounds(checkX, checkY))
                    Neighbours.Add(grid[checkX, checkY]);

            }
        }

        return Neighbours;

    }

    #endregion


    public List<NodeGridPosition> GetNodeLineFromNodes(NodeGridPosition StartNodeGridPos, NodeGridPosition EndNodeGridPos)
    {
        if (CheckIfOutOfBounds(StartNodeGridPos))
            return null;

        List<NodeGridPosition> Line = new List<NodeGridPosition>();

        int DifferenceX = Mathf.Abs(EndNodeGridPos.x - StartNodeGridPos.x);
        int DifferenceY = Mathf.Abs(EndNodeGridPos.y - StartNodeGridPos.y);

        NodeGridPosition CurrentNodeGridPos = StartNodeGridPos;


        //If true we will get a line of nodes along the X axis of the lenght of DifferenceX
        if (DifferenceX > DifferenceY)
        {
            int X = StartNodeGridPos.x;

            EndNodeGridPos = new NodeGridPosition(EndNodeGridPos.x, StartNodeGridPos.y);
            

            while (CurrentNodeGridPos != EndNodeGridPos)
            {
                CurrentNodeGridPos = new NodeGridPosition(X, StartNodeGridPos.y);

                if (!CheckIfOutOfBounds(CurrentNodeGridPos))                    
                    Line.Add(CurrentNodeGridPos);

                if (StartNodeGridPos.x < EndNodeGridPos.x)
                    X++;
                else
                    X--;
            }

        }
        //else we will get a line of nodes along the Y axis of the lenght of DifferenceY
        else
        {
            int Y = StartNodeGridPos.y;

            EndNodeGridPos = new NodeGridPosition( StartNodeGridPos.x, EndNodeGridPos.y);


            while (CurrentNodeGridPos != EndNodeGridPos)
            {
                CurrentNodeGridPos = new NodeGridPosition(StartNodeGridPos.x, Y);

                if (!CheckIfOutOfBounds(CurrentNodeGridPos))
                    Line.Add(CurrentNodeGridPos);                

                if (StartNodeGridPos.y < EndNodeGridPos.y)
                    Y++;
                else
                    Y--;
            }
        }

        return Line;

    }
    
    public List<NodeGridPosition> GetNodeAreaFromNodes(NodeGridPosition StartNodeGridPos, NodeGridPosition EndNodeGridPos)
    {
        if (StartNodeGridPos == EndNodeGridPos)
            return new List<NodeGridPosition> { StartNodeGridPos };

        List<NodeGridPosition> Area = new List<NodeGridPosition>();

        int DifferenceX = Mathf.Abs(EndNodeGridPos.x - StartNodeGridPos.x);
        int DifferenceY = Mathf.Abs(EndNodeGridPos.y - StartNodeGridPos.y);

        NodeGridPosition CurrentNode = StartNodeGridPos;

        int x = StartNodeGridPos.x, y = StartNodeGridPos.y;

        while (CurrentNode != EndNodeGridPos)
        {
            while (CurrentNode.x != EndNodeGridPos.x)
            {
                if (!CheckIfOutOfBounds(x, y))
                {
                    CurrentNode = new NodeGridPosition(x, y);

                    if (CurrentNode != null)
                        Area.Add(CurrentNode);
                }

                if (StartNodeGridPos.x < EndNodeGridPos.x)
                    x++;
                else
                    x--;
            }

            x = StartNodeGridPos.x;


            if (StartNodeGridPos.y < EndNodeGridPos.y)
                y++;
            else
                y--;

        }
        return Area;
    }


    public Node GetNodeFromRayCastOnGrid()
    {
        RaycastHit hit;

        Utility.DoRayCast(out hit, GridCollider.GridColliderMask);

        if (hit.collider != GridCollider.gridCollider)
            return null;

        return WorldPointToNode(hit.point);
    }

    #region CheckIfOutOfBounds

    public bool CheckIfOutOfBounds(NodeGridPosition GridPos)
    {
        return (GridPos.x < 0 || GridPos.x >= GridSize.x || GridPos.y < 0 || GridPos.y >= GridSize.y);
    }

    public bool CheckIfOutOfBounds(int Gridx, int Gridy)
    {
        return (Gridx < 0 || Gridx >= GridSize.x || Gridy < 0 || Gridy >= GridSize.y);
    }

    public bool CheckIfOutOfBounds(Node node)
    {
        return (node.GridPos.x < 0 || node.GridPos.x >= GridSize.x || node.GridPos.y < 0 || node.GridPos.y >= GridSize.y);
    }

    #endregion

    public static Node DoRayCast()
    {
        RaycastHit hit;

        Utility.DoRayCast(out hit, Instance.GridCollider.GridColliderMask);

        if (hit.collider != Instance.GridCollider.gridCollider)
            return null;

        return WorldPointToNode(hit.point);
    }

    public static NodeGridPosition NodeGridPositionFromRayCast()
    {
        RaycastHit hit;

        Utility.DoRayCast(out hit, Instance.GridCollider.GridColliderMask);

        if (hit.collider != Instance.GridCollider.gridCollider)
            return NodeGridPosition.Null;

        return WorldPointToNodeGridPosition(hit.point);
    }

}
