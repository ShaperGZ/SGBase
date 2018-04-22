using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class Tool {

    public Plane workPlane;
    Vector3 mousePosition;
	// Use this for initialization
	public void Start () {
        workPlane = new Plane(Vector3.up, Vector3.zero);
    }

    // Update is called once per frame
    public void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            MouseDown(0);
        }
        else if (Input.GetMouseButtonDown(1))
        {
            MouseDown(1);
        }
        else if (Input.GetMouseButton(0))
        {
            MouseDrag(0);
        }
        else if (Input.GetMouseButton(1))
        {
            MouseDrag(1);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            MouseUp(0);
        }
        else if (Input.GetMouseButtonUp(1))
        {
            MouseUp(1);
        }
        else
        {
            MouseMove();
        }
    }
    protected virtual void MouseDrag(int button)
    {

    }
    protected virtual void MouseDown(int button)
    {
        Debug.Log("mouse down");
    }
    protected virtual void MouseUp(int button)
    {

    }
    protected virtual void MouseMove()
    {
        mousePosition = GetMouseInWorld();
        //Debug.LogFormat("MouseMove mousePOsition={0}", mousePosition);
    }

    public virtual void OnRenderObject()
    {
        Vector3[] uv = new Vector3[4];
        float max = 10000;
        uv[0] = new Vector3(-max,0, mousePosition.y);
        uv[1] = new Vector3(max, 0, mousePosition.y);
        uv[2] = new Vector3(mousePosition.x, 0, -max);
        uv[3] = new Vector3(mousePosition.x, 0,max);
        SGGeometry.GLRender.Lines(uv, Color.white);

    }
    protected Vector3 worldToScreenPts(Vector3 wp)
    {
        Vector3 sp = Camera.main.ViewportToWorldPoint(wp);
        //Vector3 sp = Camera.main.WorldToScreenPoint(wp);
        return sp;
    }
    protected Vector3 GetMouseInWorld()
    {
        Vector3 sp = Input.mousePosition;
        //Debug.LogFormat("mousepos:{0}", sp);
        Vector3 wp = Camera.main.ScreenToWorldPoint(sp);
        //Debug.LogFormat("mouseWorldpos:{0}", wp);
        return wp;
    }
}
