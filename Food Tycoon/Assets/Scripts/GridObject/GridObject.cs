using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridObject : MonoBehaviour
{
    public GridObjectID ID;

    /// <summary>
    /// Bottom Left Node occupied by this object
    /// </summary>
    [HideInInspector] public NodeGridPosition GridPos = NodeGridPosition.Null;

    private int _Rot;
    /// <summary>
    /// every 1 equals a 90 clockwise degree rotation
    /// </summary>
    [HideInInspector] public int Rot
    {
        get { return _Rot; }
        set
        {
            if (value >= 4)
                value %= 4;
            else 
                while (value < 0)
                    value += 4;

            _Rot = value;
        }
    }


    /// <summary>
    /// Size in GridUnits of this object
    /// </summary>
    public NodeGridPosition Size { get { return GetData().Size; } }

    /// <summary>
    /// returns all nodes occupied by this object
    /// </summary> 
    public List<NodeGridPosition> GetOccupiedNodes()
    {
        List<NodeGridPosition> OccupiedNodes = new List<NodeGridPosition>();

        if (Size.x == 1 && Size.x == 1)
        {
            OccupiedNodes.Add(GridPos);
            return OccupiedNodes;
        }
        

        for (int x = 0; x < Size.x; x++)
        {
            for (int y = 0; y < Size.y; y++)
            {
                //I used _x for where the occupied node actually is and x for storing the loop
                int _x = 0;
                int _y = 0;
                switch (Rot)//rot is an int used this way 0 = 0°, 1 = 90°, 2 = 180°, 3 = 270°
                {
                    case 0:
                        _x = x;
                        _y = y;
                        break;
                    case 1:
                        _x = y;
                        _y = -x;
                        break;
                    case 2:
                        _x = -x;
                        _y = -y;
                        break;
                    case 3:
                        _x = -y;
                        _y = x;
                        break;
                    default:
                        _x = x;
                        _y = y;
                        break;
                }

                OccupiedNodes.Add(new NodeGridPosition(_x, _y));

            }
        }
        return OccupiedNodes;
    }

   

    public GridObjectData GetData()
    {
        return IDManager.Instance.GetData(ID);
    }



    /*private void OnDestroy()
    {
        foreach (NodeGridPosition OccupiedGridPos in GetOccupiedNodes())
        {
            Grid.Instance.grid[OccupiedGridPos.x, OccupiedGridPos.y].UpdateWalkable();
        }
    }
    */
}


