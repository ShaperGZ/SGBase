using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SGGeometry;
using System.Linq;

public class TestShapeObject : MonoBehaviour {

    // Use this for initialization
    Polyline pl;
	void Start () {
        Vector3[] pts = new Vector3[5];
        pts[0] = new Vector3(-80, 0, 46);
        pts[1] = new Vector3(-80, 0, -11);
        pts[2] = new Vector3(-10, 0, -54);
        pts[3] = new Vector3(103, 0, 25);
        pts[4] = new Vector3(96, 0, 70);
        pl = new Polyline(pts);
        Vector3? ld = PointsBase.LongestDirection(pts);

        //passed
        //ShapeObject so = ShapeObject.CreatePolygon(pts);
        ShapeObject so = ShapeObject.CreateExtrusion(pts, 40);

    }
	
	// Update is called once per frame
	void Update () {
		
	}
    private void OnRenderObject()
    {
    }
}
