using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaningScheme{
    public SiteProperty site;
    public float gfa=0;
    public float plotRatio=0;
    public float coverage=0;
    
    public List<BuildingType> buildingTypes;
    public List<int> counts;

    public float difGFA { get { return gfa - site.gfa; } }
    public float difPlotRatio { get { return plotRatio- site.plotRatio; } }
    public float difCoverage { get { return coverage - site.coverage; } }
	public float TotalSales
    {
        get
        {
            float ttl = 0;
            for (int i = 0; i < buildingTypes.Count; i++)
            {
                float p = buildingTypes[i].unitPrice * buildingTypes[i].GFA;
                ttl += p * counts[i];
            }
            return ttl;
        }
    }

    public PlaningScheme()
    {
        buildingTypes = new List<BuildingType>();
        counts = new List<int>();
    }

    public void AddTypeAndQuantity(BuildingType type, int count)
    {
        buildingTypes.Add(type);
        counts.Add(count);
    }
    public string Format()
    {
        string txt = string.Format(" GFA={0}({1})",gfa,difGFA);
        txt += string.Format("\n PlotRatio={0}({1})",plotRatio,difPlotRatio);
        txt += string.Format("\n Coverage={0}({1})",coverage,difCoverage);
        for (int i = 0; i < counts.Count; i++)
        {
            txt += string.Format("\n type{0}:{1}",i,counts[i]);
        }
        return txt;
    }

}

public class PlaningSchemeA : PlaningScheme
{

    public PlaningSchemeA() : base()
    {

    }
}
