using System.Collections;
using System.Collections.Specialized;
using System.Collections.Generic;
using UnityEngine;
using SGGeometry;
using SGCore;

namespace Rules
{
    public class DcpA : Rule
    {
        public DcpA() : base()
        {
            inputs.names.Add("A");
            ((ParameterGroup)paramGroups["Width"]).parameters[0].value = 6;
            ((ParameterGroup)paramGroups["Height"]).parameters[0].value = 3;
        }
        public DcpA(string inName, int w, int h) : base(inName, "Terminal")
        {
            ((ParameterGroup)paramGroups["Width"]).parameters[0].value = w;
            ((ParameterGroup)paramGroups["Height"]).parameters[0].value = h;

        }
        public override void Execute()
        {
            int w = (int)(((ParameterGroup)paramGroups["Width"]).parameters[0].value);
            int h = (int)(((ParameterGroup)paramGroups["Height"]).parameters[0].value);
            Debug.LogFormat("inputs.shapes.Count={0}", inputs.shapes.Count);
            if (inputs.shapes.Count <= 0) return;
            List<ShapeObject> outSos = new List<ShapeObject>();
            
            for (int i = 0; i < inputs.shapes.Count; i++)
            {
                //try
                //{
                ShapeObject so = inputs.shapes[i].Clone();
                so.parentRule = inputs.shapes[i].parentRule;
                outSos.Add(so);

                Debug.LogFormat("mesable type={0}", so.meshable.GetType());

                ShapeObjectM som = ShapeObjectM.CreateSchemeA((CompositMeshable)so.meshable);
                outSos.Add(som);
                //}
                //catch
                //{
                //    ShapeObject so = inputs.shapes[i];
                //    Debug.LogFormat("FAILED: mesable type={0}", so.meshable.GetType());
                //}


            }
            if (outputs.shapes != null)
                foreach (ShapeObject o in outputs.shapes)
                {
                    GameObject.Destroy(o.gameObject);
                }
            outputs.shapes = outSos;
        }
        public override OrderedDictionary DefaultParam()
        {
            OrderedDictionary dict = new OrderedDictionary();
            ParameterGroup pg1 = new ParameterGroup();
            ParameterGroup pg2 = new ParameterGroup();
            dict.Add("Width", pg1);
            pg1.Add(new Parameter(6, 3, 12, 1));
            dict.Add("Height", pg2);
            pg2.Add(new Parameter(3, 3, 12, 0.5f));

            return dict;
        }
    }
}



