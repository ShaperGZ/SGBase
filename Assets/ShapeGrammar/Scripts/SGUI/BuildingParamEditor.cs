using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SGCore;

public class BuildingParamEditor : MonoBehaviour {

    Transform[] trans = new Transform[8];
    Building building;
    Grammar grammar;
	// Use this for initialization
	void Start () {
        for (int i = 0; i < 8; i++)
        {
            string name = string.Format("panelgroup ({0})", i);
            trans[i] = transform.Find(name);
            Slider sld = trans[i].Find("Slider").GetComponent<Slider>();
            sld.onValueChanged.AddListener(delegate { InputUpdate(i); });
        }
	}
	public void SetBuilding(Building b)
    {
        building = b;
        grammar = b.grammars[0];

    }
    public void UpdateDisplay(int index, string txt, float value, float? min=null, float? max=null, float? step = null)
    {
        Text text = trans[index].Find("Text").GetComponent<Text>();
        Slider sld = trans[index].Find("Slider").GetComponent<Slider>();

        text.text = txt;
        sld.value = value;
        if (min.HasValue) sld.minValue = min.Value;
        if (max.HasValue) sld.maxValue = max.Value;
    }

    public void InputUpdate(int index)
    {
        Text text = trans[index].Find("Text").GetComponent<Text>();
        Slider sld = trans[index].Find("Slider").GetComponent<Slider>();
    }

    public void ChangeHeight(float f)
    {
        GraphNode gn = grammar.FindFisrt("SizeBuilding3D");
        gn.SetParam("Size", 1, f);
    }


	// Update is called once per frame
	void Update () {
		
	}
}
