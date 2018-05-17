﻿using System.Collections;
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
        PLANNING,MASSING,GRAPHICS, PROGRAM, STRUCTURE
    }
    public enum UpdateMode
    {
        AUTOMATIC,MANUAL
    }
    public UpdateMode updateMode = UpdateMode.MANUAL;
    public UpdateMode lastUpdateMode = UpdateMode.MANUAL;
    public DisplayMode displayMode = DisplayMode.GRAPHICS;
    public DisplayMode lastDisplayMode = DisplayMode.GRAPHICS;

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

        if (displayMode == DisplayMode.PROGRAM)
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
        //if (updateMode==UpdateMode.AUTOMATIC)
        //{
        //    UpdateParams();
        //}
    }
    public void UpdateParams(bool updateParamDisplay=true)
    {
        if (buildingParamEditor != null)
        {
            GraphNode blockRule = gPlaning.FindFirst("SizeBuilding3D");
            this.width = blockRule.GetParamVal("Size", 0);
            this.height = blockRule.GetParamVal("Size", 1);
            this.depth = blockRule.GetParamVal("Size", 2);
            this.floorCount = Mathf.RoundToInt(this.height / 4);
            //get gfa
            List<ShapeObject> shapes = gPlaning.outputs.shapes;
            float gfa = 0;
            float apt = 0;
            float sv = 0;
            float maxdepth = 0;
            foreach(ShapeObject so in shapes)
            {
                if(so.meshable.GetType() == typeof(Extrusion))
                {
                    Extrusion ext = (Extrusion)so.meshable;
                    float area = ext.polygon.Area();
                    float barea = area * Mathf.Round(ext.height / 4);
                    if (so.name == "APT") apt += barea;
                    if (so.name == "STA" || so.name == "CD") sv += barea;
                    gfa += barea;

                    float d = ext.bbox.size[2];
                    if (d > maxdepth) maxdepth = d;
                }
            }
            this.gfa = gfa;
            this.efficiency = apt / gfa;
            this.illumination = Mathf.Clamp01(1 - ((maxdepth - 5) / 10)) ;
            if(updateParamDisplay)
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
    public void PlanningMode()
    {
        Debug.Log("MissingMode");
        displayMode = DisplayMode.MASSING;
        if (gPlaning != null)
        {
            if (gFacade != null)
            {
                gFacade.SelectStep(-1);
            }
            if (gProgram != null)
            {
                gProgram.SelectStep(-1);
            }
            if (gMassing != null)
            {
                gMassing.SelectStep(-1);
                gPlaning.DisconnectDownStream(gMassing);
            }
            Execute();
        }
    }
    public void MassingMode()
    {
        Debug.Log("MissingMode");
        displayMode = DisplayMode.MASSING;
        if (gPlaning != null)
        {
            if (gFacade != null)
            {
                gFacade.SelectStep(-1);
                gMassing.DisconnectDownStream(gFacade);
            }
            if (gProgram != null)
            {
                gProgram.SelectStep(-1);
                gMassing.DisconnectDownStream(gProgram);
            }
            if (gMassing != null)
            {
                gPlaning.ConnectDownStream(gMassing);
            }
            Execute();
        }
    }
    public void ProgramMode()
    {
        Debug.Log("ProgramMode");
        displayMode = DisplayMode.PROGRAM;
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
        //Debug.Log("GraphicsMode");
        displayMode = DisplayMode.GRAPHICS;
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
