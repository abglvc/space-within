using System;
using System.Transactions;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GenerateObstaclePacks : MonoBehaviour {
    public int[] obstacleIndexes;
    public float[] obstDifficultyMultipliers = new[] {1f, 1.75f, 2.5f};
    public int minObstAmount, maxObstAmount;
    public Text dfcText;
    public Obstacle topWall, bottomWall;

    private float[] obstTypeProbability;
    public float[] probsForWalls;
    public Obstacle[] obstaclesWalls;
    
    public float[] probsForSpecials;
    public Obstacle[] specials;
    
    public float[] probsForPickups;
    public Pickup[] pickups;
    
    public float[] probsForEnemies;
    public Enemy[] enemies;
    

    private GameObject gameObjectOP;
    private DFC dific;
    private float koef;
    

    private float longHeight=3.5f, shortHeight=2.5f;
    private bool portalSpawned = false, healSpawned = false;

    private void Update() {
        if(Input.GetKeyDown(KeyCode.G)) ButtonGenerate();
        else if(Input.GetKeyDown(KeyCode.Space)) ButtonRearrange();
        else if(Input.GetKeyDown(KeyCode.R)) ButtonRecreate();
        else if(Input.GetKeyDown(KeyCode.F)) ButtonRecreateAndFreeze();
    }

    public void SavedButton() {
        obstacleIndexes[(int)dific] += 1;
    }

    enum DFC {
        EASY=0, MEDIUM=1, HARD=2
    }

    public void ButtonRecreate() {
        Time.timeScale = 1f;
        if (initialStateOP) {
            Destroy(gameObjectOP);
            gameObjectOP = Instantiate(initialStateOP, transform.parent);
            gameObjectOP.name = "OP " + dific + obstacleIndexes[(int)dific];
            gameObjectOP.SetActive(true);

            foreach (Transform tr in gameObjectOP.transform) {
                tr.gameObject.SetActive(true);
            }
            
            Projectile[] ps = FindObjectsOfType<Projectile>();
            Enemy[] enms = FindObjectsOfType<Enemy>();
            Obstahurt[] deos = FindObjectsOfType<Obstahurt>();
            
            Projectile projectile;
            Enemy ene;
            Obstahurt deo;
            
            for (int i = 0; i < ps.Length; i++) {
                projectile = ps[i];
                projectile.Rb.velocity = projectile.moveDirection * projectile.speed;
            }
            for (int i = 0; i < enms.Length; i++) {
                ene = enms[i];
                ene.SetMovementPathPoints(ene.MovementPathPoints);
            }
            for (int i = 0; i < deos.Length; i++) {
                deo = deos[i];
                deo.MaxHealth = Mathf.RoundToInt(deo.MaxHealth);
                deo.PowerDamage = Mathf.RoundToInt(deo.PowerDamage);
            }
        }
    }

    public void ButtonRecreateAndFreeze() {
        ButtonRecreate();
        Time.timeScale = 0f;
    }

    public void ButtonRearrange() {
        ObstaclePack obstaclePack = gameObjectOP.GetComponent<ObstaclePack>();
        Obstacle[] obstacles = FindObjectsOfType<Obstacle>();
        
        for (int i = 0; i < obstacles.Length; i++) {
            switch (obstacles[i]) {
                case RotaSpikes rotaSpikes:
                    RotaSpikesSetRandomPosition(obstaclePack, rotaSpikes.transform.parent);
                    break;
                case Projectile projectile:
                    ProjectileSetRandomPosition(projectile, obstaclePack, projectile.transform.parent);
                    break;
                case Portal portal:
                    PortalSetRandomPosition(obstaclePack, portal.transform.parent, portal);
                    break;
                case Enemy enemy:
                    EnemySetRandomPosition(enemy, obstaclePack, enemy.transform.parent, true);
                    break;
                case Obstahurt obstahurt:
                    ObstahurtSetRandomPosition(obstaclePack, obstahurt.transform.parent);
                    break;
                case Pickup pickup: 
                    PickupSetRandomPosition(obstaclePack, pickup.transform.parent);
                    break;
                case Obstacle obstacle:
                    if(obstacle.OBSTACLE_INDEX != 0 && obstacle.OBSTACLE_INDEX!=15)
                        WallSetRandomPosition(obstaclePack, obstacle.transform.parent, obstacle.OBSTACLE_INDEX==14);
                    break;
            }
        }
        
        Destroy(initialStateOP);
        initialStateOP = Instantiate(gameObjectOP);
        initialStateOP.name = "initial copy";
        foreach (Transform tr in initialStateOP.transform) {
            tr.gameObject.SetActive(true);
        }
        initialStateOP.SetActive(false);
    }

    
    private GameObject initialStateOP;
    
    public void ButtonGenerate() {
        Time.timeScale = 1f;
        if (gameObjectOP) {
            portalSpawned = false;
            healSpawned = false;
            Destroy(initialStateOP);
            Destroy(gameObjectOP);
        }
        
        bool isShort = Random.value < 0.7f;
        int transformsAmount = Random.Range(isShort?minObstAmount:2*minObstAmount, (isShort?maxObstAmount:2*maxObstAmount)+1);


        int k = Mathf.FloorToInt((isShort ? maxObstAmount : 2 * maxObstAmount) / 3f);
        //2 - 7        2,3  4,5  6,7
        //4 - 14       4,5,6,7   8,9,10,11 12,13,14
        
        // 0f<=dific<=1f
        //1.5f + dific -> 
        
        if (transformsAmount < 2*k) dific = DFC.EASY;
        else if (transformsAmount < 3*k) dific = DFC.MEDIUM;
        else dific = DFC.HARD;
        dfcText.text = dific.ToString();
        
        koef = obstDifficultyMultipliers[(int) dific];
        
        gameObjectOP = new GameObject("OP " + dific + obstacleIndexes[(int)dific]);
        BoxCollider2D boxCollider2D = gameObjectOP.AddComponent<BoxCollider2D>();
        boxCollider2D.isTrigger = true;
        ObstaclePack obstaclePack = gameObjectOP.AddComponent<ObstaclePack>();
        obstaclePack.evenNumberPlatforms = !isShort;
        obstaclePack.height = 10;
        obstaclePack.width = isShort ? 20 : 40;

        Transform t;
        //top wall/s
        for (int j = 0; j < (isShort ? 1 : 2); j++) {
            t = new GameObject("TopWall" + j).transform;
            t.SetParent(gameObjectOP.transform);
            t.position = isShort ? Vector3.up * 4.5f : new Vector3(j==0?-10:10, 4.5f, 0);
            Instantiate(topWall, t);
        }
        //bottom wall/s
        for (int j = 0; j < (isShort ? 1 : 2); j++) {
            t = new GameObject("BottomWall" + j).transform;
            t.SetParent(gameObjectOP.transform);
            t.position = isShort ? Vector3.up * -4.5f : new Vector3(j==0?-10:10, -4.5f, 0);
            Instantiate(bottomWall, t);
        }
        //obstacles inside
        //walls, specials, pickups, enemies
        switch (dific) {
            case DFC.HARD:
                obstTypeProbability = new[] {0.25f, 0.66f, 0.75f, 1f};
                break;
            case DFC.MEDIUM:
                obstTypeProbability = new[] {0.25f, 0.66f, 0.85f, 1f};
                break;
            case DFC.EASY:
                obstTypeProbability = new[] {0.25f, 0.66f, 0.9f, 1f};
                break;
            default:
                obstTypeProbability = new[] {0.25f, 0.66f, 0.9f, 1f};
                break;
        }
        
        for (int i = 0; i < transformsAmount; i++) {
            t = new GameObject().transform;
            t.SetParent(gameObjectOP.transform);
            SpawnEntity(t, obstaclePack, isShort);
            t.name = "T"+i+t.GetChild(0).name;
        }
        
        initialStateOP = Instantiate(gameObjectOP);
        initialStateOP.name = "initial copy";
        foreach (Transform tr in initialStateOP.transform) {
            tr.gameObject.SetActive(true);
        }
        initialStateOP.SetActive(false);
    }

    private void WallSetRandomPosition(ObstaclePack obstaclePack, Transform t, bool longWall, float gap = 0.5f) {
        t.transform.position = new Vector3(MapValue(gap, Random.Range(longWall ? 0f : (-obstaclePack.width/2f + 3f), longWall ? 0f : (obstaclePack.width/2f -3f))),
            MapValue(gap, Random.Range(-shortHeight, shortHeight)));
    }

    private void PortalSetRandomPosition(ObstaclePack obstaclePack, Transform t, Portal portal, float gap = 0.5f) {
        t.transform.position = new Vector3(MapValue(gap, Random.Range(-obstaclePack.width/2f + 1.5f, 0f)), 
            MapValue(gap, Random.Range(-shortHeight, shortHeight)));
                        
        portal.transform.GetChild(0).transform.position = 
            new Vector3(MapValue(gap, Random.Range(t.transform.position.x + 5f, obstaclePack.width/2f - 1.5f)), 
                MapValue(gap, Random.Range(-shortHeight, shortHeight)));
    }

    private void RotaSpikesSetRandomPosition(ObstaclePack obstaclePack, Transform t) {
        t.transform.position = new Vector3(MapValue(0.5f, Random.Range(-obstaclePack.width/2f + 1.5f, obstaclePack.width/2f -1.5f)),
            MapValue(0.5f, Random.Range(-longHeight, longHeight)));
    }
    
    private void ProjectileSetRandomPosition(Projectile p, ObstaclePack obstaclePack, Transform t) {
        p.transform.localPosition = Vector3.zero;
        t.transform.position = new Vector3(MapValue(0.5f, Random.Range(obstaclePack.width/4f, obstaclePack.width/2f)),
            MapValue(0.5f, Random.Range(-longHeight, longHeight)));
    }


    private void ObstahurtSetRandomPosition(ObstaclePack obstaclePack, Transform t) {
        t.transform.position = new Vector3(MapValue(0.5f, Random.Range(-obstaclePack.width/2f + 1.5f, obstaclePack.width/2f -1.5f)),
            MapValue(0.5f, Random.Range(-longHeight, longHeight)));
    }
    
    private void PickupSetRandomPosition(ObstaclePack obstaclePack, Transform t) {
        t.transform.position = new Vector3(MapValue(0.5f, Random.Range(-obstaclePack.width/2f + 1.5f, obstaclePack.width/2f -1.5f)),
            MapValue(0.5f, Random.Range(-longHeight, longHeight)));
    }

    private void EnemySetRandomPosition(Enemy e, ObstaclePack obstaclePack, Transform t, bool existingDots) {
        t.transform.position = new Vector3(MapValue(0.5f, Random.Range(-obstaclePack.width/2f + 1.5f, obstaclePack.width/2f -1.5f)),
            MapValue(0.5f, Random.Range(-longHeight, longHeight)));
        
        if (!existingDots) {
            int dotsAmount = Random.Range(1, 4);
            Transform dot;
            for (int i = 0; i < dotsAmount; i++) {
                dot = new GameObject("dot" + i).transform;
                dot.SetParent(e.transform);
                dot.transform.position = new Vector3(
                    MapValue(0.5f, Random.Range(-obstaclePack.width / 2f + 1.5f, obstaclePack.width / 2f - 1.5f)),
                    MapValue(0.5f, Random.Range(-longHeight, longHeight)));
            }
        } else {
            e.transform.localPosition = Vector3.zero;
            foreach (Transform tr in e.transform)
                if(tr.gameObject.name[0] == 'd')
                    tr.transform.position = new Vector3(
                        MapValue(0.5f, Random.Range(-obstaclePack.width / 2f + 1.5f, obstaclePack.width / 2f - 1.5f)),
                        MapValue(0.5f, Random.Range(-longHeight, longHeight)));
        }
        e.SetMovementPathPoints(e.MovementPathPoints);
    }




    private void SpawnEntity(Transform t, ObstaclePack obstaclePack, bool isShort) {
        int obstType = ProbableIndex(obstTypeProbability, obstTypeProbability.Length);
        int randomInt = 0;

        switch (obstType) {
            case 0: //walls
                randomInt = ProbableIndex(probsForWalls, probsForWalls.Length);
                Obstacle wall = Instantiate(obstaclesWalls[randomInt], t);
                WallSetRandomPosition(obstaclePack, t, wall.OBSTACLE_INDEX == 14);
                break;
            case 1: //specials
                randomInt = ProbableIndex(probsForSpecials, probsForSpecials.Length - (!portalSpawned ? 0:1));

                Obstacle o = Instantiate(specials[randomInt], t);
                switch (o) {
                    case Portal portal:
                        PortalSetRandomPosition(obstaclePack, t, portal);
                        portalSpawned = true;
                        break;
                    case RotaSpikes rotaSpikes:
                        RotaSpikesSetRandomPosition(obstaclePack, t);
                        
                        // BAZNE ZA ODREDJENE OVE * Difficulty() predjenosti koje ce se poslije racunati
                        rotaSpikes.MaxHealth = Mathf.RoundToInt(rotaSpikes.MaxHealth * koef);
                        rotaSpikes.PowerDamage = Mathf.RoundToInt(rotaSpikes.PowerDamage * koef);
                        rotaSpikes.rotationSpeed = Mathf.RoundToInt(rotaSpikes.rotationSpeed * koef);
                        break;
                    case Projectile projectile:
                        ProjectileSetRandomPosition(projectile, obstaclePack, t);
                        
                        projectile.MaxHealth = Mathf.RoundToInt(projectile.MaxHealth * koef);
                        projectile.PowerDamage = Mathf.RoundToInt(projectile.PowerDamage * koef);
                        float baseProjSpeed = projectile.speed;
                        projectile.speed = projectile.speed * koef;
                        projectile.timeAlive =  (isShort ? 4:8) * baseProjSpeed / projectile.speed;
                        
                        projectile.Rb.velocity = projectile.moveDirection * projectile.speed;
                        break;
                    case Obstahurt obstahurt:
                        ObstahurtSetRandomPosition(obstaclePack, t);

                        obstahurt.MaxHealth = Mathf.RoundToInt(obstahurt.MaxHealth * koef);
                        obstahurt.PowerDamage = Mathf.RoundToInt(obstahurt.PowerDamage * koef);
                        break;
                }
                break;
            case 2: //pickups
                PickupSetRandomPosition(obstaclePack, t);
                randomInt = ProbableIndex(probsForPickups, probsForPickups.Length - (!healSpawned ? 0:1));
                
                Pickup p = Instantiate(pickups[randomInt], t);
                switch (p) {
                    case PowerPickup powerPickup:
                        powerPickup.EffectTime = Mathf.RoundToInt(powerPickup.effectTime * koef);
                        break;
                    case HealthPickup healthPickup:
                        healthPickup.HealValue = Mathf.RoundToInt(healthPickup.healValue * koef);
                        healSpawned = true;
                        break;
                }
                break;
            case 3: //enemies
                randomInt = ProbableIndex(probsForEnemies, probsForEnemies.Length);
                Enemy e = Instantiate(enemies[randomInt], t);
                
                EnemySetRandomPosition(e, obstaclePack, t, false);
                
                switch (e) {
                    case Enemyhurt enemyhurt:
                        enemyhurt.MaxHealth = Mathf.RoundToInt(enemyhurt.MaxHealth * koef);
                        enemyhurt.PowerDamage = Mathf.RoundToInt(enemyhurt.PowerDamage * koef);
                        enemyhurt.speed *= koef;
                        enemyhurt.motionType = Random.value < 0.5f ? Enemy.MotionType.Closed : Enemy.MotionType.Open;
                        
                        //enemyhurt.attackRange *= koef;
                        enemyhurt.attackRecharge *= koef;
                        break;
                    case Enemy enemy:
                        enemy.MaxHealth = Mathf.RoundToInt(enemy.MaxHealth * koef);
                        enemy.PowerDamage = Mathf.RoundToInt(enemy.PowerDamage * koef);
                        enemy.speed *= koef;
                        enemy.motionType = Random.value < 0.5f ? Enemy.MotionType.Closed : Enemy.MotionType.Open;
                        break;
                }
                break;
        }
    }
    
    private float MapValue(float fixedGap, float value) {
        return Mathf.RoundToInt(value / fixedGap) * fixedGap;
    }

    private int ProbableIndex(float[] array, int length) {  //mora length!
        float randomValue = Random.value;
        if (array.Length > length) {
            float cof = array.Length / length;
            for (int i = 0; i < length; i++)
                array[i] *= cof;
        }

        for (int i = 0; i < length; i++)
            if (randomValue <= array[i]) return i;
        return length - 1;
    }
}
