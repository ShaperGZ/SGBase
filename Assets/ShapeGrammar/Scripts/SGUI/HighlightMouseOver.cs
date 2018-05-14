using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SGGeometry;
using SGCore;
using System;

public class HighlightMouseOver : MonoBehaviour {
    Color? orgColor;
    List<ShapeObject> commonNameObjects = new List<ShapeObject>();
    Dictionary<ShapeObject, Color> colors = new Dictionary<ShapeObject, Color>();
    Color selectColor;
    Color selectColor2;
    MeshRenderer meshRenderer;
    ShapeObject shapeObject;
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
        shapeObject = GetComponent<ShapeObject>();
    }
	
	// Update is called once per frame
	void Update () {
		
	}
    private void RecordColors()
    {

    }
    private void OnMouseEnter()
    {
        try
        {
            Debug.Log("isgraphics=" + shapeObject.isGraphics);
        }
        catch { }
        if (!shapeObject || shapeObject.isGraphics) return;
        //display GUID
        //guidText.text = "Mouse over:"+so.guid.ToString();
        guidText.text = shapeObject.sguid;

        if (shapeObject.parentRule == null) return;
        if (SceneManager.SelectedGrammar != shapeObject.parentRule.grammar) return;
        

        //colors = new Dictionary<ShapeObject, Color>();
        commonNameObjects.Clear();
        int step = SceneManager.SelectedGrammar.displayStep;
        if (step < 0) return;
        Color c= meshRenderer.material.color;
        orgColor = new Color(c.r,c.g,c.b,c.a);
        //Debug.Log("total step objs=" + SceneManager.SelectedGrammar.stagedOutputs[step].shapes.Count);
        foreach (ShapeObject s in SceneManager.SelectedGrammar.stagedOutputs[step].shapes)
        {
            try
            {
                if (shapeObject == s) continue;
                if (shapeObject.name == s.name)
                    commonNameObjects.Add(s);
                //Debug.LogFormat("name1={0}, name2={1} commonNameObjects:{2}", so.name+so.sguid, s.name+s.sguid, commonNameObjects.Count);
            }
            catch(Exception e) { Debug.Log("EXCEPTION:"+e.ToString()); }
        }
        SceneManager.selectedCommonNameShapes = commonNameObjects;
        foreach(ShapeObject s in commonNameObjects)
        {
            //Material m = so.GetComponent<MeshRenderer>().material;
            //colors[so] = new Color(m.color.r, m.color.g,m.color.b);
            try
            {
                s.GetComponent<MeshRenderer>().material = MaterialManager.GB.RuleSelectCommonName;
            }
            catch { }
            //Debug.LogFormat("seting obj mat:{0}", s.name + s.sguid);
        }

        meshRenderer.material = MaterialManager.GB.RuleSelect;
        SceneManager.ShapeInspectText.text = GetComponent<ShapeObject>().Format();
    }
    private void OnMouseExit()
    {
        if (!shapeObject || shapeObject.isGraphics) return;
        if (!orgColor.HasValue) return;
        if (shapeObject.parentRule == null) return;
        //if (SceneManager.SelectedGrammar == null) return;
        if (SceneManager.SelectedGrammar != shapeObject.parentRule.grammar) return;

        int step = SceneManager.SelectedGrammar.displayStep;
        foreach (ShapeObject s in SceneManager.SelectedGrammar.stagedOutputs[step].shapes)
        {
            //Material m = so.GetComponent<MeshRenderer>().material;
            //m.color = colors[so];
            if (!s || s.parentRule==null) continue;
            if (s.parentRule.grammar!=null && shapeObject.parentRule.grammar == SceneManager.SelectedGrammar)
                s.SetMaterial(MaterialManager.GB.RuleEditing);
            else
                s.SetMaterial(SceneManager.displayManager.currMode);
        }
        meshRenderer.material.color = orgColor.Value;
    }
    private void OnMouseDown()
    {

        SceneManager.SelectedShape = shapeObject;

        if(SceneManager.selectionMode == SelectionMode.GRAMMAR)
        {
            if (shapeObject.parentRule == null || shapeObject.parentRule.grammar == null)
            {
                //SceneManager.ruleNavigator.SetGrammar(null);
                SceneManager.SelectedGrammar = null;
                return;
            }

            SceneManager.SelectedGrammar = shapeObject.parentRule.grammar;
            SceneManager.displayManager.setRuleMode();
            commonNameObjects.Clear();
        }
       
    }
}
