using System.Collections.Specialized;
using System.Collections.Generic;
using UnityEngine;
using SGCore;
using SGGeometry;

namespace Rules
{
    public class Bisect:Rule{

        Plane cutPlane;
        public Bisect():base()
        {
            inputs.names.Add("A");
            outputs.names.Add("B");
            outputs.names.Add("C");
            
        }
        public Bisect(string inName, string[] outNames, float d, int axis):base(inName,outNames)
        {
            ((ParameterGroup)paramGroups["Position"]).parameters[0].value = d;
            ((ParameterGroup)paramGroups["Axis"]).parameters[0].value = axis;
        }
        public static List<Meshable> SplitByPlane(Meshable mb, Plane pln)
        {
            List<Meshable> outs = new List<Meshable>();
            //get the splited meshables
            Meshable[] temp = new Meshable[0];
            temp = mb.SplitByPlane(pln);

            for (int i = 0; i < temp.Length; i++)
            {
                if (temp[i] != null)
                {
                    //Debug.Log("---------------------->creating boundong box");
                    temp[i].bbox =BoundingBox.CreateFromPoints(temp[i].vertices,mb.bbox);
                    outs.Add(temp[i]);
                }
            }
            return outs;
        }
        public virtual Plane GetPlane(ShapeObject so, float d, int axis)
        {
            Vector3 normal = so.Vects[axis];
            Vector3 v = normal * so.Size[axis] * d;
            Vector3 org = so.transform.position + v;
            Plane pln = new Plane(normal, org);
            return pln;
        }
        public override List<Meshable> ExecuteShape(ShapeObject so)
        {
            /////////////////
            //get parameters
            /////////////////
            float d = ((ParameterGroup)paramGroups["Position"]).parameters[0].value;
            int axis = (int)((ParameterGroup)paramGroups["Axis"]).parameters[0].value;
            List<Meshable> outs = new List<Meshable>();

            /////////////////
            //get split plane
            /////////////////
            Plane pln = GetPlane(so, d, axis);


            outs = SplitByPlane(so.meshable, pln);
            Vector3 direct = so.Vects[0];
            
            AssignNames(outs);
            return outs;
            
        }
        public override OrderedDictionary DefaultParam()
        {
            OrderedDictionary dict = new OrderedDictionary();
            ParameterGroup pg1 = new ParameterGroup();
            ParameterGroup pg2 = new ParameterGroup();

            dict.Add("Axis", pg1);
            pg1.Add(new Parameter(0, 0, 2, 1));
            dict.Add("Position",pg2);
            pg2.Add(new Parameter(0.4f));
            
            return dict;
        }
    }
    public class BisectLength : Bisect
    {
        public BisectLength() : base() { }
        public BisectLength(string inName, string[] outNames, float d, int axis):base(inName,outNames,d,axis)
        {
        }
        public override Plane GetPlane(ShapeObject so, float d, int axis)
        {
            Vector3 normal = so.Vects[axis];
            Vector3 v = normal * d;
            Vector3 org = so.transform.position + v;
            Plane pln = new Plane(normal, org);
            return pln;
        }
    }
    public class BisectMirror : Grammar
    {
        public BisectMirror():base()
        {
            SetRules();
            name = "BisectMirror";
        }
        public BisectMirror(string inName, string[] outNames, float d, int axis) : base(inName, outNames)
        {
            ((ParameterGroup)paramGroups["Position"]).parameters[0].value = d;
            ((ParameterGroup)paramGroups["Axis"]).parameters[0].value = axis;
            name = "BisectMirror";
            SetRules();
        }
        void SetRules()
        {
            string on1 = outputs.names[0];
            string onM = outputs.names[1] + "toBeMirrored";
            string on2 = outputs.names[1];

            float d = ((ParameterGroup)paramGroups["Position"]).parameters[0].value;
            int axis = (int)((ParameterGroup)paramGroups["Axis"]).parameters[0].value;
            Rule rule1 = new Bisect(inputs.names[0], new string[] { on1, onM }, d, axis);
            Rule rule2 = new PivotMirror(onM, on2, axis);
            AddRule(rule1, false);
            AddRule(rule2, false);

            rule1.paramGroups["Position"] = paramGroups["Position"];
            rule2.paramGroups["Axis"] = paramGroups["Axis"];
        }
        public override void Execute()
        {
            //update params before execution
            //((ParameterGroup)subNodes[0].paramGroups["Position"]).parameters[0].value = ((ParameterGroup)paramGroups["Position"]).parameters[0].value;
            //((ParameterGroup)subNodes[1].paramGroups["Axis"]).parameters[0].value = ((ParameterGroup)paramGroups["Axis"]).parameters[0].value;
            base.Execute();
        }
        public override OrderedDictionary DefaultParam()
        {
            OrderedDictionary dict = new OrderedDictionary();
            ParameterGroup pg1 = new ParameterGroup();
            ParameterGroup pg2 = new ParameterGroup();

            dict.Add("Axis", pg1);
            pg1.Add(new Parameter(0, 0, 2, 1));
            dict.Add("Position", pg2);
            pg2.Add(new Parameter(0.4f));

            return dict;
        }
    }
    public class Divide : Rule
    {

