using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SGCore
{
    public class Node
    {
        public SGIO inputs;
        public SGIO outputs;
        public List<Node> upstreams;
        public List<Node> downStreams;
        public string name="unnamedNode";
        public string description = "";
        public bool invalidated = false;

        public Node()
        {
            inputs = new SGIO();
            outputs = new SGIO();
            upstreams = new List<Node>();
            downStreams = new List<Node>();
        }

        public void ConnectDownStream(Node node)
        {
            downStreams.Add(node);
            node.upstreams.Add(this);

        }
        public void DisconnectDownStream(Node node)
        {
            downStreams.Remove(node);
            node.upstreams.Remove(this);
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


    }
}