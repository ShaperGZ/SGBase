using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SGGeometry;
using SGCore;

public class Site {

    public Properties properties;
    public PlaningGrid grid;
    public List<BuildingProperties> buildings;
    public Color[] scaleColors;
    //public List<SGParticle> buildings = new List<SGParticle>();


	// Use this for initialization
	public Site(Vector3[] boundary) {

        scaleColors = new Color[3];
        scaleColors[0] = Color.red;
        scaleColors[1] = Color.green;
        scaleColors[2] = Color.blue;

        grid = new PlaningGrid(boundary,3);
        properties = new Properties();
        buildings = new List<BuildingProperties>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    public void Invalidate()
    {
        grid.ResetCellProperties();
        foreach(PlaningGridCell cell in grid.cells)
        {
            float maxDist = 60;
            float td = 0;
            foreach (BuildingProperties b in buildings)
            {
                
                float d = Vector3.Distance(b.position, cell.Position);
                d = Mathf.Clamp(d, 0.01f, maxDist);
                d = 1-((d*d) / (maxDist*maxDist));
                td += d;
            }
            cell.SetProp("density", td);
            //cell.SetHeight(td);
            cell.SetPlanarSize(td);
            float ntd = Mathf.Clamp(td / 5, 0, 1);
            Color c = colorScale(scaleColors, ntd);
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
