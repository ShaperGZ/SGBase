using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SGCore;
using SGGeometry;

public class TestParticleSystem : MonoBehaviour {
    public SGParticleSystem particleSystem;
    // Use this for initialization
    public Vector3[] boundary;
    public static Vector3[] initShapeL()
    {
        float w = 300;
        Vector3[] pts = new Vector3[4];
        pts[0] = new Vector3(0, 0, 0);
        pts[1] = new Vector3(w, 0, 0);
        pts[2] = new Vector3(w, 0, w);
        pts[3] = new Vector3(0, 0, w);
        return pts;
    }
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
        Building bp = new Building();
        bp.AddGrammar(g);
        so.SetGrammar(g);
        
        float h = Random.Range(9, 80);
        float w = Random.Range(30, 50);
        float d = Random.Range(15, 25);
        if (h < 30)
        {
            w = Random.Range(40, 60);
            d = Random.Range(30, 50);
        }
        g.AddRule(new Rules.CreateOperableBox("A", new Vector3(30, 60, 20)));
        g.AddRule(new Rules.SizeBuilding3D("A", "A", new Vector3(w, h, d)));
        g.AddRule(new Rules.ApartmentLoadFilter("A", "SL", "DL", "CV"));
        g.AddRule(new Rules.SingleLoaded("SL", "APT"));
        g.AddRule(new Rules.DoubleLoaded("DL", "APT"));
        g.AddRule(new Rules.CentralVoid("CV", "APT", "APT2"));


        Grammar g2 = new Grammar();
        g2.category = "massingForm";
        g2.name = "AptFormA";
        g2.inputs.names.Add("APT");
        g2.AddRule(new Rules.Bisect("APT", new string[] { "APT", "C" }, 0.4f, 0), false);
        g2.AddRule(new Rules.Bisect("C", new string[] { "C", "APT" }, 0.25f, 2), false);
        g2.AddRule(new Rules.Scale3D("C", "APT", new Vector3(1.3f, 0.7f, 1.6f), null, Alignment.NE), false);


        g.AddRule(g2);

        //g.AddRule(new Rules.DivideToFTFH("APT", "APTL", 4));
        //g.AddRule(new Rules.DivideToFTFH("APT2", "APTL", 4));

        //g.AddRule(new Rules.DcpA("A", 9, 3));
        return so;
    }
    Site site;
    void Start () {
        //boundary = initShape1();
        boundary = initShapeL();
        BoundingBox bbox = BoundingBox.CreateFromPoints(boundary);
        Debug.Log(bbox.Format());

        site = new Site(boundary);


        particleSystem = new SGPlaningParticleSystem(boundary);
        //particleSystem = new SGPlaningParticleSystemAT(boundary);
        //particleSystem = new SGParticleSystem(boundary);
        for (int i = 0; i < 10; i++)
        {
            //ShapeObject so = SOPoint.CreatePoint();
            ShapeObject so = createBuilding();
            Building building = so.grammar.building;
            building.SetRefPos(so);
            building.site = site;
            
            site.buildings.Add(building);
            particleSystem.AddRand(so);
        }

        SOPoint[] sops = new SOPoint[3];
        sops[0] = SOPoint.CreatePoint(new Vector3(60, 0, -30));
        sops[1] = SOPoint.CreatePoint(new Vector3(-30, 0, 80));
        sops[2] = SOPoint.CreatePoint(new Vector3(320, 0, 200));
        for (int i = 0; i < sops.Length; i++)
        {
            sops[i].sizable = false;
            float r = 20;
            sops[i].Size = new Vector3(r, r, r);
        }
        site.attractions.AddRange(sops);

	}
	public void SetParticleSystem(SGParticleSystem ps)
    {
        ps.particles = this.particleSystem.particles;
        this.particleSystem = ps;
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
