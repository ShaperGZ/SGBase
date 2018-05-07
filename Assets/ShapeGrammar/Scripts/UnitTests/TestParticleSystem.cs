using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SGCore;
using SGGeometry;

public class TestParticleSystem : MonoBehaviour {
    SGParticleSystem particleSystem;
    // Use this for initialization
    public Vector3[] boundary;
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
    ShapeObject createBuilding()
    {
        ShapeObject so = SOPoint.CreatePoint();
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


        Grammar g2 = new Grammar();
        g2.name = "AptFormA";
        g2.AddRule(new Rules.Bisect("APT", new string[] { "B", "C" }, 0.4f, 0), false);
        g2.AddRule(new Rules.Bisect("C", new string[] { "C", "B" }, 0.25f, 2), false);
        g2.AddRule(new Rules.PivotMirror("C", "C", 2), false);
        g2.AddRule(new Rules.PivotMirror("C", "C", 0), false);
        g2.AddRule(new Rules.Scale3D("C", "C", new Vector3(1.3f, 0.7f, 1.6f), null, Alignment.N), false);
        g2.inputs.names.Add("APT");

        g.AddRule(g2);
        //g.AddRule(new Rules.DcpA("A", 9, 3));
        return so;
    }
    Site site;
    void Start () {
        boundary = initShape1();
        BoundingBox bbox = BoundingBox.CreateFromPoints(boundary);
        Debug.Log(bbox.Format());

        site = new Site(boundary);


        particleSystem = new SGPlaningParticleSystem(boundary);
        for (int i = 0; i < 8; i++)
        {
            //ShapeObject so = SOPoint.CreatePoint();
            ShapeObject so = createBuilding();
            BuildingProperties building = (BuildingProperties)so.grammar.properties;
            building.SetRefPos(so);
            site.buildings.Add(building);
            particleSystem.AddRand(so);
        }

	}
	
	// Update is called once per frame
	void Update () {
        particleSystem.Update();
        site.Invalidate();
	}

    private void OnRenderObject()
    {
        SGGeometry.GLRender.Polyline(boundary,true,null,Color.black);
    }
}
