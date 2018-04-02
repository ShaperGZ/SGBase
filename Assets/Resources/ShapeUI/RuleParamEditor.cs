using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SGCore;
using SGGeometry;

public class RuleParamEditor : MonoBehaviour {
    public float width=30;
    public float height = 20;
    public RectTransform ParameterGroupPrefab;
    public InputField inputFieldPrefab;
    public Slider sliderPrefab;
    public List<InputField> inputFields;
    public List<RectTransform> ParameterGroups;
    public List<Slider> sliders;

	// Use this for initialization
	void Start () {
        inputFields=new List<InputField>();
        ParameterGroups = new List<RectTransform>();
        //ParameterGroupPrefab = (Resources.Load("ParameterGroup") as GameObject).transform as RectTransform;
        //inputFieldPrefab = (Resources.Load("InputField") as GameObject).GetComponent<InputField>();
         
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    void Clear()
    {
        for(int i = 0; i < inputFields.Count; i++)
        {
            GameObject.Destroy(inputFields[i].gameObject);
        }
        for (int i = 0; i < ParameterGroups.Count; i++)
        {
            GameObject.Destroy(ParameterGroups[i].gameObject);
        }
        for (int i = 0; i < sliders.Count; i++)
        {
            GameObject.Destroy(sliders[i].gameObject);
        }
        inputFields.Clear();
        ParameterGroups.Clear();
        sliders.Clear();
    }
    public void GenerateUI(Rule r)
    {
        Clear();
        for (int i = 0; i < r.paramGroups.Count; i++)
        {
            ParameterGroup pg = r.paramGroups[i];
            //the actual UI elements are added here
            AddParameterGroup(r, pg,i);
        }
    }

    public void AddParameterGroup(Rule r, ParameterGroup pg, int index)
    {
        RectTransform pgui = Instantiate(ParameterGroupPrefab,transform).transform as RectTransform;
        pgui.anchoredPosition = new Vector2(0, index * 50);
        Vector3 size = pgui.sizeDelta;
        size.y = pg.parameters.Count * 25 + 25;
        pgui.sizeDelta = size;
        pgui.transform.Find("Title").GetComponent<Text>().text = pg.name;
        ParameterGroups.Add(pgui);

        for(int i = 0; i < pg.parameters.Count; i++)
        {
            Parameter p = pg.parameters[i];
            float value = p.value;
            GameObject o = new GameObject();
            InputField ipf = Instantiate(inputFieldPrefab, pgui);
            RectTransform ipftrans=ipf.transform as RectTransform;
            ipftrans.anchoredPosition = new Vector2(30, -25 * i -25);
            ipf.text = p.value.ToString();
            Slider sld = Instantiate(sliderPrefab, pgui);
            sld.value = p.value;
            sld.minValue = p.min;
            sld.maxValue = p.max;
            if (p.step == 1) sld.wholeNumbers = true;

            sld.onValueChanged.AddListener(delegate { OnSliderValueChanged(sld, ipf, p ,r); });
            RectTransform sldtrans = sld.transform as RectTransform;
            sldtrans.anchoredPosition = new Vector2(60, -25 * i -10);

            inputFields.Add(ipf);
            sliders.Add(sld);

           
        }
    }

    void OnSliderValueChanged(Slider sld, InputField ipf, Parameter p, Rule r)
    {
        ipf.text = sld.value.ToString();
        p.value = sld.value;
        int index = r.grammar.rules.IndexOf(r);
        r.grammar.ExecuteFrom(index); 
    }
}
