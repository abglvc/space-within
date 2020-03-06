using System;
using System.Collections.Generic;

[Serializable]
public class DataStorage{
    public bool playMusic = true;
    public bool playSounds = true;
    public bool helpTutorial = true;
    
    //0-locked, 1-0stars, 2-1stars, 3-2stars, 4-3stars
    public int freerunState = 1;
    public List<int> levelStates = new List<int>(15);
    public HighScores highScores = new HighScores();
    public bool isLevel;
    public LoadedLevel loadedLevel;
    public bool fullFreerunCycle = false;
    
    public DataStorage() {
        for(int i=0; i<levelStates.Capacity; i++)
            if(i!=0) levelStates.Add(0);
            else levelStates.Add(1);
    }

    public void SetLevelState(int level, int state) {
        levelStates[level] = state;
        if (state > 2 && level+1 < levelStates.Count && levelStates[level+1] == 0)
            levelStates[level+1] = 1; //unlock next level
    }

    public bool PlayNextLevel(int currentLevel) {
        return currentLevel + 1 < levelStates.Count && levelStates[currentLevel] > 2;
    }

    public int UnlockedLevels {
        get {
            for(int i=0; i<levelStates.Count; i++)
                if (levelStates[i] == 0) 
                    return i;
            return levelStates.Count;
        }
    }

    [Serializable]
    public class HighScores {
        public int freerun = 0;
        public List<int> levels = new List<int>(15);

        public HighScores() {
            for (int i = 0; i < levels.Capacity; i++) 
                levels.Add(0);
        }
    }
    
    [Serializable]
    public class LoadedLevel {
        public int level;
        public int planet;

        public LoadedLevel(int level, int planet) {
            this.level = level;
            this.planet = planet;
        }
    }
}