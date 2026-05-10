using UnityEngine;
using UnityEngine.AI;

public class SpeedPickup : MonoBehaviour
{
    [SerializeField] GameData gameData;

    [SerializeField] float speedMultiplier = 1.5f;

    private void Start()
    {
        if (gameData.playerSpeedCollected)
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
            if (!gameData.playerSpeedCollected)
            {
                gameData.playerSpeedCollected = true;

                gameData.playerSpeedMultiplier = speedMultiplier;

                NavMeshAgent agent =
                    player.GetComponent<NavMeshAgent>();

                if (agent != null)
                {
                    agent.speed = player.baseSpeed * gameData.playerSpeedMultiplier;
                }

                Debug.Log("Speed UP!");
            }

            Destroy(gameObject);
        }
    }
}
