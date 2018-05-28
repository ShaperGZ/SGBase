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
        public ApartmentLoadFilter(string InName, string nameSL, string nameDL, string nameCV) : 
            base(InName, new string[] { nameSL, nameDL, nameCV })
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
                else name = outputs.names[2];
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

    public class ResidentialLoadFilter : Rule
    {
        public ResidentialLoadFilter() : base()
        {

        }
        public ResidentialLoadFilter(string InName, string nameSL, string nameDL, string nameCV) :
            base(InName, new string[] { nameSL, nameDL, nameCV })
        {

        }
        public override void Execute()
        {
            removeExtraOutputs();
            for (int i = 0; i < inputs.shapes.Count; i++)
            {
                Meshable m = inputs.shapes[i].meshable;
                string name = outputs.names[0];
                float h = inputs.shapes[i].Size[1];
                float w = inputs.shapes[i].Size[0];

                if (w<10 && h <= 12) name = outputs.names[0];
                else if (h < 15) name = outputs.names[1];
                else name = outputs.names[2];

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


    public class OfficeFilter : Rule
    {
        public OfficeFilter() : base()
        {

        }
        public OfficeFilter(string InName, string nameSL, string nameDL, string nameCV) :
            base(InName, new string[] { nameSL, nameDL, nameCV })
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
                else if (depth < 32) name = outputs.names[1];
                else name = outputs.names[2];
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
