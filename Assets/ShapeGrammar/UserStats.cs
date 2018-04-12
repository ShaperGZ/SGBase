using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SGCore;
using SGGUI;
using System;

public class UserStats : MonoBehaviour {

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
    }
    public static void unAssignGrammar()
    {
        ruleCreator.SetGrammar(null);
        ruleNavigator.SetGrammar(null);
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
    
    public static RuleNavigator _ruleNavigator;
    public static RuleNavigator ruleNavigator
    {
        get
        {
            if (_ruleNavigator == null)
                _ruleNavigator = GameObject.Find("ExpandableListPanel").GetComponent<RuleNavigator>();
            return _ruleNavigator;
        }
    }
    public static Text ShapeInspectText;

    public static Dictionary<string,Color> colors = new Dictionary<string, Color>();

    //materials
    Material selectMat;

    private void Awake()
    {
        AssociateGUI();
        GetColors();
        ruleCreator = new RuleCreator();
    }

    // Use this for initialization
    void Start () {
        
        
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            if (SelectedGrammar.grammar != null)
            {
                SelectedGrammar = SelectedGrammar.grammar;
            }
        }
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
