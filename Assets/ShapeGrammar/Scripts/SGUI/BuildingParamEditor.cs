using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SGCore;

public class BuildingParamEditor : MonoBehaviour {

    Transform[] trans = new Transform[8];
    SGBuilding building=null;
    Grammar grammar=null;

    public Button btGraphicsMode;
    public Button btUnitMode;

    // Use this for initialization
    private void Awake()
    {
        SceneManager.buildingParamEditor = this;
        for (int i = 0; i < 7; i++)
        {
            //Debug.Log("i==" + i);
            string name = string.Format("panelgroup ({0})", i);
            trans[i] = transform.Find(name);
            Slider sld = trans[i].Find("Slider").GetComponent<Slider>();
            InputField ipf = trans[i].Find("InputField").GetComponent<InputField>();
            
            switch (i)
            {
                case 0:
                    ipf.onEndEdit.AddListener(
                        delegate
                        {
                            string txt = ipf.text;
                            float h = float.Parse(txt);
                            ChangeHeight(h);

                        }
                        );
                    //sld.onValueChanged.AddListener(delegate
                    //{
                    //    if (building == null) return;
                    //    float h = sld.value;
                    //    ChangeHeight(h);
                    //});
                    break;
                default:
                    break;
            }
        }

        btGraphicsMode.onClick.AddListener(delegate { buildingGraphicsMode(); });
        btUnitMode.onClick.AddListener(delegate { buildingProgramMode(); });



    }
    void Start () {
	}
    public void buildingGraphicsMode()
    {
        if (building == null) return;
        building.GraphicsMode();
    }
    public void buildingProgramMode()
    {
        if (building == null) return;
        building.ProgramMode();
    }
	public void SetBuilding(SGBuilding b)
    {
        if (b == null)
        {
            Clear();
            return;
        }
        if (building != null)
        {
            building.buildingParamEditor = null;
        }
        building = b;
        b.buildingParamEditor = this;
        grammar = b.gPlaning;
        b.UpdateParams();
        b.freeze = true;
        //UpdateBuildingParamDisplay();
        b.freeze = false;
        
    }
    public void UpdateBuildingParamDisplay(int skip=-1)
    {
        if (skip != 0) UpdateDisplay(0, "高度", building.height, 15, 200);
        if (skip != 1) UpdateDisplay(1, "层数", building.floorCount, 3, 50);
        if (skip != 2) UpdateDisplay(2, "面宽", building.width, 15, 80);
        if (skip != 3) UpdateDisplay(3, "深度", building.depth, 6, 80);
        if (skip != 4) UpdateDisplay(4, "面积", building.gfa, 500, 80000);
        if (skip != 5) UpdateDisplay(5, "效率", building.efficiency, 0, 100);
        if (skip != 6) UpdateDisplay(6, "光线", building.illumination, 0, 1);
    }
    public void Clear()
    {
        gameObject.SetActive(false);
    }
    public void UpdateDisplay(int index, string txt, float value, float? min=null, float? max=null, float? step = null)
    {
        Text text = trans[index].Find("Text").GetComponent<Text>();
        Slider sld = trans[index].Find("Slider").GetComponent<Slider>();
        InputField ifp= trans[index].Find("InputField").GetComponent<InputField>();
        text.text = txt;
        sld.value = value;
        ifp.text = value.ToString();

        if(index==4)//特别处理面积
        {
            ifp.text = (value / 10000).ToString() + "万";
        }

        if (min.HasValue) sld.minValue = min.Value;
        if (max.HasValue) sld.maxValue = max.Value;
    }
    
    public void ChangeHeight(float f)
    {

        GraphNode gn = grammar.FindFirst("SizeBuilding3D");
        if(gn!=null)
        {
            //float val = gn.GetParamVal("Size", 1);
            //Debug.LogFormat("Pre  {0}!={1}: {2}", f, val, f != val);
            //if(f!=val)
            //{
            gn.SetParam("Size", 1, f);
            building.freeze = true;
            grammar.Execute();
            this.UpdateBuildingParamDisplay();
            building.freeze = false;
            //}
                

            //val = gn.GetParamVal("Size", 1);
            //Debug.LogFormat("Post {0}!={1}: {2}", f, val, f != val);
            //grammar.Execute();
        }
            

    }


	// Update is called once per frame
	void Update () {
		
	}
}
