using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SGCore;
using SGGeometry;

public class PlanningGrammar
{
    public static Grammar AptStatic(ShapeObject so)
    {
        so.name = "A";
        Grammar g1 = new Grammar();
        g1.name = "g2";
        g1.assignedObjects.Add(so);
        g1.AddRule(new Rules.SizeBuilding3D("A", "A", so.Size), false);
        g1.AddRule(new Rules.ApartmentLoadFilter("A", "SL", "DL", "CV"), false);
        g1.AddRule(new Rules.SingleLoaded("SL", "APT"), false);
        g1.AddRule(new Rules.DoubleLoaded("DL", "APT"), false);
        g1.AddRule(new Rules.CentralVoid("CV", "APT", "APT2"), false);
        return g1;
    }
    public static Grammar OffStatic(ShapeObject so)
    {
        so.name = "A";
        Grammar g1 = new Grammar();
        g1.name = "g2";
        g1.assignedObjects.Add(so);
        g1.AddRule(new Rules.SizeOffice3D("A", "A", so.Size), false);
        g1.AddRule(new Rules.OfficeFilter("A", "SL", "DL", "CV"), false);
        g1.AddRule(new Rules.SingleLoaded("SL", "APT"), false);
        g1.AddRule(new Rules.DoubleLoaded("DL", "APT"), false);
        g1.AddRule(new Rules.CentralVoid("CV", "APT", "APT2"), false);
        return g1;
    }
}
