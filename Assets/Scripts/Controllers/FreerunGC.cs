using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FreerunGC : GameController {
    [Header("Freerun")]
    public float distanceUntilNextPlanet;
    public int nextPlanet;
    private SpriteRenderer srBackground;
    const float TRAVEL_TIME=0.8f;

    private void Start() {
        srBackground = Camera.main.GetComponentInChildren<SpriteRenderer>();
    }

    public IEnumerator LoadNextPlanet(Player p){
        p.InPortal(true);
        yield return new WaitForSeconds(TRAVEL_TIME);
        p.InPortal(false);
        srBackground.sortingOrder = 6;
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
        int previousHighScore = dao.data.highScores.freerun;
        int stars = 3;
        if (previousHighScore != 0) stars = Mathf.RoundToInt((float) player.Score / previousHighScore * 3);
        return stars > 3 ? 3 : stars;
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