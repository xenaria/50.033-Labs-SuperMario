using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using TMPro;
using UnityEngine.SocialPlatforms.Impl;

public class PlayerMovement : MonoBehaviour
{
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
    private bool jumpInput;
    public AudioClip marioDeath;
    public float deathImpulse = 15;
    int collisionLayerMask = (1 << 3) | (1 << 6) | (1 << 7);
    // state
    [System.NonSerialized]
    public bool alive = true;
    public static System.Action OnGameRestart;

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

        if (Input.GetKeyDown("space") && onGroundState)
            jumpInput = true;

        // toggle state to flip mario
        if (Input.GetKeyDown("a") && faceRightState)
        {
            faceRightState = false;
            marioSprite.flipX = true;
            if (marioBody.linearVelocity.x > 0.1f)
                marioAnimator.SetTrigger("onSkid");
        }

        if (Input.GetKeyDown("d") && !faceRightState)
        {
            faceRightState = true;
            marioSprite.flipX = false;
            if (marioBody.linearVelocity.x < -0.1f)
                marioAnimator.SetTrigger("onSkid");
        }
        marioAnimator.SetFloat("xSpeed", Mathf.Abs(marioBody.linearVelocity.x));
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        // this checks if mario is on the ground
        if (((collisionLayerMask &(1 << col.transform.gameObject.layer)) > 0 ) && !onGroundState)
        {
            onGroundState = true;
            //update animator state
            marioAnimator.SetBool("onGround", onGroundState);
        }

    }

    // FixedUpdate is called 50 times a second
    void FixedUpdate()
    {
        if (alive)
        {
            float moveHorizontal = Input.GetAxisRaw("Horizontal");

            if (Mathf.Abs(moveHorizontal) > 0)
            {
                Vector2 movement = new Vector2(moveHorizontal, 0);
                // check if it doesn't go beyond maxSpeed
                if (marioBody.linearVelocity.magnitude < maxSpeed)
                    marioBody.AddForce(movement * speed);
            }

            // stop mario when A and D keys are lifted up
            /*if (Input.GetKeyUp("a") || Input.GetKeyUp("d"))
            {
                // stop
                marioBody.linearVelocity = Vector2.zero;
            }*/

            // Handle jump using stored input
            if (jumpInput)
            {
                marioBody.AddForce(Vector2.up * upSpeed, ForceMode2D.Impulse);
                onGroundState = false;
                // update animator state
                marioAnimator.SetBool("onGround", onGroundState);
                jumpInput = false; // Reset the flag
            }

        }
    }

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
    void PlayDeathImpulse()
    {
        marioBody.AddForce(Vector2.up * deathImpulse, ForceMode2D.Impulse);
    }

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
        OnGameRestart?.Invoke();
    }

    void PlayJumpSound()
    {
        // play jump sound
        marioAudio.PlayOneShot(marioAudio.clip);
    }

}
