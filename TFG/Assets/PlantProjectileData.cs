using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantProjectileData : ProjectileData
{
    const float DISTANCE_SPEED_RELATION = 0.1f;

    [SerializeField] float maxHeight = 5f;
    //[SerializeField] bool testing = false;

    Vector3 initialPos, posLerper;
    Vector3 targetPosLow, targetPosHigh;
    Vector3 highestPosLow, highestPosHigh;
    bool goingUp = true, projectileBehaviour = true;
    float timer = 0, lerpTime = 0;

    public override void Init(Transform _origin)
    {
        base.Init(_origin);
        dmgData.attackElement = _origin.GetComponent<LifeSystem>().entityElement;
        Transform player = GameObject.FindGameObjectWithTag("Player").transform;
        if (player != null)
        {
            initialPos = _origin.position;
            targetPosLow = player.position;
            highestPosLow = CalculateHighestPoint(initialPos, targetPosLow, maxHeight);
            posLerper = highestPosHigh = highestPosLow + new Vector3(0f, highestPosLow.y / 2f, 0f);
            targetPosHigh = targetPosLow + new Vector3(0f, highestPosLow.y / 2f, 0f);
            Debug.Log("HighestPos: " + highestPosLow);
            lerpTime = Vector3.Distance(initialPos, targetPosLow) * DISTANCE_SPEED_RELATION;
            //moveDir = (highestPosHigh - initialPos).normalized;
            //transform.rotation = Quaternion.LookRotation(moveDir, transform.up);
        }
        else
            Destroy(gameObject);
        transform.rotation = Quaternion.LookRotation(moveDir, transform.up);
    }

    private Vector3 CalculateHighestPoint(Vector3 _initPoint, Vector3 _targetPoint, float _maxHeight)
    {
        Vector3 halfPoint = (_targetPoint + _initPoint) / 2f;
        Vector3 heighestPoint = new Vector3(halfPoint.x, halfPoint.y + _maxHeight, halfPoint.z);
        return heighestPoint;
    }


    protected override void Update_Call()
    {
        if (projectileBehaviour)
        {
            timer += Time.deltaTime * moveSpeed;
            Vector3 nextPos = transform.position;
            if (goingUp)
            {
                float lerpValue = timer / lerpTime;
                posLerper = Vector3.Lerp(highestPosHigh, highestPosLow, lerpValue);
                nextPos = Vector3.Lerp(initialPos, posLerper, lerpValue);
                if (timer >= lerpTime) { goingUp = false; timer = 0f; }
            }
            if (!goingUp)
            {
                float lerpValue = timer / lerpTime;
                posLerper = Vector3.Lerp(targetPosHigh, targetPosLow, lerpValue);
                nextPos = Vector3.Lerp(highestPosLow, posLerper, lerpValue);
                if (timer > lerpTime) 
                    projectileBehaviour = false;
            }
            moveDir = (nextPos - transform.position).normalized;
            transform.rotation = Quaternion.LookRotation(moveDir, transform.up);
            transform.position = nextPos;
        }
        else
        {
            //transform.position += moveDir * moveSpeed * Time.deltaTime;
            //transform.rotation = Quaternion.LookRotation(rb.velocity, transform.up);
            base.Update_Call();
            RaycastHit hit;
            if(Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out hit, 5f))
            {
                if (hit.transform.CompareTag("floor")) 
                    DestroyObject(0.3f);
            }
        }

    }


    public override void DestroyObject(float _timer = -1)
    {
        base.DestroyObject(_timer);
    }


    protected override void OnTriggerEnter_Call(Collider other)
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

        //base.OnTriggerEnter_Call(other);
    }

}
