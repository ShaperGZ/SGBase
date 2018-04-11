using System.Collections.Specialized;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using System.Xml.Serialization;


namespace SGCore
{
    public class Grammar:GraphNode
    {
        public int currentStep = -1;
        public int displayStep = -1;
        public List<GraphNode> subNodes;
        public List<SGIO> stagedOutputs;
        public List<ShapeObject> assignedObjects;

        public Grammar():base()
        {
            stagedOutputs = new List<SGIO>();
            subNodes = new List<GraphNode>();
            assignedObjects = new List<ShapeObject>();
            paramGroups = DefaultParam();
        }
        public Grammar(string inName, string[] outNames):this()
        {
            inputs.names.Add(inName);
            outputs.names.AddRange(outNames);
        }
        public void AddRule(GraphNode r, bool execute = true)
        {
            currentStep = subNodes.Count;
            displayStep = currentStep;
            subNodes.Add(r);
            r.grammar = this;
            r.step = subNodes.Count - 1;
            if (execute)
            {
                Execute(subNodes.Count-1);
                SelectStep(subNodes.Count - 1);
            }
        }
        public void SubRule(Rule r, bool execute = true)
        {
            int index = subNodes.IndexOf(r);
            subNodes.Remove(r);
            if (execute && index > 0 && index < subNodes.Count)
            {
                Execute(index);
            }
        }



        public override void Execute()
        {
            ExecuteFrom(0);
        }
        public virtual void ExecuteFrom(GraphNode rule)
        {
            int index = subNodes.IndexOf(rule);
            if (index >= 0 && index < subNodes.Count)
                ExecuteFrom(index);
        }
        public virtual void ExecuteFrom(int start)
        {
            for (int i = start; i < subNodes.Count; i++)
            {
                Debug.Log("--------------executing (" + i + ")-----");
                Execute(i);
            }
            DisplayStep(subNodes.Count - 1);
            SelectStep(subNodes.Count - 1);
            outputs = stagedOutputs[stagedOutputs.Count - 1];

        }
        public virtual void Execute(int i)
        {
            //------------------------------------
            currentStep = i;
            SGIO tobeMerged = PreExecution(i);

            //Debug.Log("rule inputSHapes.Count=" + subNodes[i].inputs.shapes.Count);
            //Debug.Log(string.Format("{0}:step{1} about to execute {2} ",name,i,subNodes[i].name));
            subNodes[i].step = i;
            subNodes[i].Execute();
            //------------------------------------
            //Debug.Log("rule outputSHapes.Count=" + subNodes[i].outputs.shapes.Count);

            PostExecution(i, tobeMerged);
        }
        
        
        public virtual SGIO PreExecution(int step)
        {
            List<ShapeObject> availableShapes = new List<ShapeObject>();
            List<ShapeObject> outShapes = new List<ShapeObject>();
            SGIO tobeMerged = new SGIO();
            string txt = string.Format("PreExe step {0}, stg.Count={1}", step, stagedOutputs.Count);
            //Debug.Log(txt);

            //get available shapes
            if (step == 0)
            {
                //Debug.Log(string.Format("step==0, inputShapeCount={0} assignedCount={1}", inputs.shapes.Count , assignedObjects.Count));
                if (assignedObjects.Count > 0)
                {
                    foreach (ShapeObject so in assignedObjects) so.gameObject.active = false;
                    subNodes[0].inputs.shapes = assignedObjects;
                    return tobeMerged;
                }
                subNodes[0].inputs.shapes = inputs.shapes;
                return tobeMerged;
            }
            else
            {
                availableShapes.AddRange(stagedOutputs[step - 1].shapes);
            }

            foreach (ShapeObject o in availableShapes)
            {
                try
                {
                    if (subNodes[step].inputs.names.Contains(o.name))
                    {
                        outShapes.Add(o);
                    }
                    //if (rules[step].inputs.names.Contains(o.name))
                    //    outShapes.Add(o);
                    else
                    {
                        //Debug.Log(step + " " + rules[step].inputs.names[0] + " name not match " + o.name);
                        if (!tobeMerged.names.Contains(o.name))
                            tobeMerged.names.Add(o.name);
                        tobeMerged.shapes.Add(o);
                    }

                }
                catch
                {
                    //toBeDeleted.Add(o);
                    Debug.Log("TBD found at " + step + "ShapeObject=" + o);
                }

            }
            ////foreach(ShapeObject o in toBeDeleted)
            ////{
            ////    availableShapes.Remove(o);
            ////}
            subNodes[step].inputs.shapes = outShapes;

            return tobeMerged;

        }
        public virtual void PostExecution(int step, SGIO tobeMerged)
        {


            SGIO tempOut = new SGIO();
            tempOut = SGIO.Merge(subNodes[step].outputs, tobeMerged);
            if (step >= stagedOutputs.Count)
            {
                stagedOutputs.Add(tempOut);
            }
            else
            {
                stagedOutputs[step] = tempOut;
            }
        }
        public void HighlightStepObjectScope(int index)
        {
            for (int i = 0; i < stagedOutputs.Count; i++)
            {
                SGIO io = stagedOutputs[i];
                foreach (ShapeObject o in io.shapes)
                {
                    o.highlightScope = false;
                }
            }
            foreach (ShapeObject o in stagedOutputs[index].shapes)
            {
                o.highlightScope = true;
            }
        }
        public void DisplayStep(int step)
        {
            displayStep = step;
        }
        public void SelectStep(int index)
        {
            if (index < 0 || index >= subNodes.Count) return;
            currentStep = index;
            displayStep = index;
            SGIO sgio;
            //TODO: add this to grammar
            if (stagedOutputs.Count > 0)
            {
                for (int i = 0; i < stagedOutputs.Count; i++)
                {
                    sgio = stagedOutputs[i];
                    List<ShapeObject> tobeRemoved = new List<ShapeObject>();
                    foreach (ShapeObject o in sgio.shapes)
                    {
                        try
                        {
                            //o.gameObject.SetActive(false);
                            o.Show(false);
                        }
                        catch
                        {
                            tobeRemoved.Add(o);
                        }

                    }
                    foreach (ShapeObject o in tobeRemoved)
                    {
                        //sgio.shapes.Remove(o);
                    }
                }
            }

            sgio = stagedOutputs[displayStep];
            foreach (ShapeObject o in sgio.shapes)
            {
                if (o != null)
                    //o.gameObject.SetActive(true);
                    o.Show(true);

            }

        }


