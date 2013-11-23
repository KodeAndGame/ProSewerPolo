using UnityEngine;
using System.Collections;

public class GUIBehavior : MonoBehaviour {
	
	public GUIStyle BlueScoreStyle, RedScoreStyle;
	public GUIStyle BlueTurboStyle, RedTurboStyle;
	public GUIStyle SplashStyle;
	
	public int redScore = 0, blueScore = 0;
	public SwimmerBehavior Swimmer1, Swimmer2, Swimmer3, Swimmer4;
	public int MaxScore = 10;
	
	protected float WinScreenTimer = 0f;
	private bool showPlayText;

	protected string WinScreenMessage = string.Empty;

	
	public void AddPointToScore(bool isRedTeam) {
		if(isRedTeam) {
			redScore++;
		}
		else {
			blueScore++;
		}
		
		if(redScore >= MaxScore || blueScore >= MaxScore) {
			GameStateManager.singleton.State = GameState.WinScreen;

			StartCoroutine(TogglePlay());

			
			if(redScore >= MaxScore) {
				WinScreenMessage = "Red Team Wins";
			}
			else if (blueScore >= MaxScore) {
				WinScreenMessage = "Blue Team Wins";
			}
}
	}
	
	void Update () {
		if (GameStateManager.singleton.State == GameState.WinScreen && WinScreenTimer > 2f && Input.anyKeyDown)
		{
			GameStateManager.singleton.State = GameState.Play;
			Application.LoadLevel("master");
		}
	}
	
	void OnGUI(){
		if(GameStateManager.singleton.State == GameState.WinScreen) {
			GUI.Label(new Rect(Screen.width / 2 - 200, Screen.height / 2 - 100, 200, 200), WinScreenMessage, BlueScoreStyle);
			WinScreenTimer += Time.deltaTime;
			
			if (showPlayText) {  	
				GUI.Label(new Rect((Camera.main.pixelWidth/2 - 200), (Camera.main.pixelHeight/2), 400, 72), "PRESS TO RESTART", SplashStyle);
			}
		}
		
		//Turbo Bars
		GUI.Label(new Rect(10, 80, Swimmer1.TurboAmount, 15),"Left Turbo", BlueTurboStyle);
		GUI.Label(new Rect(10, 100, Swimmer2.TurboAmount, 15), "Right Turbo", BlueTurboStyle);
		GUI.Label(new Rect(Screen.width - (15 + Swimmer3.TurboAmount), 80, Swimmer3.TurboAmount, 15), "Left Turbo ", RedTurboStyle);
		GUI.Label(new Rect(Screen.width - (15 + Swimmer4.TurboAmount), 100, Swimmer4.TurboAmount, 15), "Right Turbo ", RedTurboStyle);
		
		GUI.Label(new Rect(10, 10, 100, 100), blueScore.ToString(), BlueScoreStyle);//blueScore
		GUI.Label(new Rect(Screen.width - 115, 10, 100, 100), redScore.ToString(), RedScoreStyle);//redScore
		
		
	}
	
	IEnumerator TogglePlay () {
		if (GameStateManager.singleton.State == GameState.WinScreen) {
			showPlayText = !showPlayText;
			yield return new WaitForSeconds(0.72f);
			StartCoroutine(TogglePlay());
		}
		else {
			showPlayText = false;
		}
	}
}
