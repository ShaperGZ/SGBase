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
            name = "Bisect";
            inputs.names.Add("A");
            outputs.names.Add("B");
            outputs.names.Add("C");
            
        }
        public Bisect(string inName, string[] outNames, float d, int axis):base(inName,outNames)
        {
            name = "Bisect";
            ((ParameterGroup)paramGroups["Position"]).parameters[0].Value = d;
            ((ParameterGroup)paramGroups["Axis"]).parameters[0].Value = axis;
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
            //Vector3 org = so.transform.position + v;
            Vector3 org = so.Position + v;
            Plane pln = new Plane(normal, org);
            return pln;
        }
        public override List<Meshable> ExecuteShape(ShapeObject so)
        {
            List<Meshable> outs = new List<Meshable>();
            if (so == null || so.meshable == null) return outs;

            /////////////////
            //get parameters
            /////////////////
            Parameter pm = GetParam("Position", 0);
            float d;
            if (pm.getSORefValueCallback != null)
            {
                d = pm.getSORefValueCallback(so);
            }
            else
                d = pm.Value;

            
            int axis = (int)GetParamVal("Axis", 0);

            //round to divisible by 4
            if (axis==1)
            {
                float H = so.Size[1];
                float mag = d * H;
                mag -= mag % 4;
                d = mag / H;
            }
            
            

            /////////////////
            //get split plane
            /////////////////
            Plane pln = GetPlane(so, d, axis);


            outs = SplitByPlane(so.meshable, pln);
            foreach (Meshable mb in outs)
            {
                if (mb == null) continue;
                mb.bbox = BoundingBox.CreateFromPoints(mb.vertices, so.meshable.bbox);
            }

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
        public BisectLength() : base() { name = "BisectLength"; }
        public BisectLength(string inName, string[] outNames, float d, int axis):base(inName,outNames,d,axis)
        {
            name = "BisectLength";
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
    
    public class Hide : Rule
    {
        public Hide() : base() { name = "Hide"; }
        public Hide(string[] inNames) :base(){
            name = "Hide";
            inputs.names.Clear();
            inputs.names.AddRange(inNames);
        }
        public override void Execute()
        {
            Debug.Log("inputshapes.Count=" + inputs.shapes.Count);
            foreach (ShapeObject so in inputs.shapes)
            {
                Debug.Log("shapename="+so.name);
            }
            outMeshables.Clear();
            outputs.shapes.Clear();
        }
    }
    public class Colorize : Rule
    {
        Color color = SchemeColor.ColorSetDefault[0];
        public Colorize() : base() { name = "Hide"; }
        public Colorize(string[] inNames, Color color) : base()
        {
            name = "SetMaterial";
            inputs.names.Clear();
            inputs.names.AddRange(inNames);
            this.color = color;
        }
        public override void Execute()
        {
            outMeshables.Clear();
            outputs.shapes = inputs.shapes;
            foreach (ShapeObject so in outputs.shapes)
            {
                MeshRenderer mr= so.GetComponent<MeshRenderer>();
                if (mr != null)
                {
                    mr.material.color = color;
                }
            }
            
        }
    }

    public class DivideToFTFH : Rule
    {
        Plane cutPlane;
        public DivideToFTFH() : base()
        {
            name = "DivideToFTFH";
            inputs.names.Add("A");
            outputs.names.Add("B");
            outputs.names.Add("C");
        }
        public DivideToFTFH(string inName, string outName, float h) : base(inName, outName)
        {
            name = "DivideToFTFH";
            ((ParameterGroup)paramGroups["FTFHeight"]).parameters[0].Value = h;
        }
        public DivideToFTFH(string inName, string[] outNames, float h) : base(inName, outNames)
        {
            name = "DivideToFTFH";
            ((ParameterGroup)paramGroups["FTFHeight"]).parameters[0].Value = h;
        }
        public virtual List<float> GetDivs(float ftfh, ShapeObject so)
        {
            if (ftfh == 0) throw new System.Exception("ftfh can not be zero!");
            List<float> outDivs = new List<float>();

            float bot = so.Position.y;
            float top = bot + so.Size[1];
            //float totalH = top - bot;
            float totalH = so.Size[1];
            //Debug.LogFormat("bot={0}, top={1}", bot, top);

            float h = Mathf.Ceil(bot / ftfh);
            float trunk = (h - bot);
            if (trunk != 0)
                outDivs.Add(trunk);
            while (h < top)
            {
                if (h+ftfh < top)
                {
                    outDivs.Add(ftfh);
                }
                else
                {
                    trunk = top - h;
                    if(trunk!=0)
                        outDivs.Add(trunk);
                    break;
                }
                h += ftfh;
                //Debug.Log("h=" + h);
            }
            
            if (outDivs.Count == 0) throw new System.Exception("outDives is empty");
            //foreach (float f in outDivs) Debug.Log(f);
            return outDivs;
        }
        public override List<Meshable> ExecuteShape(ShapeObject so)
        {
            //Debug.Log("executing so");
            List<Meshable> outs = new List<Meshable>();
            ////////////////
            //get paramters
            ////////////////

            ///////////////////////////
            //sitribute division points
            ///////////////////////////
            float ftfh = GetParamVal("FTFHeight", 0);
            List<float> divs = GetDivs(ftfh, so);

            //tout is the remaining part in division
            //tout will be updated at each iteration
            //each iteration divides tout, and stops when there are no more left.
            List<Meshable> touts = new List<Meshable>();
            Vector3 org = so.transform.position;
            int counter = 0;
            while (counter == 0 || touts.Count > 1)
            {
                if (counter >= divs.Count)
                {
                    Debug.LogWarning("counter>divs.Count");
                    break;
                }
                //Debug.Log(counter+" div="+divs[counter]);
                //get split plane
                Vector3 normal = so.Vects[1];
                Vector3 v = normal * divs[counter];
                
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

            dict["FTFHeight"] = pg1;
            pg1.Add(new Parameter(10, 5, 100, 1));

            return dict;
        }
    }
    public class Divide : Rule
    {
        Plane cutPlane;
        public Divide() : base()
        {
            name = "Divide";
            inputs.names.Add("A");
            outputs.names.Add("B");
            outputs.names.Add("C");

        }
        public Divide(string inName, string[] outNames, float[] ds, int axis) : base(inName, outNames)
        {
            name = "Divide";
            for (int i = 0; i < ds.Length; i++)
            {
                float d = ds[i];
                ParameterGroup pg = (ParameterGroup)paramGroups["Position"];
                if (i < pg.parameters.Count)
                {
                    pg.parameters[i].Value = d;
                }
                else
                {
                    pg.parameters.Add(new Parameter(d, 0, 1, 0.01f));
                }
            }
            
            ((ParameterGroup)paramGroups["Axis"]).parameters[0].Value = axis;

        }
        public virtual List<float> GetDivs(List<Parameter> pms, float max)
        {
            float total = 0;
            int axis =(int) GetParamVal("Axis", 0);

            List<float> outDivs = new List<float>();
            foreach (Parameter p in pms)
            {
                if (total >= max) break;
                float rest = max - total;
                float val = p.Value * max;

                //round to divisible by 4
                if (axis == 1)
                    val -= val % 4;
                if (val > rest) val = rest;
                outDivs.Add(val);
                total += val;
            }
            return outDivs;
        }
        public override List<Meshable> ExecuteShape(ShapeObject so)
        {
            //Debug.Log("executing so");
            List<Meshable> outs = new List<Meshable>();
            if (so.meshable == null) return outs;

            ////////////////
            //get paramters
            ////////////////
            int axis = (int)((ParameterGroup)paramGroups["Axis"]).parameters[0].Value;
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
            name = "DivideTo";
            inputs.names.Add("A");
            outputs.names.Add("A");
        }
        public DivideTo(string inName, string outName, float ds, int axis) : base(inName, new string[] { outName }, new float[]{ ds}, axis)
        {
            name = "DivideTo";
        }
        public DivideTo(string inName, string[] outNames, float ds, int axis) : base(inName, outNames, new float[]{ ds},axis)
        {
            name = "DivideTo";
        }
        public override List<float> GetDivs(List<Parameter> pms, float max)
        {
            //Debug.Log("cal culating GetDivs");
            float d = pms[0].Value;
            float count = Mathf.Round(max / d);
            d = max / count;
            //Debug.Log(string.Format("cal culating GetDivs d={0}, max={1}, count={2}", d,max,count));
            List<float> outDivs = new List<float>();
            for (int i = 0; i < count; i++)
            {
                outDivs.Add(d);
                //Debug.Log(i + " adding" + d);
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
            name = "Scale";
            inputs.names.Add("A");
            outputs.names.Add("A");
            paramGroups = DefaultParam();
        }
        public Scale(string inName, string outName, float d, int axis) : base(inName, new string[] { outName })
        {
            name = "Scale";
            ((ParameterGroup)paramGroups["Position"]).parameters[0].Value = d;
            ((ParameterGroup)paramGroups["Axis"]).parameters[0].Value = axis;
        }
        public override List<Meshable> ExecuteShape(ShapeObject so)
        {
            float d = ((ParameterGroup)paramGroups["Position"]).parameters[0].Value;
            int axis = (int)((ParameterGroup)paramGroups["Axis"]).parameters[0].Value;

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
    public class Extrude : Rule
    {
        //protected int? alignment = null;
        Plane cutPlane;
        public bool isGraphics = false;
        public Extrude() : base()
        {
            inputs.names.Add("A");
            outputs.names.Add("A");
            paramGroups = DefaultParam();
            name = "Extrude";
        }
        public Extrude(string inName, string outName, float mag, bool isGraphics=false, Vector2? randRange = null) : base(inName, new string[] { outName })
        {
            name = "Scale3D";
            SetParam("Up", 0, mag);
            this.randRange = randRange;
            this.isGraphics = isGraphics;

        }
        public override List<Meshable> ExecuteShape(ShapeObject so)
        {
            if (so.meshable.GetType() != typeof(Polygon)) return new List<Meshable>();

            float up = ((ParameterGroup)paramGroups["Up"]).parameters[0].Value;

            //get scale
            Vector3 extrudeVect = new Vector3(0, up, 0);

            if (randRange.HasValue)
            {
                float min = randRange.Value[0];
                float max = randRange.Value[1];
                extrudeVect[1] = extrudeVect[1] * (1 + Random.Range(min, max));
                
            }
            
            //get the splited meshables
            Polygon pg = (Polygon)so.meshable;
            Extrusion exts = pg.Extrude(extrudeVect);
           
            
            List<Meshable> outs = new List<Meshable>();
            outs.Add(exts);
            AssignNames(outs);
            //AssignNames(outMeshables.ToArray());
            return outs;

            //Rule.Execute() will take care of the outMeshables
        }
        public override OrderedDictionary DefaultParam()
        {
            OrderedDictionary dict = new OrderedDictionary();
            ParameterGroup pg1 = new ParameterGroup();
            List<ParameterGroup> outParamGroups = new List<ParameterGroup>();
            outParamGroups.Add(pg1);

            dict["Up"] = pg1;
            pg1.Add(new Parameter(1f, 0.2f, 100f, 0.01f));

            return dict;
        }
    }
    public class Scale3D : Rule
    {
        protected Alignment? alignment = null;
        Plane cutPlane;
        public Scale3D() : base()
        {
            inputs.names.Add("A");
            outputs.names.Add("A");
            paramGroups = DefaultParam();
            name = "Scale3D";
        }
        public Scale3D(string inName, string outName, Vector3 scale, Vector2? randRange=null,Alignment? ialignment=null) : base(inName, new string[] { outName })
        {
            name = "Scale3D";
            alignment = ialignment;
            ((ParameterGroup)paramGroups["Scale"]).parameters[0].Value = scale[0];
            ((ParameterGroup)paramGroups["Scale"]).parameters[1].Value = scale[1];
            ((ParameterGroup)paramGroups["Scale"]).parameters[2].Value = scale[2];
            this.randRange = randRange;

        }
        public override List<Meshable> ExecuteShape(ShapeObject so)
        {
            float dx = ((ParameterGroup)paramGroups["Scale"]).parameters[0].Value;
            float dy = ((ParameterGroup)paramGroups["Scale"]).parameters[1].Value;
            float dz = ((ParameterGroup)paramGroups["Scale"]).parameters[2].Value;

            //round to divisible by 4
            float h = so.Size[1] * dy;
            h -= h % 4;
            dy = h / so.Size[1];

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
            if (alignment.HasValue)
            {
                origin=so.meshable.bbox.GetOriginFromAlignment(alignment.Value);
            }
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
            name = "Size3D";
        }
        public Size3D(string inName, string outName, Vector3 size, Vector2? randRange = null) : base(inName, new string[] { outName })
        {
            SetParam("Size", 0, size[0], 30, 80, 0.1f);
            SetParam("Size", 1, size[1], 3, 180, 0.1f);
            SetParam("Size", 2, size[2], 50, 0.1f);
            name = "Size3D";
            //paramGroups = DefaultParam();
            //((ParameterGroup)paramGroups["Size"]).parameters[0].Value = size[0];
            //((ParameterGroup)paramGroups["Size"]).parameters[1].Value = size[1];
            //((ParameterGroup)paramGroups["Size"]).parameters[2].Value = size[2];
            this.randRange = randRange;

        }
        public override List<Meshable> ExecuteShape(ShapeObject so)
        {
            float dx = ((ParameterGroup)paramGroups["Size"]).parameters[0].Value;
            float dy = ((ParameterGroup)paramGroups["Size"]).parameters[1].Value;
            float dz = ((ParameterGroup)paramGroups["Size"]).parameters[2].Value;

            //Debug.LogFormat("{0}->{1}", dy, dy - (dy % 4));
            dy = dy - (dy % 4);

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
    public class SizeBuilding3D : Size3D
    {
        public SizeBuilding3D() : base()
        {
            if (grammar.properties == null) grammar.properties = new BuildingProperties();
            name = "SizeBuilding3D";
        }
        public SizeBuilding3D(string inName, string outName, Vector3 size) : base(inName, outName, size)
        {
            name = "SizeBuilding3D";
        }
        public override OrderedDictionary DefaultParam()
        {
            OrderedDictionary dict = new OrderedDictionary();
            ParameterGroup pg1 = new ParameterGroup();
            List<ParameterGroup> outParamGroups = new List<ParameterGroup>();
            outParamGroups.Add(pg1);

            dict["Size"] = pg1;
            pg1.Add(new Parameter(30f, 30f, 80f, 0.01f));
            pg1.Add(new Parameter(18f, 3f, 100f, 0.01f));
            pg1.Add(new Parameter(8f, 8f, 40f, 0.01f));

            return dict;
        }
        public override void Execute()
        {
            base.Execute();
            if (grammar == null) return;

            if(grammar.building != null)
            {
                Building bp = grammar.building;
                bp.width = GetParamVal("Size", 0);
                bp.depth = GetParamVal("Size", 2);
                bp.height = GetParamVal("Size", 1);
            }
            
        }
    }



    public class SizeAdd : Rule
    {
        Plane cutPlane;
        public SizeAdd() : base()
        {
            inputs.names.Add("A");
            outputs.names.Add("A");
            paramGroups = DefaultParam();
        }
        public SizeAdd(string inName, string outName, Vector3 size, Vector2? randRange = null) : base(inName, new string[] { outName })
        {
            paramGroups = DefaultParam();
            ((ParameterGroup)paramGroups["SizeAddition"]).parameters[0].Value = size[0];
            ((ParameterGroup)paramGroups["SizeAddition"]).parameters[1].Value = size[1];
            ((ParameterGroup)paramGroups["SizeAddition"]).parameters[2].Value = size[2];
            this.randRange = randRange;

        }
        public override List<Meshable> ExecuteShape(ShapeObject so)
        {
            float dx = ((ParameterGroup)paramGroups["SizeAddition"]).parameters[0].Value;
            float dy = ((ParameterGroup)paramGroups["SizeAddition"]).parameters[1].Value;
            float dz = ((ParameterGroup)paramGroups["SizeAddition"]).parameters[2].Value;

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

            if (size.z > size.x) size.x = size.z;

            Vector3 scale = new Vector3(1, 1, 1);
            Vector3 soSize = so.transform.localScale;
            for (int i = 0; i < 3; i++)
            {
                scale[i] = 1+(size[i] / soSize[i]);
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

            dict["SizeAddition"] = pg1;
            pg1.Add(new Parameter(30f, 30f, 80f, 0.01f));
            pg1.Add(new Parameter(18f, 3f, 100f, 0.01f));
            pg1.Add(new Parameter(8f, 8f, 80f, 0.01f));

            return dict;
        }
    }
    //delete this
    public class DivideSurfaceTBD : Rule
    {
        public DivideSurfaceTBD()
        {
            inputs.names.Add("A");
            outputs.names.Add("FA");
        }
        public DivideSurfaceTBD(string inName, string OutName, float w, float h) : base(inName, new string[] { OutName })
        {
            paramGroups = DefaultParam();
            ((ParameterGroup)paramGroups["dim width"]).parameters[0].Value = w;
            ((ParameterGroup)paramGroups["dim height"]).parameters[0].Value = h;
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
            float w = ((ParameterGroup)paramGroups["dim width"]).parameters[0].Value;
            float h = (int)((ParameterGroup)paramGroups["dim height"]).parameters[0].Value;
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
            ((ParameterGroup)paramGroups["Axis"]).parameters[0].Value = axis;
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
            int axis = (int)((ParameterGroup)paramGroups["Axis"]).parameters[0].Value;
            removeExtraOutputs();

            //update each output
            for (int i = 0; i < inputs.shapes.Count; i++)
            {
                if (i >= outputs.shapes.Count)
                {
                    outputs.shapes.Add(ShapeObject.CreateBasic());
                }
                //Debug.Log("cloning shapeobject, has bbox="+(inputs.shapes[i].meshable.bbox!=null));
                inputs.shapes[i].CloneTo(outputs.shapes[i]);
                outputs.shapes[i].parentRule = this;
                //outputs.shapes[i].meshable.bbox = inputs.shapes[i].meshable.bbox;
                //Debug.Log("cloned, has bbox=" + (outputs.shapes[i].meshable.bbox != null));
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
            ((ParameterGroup)paramGroups["Count"]).parameters[0].Value = count;
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
            int count = (int)((ParameterGroup)paramGroups["Count"]).parameters[0].Value;
            int diff = outputs.shapes.Count - inputs.shapes.Count;
            int absDiff = Mathf.Abs(diff);
            removeExtraOutputs();

      

            //update each output
            for (int i = 0; i < inputs.shapes.Count; i++)
            {
                if (i >= outputs.shapes.Count)
                {
                    outputs.shapes.Add(ShapeObject.CreateBasic());
                }
                //Debug.Log("cloning shapeobject, has bbox=" + (inputs.shapes[i].meshable.bbox != null));
                inputs.shapes[i].CloneTo(outputs.shapes[i]);
                //outputs.shapes[i].meshable.bbox = inputs.shapes[i].meshable.bbox;
                //Debug.Log("cloned, has bbox=" + (outputs.shapes[i].meshable.bbox != null));
                outputs.shapes[i].PivotTurn(count);
                outputs.shapes[i].parentRule = this;
            }
            AssignNames(outputs.shapes);
        }//end execute
    }



}
