using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "Scriptable Objects/EnemyData")]
public class EnemyData : ScriptableObject
{
    [SerializeField] float MaxHealth;
    [SerializeField] float Damage;

    public float GetMaxHealth() { return MaxHealth; }
    public float GetDamage() { return Damage; }
}
