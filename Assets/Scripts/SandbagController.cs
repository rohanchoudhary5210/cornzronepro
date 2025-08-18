using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class SandbagController : MonoBehaviour
{
    // All variables for throwing logic remain the same
    #region Variables
    public float throwForceMultiplier = 5f;
    public float verticalSensitivity = 0.01f;
    public float horizontalSensitivity = 0.2f;
    public float MinSwipeDist = 30f;
    public float MaxBallSpeed = 50f;

    private float startTime, endTime, swipeDistance, swipeTime;
    private Vector2 startPos, endPos;
    private bool _isThrown = false;
    private bool _isHolding = false;
    private Rigidbody _rb;

    public bool HasLandedOnBoard { get; set; } = false;
    public bool HasHitGround { get; set; } = false;
    public bool HasScoredInHole { get; set; } = false;

    [SerializeField] private float stabilityThreshold = 0.001f;
    [SerializeField] private float stableDuration = 0.5f;
    #endregion

    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (_isThrown) return;
        #if UNITY_EDITOR || UNITY_STANDALONE
            HandleMouseInput();
        #elif UNITY_ANDROID || UNITY_IOS
            HandleTouchInput();
        #endif
    }

    // All input and throw calculation methods remain the same
    #region Throwing Logic
    void HandleMouseInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            startTime = Time.time;
            startPos = Input.mousePosition;
            _isHolding = true;
        }
        if (Input.GetMouseButton(0) && _isHolding)
        {
            // You might want to implement PickupBall logic here if needed
        }
        if (Input.GetMouseButtonUp(0) && _isHolding)
        {
            endTime = Time.time;
            endPos = Input.mousePosition;
            _isHolding = false;
            HandleRelease();
        }
    }
    void HandleTouchInput()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    startTime = Time.time;
                    startPos = touch.position;
                    _isHolding = true;
                    break;
                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    if (_isHolding)
                    {
                        endTime = Time.time;
                        endPos = touch.position;
                        _isHolding = false;
                        HandleRelease();
                    }
                    break;
            }
        }
    }
    void HandleRelease()
    {
        swipeDistance = (endPos - startPos).magnitude;
        swipeTime = endTime - startTime;
        if (swipeTime > 0 && swipeDistance >= MinSwipeDist)
        {
            StartCoroutine(DelayedThrow());
            StartCoroutine(CheckIfStable());
        }
    }
    IEnumerator DelayedThrow()
    {
        yield return new WaitForEndOfFrame();
        float ballSpeed = CalculateSpeed();
        Vector3 throwDirection = CalculateDirection();
        ballSpeed = Mathf.Clamp(ballSpeed, 5f, MaxBallSpeed);
        Vector3 force = throwDirection * ballSpeed;
        _rb.AddForce(force, ForceMode.Impulse);
        _rb.useGravity = true;
        _isThrown = true;
    }
    Vector3 CalculateDirection()
    {
        Vector3 swipeDirectionScreen = (endPos - startPos);
        Quaternion horizontalRotation = Quaternion.AngleAxis(swipeDirectionScreen.x * horizontalSensitivity, Vector3.up);
        Vector3 forwardDirection = Camera.main.transform.forward;
        forwardDirection.y = 0;
        Vector3 finalDirection = horizontalRotation * forwardDirection.normalized;
        float upwardAngle = swipeDirectionScreen.y * verticalSensitivity;
        upwardAngle = Mathf.Clamp(upwardAngle, 10f, 32f);
        return Quaternion.AngleAxis(-upwardAngle, Camera.main.transform.right) * finalDirection;
    }
    float CalculateSpeed()
    {
        if (swipeTime > 0)
        {
            float swipeVelocity = swipeDistance / swipeTime;
            return swipeVelocity * throwForceMultiplier * 3;
        }
        else
        {
            return 0;
        }
    }
    #endregion

    /// <summary>
    /// This coroutine now ONLY tells the GameManager when the bag is stable.
    /// ALL SCORING LOGIC HAS BEEN REMOVED.
    /// </summary>
    private IEnumerator CheckIfStable()
    {
        yield return new WaitForSeconds(1.5f);
        float timer = 0f;
        Vector3 lastPos = transform.position;

        while (timer < stableDuration)
        {
            yield return new WaitForSeconds(0.1f);
            float distance = Vector3.Distance(transform.position, lastPos);
            if (distance < stabilityThreshold) { timer += 0.1f; }
            else { timer = 0f; }
            lastPos = transform.position;
        }

        // The bag is stable. Tell the GameManager to evaluate the board.
        GameManager.Instance.EvaluateBoardState();
        
        // This component has done its job.
        this.enabled = false;
    }
}