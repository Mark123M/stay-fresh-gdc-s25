using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject enemy1_prefab;
    [SerializeField]
    private GameObject enemy2_prefab;
    [SerializeField]
    private GameObject enemy3_prefab;

    private float spawn_interval;

    // Start is called before the first frame update
    void Start()
    {
        spawn_interval = 3f;
        StartCoroutine(spawnEnemy());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator spawnEnemy()
    {
        yield return new WaitForSeconds(spawn_interval);
        GameObject enemy = Instantiate(enemy1_prefab, new Vector3(Random.Range(-5f, 5f), Random.Range(-5f, 5f)), Quaternion.identity);
        StartCoroutine(spawnEnemy());
    }
}
