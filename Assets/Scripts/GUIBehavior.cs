using UnityEngine;
using System.Collections;

public class GUIBehavior : MonoBehaviour {
	
	public GUIStyle BlueScoreStyle;
	public GUIStyle RedScoreStyle;
	public GUIStyle BlueTurboStyle;
	public GUIStyle RedTurboStyle;
	public int redScore = 0, blueScore = 0;
	public SwimmerBehavior Swimmer1;
	public SwimmerBehavior Swimmer2;
	public SwimmerBehavior Swimmer3;
	public SwimmerBehavior Swimmer4;
	
	void OnGUI(){
		
		//Turbo Bars
		GUI.Label(new Rect(10, 80, Swimmer1.TurboAmount, 15),"Left Turbo", BlueTurboStyle);
		GUI.Label(new Rect(10, 100, Swimmer2.TurboAmount, 15), "Right Turbo", BlueTurboStyle);
		GUI.Label(new Rect(Screen.width - 115, 80, Swimmer3.TurboAmount, 15), "Left Turbo ", RedTurboStyle);
		GUI.Label(new Rect(Screen.width - 115, 100, Swimmer4.TurboAmount, 15), "Right Turbo ", RedTurboStyle);
		
		GUI.Label(new Rect(10, 10, 100, 100), blueScore.ToString(), BlueScoreStyle);//blueScore
		GUI.Label(new Rect(Screen.width - 115, 10, 100, 100), redScore.ToString(), RedScoreStyle);//redScore
	}	
}
