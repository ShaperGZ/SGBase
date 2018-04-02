using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SGCore;
using SGGeometry;

namespace SGCore
{

    public class Rule : Node
    {
        public Grammar grammar;
        public int step=-1;
        public List<ParameterGroup> paramGroups;
        public List<Meshable> outMeshables;
        public new string description {
            get
            {
                string txt = "";
                txt += name + " | "+ inputs.names[0]+"-->";
                foreach (string n in inputs.names) txt += n + ',';
                return txt;
            }
        }
        public Rule()
        {
            paramGroups = DefaultParam();
            outMeshables = new List<Meshable>();
        }
        public Rule(string inName, string outName) : this()
        {
            inputs.names = new List<string>(new string[] { inName });
            outputs.names = new List<string>(new string[] { outName });
        }
        public Rule(string inName, string[] outNames):this()
        {
            inputs.names = new List<string>(new string[] { inName });
            outputs.names = new List<string>(outNames);
        }
        public virtual void Execute()
        {
            //operate on meshables and update existing shapeobjects
            outMeshables.Clear();
            for(int i = 0; i < inputs.shapes.Count; i++)
            {
                ExecuteShape(inputs.shapes[i]);
            }
            UpdateOutputShapes();
        }
        public void AssignNames(ShapeObject[] sos)
        {
            for(int i = 0; i < sos.Length; i++)
            {
                int nameIndex = i % outputs.names.Count;
                sos[i].name = outputs.names[nameIndex];
            }
        }
        public void AssignNames(Meshable[] mbs)
        {
            for (int i = 0; i < mbs.Length; i++)
            {
                int nameIndex = i % outputs.names.Count;
                mbs[i].name = outputs.names[nameIndex];
            }
        }
        public virtual void ExecuteShape(ShapeObject so)
        {

        }
        public virtual List<ParameterGroup> DefaultParam()
        {
            return new List<ParameterGroup>();
        }
        public void UpdateOutputShapes()
        {
            //List<string> newNames = new List<string>();
            //destroy extra shapes
            int dif = outputs.shapes.Count - outMeshables.Count;
            if ( dif >0 )
            {
                for(int i = 0; i < dif; i++)
                {
                    int index = outputs.shapes.Count - 1;
                    GameObject.Destroy(outputs.shapes[index].gameObject);
                    outputs.shapes.RemoveAt(index);
                }
            }

            //update output shapes
            int shapeCount = outputs.shapes.Count;
            for(int i = 0; i < outMeshables.Count; i++)
            {
                Meshable mb = outMeshables[i];
                if (i < shapeCount)
                {
                    outputs.shapes[i].SetMeshable(mb,mb.direction);
                    outputs.shapes[i].name = mb.name;
                }//end if 
                else
                {
                    ShapeObject so= ShapeObject.CreateMeshable(mb);
                    so.name = mb.name;
                    outputs.shapes.Add(so);
                }
                //if (!newNames.Contains(mb.name))
                //{
                //    newNames.Add(mb.name);
                //}
            }//end for i


        }
    }
}
