using System.Collections;
using UnityEngine;

public class WeaponController : MonoBehaviour, Iweapon
{
    [SerializeField] WeaponData weaponData;

    [SerializeField] GameObject user;
    [SerializeField] Transform barrel;

    int curAmmo;
    bool canShoot = true;

    void Awake()
    {
        curAmmo = weaponData.maxAmmo;
    }

    public void Shoot(EnemyController target) 
    { 
        if (canShoot == false) return;

        RaycastHit hit;
        Vector3 origin = barrel.position;
        Vector3 direction = user.transform.forward;
        Vector3 endPoint;

        if (Physics.Raycast(origin, direction, out hit, weaponData.range))
        {
            endPoint = hit.point;

            if (hit.transform.GetComponent<EnemyController>())
            {
                hit.transform.GetComponent<EnemyController>().GetDamaged(weaponData.damage);
            }
        }
        else
        {
            endPoint = origin + direction * weaponData.range;
        }

        --curAmmo;

        Debug.Log("Ammo: " + curAmmo);

        if (curAmmo == 0) Reload();
        else StartCoroutine(WaitFireRate());
    }

    public void Reload()
    {
        canShoot = false;
        StartCoroutine(Reloading());
    }

    public float GetRange() { return weaponData.range; }

    public void SwitchWeapon()
    {

    }

    IEnumerator WaitFireRate()
    {
        canShoot = false;
        yield return new WaitForSeconds(weaponData.fireRate);
        canShoot = true;
    }

    IEnumerator Reloading()
    {
        Debug.Log("Reloading...");
        yield return new WaitForSeconds(weaponData.reloadTime);
        curAmmo = weaponData.maxAmmo;
        canShoot = true;
        Debug.Log("Reloaded!");
    }
}
