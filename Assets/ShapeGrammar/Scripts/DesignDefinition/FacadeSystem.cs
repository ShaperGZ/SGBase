using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SGGeometry;
using SGCore;

public class ExtractTransform
{
    public Vector3 position;
    public Vector3 size;
    public Vector3 normal;

    public ExtractTransform(Vector3 pos, Vector3 s, Vector3 n)
    {
        position = pos;
        size = s;
        normal = n;
    }

    public void SetTransform(Transform trans)
    {
        trans.position = position;
        trans.localScale = size;
        trans.LookAt(position + normal);
    }
}


public class Facade
{
    public Meshable face;
    public float bayWidth=6;
    public float bayHeight = 4;
    public string facadeType = "";
    public GameObject prefab;
    FacadeSystem facadeSystem;
    
    public Facade(Meshable face, float width=6, float height = float.NaN)
    {
        this.face = face;
        bayWidth = width;
        bayHeight = height;
    }
    public Facade(Meshable face, string name, float width = 6, float height = float.NaN):this(face,width,height)
    {
        facadeType = name;
    }
    public void SetComponent(GameObject prefab)
    {
        this.prefab = prefab;
        facadeSystem.GenerateComponents(this);
    }
}

public class FacadeSystem {
    Building building;
    Dictionary<string, List<Facade>> faces;
    Dictionary<Facade, List<ExtractTransform>> transforms;
    Dictionary<Facade, List<GameObject>> components;

    public void AddFace(string name, Meshable face, float bayWidth= 6)
    {
        if (!faces.ContainsKey(name))
        {
            faces[name] = new List<Facade>();
        }
        Facade fc = new Facade(face, name, bayWidth);
        faces[name].Add(fc);
    }
    
    public void Clear()
    {
        ClearFaces();
        ClearComponents();
    }
    public void invalidate(bool forceAll=false)
    {
        GenerateTransforms();
        GenerateComponents();
    }
    public void ClearTransforms()
    {
        transforms = new Dictionary<Facade, List<ExtractTransform>>();
    }
    public void GenerateTransforms()
    {
        foreach (string k in faces.Keys)
        {
            foreach (Facade f in faces[k])
            {
                generateTransforms(f);
            }
        }
    }
    public void generateTransforms(Facade facade)
    {
        transforms[facade] = divideFaces(facade);
    }
    public void GenerateComponents(Facade facade)
    {
        if (components.ContainsKey(facade))
        {
            ClearComponents(facade);
        }
        else
        {
            components[facade] = new List<GameObject>();
            List<ExtractTransform> trans = transforms[facade];
            foreach(ExtractTransform t in trans)
            {
                GameObject prefab = facade.prefab;
                GameObject o = GenerateGameObjectToTransform(prefab, t);
                components[facade].Add(o);
            }
            //TODO:generate components
            //generate components
        }
    }

    public void GenerateComponents()
    {
        foreach(string k in faces.Keys)
        {
            foreach (Facade f in faces[k])
            {
                GenerateComponents(f);
            }
        }
    }
    public void ClearFaces()
    {
        faces = new Dictionary<string, List<Facade>>();
    }
    public void ClearComponents()
    {
        foreach (Facade key in components.Keys)
        {
            ClearComponents(key);
        }
        components = new Dictionary<Facade, List<GameObject>>();
    }
    public void ClearComponents(Facade key)
    {
        List<GameObject> comps = components[key];
        foreach (GameObject o in comps)
        {
            GameObject.Destroy(o);
        }
        components[key].Clear();
    }

    
    private List<ExtractTransform> divideFaces(Facade facade)
    {
        throw new System.NotImplementedException();
        
        //Vector3[] pts = face.vertices;
        //Vector3 v1 = pts[1] - pts[0];
        //Vector3 v2 = pts[3] - pts[0];

        //int countW = v1.magnitude / bayWidth;
        



        //Vector3 size = new Vector3(v1.magnitude, v2.magnitude, 1);
        //Vector3 n = Vector3.Cross(v1.normalized, v2.normalized);
        //ExtractTransform tran = new ExtractTransform(pts[0],size,n);

    }
    private GameObject GenerateGameObjectToTransform(GameObject prefab, ExtractTransform trans)
    {
        GameObject o = GameObject.Instantiate(prefab);
        trans.SetTransform(o.transform);
        return o;
    }
}
