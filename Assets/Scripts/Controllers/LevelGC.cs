using System;
using System.Collections.Generic;
using Other;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Controllers {
    public class LevelGC : GameController {
        [Header("Level")] public AudioClip[] musicTracks;

        private int level = 1;
        private List<int> seriesLevelPack = new List<int>();
        private int it = 0;

        [Serializable]
        public struct SeriesObstPacks {
            public List<int> lvlPacks;
        }

        protected new void Awake() {
            base.Awake();
            Initialize();
        }

        public override float Difficulty(bool completionCheck=false) {
            return (float) it / seriesLevelPack.Count;
        }

        public override int NextObstaclePack() {
            return seriesLevelPack[it];
        }

        public void ConfirmObstPackSpawn() {
            it++;
        }

        public override int CalculateStars() {
            return Mathf.RoundToInt(Difficulty() * 3);
        }

        public override void EndGame() {
            int nStars = CalculateStars();
            int highScore = dao.data.highScores.levels[level];
            int state = dao.data.levelStates[level];
            int oldStars = state - 1;
            bool showCongrats = false;

            if (highScore < player.Score && oldStars <= nStars) {
                showCongrats = true;
                highScore = player.Score;
                state = nStars + 1;

                dao.data.highScores.levels[level] = highScore;
                dao.data.SetLevelState(level, state);
            }

            UserInterface.sng.ShowDeathScreen(String.Format("{0} {1}", gameName, level + 1), highScore, state - 1,
                nStars, showCongrats);
        }

        protected new void Initialize() {
            level = dao.data.loadedLevel.level;

            AudioManager.sng.PlayLoopMusicTrack(musicTracks[Random.Range(0, musicTracks.Length)]);
            for (int i = 0; i < 10 + 5 * (level % 5); i++)
                seriesLevelPack.Add(Random.Range((thisPlanet - 1) * 15, (thisPlanet - 1) * 15 + 15));
        }
    }
}