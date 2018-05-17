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
        g1.AddRule(new Rules.CreateOperableBox("A", new Vector3(30, 60, 20)), false);
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
        g1.AddRule(new Rules.SizeBuilding3D("A", "A", so.meshable.bbox.size), false);
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
            new Vector3(20,0,-5),
            new Vector3(30,0,0),
            new Vector3(30,0,20),
            new Vector3(0,0,20)
        };

        SOPoint sop = SOPoint.CreatePoint();
        SGBuilding building = new SGBuilding();

        ShapeObject so2 = ShapeObject.CreateExtrusion(pts, 40);
        so2.meshable.bbox = BoundingBox.CreateFromPoints(so2.meshable.vertices, new Vector3(1, 0, 0));
        building.SetGPlanning(G2(so2));

        //building.SetGPlanning(G1(sop,30,20));
        //building.SetMassing(MassingGrammars.GA());
        building.SetMassing(MassingGrammars.GB());
        //building.SetProgram(ProgramGrammars.APT1());
        //building.SetFacade(FacadeGrammars.CW01());

        //print(building.gMassing.upStreams[0].name);
        building.Execute();
        building.GraphicsMode();
        building.UpdateParams();
        //building.Execute();

        SceneManager.SelectedGrammar = building.gPlaning;
        SceneManager.SelectedBuilding = building;
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
