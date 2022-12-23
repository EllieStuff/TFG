using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProjectileData : MonoBehaviour
{
    [SerializeField] float moveForce = 10f;
    [SerializeField] Vector3 maxVelocity = new Vector3(10, 10, 10);

    DamageData dmgData;
    Rigidbody rb;
    Vector3 moveDir;


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
        rb.AddForce(moveDir * moveForce * Time.deltaTime, ForceMode.Force);
        rb.velocity = ClampVector(rb.velocity, -maxVelocity, maxVelocity);
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

}
