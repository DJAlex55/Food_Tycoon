using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Wall : GridObject
{
    [SerializeField] private MeshRenderer UpperWall;

    public void ShowUpWall(bool value)
    {
        UpperWall.enabled = value;
    }


    private void Awake()
    {
        ID = GridObjectID.wall;
    }
}