        public void SaveXML(string path)
        {
            XmlSerializer xsl = new XmlSerializer(typeof(List<Rule>));
            System.IO.TextWriter tw = new System.IO.StreamWriter(path);
            xsl.Serialize(tw, subNodes);
            tw.Close();
        }
        public void LoadXML(string path)
        {
            XmlSerializer xsl = new XmlSerializer(typeof(List<Rule>));
            System.IO.TextReader tr = new System.IO.StreamReader(path);
            subNodes=(List<GraphNode>)xsl.Deserialize(tr);
            tr.Close();
            foreach(Rule r in subNodes)
            {
                Debug.Log(r.description);
            }

            updateRuleNavigator();
            
        }
        public void updateRuleNavigator()
        {
            if(UserStats.SelectedGrammar == this || UserStats.ruleNavigator != null)
            {
                foreach (Rule r in subNodes)
                {
                    UserStats.ruleNavigator.AddItem(r.description);
                }
            }
        }
        public void Save(string path)
        {
            string text = "";
            List<string> txts=new List<string>();
            foreach(Rule r in subNodes)
            {
                text += "\n" + r.ToSentence();
                txts.Add( "\n" + r.ToSentence());
            }
            //System.IO.File.WriteAllLines(path, txts.ToArray());
            System.IO.File.WriteAllText(path, text);
        }
        public void Load(string path, bool execute = true)
        {
            Clear();
            string texts=System.IO.File.ReadAllText(path);
            string[] lines = System.IO.File.ReadAllLines(path);
            foreach (string s in lines)
            {
                if (s.Length < 4) continue;
                //Debug.Log(s);
                Rule r = Rule.CreateFromSentence(s);
                if (r!=null)
                {
                    AddRule(r, false);
                }
            }
            
            if (execute) Execute();
            Debug.Log(">>>>>>>> FLAG 3");
            if(UserStats.ruleNavigator != null &&UserStats.SelectedGrammar == this)
            {
                Debug.Log(">>>>>>>> FLAG 4");
                UserStats.ruleNavigator.UpdateButtonDescriptions();
            }
        }
        public void Clear()
        {
            foreach(SGIO sgio in stagedOutputs)
            {
                foreach (ShapeObject so in sgio.shapes)
                {
                    if (so != null)
                    {
                        GameObject.Destroy(so.gameObject);
                        GameObject.Destroy(so);
                    }
                }
            }
            stagedOutputs.Clear();
            subNodes.Clear();
        }


    }
}

