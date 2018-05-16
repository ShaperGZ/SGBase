using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SGCore;
using SGGeometry;
using System;

public class Building {

    public List<Grammar> grammars;
    public BuildingParamEditor buildingParamEditor;
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

    //units
    public List<List<Meshable>> units;
    public List<ShapeObject> unitSOS;
    

    public float ground = 0;
    public float gfa = -1;
    public float height = -1;
    public int floorCount = -1;
    public float footPrint = -1;
    public float width = -1;
    public float depth = -1;
    public float illumination = -1;
    public float efficiency = 0;
    public bool freeze = false;
    public Guid guid;
    Dictionary<string, float> subGfa;
    public ShapeObject positionReference;
    public GameObject coordRef;
    
    public enum DisplayMode
    {
        GRAPHICS,UNIT
    }
    public DisplayMode mode = DisplayMode.GRAPHICS;
    public DisplayMode lastMode = DisplayMode.GRAPHICS;

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
        UpdateForms();
        if (freeze) return;
        //Debug.Log(buildingParamEditor != null);
        if (buildingParamEditor != null)
        {
            UpdateParams();
        }
        
        switch (mode)
        {
            case DisplayMode.GRAPHICS:
                GraphicsMode();
                break;
            case DisplayMode.UNIT:
                UnitMode();
                break;
            default:
                break;
        }
        
        lastMode = mode;
    }
    public void UpdateForms()
    {
        MaterialManager mm = GameObject.Find("ScriptLoder").GetComponent<MaterialManager>();
        if (grammars.Count > 0 || grammars[0].stagedOutputs.Count > 0)
        {
            List<ShapeObject> shps = grammars[0].stagedOutputs[grammars[0].stagedOutputs.Count-1].shapes;
            foreach (ShapeObject so in shps)
            {
                if (so == null) continue;
                //Debug.LogFormat("{0}=={1}:{2}", so.name, "T", so.name == "T");
                if (so.name == "TOP")
                {
                    //Debug.Log("found top");
                    so.GetComponent<MeshRenderer>().material = mm.Grass0;
                    //so.Show(false);
                }


            }
        }
        
    }
    public void UpdateParams()
    {
        if (buildingParamEditor != null)
        {
            buildingParamEditor.UpdateBuildingParamDisplay();
        }
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
        //if(floors !=null)
        //    foreach(Floor f in floors)
        //    {
        //        f.SetActive(false);
        //    }
        //foreach(Grammar g in grammars)
        //{
        //    g.SetVisible(true);
        //}
    }

    public void UnitMode()
    {
        Debug.LogFormat("mode==unit:{0},lastMode==unit:{1}", this.mode == DisplayMode.UNIT, this.lastMode == DisplayMode.UNIT);
        if (this.lastMode != DisplayMode.UNIT)
        {
            Debug.Log("re execute " + grammars[0].name);
            this.lastMode = DisplayMode.UNIT;
            this.mode = DisplayMode.UNIT;
            grammars[0].Execute();
            Debug.Log("finish reExecute");
            return;
        }

        mode = DisplayMode.UNIT;
        foreach(ShapeObject o in grammars[0].stagedOutputs[grammars[0].stagedOutputs.Count - 1].shapes)
        {
            o.Show(false);
        }
        if (unitSOS == null) unitSOS = new List<ShapeObject>();
        //if (unitSOS == null || unitSOS.Count<1) return;
        if (units==null || units.Count < 1) return;
        int dif = unitSOS.Count- units[0].Count;
        if (dif > 0) SGUtility.RemoveExtraShapeObjects(ref unitSOS, dif);
        for (int i = 0; i < units[0].Count; i++)
        {
            if (i >= unitSOS.Count) unitSOS.Add(ShapeObject.CreateBasic());
            Meshable m = (Meshable)units[0][i].Clone();
            SGUtility.ScaleForm(m, new Vector3(0.9f, 0.7f, 1f), Alignment.Center3D);
            unitSOS[i].SetMeshable(m);
            unitSOS[i].Show(true);
        }

    }
    public void GraphicsMode()
    {
        mode = DisplayMode.GRAPHICS;
        foreach (ShapeObject o in grammars[0].stagedOutputs[grammars[0].stagedOutputs.Count - 1].shapes)
        {
            o.Show(true);
        }
        if (unitSOS != null)
        {
            foreach (ShapeObject o in unitSOS)
            {
                o.Show(false);
            }
        }
        
    }

}
