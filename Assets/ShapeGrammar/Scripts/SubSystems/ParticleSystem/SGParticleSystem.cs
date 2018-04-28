using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SGGeometry;

public class SGParticleSystem {

    public List<SGParticle> particles;
    public Vector3[] boundary = null;
    BoundingBox bbox;
    Random rand;

    public SGParticleSystem()
    {
        particles = new List<SGParticle>();
        rand = new Random();
    }
    public SGParticleSystem(Vector3[] bd)
    {
        boundary = bd;
    }
    public void SetBoundary(Vector3[] pts)
    {
        bbox = BoundingBox.CreateFromPoints(pts,new Vector3(0,1,0));
    }
    public float W() { return bbox.vertices[0].x; }
    public float E() { return bbox.vertices[1].x; }
    public float N() { return bbox.vertices[4].z; }
    public float S() { return bbox.vertices[0].z; }

    public virtual void AddRand()
    {
        float x = Random.Range(W(), E());
        float y = 0;
        float z = Random.Range(N(), S());
        Vector3 p = new Vector3(x, y, z);

        
    }
	
    public virtual void Add(Vector3 pos)
    {

    }
}
