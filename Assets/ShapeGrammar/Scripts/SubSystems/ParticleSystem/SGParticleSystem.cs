using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SGGeometry;
using SGCore;

public class SGParticleSystem {

    //public List<SGParticle> particles;
    public List<ShapeObject> particles;
    public Vector3[] boundary = null;
    public float repellForce = 0.5f;
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
        //Debug.Log("psys update");
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
        float sideGap = 15;
        Debug.Log("particel count:" + particles.Count);
        foreach (ShapeObject p1 in particles)
        {

            Debug.Log("p1.grammar=" + p1.grammar.sgbuilding);
            Vector3 offset = new Vector3(0, 0, 0);
            foreach (ShapeObject p2 in particles)
            {
                if (p1 == p2) continue;
                if (p1.grammar == null || p2.grammar == null) continue;
                if (p1.grammar.sgbuilding == null || p2.grammar.sgbuilding == null) continue;
                ShapeObject south;
                if (p1.Position.z < p2.Position.z) south = p1;
                else south = p2;

                bool EWTooClose = false;
                bool SNTooClose = false;

                float dx = p2.Position.x - p1.Position.x;
                float dz = p2.Position.z - p1.Position.z;

                GraphNode gP1 = p1.grammar.sgbuilding.gPlaning.FindFirst("SizeBuilding3D");
                float p1W = gP1.GetParamVal("Size", 0);
                float p1H = gP1.GetParamVal("Size", 1);
                float p1D = gP1.GetParamVal("Size", 2);

                //Debug.LogFormat("w={0}, h={1}, d={2}", p1W, p1H, p1D);

                GraphNode gP2 = p2.grammar.sgbuilding.gPlaning.FindFirst("SizeBuilding3D");
                float p2W = gP2.GetParamVal("Size", 0);
                float p2H = gP2.GetParamVal("Size", 1);
                float p2D = gP2.GetParamVal("Size", 2);

                
                //float p1W = (float)p1.grammar.sgbuilding.width;
                //float p1D = (float)p1.grammar.sgbuilding.depth;
                //float p1H = (float)p1.grammar.sgbuilding.height;

                //float p2W = (float)p1.grammar.sgbuilding.width;
                //float p2D = (float)p1.grammar.sgbuilding.depth;
                //float p2H = (float)p1.grammar.sgbuilding.height;

                p1H *= 0.6f;
                if (p1H < 30) p1H = 30;
                p2H *= 0.6f;
                if (p2H < 30) p2H = 30;


                //min is p1 on the right of p2;
                float minW = (p2W + sideGap) * -1;
                float maxW = p1W + sideGap;

                //min is p1 on the north of p2;
                float minD = (p2D + p2H) * -1;
                float maxD = p1D+p1H;
                
                
                if(dx>minW && dx < maxW)
                {
                    EWTooClose = true;
                }
                if (dz > minD && dz < maxD)
                {
                    SNTooClose = true;
                }

                if(SNTooClose && EWTooClose)
                {
                    Vector3 offsetV = new Vector3(-dx, 0, -dz);
                    offsetV = offsetV.normalized * this.repellForce;
                    offset += offsetV;
                    //MoveParticle(p2, offsetV / 2);
                    entrophy += offsetV.magnitude;
                }
                if (offset.magnitude > 0) Debug.Log(offset.magnitude);
               
            }
            MoveParticle(p1, offset);
        }
        entrophy /= particles.Count;
        entrophy = Mathf.Round(entrophy * 1000);
        UpdateEntropyDisplay();
        
    }

}
public class SGPlaningParticleSystemAT : SGParticleSystem
{
    public SGPlaningParticleSystemAT() : base() { }
    public SGPlaningParticleSystemAT(Vector3[] bd) : base(bd) { }
    public override void Update()
    {
        //if (paused) return;
        entrophy = 0;
        float sideGap = 15;
        foreach (ShapeObject p1 in particles)
        {
            Vector3 offset = new Vector3(0, 0, 0);
            foreach(ShapeObject p2 in particles)
            {
                if (p1 == p2) continue;
                if (p1.grammar == null || p2.grammar == null) continue;
                if (p1.grammar.building == null || p2.grammar.building == null) continue;
                ShapeObject south;
                if (p1.Position.z < p2.Position.z) south = p1;
                else south = p2;

                bool EWTooClose = false;
                bool SNTooClose = false;

                float dx = p2.Position.x - p1.Position.x;
                float dz = p2.Position.z - p1.Position.z;

                float p1W = (float)p1.grammar.building.width;
                float p1D = (float)p1.grammar.building.depth;
                float p1H = (float)p1.grammar.building.height;

                float p2W = (float)p1.grammar.building.width;
                float p2D = (float)p1.grammar.building.depth;
                float p2H = (float)p1.grammar.building.height;

                p1H *= 0.6f;
                if (p1H < 30) p1H = 30;
                p2H *= 0.6f;
                if (p2H < 30) p2H = 30;


                //min is p1 on the right of p2;
                float minW = (p2W + sideGap) * -1;
                float maxW = p1W + sideGap;

                //min is p1 on the north of p2;
                float minD = (p2D + p2H) * -1;
                float maxD = p1D + p1H;


                if (dx > minW && dx < maxW)
                {
                    EWTooClose = true;
                }
                if (dz > minD && dz < maxD)
                {
                    SNTooClose = true;
                }

                if (SNTooClose && EWTooClose)
                {
                    Vector3 offsetV = new Vector3(-dx, 0, -dz);
                    offsetV = offsetV.normalized * this.repellForce;
                    offset += offsetV;
                    //MoveParticle(p2, offsetV / 2);
                    entrophy += offsetV.magnitude;
                }


            }

            float minDA=10000000000000000;
            Vector3 minLoc=p1.Position;
            foreach(SOPoint sop in p1.grammar.building.site.attractions)
            {
                float d = Vector3.Distance(sop.Position, p1.Position);
                if (d < minDA)
                {
                    minDA = d;
                    minLoc = sop.Position;
                }
                
            }
            Vector3 offsetVA = (minLoc - p1.Position).normalized * 0.5f;
            offset += offsetVA;

            MoveParticle(p1, offset);
        }
        entrophy /= particles.Count;
        entrophy = Mathf.Round(entrophy * 1000);
        UpdateEntropyDisplay();

    }

}

