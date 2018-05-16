using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SGCore;
using SGGeometry;
using System;

public class SGBuilding
{

    public List<Grammar> grammars;

    public Grammar gPlaning;
    public Grammar gMassing;
    public Grammar gProgram;
    public Grammar gStruct;
    public Grammar gFacade;
    public Grammar gCalculate;


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
        GRAPHICS, PROGRAM, STRUCTURE
    }
    public DisplayMode mode = DisplayMode.GRAPHICS;
    public DisplayMode lastMode = DisplayMode.GRAPHICS;

    public SGBuilding(Vector3? pos = null)
    {
        coordRef = new GameObject();
        guid = Guid.NewGuid();
        //SceneManager.AddBuilding(this);
        ResetProperties();
        grammars = new List<Grammar>();
        if (pos.HasValue) Position = pos.Value;
        else pos = new Vector3(0, 0, 0);
    }
    public void SetRefPos(ShapeObject so)
    {
        positionReference = so;
    }

    public void Execute()
    {
        if (gPlaning != null)
        {
            gPlaning.Execute();
        }
        //if (gPlaning != null)
        //{
        //    gPlaning.Execute();
        //}
        //if(gMassing != null)
        //{
        //    gMassing.inputs.shapes = gPlaning.outputs.shapes;
        //    gMassing.Execute();
        //}
        //if(gProgram !=null)
        //{
        //    gProgram.inputs.shapes = gMassing.outputs.shapes;
        //    gProgram.Execute();
        //}
        //if(gFacade!=null)
        //{
        //    gFacade.inputs.shapes = gMassing.outputs.shapes;
        //    gFacade.Execute();
        //}

    }
    private void SetGrammar(ref Grammar oldg, Grammar newg, GraphNode upstream=null)
    {
        if (oldg != null)
        {
            foreach (GraphNode gn in oldg.downStreams)
            {
                gn.ReplaceUpstream(oldg, newg);

            }
            oldg.Clear();
            oldg = newg;
        }
        else
        {
            oldg = newg;
            if (upstream != null)
            {
                upstream.ConnectDownStream(oldg);
            }
        }
            
    }
    public void SetGPlanning(Grammar g)
    {
        g.sgbuilding = this;
        SetGrammar(ref this.gPlaning, g);
    }
    public void SetMassing(Grammar g)
    {
        g.sgbuilding = this;
        SetGrammar(ref this.gMassing, g, gPlaning);
    }
    public void SetProgram2(Grammar g)
    {
        g.sgbuilding = this;

        if (mode == DisplayMode.PROGRAM)
            SetGrammar(ref this.gProgram, g, gMassing);
        else
            gProgram = g;
    }
    public void SetProgram(Grammar g)
    {
        g.sgbuilding = this;
        SetGrammar(ref this.gProgram, g, gMassing);
    }
    public void SetFacade(Grammar g)
    {
        g.sgbuilding = this;
        SetGrammar(ref this.gFacade, g, gMassing);
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
        subGfa = new Dictionary<string, float>();
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
    public void Invalidate(bool updateFromGrammar = false)
    {
        //UpdateForms();
        //if (freeze) return;
        ////Debug.Log(buildingParamEditor != null);
        //if (buildingParamEditor != null)
        //{
        //    UpdateParams();
        //}

        //switch (mode)
        //{
        //    case DisplayMode.GRAPHICS:
        //        GraphicsMode();
        //        break;
        //    case DisplayMode.PROGRAM:
        //        UnitMode();
        //        break;
        //    default:
        //        break;
        //}

        //lastMode = mode;
    }
    public void UpdateForms()
    {
        //MaterialManager mm = GameObject.Find("ScriptLoder").GetComponent<MaterialManager>();
        //if (grammars.Count > 0 || grammars[0].stagedOutputs.Count > 0)
        //{
        //    List<ShapeObject> shps = grammars[0].stagedOutputs[grammars[0].stagedOutputs.Count - 1].shapes;
        //    foreach (ShapeObject so in shps)
        //    {
        //        if (so == null) continue;
        //        //Debug.LogFormat("{0}=={1}:{2}", so.name, "T", so.name == "T");
        //        if (so.name == "TOP")
        //        {
        //            //Debug.Log("found top");
        //            so.GetComponent<MeshRenderer>().material = mm.Grass0;
        //            //so.Show(false);
        //        }


        //    }
        //}

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
        if (floors == null)
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
    public void AddToSite(Site site)
    {
        this.site = site;
        foreach (Grammar g in grammars)
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

    public void ProgramMode()
    {
        Debug.Log("ProgramMode");
        mode = DisplayMode.PROGRAM;
        if (gMassing != null)
        {
            if(gFacade!=null)
            {
                gFacade.SelectStep(-1);
                gMassing.DisconnectDownStream(gFacade);
            }
            if(gProgram!=null)
            {
                gMassing.ConnectDownStream(gProgram);
            }
            Execute();
        }
    }
    public void GraphicsMode()
    {
        Debug.Log("GraphicsMode");
        mode = DisplayMode.GRAPHICS;
        if (gMassing != null)
        {
            if (gProgram != null)
            {
                Debug.Log("gProgram !=null");
                //gProgram.Clear();
                //TODO:somthing is wrong with Clear()
                gProgram.SelectStep(-1);
                gMassing.DisconnectDownStream(gProgram);
            }
            if (gFacade != null)
            {
                gMassing.ConnectDownStream(gFacade);
            }

            gMassing.Execute();

        }

    }

}
