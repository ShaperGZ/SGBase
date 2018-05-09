using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OrbitTargetCamera : MonoBehaviour {
    public Vector3 target_pos;
    public float camera_mag;
    float zoom_factor = 0.1f;
    Camera _camera;


    public class InputStates
    {
        public static Vector3 mouseMoveDeltaPos = new Vector3();
        // this is the actuall real world vect of the mouse pointer 
        // unprojected to world space
        public static Vector3 mouseMoveVectWorld = new Vector3();
        public static Vector3 mouseDownDeltaPos = new Vector3();
        public static Vector3 mouseDownPos = new Vector3();
        public static Vector3 mouseLastPos = new Vector3();
    }

    private void Awake()
    {
        //this.runInEditMode = true;
    }

    // Use this for initialization
    public virtual void Start () {
        camera_mag = (target_pos - transform.position).magnitude;
        _camera = GetComponent<Camera>();
        lookAtTarget();
    }
    // Update is called once per frame
    public virtual void Update()
    {
       
        getInputStates();
        if (Input.GetMouseButton(1))
            cameraRotate();
        else if (Input.GetMouseButton(2))
            cameraPan();
        if (Input.mouseScrollDelta.y != 0)
            cameraZoom();
        lookAtTarget();
        
        DebugDrawings();
    }

    void DebugDrawings()
    {
        Debug.DrawLine(transform.position, target_pos);
        Debug.DrawLine(transform.position, transform.position + new Vector3(5, 0, 0));
        Debug.DrawLine(new Vector3(0,0,0), transform.position + new Vector3(5, 0, 0));
    }
    void cameraRotate()
    {
        //transform.Rotate(Vector3.up, InputStates.mouseMoveDeltaPos.x);
        Vector3 pos = transform.position;

        /////////////////////////
        //// rotate around y ////
        /////////////////////////
        Quaternion rotation = Quaternion.Euler(0, InputStates.mouseMoveDeltaPos.x, 0);
        pos = rotation * (pos - target_pos);
        transform.position = pos + target_pos;

        /////////////////////////
        //// rotate around x ////
        /////////////////////////
        pos = transform.position;
        Vector3 lookVect = target_pos - transform.position;
        lookVect.Normalize();

        Vector3 side = Vector3.Cross(lookVect, Vector3.up);
        rotation = Quaternion.AngleAxis(InputStates.mouseMoveDeltaPos.y, side);
        pos = rotation * (pos - target_pos);
        transform.position = pos + target_pos;
        //transform.position = rotation * transform.position;
        //Debug.DrawLine(target_pos, target_pos + side * 2);



    }
    void lookAtTarget()
    {
        Vector3 vect = transform.position - target_pos;
        vect.Normalize();
        vect *= camera_mag;
        transform.position = vect + target_pos;
        transform.LookAt(target_pos);
    }
    void cameraPan()
    {
        Vector3 look = target_pos - transform.position;
        Vector3 side = Vector3.Cross(look, Vector3.up);
        Vector3 up = Vector3.Cross(look, side);
        side.Normalize();
        up.Normalize();
        //float mag = look.magnitude;
        float panFactor = camera_mag*0.004f;
        //float panFactor = 0.02f;
        transform.position += side * InputStates.mouseMoveDeltaPos.x * panFactor;
        transform.position += up * InputStates.mouseMoveDeltaPos.y * panFactor;
        target_pos += side * InputStates.mouseMoveDeltaPos.x * panFactor;
        target_pos += up * InputStates.mouseMoveDeltaPos.y * panFactor;
        // TODO back project 

    }
    void cameraZoom()
    {
        if (_camera.orthographic)
        {
            _camera.orthographicSize += Input.mouseScrollDelta.y;
        }
        else
        {
            if (Input.mouseScrollDelta.y>0 && camera_mag < 1)
            {
                return;
            }
            else
            {
                camera_mag *= (1 - (zoom_factor*Input.mouseScrollDelta.y));
            }
            //DHUD.AddUpdateLine("zoom = " + zoomVect.magnitude.ToString() + "   Delta.y="+ Input.mouseScrollDelta.y.ToString());
        }

    }

    // Update is called once per frame
    void getInputStates()
    {
        // reset input states on mouse button down
        if (Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(2) || Input.mousePresent == false)
        {
            InputStates.mouseLastPos = Input.mousePosition;
            InputStates.mouseDownPos = Input.mousePosition;
            InputStates.mouseDownDeltaPos.x = 0;
            InputStates.mouseDownDeltaPos.y = 0;
        }

        //action on pressing the button
        if (Input.GetMouseButton(1) || Input.GetMouseButton(2))
        {
            InputStates.mouseDownDeltaPos = Input.mousePosition - InputStates.mouseDownPos;
            InputStates.mouseMoveDeltaPos = Input.mousePosition - InputStates.mouseLastPos;

            InputStates.mouseLastPos = Input.mousePosition;
        }
    }

 

}
