using System;
using UnityEditor.Callbacks;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Camera")]
    [SerializeField] private Camera cam;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float jumpForce;
    [SerializeField] private float doubleJumpForce;
    [SerializeField] private float groundDistance;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform wallCheck;
    [SerializeField] private Vector2 wallCheckSize;
    [SerializeField] private float slidingSpeed;
    [SerializeField] private float slideTime;
    [SerializeField] private float slideCoolDownTime;
    [SerializeField] private float checkDistanceToCelling;
    [SerializeField] private Vector2 offset1;
    [SerializeField] private Vector2 offset2;
    [SerializeField] private float maxSpeed;
    [SerializeField] private float speedMultiplier;
    [Space]
    [SerializeField] private float mileStoneIncreaser;

    [Space]
    [SerializeField] private Vector2 knockBackDirection;


    [HideInInspector] public bool edgeDetected;
    private float slideCoolDownCounter;
    private float slideTimeCounter;
    private bool _isSliding;
    private Rigidbody2D playerRb;
    private bool _startPlayerRunning = false;
    private bool _isGrounded;
    private Animator _playeranimator;
    private bool _canDoubleJump;
    private bool _isWallDetected;
    private bool _isCellingDetected;
    private Vector2 climbBegunPosition;
    private Vector2 climbOverPosition;
    private bool _canGrabEdge = true;
    private bool _CanClimb;
    private float speedMileStone;
    private float defaultSpeed;
    private float defaultMilstoneIncreaser;
    private bool _isKnockedBack;



    private void Start()
    {
        playerRb = GetComponent<Rigidbody2D>();
        _playeranimator = GetComponent<Animator>();
        speedMileStone = mileStoneIncreaser;
        defaultSpeed = moveSpeed;
        defaultMilstoneIncreaser = mileStoneIncreaser;
    }
    private void Update()
    {
        slideTimeCounter -= Time.deltaTime;
        slideCoolDownCounter -= Time.deltaTime;
        AnimatorController();
        WallDetection();

        if (_isKnockedBack)
            return;

        if (Input.GetKeyDown(KeyCode.K))
            KnockBack();

        PlayerMov();
        PlayerJump();
        PlayerSliding();
        CheckForSlide();
        checkForEdge();
        SpeedController();
        if (_isGrounded)
        {
            _canDoubleJump = true;
        }
    }

    private void KnockBack()
    {
        _isKnockedBack = true;
        playerRb.linearVelocity = knockBackDirection;
    }
    private void ChancelKnockBack()
    {
        _isKnockedBack = false;
    }
    private void speedReset()
    {
        moveSpeed = defaultSpeed;

        mileStoneIncreaser = defaultMilstoneIncreaser;
    }

    private void SpeedController()
    {
        if (moveSpeed == maxSpeed)
            return;
        if (transform.position.x > speedMileStone)
        {
            //  SetRandomBGColor();
            speedMileStone += mileStoneIncreaser;
            moveSpeed *= speedMultiplier;
            mileStoneIncreaser *= speedMultiplier;
            if (moveSpeed > maxSpeed)
            {
                moveSpeed = maxSpeed;
            }
        }

    }

    private void SetRandomBGColor()
    {
        Color randomColor = new Color(
            UnityEngine.Random.Range(0f, 1f), // red
            UnityEngine.Random.Range(0f, 1f), // green
            UnityEngine.Random.Range(0f, 1f) // blue
        );
        cam.backgroundColor = randomColor;
    }
    private void checkForEdge()
    {
        // Debug.Log(edgeDetected);
        if (edgeDetected && _canGrabEdge)
        {

            _canGrabEdge = false;
            playerRb.gravityScale = 0f;
            Vector2 ledgePosition = GetComponentInChildren<EdgeDetection>().transform.position;
            climbBegunPosition = ledgePosition + offset1;
            climbOverPosition = ledgePosition + offset2;
            _CanClimb = true;
        }
        if (_CanClimb)
        {
            transform.position = climbBegunPosition;
        }
    }
    private void EdgeClimbingOver()
    {
        _CanClimb = false;
        playerRb.gravityScale = 5f;
        // _canDoubleJump = false;
        transform.position = climbOverPosition;
        Invoke(nameof(AllowEdgeGrab), 1f);

    }

    private void AllowEdgeGrab()
    {
        _canGrabEdge = true;
    }

    private void PlayerJump()
    {
        WallDetection();

        if (Input.GetKeyDown(KeyCode.Space))
        {

            JumpButton();
        }
    }

    private void WallDetection()
    {
        _isGrounded = Physics2D.Raycast(transform.position, Vector2.down, groundDistance, groundLayer);
        _isCellingDetected = Physics2D.Raycast(transform.position, Vector2.up, checkDistanceToCelling, groundLayer);
        _isWallDetected = Physics2D.BoxCast(wallCheck.position, wallCheckSize, 0, Vector2.zero, 0, groundLayer);
    }

    private void JumpButton()
    {
        if (_isSliding)
            return;
        if (_isGrounded)
        {
            playerRb.linearVelocity = new Vector2(playerRb.linearVelocity.x, jumpForce);
        }
        else if (_canDoubleJump)
        {
            _canDoubleJump = false;
            playerRb.linearVelocity = new Vector2(playerRb.linearVelocity.x, doubleJumpForce);
        }

    }



    private void PlayerMov()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            _startPlayerRunning = true;
        }

        if (_startPlayerRunning)
        {
            if (_isWallDetected)
            {
                speedReset();
                return;
            }
            Movement();
        }
    }

    private void Movement()
    {

        if (_isSliding)
        {
            playerRb.linearVelocity = new Vector2(slidingSpeed, playerRb.linearVelocity.y);
        }
        else
        {
            playerRb.linearVelocity = new Vector2(moveSpeed, playerRb.linearVelocity.y);
        }
    }
    private void CheckForSlide()
    {
        //  Debug.Log(slideTimeCounter);
        if (slideTimeCounter < 0 && !_isCellingDetected)
        {
            _isSliding = false;
        }
    }

    private void PlayerSliding()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            SlideButton();
        }
    }

    private void SlideButton()
    {
        if (playerRb.linearVelocity.x != 0 && slideCoolDownCounter < 0 && _isGrounded)
        {
            _isSliding = true;
            slideTimeCounter = slideTime;
            slideCoolDownCounter = slideCoolDownTime;
        }

    }

    private void AnimatorController()
    {
        _playeranimator.SetFloat("xVelocity", playerRb.linearVelocity.x);
        _playeranimator.SetFloat("yVelocity", playerRb.linearVelocity.y);
        _playeranimator.SetBool("CanDoubbleJump", _canDoubleJump);
        _playeranimator.SetBool("IsGrounded", _isGrounded);
        _playeranimator.SetBool("IsSliding", _isSliding);
        _playeranimator.SetBool("CanClimb", _CanClimb);
        _playeranimator.SetBool("IsKnocked", _isKnockedBack);

        if (playerRb.linearVelocity.y < -20)
        {
            _playeranimator.SetBool("CanRoll", true);
        }


    }
    private void RollAnimationFinished()
    {
        _playeranimator.SetBool("CanRoll", false);
    }
    void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, new Vector2(transform.position.x, transform.position.y - groundDistance));
        Gizmos.DrawLine(transform.position, new Vector2(transform.position.x, transform.position.y + checkDistanceToCelling));
        Gizmos.DrawCube(wallCheck.position, wallCheckSize);
    }
}
