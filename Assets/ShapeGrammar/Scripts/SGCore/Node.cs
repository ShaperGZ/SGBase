﻿using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;

namespace SGCore
{
    public class GraphNode
    {
        public enum Category
        {
            Generic,

            Bd_Planing,
            Bd_Massing,
            Bd_Program,
            Bd_Struct,
            Bd_Graphics,
        }
        public Category category = Category.Generic;
        public DesignContext site;
        public Building building;
        public SGBuilding sgbuilding;

        public SGIO inputs;
        public SGIO outputs;
        public List<GraphNode> upStreams;
        public List<GraphNode> downStreams;
        public Grammar grammar;
        public string name = "unnamedNode";
        public string description = "";
        public bool invalidated = false;
        public OrderedDictionary paramGroups;
        public int step;
        public bool visible = true;
        public bool heavy = false;
        public bool _active;
        public bool active { get { return _active; } }
        public void SetActive(bool flag)
        {
            _active = flag;
        }
        public virtual void SetVisible(bool flag)
        {
            visible = flag;
        }
        public Properties properties;

        public GraphNode()
        {
            inputs = new SGIO();
            outputs = new SGIO();
            upStreams = new List<GraphNode>();
            downStreams = new List<GraphNode>();
        }

        
        public virtual void Execute()
        {
           
        }
        public virtual string GetDescription()
        {
            string inName = "";
            if (inputs.names != null && inputs.names.Count > 0) inName = inputs.names[0];

            string txt = "";
            txt += name + " | " + inName + "-->";
            foreach (string n in outputs.names) txt += n + ',';
            return txt;
        }
        public void ReplaceUpstream(GraphNode org, GraphNode rpl)
        {
            //org : original upstream
            //rpl : new upstream
            org.DisconnectDownStream(this);
            rpl.ConnectDownStream(this);

            //int index = upStreams.IndexOf(org);
            //if (index >= 0)
            //{
            //    upStreams[index].DisconnectDownStream(this);
            //}
            //rpl.ConnectDownStream(this);

        }
        public void ConnectDownStream(GraphNode node)
        {
            if (downStreams == null)
            {
                downStreams = new List<GraphNode>();
            }
            if(!downStreams.Contains(node))
            {
                downStreams.Add(node);
                //Debug.Log("downStream added:" + node.name);
            }
                
            if(!node.upStreams.Contains(this))
            {
                node.upStreams.Add(this);
                //Debug.Log("  upStream added:" + this);
            }
                
            
        }
        public void DisconnectDownStream(GraphNode node)
        {
            if(downStreams.Contains(node))
                downStreams.Remove(node);
            if(node.upStreams.Contains(this))
                node.upStreams.Remove(this);
        }

        public void Invalidate()
        {
            invalidated = true;
            if (downStreams.Count > 0)
            {
                foreach (GraphNode n in downStreams)
                {
                    n.Invalidate();
                }
            }
        }
        public void Clear()
        {
            foreach(ShapeObject o in outputs.shapes)
            {
                try
                {
                    GameObject.Destroy(o);
                }
                catch { }
            }
        }
        public void removeExtraOutputs()
        {
            int dif = outputs.shapes.Count - inputs.shapes.Count;
            if (dif > 0) removeOutputsByCount(dif);

        }
        public void removeOutputsByCount(int num)
        {
            if (num > 0)
            {
                //Debug.Log("dif >0 ouputs.shapesCount="+outputs.shapes.Count);
                for (int i = 0; i < num; i++)
                {
                    int index = outputs.shapes.Count - 1;
                    try
                    {
                        GameObject.Destroy(outputs.shapes[index].gameObject);
                        //outputs.shapes.RemoveAt(index);
                    }
                    catch { }
                    outputs.shapes.RemoveAt(index);
                }
                //Debug.Log("post destroy ouputs.shapesCount=" + outputs.shapes.Count);
            }
        }
        public virtual OrderedDictionary DefaultParam()
        {
            return new OrderedDictionary();
        }
        public virtual void AddParam(string key, float val, float min, float max, float step)
        {
            if (paramGroups == null)
                paramGroups = new OrderedDictionary();
            if (!paramGroups.Contains(key))
                paramGroups[key] = new ParameterGroup();
            ((ParameterGroup)paramGroups[key]).Add(new Parameter(val, min, max, step));
        }
        public virtual void SetParam(string key, int index, float val, float? min = null, float? max = null, float? step = null)
        {
            ParameterGroup pg = (ParameterGroup)paramGroups[key];
            Parameter pm = pg.parameters[index];
            pm.Value = val;
            if (min.HasValue)
                pm.min = min.Value;
            if (max.HasValue)
                pm.max = max.Value;
            if (step.HasValue)
                pm.step = step.Value;
        }
        public virtual void SetParamValCallback(string key, int index, ValueGetter callback)
        {
            Parameter pm = GetParam(key, index);
            pm.getValueCallback = callback;
        }
        public virtual Parameter GetParam(string key, int index)
        {
            ParameterGroup pg = (ParameterGroup)paramGroups[key];
            Parameter pm = pg.parameters[index];
            return pm;
        }
        public virtual float GetParamVal(string key, int index)
        {
            return GetParam(key, index).Value;
        }
        public virtual ParameterGroup GetParamGroup(string key)
        {
            ParameterGroup pg = (ParameterGroup)paramGroups[key];
            return pg;
        }
        
    }
}