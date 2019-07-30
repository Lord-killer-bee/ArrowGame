using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    #region Movement parameters

    [Header("Movement parameters")]
    public float MoveSpeed = 3f;
    public float AirMoveSpeed = 3f;
    public float _jumpSpeed = 7;

    #endregion

    #region Private variables

    private PlayerState playerState;
    private PlayerLocation playerLocation;
    int jumpCounter;
    Rigidbody rb;

    #endregion

    private void Start()
    {
        playerState = PlayerState.Idle;
        playerLocation = PlayerLocation.InAir;
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        switch (playerState)
        {
            //Give a default direction or has the direction of previous ended state
            //When no input is detected set to Idle
            case PlayerState.Idle:
                CheckForRun();
                CheckForJump();
                break;

            case PlayerState.Run:
                ExecuteRun();

                CheckForJump();
                break;
            case PlayerState.Jump:
                ExecuteJump();
                break;
        }

    }

    void ExecuteRun()
    {
        float moveDirection = ReturnMoveDirection();

        Vector3 destination = transform.position + new Vector3(moveDirection, 0, 0) * MoveSpeed * Time.deltaTime;

        transform.position = Vector3.Lerp(transform.position, destination, 0.01f);

        SetPlayerRotation();

        if (moveDirection == 0)
        {
            playerState = PlayerState.Idle;
        }
    }

    void ExecuteJump()
    {
        if (playerLocation == PlayerLocation.Grounded)
        {
            rb.AddForce(Vector3.up * _jumpSpeed);
            playerLocation = PlayerLocation.InAir;
            jumpCounter++;
        }

        float moveDirection = ReturnMoveDirection();

        //Write the case for in air maneuvering
        if (playerLocation == PlayerLocation.InAir)
        {
            transform.position += new Vector3(moveDirection, 0, 0) * AirMoveSpeed * Time.deltaTime;
            SetPlayerRotation();

            if (Input.GetKeyDown(KeyCode.Space) && jumpCounter < 2)
            {
                rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
                rb.AddForce(Vector3.up * _jumpSpeed);
                jumpCounter++;
                Debug.Log("Double jump");
            }
        }
    }

    //Checks for if a jump can be done and set to jump state
    void CheckForJump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && playerLocation == PlayerLocation.Grounded)
        {
            playerState = PlayerState.Jump;
        }
    }

    //Checks for if a run can be done and set to run state
    void CheckForRun()
    {
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow))
        {
            if (playerLocation == PlayerLocation.Grounded)
                playerState = PlayerState.Run;
        }
    }

    float ReturnMoveDirection()
    {
        float moveDirection = 0;

        if (Input.GetKey(KeyCode.LeftArrow))
            moveDirection = -1;
        else if (Input.GetKey(KeyCode.RightArrow))
            moveDirection = 1;

        return moveDirection;
    }

    void SetPlayerRotation()
    {
        float moveDirection = ReturnMoveDirection();
        float rotation = transform.rotation.eulerAngles.y;

        rotation = moveDirection != 0 ? Mathf.Rad2Deg * ((moveDirection * Mathf.PI) / 2 - Mathf.PI / 2) : rotation;

        transform.rotation = Quaternion.Euler(0, rotation, 0);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Platform" && playerLocation == PlayerLocation.InAir)
        {
            playerLocation = PlayerLocation.Grounded;
            playerState = PlayerState.Idle;
            jumpCounter = 0;

            Debug.Log("Grounded");

            CheckForRun();
        }
    }
}
