using UnityEngine;
using System.Collections;

public class GUIBehavior : MonoBehaviour {
	
	public GUIStyle BlueScoreStyle;
	public GUIStyle RedScoreStyle;
	public GUIStyle BlueTurboStyle;
	public GUIStyle RedTurboStyle;
	public int redScore = 0, blueScore = 0;
	public GameObject Swimmer1, Swimmer2, Swimmer3, Swimmer4;
	public SwimmerBehavior Swimmer1Behavior, Swimmer2Behavior, Swimmer3Behavior, Swimmer4Behavior;
	public Camera cam = Camera.main;
	
	void OnGUI(){
		
		Vector3 screenPos = cam.WorldToScreenPoint(Swimmer1.transform.position);
		GUI.Box(new Rect(screenPos.x, Screen.height - screenPos.y, 70, 5), "Name");
		
		//Turbo Bars
		GUI.Label(new Rect(10, 80, Swimmer1Behavior.TurboAmount, 15),"Left Turbo", BlueTurboStyle);
		GUI.Label(new Rect(10, 100, Swimmer2Behavior.TurboAmount, 15), "Right Turbo", BlueTurboStyle);
		GUI.Label(new Rect(Screen.width - 115, 80, Swimmer3Behavior.TurboAmount, 15), "Left Turbo ", RedTurboStyle);
		GUI.Label(new Rect(Screen.width - 115, 100, Swimmer4Behavior.TurboAmount, 15), "Right Turbo ", RedTurboStyle);
		
		GUI.Label(new Rect(10, 10, 100, 100), blueScore.ToString(), BlueScoreStyle);//blueScore
		GUI.Label(new Rect(Screen.width - 115, 10, 100, 100), redScore.ToString(), RedScoreStyle);//redScore
	}	
}
