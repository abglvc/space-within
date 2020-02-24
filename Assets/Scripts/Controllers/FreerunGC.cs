using UnityEngine;
using UnityEngine.SceneManagement;

public class FreerunGC : GameController {
    [Header("Freerun")] 
    public float distanceUntilNextPlanet;
    public int nextPlanet;

    public void LoadNextPlanet() {
        Camera.main.GetComponentInChildren<SpriteRenderer>().sortingOrder = 6;
        SceneManager.LoadScene(nextPlanet);
    }

    public override float Difficulty() {
        return distanceTraveled / distanceUntilNextPlanet;
    }

    public override int NextObstaclePack() {
        currentObstPack = Random.Range(0, obstaclePacksPool.Length);
        return currentObstPack;
    }

    public override int CalculateStars() {
        return Mathf.RoundToInt((thisPlanet - 1) + Difficulty());
    }
    
    public override void EndGame() {
        int nStars = CalculateStars();
        int highScore = dao.data.highScores.freerun;
        int state = dao.data.freerunState;
        int oldStars = state - 1;
        bool showCongrats = false;
        
        if (highScore < player.Score && oldStars<=nStars) {
            showCongrats = true;
            highScore = player.Score;
            state = nStars + 1;
            
            dao.data.highScores.freerun = highScore;
            dao.data.freerunState = state;
        }

        UserInterface.sng.ShowDeathScreen(gameName, highScore, state-1, nStars, showCongrats);
    }
}