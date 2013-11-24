using UnityEngine;
using System.Collections;


public class SplashScreen : MonoBehaviour {
	public string[] ShootAxes;
	public Texture splashTxt;
	public GUIStyle SplashStyle;
	
	private bool showPlayText = false;
	// Use this for initialization
	void Start () {
		StartCoroutine(TogglePlay());
	}
	
	// Update is called once per frame
	void Update () {

		if(Input.anyKeyDown) {
			Application.LoadLevel("master");
		}
	}
	
	
	// Gui Loop
	void OnGUI () {
		GUI.DrawTexture(new Rect(0, 0, Camera.main.pixelWidth, Camera.main.pixelHeight), splashTxt, ScaleMode.ScaleToFit, false, 1.77f);			
		if (showPlayText) {  	
			GUI.Label(new Rect((Camera.main.pixelWidth/10), (Camera.main.pixelHeight/2), 400, 72), "PRESS SHOOT TO PLAY", SplashStyle);
		}
	}
	
	IEnumerator TogglePlay () {
		showPlayText = !showPlayText;
		yield return new WaitForSeconds(0.72f);
		StartCoroutine(TogglePlay());
	}
	
}
