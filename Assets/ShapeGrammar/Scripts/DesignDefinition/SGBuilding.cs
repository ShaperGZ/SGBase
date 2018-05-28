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
    public Transform transform
    {
        get { return coordRef.transform; }
    }
    public FloorHeight floorHeight;
    public List<Floor> floors;
    public DesignContext site;

    public Grammar[] GetGrammars()
    {
        Grammar[] grammars = new Grammar[]
        {
            gPlaning,
            gMassing,
            gProgram,
            gFacade,
            gStruct,
            gCalculate
        };

        return grammars;

    }

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

    public SGBuilding()
    {
        coordRef = new GameObject();
        guid = Guid.NewGuid();
        //SceneManager.AddBuilding(this);
        ResetProperties();
        grammars = new List<Grammar>();

    }
    public static SGBuilding CreateApt(SOPoint sop, Vector3 size)
    {
        SGBuilding building = new SGBuilding();
        Grammar gp = new Grammar();
        gp.name = "planing1";
        gp.inputs.shapes.Add(sop);
        gp.AddRule(new Rules.CreateOperableBox("A", new Vector3(30, 30, 20)));
        gp.AddRule(new Rules.SizeBuilding3D("A", "A", new Vector3(30, 20, 15)));
        gp.AddRule(new Rules.ResidentialLoadFilter("A", "IHUS", "DHUS", "A"));
        gp.AddRule(new Rules.ApartmentLoadFilter("A", "SL", "DL", "CV"), false);
        gp.AddRule(new Rules.SingleLoaded("SL", "APT"), false);
        gp.AddRule(new Rules.DoubleLoaded("DL", "APT"), false);
        gp.AddRule(new Rules.CentralVoid("CV", "APT", "APT2"), false);
        building.SetGPlanning(gp);
        building.SetMassing(MassingGrammars.GA());
        //building.SetFacade(FacadeGrammars.CW01());
        building.SetSize(size);
        sop.grammar = gp;
        //sop.onPositionChanged += delegate { building.Execute(); };
        //sop.onDestroy += delegate { building.ClearForDestroy(); };
        return building;
    }

    public void Execute()
    {
        if (gPlaning != null)
        {
            gPlaning.Execute();
        }
    }

    public void SetGrammar(Grammar g,bool execute=false)
    {
        switch (g.category)
        {
            case GraphNode.Category.Bd_Planing:
                SetGPlanning(g,execute);
                break;
            case GraphNode.Category.Bd_Massing:
                SetMassing(g, execute);
                break;
            case GraphNode.Category.Bd_Program:
                SetProgram(g, execute);
                break;
            case GraphNode.Category.Bd_Struct:
               
                break;
            case GraphNode.Category.Bd_Graphics:
                SetFacade(g, execute);
                break;
            default:
                break;
        }
        

    }
    private void SetGrammar(ref Grammar oldg, Grammar newg, GraphNode upstream=null)
    {
        if (oldg != null)
        {
            if (upstream != null)
            {
                upstream.DisconnectDownStream(oldg);
                //Debug.Log("disconnect:" + oldg.guid);
            }
            for (int i = 0; i < oldg.downStreams.Count; i++)
            {
                GraphNode gn = oldg.downStreams[i];
                gn.ReplaceUpstream(oldg, newg);
            }
            
            oldg.Clear();
            oldg = newg;
        }
        else
        {
            oldg = newg;
        }
        if (upstream != null)
        {
            upstream.ConnectDownStream(oldg);
        }

    }
    public void SetGPlanning(Grammar g, bool execute = false)
    {
        g.sgbuilding = this;
        SetGrammar(ref this.gPlaning, g);
        if (execute) Execute();
    }
    public void SetMassing(Grammar g, bool execute = false)
    {
        g.sgbuilding = this;
        SetGrammar(ref this.gMassing, g, gPlaning);
        if (execute) Execute();
    }
    public void SetProgram(Grammar g, bool execute = false)
    {
        g.sgbuilding = this;
        SetGrammar(ref this.gProgram, g, gMassing);
        if (execute) Execute();
    }
    public void SetFacade(Grammar g, bool execute = false)
    {
        g.sgbuilding = this;
        SetGrammar(ref this.gFacade, g, gMassing);
        if (execute) Execute();
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
        if (gPlaning != null) gPlaning.Clear();
        if (gMassing != null) gMassing.Clear();
        if (gProgram != null) gProgram.Clear();
        if (gStruct != null) gStruct.Clear();
    }
    public void ClearAllAssociated()
    {
        Grammar[] grammars = GetGrammars();
        foreach (Grammar g in grammars)
        {
            if (g != null)
                g.ClearAllAssociated();
        }

        ResetProperties();
    }
    public void ClearForDestroy()
    {
        //grammars.Clear(); 
        Grammar[] grammars = GetGrammars();
        foreach(Grammar g in grammars)
        {
            if(g!=null)
                g.ClearForDestroy();
        }

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
                this.UpdateParamDisplay();
        }
    }
    public void UpdateParamDisplay()
    {
        if (buildingParamEditor!=null)
            buildingParamEditor.UpdateBuildingParamDisplay();
    }
    public void SetSize(Vector3 size)
    {
        if (gPlaning != null)
        {
            GraphNode g=gPlaning.FindFirst("SizeBuilding3D");
            if (g == null) return;
            Debug.Log("setSize:"+size[1]);
            g.SetParam("Size", 0, size[0]);
            g.SetParam("Size", 1, size[1]);
            g.SetParam("Size", 2, size[2]);
           
            g.Execute();
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
    public void AddToSite(DesignContext site)
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