public class SGPlaningParticleSystemAH : SGParticleSystem
{
    public SGPlaningParticleSystemAH() : base() { }
    public SGPlaningParticleSystemAH(Vector3[] bd) : base(bd) { }
    public override void Update()
    {
        //if (paused) return;
        entrophy = 0;
        float sideGap = 15;
        foreach (ShapeObject p1 in particles)
        {
            Vector3 offset = new Vector3(0, 0, 0);
            foreach (ShapeObject p2 in particles)
            {
                if (p1 == p2) continue;
                if (p1.grammar == null || p2.grammar == null) continue;
                if (p1.grammar.building == null || p2.grammar.building == null) continue;
                ShapeObject south;
                if (p1.Position.z < p2.Position.z) south = p1;
                else south = p2;

                bool EWTooClose = false;
                bool SNTooClose = false;

                float dx = p2.Position.x - p1.Position.x;
                float dz = p2.Position.z - p1.Position.z;

                float p1W = (float)p1.grammar.building.width;
                float p1D = (float)p1.grammar.building.depth;
                float p1H = (float)p1.grammar.building.height;

                float p2W = (float)p1.grammar.building.width;
                float p2D = (float)p1.grammar.building.depth;
                float p2H = (float)p1.grammar.building.height;

                p1H *= 0.6f;
                if (p1H < 30) p1H = 30;
                p2H *= 0.6f;
                if (p2H < 30) p2H = 30;


                //min is p1 on the right of p2;
                float minW = (p2W + sideGap) * -1;
                float maxW = p1W + sideGap;

                //min is p1 on the north of p2;
                float minD = (p2D + p2H) * -1;
                float maxD = p1D + p1H;


                if (dx > minW && dx < maxW)
                {
                    EWTooClose = true;
                }
                if (dz > minD && dz < maxD)
                {
                    SNTooClose = true;
                }

                if (SNTooClose && EWTooClose)
                {
                    float h1 = p1.grammar.building.height;
                    float h2 = p2.grammar.building.height;

                    Vector3 offsetV = new Vector3(-dx, 0, -dz);
                    offsetV = offsetV.normalized * this.repellForce;
                    
                    //MoveParticle(p2, offsetV / 2);
                    
                    
                    if (h1>30 && SNTooClose && dz < maxD)
                    {
                        Rule r1 = (Rule)p1.grammar.FindFirst("SizeBuilding3D");
                        Rule r2 = (Rule)p2.grammar.FindFirst("SizeBuilding3D");
                        if(r1!=null && r2!=null)
                        {
                            float val1 = r1.GetParamVal("Size", 1);
                            float val2 = r2.GetParamVal("Size", 1);

                            p1.grammar.building.height -= 4;
                            p2.grammar.building.height += 4;
                            
                            r1.SetParam("Size", 1, val1 - 4);
                            r2.SetParam("Size", 1, val2 + 4);
                            offsetV /= 2;
                        }
                    }
                    offset += offsetV;
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
public class SGPlaningParticleSystemV : SGParticleSystem
{
    public SGPlaningParticleSystemV() : base() { }
    public SGPlaningParticleSystemV(Vector3[] bd) : base(bd) { }
    public override void Update()
    {
        //if (paused) return;
        entrophy = 0;
        float sideGap = 15;
        foreach (ShapeObject p1 in particles)
        {
            Vector3 offset = new Vector3(0, 0, 0);
            foreach (ShapeObject p2 in particles)
            {
                if (p1 == p2) continue;
                if (p1.grammar == null || p2.grammar == null) continue;
                if (p1.grammar.building == null || p2.grammar.building == null) continue;
                ShapeObject south;
                if (p1.Position.z < p2.Position.z) south = p1;
                else south = p2;

                bool EWTooClose = false;
                bool SNTooClose = false;

                float dx = p2.Position.x - p1.Position.x;
                float dz = p2.Position.z - p1.Position.z;

                float p1W = (float)p1.grammar.building.width;
                float p1D = (float)p1.grammar.building.depth;
                float p1H = (float)p1.grammar.building.height;

                float p2W = (float)p1.grammar.building.width;
                float p2D = (float)p1.grammar.building.depth;
                float p2H = (float)p1.grammar.building.height;

                p1H *= 0.6f;
                if (p1H < 30) p1H = 30;
                p2H *= 0.6f;
                if (p2H < 30) p2H = 30;


                //min is p1 on the right of p2;
                float minW = (p2W + sideGap) * -1;
                float maxW = p1W + sideGap;

                //min is p1 on the north of p2;
                float minD = (p2D + p2H) * -1;
                float maxD = p1D + p1H;


                if (dx > minW && dx < maxW)
                {
                    EWTooClose = true;
                }
                if (dz > minD && dz < maxD)
                {
                    SNTooClose = true;
                }

                if (SNTooClose && EWTooClose)
                {
                    Vector3 offsetV = new Vector3(-dx, 0, -dz);
                    offsetV = offsetV.normalized * this.repellForce;
                    offset += offsetV;
                    //MoveParticle(p2, offsetV / 2);
                    entrophy += offsetV.magnitude;
                }


            }

            float minDA = 10000000000000000;
            Vector3 minLoc = p1.Position;
            foreach (SOPoint sop in p1.grammar.building.site.attractions)
            {
                float d = Vector3.Distance(sop.Position, p1.Position);
                if (d < minDA)
                {
                    minDA = d;
                    minLoc = sop.Position;
                }

            }
            minDA = Mathf.Clamp(minDA,0, 150);
            float h =15 + (minDA/2);
            Rule r =(Rule)p1.grammar.FindFirst("SizeBuilding3D");
            r.SetParam("Size", 1, h);
            p1.grammar.building.height = h;

            MoveParticle(p1, offset);
        }
        entrophy /= particles.Count;
        entrophy = Mathf.Round(entrophy * 1000);
        UpdateEntropyDisplay();

    }

}