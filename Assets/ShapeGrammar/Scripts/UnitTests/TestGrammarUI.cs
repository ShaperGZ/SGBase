using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SGCore;
using SGGeometry;
using SGGUI;
using System.Linq;
using UnityEngine.UI;


public class TestGrammarUI : MonoBehaviour {
    Grammar g1;
    Text stageIOText;
    RuleParamEditor ruleParamEditor;
    // Use this for initialization

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
  
    void Passed_BisectMirror(ShapeObject so)
    {
        Grammar g1 = new Grammar();
        g1.name = "g1";
        g1.assignedObjects.Add(so);
        g1.AddRule(new Rules.BisectMirror("A", new string[] { "A", "A" }, 0.4f, 0));
        g1.AddRule(new Rules.Bisect("A", new string[] { "B", "C" }, 0.2f, 0));
    }
    
    void Passed_CreateBuilding(ShapeObject so)
    {
        Grammar g = new Grammar();
        BuildingProperties bp = new BuildingProperties();
        bp.AddGrammar(g);
        so.SetGrammar(g);

        float h = Random.Range(9, 30);
        g.AddRule(new Rules.CreateOperableBox("A", new Vector3(30, 60, 20)));
        g.AddRule(new Rules.SizeBuilding3D("A", "A", new Vector3(30, h, 20)));
        g.AddRule(new Rules.ApartmentLoadFilter("A", "SL", "DL","CV"));
        g.AddRule(new Rules.SingleLoaded("SL", "APT"));
        g.AddRule(new Rules.DoubleLoaded("DL", "APT"));
        g.AddRule(new Rules.CentralVoid("CV", "APT", "APT2"));
    }

    void Start () {
        Vector3[] pts1 = initShape1();
        
        Polygon pg1 = new Polygon(pts1);

        Extrusion f1 = pg1.Extrude(new Vector3(0, 40, 0));
        //ShapeObject so = ShapeObject.CreateMeshable(f1);
        //so.name = "A";

        ShapeObject so = SOPoint.CreatePoint();
        Building building = new Building();

        g1 = new Grammar();
        building.AddGrammar(g1);
        g1.name = "g1";
        so.SetGrammar(g1);

        //g1.AddRule(new Rules.CreateBox("A", new Vector3(0, 0, 0), new Vector3(10, 10, 10), new Vector3(0,0,0)));
        g1.AddRule(new Rules.CreateOperableBox("A", new Vector3(30, 60, 20)));
        g1.AddRule(new Rules.SizeBuilding3D("A", "A", new Vector3(40, 30, 20)));

        g1.AddRule(new Rules.ApartmentLoadFilter("A", "SL", "DL", "CV"));
        g1.AddRule(new Rules.SingleLoaded("SL", "APT"));
        g1.AddRule(new Rules.DoubleLoaded("DL", "APT"));
        g1.AddRule(new Rules.CentralVoid("CV", "APT", "APT2"));


        Grammar g2 = new Grammar();
        g2.name = "AptFormA";
        g2.AddRule(new Rules.Bisect("APT", new string[] { "APT", "C" }, 0.4f, 0), false);
        g2.AddRule(new Rules.Bisect("C", new string[] { "C", "APT" }, 0.25f, 2), false);
        g2.AddRule(new Rules.Scale3D("C", "APT", new Vector3(1.3f, 0.7f, 1.6f), null, Alignment.NE), false);
        

        g2.inputs.names.Add("APT");




        g1.AddRule(g2);
        //g1.AddRule(new Rules.DivideTo("APT", "APTL", 4, 1));
        //g1.AddRule(new Rules.DivideTo("APT2", "APTL", 4, 1));

        g1.AddRule(new Rules.DcpFace2("APT", new string[] { "APT", "APT" }));
        g1.AddRule(new Rules.DcpFace2("APT2", new string[] { "APT", "APT" }));


        //g1.AddRule(new Rules.DivideToFTFH("APT2", "APTL", 4));
        //g1.AddRule(new Rules.DivideToFTFH("APT", "APTL", 4));
        //g1.AddRule(new Rules.DcpFace2("APTL", new string[] { "Top", "Side" }));

        g1.Execute();
        SceneManager.SelectedGrammar = g1;
        

    }
    
    void Update () {
        //g1.Execute();
	}
}
