using UnityEngine;

public enum GameState {
	Play,
	WinScreenTransition,
	WinScreen
}

[AddComponentMenu("Singletons/GameState")]
public class GameStateManager : SingletonMonoBehaviour<GameStateManager> { 
    public GameState State = GameState.Play;
}