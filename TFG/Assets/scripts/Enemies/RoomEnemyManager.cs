using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomEnemyManager : MonoBehaviour
{
    const float ENEMIES_INIT_WAIT = 2f;
    const float PLAYER_ATTACK_MARGIN = 0.03f;

    [SerializeField] ZoneScript linkedZone;
    [SerializeField] bool roomActive = false;
    [SerializeField] bool roomWithTargetSystem;
    [SerializeField] internal bool notLoadableRoom;

    PlayerAttack playerAttack;
    Rigidbody playerRB;
    LevelInfo levelInfo;

    EnemyPointerScript enemyPointer;
    List<BaseEnemyScript> enemies = new List<BaseEnemyScript>();

    internal GameObject elementChoose;

    bool endRoomEventIsTriggered = false;
    bool changingPointerTarget = false;

    // Start is called before the first frame update
    void Awake()
    {
        playerAttack = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerAttack>();
        levelInfo = playerAttack.GetComponent<LevelInfo>();
        enemyPointer = FindObjectOfType<EnemyPointerScript>();

        linkedZone.assignedRoom = this;
        InitEnemies();
        linkedZone.enemiesQuantity = enemies.Count;
    }

    private void Start()
    {
        playerRB = playerAttack.GetComponent<Rigidbody>();
        
        ActivateEnemies(roomActive);
        SetPlayerData();
    }

    private void Update()
    {
        SearchClosestTarget();
        if (elementChoose == null)
            elementChoose = GameObject.FindGameObjectWithTag("Player").GetComponent<PassiveSkills_Manager>().passiveSkillUI;
    }

    public void UpdateRoomEnemies()
    {
        InitEnemies();
        linkedZone.enemiesQuantity = enemies.Count;
    }

    void InitEnemies()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            BaseEnemyScript enemy = transform.GetChild(i).GetComponent<BaseEnemyScript>();
            if (enemy == null)
                enemy = transform.GetChild(i).GetComponentInChildren<BaseEnemyScript>();

            if (enemy == null)
                Debug.LogWarning("BaseEnemyScript not found");
            else
            {
                enemy.zoneSystem = linkedZone;
                enemies.Add(enemy);
            }
        }
    }

    public bool HasEnemiesRemainging()
    {
        return enemies.Count > 0;
    }

    public Transform GetCloserEnemy(Transform _playerTransform)
    {
        if (!HasEnemiesRemainging())
        {
            if (enemyPointer.IsDifferentTarget(null))
            {
                enemyPointer.SetTarget(null);
                enemyPointer.StopAllCoroutines();
                enemyPointer.StartCoroutine(enemyPointer.Disappear_Cor());
            }
            return null;
        }

        Transform closerEnemy = GetFirstReachableEnemyWithWallCheck();
        if (closerEnemy == null)
        {
            if (enemyPointer.IsDifferentTarget(null))
            {
                enemyPointer.SetTarget(null);
                enemyPointer.StopAllCoroutines();
                enemyPointer.StartCoroutine(enemyPointer.Disappear_Cor());
            }
            return null;
        }
        float closerDist = Vector3.Distance(closerEnemy.position, _playerTransform.position);

        bool reachedAvailableEnemy = false;
        for(int i = 0; i < enemies.Count; i++)
        {
            if(enemies[i] == null)
            {
                enemies.RemoveAt(i);
                i--;
                continue;
            }

            if (!InAttackRange(enemies[i].transform)) continue;

            float newDist = Vector3.Distance(enemies[i].transform.position, _playerTransform.position);
            if((newDist < closerDist || !reachedAvailableEnemy) && PlayerCheck(enemies[i].transform))
            {
                closerEnemy = enemies[i].transform;
                closerDist = newDist;
                reachedAvailableEnemy = true;
            }
        }

        if (closerEnemy == null || !InAttackRange(closerEnemy))
        {
            if (enemyPointer.IsDifferentTarget(null))
            {
                enemyPointer.SetTarget(null);
                enemyPointer.StopAllCoroutines();
                enemyPointer.StartCoroutine(enemyPointer.Disappear_Cor());
            }
            return null;
        }
        else
        {
            if (enemyPointer.IsDifferentTarget(closerEnemy.transform) && !changingPointerTarget)
            {
                enemyPointer.StopAllCoroutines();
                enemyPointer.StartCoroutine(ChangePointerTarget_Cor(closerEnemy));
            }
            return closerEnemy;
        }
    }

    bool InAttackRange(Transform _target)
    {
        if (_target == null) return false;
        Vector2 screenPosition = Camera.main.WorldToScreenPoint(_target.position);
        return screenPosition.x > -Screen.width * PLAYER_ATTACK_MARGIN && screenPosition.x < Screen.width * (1f + PLAYER_ATTACK_MARGIN) 
            && screenPosition.y > -Screen.height * PLAYER_ATTACK_MARGIN && screenPosition.y < Screen.height * (1f + PLAYER_ATTACK_MARGIN);
    }

    private Transform GetFirstReachableEnemyWithWallCheck()
    {
        for (int i = 0; i < enemies.Count; i++)
        {
            if(enemies[i] != null)
            {
                Transform enemy = enemies[i].transform;

                if (PlayerCheck(enemy))
                {
                    return enemy;
                }
            }
        }

        return null;
    }

    private bool PlayerCheck(Transform _enemyTransform)
    {
        RaycastHit hit;
        float raycastDistance = Vector3.Distance(_enemyTransform.position, playerAttack.transform.position);

        Vector3 enemyPos = _enemyTransform.position;

        if (_enemyTransform.name.Contains("Rat"))
            enemyPos.y += 0.5f;

        Ray ray = new Ray(enemyPos, (playerAttack.transform.position - enemyPos).normalized * raycastDistance);

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.tag.Equals("Player"))
                return true;
        }

        return false;
    }

    public void DiscardEnemy(BaseEnemyScript _enemy)
    {
        enemies.Remove(_enemy);
        playerAttack.target = GetCloserEnemy(playerAttack.transform);
    }

    public void ActivateRoom(bool _active)
    {
        roomActive = _active;
        ActivateEnemies(roomActive);
        SetPlayerData();
    }

    bool TargetCheckMode()
    {
        if (roomWithTargetSystem)
        {
            if (playerAttack.target == null || playerRB.velocity.magnitude > 1)
                return true;
        }
        else if (!roomWithTargetSystem)
            return true;

        return false;
    }

    public void SearchClosestTarget()
    {
        //CHEAT MODE TEST
        if (Input.GetKeyDown(KeyCode.F4))
            roomWithTargetSystem = !roomWithTargetSystem;

        if (roomActive && TargetCheckMode())
            playerAttack.target = GetCloserEnemy(playerAttack.transform);
    }

    void SetPlayerData()
    {
        if (roomActive)
        {
            playerAttack.roomEnemyManager = this;
            SearchClosestTarget();
        }
    }

    void ActivateEnemies(bool _active)
    {
        foreach(BaseEnemyScript enemy in enemies)
        {
            enemy.gameObject.SetActive(_active);
        }
    }

    public IEnumerator DisableEnemiesWait()
    {
        yield return new WaitForSeconds(ENEMIES_INIT_WAIT);
        foreach (BaseEnemyScript enemy in enemies)
        {
            enemy.waiting = false;
        }
    }


    IEnumerator ChangePointerTarget_Cor(Transform _enemy)
    {
        changingPointerTarget = true;
        if(enemyPointer.Target != null)
            yield return enemyPointer.Disappear_Cor();
        enemyPointer.SetTarget(_enemy);
        if (_enemy != null) enemyPointer.transform.position = _enemy.position;
        changingPointerTarget = false;
        yield return enemyPointer.Appear_Cor();
    }

}
