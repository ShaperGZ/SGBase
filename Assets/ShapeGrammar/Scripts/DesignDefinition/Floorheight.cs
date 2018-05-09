using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorHeight{

    public float defaultHeight = 3;
    public List<float> heights;
    public float ground = 0;
    public int Count
    {
        get
        {
            if (heights != null && heights.Count > 0)
                return heights.Count;
            return -1;
        }
    }

    public FloorHeight()
    {
        heights = new List<float>();
    }
    public float top
    {
        get
        {
            if (heights != null && heights.Count > 0)
                return heights[heights.Count - 1];
            return ground;        
        }
    }
    public void SetNumFloors(int num, float? ftfh=null)
    {
        float h;
        if (ftfh.HasValue) h = ftfh.Value;
        else if (Count > 0) h = heights[heights.Count - 1];
        else h = defaultHeight;

        int dif = num - Count;
        if (dif > 0)
        {
            for (int i = 0; i < dif; i++)
            {
                heights.Add(h);
            }
        }
    }
    public void Clear()
    {
        heights.Clear();
    }
    public List<float> GetHeightTypes()
    {
        List<float> outH = new List<float>();
        foreach (float h in heights)
        {
            if (outH.Contains(h)) continue;
            else outH.Add(h);
        }
        return outH;
    }

}
