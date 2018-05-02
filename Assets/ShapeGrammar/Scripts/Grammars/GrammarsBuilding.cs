using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SGCore;
using SGGeometry;

namespace Grammars
{
    public class Apartment
    {
        public static Grammar SingleLoaded()
        {
            Grammar g = new Grammar();
            g.AddRule(new Rules.PivotTurn("MSL", "MSL", 2));
            g.AddRule(new Rules.BisectLength("MSL", new string[] { "CD", "APT" }, 2, 2));
            g.AddRule(new Rules.CreateStair("CD", "STA"));
            return g;
        }
        public static Grammar DoubleLoaded()
        {
            Grammar g = new Grammar();
            g.AddRule(new Rules.Bisect("MDL", new string[] { "APT","Back" }, 0.45f, 2));
            g.AddRule(new Rules.BisectLength("Back", new string[] { "CD", "APT" }, 2, 2));
            g.AddRule(new Rules.CreateStair("CD", "STA"));
            return g;
        }

    }
}