        Plane cutPlane;
        public Divide() : base()
        {
            inputs.names.Add("A");
            outputs.names.Add("B");
            outputs.names.Add("C");

        }
        public Divide(string inName, string[] outNames, float[] ds, int axis) : base(inName, outNames)
        {
            for (int i = 0; i < ds.Length; i++)
            {
                float d = ds[i];
                ParameterGroup pg = (ParameterGroup)paramGroups["Position"];
                if (i < pg.parameters.Count)
                {
                    pg.parameters[i].value = d;
                }
                else
                {
                    pg.parameters.Add(new Parameter(d, 0, 1, 0.01f));
                }
            }
            
            ((ParameterGroup)paramGroups["Axis"]).parameters[0].value = axis;

        }
        public virtual List<float> GetDivs(List<Parameter> pms, float max)
        {
            float total = 0;
            List<float> outDivs = new List<float>();
            foreach (Parameter p in pms)
            {
                if (total >= max) break;
                float rest = max - total;
                float val = p.value * max;
                if (val > rest) val = rest;
                outDivs.Add(val);
                total += val;
            }
            return outDivs;
        }
        public override List<Meshable> ExecuteShape(ShapeObject so)
        {
            Debug.Log("executing so");
            List<Meshable> outs = new List<Meshable>();
            ////////////////
            //get paramters
            ////////////////
            int axis = (int)((ParameterGroup)paramGroups["Axis"]).parameters[0].value;
            float max = so.Size[axis];

            ///////////////////////////
            //sitribute division points
            ///////////////////////////
            List<float> divs = GetDivs(((ParameterGroup)paramGroups["Position"]).parameters, max);

            //tout is the remaining part in division
            //tout will be updated at each iteration
            //each iteration divides tout, and stops when there are no more left.
            List<Meshable> touts = new List<Meshable>();
            Vector3 org = so.transform.position;
            int counter = 0;
            while (counter==0 || touts.Count > 1)
            {
                if (counter >= divs.Count)
                {
                    Debug.LogWarning("counter>divs.Count");
                    break;
                }
                //Debug.Log(counter+" div="+divs[counter]);
                //get split plane
                Vector3 normal = so.Vects[axis];
                Vector3 v = normal *  divs[counter];
                org += v;
                Plane pln = new Plane(normal, org);

                if (counter == 0)
                    touts = Rules.Bisect.SplitByPlane(so.meshable, pln);
                else
                    touts = Rules.Bisect.SplitByPlane(touts[1], pln);
               
                outs.Add(touts[0]);
               
                counter++;
            }
            
            AssignNames(outs);
            return outs;

        }
        public override OrderedDictionary DefaultParam()
        {
            OrderedDictionary dict = new OrderedDictionary();
            ParameterGroup pg1 = new ParameterGroup();
            ParameterGroup pg2 = new ParameterGroup();
            List<ParameterGroup> outParamGroups = new List<ParameterGroup>();
            outParamGroups.Add(pg1);
            outParamGroups.Add(pg2);

            dict["Position"] = pg1;
            pg1.Add(new Parameter(0.3f));
            pg1.Add(new Parameter(0.2f));
            pg1.Add(new Parameter(0.5f));
            dict["Axis"] = pg2;
            pg2.Add(new Parameter(0, 0, 2, 1));

            return dict;
        }
    }
    public class DivideTo : Divide
    {
        Plane cutPlane;
        public DivideTo() : base()
        {
            inputs.names.Add("A");
            outputs.names.Add("A");
        }
        public DivideTo(string inName, string outName, float ds, int axis) : base(inName, new string[] { outName }, new float[]{ ds}, axis)
        {
        }
        public DivideTo(string inName, string[] outNames, float ds, int axis) : base(inName, outNames, new float[]{ ds},axis)
        {
        }
        public override List<float> GetDivs(List<Parameter> pms, float max)
        {
            Debug.Log("cal culating GetDivs");
            float d = pms[0].value;
            float count = Mathf.Round(max / d);
            d = max / count;
            Debug.Log(string.Format("cal culating GetDivs d={0}, max={1}, count={2}", d,max,count));
            List<float> outDivs = new List<float>();
            for (int i = 0; i < count; i++)
            {
                outDivs.Add(d);
                Debug.Log(i + " adding" + d);
            }
            return outDivs;
        }
        public override OrderedDictionary DefaultParam()
        {
            OrderedDictionary dict = new OrderedDictionary();
            ParameterGroup pg1 = new ParameterGroup();
            ParameterGroup pg2 = new ParameterGroup();
            List<ParameterGroup> outParamGroups = new List<ParameterGroup>();
            outParamGroups.Add(pg1);
            outParamGroups.Add(pg2);

            dict["Position"] = pg1;
            pg1.Add(new Parameter(10,5,100,1));
            dict["Axis"] = pg2;
            pg2.Add(new Parameter(0, 0, 2, 1));

            return dict;
        }

    }
    public class Scale : Rule
    {

