using System.Collections;
using System.Collections.Specialized;
using System.Collections.Generic;
using UnityEngine;
using SGGeometry;
using SGCore;

namespace Rules
{
    public class AggBasic : Rule
    {
        protected string prefabPath = "Components\\suinit";
        public AggBasic(string prefabPath=null)
        {
            if(prefabPath!=null)
                this.prefabPath = prefabPath;
            name = "AggBasic";
        }
        public AggBasic(string inName, string outName, int width, int height, string prefabPath=null) : base(inName, outName)
        {
            if (prefabPath != null)
                this.prefabPath = prefabPath;
            name = "AggBasic";
            SetParam("DivisionSize", 0, width);
            SetParam("DivisionSize", 1, height);
        }
        public override void Execute()
        {
            removeOutputsByCount(outputs.shapes.Count);
            float stepW = GetParamVal("DivisionSize", 0);
            float stepH = GetParamVal("DivisionSize", 1);
            int counter = 0;
            foreach (ShapeObject so in inputs.shapes)
            {

                Vector3 v = (so.meshable.vertices[1] - so.meshable.vertices[0]).normalized;
                BoundingBox bbox = BoundingBox.CreateFromPoints(so.meshable.vertices, v);
                counter = GenerateObjects(null, bbox, stepW, stepH, counter);
            }
            int dif = outputs.shapes.Count - counter;
            if (dif > 0)
                removeOutputsByCount(dif);
        }

        private int GenerateObjects(GameObject prefab, BoundingBox bbox, float stepW, float stepH, int totalObjectCount)
        {
            if (prefab == null)
            {
                prefab = Resources.Load(prefabPath) as GameObject;
            }
            Vector3 org = bbox.position;
            float totalW = bbox.size[0];
            float totalH = bbox.size[1];

            int countW = Mathf.RoundToInt(totalW / stepW);
            int countH = Mathf.RoundToInt(totalH / stepH);

            float astepW = totalW / (float)countW;
            float astepH = totalH / (float)countH;


            Vector3 vectW = bbox.vects[0] * astepW;
            Vector3 vectH = bbox.vects[1] * astepH;
            Vector3 vectD = bbox.vects[2] * 1;

            for (int i = 0; i < countH; i++)
            {
                for (int j = 0; j < countW; j++)
                {
                    Vector3 bp = org + (vectW * j) + (vectH * i);
                    Vector3[] ptsi = new Vector3[2];
                    ptsi[0] = bp;
                    ptsi[1] = ptsi[0] + vectW + vectH + vectD;
                    BoundingBox bboxi = BoundingBox.CreateFromPoints(ptsi, vectW);

                    if (totalObjectCount >= outputs.shapes.Count)
                    {
                        ShapeObject nso = ShapeObject.CreateBasic(prefab, true);
                        nso.parentRule = this;
                        outputs.shapes.Add(nso);
                    }
                    ShapeObject shpo = outputs.shapes[totalObjectCount];
                    shpo.ConformToBBoxTransform(bboxi);
                    totalObjectCount += 1;
                }
            }
            return totalObjectCount;
        }
        public override OrderedDictionary DefaultParam()
        {
            OrderedDictionary dict = new OrderedDictionary();
            ParameterGroup pg1 = new ParameterGroup();
            ParameterGroup pg2 = new ParameterGroup();
            List<ParameterGroup> outParamGroups = new List<ParameterGroup>();
            outParamGroups.Add(pg1);
            outParamGroups.Add(pg2);

            dict["DivisionSize"] = pg1;
            pg1.Add(new Parameter(3f, 1, 12, 1));
            pg1.Add(new Parameter(4f, 1, 12, 1));

            return dict;
        }

    }

