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
    public List<BuildingType> buildingTypes;
    public List<GameObject> cells;
    public List<float> countAs;
    public List<float> countBs;
    public List<float> countCs;
    public SiteProperty siteProp;

    List<Vector3> positions;
    public PlaningMatrix3(ref SiteProperty siteProp)
    {
        buildingTypes = new List<BuildingType>();
        this.siteProp = siteProp;
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

        countAs = new List<float>();
        countBs = new List<float>();
        countCs = new List<float>();
        float targetGFA = siteProp.siteArea * siteProp.sitePlotRatio;

        float stepX = 1f / (float)maxCountA;
        float stepY = 1f / (float)maxCountB;
        float stepZ = 1f / (float)maxCountC;

        float cellSizeX = stepX * 0.8f;  
        float cellSizeY = stepY * 0.8f;  
        float cellSizeZ = stepZ * 0.8f;


        positions = new List<Vector3>();
        List<float> difs = new List<float>();
        List<float> gfas = new List<float>();
        float maxDif=0;
        float minDif=100000000000000;
        for (int i = 0; i < maxCountA; i++)
        {
            for (int k = 0; k < maxCountB; k++)
            {
                //for (int k = 0; k < maxCountC; k++)
                //{

                float designAreaA = (float)i * buildingTypes[0].GFA;
                float designAreaB = (float)k * buildingTypes[2].GFA;
                
                float j = (targetGFA - designAreaA - designAreaB) / buildingTypes[1].GFA;
                j = Mathf.Round(j);
                if (j < 0) j = 0;
                //if (j < 0) continue;

                countAs.Add((float)i);
                countBs.Add(j);
                countCs.Add((float)k);
               
                float designArea =
                    ((float)i * buildingTypes[0].GFA)  +
                    ((float)j * buildingTypes[1].GFA)  +
                    ((float)k * buildingTypes[2].GFA)
                    ;
                gfas.Add(designArea);
                float dif = targetGFA - designArea;
                dif = Mathf.Abs(dif);
                difs.Add(dif);

                if (dif > maxDif) maxDif = dif;
                //if (dif < minDif) minDif = dif;
                
                Vector3 pos = new Vector3(i * stepX, j * stepY *0.3f, k * stepZ);
                positions.Add(pos);
                //Debug.LogFormat("A:{0}, B:{1}, C:{2}, dif:{3}", i, j, k, dif);
                
            }
        }
        Debug.LogFormat("minDif={0}, maxDif={1}", minDif, maxDif);
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
            o.layer = 10;
            OnMouseOverCalCell cc= o.GetComponent<OnMouseOverCalCell>();
            cc.calCamera = camera;
            cc.countA = countAs[i];
            cc.countB = countBs[i];
            cc.countC = countCs[i];
            cc.dif = difs[i];
            cc.gfa = gfas[i];
            cc.totalSales =
                    (cc.countA * buildingTypes[0].GFA * buildingTypes[0].unitPrice) +
                    (cc.countB * buildingTypes[1].GFA * buildingTypes[1].unitPrice) +
                    (cc.countC * buildingTypes[2].GFA * buildingTypes[2].unitPrice)
                    ;
            float d = difs[i];
            d = Mathf.Clamp(d, 100, 10000);
            float ratio = d / 10000;
            ratio = Mathf.Clamp(ratio, 0f, 0.9f);
            //float ratio = d/ maxDif;
            //float ratio = (d +1000)/ (2000);
            Color color = SGUtility.colorScale( colorSet, ratio);
            o.GetComponent<MeshRenderer>().material.color = color;
            Debug.Log(ratio);
            //cellSizeY = dif / targetGFA / 30;
            o.transform.localScale = new Vector3(cellSizeX * (1-ratio), cellSizeY * (1 - ratio), cellSizeZ * (1 - ratio));
            
        }// for i
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
    }
    


}
