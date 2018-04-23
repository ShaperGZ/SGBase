using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawingGrid : MonoBehaviour {

    public float spacing;
    public int count;
    public Plane workplane = new Plane(Vector3.up, Vector3.zero);
    List<Vector3> pts = new List<Vector3>();
    Vector3[] ptsX = new Vector3[2];
    Vector3[] ptsZ = new Vector3[2];
    Color color = new Color(0, 0, 0, 0.2f);
    Vector3? pointOnPlane;

	// Use this for initialization
	void Start () {
        //make grid
        
        float max = count * spacing;
        for (int i = 0; i <= count; i++)
        {
            //vertical:
            float xposL = i * spacing;
            float xposR = -xposL;
            float zposU = max;
            float zposD = -max;
            pts.Add(new Vector3(xposL,0,zposD));
            pts.Add(new Vector3(xposL,0,zposU));
            pts.Add(new Vector3(xposR, 0, zposD));
            pts.Add(new Vector3(xposR, 0, zposU));
            //horizontal:
            xposL = -max;
            xposR = max;
            zposU = i * spacing;
            zposD = -i * spacing;
            pts.Add(new Vector3(xposL, 0, zposU));
            pts.Add(new Vector3(xposR, 0, zposU));
            pts.Add(new Vector3(xposL, 0, zposD));
            pts.Add(new Vector3(xposR, 0, zposD));
        }
        ptsX[0] = Vector3.zero;
        ptsX[1] = new Vector3(max, 0, 0);
        ptsZ[0] = Vector3.zero;
        ptsZ[1] = new Vector3(0, 0, max);
	}
	
	// Update is called once per frame
	void Update () {
        ////Vector3 vp = Camera.main.WorldToScreenPoint(Input.mousePosition);
        ////Vector3 sp = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        //Ray r=Camera.main.ViewportPointToRay(Input.mousePosition);
        //float d;
        //bool flag=workplane.Raycast(r, out d);
        //if (true)
        //    pointOnPlane = r.GetPoint(d);
        //else
        //    pointOnPlane = null;
	}

    private void OnRenderObject()
    {
        SGGeometry.GLRender.Lines(pts.ToArray(), color);
        SGGeometry.GLRender.Lines(ptsX, Color.red);
        SGGeometry.GLRender.Lines(ptsZ, Color.blue);

        if (pointOnPlane.HasValue)
        {
            Vector3 pop = pointOnPlane.Value;
            Vector3[] hps = new Vector3[4];
            hps[0] = new Vector3(0, 0, pop.z);
            hps[1] = new Vector3(pop.x, 0, pop.z);
            hps[2] = new Vector3(pop.x, 0, pop.z);
            hps[3] = new Vector3(pop.x, 0, 0);
            SGGeometry.GLRender.Lines(hps, color);
        }
        
    }
}
