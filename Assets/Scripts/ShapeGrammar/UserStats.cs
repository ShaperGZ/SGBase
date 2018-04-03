﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SGCore;

public class UserStats : MonoBehaviour {


    
    private static Grammar _selectedGrammar;

    public static Material lineMat;
    public static Material LineMat
    {
        get
        {
            if (lineMat == null)
                lineMat = Resources.Load("LineMaterial") as Material;
            return lineMat;
        }
    }
    public static RuleCreator ruleCreator;
    public static Grammar SelectedGrammar
    {
        get { return _selectedGrammar; }
        set
        {
            _selectedGrammar = value;
            if (ruleCreator != null) ruleCreator.grammar = _selectedGrammar;
        }
    }
    public static RuleNavigator ruleNavigator;
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
