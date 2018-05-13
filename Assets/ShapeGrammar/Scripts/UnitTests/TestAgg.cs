using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SGCore;
using SGGeometry;

public class TestAgg : MonoBehaviour {

	// Use this for initialization
	void Start () {

        //Vector3[] pts = new Vector3[]
        //{
        //    new Vector3(0,0,0),
        //    new Vector3(1,0,0),
        //    new Vector3(1,0,1),
        //    new Vector3(0,0,1),
        //};

        //Extrusion ext = new Extrusion(pts, 1);
        //ShapeObject.CreatePolygon(ext.top.vertices);

        SOPoint sop = SOPoint.CreatePoint();
        Grammar g1 = new Grammar();
        g1.assignedObjects.Add(sop);
        g1.AddRule(new Rules.CreateOperableBox("A", new Vector3(30, 60, 20)));
        g1.AddRule(new Rules.SizeBuilding3D("A", "A", new Vector3(30, 60, 20)));
        g1.AddRule(new Rules.DivideToFTFH("A", new string[]{"A","B"}, 4));
        g1.AddRule(new Rules.DcpFace2("A", new string[] { "S", "T" }));
        g1.AddRule(new Rules.AggBasicSimp("S","S", 3, 4));
        //g1.AddRule(new Rules.AggBasic("T","S", 3, 4));

        SceneManager.SelectedGrammar = g1;
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