        Plane cutPlane;
        public Scale() : base()
        {
            inputs.names.Add("A");
            outputs.names.Add("A");
            paramGroups = DefaultParam();
        }
        public Scale(string inName, string outName, float d, int axis) : base(inName, new string[] { outName })
        {
            ((ParameterGroup)paramGroups["Position"]).parameters[0].value = d;
            ((ParameterGroup)paramGroups["Axis"]).parameters[0].value = axis;
        }
        public override List<Meshable> ExecuteShape(ShapeObject so)
        {
            float d = ((ParameterGroup)paramGroups["Position"]).parameters[0].value;
            int axis = (int)((ParameterGroup)paramGroups["Axis"]).parameters[0].value;

            //get scale
            Vector3 scale = new Vector3(1, 1, 1);
            scale[axis] = d;
            Vector3[] vects = so.Vects;
            Vector3 origin = so.transform.position;
            
            //get the splited meshables
            Meshable mb = so.meshable;
            Meshable scaledMb = mb.Scale(scale, vects, origin, true);
            //TODO: just scale the bbox
            scaledMb.bbox = BoundingBox.CreateFromPoints(scaledMb.vertices, so.meshable.bbox);
            //outMeshables.Add(scaledMb);
            List<Meshable> outs = new List<Meshable>();
            outs.Add(scaledMb);
            AssignNames(outs);
            //AssignNames(outMeshables.ToArray());
            return outs;

            //Rule.Execute() will take care of the outMeshables
        }
        public override OrderedDictionary DefaultParam()
        {
            OrderedDictionary dict = new OrderedDictionary();
            ParameterGroup pg1 = new ParameterGroup();
            ParameterGroup pg2 = new ParameterGroup();
            List<ParameterGroup> outParamGroups = new List<ParameterGroup>();
            outParamGroups.Add(pg1);
            outParamGroups.Add(pg2);

            dict["Axis"] = pg1;
            pg1.Add(new Parameter(1, 0, 2, 1));
            dict["Position"] = pg2;
            pg2.Add(new Parameter(2f, 0.2f, 10f, 0.01f));
            
            return dict;
        }
    }
    public class Scale3D : Rule
    {
        Plane cutPlane;
        public Scale3D() : base()
        {
            inputs.names.Add("A");
            outputs.names.Add("A");
            paramGroups = DefaultParam();
        }
        public Scale3D(string inName, string outName, Vector3 scale, Vector2? randRange=null) : base(inName, new string[] { outName })
        {
            ((ParameterGroup)paramGroups["Scale"]).parameters[0].value = scale[0];
            ((ParameterGroup)paramGroups["Scale"]).parameters[1].value = scale[1];
            ((ParameterGroup)paramGroups["Scale"]).parameters[2].value = scale[2];
            this.randRange = randRange;

        }
        public override List<Meshable> ExecuteShape(ShapeObject so)
        {
            float dx = ((ParameterGroup)paramGroups["Scale"]).parameters[0].value;
            float dy = ((ParameterGroup)paramGroups["Scale"]).parameters[1].value;
            float dz = ((ParameterGroup)paramGroups["Scale"]).parameters[2].value;
            
            //get scale
            Vector3 scale = new Vector3(dx,dy,dz);

            if (randRange.HasValue)
            {
                float min = randRange.Value[0];
                float max = randRange.Value[1];
                for (int i = 0; i < 3; i++)
                {
                    scale[i] = scale[i] * (1+Random.Range(min, max));
                }
            }

            Vector3[] vects = so.Vects;
            Vector3 origin = so.transform.position;

            //get the splited meshables
            Meshable mb = so.meshable;
            Meshable scaledMb = mb.Scale(scale, vects, origin, true);
            //TODO: just scale the bbox
            scaledMb.bbox = BoundingBox.CreateFromPoints(scaledMb.vertices, so.meshable.bbox);
            //outMeshables.Add(scaledMb);
            List<Meshable> outs = new List<Meshable>();
            outs.Add(scaledMb);
            AssignNames(outs);
            //AssignNames(outMeshables.ToArray());
            return outs;

            //Rule.Execute() will take care of the outMeshables
        }
        public override OrderedDictionary DefaultParam()
        {
            OrderedDictionary dict = new OrderedDictionary();
            ParameterGroup pg1 = new ParameterGroup();
            ParameterGroup pg2 = new ParameterGroup();
            List<ParameterGroup> outParamGroups = new List<ParameterGroup>();
            outParamGroups.Add(pg1);
            outParamGroups.Add(pg2);

            dict["Scale"] = pg1;
            pg1.Add(new Parameter(1f, 0.2f, 10f, 0.01f));
            pg1.Add(new Parameter(1f, 0.2f, 10f, 0.01f));
            pg1.Add(new Parameter(1f, 0.2f, 10f, 0.01f));

            return dict;
        }
    }
    public class Size3D : Rule
    {
        Plane cutPlane;
        public Size3D() : base()
        {
            inputs.names.Add("A");
            outputs.names.Add("A");
            paramGroups = DefaultParam();
        }
        public Size3D(string inName, string outName, Vector3 size, Vector2? randRange = null) : base(inName, new string[] { outName })
        {
            paramGroups = DefaultParam();
            ((ParameterGroup)paramGroups["Size"]).parameters[0].value = size[0];
            ((ParameterGroup)paramGroups["Size"]).parameters[1].value = size[1];
            ((ParameterGroup)paramGroups["Size"]).parameters[2].value = size[2];
            this.randRange = randRange;

        }
        public override List<Meshable> ExecuteShape(ShapeObject so)
        {
            float dx = ((ParameterGroup)paramGroups["Size"]).parameters[0].value;
            float dy = ((ParameterGroup)paramGroups["Size"]).parameters[1].value;
            float dz = ((ParameterGroup)paramGroups["Size"]).parameters[2].value;

            //get Size
            Vector3 size = new Vector3(dx, dy, dz);

            if (randRange.HasValue)
            {
                float min = randRange.Value[0];
                float max = randRange.Value[1];
                for (int i = 0; i < 3; i++)
                {
                    size[i] = size[i] * (1 + Random.Range(min, max));
                }
            }

            Vector3 scale = new Vector3(1, 1, 1);
            Vector3 soSize = so.transform.localScale;
            for (int i = 0; i < 3; i++)
            {
                scale[i] = size[i] / soSize[i];
            }
            
            Vector3[] vects = so.Vects;
            Vector3 origin = so.transform.position;

            //get the splited meshables
            Meshable mb = so.meshable;
            Meshable scaledMb = mb.Scale(scale, vects, origin, true);
            //TODO: just scale the bbox
            scaledMb.bbox = BoundingBox.CreateFromPoints(scaledMb.vertices, so.meshable.bbox);
            //outMeshables.Add(scaledMb);
            List<Meshable> outs = new List<Meshable>();
            outs.Add(scaledMb);
            AssignNames(outs);
            //AssignNames(outMeshables.ToArray());
            return outs;

            //Rule.Execute() will take care of the outMeshables
        }
        public override OrderedDictionary DefaultParam()
        {
            OrderedDictionary dict = new OrderedDictionary();
            ParameterGroup pg1 = new ParameterGroup();
            ParameterGroup pg2 = new ParameterGroup();
            List<ParameterGroup> outParamGroups = new List<ParameterGroup>();
            outParamGroups.Add(pg1);
            outParamGroups.Add(pg2);

            dict["Size"] = pg1;
            pg1.Add(new Parameter(30f, 30f, 80f, 0.01f));
            pg1.Add(new Parameter(18f, 3f, 100f, 0.01f));
            pg1.Add(new Parameter(8f, 8f, 80f, 0.01f));

            return dict;
        }
    }



