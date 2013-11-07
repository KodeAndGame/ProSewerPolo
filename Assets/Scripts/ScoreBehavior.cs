using UnityEngine;
using System.Collections;

public class ScoreBehavior : MonoBehaviour {
	
	public GUIStyle scoreStyle;
	public int redScore = 0, blueScore = 0;
	
	void OnGUI(){
		
		GUI.Label(new Rect(10, 10, 100, 100), blueScore.ToString(), scoreStyle);//blueScore
		GUI.Label(new Rect(Screen.width - 45, 10, 100, 100), redScore.ToString(), scoreStyle);//redScore
		
	}
}
