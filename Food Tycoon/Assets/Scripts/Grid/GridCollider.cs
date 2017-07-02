using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class GridCollider : MonoBehaviour
{
    [HideInInspector] public Grid grid;

    [HideInInspector] public BoxCollider gridCollider;
    public LayerMask GridColliderMask;

    private void Awake()
    {
        gridCollider = GetComponent<BoxCollider>();
        gridCollider.isTrigger = true;        
    }

    private void Start()
    {
        if ((gameObject.layer | GridColliderMask )<= 0)
        {
            Debug.LogWarning("layer not set Correcty");
            gameObject.layer = 1 << GridColliderMask;
        }
    }

    public void Set(Vector3 Center, Vector3 Size)
    {        
        gridCollider.size = Size;

        gridCollider.center = new Vector3(Center.x, -gridCollider.size.y / 2, Center.z);
    }
            
}
