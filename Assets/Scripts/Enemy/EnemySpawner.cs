using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public List<GameObject> enemies = new List<GameObject>();
    public float spawnRate = 1;
    public float range =2;

    private float currentTime; 

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(currentTime <= 0)
        {
            currentTime = 1;
            GameObject enemy = Instantiate(enemies[Random.Range(0, enemies.Count)]);
            Vector2 randomPos = Random.insideUnitCircle;
            randomPos *= range;
            enemy.transform.position = this.transform.position + new Vector3(randomPos.x, 0, randomPos.y);
        }
        else
        {
            currentTime -= Time.deltaTime * spawnRate;
        }
    }
}
