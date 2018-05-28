using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SGGeometry;
using SGCore;

public class BuildingType
{
    public string name = "";
    public float footprint = 1.5f;
    public float height = 100f;
    public float floorCount = 30;
    public float unitPrice = 1.2f;
    public float width = 40;
    public float depth = 20;
 
    public float GFA
    {
        get
        {
            return footprint * floorCount;
        }
    }

    
}

public class PlaningMatrix3 {
    public Camera camera;
    public float maxH = 0.3f;
    public float area = 80000;
    public float plotRatio = 1.5f;
    public float density = 0.5f;
    public float cellSize = 0.1f;
    public Recommendation recommendation=null;
    public PlaningScheme recommendedScheme=null;
    public int? recommendationIndex = null;

    public List<PlaningScheme> schemes;
    public SiteProperty siteProp;

    public List<BuildingType> buildingTypes;
    public List<GameObject> cells;
    //public List<float> countAs;
    //public List<float> countBs;
    //public List<float> countCs;


    List<Vector3> positions;
    public PlaningMatrix3(ref SiteProperty siteProp)
    {
        buildingTypes = new List<BuildingType>();
        schemes = new List<PlaningScheme>();
        this.siteProp = siteProp;
        recommendation = new Recommendation(this);
    }
    public void AddType(string name,Vector3 size, float unitPrice)
    {
        BuildingType bt = new BuildingType();
        bt.name = name;
        bt.width = size.x;
        bt.height = size.y;
        bt.depth = size.z;
        bt.footprint = size.x * size.z;
        bt.floorCount = size.y / 3;
        bt.unitPrice = unitPrice;
        buildingTypes.Add(bt);
    }

    public void AddType(string name, float footPrint, float height, float floorCount, float unitPrice)
    {
        BuildingType bt = new BuildingType();
        bt.name = name;
        bt.footprint = footPrint;
        bt.height = height;
        bt.floorCount = floorCount;
        bt.unitPrice = unitPrice;
        buildingTypes.Add(bt);
    }

    public void genGrid(int maxCountA=10, int maxCountB = 10, int maxCountC = 10)
    {
        if(cells==null)cells = new List<GameObject>();
        //countAs = new List<float>();
        //countBs = new List<float>();
        //countCs = new List<float>();
        float targetGFA = siteProp.siteArea * siteProp.plotRatio;


        positions = new List<Vector3>();
        List<float> difs = new List<float>();
        List<float> gfas = new List<float>();

        //calculate all options and update cell positions
        for (int i = 0; i < maxCountA; i++)
        {
            for (int k = 0; k < maxCountB; k++)
            {
                float designAreaA = (float)i * buildingTypes[0].GFA;
                float designAreaB = (float)k * buildingTypes[2].GFA;

                float remainGFA = targetGFA - designAreaA - designAreaB;
                int j = Mathf.RoundToInt(remainGFA / buildingTypes[1].GFA);
                if (j < 0) j = 0;
               
                float designArea =
                    ((float)i * buildingTypes[0].GFA)  +
                    ((float)j * buildingTypes[1].GFA)  +
                    ((float)k * buildingTypes[2].GFA)
                    ;
                gfas.Add(designArea);
                float dif = targetGFA - designArea;
                dif = Mathf.Abs(dif);
                difs.Add(dif);
                if(j>0)
                    Debug.LogFormat("targetGFA={4}, remain={3},i={0},j={1},k={2}",i,j,k,remainGFA,targetGFA);
                PlaningScheme scheme = new PlaningScheme();
                if (j > 0)
                    Debug.LogFormat("---Pre i={0},j={1},k={2}", i,j,k);
                scheme.AddTypeAndQuantity(buildingTypes[0], i);
                scheme.AddTypeAndQuantity(buildingTypes[1], j);
                scheme.AddTypeAndQuantity(buildingTypes[2], k);
                if (j > 0)
                    Debug.LogFormat("---Pst i={0},j={1},k={2}", 
                    scheme.counts[0],
                    scheme.counts[1],
                    scheme.counts[2]);
                scheme.site = siteProp;
                scheme.gfa = designArea;
                schemes.Add(scheme);
                
                Vector3 pos = new Vector3(i * cellSize, j * cellSize * 0.3f, k * cellSize);
                positions.Add(pos);
                //Debug.LogFormat("A:{0}, B:{1}, C:{2}, dif:{3}", i, j, k, dif);
                
            }
        }
        //Debug.LogFormat("minDif={0}, maxDif={1}", minDif, maxDif);
        Color[] colorSet = new Color[] { Color.blue, Color.green, Color.yellow, Color.red };
        for (int i = 0; i < positions.Count; i++)
        {

            if (i >= cells.Count)
            {
                GameObject oi = GameObject.CreatePrimitive(PrimitiveType.Cube);
                oi.AddComponent<OnMouseOverCalCell>();
                cells.Add(oi);

            }
            GameObject o = cells[i];
            o.transform.position = (positions[i]);
            o.transform.localScale = new Vector3(cellSize,cellSize,cellSize);
            o.layer = 10;

            OnMouseOverCalCell cc= o.GetComponent<OnMouseOverCalCell>();
            cc.scheme = schemes[i];
        }// for i

        Recommand();
    }
    public void Recommand()
    {
        if (recommendation != null)
        {
            recommendationIndex = null;
            recommendation.visualize();
            recommendationIndex = recommendation.recommendedIndex;
            //Debug.Log("recommendedIndex=" + recommendationIndex);
            if (recommendationIndex.HasValue)
                recommendedScheme = schemes[recommendationIndex.Value];
            //Debug.Log("recommendedScheme=" + recommendedScheme);
        }
    }
    public void OnRenderObjects()
    {
        List<Vector3> pts = new List<Vector3>();
        foreach(Vector3 p in positions)
        {
            pts.Add(p);
            pts.Add(new Vector3(p.x, 0, p.z));
        }
        for (float i = 0; i <= 1; i+=0.1f)
        {
            pts.Add(new Vector3(i, 0, 0));
            pts.Add(new Vector3(i, 0, 1));
            pts.Add(new Vector3(0, 0, i));
            pts.Add(new Vector3(1, 0, i));

        }
        
        SGGeometry.GLRender.Lines(pts.ToArray(), new Color(0.1f,0.1f,0.1f));

        if (recommendationIndex != null)
        {
            int i = recommendationIndex.Value;
            pts = new List<Vector3>();
            Vector3 pos = cells[i].transform.position;
            pts.Add(pos);
            pts.Add(pos + new Vector3(0, 1, 0));
            SGGeometry.GLRender.Lines(pts.ToArray(), Color.white);
        }
        

    }
    


}
