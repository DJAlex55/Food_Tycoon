using System;
using System.Collections.Generic;
using UnityEngine;

public class PathRequestManager : MonoBehaviour
{
    public static PathRequestManager Instance { get; private set; }

    private Queue<PathRequest> pathRequestQueue;
    private PathRequest currentPathRequest;

    private PathFinderManager pathFinding;
    private bool isProcessingPath;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
        {
            Debug.LogError(gameObject + " was a second Instance! and was destroyed for it!");
            Destroy(gameObject);
        }

        pathFinding = GetComponent<PathFinderManager>();
        pathRequestQueue = new Queue<PathRequest>();
    }

    public static void RequestPath(Vector3 PathStart, Vector3 PathEnd, Action<Vector3[], bool> CallBack )
    {
        PathRequest newRequest = new PathRequest(PathStart, PathEnd, CallBack);
        
        Instance.pathRequestQueue.Enqueue(newRequest);

        Instance.TryProcessNext();
    }




    void TryProcessNext()
    {
        if(!isProcessingPath && pathRequestQueue.Count > 0)
        {
            currentPathRequest = pathRequestQueue.Dequeue();
            isProcessingPath = true;

            pathFinding.StartFindPath(currentPathRequest.PathStart, currentPathRequest.PathEnd, currentPathRequest.CallBack);
        }
    }


    public void FinisedProcessingPath(Vector3[] Path, bool Success)
    {
        currentPathRequest.CallBack(Path, Success);
        isProcessingPath = false;
        TryProcessNext();
    }



    struct PathRequest
    {
        public Vector3 PathStart;
        public Vector3 PathEnd;
        public Action<Vector3[], bool> CallBack;

        public PathRequest(Vector3 _PathStart, Vector3 _PathEnd, Action<Vector3[], bool> _CallBack)
        {
            PathStart = _PathStart;
            PathEnd = _PathEnd;
            CallBack = _CallBack;
        }
    }

}
