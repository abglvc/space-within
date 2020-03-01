using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour {
    public GameObject[] panels;
    public Color buttonOnColor, buttonOffColor;
    public Image musicImage, soundImage, helpImage;
    public Text freerunHighscore, levelsUnlockRatio;
    public Sprite[] levelStates;
    public Image[] levels;

    private int currentPanel=0;
    private DAO dao;
    private AudioManager audioManager;

    private void Start() {
        Initialize();
    }

    private void OnDisable() {
        dao.Save();
    }
    
    private void OnApplicationPause(bool p){
        if(p) dao.Save();
    }

    private void Initialize() {
        dao = new DAO();
        audioManager = AudioManager.s_inst;
        audioManager.PlaySounds = dao.data.playSounds;
        audioManager.PlayMusic = dao.data.playMusic;
        
        musicImage.color = dao.data.playMusic ? buttonOnColor : buttonOffColor;
        soundImage.color = dao.data.playSounds ? buttonOnColor : buttonOffColor;
        helpImage.color = dao.data.helpTutorial ? buttonOnColor : buttonOffColor;

        freerunHighscore.text = dao.data.highScores.freerun.ToString();
        levelsUnlockRatio.text = String.Format("{0}/{1}", dao.data.UnlockedLevels, dao.data.levelStates.Count);
        
        for (int i = 0; i<levels.Length; i++)
            levels[i].sprite = levelStates[dao.data.levelStates[i]];
    }

    public void OnFreerunButtonPressed() {
        audioManager.Play2DSound(0);
        dao.data.loadedScene = 1;
        SceneManager.LoadScene(1);
    }

    public void OnQuitButtonPressed() {
        audioManager.Play2DSound(0);
        F1SLink.sng.QuitGameSession();
    }

    public void OnMusicButtonPressed() {
        audioManager.Play2DSound(0);
        dao.data.playMusic = !dao.data.playMusic;
        audioManager.PlayMusic = dao.data.playMusic;
        musicImage.color = dao.data.playMusic ? buttonOnColor : buttonOffColor;
    }

    public void OnSoundButtonPressed() {
        audioManager.Play2DSound(0);
        dao.data.playSounds = !dao.data.playSounds;
        audioManager.PlaySounds = dao.data.playSounds;
        soundImage.color = dao.data.playSounds ? buttonOnColor : buttonOffColor;
    }
    
    public void OnHelpButtonPressed() {
        audioManager.Play2DSound(0);
        dao.data.helpTutorial = !dao.data.helpTutorial;
        helpImage.color = dao.data.helpTutorial ? buttonOnColor : buttonOffColor;
    }

    public void OnInfoButtonPressed() {
        audioManager.Play2DSound(0);
        Debug.Log("Info pressed !");
    }

    public void OnLevelSelected(int level) {
        audioManager.Play2DSound(0);
        if (dao.data.levelStates[level] > 0) {
            dao.data.loadedScene = level + 4;
            SceneManager.LoadScene(level + 4);
        }
    }

    public void MoveToPanel(int panel) {
        audioManager.Play2DSound(0);
        panels[currentPanel].SetActive(false);
        currentPanel = panel;
        panels[currentPanel].SetActive(true);
    }
}