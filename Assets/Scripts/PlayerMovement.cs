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
    public TextMeshProUGUI scoreText;
    public GameObject enemies;
    public JumpOverGoomba jumpOverGoomba;
    public GameObject gameOverScreen;
    public TextMeshProUGUI finalScore;
    public Animator marioAnimator;
    public AudioSource marioAudio;
    public AudioClip marioDeath;
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
            marioAudio.PlayOneShot(marioDeath);
            // prevent collision with Goomba to be retriggered
            alive = false;
            ShowGameOverScreen();
        }
    }
    
    
    /*** Game Restart ***/
    public void ShowGameOverScreen()
    {
        Time.timeScale = 0.0f;
        gameOverScreen.SetActive(true);
        finalScore.text = "Score: " + jumpOverGoomba.score;
    }

    public void RestartButtonCallback(int input)
    {
        // reset everything
        ResetGame();
        // resume time
        Time.timeScale = 1.0f;
    }
    
    private void ResetGame()
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

        // reset score
        scoreText.text = "Score: 0";
        
        // reset Goomba
        foreach (Transform eachChild in enemies.transform)
        {
            eachChild.transform.localPosition = eachChild.GetComponent<EnemyMovement>().startPosition;
        }
        // reset score
        jumpOverGoomba.score = 0;
        // hide game over screen
        gameOverScreen.SetActive(false);
        // reset animation
        marioAnimator.SetTrigger("gameRestart");
        alive = true;
        OnGameRestart?.Invoke();        // reset camera position
        gameCamera.position = new Vector3(0, 0, -10);

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

}
