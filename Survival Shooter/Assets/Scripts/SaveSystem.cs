using UnityEngine;
using System.IO;

public static class SaveSystem
{
    static string path = Application.persistentDataPath + "/save.json";

    [System.Serializable]
    class SaveData
    {
        public Vector3 playerPosition;

        public float playerSpeedMultiplier;
        public float enemySpeedMultiplier;

        public bool playerSpeedCollected;
        public bool enemySlowCollected;

        public Vector3[] enemyPositions;
    }

    public static void Save(GameData dataSave, EnemyController[] enemies)
    {
        SaveData data = new SaveData();

        data.playerPosition = dataSave.playerPosition;

        data.playerSpeedMultiplier = dataSave.playerSpeedMultiplier;
        data.enemySpeedMultiplier = dataSave.enemySpeedMultiplier;

        data.playerSpeedCollected = dataSave.playerSpeedCollected;
        data.enemySlowCollected = dataSave.enemySlowCollected;

        data.enemyPositions = new Vector3[enemies.Length];

        for (int i = 0; i < enemies.Length; i++)
        {
            data.enemyPositions[i] = enemies[i].transform.position;
        }

        string json = JsonUtility.ToJson(data, true);

        File.WriteAllText(path, json);
    }

    public static void Load(GameData dataSave, GameObject player, EnemyController[] enemies)
    {
        if (!File.Exists(path))
            return;

        string json = File.ReadAllText(path);

        SaveData data = JsonUtility.FromJson<SaveData>(json);

        dataSave.playerPosition = data.playerPosition;

        dataSave.playerSpeedMultiplier = data.playerSpeedMultiplier;
        dataSave.enemySpeedMultiplier = data.enemySpeedMultiplier;

        dataSave.playerSpeedCollected = data.playerSpeedCollected;
        dataSave.enemySlowCollected = data.enemySlowCollected;

        player.transform.position = data.playerPosition;

        for (int i = 0; i < enemies.Length; i++)
        {
            if (i < data.enemyPositions.Length)
            {
                enemies[i].transform.position = data.enemyPositions[i];
            }
        }

        Debug.Log("Partida cargada");
    }

    public static void DeleteSave()
    {
        if (File.Exists(path))
        {
            File.Delete(path);

            Debug.Log("Save eliminado");
        }
    }
}