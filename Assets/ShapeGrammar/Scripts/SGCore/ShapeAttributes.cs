using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SGCore
{
    public class ShapeAttributes
    {
       
    }

    namespace BuilingTypes
    {
        public class BuildingTypes {
            public Color color = new Color(1, 1, 1);
            public virtual Color GetColor() { return color; }
        }
        public class C1 : BuildingTypes { public override Color GetColor(){ return new Color(1, 0, 0); }}
        public class C2 : BuildingTypes { public override Color GetColor() { return new Color(0.9f, 0.3f, 0); } }
        public class C3 : BuildingTypes { public override Color GetColor() { return new Color(0.8f, 0.6f, 0); } }
        public class R1 : BuildingTypes { public override Color GetColor() { return new Color(1f, 1f, 0); } }
        public class R2 : BuildingTypes { public override Color GetColor() { return new Color(1f, 1f, 0.3f); } }
        public class R3 : BuildingTypes { public override Color GetColor() { return new Color(1f, 1f, 0.6f); } }
        public class Green : BuildingTypes { public override Color GetColor() { return new Color(0.5f, 1f, 0.2f); } }
    }
    
    

}

