using UnityEngine;
using System.Collections;

public class ScoreBehavior : MonoBehaviour {
	
	public GUIStyle scoreStyle;
	public int redScore = 0, blueScore = 0;
	public SwimmerBehavior Swimmer1;
	
	void OnGUI(){
		
		GUI.Label(new Rect(10, 70, 100, 100), Swimmer1.TurboAmount.ToString(), scoreStyle);//Turbo
		
		GUI.Label(new Rect(10, 10, 100, 100), blueScore.ToString(), scoreStyle);//blueScore
		GUI.Label(new Rect(Screen.width - 45, 10, 100, 100), redScore.ToString(), scoreStyle);//redScore
		
	}
}
