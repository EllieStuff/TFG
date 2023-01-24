using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProjectileData : MonoBehaviour
{
    [SerializeField] float moveSpeed = 25f;

    internal DamageData dmgData;
    Rigidbody rb;
    Vector3 moveDir;
    int pierceAmount = 0;
    int bounceAmount = 0;

    public void Init(PlayerAttack _player)
    {
        dmgData = GetComponent<DamageData>();
        rb = transform.GetComponent<Rigidbody>();
        dmgData.attackElement = _player.currentAttackElement;
        dmgData.ownerTransform = _player.transform;
        if (_player.target != null)
            moveDir = (_player.target.position - _player.transform.position).normalized;
        else
            moveDir = _player.transform.forward;
        transform.rotation = Quaternion.LookRotation(moveDir, transform.up);
    }


    private void Update()
    {
        if (rb == null) return;
        rb.MovePosition(transform.position + moveDir * moveSpeed * Time.deltaTime);
        transform.rotation = Quaternion.LookRotation(rb.velocity, transform.up);
    }


    Vector3 ClampVector(Vector3 _originalVec, Vector3 _minVec, Vector3 _maxVec)
    {
        return new Vector3(
            Mathf.Clamp(_originalVec.x, _minVec.x, _maxVec.x),
            Mathf.Clamp(_originalVec.y, _minVec.y, _maxVec.y),
            Mathf.Clamp(_originalVec.z, _minVec.z, _maxVec.z)
        );
    }


    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Enemy"))
        {
            if(pierceAmount > 0)
            {
                pierceAmount--;
                return;
            }
            Destroy(gameObject, 0.1f);
        }

        if (other.CompareTag("Wall"))
        {
            if (bounceAmount > 0)
            {
                bounceAmount--;
                //DoBound
                return;
            }
            Destroy(gameObject, 0.1f);
        }
    }

}
