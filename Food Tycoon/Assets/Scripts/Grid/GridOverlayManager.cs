using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridOverlayManager : MonoBehaviour
{
    public static GridOverlayManager Instance { get; private set; }
    
    private BuildManager buildManager;

    public GridOverlay BaseOverlay;
    public GridOverlay SelectedOverlay;

    [SerializeField] private Color SelectedOverlayNormalColor;
    [SerializeField] private Color SelectedOverlayBullDozerColor;

    private bool BuildMode { get { return buildManager.BuildMode; } }
    private bool BullDozerMode { get { return buildManager.BullDozerMode; } }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
        {
            Debug.LogError(gameObject + " was a second Instance! and was destroyed for it!");
            Destroy(gameObject);
        }

        if(BaseOverlay == null || SelectedOverlay == null)
        {
            Debug.LogError("Overlays Unassigned");
        }

    }

    private void Start()
    {
        buildManager = BuildManager.Instance;

        buildManager.OnBuildModeChanged += delegate { OnBuilModeChanged(); } ;
        buildManager.OnBullDozerModeChanged += delegate { OnBullDozerModeChanged(); };

        OnBuilModeChanged();
        OnBullDozerModeChanged();
    }


    public void OnBuilModeChanged()
    {
        BaseOverlay.gameObject.SetActive(BuildMode);
        SelectedOverlay.gameObject.SetActive(BuildMode);
    }

    public void OnBullDozerModeChanged()
    {
        if(BullDozerMode)
        {
            SelectedOverlay.mainColor = SelectedOverlayBullDozerColor;
        }
        else
        {
            SelectedOverlay.mainColor = SelectedOverlayNormalColor;
        }
    }


    #region ShowSelectedNodesInGrid

    /*public void ShowSelectedNodesInGrid(List<NodeGridPosition> nodes, GridOverlay gridOverlay)
    {
        if (nodes == null || nodes.Count <= 0 || !BuildMode)
            return;
        else if (nodes.Count == 1)
            ShowSelectedNodesInGrid(nodes[0], gridOverlay);


        gridOverlay.showMain = true;

        float Lenght = nodes.Count * grid.nodeRadius * 2f;
        float width = grid.nodeRadius * 2f;

        Vector3 size;
        Vector3 center;

        Node n1 = nodes[0];
        Node n2 = nodes[nodes.Count - 1];

        float differenceX = Mathf.Abs(n2.GridPos.x - n1.GridPos.x);
        float differenceY = Mathf.Abs(n2.GridPos.y - n1.GridPos.y);

        if (differenceX > differenceY)
        {
            size = new Vector3(Lenght, 0f, width);
        }
        else
        {
            size = new Vector3(width, 0f, Lenght);
        }

        center = new Vector3((n1.WorldPosition.x + n2.WorldPosition.x - size.x) / 2f, 0f, (n1.WorldPosition.z + n2.WorldPosition.z - size.z) / 2f);



        gridOverlay.gridSizeX = size.x;
        gridOverlay.gridSizeY = 0;
        gridOverlay.gridSizeZ = size.z;

        gridOverlay.startX = center.x;
        gridOverlay.startY = 0f;
        gridOverlay.startZ = center.z;
    }

    public void ShowSelectedNodesInGrid(List<NodeGridPosition> nodes)
    {
        if (nodes == null || nodes.Count <= 0 || !BuildMode)
            return;

        else if (nodes.Count == 1)
        {
            ShowSelectedNodesInGrid(nodes[0]);
            return;
        }


        SelectedOverlay.showMain = true;

        float Lenght = nodes.Count * grid.nodeRadius * 2f;
        float width = grid.nodeRadius * 2f;

        Vector3 size;
        Vector3 center;

        Node n1 = nodes[0];
        Node n2 = nodes[nodes.Count - 1];

        float differenceX = Mathf.Abs(n2.GridPos.x - n1.GridPos.x);
        float differenceY = Mathf.Abs(n2.GridPos.y - n1.GridPos.y);

        if (differenceX > differenceY)
        {
            size = new Vector3(Lenght, 0f, width);
        }
        else
        {
            size = new Vector3(width, 0f, Lenght);
        }

        center = new Vector3((n1.WorldPosition.x + n2.WorldPosition.x - size.x) / 2f, 0f, (n1.WorldPosition.z + n2.WorldPosition.z - size.z) / 2f);

        SetSelectedOverlayTransform(size, center, 0f);
    }
    */

    public void ShowSelectedNodesInGrid(List<NodeGridPosition> GridPos, GridOverlay gridOverlay)
    {
        if (GridPos == null || GridPos.Count <= 0 || !BuildMode)
            return;

        else if (GridPos.Count == 1)
        {
            ShowSelectedNodesInGrid(GridPos[0]);
            return;
        }


        gridOverlay.showMain = true;

        float Lenght = GridPos.Count * Grid.Instance.nodeRadius * 2f;
        float width = Grid.Instance.nodeRadius * 2f;

        Vector3 size;
        Vector3 center;

        NodeGridPosition n1GridPos = GridPos[0];
        NodeGridPosition n2GridPos = GridPos[GridPos.Count - 1];

        float differenceX = Mathf.Abs(n2GridPos.x - n1GridPos.x);
        float differenceY = Mathf.Abs(n2GridPos.y - n1GridPos.y);

        if (differenceX > differenceY)
        {
            size = new Vector3(Lenght, 0f, width);
        }
        else
        {
            size = new Vector3(width, 0f, Lenght);
        }

        Vector3 n1WorldPos = Grid.GetWorldPointFromNodeGridPosition(n1GridPos);
        Vector3 n2WorldPos = Grid.GetWorldPointFromNodeGridPosition(n2GridPos);

        center = new Vector3((n1WorldPos.x + n2WorldPos.x - size.x) / 2f, 0f, (n1WorldPos.z + n2WorldPos.z - size.z) / 2f);

        SetOverlayTransform(size, center, 0f, gridOverlay);
    }

    public void ShowSelectedNodesInGrid(List<NodeGridPosition> GridPos )
    {
        if (GridPos == null || GridPos.Count <= 0 || !BuildMode)
            return;

        
        else if (GridPos.Count == 1)
        {
            ShowSelectedNodesInGrid(GridPos[0]);
            return;
        }


        SelectedOverlay.showMain = true;

        float Lenght = GridPos.Count * Grid.Instance.nodeRadius * 2f;
        float width = Grid.Instance.nodeRadius * 2f;

        Vector3 size;
        Vector3 center;

        NodeGridPosition n1GridPos = GridPos[0];
        NodeGridPosition n2GridPos = GridPos[GridPos.Count - 1];

        float differenceX = Mathf.Abs(n2GridPos.x - n1GridPos.x);
        float differenceY = Mathf.Abs(n2GridPos.y - n1GridPos.y);

        if (differenceX > differenceY)
        {
            size = new Vector3(Lenght, 0f, width);
        }
        else
        {
            size = new Vector3(width, 0f, Lenght);
        }

        Vector3 n1WorldPos = Grid.GetWorldPointFromNodeGridPosition(n1GridPos);
        Vector3 n2WorldPos = Grid.GetWorldPointFromNodeGridPosition(n2GridPos);
        
        center = new Vector3((n1WorldPos.x + n2WorldPos.x - size.x) / 2f, 0f, (n1WorldPos.z + n2WorldPos.z - size.z) / 2f);

        SetSelectedOverlayTransform(size, center, 0.01f);
    }



    public void ShowSelectedNodesInGrid(NodeGridPosition GridPos, GridOverlay gridOverlay)
    {
        if (Grid.Instance.CheckIfOutOfBounds(GridPos) || !BuildMode)
            return;

        SelectedOverlay.showMain = true;
        float radius = Grid.Instance.nodeRadius;


        Vector3 center = Grid.GetWorldPointFromNodeGridPosition(GridPos) - Vector3.one * radius;

        SetOverlayTransform(Vector3.one * radius * 2, center, 0f, gridOverlay);


    }

    public void ShowSelectedNodesInGrid(NodeGridPosition GridPos)
    {
        if (Grid.Instance.CheckIfOutOfBounds(GridPos) || !BuildMode)
            return;

        SelectedOverlay.showMain = true;
        float radius = Grid.Instance.nodeRadius;        
        

        Vector3 center = Grid.GetWorldPointFromNodeGridPosition(GridPos) - Vector3.one * radius;

        SetSelectedOverlayTransform(Vector3.one * radius * 2, center, 0.01f);

    }

    #endregion

    #region SetSelectedOverlayTransform 

    private void SetSelectedOverlayTransform(Vector3 Size, Vector3 Center)
    {
        SelectedOverlay.gridSizeX = Size.x;
        SelectedOverlay.gridSizeY = 0f;
        SelectedOverlay.gridSizeZ = Size.z;

        SelectedOverlay.startX = Center.x;
        SelectedOverlay.startY = Center.y;
        SelectedOverlay.startZ = Center.z;
    }
    
    private void SetSelectedOverlayTransform(Vector3 Size, Vector3 Center, float ForcedY)
    {
        SelectedOverlay.gridSizeX = Size.x;
        SelectedOverlay.gridSizeY = 0f;
        SelectedOverlay.gridSizeZ = Size.z;

        SelectedOverlay.startX = Center.x;
        SelectedOverlay.startY = ForcedY;
        SelectedOverlay.startZ = Center.z;
    }


    private void SetOverlayTransform(Vector3 Size, Vector3 Center, GridOverlay gridOverlay)
    {
        gridOverlay.gridSizeX = Size.x;
        gridOverlay.gridSizeY = 0f;
        gridOverlay.gridSizeZ = Size.z;

        gridOverlay.startX = Center.x;
        gridOverlay.startY = Center.y;
        gridOverlay.startZ = Center.z;
    }

    private void SetOverlayTransform(Vector3 Size, Vector3 Center, float ForcedY, GridOverlay gridOverlay)
    {
        gridOverlay.gridSizeX = Size.x;
        gridOverlay.gridSizeY = 0f;
        gridOverlay.gridSizeZ = Size.z;

        gridOverlay.startX = Center.x;
        gridOverlay.startY = ForcedY;
        gridOverlay.startZ = Center.z;
    }

    #endregion

}
