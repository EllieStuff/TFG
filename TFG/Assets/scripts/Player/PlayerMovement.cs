using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    const float INPUT_THRESHOLD = 0;
    const float MIN_FALL_SPEED = 10;
    const float MIN_SPEED_WALK = 0.8f;
    const float SPEED_REDUCTION = 1.4f;
    const float DIAGONAL_SPEED_REDUCTION = 0.8f;
    const float SCREEN_WIDTH = 1000;
    const float SCREEN_HEIGHT = 500;

    [SerializeField] float baseMoveForce = 50;
    [SerializeField] float baseRotSpeed = 300;
    [SerializeField] Vector3 baseMaxSpeed = new Vector3(50, 0, 50);
    [SerializeField] float fallSpeed;
    [SerializeField] float damageAnimationTime;
    [SerializeField] Animator playerAnimator;

    float actualMoveForce;
    float actualRotSpeed;
    internal float speedMultiplier = 2.0f;
    Vector3 actualMaxSpeed;
    internal bool canMove = true;
    internal bool canRotate = true;
    internal Vector3 targetMousePos;
    LifeSystem lifeStatus;

    const float minFallSpeed = 10;


    Rigidbody rb;
    [HideInInspector] public Vector3 moveDir = Vector3.zero;
    [HideInInspector] public Vector3 lookDir = Vector3.zero;
    bool moving = false;
    Camera mainCam;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        lifeStatus = GetComponent<LifeSystem>();
        mainCam = GameObject.Find("Main Camera").GetComponent<Camera>();

        ResetSpeed();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector2 mouseLookVec = GetMouseLookVector();
        float horizontalInput = mouseLookVec.x;
        //if (Mathf.Abs(horizontalInput) < INPUT_THRESHOLD) horizontalInput = 0;
        float verticalInput = mouseLookVec.y;
        //if (Mathf.Abs(verticalInput) < INPUT_THRESHOLD) verticalInput = 0;
        lookDir = new Vector3(horizontalInput, 0, verticalInput);
        moveDir = MoveToTargetVector(targetMousePos);


        if (canMove && (Mathf.Abs(verticalInput) > INPUT_THRESHOLD || Mathf.Abs(horizontalInput) > INPUT_THRESHOLD) && moveDir != Vector3.zero && lifeStatus.currLife > 0)
        {
            moving = true;

            if(rb.velocity.magnitude > MIN_SPEED_WALK)
                playerAnimator.SetFloat("state", 1);
            else
                playerAnimator.SetFloat("state", 0);

            if (Mathf.Abs(verticalInput) > INPUT_THRESHOLD && Mathf.Abs(horizontalInput) > INPUT_THRESHOLD)
                moveDir *= DIAGONAL_SPEED_REDUCTION;
            rb.AddForce(moveDir * actualMoveForce * speedMultiplier, ForceMode.Force);
            Vector3 finalVelocity = ClampVector(rb.velocity, -actualMaxSpeed * speedMultiplier, actualMaxSpeed * speedMultiplier) + new Vector3(0, rb.velocity.y, 0);
            rb.velocity = finalVelocity;
        }
        else if (moving)
        {
            moving = false;
            playerAnimator.SetFloat("state", 0);
            Vector3 reducedVel = rb.velocity;

            if (Mathf.Abs(reducedVel.x) > 0)
                reducedVel = new Vector3(rb.velocity.x / SPEED_REDUCTION, rb.velocity.y, rb.velocity.z);
            if (Mathf.Abs(reducedVel.z) > 0)
                reducedVel = new Vector3(rb.velocity.x, rb.velocity.y, rb.velocity.z / SPEED_REDUCTION);

            rb.velocity = reducedVel;
        }

        rb.velocity = FallSystem(rb.velocity);

        if (canRotate && (moveDir == Vector3.zero || Input.GetKey(KeyCode.Mouse1)) && lifeStatus.currLife > 0)
        {
            Quaternion targetRot = Quaternion.LookRotation(lookDir, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, actualRotSpeed * speedMultiplier * Time.deltaTime);
        }
    }

    Vector2 GetMouseLookVector()
    {
        Vector3 MousePosWithPlayer = Input.mousePosition - mainCam.WorldToScreenPoint(transform.position);
        MousePosWithPlayer.z = 5.23f;

        return MousePosWithPlayer;
    }

    Vector3 FallSystem(Vector3 actualVelocity)
    {
        if(actualVelocity.y < MIN_FALL_SPEED && rb.useGravity)
            actualVelocity.y -= Time.deltaTime * fallSpeed;

        return actualVelocity;
    }

    Vector3 MoveToTargetVector(Vector3 mousePositionInWorld)
    {
        if (Vector3.Distance(mousePositionInWorld, transform.position) <= 1)
            return Vector3.zero;

        Vector3 vectorToMove = (mousePositionInWorld - transform.position).normalized;
        vectorToMove.y = 0;

        return vectorToMove;
    }

    Vector3 NormalizeDirection(Vector3 moveDir)
    {
        Vector2 MoveDirWithScreen = new Vector2(moveDir.x / SCREEN_WIDTH, moveDir.z / SCREEN_HEIGHT);

        if (moveDir.x > 0 && MoveDirWithScreen.x > 0.1f)
            moveDir = new Vector3(MIN_SPEED_WALK + (moveDir.x / SCREEN_WIDTH), moveDir.y, moveDir.z);
        else if (moveDir.x < 0 && MoveDirWithScreen.x < -0.1f)
            moveDir = new Vector3(-MIN_SPEED_WALK + (moveDir.x / SCREEN_WIDTH), moveDir.y, moveDir.z);
        else
            moveDir.x = 0;

        if (moveDir.z > 0 && MoveDirWithScreen.y > 0.1f)
            moveDir = new Vector3(moveDir.x, moveDir.y, MIN_SPEED_WALK + (moveDir.z / SCREEN_HEIGHT));
        else if (moveDir.z < 0 && MoveDirWithScreen.y < -0.1f)
            moveDir = new Vector3(moveDir.x, moveDir.y, -MIN_SPEED_WALK + (moveDir.z / SCREEN_HEIGHT));
        else
            moveDir.z = 0;

        return moveDir;
    }

    Vector3 ClampVector(Vector3 _originalVec, Vector3 _minVec, Vector3 _maxVec)
    {
        return new Vector3(
            Mathf.Clamp(_originalVec.x, _minVec.x, _maxVec.x),
            Mathf.Clamp(_originalVec.y, _minVec.y, _maxVec.y),
            Mathf.Clamp(_originalVec.z, _minVec.z, _maxVec.z)
        );
    }

    public void ChangeSpeed(float _moveForce, float _rotSpeed, Vector3 _maxSpeed)
    {
        actualMoveForce = _moveForce;
        actualRotSpeed = _rotSpeed;
        actualMaxSpeed = _maxSpeed;
    }
    public void ResetSpeed()
    {
        actualMoveForce = baseMoveForce;
        actualRotSpeed = baseRotSpeed;
        actualMaxSpeed = baseMaxSpeed;
    }
    public void DamageStartCorroutine()
    {
        StartCoroutine(DamageCorroutine());
    }
    IEnumerator DamageCorroutine()
    {
        canMove = false;
        canRotate = false;

        //Play animation here and sound

        //_____________________________

        yield return new WaitForSeconds(damageAnimationTime);

        canMove = true;
        canRotate = true;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag.Equals("floor"))
            rb.useGravity = false;
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag.Equals("floor"))
            rb.useGravity = true;
    }
}
