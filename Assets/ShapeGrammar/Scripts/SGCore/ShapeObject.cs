﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SGGeometry;
using SGCore;


public class ShapeObject : MonoBehaviour {
    public virtual Vector3 Size
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
    public System.Guid guid;
    public string sguid;
    public Meshable meshable;
    public Rule parentRule;
    public Grammar grammar;
    public int step;

    public Dictionary<int, Material> materialsByMode;
    public Material matDefault;
    public Material matRuleMode;
    public Material matNameMode;
    public Material matVisualMode;
    public Material matProgramMode;
    

    public static Material DefaultMat
    {
        get
        {
            if(_defaultMat == null)
            {
                _defaultMat = MaterialManager.GB.Default;
                //_defaultMat = Resources.Load("Mat0") as Material;
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
    public bool stale = false;

    public virtual void Clear()
    {

    }
    private void Awake()
    {
        //guid = System.Guid.NewGuid();
        //sguid = ShortGuid();
    }

    // Use this for initialization
    void Start () {
        materialsByMode = new Dictionary<int, Material>();
        meshFilter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();
        SetDefaultMaterials();
        boxCollider = GetComponent<BoxCollider>();
        guid = System.Guid.NewGuid();
        sguid = ShortGuid();
        SceneManager.CreateShape(this);
    }
	public void SetGrammar(Grammar g, bool execute=false)
    {
        if(grammar != null)
        {
            grammar.Clear();
        }
        if (g == null) return;

        grammar = g;
        g.assignedObjects.Clear();
        g.assignedObjects.Add(this);

        //g.inputs.shapes = new List<ShapeObject>();
        //g.inputs.shapes.Add(this);
        if (execute)
        {
            g.Execute();
        }
    }
    void SetDefaultMaterials()
    {
        materialsByMode[DisplayMode.NORMAL]= MaterialManager.GB.Default;
        materialsByMode[DisplayMode.NAMES] = MaterialManager.GB.NameDifferentiate;
        materialsByMode[DisplayMode.RULE] = MaterialManager.GB.RuleEditing;
        materialsByMode[DisplayMode.VISUAL] = MaterialManager.GB.Wall0;
        if(meshRenderer!=null)
            meshRenderer.material = materialsByMode[DisplayMode.NORMAL];
    }
    public void SetMaterial(int mode)
    {
        meshRenderer.material = materialsByMode[mode];
    }
    public void SetMaterial(Material m)
    {
        meshRenderer.material = m;
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
    public string ShortGuid()
    {
        string l = guid.ToString();
        int[] indices = new int[] { 0, 1, 2, 3, 10, 11, 12, 13 };
        string s="";
        s += l.Substring(0, 4) + l.Substring(10, 4);
        return s;


    }
    public void Show(bool flag)
    {
        //if(grammar==null)
        gameObject.SetActive(flag);
        if(flag)
            Invalidate();
    }
    public void SetDisplayMode(int mode)
    {
        switch (mode)
        {
            case DisplayMode.NORMAL:
                meshRenderer.material = matNameMode;
                break;
            case DisplayMode.RULE:
                meshRenderer.material = matRuleMode;
                break;
            case DisplayMode.VISUAL:
                meshRenderer.material = matVisualMode;
                break;
            default:
                Debug.LogWarning("the given mode is not implimented, mode=" + mode);
                break;
        }
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
    protected void GLDrawScope(Color color)
    {
        //Color color = Color.black;
        Material mat = SceneManager.LineMat;
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
        BoundingBox bbox;
        if (direction.HasValue)
        {
            vectu = direction.Value;
            bbox = meshable.GetBoundingBox(vectu);
            meshable.bbox = bbox;
        }
        else if (meshable.bbox == null)
        {
            //Debug.Log("bounding box not found !!!");
            vectu = new Vector3(1, 0, 0);
            bbox = meshable.GetBoundingBox(vectu);
            meshable.bbox = bbox;
        }
        else
        {
            //Debug.Log("assigning existing bounding box");
            bbox = meshable.bbox;
        }
        ConformToBBox(bbox);
        stale = true;
    }
    
    public void SetMeshable(Meshable imeshable, BoundingBox refBbox)
    {
        meshable = imeshable;
        Vector3 vectu;
        BoundingBox bbox = meshable.GetBoundingBox(refBbox);
        
        ConformToBBox(bbox);
        stale = true;
    }
    private void ConformToBBox(BoundingBox bbox)
    {
        transform.position = bbox.vertices[0];
        transform.LookAt(bbox.vertices[3]);
        transform.localScale = bbox.GetSignedSize();
        
        Mesh mesh = meshable.GetNormalizedMesh(bbox);
        meshable.bbox = bbox;
        GetComponent<MeshFilter>().mesh = mesh;
    }
    public void Invalidate()
    {
        if (!stale) return;
        if (grammar != null)
        {
            //Show(false);
            grammar.Execute();
        }
        stale = false;
    }
    private void OnDestroy()
    {
        SceneManager.DestroyShape(guid);
        if (grammar != null)
        {
            grammar.Clear(true);
        }
        //Debug.LogWarning("SHAPE OBJECT DESTROY WARNING:" + Format());
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
        //so.meshRenderer.material = DefaultMat;
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
        Form ext = pg.ExtrudeToForm(magUp);

        ShapeObject so = ShapeObject.CreateBasic();
        Vector3? ld = PointsBase.LongestDirection(pts);
        so.SetMeshable(ext, ld);
        return so;
    }
    public static ShapeObject CreateMeshable(Meshable mb, Vector3? direction=null)
    {
        ShapeObject so = ShapeObject.CreateBasic();
        so.SetMeshable(mb,direction);
        
        return so;
    }
    public void PivotTurn(int num=1)
    {
        if (num <= 0) return;

        Vector3 size = Size;
        Vector3[] vects = Vects;
        Vector3 offset;
        Vector3 uOffset;
        float euler;
        num = Mathf.Clamp(num, -3, 3);
        
        if (num > 0)//clock wise
        {
            euler = 90;
            offset = vects[2] * size[2];
            uOffset = new Vector3(1, 0, 0);
        }
        else//counter clck wise
        {
            euler = -90;
            offset = vects[2] * size[2] * -1;
            uOffset = new Vector3(-1, 0, 0);
        }
        offset = vects[2] * size[2];
        Matrix4x4 mt = Matrix4x4.Translate(uOffset);
        Matrix4x4 mr = Matrix4x4.Rotate(Quaternion.Euler(new Vector3(0, -euler, 0)));

        Mesh m = GetComponent<MeshFilter>().mesh;
        Vector3[] opts = m.vertices.Clone() as Vector3[];
        for (int i = 0; i < opts.Length; i++)
        {
            opts[i] = mr.MultiplyPoint3x4(opts[i]);
            opts[i] = mt.MultiplyPoint3x4(opts[i]);
        }
        m.vertices = opts;
        m.RecalculateBounds();
        m.RecalculateNormals();

        transform.Rotate(new Vector3(0, euler, 0));
        Vector3 scale = transform.localScale;
        scale[0] = transform.localScale[2];
        scale[2] = transform.localScale[0];
        transform.localScale = scale;
        transform.position += offset;

        PivotTurn(num - 1);

    }
    public void PivotMirror(int axis)
    {
        meshable.bbox = BoundingBox.Reflect(meshable.bbox,axis);
        meshable.ReverseSide();
        SetMeshable(meshable);
    }
    //public void PivotMirror2(int axis)
    //{
    //    Vector3 size = Size;
    //    Vector3 vect = Vects[axis].normalized;
    //    Vector3 offset = vect*size[axis];
    //    Mesh m = GetComponent<MeshFilter>().mesh;
    //    Vector3 reflection = new Vector3(1, 1, 1);
    //    reflection[axis] *= -1;
        
    //    Matrix4x4 m1 = Matrix4x4.Scale(reflection);
    //    Matrix4x4 m2 = Matrix4x4.Translate(offset);
        
    //    Vector3 scale = transform.localScale;
    //    scale[axis] *= -1;
    //    transform.localScale = scale;
    //    transform.position = m2.MultiplyPoint3x4(transform.position);

    //    Vector3 uOffset = new Vector3(0, 0, 0);
    //    uOffset[axis] = 1;
    //    Matrix4x4 m3 = Matrix4x4.Translate(uOffset);

    //    Vector3[] opts = m.vertices.Clone() as Vector3[];
    //    for (int i = 0; i < opts.Length; i++)
    //    {
    //        opts[i] = m1.MultiplyPoint3x4(opts[i]);
    //        opts[i] = m3.MultiplyPoint3x4(opts[i]);

    //    }
    //    int[] tris = m.triangles.Clone() as int[];
    //    for (int i = 0; i < tris.Length; i += 3)
    //    {
    //        tris[i + 1] = m.triangles[i + 2];
    //        tris[i + 2] = m.triangles[i + 1];
    //    }
        
    //    m.vertices = opts;
    //    m.triangles = tris;
    //    m.RecalculateNormals();
    //    m.RecalculateBounds();

    //    meshable.bbox = BoundingBox.CreateFromBox(transform.position, Vects, Size);

    //}
    public ShapeObject Clone( bool geometryOnly = true)
    {
        ShapeObject so = ShapeObject.CreateBasic();
        CloneTo(so);
        return so;
    }
    public void CloneTo(ShapeObject so, bool geometryOnly=true)
    {
        so.transform.position = transform.position;
        //so.transform.Rotate(transform.eulerAngles);
        //so.transform.Rotate(transform.rotation.eulerAngles);
        so.transform.up = transform.up;
        so.transform.localScale = transform.localScale;
        //so.transform.localPosition = transform.localPosition;
        so.transform.localRotation = transform.localRotation;
        //so.transform.localEulerAngles = transform.localEulerAngles;
        Debug.Log("cloning Meshable");
        so.meshable = (Meshable)meshable.Clone();
        so.GetComponent<MeshFilter>().mesh = GetComponent<MeshFilter>().mesh;
        if (!geometryOnly)
        {
            so.name = name;
            so.step = step;
            so.parentRule = parentRule;
        }
    }
}
