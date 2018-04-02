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
    public RuleNavigator ruleNavigator;
     
    float h = 25;

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
        InputField ipfNameIn = Instantiate(inputFieldPrefab, transform);
        ipfNameIn.text = JoinNames(r.inputs.names);
        ipfNameIn.onValueChanged.AddListener(delegate { OnNamesChanged(ipfNameIn, r, true); });
        Vector2 size = ((RectTransform)ipfNameIn.transform).sizeDelta;
        size.x = 100;
        ((RectTransform)ipfNameIn.transform).sizeDelta = size;
        ((RectTransform)ipfNameIn.transform).anchoredPosition = new Vector2(0, h*2);

        InputField ipfNameOut = Instantiate(inputFieldPrefab, transform);
        ipfNameOut.text = JoinNames(r.outputs.names);
        ipfNameOut.onValueChanged.AddListener(delegate { OnNamesChanged(ipfNameOut, r, false); });
        ((RectTransform)ipfNameOut.transform).sizeDelta = size;
        ((RectTransform)ipfNameOut.transform).anchoredPosition = new Vector2(0, h);
        


        for (int i = 0; i < r.paramGroups.Count; i++)
        {
            ParameterGroup pg = r.paramGroups[i];
            //the actual UI elements are added here
            AddParameterGroup(r, pg,i);
        }
    }

    public void AddParameterGroup(Rule r, ParameterGroup pg, int index)
    {

        float h2 = h * 2;
        RectTransform pgui = Instantiate(ParameterGroupPrefab,transform).transform as RectTransform;
        pgui.anchoredPosition = new Vector2(0, index * h2 - h2);
        Vector3 size = pgui.sizeDelta;
        size.y = pg.parameters.Count * h + (h *3);
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
            ipftrans.anchoredPosition = new Vector2(30, -h * i - h2);
            ipf.text = p.value.ToString();
            Slider sld = Instantiate(sliderPrefab, pgui);
            sld.value = p.value;
            sld.minValue = p.min;
            sld.maxValue = p.max;
            if (p.step == 1) sld.wholeNumbers = true;

            sld.onValueChanged.AddListener(delegate { OnSliderValueChanged(sld, ipf, p ,r); });
            RectTransform sldtrans = sld.transform as RectTransform;
            sldtrans.anchoredPosition = new Vector2(60, -h * i - 10);

            inputFields.Add(ipf);
            sliders.Add(sld);

           
        }
    }

    void OnNamesChanged(InputField ipf, Rule r, bool isInput=true)
    {
        string[] trunks = ipf.text.Split(',');
        List<string> names = new List<string>();
        foreach(string n in trunks)
        {
            if(n!="") names.Add(n);
        }
        if (isInput) r.inputs.names = names;
        else r.outputs.names = names;
        r.grammar.ExecuteFrom(r);
        ruleNavigator.UpdateButtonDescriptions();
    }


    void OnSliderValueChanged(Slider sld, InputField ipf, Parameter p, Rule r)
    {
        ipf.text = sld.value.ToString();
        p.value = sld.value;
        r.grammar.ExecuteFrom(r);
    }

    string JoinNames(IEnumerable<string> names)
    {
        string text = "";
        foreach(string n in names)
        {
            text += n + ",";
        }
        return text;
    }
}