    public class AggCW01 : AggBasicSimp
    {
        public AggCW01()
        {
            name = "AggCW01";
            prefabPath = "Components\\comp1u";
        }
        public AggCW01(string inName, string outName, int width, int height) : base(inName, outName, width, height)
        {
            name = "AggCW01";
            prefabPath = "Components\\comp1u";
            SetParam("DivisionSize", 0, width);
            SetParam("DivisionSize", 1, height);
        }
    }
    public class AggCW02 : AggBasicSimp
    {
        public AggCW02()
        {
            name = "AggCW02";
            prefabPath = "Components\\comp2u";
        }
        public AggCW02(string inName, string outName, int width, int height) : base(inName, outName, width, height)
        {
            name = "AggCW02";
            prefabPath = "Components\\comp2u";
            SetParam("DivisionSize", 0, width);
            SetParam("DivisionSize", 1, height);
        }

    }
    public class AggSD01 : AggBasicSimp
    {
        public AggSD01()
        {
            name = "AggCW01";
            prefabPath = "Components\\comp2";
        }
        public AggSD01(string inName, string outName, int width, int height) : base(inName, outName, width, height)
        {
            name = "AggCW01";
            prefabPath = "Components\\comp2";
            SetParam("DivisionSize", 0, width);
            SetParam("DivisionSize", 1, height);
        }

    }
    public class AggBasicSimp : Rule
    {
        protected string prefabPath = "Components\\suinit";
        public AggBasicSimp()
        {
            name = "AggBasic";
        }
        public AggBasicSimp(string inName, string outName, int width, int height) : base(inName, outName)
        {
            name = "AggBasic";
            SetParam("DivisionSize", 0, width);
            SetParam("DivisionSize", 1, height);
        }
        public override void Execute()
        {
            removeOutputsByCount(outputs.shapes.Count);
            float stepW = GetParamVal("DivisionSize", 0);
            float stepH = GetParamVal("DivisionSize", 1);
            int counter = 0;
            foreach (ShapeObject so in inputs.shapes)
            {
                
                Vector3 v = (so.meshable.vertices[1] - so.meshable.vertices[0]).normalized;
                BoundingBox bbox = BoundingBox.CreateFromPoints(so.meshable.vertices, v);
                counter = GenerateObjects(null, bbox, stepW, stepH,counter);
            }
            int dif = outputs.shapes.Count - counter;
            if(dif>0)
                removeOutputsByCount(dif);
        }

        private int GenerateObjects(GameObject prefab, BoundingBox bbox, float stepW, float stepH, int totalObjectCount)
        {
            if (prefab == null)
            {
                prefab = Resources.Load(prefabPath) as GameObject;
            }
            Vector3 org = bbox.position;
            float totalW = bbox.size[0];
            float totalH = bbox.size[1];

            

            int countW = Mathf.RoundToInt(totalW / stepW);
            int countH = Mathf.RoundToInt(totalH / stepH);

            if (countW < 1) countW = 1;
            if (countH < 1) countH = 1;

            float astepW = totalW / (float)countW;
            float astepH = totalH / (float)countH;


            Vector3 vectW = bbox.vects[0] * astepW;
            Vector3 vectH = bbox.vects[1] * astepH;
            Vector3 vectD = bbox.vects[2] * 1;

            for (int i = 0; i < countH; i++)
            {
                for (int j = 0; j < countW; j++)
                {
                    Vector3 bp = org + (vectW * j) + (vectH * i);
                    Vector3[] ptsi = new Vector3[2];
                    ptsi[0] = bp;
                    ptsi[1] = ptsi[0] + vectW + vectH + vectD;
                    BoundingBox bboxi = BoundingBox.CreateFromPoints(ptsi, vectW);

                    if (totalObjectCount >= outputs.shapes.Count)
                    {
                        ShapeObject nso = ShapeObject.CreateBasic(prefab,true);
                        nso.parentRule = this;
                        outputs.shapes.Add(nso);
                    }
                    ShapeObject shpo = outputs.shapes[totalObjectCount];
                    shpo.ConformToBBoxTransform(bboxi);
                    totalObjectCount += 1;
                }
            }
            return totalObjectCount;
        }
        public override OrderedDictionary DefaultParam()
        {
            OrderedDictionary dict = new OrderedDictionary();
            ParameterGroup pg1 = new ParameterGroup();
            ParameterGroup pg2 = new ParameterGroup();
            List<ParameterGroup> outParamGroups = new List<ParameterGroup>();
            outParamGroups.Add(pg1);
            outParamGroups.Add(pg2);

            dict["DivisionSize"] = pg1;
            pg1.Add(new Parameter(3f, 1, 12, 1));
            pg1.Add(new Parameter(4f, 1, 12, 1));

            return dict;
        }

    }
}
