using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SGCore;
using SGGeometry;

public class TestPivot : MonoBehaviour {
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
    public static Vector3[] initShape2()
    {
        Vector3[] pts = new Vector3[4];
        pts[0] = new Vector3(-20, 0, 15);
        pts[1] = new Vector3(-20, 0, -15);
        pts[2] = new Vector3(20, 0, -15);
        pts[3] = new Vector3(20, 0, 15);
        return pts;
    }
    // Use this for initialization
    void Start ()
    { 
        Vector3[] pts = initShape1();
        Polygon pg = new Polygon(pts);
        Form f = pg.Extrude(new Vector3(0, 40, 0));

        ShapeObject rf = ShapeObject.CreateMeshable(f);
        rf.name = "rf";


        ShapeObject so = ShapeObject.CreateMeshable(f);
        so.PivotMirror(0);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
