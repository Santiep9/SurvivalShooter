using UnityEngine;

[CreateAssetMenu(fileName = "WeaponData", menuName = "Scriptable Objects/WeaponData")]
public class WeaponData : ScriptableObject
{
    public float damage;
    public float range;
    public int maxAmmo;
    public float reloadTime;
    public float fireRate;
}
