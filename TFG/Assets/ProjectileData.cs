using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileData : MonoBehaviour
{
    [SerializeField] protected float moveSpeed = 25f;

    internal DamageData dmgData;
    protected Rigidbody rb;
    internal Vector3 moveDir;
    protected string originTag = "";
    int pierceAmount = 0;
    int bounceAmount = 0;
    protected bool affectedByObstacles = true;
    protected bool destroying = false;
    protected float destroyTimer = -1f;

    public virtual void Init(Transform _origin)
    {
        dmgData = GetComponent<DamageData>();
        rb = transform.GetComponent<Rigidbody>();

        dmgData.ownerTransform = _origin;
        originTag = _origin.tag;
    }


    private void Update()
    {
        Update_Call();
    }
    protected virtual void Update_Call()
    {
        if (rb == null) return;
        rb.MovePosition(transform.position + moveDir * moveSpeed * Time.deltaTime);
        transform.rotation = Quaternion.LookRotation(moveDir, transform.up);
        Debug.Log(moveDir);
    }


    public virtual void DestroyObject(float _timer = -1f)
    {
        if (destroying && _timer >= destroyTimer) return;
        else if (destroying && _timer < destroyTimer) { destroyTimer = _timer; return; }

        if (_timer > 0)
        {
            destroying = true;
            StartCoroutine(DestroyCoroutine(gameObject, _timer));
        }
        else Destroy(gameObject);

    }
    IEnumerator DestroyCoroutine(GameObject _gameObject, float _destroyTime)
    {
        destroyTimer = _destroyTime;
        while(destroyTimer > 0f)
        {
            yield return new WaitForEndOfFrame();
            destroyTimer -= Time.deltaTime;
        }
        Destroy(gameObject);
    }


    private void OnTriggerEnter(Collider other)
    {
        OnTriggerEnter_Call(other);
    }
    protected virtual void OnTriggerEnter_Call(Collider other)
    {
        if (!other.CompareTag(originTag) && (other.CompareTag("Enemy") || other.CompareTag("Player")))
        {
            if (pierceAmount > 0)
            {
                pierceAmount--;
                return;
            }
            DestroyObject();
        }

        //if (other.CompareTag("Player") && !other.CompareTag(originTag))
        //{
        //    if (pierceAmount > 0)
        //    {
        //        pierceAmount--;
        //        return;
        //    }
        //    Destroy_Call(0.1f);
        //}

        if (other.CompareTag("Wall"))
        {
            DestroyObject();
        }

        if (affectedByObstacles && other.CompareTag("Obstacle"))
        {
            DestroyObject(0.1f);
        }

    }

}
