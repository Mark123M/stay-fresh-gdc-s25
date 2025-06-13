using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public float spawn_increase_rate = 0.2f;
    [SerializeField]
    private EnemyController enemy1_prefab;
    [SerializeField]
    private EnemyController enemy2_prefab;
    [SerializeField]
    private EnemyController enemy3_prefab;

    public float spawn_interval;

    public List<EnemyController> enemies;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(spawnEnemy());
    }

    // Update is called once per frame
    void Update()
    {
          
    }
    private void FixedUpdate()
    {
        spawn_interval = Mathf.Clamp(spawn_interval - spawn_increase_rate * Time.fixedDeltaTime, 0.5f, 100f) ;
    }

    private IEnumerator spawnEnemy()
    {
        yield return new WaitForSeconds(spawn_interval);
        EnemyController enemy = Instantiate(enemy1_prefab, new Vector3(Random.Range(-5f, 5f), Random.Range(-5f, 5f)), Quaternion.identity);
        //enemies.Add(enemy);
        StartCoroutine(spawnEnemy());
    }
}
