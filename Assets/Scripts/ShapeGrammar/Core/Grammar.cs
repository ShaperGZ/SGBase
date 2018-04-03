using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

namespace SGCore
{
    public class Grammar:Node
    {
        public int currentStep=-1;
        public int displayStep = -1;
        public List<SGIO> stagedOutputs;
        public List<Rule> rules;
        public List<ShapeObject> assignedObjects;

        public Grammar():base()
        {
            stagedOutputs = new List<SGIO>();
            rules = new List<Rule>();
            assignedObjects = new List<ShapeObject>();
        }
        public void AddRule(Rule r, bool execute = true)
        {
            currentStep = rules.Count;
            displayStep = currentStep;
            rules.Add(r);
            r.grammar = this;
            r.step = rules.Count - 1;
            if (execute)
            {
                Execute(rules.Count-1);
                SelectStep(rules.Count - 1);
            }
            
        }
        public void SubRule(Rule r, bool execute = true)
        {
            int index = rules.IndexOf(r);
            rules.Remove(r);
            if (execute && index > 0 && index < rules.Count)
            {
                Execute(index);
            }
        }
        
        public void Execute()
        {
            for (int i = 0; i < rules.Count; i++)
            {
                Execute(i);
            }
            SelectStep(rules.Count - 1);
        }
        public void Execute(int i)
        {
            currentStep = i;
            SGIO tobeMerged = PreExecution(i);
            //Debug.Log("rule inputSHapes.Count=" + rules[i].inputs.shapes.Count);
            rules[i].step = i;
            rules[i].Execute();
           // Debug.Log("  ---rule outputSHapes.Count=" + rules[i].outputs.shapes.Count);
            PostExecution(i,tobeMerged);

        }
        public void ExecuteFrom(Rule rule)
        {
            int index = rules.IndexOf(rule);
            if (index >= 0 && index < rules.Count)
                ExecuteFrom(index);
            SelectStep(rules.Count - 1);
        }
        public void ExecuteFrom(int start)
        {
            for (int i = start; i < rules.Count; i++)
            {
                Execute(i);
            }
            SelectStep(rules.Count - 1);
        }
        public SGIO PreExecution(int step)
        {
            List<ShapeObject> availableShapes=new List<ShapeObject>();
            List<ShapeObject> outShapes=new List<ShapeObject>();
            SGIO tobeMerged=new SGIO();
            //string txt = string.Format("PreExe step {0}, stg.Count={1}", step, stagedOutputs.Count);
            //Debug.Log(txt);

            //get available shapes
            if (step == 0)
            {
                if (assignedObjects.Count > 0)
                {
                    foreach (ShapeObject so in assignedObjects) so.gameObject.active = false;
                    rules[0].inputs.shapes = assignedObjects;
                    return tobeMerged;
                }
                availableShapes.AddRange(inputs.shapes);
            }
            else
            {
                //before fetching the objects, remove all nul values;
                List<ShapeObject> noNull = new List<ShapeObject>();
                foreach(ShapeObject so in stagedOutputs[step - 1].shapes)
                {
                    if (so != null) noNull.Add(so);
                }
                stagedOutputs[step - 1].shapes = noNull;
                availableShapes.AddRange(stagedOutputs[step-1].shapes);
            }

            //turnoff all available shapes
            //foreach (ShapeObject so in availableShapes) so.gameObject.active = false;

            //select shapes from available shapes base on required names 
            List<ShapeObject> toBeDeleted = new List<ShapeObject>();
            foreach (ShapeObject o in availableShapes)
            {
                try
                {
                    if (rules[step].inputs.names.Contains(o.name))
                        outShapes.Add(o);
                    else
                    {
                        if (!tobeMerged.names.Contains(o.name))
                            tobeMerged.names.Add(o.name);
                        tobeMerged.shapes.Add(o);
                    }
                    
                }
                catch
                {
                    //toBeDeleted.Add(o);
                    Debug.Log("TBD found at "+step + "ShapeObject="+o);
                }
                
            }
            foreach(ShapeObject o in toBeDeleted)
            {
                availableShapes.Remove(o);
            }
            rules[step].inputs.shapes = outShapes;
            return tobeMerged;

        }
        public void PostExecution(int step, SGIO tobeMerged)
        {
            SGIO tempOut=new SGIO();
            tempOut = SGIO.Merge(rules[step].outputs, tobeMerged);
            if (step >= stagedOutputs.Count)
            {
                stagedOutputs.Add(tempOut);
            }
            else
            {
                stagedOutputs[step] = tempOut;
            }
        }
        public void SelectStep(int index)
        {
            if (index < 0 || index >= rules.Count) return;
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
                            o.gameObject.SetActive(false);
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
                if(o!=null)
                    o.gameObject.SetActive(true);
            }

        }

    }
}

