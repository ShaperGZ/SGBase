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
        g.AddRule(new Rules.ApartmentLoadFilter("A", "SL", "DL"));
        g.AddRule(new Rules.SingleLoaded("SL", "APT"));
        g.AddRule(new Rules.DoubleLoaded("DL", "APT"));
        //g.AddRule(new Rules.DcpA("A", 9, 3));
        return so;
    }
    void Start () {
        boundary = initShape1();
        particleSystem = new SGPlaningParticleSystem(boundary);
        for (int i = 0; i < 6; i++)
        {
            //ShapeObject so = SOPoint.CreatePoint();
            ShapeObject so = createBuilding();
            particleSystem.AddRand(so);
        }
	}
	
	// Update is called once per frame
	void Update () {
        particleSystem.Update();
	}

    private void OnRenderObject()
    {
        SGGeometry.GLRender.Polyline(boundary,true,null,Color.black);
    }
}
