using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SGGeometry;
using SGCore;

public class DesignContext {

    public Properties properties;
    public PlaningGrid gridProximity;
    public PlaningGrid gridAttraction;
    public List<Building> buildings;
    public List<SOPoint> attractions;
    public Color[] scaleColors;
    public Color[] colorProximity;
    public Color[] colorAttraction;
    public float gridSpacing;
    //public List<SGParticle> buildings = new List<SGParticle>();


    // Use this for initialization
    public DesignContext(Vector3[] boundary, float gridSpacing=6) {
        this.gridSpacing = gridSpacing;
        scaleColors = new Color[3];
        scaleColors[0] = Color.red;
        scaleColors[1] = Color.green;
        scaleColors[2] = Color.blue;

        colorProximity = new Color[3];
        colorProximity[0] = Color.red;
        colorProximity[1] = Color.green;
        colorProximity[2] = Color.blue;

        colorAttraction = new Color[2];
        colorAttraction[0] = Color.yellow;
        colorAttraction[1] = Color.red;

        gridProximity = new PlaningGrid(boundary, gridSpacing,0);
        gridAttraction = new PlaningGrid(boundary, gridSpacing,-1);

        properties = new Properties();
        buildings = new List<Building>();
        attractions = new List<SOPoint>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    public void Invalidate()
    {
        gridProximity.ResetCellProperties();
        foreach(PlaningGridCell cell in gridProximity.cells)
        {
            float maxDist = 60;
            float td = 0;
            foreach (Building b in buildings)
            {
                
                float d = Vector3.Distance(b.Position, cell.Position);
                d = Mathf.Clamp(d, 0.01f, maxDist);
                d = 1-((d*d) / (maxDist*maxDist));
                td += d;
            }
            cell.SetProp("density", td);
            //cell.SetHeight(td);
            cell.SetPlanarSize(td*3);
            float ntd = Mathf.Clamp(td / 5, 0, 1);
            Color c = colorScale(colorProximity, ntd);
            cell.SetColor(c);

            
        }
        foreach (PlaningGridCell cell in gridAttraction.cells)
        {
            float maxDist = 160;
            float td = 0;
            foreach (SOPoint sop in attractions)
            {

                float d = Vector3.Distance(sop.Position, cell.Position);
                d = Mathf.Clamp(d, 0.01f, maxDist);
                d = 1 - ((d*d) /( maxDist*maxDist));
                td += d;
            }
            //cell.SetHeight(td);
            cell.SetPlanarSize(td * 3);
            float ntd = Mathf.Clamp(td/3, 0, 1);
            Color c = colorScale(colorAttraction, ntd);
            cell.SetColor(c);
        }

    }

    public Color colorScale(Color[] colors, float i)
    {
        i *= colors.Length;
        float zone = Mathf.Floor(i);
        int indexA = (int)zone;
        int indexB = (int)zone + 1;
        float pos = i - zone;

        if (indexB < colors.Length)
        {
            Color c1 = colors[indexA];
            Color c2 = colors[indexB];
            Color co = new Color();
            for (int j = 0;  j<4; j++)
            {
                co[j] = c1[j] + ((c2[j] - c1[j]) * pos);
            }
            float r = c1.r + (c2.r - c1.r) * pos;
            return co;
        }
        return colors[colors.Length - 1];
    }
}
