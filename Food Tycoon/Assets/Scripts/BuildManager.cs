using System.Collections.Generic;
using System;
using UnityEngine;

public class BuildManager : MonoBehaviour
{
    public static BuildManager Instance { get; private set; }

    private GridObject[,] ObjectGrid;
    private Floor[,] FloorGrid;
    private Grid grid;

    [SerializeField] private Transform WallParent;
    [SerializeField] private Transform FloorParent;

    private GridObjectData GridObjectToBuild;
    private NodeGridPosition ObjectToBuildSize;
    /// <summary>
    /// every 1 equals a 90 clockwise degree rotation
    /// </summary>
    private int ObjectToBuildRotation = 0;


    private List<Wall> Walls = new List<Wall>();
    private bool AllWallsShowing = true;

    private bool _buildMode;
    public bool BuildMode
    {
        get { return _buildMode; }
        private set
        {
            _buildMode = value;

            if (value == false)
                BullDozerMode = value;

            if (OnBuildModeChanged != null)
                OnBuildModeChanged();
        }
    }

    private bool _bullDozerMode;
    public bool BullDozerMode
    {
        get { return _bullDozerMode; }
        private set
        {
            _bullDozerMode = value;
            if (OnBullDozerModeChanged != null)
                OnBullDozerModeChanged();
        }
    }

    public Action OnBuildModeChanged;
    public Action OnBullDozerModeChanged;
    public Action OnGridObjectToBuildChanged;
    public Action OnMapChanged;


    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
        {
            Debug.LogError(gameObject + " was a second Instance! and was destroyed for it!");
            Destroy(gameObject);
        }

        grid = Grid.Instance;

        CreateGrid();

