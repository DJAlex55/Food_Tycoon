using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }



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
        if (!Input.anyKeyDown)
            return;
        
        if (Input.GetKeyDown(KeyCode.H))
        {
            BuildManager.Instance.SwitchAllUpperWalls();
        }
        if (Input.GetKeyDown(KeyCode.Return))
        {
            BuildManager.Instance.SwitchBuildMode();
        }
        if (Input.GetKeyDown(KeyCode.B) && BuildManager.Instance.BuildMode)
        {
            BuildManager.Instance.SwitchBullDozerMode();
        }

    }



}
