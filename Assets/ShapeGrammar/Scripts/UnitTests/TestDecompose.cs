using UnityEngine;
using SGGeometry;
using SGCore;
public class TestDecompose : MonoBehaviour
{
    public GameObject comp;
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
    public static Vector3[] initShape2()
    {
        Vector3[] pts = new Vector3[4];
        pts[0] = new Vector3(-20, 0, 15);
        pts[1] = new Vector3(-20, 0, -15);
        pts[2] = new Vector3(20, 0, -15);
        pts[3] = new Vector3(20, 0, 15);
        return pts;
    }

    public Grammar JsPollock(ShapeObject initSo)
    {
        Grammar gs = new Grammar();
        gs.inputs.shapes.Add(initSo);
        gs.name = "Site";
        //gs.AddRule(new Rules.BisectMirror("A", new string[] { "A", "A" }, 0.45f, 1));
        gs.AddRule(new Rules.DivideTo("A", new string[] { "A", "B" }, 60, 0));
        gs.AddRule(new Rules.PivotTurn("B", "A", 2));
        for (int i = 0; i < 3; i++)
        {
            int orientation = (i+1) % 2;
            if (orientation == 1) orientation = 2;
            gs.AddRule(new Rules.Bisect("A", new string[] { "B", "A" }, 0.4f, orientation));
            gs.AddRule(new Rules.PivotTurn("B", "A", 2));
            
            //gs.AddRule(new Rules.Bisect("A", new string[] { "B", "A" }, 0.4f, 2));
            //gs.AddRule(new Rules.PivotTurn("B", "A", 2));
        }
        gs.AddRule(new Rules.NamesByAreaEdges("A", "Building", 800, 4));
        gs.AddRule(new Rules.Scale3D("Building", "Building", new Vector3(1, 25, 1), new Vector2(-0.2f, 0.1f)));
        //gs.AddRule(new Rules.DcpA("Building", 6, 3));
        //gs.AddRule(new Rules.BisectLength("Building", new string[] { "Tower", "Podium"}, 20, 2));
        //gs.AddRule(new Rules.Scale3D("Tower", "Tower", new Vector3(0.6f, 3, 1)));
        //gs.AddRule(new Rules.DubTop("Building"));

        return gs;
    }

    public Grammar GenSite(ShapeObject initSo)
    {
        //#3 "position":0.19
        //#5 "position":0.14
        Grammar gs = new Grammar();
        gs.inputs.shapes.Add(initSo);
        gs.name = "Site";
        //gs.AddRule(new Rules.BisectMirror("A", new string[] { "A", "A" }, 0.45f, 1));
        gs.AddRule(new Rules.Bisect("A", new string[] { "A", "C" }, 0.2f, 0));
        gs.AddRule(new Rules.PivotMirror("C", "A", 0));
        gs.AddRule(new Rules.Bisect("A", new string[] { "B", "C" }, 0.2f, 0));
        gs.AddRule(new Rules.PivotTurn("C", "C", 1));
        //gs.AddRule(new Rules.Divide("A", new string[] { "A" }, new float[] { 0.2f, 0.3f, 0.5f }, 0));
        //gs.AddRule(new Rules.DivideTo("A", new string[] { "B", "C" }, 20f, 0));
        return gs;
    }

    // Use this for initialization
    void Start () {
        Vector3[] pts = initShape1();
        //ShapeObject site = ShapeObject.CreatePolygon(pts);
        ShapeObject site = ShapeObject.CreateExtrusion(pts, 1);
        Grammar gs = JsPollock(site);
        gs.SetSubParamValue(2, "Position", 0.19f);
        gs.SetSubParamValue(4, "Position", 0.14f);
        gs.Execute();

        gs.ExtractParam(2, "Position","d1");
        gs.ExtractParam(4, "Position","d2");

        //ShapeObjectM som = ShapeObjectM.CreateSchemeA((CompositMeshable)so.meshable);
        Properties buildingA = new BuildingProperties();
        buildingA.AddGrammar(gs);
        gs.InvalidateBuilding();
    }

	// Update is called once per frame
	void Update () {
		//Time.deltaTime
	}


    public void normalizeMesh(GameObject o)
    {
        Mesh mesh = o.GetComponent<MeshFilter>().sharedMesh;
        
        float yMax = 0;
        float xMax = 0;
        float zMax = 0;
        foreach (Vector3 v in mesh.vertices)
        {
            //Vector3(transform)
            if (v.y > yMax) yMax = v.y;
            if (v.x > xMax) xMax = v.x;
            if (v.z > zMax) zMax = v.z;
        }
        Debug.LogFormat("max values={0},{1},{2}", xMax, yMax, zMax);
        float scaleY = 1 / yMax;
        float scaleX = 1 / xMax;
        float scaleZ = 1;
        //float scaleZ = 1 / zMax;
        Vector3 scale = new Vector3(scaleX, scaleY, scaleZ);
        Debug.LogFormat("scale={0}", scale);
        Vector3[] pts = mesh.vertices;
        for (int i = 0; i < pts.Length; i++)
        {
            pts[i].Scale(scale);
        }
        mesh.vertices = pts;
        mesh.RecalculateBounds();
    }
}
