using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{

    private float originalX;
    private float maxOffset = 5.0f;
    private float enemyPatroltime = 2.0f;
    private int moveRight = -1;
    private Vector2 velocity;
    private Rigidbody2D enemyBody;
    public Vector3 startPosition = new Vector3(0.0f, 0.0f, 0.0f);
    // stomp behaviour fields
    public Animator animator;
    public float stompBounce = 6f;
    public float stompDestroyDelay = 0.6f;
    private Collider2D enemyCollider;
    private bool alive = true;

    public AudioClip stompClip;
    public AudioSource sfxSource;
    void Start()
    {
        enemyBody = GetComponent<Rigidbody2D>();
        // get the starting position
        enemyCollider = GetComponent<Collider2D>();
        if (animator == null) animator = GetComponent<Animator>();
        originalX = transform.position.x;
        startPosition = transform.position;
        ComputeVelocity();
    }
    void ComputeVelocity()
    {
        velocity = new Vector2((moveRight) * maxOffset / enemyPatroltime, 0);
    }
    void Movegoomba()
    {
        enemyBody.MovePosition(enemyBody.position + velocity * Time.fixedDeltaTime);
    }

    // note that this is Update(), which still works but not ideal. See below.
    void Update()
    {
        if (!alive) return;
    }

    void FixedUpdate()
    {
        if (!alive) return;
        if (Mathf.Abs(enemyBody.position.x - originalX) < maxOffset)
        {// move goomba
            Movegoomba();
        }
        else
        {
            // change direction
            moveRight *= -1;
            ComputeVelocity();
            Movegoomba();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {

    }

    public void Stomp()
    {
        if (sfxSource != null && stompClip != null)
        {
            sfxSource.PlayOneShot(stompClip);
        }
        if (!alive) return;
        alive = false;

        if (enemyCollider != null) enemyCollider.enabled = false;

        if (enemyBody != null)
        {
            enemyBody.linearVelocity = Vector2.zero;
            // Keep as Kinematic - no change needed if already Kinematic
        }

        if (animator != null) animator.SetTrigger("stomp");

        // Instead of Destroy, just hide after delay
        StartCoroutine(HideAfterDelay(stompDestroyDelay));
    }

    private IEnumerator HideAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        gameObject.SetActive(false); // Hide instead of destroy
    }

    void PlayJumpSound()
    {
        // play jump sound
        if (sfxSource != null && stompClip != null) sfxSource.PlayOneShot(stompClip);
    }

    public void ResetState()
    {
        // restore flag
        alive = true;

        // ensure references
        if (enemyCollider == null) enemyCollider = GetComponent<Collider2D>();
        if (enemyBody == null) enemyBody = GetComponent<Rigidbody2D>();
        if (animator == null) animator = GetComponent<Animator>();

        // restore transform & saved start position
        transform.position = startPosition;
        transform.localScale = Vector3.one;

        // restore physics
        if (enemyBody != null)
        {
            enemyBody.bodyType = RigidbodyType2D.Dynamic;
            enemyBody.linearVelocity = Vector2.zero;
            enemyBody.angularVelocity = 0f;
        }

        // restore collider and animator
        if (enemyCollider != null) enemyCollider.enabled = true;
        if (animator != null)
        {
            animator.ResetTrigger("stomp");
            animator.Play("goomba-idle",0,0);
        }
        // reset patrol variables
        originalX = startPosition.x;
        moveRight = -1;
        ComputeVelocity();
        // ensure object is active
        gameObject.SetActive(true);
    }
}