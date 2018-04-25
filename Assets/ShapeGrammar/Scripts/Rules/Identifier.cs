using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SGCore;
using SGGeometry;
using System.Collections.Specialized;

namespace Rules
{
   
    public class NamesByAreaEdges:Rule
    {
        public NamesByAreaEdges() : base() { }
        public NamesByAreaEdges(string inName, string outName,float area=800,int edgeCount = -1) : this()
        {
            inputs.names.Add(inName);
            outputs.names.Add(outName);
            outputs.names.Add(inName);
            ((ParameterGroup)paramGroups["EdgeCount"]).parameters[0].value = edgeCount;
            ((ParameterGroup)paramGroups["Area"]).parameters[0].value = area;

        }


        public override void Execute()
        {
            //outMeshables.Clear();
            int rqEdgeCount = (int)(((ParameterGroup)paramGroups["EdgeCount"]).parameters[0].value);
            int rqArea = (int)(((ParameterGroup)paramGroups["Area"]).parameters[0].value);
            //outputs.shapes.Clear();
            List<ShapeObject> pass = new List<ShapeObject>();
            List<ShapeObject> fail = new List<ShapeObject>();

            foreach (ShapeObject so in inputs.shapes)
            {
                Meshable m = so.meshable;
                int edgeCount;
                float area;
                if(m.GetType() == typeof(SGGeometry.Extrusion))
                {
                    Extrusion etx = (Extrusion)m;
                    edgeCount = etx.polygon.vertices.Length;
                    area = etx.polygon.Area();
                }
                else if(m.GetType() == typeof(SGGeometry.Polygon))
                {
                    Polygon pg = (Polygon)m;
                    edgeCount = pg.vertices.Length;
                    area = pg.Area();
                }
                else
                {
                    area = -1;
                    edgeCount = -1;
                }

                bool meetEdgeCount = false;
                if (rqEdgeCount < 0) meetEdgeCount = true; 
                if (rqEdgeCount == edgeCount) meetEdgeCount = true; 

                if (area >= rqArea && meetEdgeCount)
                {
                    pass.Add(so);
                }
                else
                {
                    fail.Add(so);
                }
            }
            UpdateOutputShapes(pass);
        }
        public new void UpdateOutputShapes(List<ShapeObject> pass)
        {
            int dif = outputs.shapes.Count - inputs.shapes.Count;
            if (dif > 0)
            {
                //Debug.Log("dif >0 ouputs.shapesCount="+outputs.shapes.Count);
                for (int i = 0; i < dif; i++)
                {
                    int index = outputs.shapes.Count - 1;
                    GameObject.Destroy(outputs.shapes[index].gameObject);
                    outputs.shapes.RemoveAt(index);
                }
                //Debug.Log("post destroy ouputs.shapesCount=" + outputs.shapes.Count);
            }

            string namePass = outputs.names[0];
            string nameFail = inputs.names[0];
            if (outputs.names[1] != null)
                nameFail = outputs.names[1];

            //update output shapes
            int shapeCount = outputs.shapes.Count;
            for (int i = 0; i < inputs.shapes.Count; i++)
            {
                if (i < shapeCount)
                {
                    inputs.shapes[i].CloneTo(outputs.shapes[i]);
                    outputs.shapes[i].Invalidate();

                }//end if 
                else
                {
                    ShapeObject so = inputs.shapes[i].Clone();
                    outputs.shapes.Add(so);
                }
                outputs.shapes[i].parentRule = this;
                if (pass.Contains(inputs.shapes[i]))
                    outputs.shapes[i].name = namePass;
                else
                    outputs.shapes[i].name = nameFail;
            }//end for i
            //Debug.Log(string.Format("{0} outputShapes:{1}, mesable:{2}", name, outputs.shapes.Count, outMeshables.Count));

        }
        public override OrderedDictionary DefaultParam()
        {
            OrderedDictionary dict = new OrderedDictionary();
            ParameterGroup pg1 = new ParameterGroup();
            ParameterGroup pg2 = new ParameterGroup();

            dict.Add("Area", pg1);
            dict.Add("EdgeCount", pg2);
            pg1.Add(new Parameter(800, 0, 6, 1));
            pg2.Add(new Parameter(-1, -1, 6, 1));
            return dict;
        }
    }
}

