using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SGGeometry;
using SGCore;


public class ShapeObject : MonoBehaviour {
    public Vector3 Size
    {
        get
        {
            return transform.localScale;
        }
        set
        {
            for (int i = 0; i < 3; i++)
                if (value[i] == 0) value[i] = 1;
            transform.localScale = value;
        }
    }
    public Vector3[] Vects
    {
        get
        {
            Vector3 z = transform.forward;
            Vector3 y = transform.up;
            Vector3 x = Vector3.Cross(y,z);
            return new Vector3[] { x, y, z };
        }
    }
    public Meshable meshable;
    public Rule parentRule;
    public int step;

    public static Material DefaultMat
    {
        get
        {
            if(_defaultMat == null)
            {
                _defaultMat = Resources.Load("Mat0") as Material;
            }
            return _defaultMat;
        }
    }

    public bool highlightScope = false;
    public bool drawScope = true;
    private static Material _defaultMat;
    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;
    private BoxCollider boxCollider;
    // Use this for initialization
    void Start () {
        meshFilter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();
        boxCollider = GetComponent<BoxCollider>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public string Format()
    {
        string txt = "";
        string draw = "-";
        if (gameObject.activeSelf) draw = "+";
        string ruleName;
        if (parentRule == null) ruleName = "unnamedRule";
        else ruleName = parentRule.name;
        txt += draw + name + "_("+ruleName + "_step" + step+")";

        return txt;
    }
    
    public void Show(bool flag)
    {
        gameObject.active = flag;
        //if (meshRenderer == null) meshRenderer = GetComponent<MeshRenderer>();
        //meshRenderer.enabled = flag;
        //if(boxCollider == null) boxCollider = GetComponent<BoxCollider>();
        //boxCollider.enabled = flag;
    }
    private void OnRenderObject()
    {
        //GLDrawScope(Color.black);
        if (drawScope && meshRenderer.enabled)
        {
            Color c = Color.black;
            //if (highlightScope)
            //    c = Color.red;

            GLDrawScope(c);
            try
            {
                foreach (Meshable m in ((CompositMeshable)meshable).components)
                {
                    if (m.displayLines != null && m.displayLines.Count > 0)
                    {
                        foreach (Polyline pl in m.displayLines)
                        {
                            SGGeometry.GLRender.Polyline(pl.vertices, false, null, Color.black);
                        }
                    }
                }
            }

            catch { }
        }
    }
    private void OnPostRender()
    {
        //not working
        GLDrawScope(Color.red);
        if (highlightScope)
        {

            GLDrawScope(Color.red);
        }
    }

    Vector3[] makeBoxPoints()
    {
        Vector3[] opts = new Vector3[8];
        Vector3[] vects = Vects;
        for (int i = 0; i < 3; i++)
            vects[i] *= Size[i];
        Vector3 size = Size;

        //opts[0] = new Vector3(-0.5f,-0.5f,-0.5f);
        opts[0] = transform.position;
        opts[1] = opts[0] + (vects[0]);
        opts[2] = opts[1] + (vects[2]);
        opts[3] = opts[0]+ (vects[2]);
        for( int i = 0; i < 4; i++)
        {
            opts[i + 4] = opts[i] + vects[1];
        }
        return opts;

    }
    void GLDrawScope(Color color)
    {
        //Color color = Color.black;
        Material mat = UserStats.LineMat;
        mat.SetPass(0);
        Vector3[] pts = makeBoxPoints();
        GL.Begin(GL.LINES);
        

        GL.Color(color);
        GL.Vertex(pts[1]);
        GL.Vertex(pts[2]);
        GL.Vertex(pts[2]);
        GL.Vertex(pts[3]);
        GL.Vertex(pts[3]);
        GL.Vertex(pts[0]);

        GL.Vertex(pts[4]);
        GL.Vertex(pts[5]);
        GL.Vertex(pts[5]);
        GL.Vertex(pts[6]);
        GL.Vertex(pts[6]);
        GL.Vertex(pts[7]);
        GL.Vertex(pts[7]);
        GL.Vertex(pts[4]);

        for(int i =0;i<4;i++)
        {
            GL.Vertex(pts[i]);
            GL.Vertex(pts[i+4]);
        }
        GL.Color(Color.red);
        GL.Vertex(pts[0]);
        GL.Vertex(pts[1]);
        GL.Color(Color.blue);
        GL.Vertex(pts[0]);
        GL.Vertex(pts[3]);
        GL.Color(Color.green);
        GL.Vertex(pts[0]);
        GL.Vertex(pts[4]);
        GL.End();
    }
    public void RefreshOnMeshableUpdate(Vector3? direction = null)
    {
        if (direction.HasValue)
        {
            SetMeshable(meshable,direction);
        }
        else
        {
            GetComponent<MeshFilter>().mesh = meshable.GetMeshForm();
        }
    } 
    public void SetMeshable(Meshable imeshable, Vector3? direction=null)
    {
        meshable = imeshable;
        Vector3 vectu;
        if (direction.HasValue) vectu = direction.Value;
        else vectu = new Vector3(1, 0, 0);
        BoundingBox bbox = meshable.GetBoundingBox(vectu);

       

        transform.position = bbox.vertices[0];
        transform.LookAt(bbox.vertices[3]);
        Size = bbox.size;
        
        Mesh mesh = meshable.GetNormalizedMesh(bbox);
        GetComponent<MeshFilter>().mesh = mesh;
        //print("mesh.verticeCount=" + mesh.vertexCount.ToString());

    }
    private void OnDestroy()
    {
        Debug.LogWarning("SHAPE OBJECT DESTROY WARNING:" + Format());
    }
    public static ShapeObject CreateBasic()
    {
        GameObject o = new GameObject();
        ShapeObject so = o.AddComponent<ShapeObject>();
        so.meshFilter = o.AddComponent<MeshFilter>();
        so.meshRenderer = o.AddComponent<MeshRenderer>();
        BoxCollider bc= o.AddComponent<BoxCollider>();
        bc.center = new Vector3(0.5f, 0.5f, 0.5f);
        o.AddComponent<HighlightMouseOver>();
        so.meshRenderer.material = DefaultMat;
        return so;
    }
    public static ShapeObject CreatePolygon(Vector3[] pts)
    {
        Polygon pg = new Polygon(pts);
        ShapeObject so = ShapeObject.CreateBasic();
        Vector3? ld = PointsBase.LongestDirection(pts);
        so.SetMeshable(pg, ld);
        return so;

    }
    public static ShapeObject CreateExtrusion(Vector3[] pts, float d)
    {
        Vector3 magUp = new Vector3(0, d, 0);
        Polygon pg = new Polygon(pts);
        Form ext = pg.Extrude(magUp);

        ShapeObject so = ShapeObject.CreateBasic();
        Vector3? ld = PointsBase.LongestDirection(pts);
        so.SetMeshable(ext, ld);
        return so;
    }
    public static ShapeObject CreateMeshable(Meshable mb)
    {
        ShapeObject so = ShapeObject.CreateBasic();
        Vector3 ld = mb.direction;
        so.SetMeshable(mb, ld);
        return so;
    }

    public ShapeObject PivotMirror(int axis, bool duplicate=false)
    {
        Vector3 size = Size;
        Vector3 vect = Vects[axis].normalized;
        Vector3 offset = vect*size[axis];
        
        size[axis] *= -1;
        //transform.position += offset;
        //transform.localScale = scale;
        Vector3 mscale = new Vector3(1, 1, 1);
        mscale[axis] *= -1;
        meshable.Scale(mscale, Vects, transform.position, false);
        meshable.ReverseTriangle();

        //for (int i = 0; i < meshable.vertices.Length; i++)
        //{
        //    meshable.vertices[i] = matrix.MultiplyPoint3x4(meshable.vertices[i]);
        //    //meshable.vertices[i] += offset;
        //}

        //meshable=((CompositMeshable)meshable).Transform(matrix,true);
        SetMeshable(meshable);

        return null;
    }
    
}
