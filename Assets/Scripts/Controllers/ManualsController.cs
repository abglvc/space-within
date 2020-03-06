using System;
using System.Collections.Generic;
using MyObjectPooling;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class ManualsController : MonoBehaviour {
    public Transform obstaclePackHeap;
    public Player playerPrefab;
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
    private Player player;
    
    
    protected void Awake() {
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
        int depth = (int) sensorSlider.value;
        Player.sng.UpdateDepth(depth);
        for(int i=Mathf.Min(previousDepth, depth); i<Mathf.Max(previousDepth, depth); i++)
            if (previousDepth <= depth)
                EnableDepthSensor(i, true);
            else EnableDepthSensor(i, false);

        if (depth > previousDepth && mainMsg.activeSelf) {
            messageText.text = messages[1];
            mainMsg.SetActive(false);
        }else if (depth < previousDepth) {
            messageText.transform.parent.gameObject.SetActive(false);
            buttons.SetActive(true);
        }
        previousDepth = depth;
    }
    
    public void EnableDepthSensor(int i, bool b) {
        sensors[i].color = b ? sensorOnColor : sensorOffColor;
    }
    
    private void InitialiseSensorVisuals() {
        for (int i = 0; i < sensors.Length; i++)
            if (i < sensorSlider.value)
                EnableDepthSensor(i, true);
            else EnableDepthSensor(i, false);
    }
}
