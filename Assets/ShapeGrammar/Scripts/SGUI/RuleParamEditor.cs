using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SGCore;
using SGGeometry;

namespace SGGUI {
    public class RuleParamEditor : MonoBehaviour
    {
        public float width = 30;
        public float height = 15;
        public RectTransform ParameterGroupPrefab;
        public InputField inputFieldPrefab;
        public Slider sliderPrefab;
        public List<InputField> inputFields;
        public List<RectTransform> ParameterGroups;
        public List<Slider> sliders;
        public Navigator ruleNavigator;

        float h = 25;

        // Use this for initialization
        void Start()
        {
            inputFields = new List<InputField>();
            ParameterGroups = new List<RectTransform>();
            //ParameterGroupPrefab = (Resources.Load("ParameterGroup") as GameObject).transform as RectTransform;
            //inputFieldPrefab = (Resources.Load("InputField") as GameObject).GetComponent<InputField>();

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void Clear()
        {
            for (int i = 0; i < inputFields.Count; i++)
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
        public void GenerateUI(GraphNode r)
        {
            Clear();
            InputField ipfNameIn = Instantiate(inputFieldPrefab, transform);
            ipfNameIn.text = JoinNames(r.inputs.names);
            ipfNameIn.onValueChanged.AddListener(delegate { OnNamesChanged(ipfNameIn, r, true); });
            Vector2 size = ((RectTransform)ipfNameIn.transform).sizeDelta;
            size.x = 100;
            ((RectTransform)ipfNameIn.transform).sizeDelta = size;
            ((RectTransform)ipfNameIn.transform).anchoredPosition = new Vector2(0, 0);

            InputField ipfNameOut = Instantiate(inputFieldPrefab, transform);
            ipfNameOut.text = JoinNames(r.outputs.names);
            ipfNameOut.onValueChanged.AddListener(delegate { OnNamesChanged(ipfNameOut, r, false); });
            ((RectTransform)ipfNameOut.transform).sizeDelta = size;
            ((RectTransform)ipfNameOut.transform).anchoredPosition = new Vector2(0, -h);
            inputFields.Add(ipfNameIn);
            inputFields.Add(ipfNameOut);
            
            //IDictionaryEnumerator em = r.paramGroups.GetEnumerator();
            int i = 0;
            foreach (DictionaryEntry em in r.paramGroups)
            {
                string name = (string)em.Key;
                ParameterGroup pg = (ParameterGroup)em.Value;
                AddParameterGroup(r, name, pg, i);
                i++;
            }
        }
        public void ExtractParamGroup(GraphNode r, ParameterGroup pg, InputField ipf)
        {
            string extractName = ipf.text;
            string orgExtractName = pg.extractName;
            pg.extractName = extractName;
            if (extractName != null && r.grammar != null)
            {
                GraphNode g = r.grammar;
                g.paramGroups[extractName] = pg;
                
            }
            if (extractName == null && r.grammar != null && orgExtractName != null)
            {
                GraphNode g = r.grammar;
                if (g.paramGroups.Contains(orgExtractName))
                {
                    g.paramGroups.Remove(orgExtractName);
                }
            }
        }
        public void ExtractParamGroup(GraphNode r, string key, string extractName=null)
        {
            object pg = r.paramGroups[key];
            string orgExtractName = ((ParameterGroup)pg).extractName;
            ((ParameterGroup)pg).extractName = extractName;
            if (extractName != null && r.grammar!=null)
            {
                GraphNode g = r.grammar;
                g.paramGroups[extractName]= pg;
                
            }
            if(extractName == null && r.grammar != null && orgExtractName!=null)
            {
                GraphNode g = r.grammar;
                if (g.paramGroups.Contains(orgExtractName))
                {
                    g.paramGroups.Remove(orgExtractName);
                }
            }
        }

        public void AddParameterGroup(GraphNode r, string pgName, ParameterGroup pg, int index)
        {
            float height = pg.parameters.Count * h;
            float h2 = h * 2;

            float locY;
            RectTransform pgui = Instantiate(ParameterGroupPrefab, transform).transform as RectTransform;
            if (ParameterGroups.Count > 0)
            {
                RectTransform lastpgui = ParameterGroups[ParameterGroups.Count - 1];
                locY = lastpgui.anchoredPosition.y - lastpgui.sizeDelta.y;
            }
            else
            {
                locY = h2*-1;
            }
            pgui.anchoredPosition = new Vector2(0, locY);
            Vector3 size = pgui.sizeDelta;
            size.y = pg.parameters.Count * h + h;
            pgui.sizeDelta = size;

            //dispaly name of the param group
            pgui.transform.Find("Title").GetComponent<Text>().text = pgName;
            //assign parameter extraction callback
            
            string key = pg.name;
            string extracName = pg.extractName;
            InputField extractNameIpf = pgui.transform.Find("InputField").GetComponent<InputField>();
            if (pg.extractName != null && pg.extractName != "")
                extractNameIpf.text = pg.extractName;
            extractNameIpf.onEndEdit.AddListener(
                delegate 
                {
                    ExtractParamGroup(r, pg, extractNameIpf);
                });

            ParameterGroups.Add(pgui);

            for (int i = 0; i < pg.parameters.Count; i++)
            {
                Parameter p = pg.parameters[i];
                float value = p.Value;
                Debug.Log("value=" + value);
                //GameObject o = new GameObject();
                InputField ipf = Instantiate(inputFieldPrefab, pgui);
                RectTransform ipftrans = ipf.transform as RectTransform;
                ipftrans.anchoredPosition = new Vector2(0, -h * i -h);
                ipf.text = p.Value.ToString();
                Slider sld = Instantiate(sliderPrefab, pgui);
                sld.value = p.Value;

                float min = p.min;
                float max = p.max;
                if (min > p.Value) min = p.Value / 2;
                if (max < p.Value) max = p.Value * 2;

                sld.minValue = min;
                sld.maxValue = max;
                if (p.step == 1) sld.wholeNumbers = true;
                ipf.onEndEdit.AddListener(delegate { OnIpfChanged(ipf, sld, p, r);});
                sld.onValueChanged.AddListener(delegate { OnSliderValueChanged(sld, ipf, p, r); });
                RectTransform sldtrans = sld.transform as RectTransform;
                sldtrans.anchoredPosition = new Vector2(30, -h * i - (h/2));

                inputFields.Add(ipf);
                sliders.Add(sld);


            }
        }

        void OnNamesChanged(InputField ipf, GraphNode r, bool isInput = true)
        {
            string[] trunks = ipf.text.Split(',');
            List<string> names = new List<string>();
            foreach (string n in trunks)
            {
                if (n != "") names.Add(n);
            }
            if (isInput) r.inputs.names = names;
            else r.outputs.names = names;
            r.grammar.ExecuteFrom(r);
            ruleNavigator.UpdateButtonDescriptions();
        }

        void OnIpfChanged(InputField ipf, Slider sld, Parameter p, GraphNode r)
        {
            float val = float.Parse(ipf.text);
            if (val < p.min) sld.minValue = val * 0.2f;
            else sld.minValue = p.min;
            if (val > p.max) sld.maxValue = val * 5;
            else sld.maxValue = p.max;
            sld.value = val;
            if (r.grammar != null)
                r.grammar.ExecuteFrom(r);
            else
                r.Execute();
        }
        
        void OnSliderValueChanged(Slider sld, InputField ipf, Parameter p, GraphNode r)
        {
            ipf.text = sld.value.ToString();
            p.Value = sld.value;
            if (r.grammar != null)
                r.grammar.ExecuteFrom(r);
            else
                r.Execute();
        }

        string JoinNames(IEnumerable<string> names)
        {
            string text = "";
            foreach (string n in names)
            {
                text += n + ",";
            }
            return text;
        }
    }
}



