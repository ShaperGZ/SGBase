using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;

namespace SGCore
{
    public class GraphNode
    {
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
        public bool _active;
        public bool active { get { return _active; } }
        public void SetActive(bool flag)
        {
            _active = flag;
        }
        public Properties properties;

        public GraphNode()
        {
            inputs = new SGIO();
            outputs = new SGIO();
            upStreams = new List<GraphNode>();
            downStreams = new List<GraphNode>();
        }

        public virtual void Execute() { }
        public virtual string GetDescription()
        {
            string inName = "";
            if (inputs.names != null && inputs.names.Count > 0) inName = inputs.names[0];

            string txt = "";
            txt += name + " | " + inName + "-->";
            foreach (string n in outputs.names) txt += n + ',';
            return txt;
        }

        public void ConnectDownStream(GraphNode node)
        {
            downStreams.Add(node);
            node.upStreams.Add(this);

        }
        public void DisconnectDownStream(GraphNode node)
        {
            downStreams.Remove(node);
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
                    GameObject.Destroy(outputs.shapes[index].gameObject);
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