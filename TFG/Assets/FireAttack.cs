using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireAttack : MonoBehaviour
{
    [SerializeField] GameObject disappearEffect;
    [SerializeField] float dmg = 10;
    [SerializeField] float moveForce;
    [SerializeField] Vector3 cadency = Vector3.zero;

    Rigidbody rb;
    PlayerMovement playerMov;
    bool lockPlayerMove;



    public void Initialize(PlayerMovement _playerMov, bool _lockPlayerMove)
    {
        rb = GetComponent<Rigidbody>();

        playerMov = _playerMov;
        lockPlayerMove = _lockPlayerMove;
        playerMov.canMove = playerMov.canRotate = !lockPlayerMove;
    }

    public void Shoot(Vector3 _moveDir)
    {
        Vector3 finalMove = _moveDir * moveForce;
        Vector3 rndCadency = GetRndVector3(-cadency, cadency);
        finalMove = finalMove + rndCadency;
        rb.AddForce(finalMove, ForceMode.Impulse);
    }

    void Despawn(Vector3 _feedbackSpawnPos)
    {
        playerMov.canMove = playerMov.canRotate = true;
        GameObject.Instantiate(disappearEffect, _feedbackSpawnPos, Random.rotation);
        Destroy(gameObject);
    }


    Vector3 GetRndVector3(Vector3 _minValue, Vector3 _maxValue)
    {
        return new Vector3(
            Random.Range(_minValue.x, _maxValue.x), 
            Random.Range(_minValue.y, _maxValue.y), 
            Random.Range(_minValue.z, _maxValue.z)
        );
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            //other.GetComponent<Health>().Damage(dmg);
            Despawn(other.ClosestPoint(transform.position));
        }
        if (other.CompareTag("Wall"))
        {
            Despawn(other.ClosestPoint(transform.position));
        }
    }

    //Fer cas si surt de la pantalla

}
