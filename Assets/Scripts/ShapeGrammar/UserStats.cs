using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SGCore;

public class UserStats : MonoBehaviour {

    private static Grammar _selectedGrammar;

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
    public static RuleCreator ruleCreator;
    public static Grammar SelectedGrammar
    {
        get { return _selectedGrammar; }
        set
        {
            _selectedGrammar = value;
            if (ruleCreator != null) ruleCreator.grammar = _selectedGrammar;
        }
    }
    public static RuleNavigator UI_RuleList;

    // Use this for initialization
    void Start () {
        ruleCreator = new RuleCreator();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
