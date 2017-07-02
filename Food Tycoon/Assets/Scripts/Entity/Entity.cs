using UnityEngine;
using System.Collections;

public class Entity : MonoBehaviour
{
    private Vector3 target;
    public float Speed = 20;

    Vector3[] path = new Vector3[0];
    int targetIndex;

    float InteractRange = 0.5f;


    private void Awake()
    {
        path = new Vector3[0];
    }

    public void SetTarget(Vector3 _Target)
    {
        target = _Target;
        PathRequestManager.RequestPath(transform.position, target, OnPathFound);
    }


    public void OnPathFound(Vector3[] NewPath, bool PathSuccessful)
    {
        if (!PathSuccessful)
            return;

        targetIndex = 0;
        path = NewPath;

        StopCoroutine(FollowPath());

        StartCoroutine(FollowPath());
    }


    private IEnumerator FollowPath()
    {
        if(path.Length == 0)
            yield break;

        targetIndex = 0;
        Vector3 CurrentWaypoint = path[targetIndex];
        

        while (path.Length > 0)
        {
            if(Vector3.Distance(transform.position,CurrentWaypoint) <= .1f)
            {
                targetIndex++;
                if(targetIndex >= path.Length)
                {
                    path = new Vector3[0];
                    yield break;
                }
                CurrentWaypoint = path[targetIndex];
            }

            transform.position = Vector3.MoveTowards(transform.position, CurrentWaypoint, Speed * Time.deltaTime);
            transform.LookAt(CurrentWaypoint, Vector3.up);

            transform.eulerAngles = new Vector3(0f, transform.eulerAngles.y , 0f );
            yield return null;
        }
        yield break;
    }


    private void OnDrawGizmos()
    {
        if(path != null && path.Length > 0)
        {
            for (int i = targetIndex; i < path.Length; i++)
            {
                Gizmos.color = Color.black;
                Vector3 TargetPoint = path[i];
                TargetPoint.y = 1f;

                Gizmos.DrawCube(TargetPoint, Vector3.one * 0.5f);

                if(i == targetIndex)
                    Gizmos.DrawLine(new Vector3(transform.position.x,1f,transform.position.z), TargetPoint);
                else
                    Gizmos.DrawLine(new Vector3(path[i-1].x,1f, path[i - 1].z), TargetPoint);
            }
            

        }
    }

}
