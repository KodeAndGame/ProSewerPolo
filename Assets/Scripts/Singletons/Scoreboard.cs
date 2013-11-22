using UnityEngine;

[AddComponentMenu("Singletons/Scoreboard")]
public class Scoreboard : SingletonMonoBehaviour<Scoreboard> {
 
    public int teamBlueScore, teamRedScore;
 
    public static void AddPoints(bool redTeam, int points) { 
        if( !redTeam ) singleton.teamBlueScore += points;
        else singleton.teamRedScore += points;
    }
}