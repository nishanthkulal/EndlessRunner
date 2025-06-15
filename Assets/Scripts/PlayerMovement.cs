using System;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private float jumpForce;
    [SerializeField] private float groundDistance;
    [SerializeField] private LayerMask groundLayer;
    private Rigidbody2D playerRb;
    private bool _startPlayerRunning = false;
    private bool _isGrounded;
    private Animator _playeranimator;

    private void Start()
    {
        playerRb = GetComponent<Rigidbody2D>();
        _playeranimator = GetComponent<Animator>();
    }
    private void Update()
    {
        AnimatorController();
        PlayerMov();
        PlayerJump();

    }

    private void PlayerJump()
    {
        _isGrounded = Physics2D.Raycast(transform.position, Vector2.down, groundDistance, groundLayer);

        if (Input.GetKeyDown(KeyCode.Space) && _isGrounded)
        {
            playerRb.linearVelocity = new Vector2(playerRb.linearVelocity.x, jumpForce);
        }
    }
    void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, new Vector2(transform.position.x, transform.position.y - groundDistance));
    }

    private void PlayerMov()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            _startPlayerRunning = true;
        }

        if (_startPlayerRunning)
        {
            playerRb.linearVelocity = new Vector2(moveSpeed, playerRb.linearVelocity.y);
        }
    }

    private void AnimatorController()
    {
        _playeranimator.SetBool("IsGrounded", _isGrounded);
        _playeranimator.SetFloat("xVelocity", playerRb.linearVelocity.x);
        _playeranimator.SetFloat("yVelocity", playerRb.linearVelocity.y);

    }
}
