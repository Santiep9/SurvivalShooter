using UnityEngine;

public interface Iweapon
{
    void Shoot(EnemyController target);
    void Reload();
    float GetRange();
    void SwitchWeapon();
}
