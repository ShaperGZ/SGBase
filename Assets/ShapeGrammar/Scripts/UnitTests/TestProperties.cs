using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SGCore;
using SGGeometry;

public class TestProperties : MonoBehaviour {
    public GameObject comp;
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
    void Start () {
        Vector3[] pts = initShape2();
        ShapeObject so = ShapeObject.CreateExtrusion(pts, 30);
        Grammar g1 = new Grammar();
        g1.inputs.shapes.Add(so);
        g1.AddRule(new Rules.Scale3D("A", "A", new Vector3(1, 1, 1)));

        Properties buildingA = new BuildingProperties();
        buildingA.AddGrammar(g1);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