        OnGridObjectToBuildChanged += delegate { ObjectToBuildSize = GridObjectToBuild.Size; };
    }
    
    private void CreateGrid()
    {
        ObjectGrid = new GridObject[grid.GridSize.x, grid.GridSize.y];
    }


    public void SetObjectToBuild(GridObjectID ID)
    {
        GridObjectData DATA = IDManager.Instance.GetData(ID);
        if (DATA == null)
            return;

        GridObjectToBuild = DATA;

        if (OnGridObjectToBuildChanged != null)
            OnGridObjectToBuildChanged();
    }
    
    public void SetObjectToBuild(GridObjectData DATA)
    {
        if (DATA == null)
            return;

        GridObjectToBuild = DATA;

        if (OnGridObjectToBuildChanged != null)
            OnGridObjectToBuildChanged();
    }
    


    public void AddWall(Wall wall)
    {
        if (wall == null)
        {
            Debug.LogWarning("Tried to ad a wall wich is null!");
            return;
        }

        if (!Walls.Contains(wall))
            Walls.Add(wall);
    }



    public void SwitchAllUpperWalls()
    {
        AllWallsShowing = !AllWallsShowing;

        foreach (Wall wall in Walls)
        {
            wall.ShowUpWall(AllWallsShowing);
        }
    }


    #region Build

    public bool BuildSingleObject(NodeGridPosition GridPos)
    {
        bool success = BuildObject(GridPos);

        if (success && OnMapChanged != null)
            OnMapChanged();

        return success;
    }


    public bool BuildObjectsLine(NodeGridPosition StartNodeGridPos, NodeGridPosition EndNodeGridPos)
    {
        bool success = true;

        if (GridObjectToBuild.CanBeBuiltInLines)
        {
            foreach (NodeGridPosition GridPos in grid.GetNodeLineFromNodes(StartNodeGridPos, EndNodeGridPos))
            {
                if (!BuildObject(GridPos))
                    success = false;
            }
        }
        else
        {
            success = BuildObject(StartNodeGridPos);
        }

        if (success && OnMapChanged != null)
            OnMapChanged();

        return success;
    }


    private bool BuildObject(NodeGridPosition GridPos)
    {
        if (!BuildMode || GridPos == NodeGridPosition.Null || CheckIfOccupied(GetOccupiedNodes(GridPos)))
            return false;

        if (GridObjectToBuild == null)
        {
            Debug.LogError("GridObjectToBuild is null!");
            return false;
        }



        Vector3 WorldPos = Grid.GetWorldPointFromNodeGridPosition(GridPos);
        
        while (ObjectToBuildRotation < 0)
            ObjectToBuildRotation += 4;

        ObjectToBuildRotation %= 4;

        Quaternion Rot = Quaternion.Euler(0f, ObjectToBuildRotation * 90f, 0f);
        

        GridObject gridObj = Instantiate(GridObjectToBuild.Prefab, WorldPos, Rot);
        gridObj.GridPos = GridPos;
        gridObj.Rot = ObjectToBuildRotation;

        if (GridObjectToBuild.ID == GridObjectID.wall)
        {
            Wall wall = gridObj.GetComponent<Wall>();
            if (wall != null)
            {
                wall.transform.SetParent(WallParent);
                wall.ShowUpWall(AllWallsShowing);
                AddWall(wall);
            }
        }

        //If the size of the object is more that 1 in any axis
        if (gridObj.Size.x > 1 && gridObj.Size.y > 1)
        {
            //We go through all the occupied nodes to tell them they have been occupied
            foreach (NodeGridPosition OccupiedGridPosition in gridObj.GetOccupiedNodes())
            {
                ObjectGrid[OccupiedGridPosition.x, OccupiedGridPosition.y] = gridObj;
                Node occupiedNode = grid.grid[OccupiedGridPosition.x, OccupiedGridPosition.y];
                occupiedNode.Occupied = true;
                occupiedNode.UpdateWalkable();
            }
        }
        else
        {
            ObjectGrid[GridPos.x, GridPos.y] = gridObj;
            Node node = grid.grid[GridPos.x, GridPos.y];
            node.Occupied = true;
            node.UpdateWalkable();
        }



        return true;
    }

    #endregion


    #region Destroy

    public bool DestroySingleGridObject(NodeGridPosition GridPos)
    {
        bool success = DestroyGridObject(GridPos);
        
        if (success && OnMapChanged != null)
            OnMapChanged();

        return success;
    }

    public bool DestroyLineOfGridObject(NodeGridPosition StartNodeGridPos, NodeGridPosition EndNodeGridPos)
    {
        bool success = false;
        
        foreach (NodeGridPosition GridPos in grid.GetNodeLineFromNodes(StartNodeGridPos, EndNodeGridPos))
        {
            if (DestroyGridObject(GridPos))
                success = true;
        }
        if (success && OnMapChanged != null)
            OnMapChanged();

        return success;
    }

    private bool DestroyGridObject(GridObject GridObj)
    {
        if (GridObj == null || !BullDozerMode || !BuildMode )
            return false;


        List<NodeGridPosition> NodesOccupied = GridObj.GetOccupiedNodes();

        Destroy(GridObj.gameObject);

        foreach (NodeGridPosition OccupiedGridPos in NodesOccupied)
        {

            ObjectGrid[OccupiedGridPos.x, OccupiedGridPos.y] = null;
            grid.grid[OccupiedGridPos.x, OccupiedGridPos.y].Occupied = false;
            grid.grid[OccupiedGridPos.x, OccupiedGridPos.y].UpdateWalkable();
        }
        return true;
    }


    private bool DestroyGridObject(NodeGridPosition GridPos)
    {
        if (grid.CheckIfOutOfBounds(GridPos) || !BullDozerMode || !BuildMode)
            return false;

        GridObject GridObj = ObjectGrid[GridPos.x, GridPos.y];
        
        if (GridObj == null)
            return false;


        List<NodeGridPosition> NodesOccupied = GridObj.GetOccupiedNodes();

        if (GridObj.ID == GridObjectID.wall)
            Walls.Remove(GridObj.GetComponent<Wall>());


        Destroy(GridObj.gameObject);


        foreach (NodeGridPosition OccupiedGridPos in NodesOccupied)
        {
            if (GridObj.ID == GridObjectID.Floor)//Floors get stored in Floors and not ObjectGrid
                FloorGrid[OccupiedGridPos.x, OccupiedGridPos.y] = null;
            else //Everything Else gets stored in ObjectGrid
            {
                ObjectGrid[OccupiedGridPos.x, OccupiedGridPos.y] = null;
                grid.grid[OccupiedGridPos.x, OccupiedGridPos.y].Occupied = false;
                //no need to check this if the GridObject is a floor. Floors Don't Block People, You should try them. 
                grid.grid[OccupiedGridPos.x, OccupiedGridPos.y].UpdateWalkable();
            }
        }

        return true;
    }


    #endregion


    public List<NodeGridPosition> GetOccupiedNodes(NodeGridPosition GridPos)
    {
        List<NodeGridPosition> OccupiedNodes = new List<NodeGridPosition>();

        if (ObjectToBuildSize.x == 1 && ObjectToBuildSize.x == 1)
        {
            OccupiedNodes.Add(GridPos);
            return OccupiedNodes;
        }

        for (int _x = 0; _x < ObjectToBuildSize.x; _x++)
        {
            for (int _y = 0; _y < ObjectToBuildSize.y; _y++)
            {
                //I used x for where the occupied node actually is and _x for storing the loop
                int x = 0;
                int y = 0;
                switch (ObjectToBuildRotation)//rot is an int used this way 0 = 0°, 1 = 90°, 2 = 180°, 3 = 270°
                {
                    case 0:
                        x = _x;
                        y = _y;
                        break;
                    case 1:
                        x = _y;
                        y = -_x;
                        break;
                    case 2:
                        x = -_x;
                        y = -_y;
                        break;
                    case 3:
                        x = -_y;
                        y = _x;
                        break;
                    default:
                        x = _x;
                        y = _y;
                        break;
                }

                x += GridPos.x;
                y += GridPos.y;

                OccupiedNodes.Add(new NodeGridPosition(x, y));

            }
        }

        return OccupiedNodes;
    }

    public bool CheckIfOccupied(List<NodeGridPosition> NodesGridPositionTocheck)
    {
        foreach (NodeGridPosition GridPos in NodesGridPositionTocheck)
        {
            if (CheckIfOccupied(GridPos))
                return true;
        }

        return false;
    }

    public bool CheckIfOccupied(NodeGridPosition NodeGridPositionTocheck)
    {
            if (grid.CheckIfOutOfBounds(NodeGridPositionTocheck))
                return true;

            else if (ObjectGrid[NodeGridPositionTocheck.x, NodeGridPositionTocheck.y] != null)
                return true;

            else if (grid.grid[NodeGridPositionTocheck.x, NodeGridPositionTocheck.y].Occupied)
                return true;

            else
                return false;
    }


    public void SwitchBuildMode()
    {
        SetBuildMode(!BuildMode);
    }

    public void SetBuildMode(bool value)
    {
        BuildMode = value;
    }

    public void SwitchBullDozerMode()
    {
        SetBullDozerMode(!BullDozerMode);
    }

    public void SetBullDozerMode(bool value)
    {
        BullDozerMode = value;
    }

}
