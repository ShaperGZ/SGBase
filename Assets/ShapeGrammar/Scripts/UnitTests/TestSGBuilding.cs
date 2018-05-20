using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SGCore;
using SGGeometry;

public class TestSGBuilding : MonoBehaviour {

    Grammar G1(SOPoint sop, float w, float h)
    {
        Grammar g1 = new Grammar();
        g1.name = "g1";
        g1.assignedObjects.Add(sop);
        g1.AddRule(new Rules.CreateOperableBox("A", new Vector3(w, h, 20)), false);
        g1.AddRule(new Rules.SizeBuilding3D("A", "A", new Vector3(w,h, 20)), false);
        g1.AddRule(new Rules.ApartmentLoadFilter("A", "SL", "DL", "CV"), false);
        g1.AddRule(new Rules.SingleLoaded("SL", "APT"), false);
        g1.AddRule(new Rules.DoubleLoaded("DL", "APT"), false);
        g1.AddRule(new Rules.CentralVoid("CV", "APT", "APT2"), false);
        return g1;
    }

    Grammar G2(ShapeObject so)
    {
        so.name = "A";
        Grammar g1 = new Grammar();
        g1.name = "g2";
        g1.assignedObjects.Add(so);
        g1.AddRule(new Rules.SizeBuilding3D("A", "A", so.Size), false);
        g1.AddRule(new Rules.ApartmentLoadFilter("A", "SL", "DL", "CV"), false);
        g1.AddRule(new Rules.SingleLoaded("SL", "APT"), false);
        g1.AddRule(new Rules.DoubleLoaded("DL", "APT"), false);
        g1.AddRule(new Rules.CentralVoid("CV", "APT", "APT2"), false);
        return g1;
    }


    // Use this for initialization
    void Start () {


        Vector3[] pts = new Vector3[]
        {
            new Vector3(),
            new Vector3(35,0,-4),
            new Vector3(50,0,0),
            new Vector3(50,0,15),
            new Vector3(0,0,15)
        };

        SOPoint sop = SOPoint.CreatePoint();
        SGBuilding b1 = new SGBuilding();

        ShapeObject so2 = ShapeObject.CreateExtrusion(pts, 30);
        so2.meshable.bbox = BoundingBox.CreateFromPoints(so2.meshable.vertices, new Vector3(1, 0, 0));
        b1.SetGPlanning(G2(so2));
        b1.SetMassing(MassingGrammars.GB());
        b1.SetProgram(ProgramGrammars.APT1());
        b1.SetFacade(FacadeGrammars.CW01());

        SGBuilding b2 = new SGBuilding();
        SOPoint so3 = SOPoint.CreatePoint(new Vector3(20,0,40));
        b2.SetGPlanning(G1(so3,40,60));
        b2.SetMassing(MassingGrammars.GA());
        b2.SetProgram(ProgramGrammars.APT1());
        b2.SetFacade(FacadeGrammars.CW01());



        SceneManager.SelectedGrammar = b2.gPlaning;
        SceneManager.SelectedBuilding = b2;

        b1.MassingMode();
        b2.MassingMode();

        //b1.Execute();
        //b2.Execute();
        b2.UpdateParams();
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
