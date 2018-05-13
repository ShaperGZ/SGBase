using System.Collections;
using System.Collections.Specialized;
using System.Collections.Generic;
using UnityEngine;
using SGGeometry;
using SGCore;

namespace Rules
{
    public class DcpFace2 : Rule
    {
        public DcpFace2():base("A",new string[] { "ATOP","ASIDES"})
        {
            name = "DcpFace2";
        }
        public DcpFace2(string inName, string[] outNames) : base(inName, outNames)
        {
            name = "DcpFace2";
        }
        public override void Execute()
        {
            string nameSide = outputs.names[0];
            string nameTop = outputs.names[1];

            List<Meshable> top = new List<Meshable>();
            List<Meshable> sides = new List<Meshable>();
            List<Meshable> outMeshables = new List<Meshable>();

            for(int i = 0; i < inputs.shapes.Count; i++)
            {
                //Debug.Log("i=" + i);
                ShapeObject so = inputs.shapes[i];
                if(so!=null && so.meshable.GetType() == typeof(Extrusion))
                {
                    //Debug.Log("is extrusion");
                    Extrusion ext = (Extrusion)so.meshable;
                    top.Add(ext.top);
                    sides.AddRange(ext.sides);
                    if (top == null) throw new System.Exception("top is null");
                    for (int j = 0; j < ext.sides.Count; j++)
                    {
                        Meshable m = ext.sides[j];
                        if (m == null) throw new System.Exception("side is null at " + j);
                    }
                }

            }//for i

            outMeshables.AddRange(top);
            outMeshables.AddRange(sides);
            //Debug.Log("meshable.count=" + outMeshables.Count);

            int count = outMeshables.Count;
            int removeCount= outputs.shapes.Count-count;
            if (count > 0) removeOutputsByCount(removeCount);

            foreach(Meshable m in outMeshables)
            {
                //Debug.Log("m=" + m);
            }

            for(int i = 0; i < count; i++)
            {
                //if (outMeshables[i] == null) continue;
                if (i >= outputs.shapes.Count)
                {
                    ShapeObject nso = ShapeObject.CreateBasic();
                    nso.parentRule = this;
                    nso.step = this.step;
                    outputs.shapes.Add(nso);
                }
                Meshable m=outMeshables[i];
                if (m == null) throw new System.Exception("side is null at " + i);
                outputs.shapes[i].SetMeshable(m);
                if (top.Contains(m)) outputs.shapes[i].name = nameTop;
                else outputs.shapes[i].name = nameSide;
            }
        }
    }

