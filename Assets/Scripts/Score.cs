using UnityEngine;
using System.Collections;

public class Score : MonoBehaviour {
	
	void OnGUI(){
		
		GUI.Label(new Rect(10, 10, 500, 500), "redScore");
		GUI.Label(new Rect(1600, 10, 500, 500), "blueScore");
		
	}
}
