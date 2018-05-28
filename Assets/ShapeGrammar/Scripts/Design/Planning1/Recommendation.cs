using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SGCore;
using SGGeometry;

public class Recommendation {
    public PlaningMatrix3 matrix;
    //public Color[] colorSet = new Color[] { Color.blue, Color.green, Color.yellow, Color.red };
    public Color[] colorSet = new Color[] { Color.red, Color.yellow, Color.green, Color.blue };
    public int? recommendedIndex = null;

    public Recommendation(PlaningMatrix3 matrix)
    {
        this.matrix = matrix;
    }
    public void visualize()
    {
        float minDif = 10000000;
        //when the site property is not given
        if (matrix.siteProp.gfa == 0)
        {
            for (int i = 0; i < matrix.cells.Count; i++)
            {
                GameObject cell = matrix.cells[i];
                cell.GetComponent<MeshRenderer>().material.color = Color.white;
                cell.transform.localScale = new Vector3(0.02f, 0.02f, 0.02f);
            }
            recommendedIndex = null;
            return;
        }

        //normal visualization
        for (int i = 0; i < matrix.cells.Count; i++)
        {
            GameObject cell = matrix.cells[i];
            float d = matrix.schemes[i].difGFA;
            d = Mathf.Abs(d);
            if (d < minDif)
            {
                minDif = d;
                recommendedIndex = i;
            }
            float max = matrix.siteProp.gfa * 0.2f;
            
            float clampD = Mathf.Clamp(d, 800, max);
            float ratio = 1 - (clampD / max);
            
            //Debug.LogFormat("siteArea={0}, plotRatio={1}", matrix.siteProp.siteArea, matrix.siteProp.plotRatio);
            //Debug.LogFormat("d={0}, campD={1},ratio={2}, max={3}", d, clampD, ratio, max);
            Color color = SGUtility.colorScale(colorSet, ratio);
            ratio = Mathf.Clamp(ratio, 0.2f, 1f);
            float r = matrix.cellSize * ratio;
                
            cell.GetComponent<MeshRenderer>().material.color = color;
            cell.transform.localScale = new Vector3(r, r, r);
        }


    }
}
