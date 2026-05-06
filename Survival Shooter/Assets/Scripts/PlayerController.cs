using System;
using System.Collections;
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
    bool canShoot = true;
    [SerializeField] float AttackRange = 5f;
    [SerializeField] float Cooldown = 0.5f;

    NavMeshAgent agent;
    InputActions input;
    InputAction m_interactAction;

    InputAction[] m_switchWeaponAction = new InputAction[3];

    [SerializeField] GameObject[] Weapons = new GameObject[3];
    Iweapon currentWeapon;
    int currentWeaponPosNum;

    Vector3 MousePosition = new Vector3();

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        input = new InputActions();
        input.Main.Enable();

        m_interactAction = input.Main.Interact;

        m_switchWeaponAction = new InputAction[3] { input.Main.Weapon1, input.Main.Weapon2, input.Main.Weapon3 };
    }

    private void Start()
    {
        for (int i = 0; i < Weapons.Length; i++)
        {
            if (i == 0)
            {
                currentWeapon = Weapons[i].GetComponent<Iweapon>();
                currentWeaponPosNum = i;
            }
            else
            {
                Weapons[i].SetActive(false);
            }
        }
    }

    void Update()
    {
        if (m_interactAction.WasPressedThisFrame())
        {
            Move();
        }

        switch (target.Type)
        {
            case TargetType.enemy:
                agent.destination = target.Hit.transform.position;
                float distance = Vector3.Distance(transform.position, agent.destination);
                if (distance <= currentWeapon.GetRange())
                {
                    agent.isStopped = true;
                    transform.LookAt(agent.destination);
                    currentWeapon.Shoot(target.Hit.transform.GetComponent<EnemyController>());
                }
                break;
            case TargetType.position:
            default:
                break;
        }

        for (int i = 0; i < m_switchWeaponAction.Length; i++)
        {
            if (m_switchWeaponAction[i].WasPressedThisFrame())
            {
                if (Weapons[i] & currentWeapon != Weapons[i].GetComponent<Iweapon>())
                {
                    Weapons[currentWeaponPosNum].SetActive(false);
                    Weapons[i].SetActive(true);
                    currentWeapon = Weapons[i].GetComponent<Iweapon>();
                    currentWeapon.SwitchWeapon();
                    currentWeaponPosNum = i;
                    break;
                }
            }
        }
    }

    void Move()
    {
        MousePosition = Mouse.current.position.value;

        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(MousePosition), out hit, 100, mask))
        {
            string LayerName = LayerMask.LayerToName(hit.transform.gameObject.layer);

            switch (LayerName)
            {
                case "Enemy":
                    target = new Target(TargetType.enemy, hit);
                    break;
                case "Floor":
                    target = new Target(TargetType.position, hit);
                    break;
                default:
                    break;
            }

            agent.destination = target.Hit.point;
            agent.isStopped = false;
        }
    }

    void OnDrawGizmos()
    {
        if (target.Type != TargetType.none)
        {
            Gizmos.color = new Color(1f, 1f, 0f, 1f);
            Gizmos.DrawWireSphere(agent.destination, 0.5f);
        }
        Gizmos.color = new Color(1f, 0f, 0f, 1f);
        if (currentWeapon != null) Gizmos.DrawWireSphere(transform.position, currentWeapon.GetRange());
    }
}
