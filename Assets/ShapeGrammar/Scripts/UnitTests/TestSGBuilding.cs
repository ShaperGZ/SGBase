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
    Grammar G2()
    {
        Grammar g2 = new Grammar();
        g2.name = "AptFormA";
        g2.category = "massingForm";
        g2.inputs.names.Add("APT");
        g2.AddRule(new Rules.Bisect("APT", new string[] { "APT", "C" }, 0.4f, 0), false);
        g2.AddRule(new Rules.Bisect("C", new string[] { "C", "APT" }, 0.25f, 2), false);
        g2.AddRule(new Rules.Scale3D("C", "APT", new Vector3(1.3f, 0.7f, 1.6f), null, Alignment.NE), false);
        return g2;
    }

	// Use this for initialization
	void Start () {
        SOPoint sop = SOPoint.CreatePoint();
        SGBuilding building = new SGBuilding();

        building.SetGPlanning(G1(sop,30,20));
        building.SetMassing(MassingGrammars.GA());
        building.SetProgram(ProgramGrammars.APT1());
        building.SetFacade(FacadeGrammars.CW01());

        //print(building.gMassing.upStreams[0].name);
        

        building.Execute();

        SceneManager.SelectedGrammar = building.gPlaning;
        SceneManager.SelectedBuilding = building;
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
