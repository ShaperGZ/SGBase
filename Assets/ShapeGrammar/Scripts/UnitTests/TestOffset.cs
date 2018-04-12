using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SGCore;
using SGGeometry;

public class TestOffset : MonoBehaviour {

    Vector3[] pts;
    Vector3[] opts;

    public static Vector3[] initShape1()
    {
        Vector3[] pts = new Vector3[5];
        pts[0] = new Vector3(-80, 0, 46);
        pts[1] = new Vector3(-80, 0, -11);
        pts[2] = new Vector3(-10, 0, -54);
        pts[3] = new Vector3(103, 0, 25);
        pts[4] = new Vector3(96, 0, 70);
        return pts;
    }
    // Use this for initialization
    void Start () {
        pts = initShape1();
        Polygon pg = new Polygon(pts);

        ShapeObject init = ShapeObject.CreateMeshable(pg);
        init.name = "init";

        Meshable[] pgs = pg.Offset(10,true);
        foreach(Meshable g in pgs)
        {
            ShapeObject.CreateMeshable(g);
        }
	}
	
	// Update is called once per frame
	void Update () {
        
	}
    private void OnRenderObject()
    {
        //SGGeometry.GLRender.Polyline(pts, true, null, Color.black);
        //SGGeometry.GLRender.Polyline(opts, true, null, Color.black);
    }
}
