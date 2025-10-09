using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using TMPro;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.Events;
using Unity.VisualScripting;

public class PlayerMovement : MonoBehaviour
{
    /* Variable declarations */
    public float speed = 10;
    private bool onGroundState = true;
    private Rigidbody2D marioBody;

    public float maxSpeed = 20;
    public float upSpeed = 10;
    private SpriteRenderer marioSprite;
    private bool faceRightState = true;

    public GameObject enemies;
    public GameObject gameManager;

    public Animator marioAnimator;
    public AudioSource marioAudio;
    public AudioSource marioDeath;
    public float deathImpulse = 15;
    int collisionLayerMask = (1 << 3) | (1 << 6) | (1 << 7);
    // state
    [System.NonSerialized]
    public bool alive = true;
    public static System.Action OnGameRestart;
    public Transform gameCamera;
    private bool moving = false;
    private bool jumpedState = false;

    /*** Unity Callbacks ***/

    // Start is called before the first frame update
    void Start()
    {
        // Set to be 30 FPS
        Application.targetFrameRate = 30;
        marioBody = GetComponent<Rigidbody2D>();
        marioSprite = GetComponent<SpriteRenderer>();
        // update animator state
        marioAnimator.SetBool("onGround", onGroundState);
    }

    // Update is called once per frame
    void Update()
    {
        marioAnimator.SetFloat("xSpeed", Mathf.Abs(marioBody.linearVelocity.x));
    }

    // FixedUpdate is called 50 times a second
    void FixedUpdate()
    {
        if (alive && moving)
        {
            Move(faceRightState == true ? 1 : -1);
        }
    }

    /*** Movement Control ***/
    void FlipMarioSprite(int value)
    {
        if (value == -1 && faceRightState)
        {
            faceRightState = false;
            marioSprite.flipX = true;
            if (marioBody.linearVelocity.x > 0.05f)
                marioAnimator.SetTrigger("onSkid");

        }

        else if (value == 1 && !faceRightState)
        {
            faceRightState = true;
            marioSprite.flipX = false;
            if (marioBody.linearVelocity.x < -0.05f)
                marioAnimator.SetTrigger("onSkid");
        }
    }

    void Move(int value)
    {

        Vector2 movement = new Vector2(value, 0);
        // check if it doesn't go beyond maxSpeed
        if (marioBody.linearVelocity.magnitude < maxSpeed)
            marioBody.AddForce(movement * speed);
    }

    public void MoveCheck(int value)
    {
        if (value == 0)
        {
            moving = false;
        }
        else
        {
            FlipMarioSprite(value);
            moving = true;
            Debug.Log("MoveCheck() called");
            Move(value);
        }
    }

    public void Jump()
    {
        if (alive && onGroundState)
        {
            // jump
            marioBody.AddForce(Vector2.up * upSpeed, ForceMode2D.Impulse);
            onGroundState = false;
            jumpedState = true;
            // Debug.Log("Jumping");
            // update animator state
            marioAnimator.SetBool("onGround", onGroundState);

        }
    }

    public void JumpHold()
    {
        if (alive && jumpedState)
        {
            // jump higher
            // Debug.Log("Jumping more");
            marioBody.AddForce(Vector2.up * upSpeed * 30, ForceMode2D.Force);
            jumpedState = false;

        }
    }


    /*** OnCollisions ***/

    // Mario collides with ground
    void OnCollisionEnter2D(Collision2D col)
    {
        // this checks if mario is on the ground
        if (((collisionLayerMask & (1 << col.transform.gameObject.layer)) > 0) && !onGroundState)
        {
            onGroundState = true;
            //update animator state
            marioAnimator.SetBool("onGround", onGroundState);
        }

    }

    // Mario collides with Goomba
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Enemy") && alive)
        {
            Debug.Log("Collided with goomba!");

            // play death animation
            marioAnimator.Play("mario-die");
            // marioAudio.PlayOneShot(marioDeath);

            // prevent collision with Goomba to be retriggered
            alive = false;
            Time.timeScale = 0.0f;
            //ShowGameOverScreen();
            // detect stomp: Mario is moving downward AND above the enemy (tweak threshold as needed)
            float yThreshold = 0.15f;
            bool movingDown = marioBody != null && marioBody.linearVelocity.y < 0f;
            bool above = transform.position.y > other.transform.position.y + yThreshold;

            EnemyMovement enemy = other.GetComponent<EnemyMovement>();
            if (enemy != null && movingDown && above)
            {
                // bounce Mario up a bit
                if (marioBody != null)
                {
                    marioBody.linearVelocity = new Vector2(marioBody.linearVelocity.x, 0f);
                    marioBody.AddForce(Vector2.up * (upSpeed * 0.6f), ForceMode2D.Impulse);
                }

                // stomp the enemy
                enemy.Stomp();
            }
            else if (alive)
            {                // not a stomp -> Mario dies
                Debug.Log("Collided with goomba!");
                if (marioAnimator != null) marioAnimator.Play("mario-die");
                if (marioDeath != null && marioDeath.clip != null) marioDeath.PlayOneShot(marioDeath.clip);
                alive = false;
            }
        }
    }


    /*** Animation and Sounds ***/
    void PlayDeathImpulse()
    {
        marioBody.AddForce(Vector2.up * deathImpulse, ForceMode2D.Impulse);
    }
    void PlayJumpSound()
    {
        // play jump sound
        marioAudio.PlayOneShot(marioAudio.clip);
    }

    
    /*** Game Restart ***/
    public void RestartButtonCallback(int input)
    {
        // reset everything
        GameRestart();
        // resume time
        Time.timeScale = 1.0f;
    }
    

    public void GameRestart()
    {
        // reset position
        marioBody.transform.position = new Vector3(-8.0f, -5.71f, 0.0f);
        // reset sprite direction
        faceRightState = true;
        marioSprite.flipX = false;

        // reset Mario velocity to 0 to avoid randomly jumping
        marioBody.linearVelocity = Vector3.zero;
        // reset states
        onGroundState = true;
        marioAnimator.SetBool("onGround", true);
        marioAnimator.SetFloat("xSpeed", 0f);

        // reset animation
        marioAnimator.SetTrigger("gameRestart");
        alive = true;

        // reset camera position
        gameCamera.position = new Vector3(0, 0, -10);
    }
}
