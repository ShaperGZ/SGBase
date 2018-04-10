using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DynamicInspector : MonoBehaviour {

    Text debugText;
    ShapeObject so
    {
        get
        {
            return GetComponent<ShapeObject>();
        }
    }
    // Use this for initialization
    void Start () {
        debugText = GameObject.Find("DynamicInspect").GetComponent<Text>();
	}
	
	// Update is called once per frame
	void Update () {
        string t = "";
        t += "\n m.refBbox." + so.meshable.bbox.Format();
        debugText.text = t;
	}
}