    //delete this
    public class DivideSurface : Rule
    {
        public DivideSurface()
        {
            inputs.names.Add("A");
            outputs.names.Add("FA");
        }
        public DivideSurface(string inName, string OutName, float w, float h) : base(inName, new string[] { OutName })
        {
            paramGroups = DefaultParam();
            ((ParameterGroup)paramGroups["dim width"]).parameters[0].value = w;
            ((ParameterGroup)paramGroups["dim height"]).parameters[0].value = h;
        }

        public override OrderedDictionary DefaultParam()
        {
            OrderedDictionary dict = new OrderedDictionary();
            ParameterGroup pg1 = new ParameterGroup();
            ParameterGroup pg2 = new ParameterGroup();
            dict.Add("dim width", pg1);
            dict.Add("dim height", pg2);
            pg1.Add(new Parameter(5, 1, 30, 0.1f));
            pg2.Add(new Parameter(3, 2, 10, 0.1f));
            return dict;
        }
        public override List<Meshable> ExecuteShape(ShapeObject so)
        {
            float w = ((ParameterGroup)paramGroups["dim width"]).parameters[0].value;
            float h = (int)((ParameterGroup)paramGroups["dim height"]).parameters[0].value;
            List<Meshable> outs = new List<Meshable>();

            if (so.meshable.GetType() == typeof(CompositMeshable))
            {
                foreach (Meshable m in ((CompositMeshable)so.meshable).components)
                {
                    try
                    {
                        Meshable[] units = m.GridByMag(w, h);
                        if (units == null) outs.Add(m);
                        else outs.AddRange(units);
                    }
                    catch
                    {
                        Debug.Log(string.Format("{0} is not {1}", m.GetType(), typeof(Polygon)));
                        outs.Add(m);
                    }


                }
            }
            else
            {
                Debug.Log(string.Format("{0} is not {1}", so.meshable.GetType(),typeof(CompositMeshable)));
            }
            Form f = new Form(outs.ToArray());
            return new List<Meshable>{ f };

        }
        
    }
   

