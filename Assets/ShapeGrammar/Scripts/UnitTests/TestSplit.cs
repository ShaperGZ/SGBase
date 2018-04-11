using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SGGeometry;
using SGCore;

public class TestSplit : MonoBehaviour {

	// Use this for initialization
	void Start () {
        Vector3[] pts = new Vector3[5];
        pts[0] = new Vector3(-80, 0, 46);
        pts[1] = new Vector3(-80, 0, -11);
        pts[2] = new Vector3(-10, 0, -54);
        pts[3] = new Vector3(103, 0, 25);
        pts[4] = new Vector3(96, 0, 70);
        Polygon pg = new Polygon(pts);
        Form f = pg.Extrude(new Vector3(0, 30, 0));

        Vector3 nml = pts[4] - pts[0];
        nml.Normalize();
        Plane pln = new Plane(nml, new Vector3());

        Meshable[] forms=f.SplitByPlane(pln);

        ShapeObject so0 = ShapeObject.CreateBasic();
        so0.SetMeshable(forms[0]);
        so0.transform.position -= new Vector3(-20, 0, -20);

        ShapeObject so1 = ShapeObject.CreateBasic();
        so1.SetMeshable(forms[1]);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
