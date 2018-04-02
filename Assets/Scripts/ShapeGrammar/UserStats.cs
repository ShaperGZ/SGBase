using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SGCore;

public class UserStats : MonoBehaviour {

    public static Material lineMat;
    public static Material LineMat
    {
        get
        {
            if (lineMat == null)
                lineMat = Resources.Load("LineMaterial") as Material;
            return lineMat;
        }
    }
    public static Grammar SelectedGrammar;
    public static ExpandableList UI_RuleList;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
