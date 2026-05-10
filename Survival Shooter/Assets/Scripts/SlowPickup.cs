using UnityEngine;
using UnityEngine.AI;

public class SlowPickup : MonoBehaviour
{
    [SerializeField] GameData gameData;

    [SerializeField] float slowMultiplier = 0.5f;

    private void Start()
    {
        if (gameData.enemySlowCollected)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerController player =
            other.GetComponentInParent<PlayerController>();

        if (player != null)
        {
            if (!gameData.enemySlowCollected)
            {
                gameData.enemySlowCollected = true;

                gameData.enemySpeedMultiplier = slowMultiplier;

                EnemyController[] enemies =
                    FindObjectsOfType<EnemyController>();

                foreach (EnemyController enemy in enemies)
                {
                    NavMeshAgent agent =
                        enemy.GetComponent<NavMeshAgent>();

                    if (agent != null)
                    {
                        agent.speed = enemy.baseSpeed * gameData.enemySpeedMultiplier;
                    }
                }

                Debug.Log("Enemy Slowed");
            }

            Destroy(gameObject);
        }
    }
}
