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
        public override void ExecuteShape(ShapeObject so)
        {
            float d = ((ParameterGroup)paramGroups["Position"]).parameters[0].value;
            int axis = (int)((ParameterGroup)paramGroups["Axis"]).parameters[0].value;

            //get split plane
            Vector3 normal = so.Vects[axis];
            Vector3 v = normal * so.Size[axis] * d;
            Vector3 org = so.transform.position + v;
            Plane pln = new Plane(normal,org);

            //get the splited meshables
            Meshable mb = so.meshable;
            Meshable[] temp=new Meshable[0];
            //Debug.Log("+++ Before split");
            //Debug.Log("mb.verticeCount:"+mb.vertices.Length);
            //Debug.Log("mb.type:" + mb.GetType());
            temp = mb.SplitByPlane(pln);
            //Debug.Log("+++ After split");
            //Debug.Log("temp[0]="+temp[0]);
            //Debug.Log("temp[1]=" + temp[1]);

            List<Meshable> outs = new List<Meshable>();
            for(int i=0;i<temp.Length;i++)
            {
                if (temp[i] != null)
                {
                    temp[i].direction = mb.direction;
                    outs.Add(temp[i]);
                }
            }
            
            outMeshables.AddRange(outs.ToArray());

            string txt = "post executing bisect: outMeshables.Count=" + outMeshables.Count;
            foreach (Meshable s in outMeshables)
            {
                txt += "mb(" + s.vertices.Length + "),";
            }
            //Debug.Log(txt);

            AssignNames(outMeshables.ToArray());
            
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
    public class Divide : Rule
    {

        Plane cutPlane;
        public Divide() : base()
        {
            inputs.names.Add("A");
            outputs.names.Add("B");
            outputs.names.Add("C");

        }
        public Divide(string inName, string[] outNames, float d, int axis) : base(inName, outNames)
        {
            ((ParameterGroup)paramGroups["Position"]).parameters[0].value = d;
            ((ParameterGroup)paramGroups["Axis"]).parameters[0].value = axis;

        }
        public override void ExecuteShape(ShapeObject so)
        {
            //get paramters
            float d = ((ParameterGroup)paramGroups["Position"]).parameters[0].value;
            int axis = (int)((ParameterGroup)paramGroups["Axis"]).parameters[0].value;

            //get split plane
            Vector3 normal = so.Vects[axis];
            Vector3 v = normal * so.Size[axis] * d;
            Vector3 org = so.transform.position + v;
            Plane pln = new Plane(normal, org);

            //get the splited meshables
            Meshable mb = so.meshable;
            Meshable[] temp = new Meshable[0];
            //Debug.Log("+++ Before split");
            //Debug.Log("mb.verticeCount:" + mb.vertices.Length);
            //Debug.Log("mb.type:" + mb.GetType());
            temp = mb.SplitByPlane(pln);
            //Debug.Log("+++ After split");
            //Debug.Log("temp[0]=" + temp[0]);
            //Debug.Log("temp[1]=" + temp[1]);

            List<Meshable> outs = new List<Meshable>();
            for (int i = 0; i < temp.Length; i++)
            {
                if (temp[i] != null)
                {
                    temp[i].direction = mb.direction;
                    outs.Add(temp[i]);
                }
            }

            outMeshables.AddRange(outs.ToArray());

            string txt = "post executing bisect: outMeshables.Count=" + outMeshables.Count;
            foreach (Meshable s in outMeshables)
            {
                txt += "mb(" + s.vertices.Length + "),";
            }
            //Debug.Log(txt);

            AssignNames(outMeshables.ToArray());

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
        public override void ExecuteShape(ShapeObject so)
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
            scaledMb.direction = mb.direction;
            outMeshables.Add(scaledMb);
            AssignNames(outMeshables.ToArray());

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

}
