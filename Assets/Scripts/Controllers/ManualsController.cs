using System;
using System.Collections.Generic;
using Other;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Controllers {
    public class ManualsController : MonoBehaviour {
        public static ManualsController sng { get; private set; } //singletone
        
        public Transform obstaclePackHeap;
        public Player.Player playerPrefab;
        public GameObjectPool[] obstaclePacksPool;
        public float packWidths;
        public Slider sensorSlider;
        public Image[] sensors;
        public Color sensorOnColor, sensorOffColor;
        public Text messageText;
        public List<String> messages;
        public GameObject mainMsg, buttons;

        private DAO dao;
        private float visionDistance;
        private float distanceTraveled;
        private float nextObstacleX = 0f;
        private Player.Player player;


        protected void Awake() {
            if (sng == null) sng = this;
            else {
                Destroy(sng);
                sng = this;
            }
            Initialize();
        }

        void Update() {
            distanceTraveled = player.DistanceTraveled();
            if (distanceTraveled + visionDistance > nextObstacleX) SpawnObstaclePack();
        }

        public void OnStartButton() {
            if (dao.data.isLevel)
                SceneManager.LoadScene(4);
            else SceneManager.LoadScene(1);
        }

        private void Initialize() {
            Time.timeScale = 1f;
            messageText.text = messages[0];
            visionDistance = 15f / (16f / 9) * (Camera.main.aspect);
            dao = new DAO();
            player = Instantiate(playerPrefab, transform.parent);
            player.previousDistance = 0;
            player.transform.position = new Vector3(3f, 0, 0);
            InitialiseSensorVisuals();
        }

        private void SpawnObstaclePack() {
            int index = Random.Range(0, obstaclePacksPool.Length);
            GameObject obstaclePack = obstaclePacksPool[index].GetOrSpawnIn(obstaclePackHeap);
            if (obstaclePack) {
                obstaclePack.SetActive(true);
                obstaclePack.transform.position = new Vector3(nextObstacleX + packWidths / 2, 0, 0);
                nextObstacleX += packWidths;
            }
        }

        private int previousDepth = 0;

        public void SliderDepthSensorUpdate() {
            Player.Player.sng.UpdateDepth((int) sensorSlider.value);
        }

        public void UpdateDepth(int depth) {
            for (int i = Mathf.Min(previousDepth, depth); i < Mathf.Max(previousDepth, depth); i++)
                if (previousDepth <= depth)
                    EnableDepthSensor(i, true);
                else EnableDepthSensor(i, false);

            if (depth > previousDepth && mainMsg.activeSelf) {
                messageText.text = messages[1];
                mainMsg.SetActive(false);
            }
            else if (depth < previousDepth) {
                messageText.transform.parent.gameObject.SetActive(false);
                buttons.SetActive(true);
            }

            previousDepth = depth;
        }

        private void EnableDepthSensor(int i, bool b) {
            sensors[i].color = b ? sensorOnColor : sensorOffColor;
        }

        private void InitialiseSensorVisuals() {
            for (int i = 0; i < sensors.Length; i++)
                if (i < sensorSlider.value)
                    EnableDepthSensor(i, true);
                else EnableDepthSensor(i, false);
        }
    }
}