    //pivot
    public class PivotMirror : Rule
    {
        public PivotMirror()
        {
            inputs.names.Add("A");
            outputs.names.Add("A");
        }
        public PivotMirror(string inName, string OutName, float axis) : base(inName, new string[] { OutName })
        {
            paramGroups = DefaultParam();
            ((ParameterGroup)paramGroups["Axis"]).parameters[0].value = axis;
        }
        public override OrderedDictionary DefaultParam()
        {
            OrderedDictionary dict = new OrderedDictionary();
            ParameterGroup pg1 = new ParameterGroup();
            pg1.Add(new Parameter(0, 0, 2, 1));
            dict.Add("Axis", pg1);
            return dict;
        }
        public override void Execute()
        {
            //remove extra or add new
            int axis = (int)((ParameterGroup)paramGroups["Axis"]).parameters[0].value;
            int diff = outputs.shapes.Count - inputs.shapes.Count;
            int absDiff = Mathf.Abs(diff);
            for (int i = 0; i < absDiff; i++)
            {
                if (diff > 0)//remove extra
                {
                    int index = outputs.shapes.Count - i;
                    GameObject.Destroy(outputs.shapes[index].gameObject);
                    outputs.shapes.RemoveAt(index);
                }
                else//add new
                {
                    outputs.shapes.Add(ShapeObject.CreateBasic());
                }
            }

            //update each output
            for (int i = 0; i < inputs.shapes.Count; i++)
            {
                Debug.Log("cloning shapeobject, has bbox="+(inputs.shapes[i].meshable.bbox!=null));
                inputs.shapes[i].CloneTo(outputs.shapes[i]);
                //outputs.shapes[i].meshable.bbox = inputs.shapes[i].meshable.bbox;
                Debug.Log("cloned, has bbox=" + (outputs.shapes[i].meshable.bbox != null));
                outputs.shapes[i].PivotMirror(axis);
            }
            AssignNames(outputs.shapes);
        }//end execute
    }
    public class PivotTurn : Rule
    {
        public PivotTurn()
        {
            inputs.names.Add("A");
            outputs.names.Add("A");
        }
        public PivotTurn(string inName, string OutName, float count) : base(inName, new string[] { OutName })
        {
            paramGroups = DefaultParam();
            ((ParameterGroup)paramGroups["Count"]).parameters[0].value = count;
        }
        public override OrderedDictionary DefaultParam()
        {
            OrderedDictionary dict = new OrderedDictionary();
            ParameterGroup pg1 = new ParameterGroup();
            pg1.Add(new Parameter(1, 1, 3, 1));
            dict.Add("Count", pg1);
            return dict;
        }
        public override void Execute()
        {
            //remove extra or add new
            int count = (int)((ParameterGroup)paramGroups["Count"]).parameters[0].value;
            int diff = outputs.shapes.Count - inputs.shapes.Count;
            int absDiff = Mathf.Abs(diff);
            for (int i = 0; i < absDiff; i++)
            {
                if (diff > 0)//remove extra
                {
                    int index = outputs.shapes.Count - i;
                    GameObject.Destroy(outputs.shapes[index].gameObject);
                    outputs.shapes.RemoveAt(index);
                }
                else//add new
                {
                    outputs.shapes.Add(ShapeObject.CreateBasic());
                }
            }

            //update each output
            for (int i = 0; i < inputs.shapes.Count; i++)
            {
                //Debug.Log("cloning shapeobject, has bbox=" + (inputs.shapes[i].meshable.bbox != null));
                inputs.shapes[i].CloneTo(outputs.shapes[i]);
                //outputs.shapes[i].meshable.bbox = inputs.shapes[i].meshable.bbox;
                //Debug.Log("cloned, has bbox=" + (outputs.shapes[i].meshable.bbox != null));
                outputs.shapes[i].PivotTurn(count);
            }
            AssignNames(outputs.shapes);
        }//end execute
    }

