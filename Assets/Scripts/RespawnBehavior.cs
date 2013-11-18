using UnityEngine;
using System.Collections;

public enum RespawnType {
	Center,
	Randomize,
	RandomizeLeftOnly,
	RandomizeRightOnly
}

public class RespawnBehavior : MonoBehaviour {
	
	public RespawnType DefaultRespawnType = RespawnType.Center;
	public Rect RespawnZone;
	
	public Vector2 Respawn () {
		return Respawn(DefaultRespawnType);
	}
	
	public Vector2 Respawn (RespawnType type) {
		switch (type) {
			case RespawnType.Randomize:
				return RespawnRandom();
			case RespawnType.RandomizeLeftOnly:
				return RespawnRandomLeft();
			case RespawnType.RandomizeRightOnly:
				return RespawnRandomRight();
			case RespawnType.Center:
			default:
				return RespawnCenter();
		}
	}
	
	private Vector2 RespawnCenter () {
		return RespawnZone.center;
	}
	
	private Vector2 RespawnRandom () {
		return new Vector2 (
			Random.Range (RespawnZone.xMin, RespawnZone.xMax),
			Random.Range (RespawnZone.yMin, RespawnZone.yMax));
	}
	
	private Vector2 RespawnRandomLeft () {
		return new Vector2 (
			Random.Range (RespawnZone.xMin, RespawnZone.center.x),
			Random.Range (RespawnZone.yMin, RespawnZone.yMax));
	}
	
	private Vector2 RespawnRandomRight () {
		return new Vector2 (
			Random.Range (RespawnZone.center.x, RespawnZone.xMax),
			Random.Range (RespawnZone.yMin, RespawnZone.yMax));
	}
}
