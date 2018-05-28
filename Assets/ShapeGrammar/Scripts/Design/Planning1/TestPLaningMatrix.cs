using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SGGeometry;
using SGCore;

public class SiteProperty
{
    public float siteArea=0;
    public float plotRatio=0;
    public float coverage=0;
    public float gfa { get { return siteArea * plotRatio; } }
    
}

public class TestPLaningMatrix : MonoBehaviour {
    PlaningMatrix3 pm3;
    public Camera calCamera;
    DesignContext context;
    public SGPlaningParticleSystem particleSystem;
    public ShapeObject initShape;
    public Grammar grammar;

    public GameObject sldSiteW;
    public GameObject sldSiteH;
    public GameObject sldSitePlotRatio;
    public GameObject sldSiteCoverage;
    public Vector3[] boundaryPts;
    public LineRenderer boundaryLine;

    SiteProperty siteProp;
    private float siteW;
    private float siteH;


    // Use this for initialization
    void Start () {
        GameObject lro = new GameObject();
        boundaryLine = lro.AddComponent<LineRenderer>();
        boundaryLine.positionCount=4;
        boundaryLine.SetWidth(3, 3);

        particleSystem = new SGPlaningParticleSystem();
        siteProp = new SiteProperty();

        pm3 = new PlaningMatrix3(ref siteProp);
        pm3.camera = calCamera;
        pm3.AddType("A", new Vector3(45, 100, 20), 1.2f);
        pm3.AddType("B", new Vector3(45, 60, 20), 1.6f);
        pm3.AddType("C", new Vector3(45, 9, 20), 1.8f);
        //pm3.AddType("A", 1200, 100, 30, 1.2f);
        //pm3.AddType("B", 1200, 50, 15, 1.5f);
        //pm3.AddType("C", 800, 40, 8,1.8f);

        pm3.genGrid();

        UpdateSiteParam();

        AssignSliderAction(sldSiteW);
        AssignSliderAction(sldSiteH);
        AssignSliderAction(sldSitePlotRatio);
        AssignSliderAction(sldSiteCoverage);

        //updateDesign();

    }
    private void AssignSliderAction(GameObject go)
    {
        Slider sld = go.transform.Find("Slider").GetComponent<Slider>();
        sld.onValueChanged.AddListener(delegate { UpdateSliderStatus(go); });
        
    }
    private void UpdateSliderStatus(GameObject go)
    {
        Slider sld = go.transform.Find("Slider").GetComponent<Slider>();
        InputField ipf = go.transform.Find("InputField").GetComponent<InputField>();
        ipf.text = sld.value.ToString();

        UpdateSiteParam();
        pm3.genGrid();
        updateDesign();
    }
    public void UpdateSiteParam()
    {
        float w = sldSiteW.transform.Find("Slider").GetComponent<Slider>().value;
        float h = sldSiteH.transform.Find("Slider").GetComponent<Slider>().value;
        siteProp.plotRatio = sldSitePlotRatio.transform.Find("Slider").GetComponent<Slider>().value;

        siteProp.siteArea = w * h;
        siteW = w;
        siteH = h;

        boundaryPts = new Vector3[] {
            new Vector3(-siteW/2,0,-siteH/2),
            new Vector3(siteW/2,0,-siteH/2),
            new Vector3(siteW/2,0,siteH/2),
            new Vector3(-siteW/2,0,siteH/2),
        };

        particleSystem.boundary = boundaryPts;
        boundaryLine.SetPositions(boundaryPts);
        boundaryLine.loop = true;

        updateDesign();
    }
    public void updateDesign()
    {
        if (initShape == null)
        {
            initShape = ShapeObject.CreateBasic();
        }
        
        Polygon pg = new Polygon(boundaryPts);
        initShape.SetMeshable(pg);
        

        if(grammar == null)
        {
            grammar = new Grammar();
            grammar.inputs.shapes.Add(initShape);
            grammar.AddRule(new Rules.PivotTurn("A", "A", 2));
            grammar.AddRule(new Rules.DivideTo("A", new string[] { "A", "B" }, 30, 2),false);
            grammar.AddRule(new Rules.DivideTo("A", new string[] { "A", "A" }, 60, 0), false);
            grammar.AddRule(new Rules.Hide(new string[] { "B" }), false);
            grammar.AddRule(new Rules.Extrude("A", "A", 30),false);
            grammar.AddRule(new Rules.CreateBuilding("A",pm3,particleSystem), false);

        }

        grammar.Execute();
        
    }

    // Update is called once per frame
    void Update () {

        particleSystem.Update();
        particleSystem.UpdateEntropyDisplay();
    }
    private void OnRenderObject()
    {
        if(pm3!=null)
            pm3.OnRenderObjects();
    }




}
