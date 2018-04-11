using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class TestTransform : MonoBehaviour {
    Text debugText;
	// Use this for initialization
	void Start () {
        debugText = GameObject.Find("ShapeInspectText").GetComponent<Text>();
	}
	
	// Update is called once per frame
	void Update () {
        string t = "";
        t += "position="+ transform.position.ToString();
        t += "\nrotation="+transform.rotation.eulerAngles.ToString();
        t += "\nlocalPosition="+transform.localPosition.ToString();
        t += "\nlocalRotation="+ transform.localRotation.eulerAngles.ToString();
        t += "\nlocalSacale="+ transform.localScale.ToString();
        debugText.text = t;
	}
}
