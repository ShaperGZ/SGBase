﻿using System.Collections.Specialized;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
using System.Xml.Serialization;


namespace SGCore
{

    public class GrammarTypes
    {
        public const int MASSING= 111;
        public const int PLANNING= 222;
        public const int FACADE = 333;
        public const int LANDSCAPE = 444;
        public const int TERRACE = 555;
    }

    public class Grammar:GraphNode
    {
        public int selectedStep = -1;
        public int currentStep = -1;
        public int displayStep = -1;
        public List<GraphNode> subNodes;
        public List<SGIO> stagedOutputs;
        public List<ShapeObject> assignedObjects;
        public Guid guid;
        

        public Grammar():base()
        {
            guid = Guid.NewGuid();
            SceneManager.AddGrammar(this);
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
        public void CloneTo(Grammar g)
        {
            g.Clear();
            g.name = name;
            foreach(GraphNode n in subNodes)
            {
                g.AddRule(n);
            }
            g.inputs.names.Clear();
            g.inputs.names.AddRange(inputs.names.ToArray());
        }
        public GraphNode FindFisrt(string name)
        {
            foreach (GraphNode n in subNodes)
            {
                if (n.name == name) return n;
            }
            return null;
        }
        public void AddRule(GraphNode r, bool execute = true)
        {
            currentStep = subNodes.Count;
            displayStep = currentStep;
            subNodes.Add(r);
            r.grammar = this;
            r.building = building;
            r.site = site;
            r.step = subNodes.Count - 1;
            if (execute)
            {
                Execute(subNodes.Count-1);
                SelectStep(subNodes.Count - 1);
            }
        }
        public void ClearStage(int index)
        {
            SGIO io = stagedOutputs[index];
            foreach(ShapeObject o in io.shapes)
            {
                try
                {
                    GameObject.Destroy(o);
                }
                catch { }
            }
        }
        public void ReplaceRule(GraphNode r, int index, bool execute=true)
        {
            r.grammar = this;
            r.step = index;
            ClearStage(index);
            subNodes[index].Clear();
            subNodes[index] = r;

            if (execute)
            {
                Execute();
            }

        }
        public override void SetVisible(bool flag)
        {
            base.SetVisible(flag);
            foreach(SGIO sgio in stagedOutputs)
            {
                foreach(ShapeObject so in sgio.shapes)
                {
                    so.SetVisible(flag);
                }
            }
        }
        public void AddRuleAfter(GraphNode r, int i, bool execute = true)
        {
            subNodes.Insert(i + 1, r);
            r.grammar = this;
            r.building = building;
            r.site = site;
            for (int j = i; j < subNodes.Count; j++)
            {
                subNodes[j].step = j;
            }
            if (execute)
            {
                ExecuteFrom(i+1);
            }
        }
        public void AddRuleAt(GraphNode r, int i, bool execute = true)
        {
            subNodes.Insert(i, r);
            r.grammar = this;
            r.building = building;
            r.site = site;
            for (int j = i; j < subNodes.Count; j++)
            {
                subNodes[j].step = j;
            }
            if (execute)
            {
                ExecuteFrom(i);
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

        public void ExtractParam(int i, string key, string extractName="")
        {
            Rule rule = (Rule)subNodes[i];
            try
            {
                object paramGroup = rule.paramGroups[key];
                string extName = ((ParameterGroup)paramGroup).extractName;
                if(extractName==null || extractName == "")
                {
                    extractName = i.ToString() + "_Ext_" + key;
                }
                paramGroups[extName] = paramGroup;
            }
            catch 
            {
                Debug.LogWarning("either no param or no extractName");
            }
        }
        public void ExtractParam(ParameterGroup pg)
        {
            try
            {
                string extName = pg.extractName;
                paramGroups[extName] = pg;
            }
            catch
            {
                Debug.LogWarning("either no param or no extractName");
            }
        }
        public void SetSubParamValue(int i, string name, float value)
        {
            Rule rule = GetRule(i);
            ParameterGroup pg = (ParameterGroup)rule.paramGroups[name];
            pg.parameters[0].Value = value;
        }
        public void SetParamValue(string name, float value)
        {
            ParameterGroup pg = (ParameterGroup)paramGroups[name];
            pg.parameters[0].Value = value;
        } 

        public override void Execute()
        {
            if (assignedObjects != null)
                foreach (ShapeObject s in assignedObjects) s.Show(false);
            if (inputs.shapes != null)
                foreach (ShapeObject s in inputs.shapes) s.Show(false);
            //removeParent();
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
                //Debug.Log("--------------executing (" + i + ")-----");
                Execute(i);
            }
            DisplayStep(subNodes.Count - 1);
            SelectStep(subNodes.Count - 1);
            outputs = stagedOutputs[stagedOutputs.Count - 1];

            InvalidateProperties();
            
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
        public void removeParent()
        {
            Vector3 pos = building.transform.position;
            building.transform.position = new Vector3(0, 0, 0);
            foreach (SGIO io in stagedOutputs)
            {
                foreach (ShapeObject so in io.shapes)
                {
                    //so.gameObject.transform.position += building.transform.position;
                    so.gameObject.transform.parent = null;
                }
            }
            building.transform.position = pos;
        }
        public void addParent()
        {
            Vector3 pos = building.transform.position;
            building.transform.position = new Vector3(0, 0, 0);
            foreach (SGIO io in stagedOutputs)
            {
                foreach (ShapeObject so in io.shapes)
                {
                    so.gameObject.transform.position += building.transform.position;
                    //so.gameObject.transform.parent = null;
                }
            }
            building.transform.position = pos;
        } 
        public void InvalidateProperties()
        {
            if (building != null)
            {
                
                building.Invalidate(true);
                GameObject.Find("BuildingPropText").GetComponent<Text>().text = building.FormatProperties();
            }
            else
            {
                GameObject.Find("BuildingPropText").GetComponent<Text>().text = "this grammar is not associated to a building";
            }
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
        public void SelectStep(int index, bool userSelect=false)
        {
            //when index is -1, show the original input shape
            if (index < -1 || index >= subNodes.Count) return;
            currentStep = index;
            displayStep = index;
            if(userSelect)
                selectedStep = index;
            SGIO sgio;
            //TODO: add this to grammar
            if (stagedOutputs.Count > 0)
            {
                if (assignedObjects != null)
                    foreach (ShapeObject s in assignedObjects) s.Show(false);
                if (inputs.shapes != null)
                    foreach (ShapeObject s in inputs.shapes) s.Show(false);

                for (int i = 0; i < stagedOutputs.Count; i++)
                {
                    sgio = stagedOutputs[i];
                    foreach (ShapeObject o in sgio.shapes)
                    {
                        try
                        {
                            //o.gameObject.SetActive(false);
                            if(o.gameObject.activeSelf)
                                o.Show(false);
                        }
                        catch
                        {
                            Debug.LogWarning("Found null so!");
                        }
                    }
                }
            }
            if (index >= 0)
            {
                sgio = stagedOutputs[displayStep];
                foreach (ShapeObject o in sgio.shapes)
                {
                    if (o != null)
                        //o.gameObject.SetActive(true);
                        if (o.grammar==null || o.alwaysActive)
                            if (!o.gameObject.activeSelf)
                                o.Show(true);

                }
            }
            else
            {
                if(assignedObjects !=null)
                    foreach (ShapeObject s in assignedObjects) s.Show(true);
                if (inputs.shapes != null)
                    foreach (ShapeObject s in inputs.shapes) s.Show(true);
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
            if(SceneManager.SelectedGrammar == this || SceneManager.ruleNavigator != null)
            {
                foreach (Rule r in subNodes)
                {
                    SceneManager.ruleNavigator.AddRuleListItem(r.description);
                }
            }
        }
        public void Save(string path)
        {
            //TODO: how to record exported params
            //Rules.Bisect A -> B,C, | Axis(0-0-2-1,) Position(0-0.4-1-0.01,)

            //test code to export params
            //#EXPORTPARAM
            //#EXPORTPARAM,1,"position"
            //maker,rule index, paramGroup Name
            string extractTxt = "";
            for(int i = 0; i < subNodes.Count; i++)
            {
                GraphNode rule =subNodes[i];
                Debug.Log("rule.paramGroups.keyCount=" + rule.paramGroups.Keys.Count);
                foreach(string key in rule.paramGroups.Keys)
                {
                    ParameterGroup pg = (ParameterGroup)rule.paramGroups[key];
                    if (pg.extractName != null && pg.extractName != ""  && this.paramGroups.Contains(pg.extractName))
                    {
                        string txt = string.Format("\n#EXPORTPARAM,{0},{1}",i, key);
                        extractTxt += txt;
                    }
                }
            }

            //belowing working code to save grammar
            //start writing
            string text = "";
            text += extractTxt;
            List<string> txts=new List<string>();
            foreach(Rule r in subNodes)
            {
                text += "\n" + r.ToSentence();
                txts.Add( "\n" + r.ToSentence());
            }
            //System.IO.File.WriteAllLines(path, txts.ToArray());
            System.IO.File.WriteAllText(path, text);
        }
        public static void CreateFromFile(string path, bool execute = true)
        {
            Grammar g = new Grammar();
            g.Load(path, execute);
        }
        public void Load(string path, bool execute = true)
        {
            Clear();
            string texts=System.IO.File.ReadAllText(path);
            string[] lines = System.IO.File.ReadAllLines(path);
            List<string> extractLines = new List<string>();
            foreach (string s in lines)
            {
                if (s.Length < 4) continue;
                //Debug.Log(s);
                if (s != "")
                {
                    if(s.Substring(0, 12) == "#EXPORTPARAM")
                    {
                        extractLines.Add(s);
                    }
                    else
                    {
                        Rule r = Rule.CreateFromSentence(s);
                        if (r != null)
                        {
                            AddRule(r, false);
                        }
                    }
                }                
            }
            
            //export param
            foreach(string s in extractLines)
            {
                //#EXPORTPARAM,1,position
                string[] trunks = s.Split(',');
                int index = int.Parse(trunks[1]);
                string key = trunks[2];
                object pg = subNodes[index].paramGroups[key];
                string extractName = ((ParameterGroup)pg).extractName;
                paramGroups.Add(extractName, pg);

            }

            if (execute) Execute();
            //Debug.Log(">>>>>>>> FLAG 3");
            if(SceneManager.ruleNavigator != null && SceneManager.SelectedGrammar == this)
            {
                //Debug.Log(">>>>>>>> FLAG 4");
                SceneManager.ruleNavigator.UpdateButtonDescriptions();
            }
        }
        public Rule GetRule(int i)
        {
            Rule r = (Rule)subNodes[i];
            return r;
        }
       public void Translate(Vector3 offset)
        {
            //foreach (ShapeObject so in inputs.shapes) so.Translate(offset);
            foreach (ShapeObject so in outputs.shapes) so.Translate(offset);
            foreach(SGIO io in stagedOutputs)
            {
                foreach(ShapeObject so in io.shapes)
                {
                    so.Translate(offset);
                }
            }
        }
        public void UpdateToBuildingPosition()
        {
            if (building == null) return;
            foreach (ShapeObject so in stagedOutputs[stagedOutputs.Count - 1].shapes)
            {
                so.gameObject.transform.position += building.transform.position;
                //so.gameObject.transform.parent = building.transform;
            }
        }

        public void Clear(bool callByDestroy=false)
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
            paramGroups.Clear();
            if (!callByDestroy)
            {
                if (assignedObjects.Count > 0)
                {
                    foreach (ShapeObject o in assignedObjects)
                        o.Show(true);
                        //if (o != null && o.gameObject != null)
                        //    o.gameObject.SetActive(true);
                }
                if (inputs.shapes != null)
                {
                    foreach (ShapeObject o in inputs.shapes)
                    {
                        if (o != null && o.gameObject != null)
                            o.Show(true);
                            //o.gameObject.SetActive(true);
                    }
                }
                
            }
            
        }


    }
}

