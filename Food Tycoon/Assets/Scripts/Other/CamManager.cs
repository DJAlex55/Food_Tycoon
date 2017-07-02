using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamManager : MonoBehaviour {

    [SerializeField] [Range(-90f, 90f)] float MinXAngle;
    [SerializeField] [Range( -90f, 90f)] float MaxXAngle;

    [Space(15)]
    [SerializeField] [Range(0.01f, 10f)] float Sensibility;
    [SerializeField] [Range(0.01f, 10f)] float XSensibility;
    [SerializeField] [Range(0.01f, 10f)] float YSensibility;
    [SerializeField] [Range(0.01f, 10f)] float ZoomSensitivity;
    [Space(5)]
    [SerializeField] [Range(0.01f, 20f)] float Speed;
    [Space(5)]
    [SerializeField] float Threshold;

    [Space(15)]
    [SerializeField] Transform CamRig;
    [SerializeField] Transform Cam;

    [Space(15)]
    [SerializeField] [Range(1f, 10f)] float MinOffSet;
    [SerializeField] [Range(1f, 20f)] float MaxOffSet;

    [Space(15)]
    [SerializeField] bool InvertXRot;
    [SerializeField] bool InvertYRot;
    [SerializeField] bool InvertZoom;


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftAlt))
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else if (Input.GetKeyUp(KeyCode.LeftAlt))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }        
        if (Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.Mouse2))
        {
            Rotate();
        }        



        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
        {
            Move();
        }

        if (Input.GetAxis("Mouse ScrollWheel") != 0f)
        {
            Zoom();
        }

    }




    private void Rotate()
    {

        float Vertical = 0f;
        float Horizontal = 0f;
        
        if (Input.GetAxis("Mouse Y") > Threshold|| Input.GetAxis("Mouse Y") < -Threshold)
            Vertical = Input.GetAxis("Mouse Y") * Sensibility * XSensibility ;
        
        if (Input.GetAxis("Mouse X") > Threshold || Input.GetAxis("Mouse X") < -Threshold)
            Horizontal = Input.GetAxis("Mouse X") * Sensibility * YSensibility;


        if (InvertXRot)
            Vertical = -Vertical;

        if (InvertYRot)
            Horizontal = -Horizontal;

        Horizontal += CamRig.eulerAngles.y;
        Vertical += CamRig.eulerAngles.x;

        if (Vertical > MaxXAngle)
            Vertical = MaxXAngle;
        if (Vertical < MinXAngle)
            Vertical = MinXAngle;


        CamRig.rotation = Quaternion.Euler(Vertical, Horizontal, 0f);
    }


    private void Move()
    {
        Vector3 Dir = new Vector3() ;


        if (Input.GetKey(KeyCode.W))
        {
            Dir += new Vector3(CamRig.forward.x, 0f, CamRig.forward.z);
            
        }
        else if (Input.GetKey(KeyCode.S))
        {
            Dir -= new Vector3(CamRig.forward.x, 0f, CamRig.forward.z);
        }
        Dir.Normalize();

        if (Input.GetKey(KeyCode.D))
        {
            Dir += CamRig.right;
        }
        else if (Input.GetKey(KeyCode.A))
        {
            Dir -= CamRig.right;
        }

        Dir.Normalize();


        CamRig.position += Dir * Time.deltaTime * Speed;


    }


    private void Zoom()
    {
        float delta = -Input.GetAxis("Mouse ScrollWheel") * ZoomSensitivity;

        if (InvertZoom)
            delta = -delta;


        Vector3 changeOfPosition = Cam.transform.localPosition * delta;

        Vector3 newPosition = Cam.transform.localPosition + changeOfPosition;

        newPosition = newPosition.normalized * ClampCameraDistance(newPosition);

        Cam.transform.localPosition = newPosition;
    }


    private float ClampCameraDistance(Vector3 position)
    {
        return Mathf.Clamp(position.magnitude, MinOffSet, MaxOffSet);
    }


}
