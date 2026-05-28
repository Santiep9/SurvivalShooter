using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class SlowProjectile : MonoBehaviour
{
    [SerializeField] float speed = 15f;
    [SerializeField] float slowMultiplier = 0.5f;
    [SerializeField] float slowTime = 3f;

    Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        rb.linearVelocity = transform.forward * speed;

        Destroy(gameObject, 5);
    }

    void OnTriggerEnter(Collider other)
    {
        EnemyController enemy = other.GetComponentInParent<EnemyController>();

        if (enemy == null) return;

        StartCoroutine(Slow(enemy));

        Destroy(gameObject);
    }

    IEnumerator Slow(EnemyController enemy)
    {
        NavMeshAgent agent = enemy.GetComponent<NavMeshAgent>();

        float originalSpeed = agent.speed;

        agent.speed *= slowMultiplier;

        yield return new WaitForSeconds(slowTime);

        if (agent != null)
            agent.speed = originalSpeed;
    }
}