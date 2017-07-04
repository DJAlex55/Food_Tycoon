using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chair : GridObject
{
    [SerializeField] private Transform SitSpot;

    public bool IsOccupied { get; private set; }


    private void Awake()
    {
        IsOccupied = false;
    }

}
