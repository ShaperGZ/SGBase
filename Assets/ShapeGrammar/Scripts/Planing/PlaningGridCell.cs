using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SGGeometry;
using SGCore;


public class PlaningGridCell:MonoBehaviour{
    public Properties properties;
    public Vector3 Position
    {
        get
        {
            return gameObject.transform.position;
        }
        set
        {
            gameObject.transform.position = value;
        }
    }
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    public void SetProp(string key, float value)
    {
        if (properties != null)
        {
            properties.properties[key] = value;
        }
    }
    public float GetProp(string key)
    {
        if (properties != null && properties.properties.ContainsKey(key))
            return (float)properties.properties[key];
        return float.NaN;
    }
    public void ResetProperties()
    {
        initProperties();
    }
    void initProperties()
    {
        if(properties == null)
        {
            properties = new Properties();
        }
        SetProp("density", 0);
        SetProp("environment", 0);
    }
    public void SetPlanarSize(float h)
    {
        Vector3 size = transform.localScale;
        size.x = h;
        size.z = h;
        transform.localScale = size;
    }
    public void SetHeight(float h)
    {
        Vector3 size = transform.localScale;
        size.y = h;
        transform.localScale = size;
    }
    public static PlaningGridCell CreateCell()
    {
        return PlaningGridCell.CreateCell(Vector3.zero, 2);

    }
    public static PlaningGridCell CreateCell(Vector3 pos, float size=2)
    {
        Vector3 scale = new Vector3(size, size, size);
        GameObject o = GameObject.CreatePrimitive(PrimitiveType.Cube);
        o.transform.position = pos;
        o.transform.localScale = scale;
        PlaningGridCell pgc= o.AddComponent<PlaningGridCell>() as PlaningGridCell;
        return pgc;
    }
    public void SetColor(Color c)
    {
        GetComponent<MeshRenderer>().material.color = c;
    }
}
