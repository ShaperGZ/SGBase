using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SGCore;
using SGGUI;
using System;

public class SelectionMode
{
    public const int GRAMMAR = 111;
    public const int BUILDING = 222;
}

public class SceneManager : MonoBehaviour {

    public static Tool _tool=new Tool();
    public static Tool tool
    {
        get
        {
            return _tool;
        }
        set
        {
            _tool = value;
        }
    }

    public static string directoryRules;
    public static int selectionMode=SelectionMode.BUILDING;
    public void SetSelectModeBuilding()
    {
        selectionMode = SelectionMode.BUILDING;
        GameObject.Find("TextSelectionMode").GetComponent<Text>().text = "Building Mode";
    }
    public void SetSelectModeGrammar()
    {
        selectionMode = SelectionMode.GRAMMAR;
        GameObject.Find("TextSelectionMode").GetComponent<Text>().text = "Grammar Mode";
        foreach(Guid id in existingBuildings.Keys)
        {
            existingBuildings[id].ShowGrammar();
        }
    }

    static Text _selectedShapeText;
    static Text selectedShapeText
    {
        get
        {
            if (_selectedShapeText == null)
            {
                _selectedShapeText = GameObject.Find("shapeSelectedText").GetComponent<Text>();
            }
            return _selectedShapeText;
        }
    }
    private static ShapeObject _selectedShape;
    public static ShapeObject SelectedShape
    {
        get { return _selectedShape; }
        set
        {
            SetSelectedShape(value);
        }
    }

    private static void SetSelectedShape(ShapeObject so)
    {
        _selectedShape = so;
        selectedShapeText.text = so.sguid;
    }
    public static List<ShapeObject> selectedCommonNameShapes;

    private static Grammar _selectedGrammar;
    public static Grammar SelectedGrammar
    {
        get { return _selectedGrammar; }
        set
        {
            if (value != null)
                assignGrammar(value);
            else
                unAssignGrammar();
        }
    }
    public static void assignGrammar(Grammar g)
    {
        _selectedGrammar = g;
        if (ruleCreator != null)
        {
            ruleCreator.SetGrammar(g);
            ruleNavigator.SetGrammar(g);
        }

        for (int i = 0; i < g.stagedOutputs.Count; i++)
        {
            for (int j = 0; j < g.stagedOutputs[i].shapes.Count; j++)
            {
                ShapeObject so = g.stagedOutputs[i].shapes[j];
                if(!so.isGraphics)
                    so.SetMaterial(MaterialManager.GB.RuleEditing);
            }
        }
    }
    public static void unAssignGrammar()
    {
        if (_selectedGrammar != null)
        {
            Grammar g = _selectedGrammar;
            for (int i = 0; i < g.stagedOutputs.Count; i++)
            {
                for (int j = 0; j < g.stagedOutputs[i].shapes.Count; j++)
                {
                    ShapeObject so = g.stagedOutputs[i].shapes[j];
                    so.SetMaterial(SceneManager.displayManager.currMode);
                }
            }
        }
        
        ruleCreator.SetGrammar(null);
        ruleNavigator.SetGrammar(null);
    }

    public static Dictionary<Guid, Building> existingBuildings = new Dictionary<Guid, Building>();
    public static Dictionary<Guid, Site> existingSites = new Dictionary<Guid, Site>();
    public static Dictionary<Guid, ShapeObject> existingShapes = new Dictionary<Guid, ShapeObject>();
    public static Dictionary<Guid, Grammar> existingGrammar = new Dictionary<Guid, Grammar>();
    public static void CreateShape(ShapeObject so)
    {
        updateSystemInspectText();
        existingShapes.Add(so.guid, so);
    }
    public static void DestroyShape(Guid id)
    {
        updateSystemInspectText();
        existingShapes.Remove(id);
    }
    public static void AddBuilding(Building building)
    {
        existingBuildings[building.guid] = building;
    }
    public static void RemoveBuilding(Guid id)
    {
        throw new System.NotImplementedException();
        //remove all generated objects from building grammars
        //remove all grammars
        existingBuildings.Remove(id);
    }
    public static void AddGrammar(Grammar g)
    {
        updateSystemInspectText();
        existingGrammar.Add(g.guid, g);
    }
    public static void DestroyGrammar(Guid id)
    {
        updateSystemInspectText();
        existingGrammar.Remove(id);
    }
    public static void updateSystemInspectText()
    {
        string txt = "";
        txt = string.Format("Shapes({0}) Grammars({1})",
            existingShapes.Values.Count,
            existingGrammar.Values.Count
            );
        if(systemInspectText!=null)
            systemInspectText.text = txt;
    }


    public static ShapeObject selectedShape
    {
        get
        {
            return _selectedShape;
        }
        set
        {
            _selectedShape = value;
            //TODO:assign all selected shape association here
        }
    }
    public static Material lineMat;
    public static Material LineMat
    {
        get
        {
            if (lineMat == null)
                lineMat=SGGeometry.GLRender.GetLineMaterial();
                //lineMat = Resources.Load("LineMaterial") as Material;
            return lineMat;
        }
    }
    public static RuleCreator ruleCreator;
    public static Navigator _ruleNavigator;
    public static Navigator ruleNavigator
    {
        get
        {
            if (_ruleNavigator == null)
                _ruleNavigator = GameObject.Find("CanvasMain").GetComponent<Navigator>();
            return _ruleNavigator;
        }
    }
    public static Text ShapeInspectText;
    public static Text _systemInspectText;
    public static Text systemInspectText
    {
        get
        {
            try
            {
                if (_systemInspectText == null)
                    _systemInspectText = GameObject.Find("SystemInspectText").GetComponent<Text>();
            }
            catch { }
            
            return _systemInspectText;
        }
    }

    public static DisplayManager _displayManager;
    public static DisplayManager displayManager
    {
        get
        {
            if (_displayManager == null)
                _displayManager = new DisplayManager();
            return _displayManager;
        }
    }
    

    public static Dictionary<string,Color> colors = new Dictionary<string, Color>();

    public string direcotyRules;

    //materials
    Material selectMat;

    private void Awake()
    {
        AssociateGUI();
        GetColors();
        ruleCreator = new RuleCreator();
        SceneManager.directoryRules = this.direcotyRules;
    }

    // Use this for initialization
    void Start () {
        tool = new Tool();
        
	}
	
	// Update is called once per frame
	void Update () {
        tool.Update();
        
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            if (SelectedGrammar.grammar != null)
            {
                SelectedGrammar = SelectedGrammar.grammar;
            }
        }
	}

    private void OnRenderObject()
    {
        //tool.OnRenderObject();
    }

    void AssociateGUI()
    {
        ShapeInspectText = GameObject.Find("ShapeInspectText").GetComponent<Text>();
    }

    void GetColors()
    {
        colors["red"] = new Color(0.95f, 0.32f, 0.36f);
        colors["yellow"] = new Color(0.99f, 0.81f, 0.27f);
        colors["green"] = new Color(0.58f, 0.70f, 0.23f);
        colors["blue"] = new Color(0.14f, 0.65f, 0.90f);
        string[] names = new string[] { "red", "yellow", "green", "blue" };

        float incrm = 0.2f;
        for (int i = 1; i < 4; i++)
        {
            foreach (string n in names)
            {
                string newName = n + i.ToString();
                Color c = colors[n];
                float r = Mathf.Clamp01(c.r * (1 + incrm * i));
                float g = Mathf.Clamp01(c.g * (1 + incrm * i));
                float b = Mathf.Clamp01(c.b * (1 + incrm * i));
                colors[newName] = new Color(r, g, b);
            }
        }
    }


}
