using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SGGeometry;
using System;

namespace SGCore
{
    public class Properties
    {

        
        protected Dictionary<string, object> _properties;
        public virtual Dictionary<string, object> properties
        {
            get
            {
                return _properties;
            }
            set
            {
                _properties = value;
            }
        }
        public List<Grammar> grammars;
        public Properties()
        {
            grammars = new List<Grammar>();
            _properties = new Dictionary<string, object>();
        }
        
        public void AddGrammar(Grammar g)
        {
            grammars.Add(g);
            g.properties = this;
            Invalidate();
        }
        
        public string Format()
        {
            string txt = "Properties:";
            Dictionary<string, object> props = properties;
            foreach(KeyValuePair<string,object> kv in props)
            {
                txt += "\n" + kv.Key + " : " + kv.Value.ToString();
            }
            return txt;
        }
        public virtual void Invalidate()
        {

        }
        public virtual void updateValues(Dictionary<string,object> newValues)
        {

        }
    }
    public class BuildingProperties:Properties
    {
        public Vector3 position
        {
            get { if (refPos != null)
                    return refPos.Position;
                return Vector3.zero;
            }
        }
        public Vector3[] boundary;
        public Vector3 center;
        ShapeObject refPos;
        ShapeObject refSize;
        public void SetRefPos(ShapeObject so)
        {
            refPos = so;
        }
        public void SetRefSize(ShapeObject so)
        {
            refSize = so;
        }
        public override Dictionary<string, object> properties
        {
            get
            {
                //Debug.Log("accessing BuildingProperties get");
                Dictionary<string, object> dict = new Dictionary<string, object>();
                dict["ground"] = ground;
                dict["gfa"] = gfa;
                dict["height"] = height;
                dict["floors"] = floors;
                dict["floorHeight"] = floorHeight;
                dict["footPrint"] = footPrint;
                dict["width"] = width;
                dict["depth"] = depth;
                dict["illumination"] = illumination;
                return dict;
            }
            set
            {
                Dictionary<string, object> dict =value;
                ground = (float)dict["ground"];
                gfa = (float)dict["gfa"];
                height = (float)dict["height"];
                floors = (int)dict["floors"];
                floorHeight = (float)dict["floorHeight"];
                footPrint = (float)dict["footPrint"];
                width = (float)dict["width"];
                depth = (float)dict["depth"];
                illumination = (float)dict["illumination"];
            }
        }

        public float ground = 0;
        public float gfa = -1;
        public float height = -1;
        public int floors = -1;
        public float floorHeight = 3;
        public float footPrint = -1;
        public float width = -1;
        public float depth = -1;
        public float illumination = -1;
        public override void Invalidate()
        {
            gfa = 0;
            height = -1;
            footPrint = 0;
            foreach (Grammar grammar in grammars)
            {
                int lastIndex = grammar.stagedOutputs.Count - 1;
                if (grammar.stagedOutputs == null || lastIndex<0) continue;
                foreach (ShapeObject s in grammar.stagedOutputs[lastIndex].shapes)
                {
                    try
                    {
                        Extrusion ext = (Extrusion)s.meshable;
                        //find highest height
                        float top = ext.polygon.vertices[0].y + ext.height;
                        if (top > height) height = top;
                        //local floor for gfa calculation, add local gfa to global gfa
                        float baseArea = ext.polygon.Area();
                        float flrs = Mathf.Round(ext.height / floorHeight);
                        gfa += (baseArea * flrs);

                        //footprint
                        if (ext.polygon.vertices[0].y == ground)
                            footPrint += baseArea;
                    }
                    catch (Exception e)
                    { Debug.Log("Exception e=:" + e.ToString()); }
                }
                //once we have the buiding height, we can have number of floors
                
            }
            floors = (int)(height / floorHeight);
        }
    }

    public class SiteProperties
    {
        public float gfa = -1;
        public float heightLimit = -1;
        public float density = -1;
        public float greenRatio = -1;
        public List<float> typeMix;
        public List<float> typeAreas;
    }
}
