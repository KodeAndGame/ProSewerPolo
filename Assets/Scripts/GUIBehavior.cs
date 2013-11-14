using UnityEngine;
using System.Collections;

public class GUIBehavior : MonoBehaviour {
	
	public GUIStyle BlueScoreStyle, RedScoreStyle;
	public GUIStyle BlueTurboStyle, RedTurboStyle;
	public GUIStyle MenuStyle;
	public int RedScore = 0, BlueScore = 0;
	public SwimmerBehavior Swimmer1, Swimmer2, Swimmer3, Swimmer4;
	
	private int ScreenWidth = Screen.width;
	private int ScreenHeight = Screen.height;
	
	void OnGUI(){
		
		//Turbo Bars
		GUI.Label(new Rect(10, 80, Swimmer1.TurboAmount, 15),"Left Turbo", BlueTurboStyle);
		GUI.Label(new Rect(10, 100, Swimmer2.TurboAmount, 15), "Right Turbo", BlueTurboStyle);
		GUI.Label(new Rect(ScreenWidth - 115, 80, Swimmer3.TurboAmount, 15), "Left Turbo ", RedTurboStyle);
		GUI.Label(new Rect(ScreenWidth - 115, 100, Swimmer4.TurboAmount, 15), "Right Turbo ", RedTurboStyle);
		
		GUI.Label(new Rect(10, 10, 100, 100), BlueScore.ToString(), BlueScoreStyle);//blueScore
		GUI.Label(new Rect(ScreenWidth - 115, 10, 100, 100), RedScore.ToString(), RedScoreStyle);//redScore
		
		if (Input.GetKey("escape"))
  			MainMenu();
	}	
	
	void MainMenu(){
		if (GUI.Button(new Rect(ScreenWidth/4, ScreenHeight/4, ScreenWidth/6, ScreenWidth/6),"Start Game", MenuStyle))
			GameReset();
		if (GUI.Button(new Rect((ScreenWidth/4)+ScreenWidth/3, Screen.height/4, ScreenWidth/6, ScreenWidth/6),"Quit", MenuStyle)){
			Application.Quit();
			#if UNITY_EDITOR
    			UnityEditor.EditorApplication.isPlaying = false;
			#endif
		}
	}
	
	void GameReset(){
		Application.LoadLevel(0);
	}
	
}
