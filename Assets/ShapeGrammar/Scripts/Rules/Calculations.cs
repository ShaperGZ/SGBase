using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SGCore;
using SGGeometry;

namespace Rules
{
    public class CalBuildingParams:Rule
    {
        public CalBuildingParams(string[] names) : base()
        {
            inputs.names.AddRange(names);
        }
        public override void Execute()
        {
            if (building != null)
            {
                building.gfa = 0;
                building.footPrint = 0;
                building.floorCount = Mathf.RoundToInt(building.height / 4);
                //Debug.Log("Shape Count="+inputs.shapes.Count);
                foreach (ShapeObject so in inputs.shapes)
                {
                    Extrusion ext = null;
                    Polygon pg = null;
                    try
                    {
                        ext = (Extrusion)so.meshable;
                        pg = ext.polygon;
                    }
                    catch { }

                    float area = pg.Area();
                    if (ext != null)
                    {
                        if (so.Position.y == building.ground) building.footPrint += area;
                    }
                    int count = Mathf.RoundToInt(ext.height / 4);
                    building.gfa += count * area;
                }
                //Debug.Log("post cal gfa=" + building.gfa);
            }
            

            outputs.shapes = inputs.shapes;
        }

    }
}

