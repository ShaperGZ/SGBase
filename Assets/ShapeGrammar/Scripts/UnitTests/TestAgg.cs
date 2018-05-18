using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SGCore;
using SGGeometry;

public class TestAgg : MonoBehaviour {

	// Use this for initialization
	void Start () {


        SOPoint sop = SOPoint.CreatePoint();

        SOPoint[] sops = new SOPoint[2];
        sops[0] = SOPoint.CreatePoint();
        sops[1] = SOPoint.CreatePoint(new Vector3(-30, 0, -40));

        float[] hs = new float[2];
        hs[0] = 60;
        hs[1] = 16;

        float[] ws = new float[2];
        ws[0] = 30;
        ws[1] = 45;

        Building[] buildings = new Building[2];

        for (int i = 0; i < sops.Length; i++)
        {
            buildings[i] = new Building();
            Grammar g1 = new Grammar();
            buildings[i].AddGrammar(g1);
            g1.name = "g1";
            g1.assignedObjects.Add(sops[i]);
            g1.AddRule(new Rules.CreateOperableBox("A", new Vector3(30, 60, 20)), false);
            g1.AddRule(new Rules.SizeBuilding3D("A", "A", new Vector3(ws[i], hs[i], 20)), false);
            g1.AddRule(new Rules.ApartmentLoadFilter("A", "SL", "DL", "CV"), false);
            g1.AddRule(new Rules.SingleLoaded("SL", "APT"), false);
            g1.AddRule(new Rules.DoubleLoaded("DL", "APT"), false);
            g1.AddRule(new Rules.CentralVoid("CV", "APT", "APT2"), false);

            Grammar g2 = new Grammar();
            g2.name = "AptFormA";
            g2.category = GraphNode.Category.Bd_Massing;
            g2.inputs.names.Add("APT");
            g2.AddRule(new Rules.Bisect("APT", new string[] { "APT", "C" }, 0.4f, 0), false);
            g2.AddRule(new Rules.Bisect("C", new string[] { "C", "APT" }, 0.25f, 2), false);
            g2.AddRule(new Rules.Scale3D("C", "APT", new Vector3(1.3f, 0.7f, 1.6f), null, Alignment.NE), false);
            g1.AddRule(g2,false);
            
            g1.AddRule(new Rules.CalBuildingParams(new string[] { "APT" }), false);
            g1.AddRule(new Rules.ExtractFace(new string[] { "APT" }, "TOP", "TOP"),false);
            g1.AddRule(new Rules.DivideToFTFH("APT", new string[] { "APTL", "APTLM" }, 4), false);
            g1.AddRule(new Rules.PivotMirror("APTLM", "APTL", 0), false);
            g1.AddRule(new Rules.CspFlrToUnitsAbstract(new string[] { "APTL" }), false);

            g1.AddRule(new Rules.DcpFace5("APTL", new string[] { "F", "DELETE", "S", "DELETE", "BT" }), false);
            g1.AddRule(new Rules.Extrude("BT", "BT", -0.1f), false);
            g1.AddRule(new Rules.AggCW01("F", "F", 3, 4), false);
            g1.AddRule(new Rules.AggBasicSimp("S", "S", 3, 4), false);
            
            g1.Execute();
        }
        buildings[0].Invalidate();
        buildings[1].Invalidate();

        //SceneManager.buildingParamEditor.SetBuilding(buildings[0]);
        //SceneManager.SelectedGrammar = g1;
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
