using UnityEngine;

[CreateAssetMenu(fileName = "SaveData", menuName = "Scriptable Objects/SaveData")]
public class GameData : ScriptableObject
{
    [Header("Player")]
    public Vector3 playerPosition;
    public float playerSpeedMultiplier = 1f;

    [Header("Enemies")]
    public float enemySpeedMultiplier = 1f;

    [Header("Collectibles")]
    public bool playerSpeedCollected;
    public bool enemySlowCollected;

    public void ResetData()
    {
        playerPosition = Vector3.zero;

        playerSpeedMultiplier = 1f;
        enemySpeedMultiplier = 1f;

        playerSpeedCollected = false;
        enemySlowCollected = false;
    }
}
