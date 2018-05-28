using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SGCore;
using SGGeometry;

public class TestMoveAnimateSGBuilding : MonoBehaviour {


	// Use this for initialization
	void Start () {

        List<SGBuilding> buildings = new List<SGBuilding>();
        for (int i = 0; i < 5; i++)
        {
            SOPoint sop = SOPoint.CreatePoint(new Vector3(i*40,0,0));
            SGBuilding building = SGBuilding.CreateApt(sop, new Vector3(30, 60, 15));
            building.Execute();
            
            buildings.Add(building);
        }

        buildings[buildings.Count - 1].ClearAllAssociated();
        buildings.RemoveAt(buildings.Count - 1);

        
        //building.ClearForDestroy();
        //Destroy(sop.gameObject);

        //building.Clear();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
