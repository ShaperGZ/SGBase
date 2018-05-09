using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectStrategy : MonoBehaviour {
    TestParticleSystem testApp;
    Text strategyText;
    // Use this for initialization
    void Start () {
        testApp = GameObject.Find("ScriptLoder").GetComponent<TestParticleSystem>();
        strategyText = transform.Find("Text").GetComponent<Text>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SetBastic()
    {
        testApp.SetParticleSystem(new SGPlaningParticleSystem(testApp.boundary));
        strategyText.text = "basic";
    }
    public void SetAutoHeight()
    {
        testApp.SetParticleSystem(new SGPlaningParticleSystemAH(testApp.boundary));
        strategyText.text = "auto height";
    }
    public void SetAttract()
    {
        testApp.SetParticleSystem(new SGPlaningParticleSystemAT(testApp.boundary));
        strategyText.text = "attract";
    }
    public void SetStepped()
    {
        testApp.SetParticleSystem(new SGPlaningParticleSystemV(testApp.boundary));
        strategyText.text = "stepped";
    }

}
