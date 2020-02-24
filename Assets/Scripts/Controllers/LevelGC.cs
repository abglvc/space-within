using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGC : GameController {
    [Header("Level")] 
    public int level=1;
    public int[] seriesObstPacks;
    private int it = 0;
    
    public override float Difficulty() {
        return (float) it / seriesObstPacks.Length;
    }

    public override int NextObstaclePack() {
        return seriesObstPacks[it];
    }

    public void ConfirmObstPackSpawn() { it++; }

    public override int CalculateStars() {
        Debug.Log(Difficulty());
        return Mathf.RoundToInt(Difficulty() * 3);
    }
    
    public override void EndGame() {
        int nStars = CalculateStars();
        int highScore = dao.data.highScores.levels[level-1];
        int state = dao.data.levelStates[level - 1];
        int oldStars = state - 1;
        bool showCongrats = false;
        
        if (highScore < player.Score && oldStars<=nStars) {
            showCongrats = true;
            highScore = player.Score;
            state = nStars + 1;
            
            dao.data.highScores.levels[level - 1] = highScore;
            dao.data.SetLevelState(level - 1, state);
        }
        
        UserInterface.sng.ShowDeathScreen(String.Format("{0} {1}", gameName, level), highScore, state-1, nStars, showCongrats);
    }
}
