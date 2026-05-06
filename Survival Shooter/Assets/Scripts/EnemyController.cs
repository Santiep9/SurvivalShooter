using UnityEngine;
using UnityEngine.AI;
using System;

public class EnemyController : MonoBehaviour
{
    NavMeshAgent agent;

    [SerializeField] GameObject target;

    [SerializeField] EnemyData data;

    float maxHealth;
    float curHealth;

    void Start()
    {
        maxHealth = data.GetMaxHealth();
        curHealth = maxHealth;

        agent = GetComponent<NavMeshAgent>();
    }

    public void SetTarget(GameObject newTarget)
    {
        target = newTarget;
    }

    void Update()
    {
        if (target)
        {
            Move();
        }
    }
    void Move()
    {
        agent.destination = target.transform.position;
    }

    public void GetDamaged(float amount)
    {
        curHealth -= amount;
        if (curHealth <= 0)
        {
            Destroy(this.gameObject);
        }
    }
}
