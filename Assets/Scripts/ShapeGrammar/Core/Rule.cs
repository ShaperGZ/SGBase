
using System;
using System.Collections;
using System.Collections.Specialized;
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
        public OrderedDictionary paramGroups;
        public List<Meshable> outMeshables;
        public new string description {
            get
            {
                string txt = "";
                txt += name + " | "+ inputs.names[0]+"-->";
                foreach (string n in outputs.names) txt += n + ',';
                return txt;
            }
        }
        public Rule()
        {
            inputs.names = new List<string>();
            outputs.names = new List<string>();
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
            if(inputs.shapes.Count>0)
            {
                string txt = "";
                txt = string.Format("{0} | inputs[0]=({1})", name, inputs.shapes[0].Format());
            }
                

            //operate on meshables and update existing shapeobjects
            outMeshables.Clear();
            for(int i = 0; i < inputs.shapes.Count; i++)
            {
                Debug.Log("processing:" + inputs.shapes[i].Format());
                ExecuteShape(inputs.shapes[i]);
                string txt = "";
                foreach(Meshable so in outMeshables)
                {
                    txt+="mb(" + so.vertices.Length+"),";
                }
                Debug.Log(txt);
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
        public virtual OrderedDictionary DefaultParam()
        {
            return new OrderedDictionary();
        }
        public void UpdateOutputShapes()
        {
            //List<string> newNames = new List<string>();
            //destroy extra shapes
            //Debug.Log(string.Format("{0} outputShapes:{1}, mesable:{2}", name, outputs.shapes.Count, outMeshables.Count));
            int dif = outputs.shapes.Count - outMeshables.Count;
            if ( dif >0 )
            {
                Debug.Log("dif >0 ouputs.shapesCount="+outputs.shapes.Count);
                for (int i = 0; i < dif; i++)
                {
                    int index = outputs.shapes.Count - 1;
                    Debug.Log("DELETING ---->" + outputs.shapes[index].Format());
                    GameObject.Destroy(outputs.shapes[index].gameObject);
                    outputs.shapes.RemoveAt(index);
                }
                Debug.Log("post destroy ouputs.shapesCount=" + outputs.shapes.Count);
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
                    outputs.shapes[i].parentRule = this;
                    outputs.shapes[i].step = step;
                }//end if 
                else
                {
                    ShapeObject so= ShapeObject.CreateMeshable(mb);
                    so.name = mb.name;
                    so.parentRule = this;
                    so.step = step;
                    outputs.shapes.Add(so);
                }
            }//end for i
            //Debug.Log(string.Format("{0} outputShapes:{1}, mesable:{2}", name, outputs.shapes.Count, outMeshables.Count));

        }
        public new string name
        {
            get
            {
                return this.GetType().ToString();
            }
        }


        public string ToSentence()
        {
            //sample: Bisec A -> B,C, | bisectDistance(0.5f,),axies(0,), 

            string outnames = "";
            foreach(string n in outputs.names)
            {
                outnames += n + ",";
            }
            string paramTexts = "";
            foreach( DictionaryEntry kvp in paramGroups)
            {
                ParameterGroup pg = (ParameterGroup)kvp.Value;
                paramTexts += (string)kvp.Key+"(";
                foreach(Parameter p in pg.parameters)
                {
                    paramTexts += p.value.ToString() + ",";
                }
                paramTexts += "),";
            }

            string txt = string.Format(
                "{0} {1} -> {2} | {3}",
                name, 
                inputs.names[0],
                outnames,
                paramTexts
                );
            return txt;

        }
        public static Rule CreateFromSentence(string txt)
        {
            //sample: Bisec A -> B,C, | bisectDistance(0.5f,),axies(0,), 
            Debug.Log(txt);
            string[] truncks = txt.Split('|');
            foreach (string t in truncks) Debug.Log(t);
            string front = truncks[0];
            string back = truncks[1];
            truncks = front.Split('>');
            string[] header1 = truncks[0].Split(' ');
            string[] header2 = truncks[1].Remove(' ').Split(',');

            string ruleName = header1[0];
            string inputName = header1[1];
            List<string> outNames = new List<string>();
            foreach(string n in header2)
            {
                outNames.Add(n);
            }

            Rule r=new Rule();
            return r;

        }
    }
}
