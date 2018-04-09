using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class test : MonoBehaviour {

	// Use this for initialization
	void Start () {
        string path = Crosstales.FB.FileBrowser.OpenSingleFile("openfileStr", @"c:\","");
        print("sel path=" + path);
          
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