    //creation
    
    public class CreateBox : Rule
    {
        public CreateBox() : base()
        {
            outputs.names.Add("A");

        }
        public CreateBox(string outName, Vector3 pos, Vector3 size, Vector3 rotation) :base()
        {
            outputs.names.Add("A");
            for (int i = 0; i < 3; i++)
            {
                ((ParameterGroup)paramGroups["Position"]).parameters[i].value = pos[i];
                ((ParameterGroup)paramGroups["Position"]).parameters[i].min = pos[i] / 5;
                ((ParameterGroup)paramGroups["Position"]).parameters[i].max = pos[i] * 5;

                ((ParameterGroup)paramGroups["Size"]).parameters[i].value = size[i];
                ((ParameterGroup)paramGroups["Size"]).parameters[i].min = pos[i] / 5;
                ((ParameterGroup)paramGroups["Size"]).parameters[i].max = pos[i] * 5;

                ((ParameterGroup)paramGroups["Rotation"]).parameters[i].value = rotation[i];
            }
        }
        public override List<Meshable> ExecuteShape(ShapeObject so)
        {
            float[] pos = new float[3];
            float[] size = new float[3];
            for (int i = 0; i < 3; i++)
            {
                pos[i] = ((ParameterGroup)paramGroups["Position"]).parameters[i].value;
                size[i] = ((ParameterGroup)paramGroups["Size"]).parameters[i].value;
            }
            Vector3 vpos = new Vector3(pos[0], pos[1], pos[2]);
            Vector3 vsize = new Vector3(size[0], size[1], size[2]);

            Vector3[] pts = new Vector3[4];
            Vector3 vx = new Vector3(size[0], 0, 0);
            Vector3 vy = new Vector3(0, size[1], 0);
            Vector3 vz = new Vector3(0, 0, size[2]);


            pts[0] = vpos;
            pts[1] = vpos + vx;
            pts[2] = pts[1] + vz;
            pts[3] = pts[0] + vz;
            Polygon pg = new Polygon(pts);
            Form f = pg.ExtrudeToForm(vy);
            
            List<Meshable> outs = new List<Meshable>();
            outs.Add(f);
            AssignNames(outs);
            return outs;

        }
        public override OrderedDictionary DefaultParam()
        {
            OrderedDictionary dict = new OrderedDictionary();
            ParameterGroup pg1 = new ParameterGroup();
            ParameterGroup pg2 = new ParameterGroup();
            ParameterGroup pg3 = new ParameterGroup();

            for (int i = 0; i < 3; i++)
            {
                pg1.Add(new Parameter(0, 0, 100, 3f));
                pg2.Add(new Parameter(10, 0, 100, 3f));
                pg3.Add(new Parameter(0, 0, 90, 1));
            }
            dict.Add("Position", pg1);
            dict.Add("Size", pg2);
            dict.Add("Rotation", pg3);

            return dict;
        }

    }
    public class CreateOperableBox : Rule
    {
        public CreateOperableBox() : base()
        {
            outputs.names.Add("A");

        }
        public CreateOperableBox(string outName, Vector3 size) : base()
        {
            outputs.names.Add("A");
            for (int i = 0; i < 3; i++)
            {
                ((ParameterGroup)paramGroups["Size"]).parameters[i].value = size[i];
                ((ParameterGroup)paramGroups["Rotation"]).parameters[i].value = 0;
            }
        }
        
