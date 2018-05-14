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
        g1.AddRule(new Rules.ApartmentLoadFilter("A", "SL", "DL", "CV"));
        g1.AddRule(new Rules.SingleLoaded("SL", "APT"));
        g1.AddRule(new Rules.DoubleLoaded("DL", "APT"));
        g1.AddRule(new Rules.CentralVoid("CV", "APT", "APT2"));

        Grammar g2 = new Grammar();
        g2.name = "AptFormA";
        g2.inputs.names.Add("APT");
        g2.AddRule(new Rules.Bisect("APT", new string[] { "APT", "C" }, 0.4f, 0), false);
        g2.AddRule(new Rules.Bisect("C", new string[] { "C", "APT" }, 0.25f, 2), false);
        g2.AddRule(new Rules.Scale3D("C", "APT", new Vector3(1.3f, 0.7f, 1.6f), null, Alignment.NE), false);
        g1.AddRule(g2);




        g1.AddRule(new Rules.DivideToFTFH("APT", new string[]{"APTL","APTLM"}, 4));
        g1.AddRule(new Rules.PivotMirror("APTLM", "APTL", 0));
        //g1.AddRule(new Rules.DcpFace5("APTL", new string[] { "F","B","S", "T","B"}));
        g1.AddRule(new Rules.DcpFace5("APTL", new string[] { "F","DELETE","S", "T", "BT" }));
        
        g1.AddRule(new Rules.AggCW01("F","F", 3, 4));
        g1.AddRule(new Rules.AggBasicSimp("S","S", 3, 4));

        SceneManager.SelectedGrammar = g1;
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
