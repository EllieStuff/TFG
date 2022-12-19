using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [System.Serializable]
    public class AttackData
    {
        public enum Type { NORMAL, FIRE, GRASS, WATER }
        //[HideInInspector] public string name;
        public Type type;
        public GameObject prefab;
        //public Attack()
        //{
        //    name = type.ToString();
        //}
    }


    [SerializeField] internal RoomEnemyManager roomEnemyManager;

    [Header("Attacks")]
    [SerializeField] AttackData.Type currentAttack = AttackData.Type.WATER;
    [SerializeField] float attackDelay = 1.5f;
    [Space]
    //[SerializeField] List<Attack> attacks = new List<Attack>();
    [SerializeField] AttackData fireAttack;
    [SerializeField] AttackData grassAttack, waterAttack;

    Dictionary<AttackData.Type, GameObject> attacksDictionary = new Dictionary<AttackData.Type, GameObject>();

    internal Transform target;
    PlayerMovement playerMovement;
    internal bool canAttack = true;
    float attackTimer;

    bool IsMoving { get { return playerMovement.moveDir != Vector3.zero; } }


    // Start is called before the first frame update
    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();

        attackTimer = attackDelay;
        attacksDictionary.Add(fireAttack.type, fireAttack.prefab);
        attacksDictionary.Add(grassAttack.type, grassAttack.prefab);
        attacksDictionary.Add(waterAttack.type, waterAttack.prefab);

    }

    // Update is called once per frame
    void Update()
    {
        if (CanAttack() && roomEnemyManager.HasEnemiesRemainging())
        {
            Attack();
            attackTimer = attackDelay;
        }

        if (attackTimer > 0) attackTimer -= Time.deltaTime;

    }


    bool CanAttack()
    {
        return canAttack && !IsMoving && attackTimer <= 0;
    }

    void Attack()
    {
        Transform attack = Instantiate(attacksDictionary[currentAttack], transform).transform;
        attack.SetParent(null);
        attack.rotation = playerMovement.targetRot;

    }


}
