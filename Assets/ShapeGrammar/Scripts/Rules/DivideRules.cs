using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SGCore;

namespace Rules
{
    public class DivideToFTFH2 : DivideTo
    {
        public DivideToFTFH2() : base()
        {
            name = "DivideTo";
            inputs.names.Add("A");
            outputs.names.Add("A");
        }
        public DivideToFTFH2(string inName, string outName, float ds) : base(inName, outName, ds, 1)
        {
            name = "DivideTo";
        }
        public DivideToFTFH2(string inName, string[] outNames, float ds) : base(inName, outNames,ds, 1)
        {
            name = "DivideTo";
        }
        public virtual List<float> GetDivs(float ftfh, ShapeObject so)
        {
            
            if (ftfh == 0) throw new System.Exception("ftfh can not be zero!");
            List<float> outDivs = new List<float>();

            float bot = so.Position.y;
            float top = bot + so.Size[1];
            float totalH = top - bot;
            Debug.LogFormat("bot={0}, top={1}", bot, top);

            float h = Mathf.Ceil(bot / ftfh);
            float trunk = (h - bot);
            if (trunk != 0)
                outDivs.Add(trunk);
            while (h < top)
            {
                if (h + ftfh < top)
                {
                    outDivs.Add(ftfh);
                }
                else
                {
                    trunk = top - h;
                    if (trunk != 0)
                        outDivs.Add(trunk);
                    break;
                }
                h += ftfh;
                Debug.Log("h=" + h);
            }

            if (outDivs.Count == 0) throw new System.Exception("outDives is empty");
            foreach (float f in outDivs) Debug.Log(f);
            return outDivs;
        }
    }

}
