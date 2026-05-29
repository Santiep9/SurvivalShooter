using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class SpawnerController : MonoBehaviour
{
    [SerializeField] GameObject Player;
    [SerializeField] GameObject Enemy;
    [SerializeField] float SpawnRange = 2f;
    [SerializeField] float SpawnCooldown = 0.5f;
    bool stopSpawner = false;

    void Start()
    {
        StartCoroutine(Cooldown());
    }
    IEnumerator Cooldown()
    {
        yield return new WaitForSeconds(SpawnCooldown);
        while (!stopSpawner)
        {
            SpawnEnemy();
            yield return new WaitForSeconds(SpawnCooldown);
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = new Color(1f, 0f, 0f, 1f);
        Gizmos.DrawWireSphere(transform.position, SpawnRange);
    }

    void SpawnEnemy()
    {
        float distance = Random.Range(0, SpawnRange);
        float angle = Random.Range(0, 360);

        Vector3 newPosition = transform.position
            + Quaternion.Euler(0, angle, 0) * Vector3.forward * distance;

        NavMeshHit hit;

        if (NavMesh.SamplePosition(newPosition, out hit, 5f, NavMesh.AllAreas))
        {
            GameObject spawn = Instantiate(Enemy, hit.position, Quaternion.identity);
            spawn.GetComponent<EnemyController>().SetTarget(Player);
        }
    }
}
