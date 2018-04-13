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
        public string name="unnamedNode";
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
                foreach(GraphNode n in downStreams)
                {
                    n.Invalidate();
                }
            }
        }
        public virtual OrderedDictionary DefaultParam()
        {
            return new OrderedDictionary();
        }


    }
}