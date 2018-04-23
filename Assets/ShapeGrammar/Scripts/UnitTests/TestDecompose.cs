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

    // Use this for initialization
    void Start () {
        Vector3[] pts = initShape2();
        ShapeObject so = ShapeObject.CreateExtrusion(pts, 30);

        //ShapeObjectM som = ShapeObjectM.CreateSchemeA((CompositMeshable)so.meshable);

        Grammar g1 = new Grammar();
        g1.assignedObjects.Add(so);
        SceneManager.assignGrammar(g1);
        g1.AddRule(new Rules.Scale("A", "A", 1, 0));
        g1.AddRule(new Rules.Scale("A", "A", 1, 1));
        g1.AddRule(new Rules.Bisect("A", new string[] { "B", "C" }, 0.4f, 0));
        g1.AddRule(new Rules.Bisect("C", new string[] { "C", "C" }, 0.4f, 2));
        //g1.AddRule(new Rules.DcpA("A", 6, 3));
        

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
