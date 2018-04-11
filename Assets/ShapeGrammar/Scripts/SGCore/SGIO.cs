using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SGCore
{
    public class SGIO
    {
        public List<string> names;
        public List<ShapeObject> shapes;
        public SGIO()
        {
            names = new List<string>();
            shapes = new List<ShapeObject>();
        }
        public SGIO(List<string> inames, List<ShapeObject> ishapes)
        {
            names = inames;
            shapes = ishapes;
        }
        public static SGIO Merge(SGIO io1, SGIO io2)
        {
            List<string> names=new List<string>(io1.names);
            List<ShapeObject> shapes = new List<ShapeObject>(io1.shapes);
            
            foreach(string name in io2.names)
            {
                if (!names.Contains(name)) names.Add(name);
            }
            foreach (ShapeObject shape in io2.shapes)
            {
                if (!shapes.Contains(shape)) shapes.Add(shape);
            }
            return new SGIO(names, shapes);

        }
    }
}

