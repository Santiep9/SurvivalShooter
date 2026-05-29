using System;
using System.Collections;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public enum TargetType
    { none, enemy, position }
    public struct Target
    {
        public Target(TargetType type, RaycastHit hit)
        {
            Type = type;
            Hit = hit;
        }
        public TargetType Type;
        public RaycastHit Hit;
    }

    [SerializeField] LayerMask mask;
    Target target = new Target(TargetType.none, new RaycastHit());

    NavMeshAgent agent;
    InputActions input;
    InputAction m_interactAction;
    InputAction m_saveAction;
    InputAction m_resetAction;
    InputAction switchWeaponAction;

    InputAction m_switchWeaponAction;
    InputAction m_slowShotAction;
    InputAction m_trapAction;

    [SerializeField] WeaponController currentWeapon;
    [SerializeField] Transform barrel;
    [SerializeField] GameObject slowProjectilePrefab;
    [SerializeField] GameObject trapPrefab;
    [SerializeField] Animation anim;

    [SerializeField] GameObject pauseCanvas;

    Vector3 MousePosition = new Vector3();

    [SerializeField] GameData gameData;

    [SerializeField] float maxHealth = 500f;
    [SerializeField] TMP_Text healthText;

    float currentHealth;

    public float baseSpeed = 5f;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        input = new InputActions();
        input.Main.Enable();

        m_interactAction = input.Main.Interact;

        m_switchWeaponAction = input.Main.SwitchWeapon;
        m_slowShotAction = input.Main.SlowShot;
        m_trapAction = input.Main.Trap;

        m_saveAction = input.Main.SaveAndQuit;
        m_resetAction = input.Main.ResetGame;

        switchWeaponAction = input.Main.SwitchWeapon;
    }

    private void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthUI();

        anim = GetComponent<Animation>();

        EnemyController[] enemies = FindObjectsByType<EnemyController>(FindObjectsSortMode.None);

        SaveSystem.Load(gameData, gameObject, enemies);

        agent.speed = baseSpeed * gameData.playerSpeedMultiplier;
    }

    void Update()
    {
        if (m_saveAction.WasPressedThisFrame())
        {
            Pause();
            SaveAndQuit();
        }

        if (!anim.isPlaying)
        {
            if (agent.velocity.magnitude > 0.1f)
                PlayRun();
            else
                PlayIdle();
        }

        if (m_interactAction.WasPressedThisFrame())
        {
            Move();
        }

        if (m_switchWeaponAction.WasPressedThisFrame())
        {
            currentWeapon.SwitchWeapon();
        }

        if (m_slowShotAction.WasPressedThisFrame())
        {
            PlaySpellW();

            Instantiate(slowProjectilePrefab, barrel.position, transform.rotation);

            StartCoroutine(ReturnToMovement(1f));

            Instantiate(slowProjectilePrefab, barrel.position, transform.rotation);
        }

        if (m_trapAction.WasPressedThisFrame())
        {
            PlaySpellE();

            Instantiate(trapPrefab, transform.position, Quaternion.identity);

            StartCoroutine(ReturnToMovement(1f));

            Instantiate(trapPrefab, transform.position, Quaternion.identity);
        }

        switch (target.Type)
        {
            case TargetType.enemy:

                if (target.Hit.transform == null)
                {
                    target = new Target(TargetType.none, new RaycastHit());

                    agent.isStopped = false;
                    break;
                }

                agent.destination = target.Hit.transform.position;

                float distance = Vector3.Distance(transform.position, agent.destination);

                if (distance <= currentWeapon.GetRange())
                {
                    agent.isStopped = true;

                    Vector3 lookPos = target.Hit.transform.position;

                    lookPos.y = transform.position.y;

                    transform.LookAt(lookPos);

                    EnemyController enemy = target.Hit.transform.GetComponentInParent<EnemyController>();

                    if (enemy != null)
                    {
                        currentWeapon.Shoot(enemy);
                    }
                }
                break;
            case TargetType.position:
            default:
                break;
        }

        if (m_resetAction.WasPressedThisFrame())
        {
            ResetGame();
        }
    }

    void Move()
    {
        MousePosition = Mouse.current.position.value;

        RaycastHit hit;
        if (Physics.Raycast(
            Camera.main.ScreenPointToRay(MousePosition),
            out hit,
            100,
            mask))
        {
            Debug.Log("Hit: " + hit.transform.name);

            string LayerName =
                LayerMask.LayerToName(hit.transform.gameObject.layer);

            Debug.Log("Layer: " + LayerName);

            switch (LayerName)
            {
                case "Enemy":
                    target = new Target(TargetType.enemy, hit);
                    break;

                case "Floor":
                    target = new Target(TargetType.position, hit);
                    break;
            }

            agent.destination = target.Hit.point;
            agent.isStopped = false;
        }
    }

    void OnDrawGizmos()
    {
        if (agent != null && target.Type != TargetType.none)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(agent.destination, 0.5f);
        }

        if (currentWeapon != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, currentWeapon.GetRange());
        }
    }

    void SaveAndQuit()
    {
        gameData.playerPosition = transform.position;

        EnemyController[] enemies = FindObjectsByType<EnemyController>(FindObjectsSortMode.None);

        SaveSystem.Save(gameData, enemies);

        Debug.Log("Game Saved!");
    }

    public void CloseCanvas()
    {
        pauseCanvas.SetActive(false);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    void ResetGame()
    {
        SaveSystem.DeleteSave();

        gameData.ResetData();

        Debug.Log("Game Reset");

        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
    }

    void PlayIdle()
    {
        if (currentWeapon.IsRocketMode())
            anim.CrossFade("Jinx_Rlauncher_idle1_anm");
        else
            anim.CrossFade("Idle1_Base");
    }

    void PlayRun()
    {
        if (currentWeapon.IsRocketMode())
            anim.CrossFade("Jinx_Rlauncher_run.anm");
        else
            anim.CrossFade("Run_Base");
    }

    void PlayAttack()
    {
        if (currentWeapon.IsRocketMode())
            anim.Play("Jinx_Rlauncher_attack1_anm");
        else
            anim.Play("Attack1");
    }

    void PlaySpellW()
    {
        anim.Play("Spell2");
    }

    void PlaySpellE()
    {
        anim.Play("Spell3");
    }

    IEnumerator ReturnToMovement(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (!currentWeapon.IsPlayingAction)
        {
            if (agent.velocity.magnitude > 0.1f)
                PlayRun();
            else
                PlayIdle();
        }
    }

    void Pause()
    {
        pauseCanvas.SetActive(true);
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;

        if (currentHealth < 0)
            currentHealth = 0;

        UpdateHealthUI();

        if (currentHealth <= 0)
        {
            currentHealth = 0;

            Debug.Log("GAME OVER");

            #if UNITY_EDITOR
            EditorApplication.isPlaying = false;
            #else
            Application.Quit();
            #endif
        }
    }

    void UpdateHealthUI()
    {
        healthText.text = "Health: " + currentHealth + " / " + maxHealth;
    }
}
