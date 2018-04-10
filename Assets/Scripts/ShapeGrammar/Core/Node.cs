using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;

namespace SGCore
{
    public class Node
    {
        public SGIO inputs;
        public SGIO outputs;
        public List<Node> upStreams;
        public List<Node> downStreams;
        public Grammar grammar;
        public string name="unnamedNode";
        public string description = "";
        public bool invalidated = false;
        public OrderedDictionary paramGroups;
        public int step;
        

        public Node()
        {
            inputs = new SGIO();
            outputs = new SGIO();
            upStreams = new List<Node>();
            downStreams = new List<Node>();
        }

        public virtual void Execute() { }


        public void ConnectDownStream(Node node)
        {
            downStreams.Add(node);
            node.upStreams.Add(this);

        }
        public void DisconnectDownStream(Node node)
        {
            downStreams.Remove(node);
            node.upStreams.Remove(this);
        }

        public void Invalidate()
        {
            invalidated = true;
            if (downStreams.Count > 0)
            {
                foreach(Node n in downStreams)
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