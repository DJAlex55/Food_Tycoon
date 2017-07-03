using UnityEngine;
using System.Collections;

public class Entity : MonoBehaviour
{
    private Vector3 target;
    public float Speed = 20;

    private Coroutine FollowPathCoroutine;

    Vector3[] path = new Vector3[0];
    int targetIndex;

    float InteractRange = 0.5f;
    
    [SerializeField] private float MaxDegreeSpeed = 90;

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

        if(FollowPathCoroutine != null)
            StopCoroutine(FollowPathCoroutine);

        FollowPathCoroutine = StartCoroutine(FollowPath());
    }


    private IEnumerator FollowPath()
    {
        if(path.Length == 0)
            yield break;

        targetIndex = 0;
        Vector3 CurrentWaypoint = path[targetIndex];
        

        while (path.Length > 0)
        {
            if(transform.position == CurrentWaypoint)
            {
                targetIndex++;
                if(targetIndex >= path.Length)
                {
                    path = new Vector3[0];
                    yield break;
                }
                CurrentWaypoint = path[targetIndex];
            }

            Quaternion TargetRotation = Quaternion.LookRotation(CurrentWaypoint - transform.position); 
            
            transform.rotation = Quaternion.RotateTowards(transform.rotation, TargetRotation, MaxDegreeSpeed * Time.deltaTime);

            transform.eulerAngles = new Vector3(0f, transform.eulerAngles.y , 0f );

            //I know this is better if put higher up BUT leave it here. I have my motivations
            transform.position = Vector3.MoveTowards(transform.position, CurrentWaypoint, Speed * Time.deltaTime);


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
