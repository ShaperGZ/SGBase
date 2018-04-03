using System.Collections;
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
            paramGroups[0].parameters[0].value = d;
            paramGroups[1].parameters[0].value = axis;
            
        }
        public override void ExecuteShape(ShapeObject so)
        {
            //get paramters
            float d = paramGroups[0].parameters[0].value;
            int axis = (int)paramGroups[1].parameters[0].value;

            //get split plane
            Vector3 normal = so.Vects[axis];
            Vector3 v = normal * so.Size[axis] * d;
            Vector3 org = so.transform.position + v;
            Plane pln = new Plane(normal,org);

            //get the splited meshables
            Meshable mb = so.meshable;
            Meshable[] temp=new Meshable[0];
            //Debug.Log("mb.verticeCount:"+mb.vertices.Length);
            temp = mb.SplitByPlane(pln);
           
            
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
            AssignNames(outMeshables.ToArray());
            
        }
        public override List<ParameterGroup> DefaultParam()
        {
            ParameterGroup pg1 = new ParameterGroup();
            ParameterGroup pg2 = new ParameterGroup();
            List<ParameterGroup> outParamGroups = new List<ParameterGroup>();
            outParamGroups.Add(pg1);
            outParamGroups.Add(pg2);

            pg1.name = "Bisect percentage";
            pg1.Add(new Parameter(0.4f));
            pg2.name = "Bisect Axis";
            pg2.Add(new Parameter(0, 0, 2, 1));

            return outParamGroups;
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
        public Scale(string inName, string outName, float d, int axis) : base(inName, outName)
        {
            name = "Scale";
            paramGroups[0].parameters[0].value = d;
            paramGroups[1].parameters[0].value = axis;

        }
        public override void ExecuteShape(ShapeObject so)
        {
            //get paramters
            float d = paramGroups[0].parameters[0].value;
            int axis = (int)paramGroups[1].parameters[0].value;

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
        public override List<ParameterGroup> DefaultParam()
        {
            ParameterGroup pg1 = new ParameterGroup();
            ParameterGroup pg2 = new ParameterGroup();
            List<ParameterGroup> outParamGroups = new List<ParameterGroup>();
            outParamGroups.Add(pg1);
            outParamGroups.Add(pg2);

            pg1.name = "Bisect percentage";
            pg1.Add(new Parameter(1.5f,0.2f,5f,0.01f));
            pg2.name = "Bisect Axis";
            pg2.Add(new Parameter(1, 0, 2, 1));

            return outParamGroups;
        }
    }

}
