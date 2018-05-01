using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SGGeometry;

public class SGParticleSystem {

    //public List<SGParticle> particles;
    public List<ShapeObject> particles;
    public Vector3[] boundary = null;
    public float repellForce = 0.2f;
    public bool paused = false;
    public bool stopped = false;
    public float entrophy = 0;
    BoundingBox bbox;
    Random rand;
    Button pauseButton;

    public SGParticleSystem()
    {
        //particles = new List<SGParticle>();
        particles = new List<ShapeObject>();
        rand = new Random();
        pauseButton = GameObject.Find("BtParticleSystem").GetComponent<Button>();
        if(pauseButton!=null)
        {
            pauseButton.onClick.AddListener(delegate { TogglePause(); });
        }
    }
    public SGParticleSystem(Vector3[] bd):this()
    {
        SetBoundary(bd);
    }
    public void TogglePause()
    {
        paused = !paused;
    }
    public void SetBoundary(Vector3[] pts)
    {
        boundary = pts;
        bbox = BoundingBox.CreateFromPoints(boundary);

        //foreach (Vector3 p in bbox.vertices)
        //    Debug.Log("bboxVertices:"+p);
    }
    //public float W() { return 0; }
    //public float E() { return 50; }
    //public float N() { return 50; }
    //public float S() { return 0; }
    public float W() { return bbox.vertices[0].x; }
    public float E() { return bbox.vertices[3].x; }
    public float N() { return bbox.vertices[3].z; }
    public float S() { return bbox.vertices[0].z; }

    public virtual void AddRand(ShapeObject so=null)
    {
        if(so == null)
        {
            so = SOPoint.CreatePoint();
        }

        Vector3 center = SGGeometry.SGUtility.CenterOfGravity(boundary);
        center[0] += Random.Range(-5, 5);
        center[2] += Random.Range(-5, 5);
        
        //float x = Random.Range(W(), E());
        //float y = 0;
        //float z = Random.Range(N(), S());
        //Vector3 p = new Vector3(x, y, z);
        Vector3 p = center;
        so.Position = p;
        particles.Add(so);
        
    }
	
    public virtual void Add(Vector3 pos)
    {

    }
    public void MoveParticle(ShapeObject p, Vector3 v)
    {
        if (paused) return;
        p.Position = CapInBound(p.Position + v);
    }
    public virtual Vector3 CapInBound(Vector3 v)
    {
        return SGGeometry.SGUtility.CapPointInBoundaryA(boundary, v);
        //return v;
    }
    public virtual void Update()
    {
        //if (paused) return;
        foreach(ShapeObject p1 in particles)
        {
            Vector3 offset = new Vector3(0, 0, 0);
            foreach (ShapeObject p2 in particles)
            {
                if (p1 == p2) continue;
                float d = Vector3.Distance(p1.Position, p2.Position);
                if (d < 10)
                {
                    Vector3 v = p2.Position - p1.Position;
                    v = v.normalized * repellForce;
                    offset -= v;
                    MoveParticle(p2, v/2);
                }
            }
            MoveParticle(p1, offset);
        }
    }
    public virtual void OnRenderObject() { }
    public void UpdateEntropyDisplay()
    {
        if (pauseButton)
        {
            pauseButton.transform.Find("Text").GetComponent<Text>().text = "entropy:" + entrophy.ToString();
        }
    }
}

public class SGPlaningParticleSystem:SGParticleSystem
{
    public SGPlaningParticleSystem() : base() { }
    public SGPlaningParticleSystem(Vector3[] bd) : base(bd) { }
    public override void Update()
    {
        //if (paused) return;
        entrophy = 0;
        foreach (ShapeObject p1 in particles)
        {
            Vector3 offset = new Vector3(0, 0, 0);
            foreach (ShapeObject p2 in particles)
            {
                if (p1 == p2) continue;
                if (p1.grammar == null || p2.grammar == null) continue;
                if (p1.grammar.properties == null || p2.grammar.properties == null) continue;
                ShapeObject south;
                if (p1.Position.z < p2.Position.z) south = p1;
                else south = p2;

                float w = 30 + 15;
                float h = (float) (south.grammar.properties.properties["height"]);
                h *= 0.6f;
                if (h < 30) h = 30;

                float distH = p1.Position.z - p2.Position.z;
                float distW = p1.Position.x - p2.Position.x;

                float distHA = Mathf.Abs(distH);
                float distWA = Mathf.Abs(distW);

                if(distWA < w && distHA < h)
                {
                    Vector3 offsetV = new Vector3(distW, 0, distH);
                    offsetV = offsetV.normalized * this.repellForce;
                    offset += offsetV;
                    MoveParticle(p2, offsetV / 2);
                    entrophy += offsetV.magnitude;
                }
                
            }
            MoveParticle(p1, offset);
        }
        entrophy /= particles.Count;
        entrophy = Mathf.Round(entrophy * 1000);
        UpdateEntropyDisplay();
    }
}