        public override List<Meshable> ExecuteShape(ShapeObject inSo)
        {
            float[] size = new float[3];
            for (int i = 0; i < 3; i++)
            {
                size[i] = ((ParameterGroup)paramGroups["Size"]).parameters[i].value;
            }
            Vector3 vpos = inSo.Position;
            Vector3 vsize = new Vector3(size[0], size[1], size[2]);

            Vector3[] pts = new Vector3[4];
            Vector3 vx = new Vector3(size[0], 0, 0);
            Vector3 vy = new Vector3(0, size[1], 0);
            Vector3 vz = new Vector3(0, 0, size[2]);


            pts[0] = vpos;
            pts[1] = vpos + vx;
            pts[2] = pts[1] + vz;
            pts[3] = pts[0] + vz;
            Polygon pg = new Polygon(pts);
            Meshable f = pg.Extrude(vy);

            List<Meshable> outs = new List<Meshable>();
            outs.Add(f);
            AssignNames(outs);
            return outs;

        }
        public override OrderedDictionary DefaultParam()
        {
            OrderedDictionary dict = new OrderedDictionary();
            ParameterGroup pg1 = new ParameterGroup();
            ParameterGroup pg2 = new ParameterGroup();

            for (int i = 0; i < 3; i++)
            {
                pg1.Add(new Parameter(10, 0, 100, 3f));
                pg2.Add(new Parameter(0, 0, 90, 1));
            }
            dict.Add("Size", pg1);
            dict.Add("Rotation", pg2);

            return dict;
        }

    }

    public class CreateStair : Rule
    {
        public float stairW = 4;
        public float stairH = 10;
        public CreateStair() : base() { }
        public CreateStair(string inName, string outName) : base(inName, outName) { }
        public override List<Meshable> ExecuteShape(ShapeObject inSo)
        {
            inSo.PivotTurn(2);
            float w = inSo.Size[0];
            List<Vector3> stairLocs = new List<Vector3>();
            if (w <= 30)
            {
                stairLocs.Add(inSo.Position);
                Vector3 v = inSo.Vects[0];
                v *= (w - stairW);
                stairLocs.Add(inSo.Position + v);
            }
            else if (w < 45)
            {
                stairLocs.Add(inSo.Position);
                Vector3 v = inSo.Vects[0];
                v *= 30;
                stairLocs.Add(inSo.Position + v);
            }
            else
            {
                float d1 = w / 2 - 15;
                float d2 = w - d1;
                Vector3 v1 = inSo.Vects[0] * d1;
                Vector3 v2 = inSo.Vects[0] * d2;
                stairLocs.Add(inSo.Position + v1);
                stairLocs.Add(inSo.Position + v2);
            }

            List<Meshable> outs = new List<Meshable>();
            float coreH = inSo.Size[1] + 3;
            foreach (Vector3 v in stairLocs)
            {
                outs.Add(CreateBox(v, new Vector3(stairW, coreH, stairH), inSo.Vects));
            }

            AssignNames(outs);
            outs.Add(inSo.meshable);
            return outs;

        }
        Meshable CreateBox(Vector3 pos, Vector3 size, Vector3[] vects)
        {
            Vector3[] pts = new Vector3[4];
            Vector3 mv1 = vects[0] * size[0];
            Vector3 mv2 = vects[1] * size[1];
            Vector3 mv3 = vects[2] * size[2];
            pts[0] = pos;
            pts[1] = pts[0] + mv1;
            pts[2] = pts[1] + mv3;
            pts[3] = pts[0] + mv3;
            Extrusion ext = new Extrusion(pts, size[1]);
            return ext;
        }
    }
}
