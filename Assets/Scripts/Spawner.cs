using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public GameObject enemyCommanderPrefab;
    public float spawnGap = 5.0f;
    public int numberOfSpawns;
    private Transform enemy;
    public int enemiesRemaining;
    int remainingToSpawn;

    public GameMode gameMode;
    public int wave;
    public List<GameObject> potions;
    public float potionSpawnTime = 10.0f;
    public int maxPotions = 5;
    public int currentPotions = 0;
    private Animator waveAnimator;
    void Start()
    {
        gameMode = GameDataManager.Instance.GetGameMode();
        if (gameMode == GameMode.Levels)
        {
            Level level = GameDataManager.Instance.GetLevel();
            numberOfSpawns = ((int)level + 1) * 4;
            LevelModeEnemySpawn();
        }else if(gameMode == GameMode.Infinite){
            waveAnimator = GameObject.Find("WaveCanvas").GetComponent<Animator>();
            wave = 0;
            InfiniteModeEnemySpawn();
        }
        StartCoroutine(PotionSpawner());

    }
    void LevelModeEnemySpawn(){
        enemiesRemaining = numberOfSpawns+1;

        SpawnEnemyCommander();
        Invoke("SpawnEnemiesInBatches",0.2f);
        // SpawnEnemyFormation(numberOfSpawns);
        // RectangleSpawn(numberOfSpawns);
        // InvokeRepeating(nameof(SpawnEnemyRandom), 0f, spawnInterval);
    }
    void InfiniteModeEnemySpawn(){
        wave++;
        numberOfSpawns = wave * 4;
        enemiesRemaining = numberOfSpawns+1;


        waveAnimator.SetTrigger("WaveStart");
        Invoke("CallSetWaveCanvas", 0.5833f);

        Invoke("SpawnEnemyCommander",0.7f);
        Invoke("SpawnEnemiesInBatches",0.9f);
    }
    private void CallSetWaveCanvas()
    {
        GameManager gameManager = GameObject.Find("GameManager").GetComponent<GameManager>(); 
        gameManager.SetWaveCanvas(wave);
    }
    void SpawnEnemyCommander(){
        GameObject enemyCommander = Instantiate(enemyCommanderPrefab, transform.position, enemyCommanderPrefab.transform.rotation); //Change this postion,rotation
        enemyCommander.name = enemyCommanderPrefab.name;
        enemy = enemyCommander.transform;
        
    }
    void SpawnEnemyFormation()
    {
        for (int i = 0; i < numberOfSpawns; i++)
        {
            Vector3 enemyPosition = enemy.position;
            Vector3 enemyForward = enemy.forward;
            Vector3 offset = enemyForward * i * spawnGap;
            Vector3 spawnPos = enemyPosition + offset;
            Instantiate(enemyPrefab, spawnPos, Quaternion.LookRotation(enemyForward));
        }
    }
    void RectangleSpawn(int totalNumber,Vector3 position)
    {
        int sqrt = (int)System.Math.Sqrt(totalNumber);
        Vector3 enemyPosition = position;
        Vector3 enemyForward = enemy.forward;

        int i = 0;
        int rem = 5;
        while(i<totalNumber){
            float x = i % sqrt;
            float z = i / sqrt;
            Vector3 offset = enemyForward * (x * spawnGap) + enemyForward * (z * spawnGap);
            Vector3 spawnPos = enemyPosition + offset + Vector3.up*10f;
            
            if (Physics.Raycast(spawnPos, Vector3.down, out RaycastHit hit, Mathf.Infinity, LayerMask.GetMask("Terrain"))){
                spawnPos = hit.point;
                Instantiate(enemyPrefab, spawnPos, Quaternion.LookRotation(enemyForward));
                remainingToSpawn--;
                i++;
                rem = 5;
            }else{
                rem--;
                if (rem < 0)
                {
                    Debug.LogError("Failed to find a valid NavMesh position for Rectangle spawning");
                    return;
                }
            }

        }
    }
    void TriangularSpawn(int totalNumber,Vector3 position)
    {
        int row = 1;
        int totalSpawned = 0;
        Vector3 enemyPosition = position;
        Vector3 enemyForward = enemy.forward;
        for (int i = 0; i < totalNumber; i++)
        {
            Vector3 offset = enemyForward * (spawnGap * totalSpawned - spawnGap * row / 2) + enemyPosition;
            Vector3 spawnPos = enemyPosition + offset;
            Instantiate(enemyPrefab, spawnPos, Quaternion.LookRotation(enemyForward));
            totalSpawned++;
            if (totalSpawned >= row)
            {
                row++;
                totalSpawned = 0;
            }
        }
    }
    void CircularSpawn(int totalNumber, Vector3 position) //dont use, its not working
    {
        float radiusStep = 4.0f;
        float currentRadius = 6.0f;

        Vector3 enemyPosition = position;
        Vector3 enemyForward = enemy.forward;

        int totalSpawned = 0;

        for (int circle = 1; totalSpawned < totalNumber; circle++)
        {
            int objectsInCircle = Mathf.Min(circle * 2, totalNumber - totalSpawned);

            for (int i = 0; i < objectsInCircle; i++)
            {
                float angle = (360.0f / objectsInCircle) * i;
                Vector3 offset = Quaternion.Euler(0, angle, 0) * (enemyForward * currentRadius);
                Vector3 spawnPos = enemyPosition + offset;

                // Adjust the y-coordinate to make sure the objects are above the terrain
                if (Physics.Raycast(spawnPos, Vector3.down, out RaycastHit hit, Mathf.Infinity, LayerMask.GetMask("Terrain")))
                {
                    spawnPos = hit.point;
                    Instantiate(enemyPrefab, spawnPos, Quaternion.LookRotation(enemyForward));
                    totalSpawned++;
                    remainingToSpawn--;
                }
            }

            currentRadius += radiusStep;
        }
    }


    void SpawnEnemiesInBatches()
    {
        int nearEnemyCount = numberOfSpawns / 2;
        remainingToSpawn = numberOfSpawns;

        RectangleSpawn(nearEnemyCount, enemy.position);

        int trials = 5;
        while (remainingToSpawn > 0){
            Vector3 batchPositionRandom = GetRandomPointOnTerrain();

            if (batchPositionRandom!=Vector3.zero)
            {
                int numSpawnsInBatch = Mathf.Min(Random.Range(3, 7), remainingToSpawn);
                RectangleSpawn(numSpawnsInBatch, batchPositionRandom);

                trials = 5;
            }else{
                if (--trials < 0)
                {
                    Debug.LogError("Failed to find a valid NavMesh position for batch spawning : "+remainingToSpawn+"/"+numberOfSpawns);
                    enemiesRemaining -= remainingToSpawn;
                    return;
                }
            }
        }
    }

    public void EnemyDecrement(){
        enemiesRemaining--;
        if(enemiesRemaining<=0){
            if(gameMode==GameMode.Levels)
                GameObject.Find("GameManager").GetComponent<GameManager>().Invoke("EnemiesDead", 0.8f);
            else if(gameMode==GameMode.Infinite){
                InfiniteModeEnemySpawn();
            }
        }
    }
    public IEnumerator PotionSpawner(){
        float currentTime = 0f;
        while(true){
            if(currentTime>=potionSpawnTime && currentPotions<maxPotions){
                int potionIndex = Random.Range(0,potions.Count);
                Vector3 batchPositionRandom = GetRandomPointOnTerrain();
                if (batchPositionRandom != Vector3.zero)
                    Instantiate(potions[potionIndex], batchPositionRandom + Vector3.up*2.5f, potions[potionIndex].transform.rotation);
                
                currentTime = 0f;
                currentPotions++;
            }
            currentTime += 1f;
            yield return new WaitForSeconds(1f);
        }
    }
    Vector3 GetRandomPointOnTerrain(){
        float minX = -70f;
        float maxX = 70f;
        float minZ = -70f;
        float maxZ = 70f;

        Vector3 randomPosition = new(Random.Range(minX, maxX), 10.0f, Random.Range(minZ, maxZ));

        if (Physics.Raycast(randomPosition, Vector3.down, out RaycastHit hit, Mathf.Infinity, LayerMask.GetMask("Terrain")))
            return hit.point;
        return Vector3.zero;
    }
}
