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
    public class DcpFace3 : Rule
    {
        public DcpFace3() : base("A", new string[] { "ASIDES","ATOP","ABOT" })
        {
            name = "DcpFace3";
        }
        public DcpFace3(string inName, string[] outNames) : base(inName, outNames)
        {
            name = "DcpFace3";
        }
        public override void Execute()
        {
            string nameSide = outputs.names[0];
            string nameTop = outputs.names[1];
            string nameBot = outputs.names[2];

            List<Meshable> top = new List<Meshable>();
            List<Meshable> bot = new List<Meshable>();
            List<Meshable> sides = new List<Meshable>();
            List<Meshable> outMeshables = new List<Meshable>();

            for (int i = 0; i < inputs.shapes.Count; i++)
            {
                //Debug.Log("i=" + i);
                ShapeObject so = inputs.shapes[i];
                if (so != null && so.meshable.GetType() == typeof(Extrusion))
                {
                    //Debug.Log("is extrusion");
                    Extrusion ext = (Extrusion)so.meshable;
                    bot.Add(ext.polygon);
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
            outMeshables.AddRange(bot);
            outMeshables.AddRange(sides);
            //Debug.Log("meshable.count=" + outMeshables.Count);

            int count = outMeshables.Count;
            int removeCount = outputs.shapes.Count - count;
            if (count > 0) removeOutputsByCount(removeCount);

            foreach (Meshable m in outMeshables)
            {
                //Debug.Log("m=" + m);
            }

            for (int i = 0; i < count; i++)
            {
                //if (outMeshables[i] == null) continue;
                if (i >= outputs.shapes.Count)
                {
                    ShapeObject nso = ShapeObject.CreateBasic();
                    nso.parentRule = this;
                    nso.step = this.step;
                    outputs.shapes.Add(nso);
                }
                Meshable m = outMeshables[i];
                if (m == null) throw new System.Exception("side is null at " + i);
                outputs.shapes[i].SetMeshable(m);
                if (top.Contains(m)) outputs.shapes[i].name = nameTop;
                else if (bot.Contains(m)) outputs.shapes[i].name = nameBot;
                else outputs.shapes[i].name = nameSide;
            }
        }
    }
    public class DcpFace4 : Rule
    {
        public DcpFace4() : base("A", new string[] { "AFRONTS","ASIDES", "ATOP", "ABOT" })
        {
            name = "DcpFace4";
        }
        public DcpFace4(string inName, string[] outNames) : base(inName, outNames)
        {
            name = "DcpFace4";
        }
        public override void Execute()
        {
            string nameFronts = outputs.names[0];
            string nameSide = outputs.names[1];
            string nameTop = outputs.names[2];
            string nameBot = outputs.names[3];

            List<Meshable> top = new List<Meshable>();
            List<Meshable> bot = new List<Meshable>();
            List<Meshable> sides = new List<Meshable>();
            List<Meshable> fronts = new List<Meshable>();
            List<Meshable> outMeshables = new List<Meshable>();

            for (int i = 0; i < inputs.shapes.Count; i++)
            {
                //Debug.Log("i=" + i);
                ShapeObject so = inputs.shapes[i];
                if (so != null && so.meshable.GetType() == typeof(Extrusion))
                {
                    //Debug.Log("is extrusion");
                    Extrusion ext = (Extrusion)so.meshable;
                    bot.Add(ext.polygon);
                    top.Add(ext.top);
                    BoundingBox bbox = ext.bbox;
                    Vector3 directF = bbox.vects[2].normalized;
                    foreach(Meshable m in ext.sides)
                    {
                        Vector3 v1 = (m.vertices[1] - m.vertices[0]).normalized;
                        Vector3 v2 = (m.vertices[3] - m.vertices[0]).normalized;
                        Vector3 n = Vector3.Cross(v1, v2);
                        Vector3 nn = n * -1;
                        //Debug.LogFormat("n={0}{3}, nn={1}{4}, vF={2} ", n,nn, directF,n==directF,nn==directF);
                        //if(Vector3.Distance(n,directF)<0.01f || Vector3.Distance(nn, directF) < 0.1f)
                        if(n==directF || nn == directF)
                        {
                            fronts.Add(m);
                        }
                        else
                        {
                            sides.Add(m);
                        }
                    }
                    if (top == null) throw new System.Exception("top is null");
                    for (int j = 0; j < ext.sides.Count; j++)
                    {
                        Meshable m = ext.sides[j];
                        if (m == null) throw new System.Exception("side is null at " + j);
                    }
                }

            }//for i

            outMeshables.AddRange(top);
            outMeshables.AddRange(bot);
            outMeshables.AddRange(sides);
            outMeshables.AddRange(fronts);
            //Debug.Log("meshable.count=" + outMeshables.Count);

            int count = outMeshables.Count;
            int removeCount = outputs.shapes.Count - count;
            if (count > 0) removeOutputsByCount(removeCount);

            foreach (Meshable m in outMeshables)
            {
                //Debug.Log("m=" + m);
            }

            for (int i = 0; i < count; i++)
            {
                //if (outMeshables[i] == null) continue;
                if (i >= outputs.shapes.Count)
                {
                    ShapeObject nso = ShapeObject.CreateBasic();
                    nso.parentRule = this;
                    nso.step = this.step;
                    outputs.shapes.Add(nso);
                }
                Meshable m = outMeshables[i];
                if (m == null) throw new System.Exception("side is null at " + i);
                outputs.shapes[i].SetMeshable(m);
                if (fronts.Contains(m)) outputs.shapes[i].name = nameFronts;
                else if (top.Contains(m)) outputs.shapes[i].name = nameTop;
                else if (bot.Contains(m)) outputs.shapes[i].name = nameBot;
                else outputs.shapes[i].name = nameSide;
            }
        }
    }
    public class DcpFace5 : Rule
    {
        public DcpFace5() : base("A", new string[] { "AFRONTS", "ABACKS","ASIDES", "ATOP", "ABOT" })
        {
            name = "DcpFace5";
        }
        public DcpFace5(string inName, string[] outNames) : base(inName, outNames)
        {
            name = "DcpFace5";
        }
        public override void Execute()
        {
            string nameFronts = outputs.names[0];
            string nameBacks = outputs.names[1];
            string nameSide = outputs.names[2];
            string nameTop = outputs.names[3];
            string nameBot = outputs.names[4];

            List<Meshable> top = new List<Meshable>();
            List<Meshable> bot = new List<Meshable>();
            List<Meshable> sides = new List<Meshable>();
            List<Meshable> fronts = new List<Meshable>();
            List<Meshable> backs = new List<Meshable>();
            List<Meshable> outMeshables = new List<Meshable>();

            for (int i = 0; i < inputs.shapes.Count; i++)
            {
                //Debug.Log("i=" + i);
                ShapeObject so = inputs.shapes[i];
                if (so != null && so.meshable.GetType() == typeof(Extrusion))
                {
                    //Debug.Log("is extrusion");
                    Extrusion ext = (Extrusion)so.meshable;
                    if (outputs.names[3] != "DELETE")
                        top.Add(ext.top);
                    if (outputs.names[4] != "DELETE")
                        bot.Add(ext.components[0]);
                    BoundingBox bbox = ext.bbox;
                    Vector3 directF = bbox.vects[2].normalized;
                    foreach (Meshable m in ext.sides)
                    {
                        Vector3 v1 = (m.vertices[1] - m.vertices[0]).normalized;
                        Vector3 v2 = (m.vertices[3] - m.vertices[0]).normalized;
                        Vector3 n = Vector3.Cross(v1, v2);
                        Vector3 nn = n * -1;
                        //Debug.LogFormat("n={0}{3}, nn={1}{4}, vF={2} ", n,nn, directF,n==directF,nn==directF);
                        //if(Vector3.Distance(n,directF)<0.01f || Vector3.Distance(nn, directF) < 0.1f)
                        if (n == directF )
                        {
                            if(outputs.names[0] != "DELETE")
                                fronts.Add(m);
                        }
                        else if (nn == directF)
                        {
                            if (outputs.names[1] != "DELETE")
                                backs.Add(m);
                        }
                        else
                        {
                            if (outputs.names[2] != "DELETE")
                                sides.Add(m);
                        }
                    }
                    //if (top == null) throw new System.Exception("top is null");
                    //for (int j = 0; j < ext.sides.Count; j++)
                    //{
                    //    Meshable m = ext.sides[j];
                    //    if (m == null) throw new System.Exception("side is null at " + j);
                    //}
                }

            }//for i

            outMeshables.AddRange(top);
            outMeshables.AddRange(bot);
            outMeshables.AddRange(sides);
            outMeshables.AddRange(fronts);
            //Debug.Log("meshable.count=" + outMeshables.Count);

            int count = outMeshables.Count;
            int removeCount = outputs.shapes.Count - count;
            if (count > 0) removeOutputsByCount(removeCount);

            foreach (Meshable m in outMeshables)
            {
                //Debug.Log("m=" + m);
            }

            for (int i = 0; i < count; i++)
            {
                //if (outMeshables[i] == null) continue;
                if (i >= outputs.shapes.Count)
                {
                    ShapeObject nso = ShapeObject.CreateBasic();
                    nso.parentRule = this;
                    nso.step = this.step;
                    outputs.shapes.Add(nso);
                }
                Meshable m = outMeshables[i];
                if (m == null) throw new System.Exception("side is null at " + i);
                outputs.shapes[i].SetMeshable(m);
                if (fronts.Contains(m)) outputs.shapes[i].name = nameFronts;
                else if (backs.Contains(m)) outputs.shapes[i].name = nameBacks;
                else if (top.Contains(m)) outputs.shapes[i].name = nameTop;
                else if (bot.Contains(m)) outputs.shapes[i].name = nameBot;
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
    

    public class ExtractFace : Rule
    {
        public string extract="NothingToExtract";
        public List<ShapeObject> extractedObjects = new List<ShapeObject>();
        public ExtractFace() : base()
        {
            
        }
        public ExtractFace(string[] inNames, string outname,string extract) : base()
        {
            inputs.names.AddRange(inNames);
            outputs.names.Add(outname);
            this.extract = extract;
        }
        public override void Execute()
        {
            string name = outputs.names[0];
            List<Meshable> outMeshables = new List<Meshable>();

            for (int i = 0; i < inputs.shapes.Count; i++)
            {
                //Debug.Log("i=" + i);
                ShapeObject so = inputs.shapes[i];
                if (so != null && so.meshable.GetType() == typeof(Extrusion))
                {
                    //Debug.Log("is extrusion");
                    Extrusion ext = (Extrusion)so.meshable;
                    if (extract == "TOP") outMeshables.Add(ext.top);
                    else if (extract == "BOT") outMeshables.Add(ext.bot);
                    else outMeshables.AddRange(ext.sides);
                }

            }//for i
            Debug.Log("outMeshables.Count=" + outMeshables.Count);
            int dif = extractedObjects.Count - outMeshables.Count;
            if (dif > 0)
            {
                for (int i = 0; i < dif; i++)
                {
                    int index = extractedObjects.Count - 1;
                    try
                    {
                        GameObject.Destroy(extractedObjects[index].gameObject);
                        
                    }
                    catch { }
                    extractedObjects.RemoveAt(index);
                }
            }
            
            

            for (int i = 0; i < outMeshables.Count; i++)
            {
                //if (outMeshables[i] == null) continue;
                if (i >= extractedObjects.Count)
                {
                    ShapeObject nso = ShapeObject.CreateBasic();
                    nso.parentRule = this;
                    nso.step = this.step;
                    nso.name = name;
                    extractedObjects.Add(nso);
                }
                Meshable m = outMeshables[i];
                if (m == null) throw new System.Exception("side is null at " + i);
                extractedObjects[i].SetMeshable(m);
                extractedObjects[i].GetComponent<MeshFilter>().mesh.RecalculateNormals();
                extractedObjects[i].name = name;
            }
            Debug.Log("extractObjects.Count=" + extractedObjects.Count);
            outputs.shapes.Clear();
            outputs.shapes.AddRange(inputs.shapes.ToArray());
            outputs.shapes.AddRange(extractedObjects.ToArray());
            
        }

    }


}



