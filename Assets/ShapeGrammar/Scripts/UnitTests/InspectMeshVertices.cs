using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SGGeometry;


public class OSP
{
    public Vector3 pos;
    public string txt;
    public OSP(string t)
    {
        t = txt;
        pos = new Vector3();
    }
    public Vector2 srcPt
    {
        get
        {
            return Camera.main.WorldToScreenPoint(pos);
        }
    }
}

public class InspectMeshVertices : MonoBehaviour {
    
    Meshable mb;
    Mesh mesh;
    List<OSP> osps;
    private void OnGUI()
    {
        foreach (OSP osp in osps)
        {
            Vector2 srp = osp.srcPt;
            GUI.Label(new Rect(srp.x, srp.y, 100, 50), osp.txt);
        }
    }

    // Use this for initialization
    void Start()
    {
        
    }
	
    void Load()
    {
        mb = GetComponent<ShapeObject>().meshable;
        mesh = GetComponent<MeshFilter>().mesh;
        osps = new List<OSP>();

        foreach (Meshable m in ((CompositMeshable)mb).components)
        {
            //foreach (Vector3 v in m.vertices)
            for (int i = 0; i < m.vertices.Length; i++)
            {
                osps.Add(new OSP(i.ToString()));
            }
        }
    }

	// Update is called once per frame
	void Update () {
        if (mesh == null || osps == null)
            Load();

        try
        {
            foreach (Meshable m in ((CompositMeshable)mb).components)
            {
                //foreach (Vector3 v in m.vertices)
                for (int i = 0; i < m.vertices.Length; i++)
                {
                    osps[i].pos = m.vertices[i];
                }
            }
        }
        catch { }
        
    }
}
