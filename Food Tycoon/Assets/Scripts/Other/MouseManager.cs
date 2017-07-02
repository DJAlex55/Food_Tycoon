using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseManager : MonoBehaviour
{
    public static MouseManager Instance { get; private set; }


    private bool Recording;
    private NodeGridPosition StartNodeGridPos;
    private NodeGridPosition EndNodeGridPos;

    private List<NodeGridPosition> SelectedNodesGridPos;

    private bool BuildMode { get { return BuildManager.Instance.BuildMode; } }
    private bool BullDozerMode { get { return BuildManager.Instance.BullDozerMode; } }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
        {
            Debug.LogError(gameObject + " was a second Instance! and was destroyed for it!");
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        //Mouse 1 (Left)
        if (Input.GetKeyDown(KeyCode.Mouse0))
            OnMouse1Down();
        else if (Input.GetKey(KeyCode.Mouse0))
            OnMouse1();
        else if (Input.GetKeyUp(KeyCode.Mouse0))
            OnMouse1Up();


        //Mouse 2 (Right)
        if (Input.GetKeyDown(KeyCode.Mouse1))
            OnMouse2Down();


        if (BuildMode)
        {
            if (Recording)
                GridOverlayManager.Instance.ShowSelectedNodesInGrid(SelectedNodesGridPos);
            else if (Grid.DoRayCast() != null)
                GridOverlayManager.Instance.ShowSelectedNodesInGrid(Grid.DoRayCast().GridPos);

        }
    }
    
    private void OnMouse1Down()
    {
        if (BuildMode)
        {
            NodeGridPosition tmp_GridPos = Grid.NodeGridPositionFromRayCast();
            if (tmp_GridPos != NodeGridPosition.Null)
            {
                StartNodeGridPos = tmp_GridPos;
                Recording = true;

                SelectedNodesGridPos = new List<NodeGridPosition> { StartNodeGridPos };
            }
        }
        else
        {
            //do a raycast with the Controllable Entity layer, if then we hit somebody make them currentEntity in (EnityManager)s
            //Utility.DoRayCast()
        }

    }

    private void OnMouse1()
    {
        if(Recording && BuildMode)
        {
            NodeGridPosition tmp_GridPos = Grid.NodeGridPositionFromRayCast();
            if (tmp_GridPos != NodeGridPosition.Null)
            {
                EndNodeGridPos = tmp_GridPos;
                SelectedNodesGridPos = Grid.Instance.GetNodeLineFromNodes(StartNodeGridPos, EndNodeGridPos);
            }
        }
    }


    private void OnMouse1Up()
    {
        if (Recording && StartNodeGridPos != NodeGridPosition.Null && BuildMode)
        {
            Utility<NodeGridPosition>.CleanUpList(SelectedNodesGridPos);
            EndNodeGridPos = Grid.NodeGridPositionFromRayCast();
            //If we don't find a EndNodeGridPosition we set the EndNodeGridPosition to the last GridPosition in the SelectedNodes List
            if (EndNodeGridPos == NodeGridPosition.Null && SelectedNodesGridPos.Count > 0)
            {   
                EndNodeGridPos = SelectedNodesGridPos[SelectedNodesGridPos.Count - 1];
            }

            if (StartNodeGridPos == EndNodeGridPos)
            {
                if (!BullDozerMode)
                {
                    BuildManager.Instance.BuildSingleObject(StartNodeGridPos);
                }
                else
                {
                    BuildManager.Instance.DestroySingleGridObject(StartNodeGridPos);
                }
            }
            else if (StartNodeGridPos != EndNodeGridPos)
            {
                if (!BullDozerMode)
                {
                    BuildManager.Instance.BuildObjectsLine(StartNodeGridPos, EndNodeGridPos);
                }
                else
                {
                    BuildManager.Instance.DestroyLineOfGridObject(StartNodeGridPos, EndNodeGridPos);
                }

            }

            Recording = false;
            SelectedNodesGridPos = null;
        }
    }


    private void OnMouse2Down()
    {
        if (BuildMode)
        {
            Recording = false;
            
            SelectedNodesGridPos = null;
        }
        else
        {
            //We should do this only if before we fired a ray and it didn't hit any Interactable GridObject
            RaycastHit Hit;
            Utility.DoRayCastOnGrid(out Hit);

            if (Hit.collider != null)
                EntityManager.Instance.SetTarget(Hit.point);
        }
    }

    private void OnMouse2()
    {

    }

    private void OnMouse2Up()
    {

    }

}
