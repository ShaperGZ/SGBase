using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SGCore;
using SGGeometry;

public class TestGrammar : MonoBehaviour {

	// Use this for initialization
	void Start () {
        Vector3[] pts = new Vector3[5];
        pts[0] = new Vector3(-80, 0, 46);
        pts[1] = new Vector3(-80, 0, -11);
        pts[2] = new Vector3(-10, 0, -54);
        pts[3] = new Vector3(103, 0, 25);
        pts[4] = new Vector3(96, 0, 70);
        Polyline pl = new Polyline(pts);
        Polygon pg = new Polygon(pts);
        Form f = pg.ExtrudeToForm(new Vector3(0, 30, 0));
        ShapeObject init = ShapeObject.CreateMeshable(f);

        Grammar g1 = new Grammar();
        g1.assignedObjects.Add(init);
        g1.AddRule(new Rules.Bisect("A",new string[] {"B","C"}, 0.4f, 0),false);
        g1.AddRule(new Rules.Bisect("B", new string[] { "B", "C" }, 0.4f, 2), false);
        g1.AddRule(new Rules.Scale("C", "C", 1.5f, 1), false);
        g1.Execute();

    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
