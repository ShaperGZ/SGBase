using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SGCore;
using SGGeometry;

public class Building {

    public List<Grammar> grammars;
    public Vector3 position;

    public float ground = 0;
    public float gfa = -1;
    public float height = -1;
    public int floors = -1;
    public float floorHeight = 3;
    public float footPrint = -1;
    public float width = -1;
    public float depth = -1;
    public float illumination = -1;
    public float efficiency = 0;
    Dictionary<string, float> subGfa;

    public Building(Vector3? pos=null)
    {
        ResetProperties();
        grammars = new List<Grammar>();
        if (pos.HasValue) position = pos.Value;
        else pos = new Vector3(0, 0, 0);
    }
    public void ResetProperties()
    {
        ground = 0;
        gfa = -1;
        height = -1;
        floors = -1;
        floorHeight = 3;
        footPrint = -1;
        width = -1;
        depth = -1;
        illumination = -1;
        efficiency = 0;
        subGfa=new Dictionary<string, float>();
    }
    public void Clear()
    {
        grammars.Clear();
        ResetProperties();
    }
}
