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
    protected bool destroying = false;

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
        transform.rotation = Quaternion.LookRotation(rb.velocity, transform.up);
    }


    public virtual void DestroyObject(float _timer = -1f)
    {
        if (destroying) return;

        if (_timer > 0)
        {
            destroying = true;
            Destroy(gameObject, _timer);
        }
        else Destroy(gameObject);
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
            DestroyObject(0.1f);
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
            if (bounceAmount > 0)
            {
                bounceAmount--;
                //DoBound
                return;
            }
            DestroyObject(0.1f);
        }
    }

}
