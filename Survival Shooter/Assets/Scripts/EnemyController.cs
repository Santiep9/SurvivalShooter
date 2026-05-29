using UnityEngine;
using UnityEngine.AI;
using System;
using System.Collections;

public class EnemyController : MonoBehaviour
{
    NavMeshAgent agent;

    [SerializeField] GameObject target;

    [SerializeField] EnemyData data;

    [SerializeField] Animation anim;

    float maxHealth;
    float curHealth;
    public float baseSpeed = 3.5f;

    [SerializeField] GameData gameData;

    [SerializeField] float attackRange = 2f;
    [SerializeField] float attackCooldown = 1f;

    int attackIndex = 1;
    bool attacking = false;

    PlayerController player;

    void Start()
    {
        anim = GetComponent<Animation>();
        agent = GetComponent<NavMeshAgent>();

        Debug.Log(agent.isOnNavMesh);

        agent.speed = baseSpeed * gameData.enemySpeedMultiplier;

        maxHealth = data.GetMaxHealth();
        curHealth = maxHealth;

        anim.Play("Run");
    }

    public void SetTarget(GameObject newTarget)
    {
        target = newTarget;
        player = target.GetComponent<PlayerController>();
    }

    void Update()
    {
        if (!target)
            return;

        float distance = Vector3.Distance(transform.position, target.transform.position);

        if (distance > attackRange)
        {
            Move();
        }
        else
        {
            Attack();
        }
    }

    void Move()
    {
        if (attacking)
            return;

        if (agent != null && agent.isOnNavMesh)
        {
            agent.isStopped = false;
            agent.destination = target.transform.position;

            if (!anim.IsPlaying("Run"))
                anim.CrossFade("Run");
        }
    }

    public void GetDamaged(float amount)
    {
        curHealth -= amount;
        if (curHealth <= 0)
        {
            Destroy(this.gameObject);
        }
    }

    void Attack()
    {
        if (attacking)
            return;

        StartCoroutine(AttackRoutine());
    }

    IEnumerator AttackRoutine()
    {
        attacking = true;

        if (agent != null && agent.isOnNavMesh)
            agent.isStopped = true;

        string attackName = "Attack" + attackIndex;

        anim.Play(attackName);

        if (player != null)
        {
            player.TakeDamage(data.GetDamage());
        }

        attackIndex++;

        if (attackIndex > 3)
            attackIndex = 1;

        yield return new WaitForSeconds(anim[attackName].length);

        attacking = false;
    }
}
