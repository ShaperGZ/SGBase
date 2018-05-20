using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SGCore;

public class BuildingParamEditor : MonoBehaviour {

    Transform[] trans = new Transform[8];
    SGBuilding building=null;

    public Button btPlanningMode;
    public Button btMassingMode;
    public Button btGraphicsMode;
    public Button btUnitMode;
    public bool freeze=false;
    public ProgramRatioVisualizer programRatioVisualizer;

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
                    sld.onValueChanged.AddListener(delegate
                    {
                        if (building == null) return;
                        float h = sld.value;
                        h -= h % 4;
                        ipf.text = h.ToString();
                        if(!freeze)
                            ChangeHeight(h);
                    });
                    break;
                case 2:
                    sld.onValueChanged.AddListener(delegate
                    {
                        if (building == null) return;
                        float h = sld.value;
                        h -= h % 3;
                        ipf.text = h.ToString();
                        if (!freeze)
                            ChangeWidth(h);
                    });
                    break;
                case 3:
                    sld.onValueChanged.AddListener(delegate
                    {
                        if (building == null) return;
                        float h = sld.value;
                        h -= h % 3;
                        ipf.text = h.ToString();
                        if (!freeze)
                            ChangeDepth(h);
                    });
                    break;
                default:
                    break;
            }
        }
        btPlanningMode.onClick.AddListener(delegate {
            SceneManager.SelectedGrammar = building.gPlaning;
            buildingPlanningMode();
        });
        btMassingMode.onClick.AddListener(delegate {
            SceneManager.SelectedGrammar = building.gMassing;
            buildingMassingMode();
        });
        btGraphicsMode.onClick.AddListener(delegate {
            SceneManager.SelectedGrammar=building.gFacade;
            buildingGraphicsMode();
        });
        btUnitMode.onClick.AddListener(delegate {
            SceneManager.SelectedGrammar = building.gProgram;
            buildingProgramMode();
        });

        programRatioVisualizer = transform.Find("ProgramRatioPanel").GetComponent<ProgramRatioVisualizer>();


    }
    void Start () {
	}
    public void buildingPlanningMode()
    {
        if (building == null) return;
        building.PlanningMode();
        ShapeObject.drawScope = true;
        //this.programRatioVisualizer.gameObject.SetActive(false);
    }
    public void buildingMassingMode()
    {
        if (building == null) return;
        building.MassingMode();
        ShapeObject.drawScope = true;
        //this.programRatioVisualizer.gameObject.SetActive(false);
    }
    public void buildingGraphicsMode()
    {
        if (building == null) return;
        ShapeObject.drawScope = false;
        building.GraphicsMode();
        //this.programRatioVisualizer.gameObject.SetActive(false);
    }
    public void buildingProgramMode()
    {
        if (building == null) return;
        ShapeObject.drawScope = false;
        building.ProgramMode();
        //this.programRatioVisualizer.gameObject.SetActive(true);
    }
	public void SetBuilding(SGBuilding b)
    {
        //if (b == null)
        //{
        //    Clear();
        //    return;
        //}
        if (building != null)
        {
            building.buildingParamEditor = null;
        }
        building = b;
        b.buildingParamEditor = this;
        b.UpdateParams();
        
    }
    public void UpdateBuildingParamDisplay(int skip=-1)
    {
        if (skip != 0) UpdateDisplay(0, "高度", building.height, 15, 200);
        if (skip != 1) UpdateDisplay(1, "层数", building.floorCount, 3, 50);
        if (skip != 2) UpdateDisplay(2, "面宽", building.width, 15, 80);
        if (skip != 3) UpdateDisplay(3, "深度", building.depth, 6, 80);
        if (skip != 4) UpdateDisplay(4, "面积", building.gfa, 500, 80000);
        if (skip != 5) UpdateDisplay(5, "效率", building.efficiency, 0, 1);
        if (skip != 6) UpdateDisplay(6, "光线", building.illumination, 0, 1);

        if (programRatioVisualizer != null && building.gProgram!=null)
        {
            List<ShapeObject> sos = building.gProgram.outputs.shapes;
            programRatioVisualizer.SetRatio(sos);
        }
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
        Grammar g = building.gPlaning;
        GraphNode gn = g.FindFirst("SizeBuilding3D");
        if(gn!=null)
        {
            gn.SetParam("Size", 1, f);
            building.freeze = true;
            g.Execute();
            building.UpdateParams(false);
            freeze = true;
            this.UpdateBuildingParamDisplay(0);
            freeze = false;
        }
    }
    public void ChangeWidth(float f)
    {
        Grammar g = building.gPlaning;
        GraphNode gn = g.FindFirst("SizeBuilding3D");
        if (gn != null)
        {
            gn.SetParam("Size", 0, f);
            building.freeze = true;
            g.Execute();
            building.UpdateParams(false);
            freeze = true;
            this.UpdateBuildingParamDisplay(2);
            freeze = false;
        }
    }
    public void ChangeDepth(float f)
    {
        Grammar g = building.gPlaning;
        GraphNode gn = g.FindFirst("SizeBuilding3D");
        if (gn != null)
        {
            gn.SetParam("Size", 2, f);
            building.freeze = true;
            g.Execute();
            building.UpdateParams(false);
            freeze = true;
            this.UpdateBuildingParamDisplay(3);
            freeze = false;
        }
    }


    // Update is called once per frame
    void Update () {
		
	}
}
