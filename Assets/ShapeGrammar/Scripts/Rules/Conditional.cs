using System;
using System.Collections;
using System.Collections.Specialized;
using System.Collections.Generic;
using UnityEngine;
using SGCore;
using SGGeometry;

namespace Rules
{
    public class Dispatcher:Rule
    {
        List<GraphNode> options;
        public Dispatcher(string inName, List<GraphNode> nodes):base(inName, new string[] { inName})
        {
            AddParam("Dispatch", 0, 0, nodes.Count - 1, 1);
            options = nodes;
        }

    }
    public class ApartmentLoadFilter : Rule
    {
        public ApartmentLoadFilter():base()
        {

        }
        public ApartmentLoadFilter(string InName, string nameSL, string nameDL) : 
            base(InName, new string[] { nameSL, nameDL })
        {

        }
        public override void Execute()
        {
            removeExtraOutputs();
            for (int i = 0; i < inputs.shapes.Count; i++)
            {
                Meshable m = inputs.shapes[i].meshable;
                string name = outputs.names[0];
                float depth = inputs.shapes[i].Size[2];
                if (depth < 13) name = outputs.names[0];
                else if (depth < 24) name = outputs.names[1];
                else name = "UnImplement";
                if (i >= outputs.shapes.Count)
                {
                    outputs.shapes.Add(ShapeObject.CreateBasic());
                }
                inputs.shapes[i].CloneTo(outputs.shapes[i]);
                outputs.shapes[i].name = name;
                outputs.shapes[i].parentRule = this;
            }
        }
    }
}
