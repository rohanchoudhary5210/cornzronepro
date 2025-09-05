using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class SandbagController : MonoBehaviour
{
    public static SandbagController Instance { get; private set; }

    [Header("Throw Controls")]
    public float throwForceMultiplier = 5f;
    public float verticalSensitivity = 0.01f;
    public float horizontalSensitivity = 0.2f;
    public float MinSwipeDist = 30f;
    public float MaxBallSpeed = 50f;

    private float startTime, endTime, swipeDistance, swipeTime;
    private Vector2 startPos, endPos;

    private bool _isThrown = false;
    private bool _isHolding = false;
    private Vector3 _newPosition;
    private Vector3 _resetPosition;
    private Quaternion _resetRotation;

    private Rigidbody _rb;

    [Header("Scoring State")]
    public bool HasLandedOnBoard { get; set; } = false;
    public bool HasHitGround { get; set; } = false;
    public bool HasScoredInHole { get; set; } = false;

    [Header("Score Settings")]
    [SerializeField] private float stabilityThreshold = 0.001f;
    [SerializeField] private float stableDuration = 0.5f;

    //wind effect
    private Vector3 windDirection = Vector3.zero; // No wind by default
    private bool windEnabled; // takes value from GameManager
    private bool windDirectionRightToLeft; // takes value from GameManager
    private Vector3 windRightToLeft;
    private Vector3 windLeftToRight;
    void Awake()
    {
        if (Instance == null) Instance = this;
        _rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        _resetPosition = transform.position;
        _resetRotation = transform.rotation;
        ResetSandbag();
        windEnabled = GameManager.Instance.windEnabled;
        windDirectionRightToLeft = GameManager.Instance.windDirectionRightToLeft;
        windLeftToRight = new Vector3(GameManager.Instance.xWindVal, 0f, 0f);
        windRightToLeft = new Vector3(-GameManager.Instance.xWindVal, 0f, 0f);
    }

    void Update()
    {
        if (_isThrown) return;

#if UNITY_EDITOR || UNITY_STANDALONE
        HandleMouseInput();
#else
        HandleTouchInput();
#endif
    }

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
            endPos = Input.mousePosition;
            endTime = Time.time;
                
            if (GameManager.Instance.powerupActive1)
            {
                PredictTrajectory();
            }
        }

        if (Input.GetMouseButtonUp(0) && _isHolding)
        {
            _isHolding = false;
            HandleRelease();
        }
    }

    void HandleTouchInput()
    {
        if (Input.touchCount > 0)
        {
            var touch = Input.GetTouch(0);
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    startTime = Time.time;
                    startPos = touch.position;
                    _isHolding = true;
                    break;
                case TouchPhase.Moved:
                    
                        // PickupBall(Input.mousePosition);
                        endPos = Input.mousePosition;
                        endTime = Time.time;
                       if (GameManager.Instance.powerupActive1)
                    {
                        PredictTrajectory();
                    }
                    break;
                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    if (_isHolding)
                    {
                        _isHolding = false;
                        HandleRelease();
                    }
                    break;
            }
        }
    }

    void PickupBall(Vector2 inputPos)
    {
        Vector3 screenPos = new Vector3(inputPos.x, inputPos.y, Camera.main.nearClipPlane + 2f);
        _newPosition = Camera.main.ScreenToWorldPoint(screenPos);
        transform.position = Vector3.Lerp(transform.position, _newPosition, Time.deltaTime * 15f);
    }

    void PredictTrajectory()
    {
        swipeDistance = Vector2.Distance(startPos, endPos);
        swipeTime = endTime - startTime;

        if (swipeTime > 0 && swipeDistance >= MinSwipeDist)
        {
            float speed = Mathf.Clamp(CalculateSpeed(), 5f, MaxBallSpeed);
            Vector3 dir = CalculateDirection();
            ProjectileSimulator.Instance.PredictTrajectory(transform.position, speed, dir);
        }
    }

    void HandleRelease()
    {
        swipeDistance = Vector2.Distance(startPos, endPos);
        swipeTime = endTime - startTime;

        if (swipeTime > 0 && swipeDistance >= MinSwipeDist)
        {
            StartCoroutine(DelayedThrow());
        }
    }

    IEnumerator DelayedThrow()
    {
        yield return new WaitForEndOfFrame();

        float speed = Mathf.Clamp(CalculateSpeed(), 5f, MaxBallSpeed);
        Vector3 dir = CalculateDirection();
        Vector3 force = dir * speed;

        _rb.AddForce(force, ForceMode.Impulse);
        _rb.useGravity = true;
        _isThrown = true;

        ProjectileSimulator.Instance.ClearPath();

        StartCoroutine(CheckIfStable());
    }

    Vector3 CalculateDirection()
    {
        Vector2 swipe = endPos - startPos;
        Quaternion yaw = Quaternion.AngleAxis(swipe.x * horizontalSensitivity, Vector3.up);

        Vector3 forward = Camera.main.transform.forward;
        forward.y = 0;

        if (windEnabled)
        {
            windDirection = windDirectionRightToLeft ? windRightToLeft : windLeftToRight;
        }
        Vector3 dir = yaw * forward.normalized + windDirection;

        float upAngle = Mathf.Clamp(swipe.y * verticalSensitivity, 10f, 32f);
        return Quaternion.AngleAxis(-upAngle, Camera.main.transform.right) * dir;
    }

    float CalculateSpeed()
    {
        return swipeTime > 0 ? (Vector2.Distance(startPos, endPos) / swipeTime) * throwForceMultiplier * 3f : 0f;
    }

    IEnumerator CheckIfStable()
    {
        yield return new WaitForSeconds(1.5f);
        float timer = 0f;
        Vector3 lastPos = transform.position;

        while (timer < stableDuration)
        {
            yield return new WaitForSeconds(0.1f);
            if (Vector3.Distance(transform.position, lastPos) < stabilityThreshold)
                timer += 0.1f;
            else
                timer = 0f;

            lastPos = transform.position;
        }

        if (HasScoredInHole)
        {
            CoinManager.Instance.AddCoins(2);
            GameManager.Instance.AddTime(2f);
            GameManager.Instance.currentgamecoins+=2;
        }
        else if (HasLandedOnBoard && !HasHitGround)
        {
            GameManager.Instance.AddTime(0);
        }
        else if (HasHitGround)
        {
            GameManager.Instance.AddTime(-1f);
        }

        GameManager.Instance.RequestNewSandbag();
        enabled = false;
    }

    public void ResetSandbag()
    {
        _isThrown = _isHolding = false;
        _rb.linearVelocity = Vector3.zero;
        _rb.angularVelocity = Vector3.zero;
        _rb.useGravity = false;
        transform.SetPositionAndRotation(_resetPosition, _resetRotation);
    }
}
