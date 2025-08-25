using UnityEngine;
using System.Collections;

/// <summary>
/// This script uses your original cornrag.cs logic for input and feel.
/// It has been updated to fix bugs and centralize scoring logic for accuracy.
/// </summary>
[RequireComponent(typeof(Rigidbody))]

public class SandbagController : MonoBehaviour
{
    public static SandbagController Instance { get; private set; }
    // --- Your Original Variables ---
    [Header("Throw Controls (Your Logic)")]
    public float throwForceMultiplier = 5f;
    public float verticalSensitivity = 0.01f;
    public float horizontalSensitivity = 0.2f;
    public float MinSwipeDist = 30f;
    public float MaxBallSpeed = 50f;

    // Swipe detection
    private float startTime, endTime, swipeDistance, swipeTime;
    private Vector2 startPos, endPos;

    // Ball state
    private bool _isThrown = false;
    private bool _isHolding = false;
    private Vector3 _newPosition;
    private Vector3 _resetPosition;
    private Quaternion _resetRotation;

    // --- Components & State for New System ---
    private Rigidbody _rb;
    public bool HasLandedOnBoard { get; set; } = false;
    public bool HasHitGround { get; set; } = false;
    public bool HasScoredInHole { get; set; } = false;

    [Header("Stability Check")]
    [SerializeField] private float stabilityThreshold = 0.001f;
    [SerializeField] private float stableDuration = 0.5f;

   void Awake()
{
    _rb = GetComponent<Rigidbody>();
    // The singleton Instance should not be set here.
}

void Start()
{
    // These values are only needed for the ResetSandbag logic, which we are no longer using at Start.
    _resetPosition = transform.position;
    _resetRotation = transform.rotation;

    // REMOVE THIS LINE: This is the cause of your problem.
    ResetSandbag(); 
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
   

    // --- YOUR ORIGINAL INPUT LOGIC ---

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
            PickupBall(Input.mousePosition);
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
                case TouchPhase.Moved:
                case TouchPhase.Stationary:
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

    void PickupBall(Vector2 inputPos)
    {
        if (_isThrown) return;
        Vector3 screenPos = new Vector3(inputPos.x, inputPos.y, Camera.main.nearClipPlane + 2f);
        _newPosition = Camera.main.ScreenToWorldPoint(screenPos);
        transform.position = Vector3.Lerp(transform.position, _newPosition, 15f * Time.deltaTime);
    }
    
    void HandleRelease()
    {
        swipeDistance = (endPos - startPos).magnitude;
        swipeTime = endTime - startTime;

        if (swipeTime > 0 && swipeDistance >= MinSwipeDist)
        {
            StartCoroutine(DelayedThrow());
            // The stability check is now correctly called only after a valid throw.
            StartCoroutine(CheckIfStable());
        }
    }

    IEnumerator DelayedThrow()
    {
        yield return new WaitForEndOfFrame();

        float ballSpeed = CalculateSpeed();
        Vector3 throwDirection = CalculateDirection();
        //Debug.Log($"Throwing with speed: {ballSpeed}, direction: {throwDirection.normalized}");

        ballSpeed = Mathf.Clamp(ballSpeed, 5f, MaxBallSpeed);
        Vector3 force = throwDirection * ballSpeed ;
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
            return swipeVelocity * throwForceMultiplier*3;
        }
        else
        {
            return 0;
        }
    }

    public void ResetSandbag()
    {
        startTime = endTime = swipeTime = swipeDistance = 0f;
        _isHolding = false;
        _isThrown = false;
        
        HasLandedOnBoard = false;
        HasHitGround = false;
        HasScoredInHole = false;

        _rb.linearVelocity = Vector3.zero;
        _rb.angularVelocity = Vector3.zero;
        _rb.useGravity = false;
        transform.SetPositionAndRotation(_resetPosition, _resetRotation);
    }

    /// <summary>
    /// This coroutine now calculates the final score after the bag has stopped moving.
    /// </summary>
    private IEnumerator CheckIfStable()
    {
        Transform target = this.transform;

        yield return new WaitForSeconds(1.5f);

        float timer = 0f;
        Vector3 lastPos = target.position;

        while (timer < stableDuration)
        {
            yield return new WaitForSeconds(0.1f);
            float distance = Vector3.Distance(target.position, lastPos);

            if (distance < stabilityThreshold)
            {
                timer += 0.1f;
            }
            else
            {
                timer = 0f;
            }
            lastPos = target.position;
        }

        // --- FINAL SCORING LOGIC ---
        //Debug.Log("Sandbag is stable. Finalizing score...");

        if (HasScoredInHole)
        {
            // Bag is in the hole. This is the highest score.
            if (HasLandedOnBoard)
            {
                // It slid in. Worth 3 points.
                //Debug.Log("Final Score: Slide in hole (3 points)");
                GameManager.Instance.AddScore(3);
                GameManager.Instance.AddCoins(30);
            }
            else
            {
                // It went straight in (Airmail). Worth 3 points.
                //Debug.Log("Final Score: Airmail (3 points)");
                GameManager.Instance.AddScore(3);
                GameManager.Instance.AddCoins(30);
            }
            GameManager.Instance.AddTime(10f);
        }
        else if (HasLandedOnBoard && !HasHitGround)
        {
            // Bag is on the board and NEVER touched the ground. Worth 1 point.
            //Debug.Log("Final Score: On board (1 point)");
            GameManager.Instance.AddScore(1);
            //GameManager.Instance.AddCoins(10);
        }
        else
        {
            // Bag hit the ground or didn't land correctly. Worth 0 points.
            //Debug.Log("Final Score: No points");
        }

        GameManager.Instance.RequestNewSandbag();
        this.enabled = false;
         //gameObject.SetActive(false); 
    }
}
