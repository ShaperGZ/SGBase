using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SGGeometry;
using SGCore;

public class HighlightMouseOver : MonoBehaviour {
    Color orgColor;
    List<ShapeObject> commonNameObjects = new List<ShapeObject>();
    Dictionary<ShapeObject, Color> colors = new Dictionary<ShapeObject, Color>();
    Color selectColor;
    Color selectColor2;
    MeshRenderer meshRenderer;

	// Use this for initialization
	void Start () {
        meshRenderer=GetComponent<MeshRenderer>();
        selectColor = UserStats.colors["blue"];
        selectColor2 = UserStats.colors["blue3"];
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnMouseEnter()
    {
        
        ShapeObject mso = GetComponent<ShapeObject>();
        if (!mso) return;
        colors = new Dictionary<ShapeObject, Color>();
        commonNameObjects.Clear();
        int step = UserStats.SelectedGrammar.displayStep;
        if (step < 0) return;
        Color c= meshRenderer.material.color;
        orgColor = new Color(c.r,c.g,c.b);
        foreach (ShapeObject so in UserStats.SelectedGrammar.stagedOutputs[step].shapes)
        {
            try
            {
                if (so == mso) continue;
                if (so.name == mso.name)
                    commonNameObjects.Add(so);
            }
            catch { }
        }
        foreach(ShapeObject so in commonNameObjects)
        { 
            Material m = so.GetComponent<MeshRenderer>().material;
            colors[so] = new Color(m.color.r, m.color.g,m.color.b);
            m.color = selectColor2;
        }
       
        
        orgColor = meshRenderer.material.color;
        meshRenderer.material.color = selectColor;
        UserStats.ShapeInspectText.text = GetComponent<ShapeObject>().Format();
    }
    private void OnMouseExit()
    {
        ShapeObject mso = GetComponent<ShapeObject>();
        if (!mso) return;
        foreach (ShapeObject so in commonNameObjects)
        {
            Material m = so.GetComponent<MeshRenderer>().material;
            m.color = colors[so];
        }
        meshRenderer.material.color = orgColor;
    }
}
