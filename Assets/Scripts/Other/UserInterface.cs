using System;
using System.Collections;
using Controllers;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Other {
    public class UserInterface : MonoBehaviour {
        public static UserInterface sng { get; private set; } //singletone
        public Text mode, planet;

        public Text scoreText, bonusText;
        public GameObject endScreen, pauseScreen;
        public Image frameSplash;
        public RectTransform cautionGraphics;
        public Slider sensorSlider;
        public Image[] sensors;
        public Color sensorOnColor;
        public Color sensorOffColor;

        [Header("End Screen")] public Text gameNameText1;
        public Text gameNameText2;
        public Text highScoreText;
        public Image stateImage;
        public Sprite[] stateSprites;
        public Text endScoreText;
        public GameObject[] stars;
        public GameObject congratsText;
        public GameObject nextLevelMain;
        public GameObject freerunMain;

        private DataStorage data;
        private AudioManager audioManager;

        private void Awake() {
            if (sng == null) sng = this;
            else {
                Destroy(sng);
                sng = this;
            }

            Initialize();
        }

        // Update is called once per frame
        void Update() {
            if (Input.GetKeyDown(KeyCode.Escape)) F1SLink.sng.QuitGameSession();
        }

        public void SliderDepthSensorUpdate() {
            //Player.Player.sng.UpdateDepth((int) sensorSlider.value);
        }

        private int previousDepth = 0;
        public void UpdateDepth(int depth) {
            for (int i = Mathf.Min(previousDepth, depth); i < Mathf.Max(previousDepth, depth); i++)
                if (previousDepth <= depth)
                    EnableDepthSensor(i, true);
                else EnableDepthSensor(i, false);

            previousDepth = depth;
        }

        public void ButtonPauseClicked() {
            audioManager.Play2DSound(0);
            pauseScreen.SetActive(true);
            Time.timeScale = 0f;
        }

        public void ButtonResumeClicked() {
            audioManager.Play2DSound(0);
            pauseScreen.SetActive(false);
            Time.timeScale = 1f;
        }

        public void ButtonRestartClicked() {
            audioManager.Play2DSound(0);
            GameController.sng.BackgroundImage.sortingOrder = 6;
            Destroy(Consingletone.sng.gameObject);
            data.fullFreerunCycle = false;
            if (data.isLevel) SceneManager.LoadScene(4);
            else SceneManager.LoadScene(1);
        }

        public void ButtonMainMenuClicked() {
            audioManager.Play2DSound(0);
            GameController.sng.BackgroundImage.sortingOrder = 6;
            Destroy(Consingletone.sng.gameObject);
            Time.timeScale = 1f;
            SceneManager.LoadScene(0);
        }

        public void ButtonQuitClicked() {
            audioManager.Play2DSound(0);
            AudioManager.sng.Play2DSound(0);
            F1SLink.sng.QuitGameSession();
        }

        public void ButtonNextLevelClicked() {
            data.loadedLevel.level += 1;
            data.loadedLevel.planet = data.loadedLevel.level / 5 + 1;
            Destroy(Consingletone.sng.gameObject);
            SceneManager.LoadScene(4);
        }

        private void EnableDepthSensor(int i, bool b) {
            sensors[i].color = b ? sensorOnColor : sensorOffColor;
        }

        private void Initialize() {
            data = GameController.sng.Dao.data;
            audioManager = AudioManager.sng;

            audioManager.PlaySounds = data.playSounds;
            audioManager.PlayMusic = data.playMusic;

            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            Time.timeScale = 1f;
            InitialiseSensorVisuals();
        }

        private void InitialiseSensorVisuals() {
            for (int i = 0; i < sensors.Length; i++)
                if (i < sensorSlider.value)
                    EnableDepthSensor(i, true);
                else EnableDepthSensor(i, false);
        }

        public void UpdateScore(int score) {
            scoreText.text = score.ToString();
        }

        public void UpdateBonusScore(int bonusScore) {
            bonusText.text = bonusScore.ToString();
        }

        private IEnumerator ShowStars(int nStars) {
            for (int i = 0; i < nStars; i++) {
                yield return new WaitForSeconds(1f);
                stars[i].SetActive(false);
                audioManager.Play2DSound(1);
            }
        }

        public IEnumerator StartModeInfo(String mode, String planet) {
            this.mode.text = mode;
            this.planet.text = planet;
            this.mode.gameObject.SetActive(true);
            yield return new WaitForSeconds(3f);
            this.mode.gameObject.SetActive(false);
        }

        public void ShowDeathScreen(String gameName, int highScore, int state, int nStars, bool showCongratulation) {
            //Debug.Log(String.Format("gamename: {0}, highscore: {1}, state: {2}, stars: {3}, congrats: {4} ", gameName, highScore, state,nStars,showCongratulation));
            audioManager.MusicSource.Stop();
            audioManager.Play2DSound(showCongratulation ? 2 : 3);

            gameNameText1.text = gameName;
            gameNameText2.text = gameName;
            stateImage.sprite = stateSprites[state];
            highScoreText.text = highScore.ToString();
            congratsText.SetActive(showCongratulation);
            endScoreText.text = scoreText.text;

            StartCoroutine(ShowStars(nStars));
            if (!data.isLevel) freerunMain.SetActive(true);
            else if (data.PlayNextLevel(data.loadedLevel.level)) nextLevelMain.SetActive(true);
            else freerunMain.SetActive(true);
            endScreen.SetActive(true);
        }

        public void SignalDanger(float yWorldPosition) {
            RectTransform s = Instantiate(cautionGraphics, transform);
            s.anchoredPosition = new Vector2(0f, yWorldPosition / 5f * 360);
            Destroy(s.gameObject, 2f);
        }
    }
}