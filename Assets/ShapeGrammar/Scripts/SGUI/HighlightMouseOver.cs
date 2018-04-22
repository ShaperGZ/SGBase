﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SGGeometry;
using SGCore;

public class HighlightMouseOver : MonoBehaviour {
    Color? orgColor;
    List<ShapeObject> commonNameObjects = new List<ShapeObject>();
    Dictionary<ShapeObject, Color> colors = new Dictionary<ShapeObject, Color>();
    Color selectColor;
    Color selectColor2;
    MeshRenderer meshRenderer;
    ShapeObject so;
    static Text _guidText;
    static Text guidText
    {
        get
        {
            if(_guidText == null)
            {
                _guidText = GameObject.Find("ShapeGUIDText").GetComponent<Text>();
            }
            return _guidText;
        }
    }

    

	// Use this for initialization
	void Start () {
        meshRenderer=GetComponent<MeshRenderer>();
        selectColor = SceneManager.colors["blue"];
        selectColor2 = SceneManager.colors["blue3"];
        so = GetComponent<ShapeObject>();
    }
	
	// Update is called once per frame
	void Update () {
		
	}
    private void RecordColors()
    {

    }
    private void OnMouseEnter()
    {
        
        if (!so) return;
        //display GUID
        //guidText.text = "Mouse over:"+so.guid.ToString();
        guidText.text = so.sguid;

        if (so.parentRule == null) return;
        if (SceneManager.SelectedGrammar != so.parentRule.grammar) return;
        

        colors = new Dictionary<ShapeObject, Color>();
        commonNameObjects.Clear();
        int step = SceneManager.SelectedGrammar.displayStep;
        if (step < 0) return;
        Color c= meshRenderer.material.color;
        orgColor = new Color(c.r,c.g,c.b);
        foreach (ShapeObject s in SceneManager.SelectedGrammar.stagedOutputs[step].shapes)
        {
            try
            {
                if (so == s) continue;
                if (so.name == s.name)
                    commonNameObjects.Add(so);
            }
            catch { }
        }
        foreach(ShapeObject s in commonNameObjects)
        { 
            //Material m = so.GetComponent<MeshRenderer>().material;
            //colors[so] = new Color(m.color.r, m.color.g,m.color.b);
            s.GetComponent<MeshRenderer>().material = MaterialManager.GB.RuleSelectCommonName;
        }

        meshRenderer.material = MaterialManager.GB.RuleSelect;
        //orgColor = meshRenderer.material.color;
        //meshRenderer.material.color = selectColor;
        SceneManager.ShapeInspectText.text = GetComponent<ShapeObject>().Format();
    }
    private void OnMouseExit()
    {
        if (!so) return;
        if (!orgColor.HasValue) return;
        if (so.parentRule == null) return;
        if (SceneManager.SelectedGrammar != so.parentRule.grammar) return;


        foreach (ShapeObject so in commonNameObjects)
        {
            //Material m = so.GetComponent<MeshRenderer>().material;
            //m.color = colors[so];
            if (so.parentRule.grammar == SceneManager.SelectedGrammar)
                so.SetMaterial(MaterialManager.GB.RuleEditing);
            else
                so.SetMaterial(SceneManager.displayManager.currMode);
        }
        meshRenderer.material.color = orgColor.Value;
    }
    private void OnMouseDown()
    {
        SceneManager.SelectedShape = so;

        if (so.parentRule == null || so.parentRule.grammar == null)
        {
            //SceneManager.ruleNavigator.SetGrammar(null);
            SceneManager.SelectedGrammar = null;
            return;
        }

        SceneManager.SelectedGrammar = so.parentRule.grammar;
        SceneManager.displayManager.setRuleMode();
        commonNameObjects.Clear();

    }
}
