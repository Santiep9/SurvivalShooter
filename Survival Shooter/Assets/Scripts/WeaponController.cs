using System.Collections;
using UnityEngine;

public class WeaponController : MonoBehaviour, Iweapon
{
    [SerializeField] WeaponData minigunData;
    [SerializeField] WeaponData rocketData;

    [SerializeField] GameObject minigunTracerPrefab;
    [SerializeField] GameObject rocketTracerPrefab;

    [SerializeField] GameObject user;
    [SerializeField] Transform barrel;
    [SerializeField] Animation anim;

    WeaponData currentData;

    bool rocketMode = false;

    int curAmmo;
    bool canShoot = true;

    public bool IsPlayingAction { get; private set; }

    void Awake()
    {
        anim = GetComponent<Animation>();

        currentData = minigunData;
        curAmmo = currentData.maxAmmo;
    }

    public void Shoot(EnemyController target)
    {
        if (!canShoot || target == null)
            return;

        IsPlayingAction = true;

        if (rocketMode)
            anim.Play("Jinx_Rlauncher_attack1_anm");
        else
            anim.Play("Attack1");

        StartCoroutine(FinishAttack(0.5f));

        Vector3 origin = barrel.position;
        Vector3 direction = (target.transform.position - origin).normalized;

        LayerMask shootMask = ~LayerMask.GetMask("Player");

        RaycastHit hit;
        Vector3 endPoint;

        if (Physics.Raycast(origin, direction, out hit, currentData.range, shootMask))
        {
            endPoint = hit.point;

            EnemyController enemy = hit.transform.GetComponentInParent<EnemyController>();

            if (enemy != null)
            {
                enemy.GetDamaged(currentData.damage);
            }
        }
        else
        {
            endPoint = origin + direction * currentData.range;
        }

        GameObject tracerPrefab = rocketMode ? rocketTracerPrefab : minigunTracerPrefab;

        GameObject tracer = Instantiate(tracerPrefab, barrel.position, barrel.rotation);

        tracer.GetComponent<BulletTracer>().Init(endPoint);

        curAmmo--;

        if (curAmmo <= 0)
            Reload();
        else
            StartCoroutine(WaitFireRate());
    }

    IEnumerator FinishAttack(float time)
    {
        yield return new WaitForSeconds(time);

        IsPlayingAction = false;
    }

    public void Reload()
    {
        canShoot = false;
        StartCoroutine(Reloading());
    }

    public float GetRange()
    {
        if (currentData == null)
            return 0;

        return currentData.range;
    }

    public void SwitchWeapon()
    {
        rocketMode = !rocketMode;

        currentData = rocketMode ? rocketData : minigunData;

        curAmmo = currentData.maxAmmo;

        Debug.Log(rocketMode ? "Rocket Launcher" : "Minigun");
    }

    public bool IsRocketMode()
    {
        return rocketMode;
    }

    IEnumerator WaitFireRate()
    {
        canShoot = false;
        yield return new WaitForSeconds(currentData.fireRate);
        canShoot = true;
    }

    IEnumerator Reloading()
    {
        Debug.Log("Reloading...");
        yield return new WaitForSeconds(currentData.reloadTime);

        curAmmo = currentData.maxAmmo;
        canShoot = true;

        Debug.Log("Reloaded!");
    }
}