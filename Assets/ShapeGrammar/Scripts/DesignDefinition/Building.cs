using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SGCore;
using SGGeometry;
using System;

public class Building {

    public List<Grammar> grammars;
    public Vector3 _position;
    public Vector3 Position
    {
        set { _position = value; }
        get
        {
            if (positionReference != null) return positionReference.Position;
            return _position;
        }
    }
    public Transform transform
    {
        get { return coordRef.transform; }
    }
    public FloorHeight floorHeight;
    public List<Floor> floors;
    public Site site;

    public float ground = 0;
    public float gfa = -1;
    public float height = -1;
    public int floorCount = -1;
    public float footPrint = -1;
    public float width = -1;
    public float depth = -1;
    public float illumination = -1;
    public float efficiency = 0;
    public Guid guid;
    Dictionary<string, float> subGfa;
    public ShapeObject positionReference;
    public GameObject coordRef;
    public Building(Vector3? pos=null)
    {
        coordRef = new GameObject();
        guid = Guid.NewGuid();
        SceneManager.AddBuilding(this);
        ResetProperties();
        grammars = new List<Grammar>();
        if (pos.HasValue) Position = pos.Value;
        else pos = new Vector3(0, 0, 0);
    }
    public void SetRefPos(ShapeObject so)
    {
        positionReference = so;
    }
    public void ResetProperties()
    {
        ground = 0;
        gfa = -1;
        height = -1;
        floorCount = -1;
        footPrint = -1;
        width = -1;
        depth = -1;
        illumination = -1;
        efficiency = 0;
        subGfa=new Dictionary<string, float>();
    }
    public void Destroy()
    {
        throw new System.NotImplementedException();
    }
    public void Clear()
    {
        grammars.Clear();
        ResetProperties();
    }
    public void Invalidate(bool updateFromGrammar=false)
    {
        //if (!updateFromGrammar)
        //{
        //    foreach(Grammar g in grammars)
        //    {
        //        g.Execute();
        //    }
        //}
        foreach(Grammar g in grammars)
        {
            //g.SetVisible(false);
        }
        //After the gramms are executed, the following actions must take action:
        // 1. (Optional) group massings by program, this 
        // 2. divide and group massing by floors
        // 3. group faces by facade type (currently only 1 type)
        // 4. divide and group faces by floors
        
        //SetHeights();
        //CutShapeObjectsToFloors();
    }
    private void SetHeights()
    {
        floorHeight = new FloorHeight();
        if(floors==null)
            floors = new List<Floor>();
        if (height <= 0) throw new Exception("building height not set");
        if (height > 0)
        {
            float h = 6;
            int counter = 0;
            while (h <= height)
            {
                floorHeight.heights.Add(h);
                h += 3;
                counter += 1;
            }
        }
        if (floorHeight.Count > 0)
        {
            int dif = floors.Count - floorHeight.Count;
            if (dif > 0)
            {
                for (int i = 0; i < dif; i++)
                {
                    int index = floors.Count - 1;
                    floors[index].Destroy();
                    floors.RemoveAt(index);
                }
            }
            else
            {
                for (int i = 0; i < -dif; i++)
                {
                    floors.Add(new Floor());
                }
            }
            
        }
    }
    public void AddGrammar(Grammar g)
    {
        g.building = this;
        
        g.site = site;
        grammars.Add(g);
       
    }
    public void AddToSite(Site site)
    {
        this.site = site;
        foreach(Grammar g in grammars)
        {
            g.site = site;
        }
    }
    public string FormatProperties()
    {
        string txt = "";
        txt += "GFA    :" + gfa;
        txt += "\nheight :" + height;
        txt += "\nfloors :" + floorCount;
        txt += "\nfootpnt:" + footPrint;
        txt += "\nwidth  :" + width;
        txt += "\ndepth  :" + depth;
        txt += "\nillum  :" + illumination;
        txt += "\nefficiency:" + efficiency;
        return txt;
    }
    public void CutShapeObjectsToFloors()
    {
        List<ShapeObject> sos = new List<ShapeObject>();
        if (floorHeight==null) throw new System.Exception("floorHeight=null");
        if (floorHeight.Count < 1) throw new System.Exception("floor count<1");
        //Debug.Log("CutShapeObjectsToFloors");
        //Debug.Log(floorHeight.Count);
        //foreach (float h in floorHeight.heights) Debug.Log(h);
        for (int i = 0; i < floorHeight.Count; i++)
        {
            List<Meshable> mbs = new List<Meshable>();
            float h = floorHeight.heights[i];
            foreach (Grammar g in grammars)
            {
                foreach (ShapeObject o in g.stagedOutputs[g.stagedOutputs.Count - 1].shapes)
                {
                    Meshable remain;
                    Meshable mb = CutShapeObjectToFloors(o.meshable, h,out remain);
                    if(mb!=null) mbs.Add(mb);
                    if (remain == null) break;
                    mb = CutShapeObjectToFloors(remain,h, out remain);
                    if (mb != null) mbs.Add(mb);
                }
            }
            floors[i].SetMeshables(mbs);
        }
    }
    public Meshable CutShapeObjectToFloors(Meshable form, float h, out Meshable remain)
    {
        //add cuted meshables to floor and return the remaining meshable
        Vector3 bot = ((Extrusion)form).polygon.vertices[0];
        Plane? cutter=null;
        if (h>bot.y)
        {
            cutter = new Plane(Vector3.up,new Vector3(0,h,0));
        }
        
        if (cutter.HasValue)
        {
            Meshable[] splits = form.SplitByPlane(cutter.Value);
            remain = splits[1];
            if (splits[0]!=null)
            {
                return splits[0];
            }
        }
        remain = null;
        return null;
    }
    public void ShowGrammar()
    {
        if(floors !=null)
            foreach(Floor f in floors)
            {
                f.SetActive(false);
            }
        foreach(Grammar g in grammars)
        {
            g.SetVisible(true);
        }
    }
}
