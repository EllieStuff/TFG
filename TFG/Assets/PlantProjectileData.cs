using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantProjectileData : ProjectileData
{
    const float THRESHOLD = 0.3f;

    [SerializeField] float maxHeight = 5f;
    [SerializeField] float speed = 2;

    Vector3 initialPos, targetPos;
    Vector3 highestPosLow, highestPosHigh, posLerper;
    Vector3 initialVel;
    //float initialY;
    float midDistance;
    float initialTime;
    float distanceTime_Relation = 0.01f;
    bool goingUp = true;
    float timer = 0;

    public override void Init(Transform _origin)
    {
        base.Init(_origin);
        dmgData.attackElement = _origin.GetComponent<LifeSystem>().entityElement;
        Transform player = GameObject.FindGameObjectWithTag("Player").transform;
        if (player != null)
        {
            initialTime = Time.timeSinceLevelLoad;
            //initialY = transform.position.y;
            initialPos = _origin.position;
            targetPos = player.position;
            highestPosLow = CalculateHighestPoint(initialPos, targetPos, maxHeight);
            posLerper = highestPosHigh = highestPosLow + new Vector3(0f, highestPosLow.y / 2f, 0f);
            moveDir = (highestPosHigh - initialPos).normalized;
            midDistance = Vector3.Distance(targetPos, highestPosLow);
            Debug.Log("HighestPos: " + highestPosLow);
            Debug.Log("MidDistance: " + midDistance);

            //float throwAngle = CalculateThrowAngle(initialPos, targetPos, maxHeight);
            //float throwForce = CalculateThrowForceFromAngle(initialPos, targetPos, throwAngle);
            //Debug.Log("ThrowAngle: " + throwAngle);
            //Debug.Log("ThrowForce: " + throwForce);

            //maxDistance = (targetPos - _origin.position).magnitude;
            //Xf = Xo + Vo*(tf-to) + 1/2*a*(t-to)^2
            //initialVel = (targetPos - _origin.position).normalized * moveSpeed * rb.mass;
            //initialVel.y = yForce;
            //initialVel.y = (initialPos.y + (maxDistance * distanceTime_Relation - initialTime) + 0f) / targetPos.y;
            //moveDir = new Vector3(
            //    initialPos.x + 0f,
            //    (initialPos.y + (maxDistance * distanceTime_Relation - initialTime) + 0f) / targetPos.y,
            //    initialPos.z + 0f
            //);
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

    private float CalculateThrowAngle(Vector3 _initPoint, Vector3 _targetPoint, float _maxHeight)
    {
        Vector3 halfPoint = (_targetPoint + _initPoint) / 2f;
        Vector3 maxHeightPoint = new Vector3(halfPoint.x, halfPoint.y + _maxHeight, halfPoint.z);
        float throwAngle_Rad = Mathf.Atan((maxHeightPoint - halfPoint).magnitude / (_targetPoint - _initPoint).magnitude);
        return throwAngle_Rad * Mathf.Rad2Deg;
    }

    private float CalculateThrowForceFromAngle(Vector3 _initPoint, Vector3 _targetPoint, float _throwAngle)
    {
        Vector3 positionsDiff = new Vector3(
            _targetPoint.x - _initPoint.x,
            _targetPoint.y - _initPoint.y,
            _targetPoint.z - _initPoint.z
        );

        float angle_Rad = _throwAngle * Mathf.Deg2Rad;
        float sin2Theta = Mathf.Sin(2f * angle_Rad);
        float cosTheta = Mathf.Cos(angle_Rad);
        float inner = (positionsDiff.x * positionsDiff.x * Physics.gravity.y) 
            / ((positionsDiff.x * sin2Theta - 2) * (positionsDiff.y * cosTheta * cosTheta));
        if (inner < 0) return float.NaN;
        else return Mathf.Sqrt(inner);
    }


    protected override void Update_Call()
    {
        //float nextY;
        //if (timer < speed) { timer += Time.deltaTime; return; }

        //timer = 0;
        timer += Time.deltaTime;
        Vector3 nextPos = transform.position;
        if (goingUp)
        {
            //float distToHighestPoint = Vector3.Distance(transform.position, highestPos);
            //Debug.Log("DistToHighestPoint: " + distToHighestPoint);
            //nextPos = Vector3.Lerp(transform.position, highestPos, Time.deltaTime * speed);
            float lerpValue = Mathf.Sin(timer / speed * Mathf.PI * 0.5f);
            posLerper = Vector3.Lerp(highestPosHigh, highestPosLow, lerpValue);
            nextPos = Vector3.Lerp(initialPos, posLerper, lerpValue);
            if (timer >= speed) { goingUp = false; timer = 0f; }
            //nextY = Mathf.Lerp(initialPos.y, highestPos.y, distToHighestPoint / midDistance);
            //if (distToHighestPoint < THRESHOLD) { goingUp = false; timer = 0; }
        }
        if(!goingUp)
        {
            ///TODO: aplicar el posLerper amb el target, també. 
            ///
            float distToTarget = Vector3.Distance(transform.position, targetPos);
            //nextPos = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * speed);
            float lerpValue = 1f - Mathf.Cos(timer / speed * Mathf.PI * 0.5f);
            nextPos = Vector3.MoveTowards(transform.position, targetPos, lerpValue);
            //nextY = Mathf.Lerp(highestPos.y, targetPos.y, distToTarget / midDistance);
            if (distToTarget < THRESHOLD) Destroy(gameObject);
        }
        //Vector3 nextPos = new Vector3(
        //    transform.position.x + moveDir.x * moveSpeed * Time.deltaTime,
        //    nextY,
        //    transform.position.z + moveDir.y * moveSpeed * Time.deltaTime
        //);
        moveDir = (nextPos - transform.position).normalized;
        transform.position = nextPos;

        ////Do things
        ////float newY = initialY + Mathf.Sin((targetPos - transform.position).magnitude / maxDistance);
        //Vector3 nextPos = new Vector3(
        //    transform.position.x + moveDir.x * moveSpeed * Time.deltaTime,
        //    initialY + Mathf.Sin((targetPos - transform.position).magnitude / maxDistance), 
        //    transform.position.z + moveDir.y * moveSpeed * Time.deltaTime
        //);
        //moveDir = (nextPos - transform.position).normalized;

        //float timeDiff = Time.timeSinceLevelLoad - initialTime;
        //transform.position = new Vector3(
        //    initialPos.x + (initialVel.x * timeDiff),
        //    initialPos.y + initialVel.y * (maxDistance * distanceTime_Relation - initialTime)
        //        + (0.5f * Physics.gravity.y * Mathf.Pow(timeDiff, 2)),
        //    initialPos.z + (initialVel.z * timeDiff)
        //);
        //moveDir = (nextPos - transform.position).normalized;


        //// Position
        //if (_initForce != Vector2.zero)
        //{
        //    //d = D(0) + V(0)*t + 1/2*a*t^2
        //    Vector2 initVel = _initForce / _mass;
        //    Vector3 finalPos = new Vector2(
        //        _initPos.x + (initVel.x * currTimeDiff),
        //        _initPos.y + (initVel.y * currTimeDiff) + (0.5f * Physics.gravity.y * Mathf.Pow(currTimeDiff, 2))
        //    );
        //    trajectoryPoints[i].transform.position = finalPos;

        //}

        //base.Update_Call();
    }


    protected override void OnTriggerEnter_Call(Collider other)
    {
        //Do things
        
        base.OnTriggerEnter_Call(other);
    }
}
