using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SGCore;
using SGGeometry;

public class TestParticleSystem : MonoBehaviour {
    SGParticleSystem particleSystem;
    // Use this for initialization
    public Vector3[] boundary;
    public static Vector3[] initShape2()
    {
        Vector3[] pts = new Vector3[4];
        pts[0] = new Vector3(-20, 0, 15);
        pts[1] = new Vector3(-20, 0, -15);
        pts[2] = new Vector3(20, 0, -15);
        pts[3] = new Vector3(20, 0, 15);
        pts[3] = new Vector3(10, 0, 30);
        return pts;
    }
    ShapeObject createBuilding()
    {
        ShapeObject so = SOPoint.CreatePoint();
        Grammar g = new Grammar();
        BuildingProperties bp = new BuildingProperties();
        bp.AddGrammar(g);
        so.SetGrammar(g);
        g.AddRule(new Rules.CreateOperableBox("A", new Vector3(1, 1, 1)));
        float h = Random.Range(9, 30);
        g.AddRule(new Rules.Size3D("A", "A", new Vector3(30, h, 15)));
        g.AddRule(new Rules.PivotTurn("A", "A", 2));
        g.AddRule(new Rules.BisectLength("A", new string[] { "CD", "A" }, 2, 2));
        g.AddRule(new Rules.CreateStair("CD", "STAR"));
        //g.AddRule(new Rules.DcpA("A", 9, 3));
        return so;
    }
    void Start () {

        boundary = initShape2();
        //Vector3[] pts = initShape2();

        ShapeObject b = ShapeObject.CreatePolygon(boundary);
        particleSystem = new SGParticleSystem(boundary);
        //particleSystem = new SGPlaningParticleSystem();
        for (int i = 0; i < 15; i++)
        {
            ShapeObject so = SOPoint.CreatePoint();
            //ShapeObject so = createBuilding();
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
