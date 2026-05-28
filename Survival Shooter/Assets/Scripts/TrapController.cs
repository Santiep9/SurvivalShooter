using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class TrapController : MonoBehaviour
{
    [SerializeField] float rootTime = 3f;

    void OnTriggerEnter(Collider other)
    {
        EnemyController enemy = other.GetComponentInParent<EnemyController>();

        if (enemy == null) return;

        StartCoroutine(Root(enemy));
    }

    IEnumerator Root(EnemyController enemy)
    {
        NavMeshAgent agent = enemy.GetComponent<NavMeshAgent>();

        float oldSpeed = agent.speed;

        agent.speed = 0;

        yield return new WaitForSeconds(rootTime);

        if (agent != null)
            agent.speed = oldSpeed;

        Destroy(gameObject);
    }
}