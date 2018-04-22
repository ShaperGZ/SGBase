using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SGGeometry;
using SGCore;

public class Draw : Tool {

    //string label = "_draw points";
    List<Vector3> pts = new List<Vector3>();
    
	// Use this for initialization
	void Start () {
        base.Start();
	}

    // Update is called once per frame
    protected override void MouseDown(int button)
    {
        base.MouseDown(button);
        if(button==0)
        {
            Vector3 p = GetMouseInWorld();
        }
        else if (button == 1)
        {
            //finish drawing the polyline and gerenate a polygon
        }
    }
    protected override void MouseMove()
    {
        base.MouseMove();
    }
    public override void OnRenderObject()
    {
        base.OnRenderObject();
        SGGeometry.GLRender.Polyline(pts.ToArray(), true, null, Color.black);
    }


}
