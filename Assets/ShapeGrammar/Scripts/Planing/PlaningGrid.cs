using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SGCore;
using SGGeometry;

public class PlaningGrid {

    public List<PlaningGridCell> cells;
    int countW = 0;
    int countD = 0;

    
	public PlaningGrid(Vector3[] boundary, float cellSize=6)
    {
        cells = new List<PlaningGridCell>();
        BoundingBox bbox = BoundingBox.CreateFromPoints(boundary);
        countW = (int)(Mathf.Round(bbox.size[0] / cellSize));
        countD = (int)(Mathf.Round(bbox.size[2] / cellSize));
        
        for (int i = 0; i < countD; i++)
        {
            
            for (int j = 0; j < countW; j++)
            {
                float skip = 0;
                //float skip = (j % 2) * cellSize;
                Vector3 pos = new Vector3((j * cellSize)+skip, 0, i * cellSize);
                pos += bbox.position;
                PlaningGridCell cell = PlaningGridCell.CreateCell(pos, 0.1f);
                cells.Add(cell);
            }
        }
    }
    public void ResetCellProperties()
    {
        foreach(PlaningGridCell cell in cells)
        {
            cell.ResetProperties();
        }
    }
    int GetIndex(int w, int d)
    {
        return w + (d * countW);
    }
    public PlaningGridCell[] AdjacentCells(int i)
    {
        throw new System.NotImplementedException();
        ////0,1,2
        //// 3,4,5
        ////6,7,8

        //int d = i % countW;
        //int w = i - (d * countW);
        //int[] indices = new int[6];
        //int skip = (d % 2);
        //indices[0] = GetIndex(w, d - 1) - skip;
        //indices[0] = GetIndex(w+1, d - 1) - skip;
        //indices[0] = GetIndex(w+1, d ) ;
        //indices[0] = GetIndex(w+1, d +1 )-skip;


        //PlaningGridCell[] cells = new PlaningGridCell[6];
       
    }

}
