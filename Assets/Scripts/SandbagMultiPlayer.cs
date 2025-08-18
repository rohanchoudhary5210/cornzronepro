using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SandbagMultiPlayer : MonoBehaviour
{

    // --- All your throwing logic variables remain the same ---
    [Header("Throw Controls")]
    [SerializeField] private float throwPower = 2.0f;
    public float verticalSensitivity = 0.01f;
    public float horizontalSensitivity = 0.05f;
    public float MinSwipeDist = 30f;
    public float MaxBallSpeed = 50f;

    // --- State & Components ---
    private Rigidbody _rb;
    private bool _isThrown = false;
    private bool _isHolding = false;
    private float startTime, endTime, swipeDistance, swipeTime;
    private Vector2 startPos, endPos;

    // Flags set by detector scripts
    public bool HasLandedOnBoard { get; set; } = false;
    public bool HasHitGround { get; set; } = false;
    public bool HasScoredInHole { get; set; } = false;

    [Header("Stability Check")]
    [SerializeField] private float stabilityThreshold = 0.001f;
    [SerializeField] private float stableDuration = 0.5f;

    void Awake() { _rb = GetComponent<Rigidbody>(); }
    void Start() { /* This is intentionally left empty, the bag is just a projectile */ }

    void Update()
    {
        if (_isThrown) return;
        // Using the flick-to-jump logic
#if UNITY_EDITOR || UNITY_STANDALONE
        HandleMouseInput();
#elif UNITY_ANDROID || UNITY_IOS
            HandleTouchInput();
#endif
    }

    // --- All your input and throwing methods remain the same ---
    void HandleMouseInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            startTime = Time.time;
            startPos = Input.mousePosition;
            _isHolding = true;
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
        Time.timeScale = 0.6f;
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
        upwardAngle = Mathf.Clamp(upwardAngle, 10f, 25f);
        return Quaternion.AngleAxis(-upwardAngle, Camera.main.transform.right) * finalDirection;
    }
    public float t = 0;
    float CalculateSpeed()
    {
        if (swipeTime > 0)
        {
            float swipeVelocity = swipeDistance / swipeTime;
            return swipeVelocity * throwPower;
        }
        else
        {
            return 0;
        }
    }

    /// <summary>
    /// *** MULTIPLAYER CHANGE ***
    /// This now calculates points and reports them to the MultiplayerGameManager.
    /// </summary>
    // private IEnumerator CheckIfStable()
    // {
    //     yield return new WaitForSeconds(0.5f);
    //     float timer = 0f;
    //     Vector3 lastPos = transform.position;
    //     while (timer < stableDuration)
    //     {
    //         yield return new WaitForSeconds(0.1f);
    //         float distance = Vector3.Distance(transform.position, lastPos);
    //         if (distance < stabilityThreshold)
    //         {
    //             timer += 0.1f;
    //         }
    //         else
    //         {
    //             timer = 0f;
    //         }
    //         lastPos = transform.position;
    //     }


    //     int pointsForThisThrow = 0;
    //     if (HasScoredInHole)
    //     {
    //         pointsForThisThrow = 3;

    //     }
    //     else if(HasLandedOnBoard && !HasHitGround)
    //     {
    //         pointsForThisThrow = 1;
    //     }
    //     GameManagerMultiplayer.Instance.RecordThrow(pointsForThisThrow, this.gameObject);

    //     // Disable this script. The manager will handle what happens next.
    //     this.enabled = false;
    // }
    // SandbagMultiPlayer.cs

private IEnumerator CheckIfStable()
{
    // --- THIS LOGIC IS REPLACED ---
    // The sandbag no longer calculates its own points.

    yield return new WaitForSeconds(0.5f);
    float timer = 0f;
    Vector3 lastPos = transform.position;
    while (timer < stableDuration)
    {
        yield return new WaitForSeconds(0.1f);
        float distance = Vector3.Distance(transform.position, lastPos);
        if (distance < stabilityThreshold)
        {
            timer += 0.1f;
        }
        else
        {
            timer = 0f;
        }
        lastPos = transform.position;
    }

    // The bag is stable. Tell the GameManager to evaluate the entire board.
    GameManagerMultiplayer.Instance.EvaluateBoardState();

    // Disable this script. The manager will handle what happens next.
    this.enabled = false;
}
}