    public class DcpA : Rule
    {
        public DcpA() : base()
        {
            inputs.names.Add("A");
            ((ParameterGroup)paramGroups["Width"]).parameters[0].Value = 6;
            ((ParameterGroup)paramGroups["Height"]).parameters[0].Value = 3;
        }
        public DcpA(string inName, int w, int h) : base(inName, "Terminal")
        {
            ((ParameterGroup)paramGroups["Width"]).parameters[0].Value = w;
            ((ParameterGroup)paramGroups["Height"]).parameters[0].Value = h;

        }
        public override void Execute()
        {
            int w = (int)(((ParameterGroup)paramGroups["Width"]).parameters[0].Value);
            int h = (int)(((ParameterGroup)paramGroups["Height"]).parameters[0].Value);
            //Debug.LogFormat("inputs.shapes.Count={0}", inputs.shapes.Count);
            if (inputs.shapes.Count <= 0) return;
            List<ShapeObject> outSos = new List<ShapeObject>();

            for (int i = 0; i < inputs.shapes.Count; i++)
            {
                ShapeObject so = inputs.shapes[i].Clone();
                so.parentRule = inputs.shapes[i].parentRule;
                outSos.Add(so);

                //Debug.LogFormat("mesable type={0}", so.meshable.GetType());

                ShapeObjectM som = ShapeObjectM.CreateSchemeA((CompositMeshable)so.meshable);
                outSos.Add(som);
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

    //public class DubTop : Rule
    //{
    //    List<ShapeObject> additional = new List<ShapeObject>();

    //    //top, east-west, south-north
    //    public DubTop(string inName, string[] outNames = null)
    //    {
    //        if (outNames == null)
    //        {
    //            outNames = new string[] { "faceTop,faceSides" };
    //        }
    //        inputs.names.Add(inName);
    //        outputs.names.AddRange(outNames);
    //    }
    //    public override void Execute()
    //    {
    //        outMeshables.Clear();


    //        List<Meshable> tops = new List<Meshable>();
    //        List<Meshable> sides = new List<Meshable>();
    //        foreach (ShapeObject so in inputs.shapes)
    //        {
    //            if (so.meshable.GetType() == typeof(SGGeometry.Extrusion))
    //            {
    //                Extrusion ext = (Extrusion)(so.meshable);
    //                int last = ext.components.Count - 1;

    //                tops.Add(ext.components[last]);
    //            }
    //        }
    //        //delete extra ouputs
    //        int dif = outputs.shapes.Count - (tops.Count+inputs.shapes.Count);
    //        removeOutputsByCount(dif);

    //        //assign outputs
    //        for (int i = 0; i < inputs.shapes.Count; i++)
    //        {
    //            Meshable m = inputs.shapes[i].meshable;
    //            if (i < outputs.shapes.Count)
    //            {
    //                outputs.shapes[i].SetMeshable(m);
    //            }
    //            else
    //            {
    //                ShapeObject o = ShapeObject.CreateMeshable(m);
    //                o.parentRule = this;
    //                o.step = step;
    //                o.name = outputs.names[0];
    //                outputs.shapes.Add(o);
    //            }

    //        }


    //        for (int i = 0; i < tops.Count; i++)
    //        {
    //            int index = i + inputs.shapes.Count;
    //            Meshable m = tops[i];
    //            if (index < outputs.shapes.Count)
    //            {
    //                outputs.shapes[index].SetMeshable(m);
    //            }
    //            else
    //            {
    //                ShapeObject o = ShapeObject.CreateMeshable(m);
    //                o.parentRule = this;
    //                o.step = step;
    //                o.name = outputs.names[0];
    //                outputs.shapes.Add(o);
    //            }

    //        }
    //    }
    //}

    //public class DcpFace2:Rule
    //{
    //    //top, east-west, south-north
    //    public DcpFace2(string inName, string[] outNames = null)
    //    {
    //        if (outNames == null)
    //        {
    //            outNames = new string[] { "faceTop,faceSides" };
    //        }
    //        inputs.names.Add(inName);
    //        outputs.names.AddRange(outNames);

    //    }
    //    public override void Execute()
    //    {
    //        outMeshables.Clear();
    //        List<Meshable> tops = new List<Meshable>();
    //        List<Meshable> sides = new List<Meshable>();
    //        foreach(ShapeObject so in inputs.shapes)
    //        {
    //            if (so.meshable.GetType() == typeof(SGGeometry.Extrusion))
    //            {
    //                Extrusion ext = (Extrusion)(so.meshable);
    //                int last = ext.components.Count-1;

    //                tops.Add(ext.components[last]);
    //            }
    //        }

    //        //delete extra ouputs
    //        int dif = outputs.shapes.Count - tops.Count;
    //        removeOutputsByCount(dif);

    //        //assign outputs
    //        for (int i = 0; i < tops.Count; i++)
    //        {
    //            Meshable m = tops[i];
    //            if (i < outputs.shapes.Count)
    //            {
    //                outputs.shapes[i].SetMeshable(m);
    //            }
    //            else
    //            {
    //                ShapeObject o = ShapeObject.CreateMeshable(m);
    //                o.parentRule = this;
    //                o.step = step;
    //                o.name = outputs.names[0];
    //                outputs.shapes.Add(o);
    //            }

    //        }
    //    }
    //}


}



