using UnityEngine;

public class QuestionBox : MonoBehaviour
{
    [Header("Coin Settings")]
    [SerializeField] private GameObject coinPrefab;
    [SerializeField] private int coinsToSpawn = 1;
    [SerializeField] private float coinSpawnForce = 10f;

    [Header("Animation")]
    [SerializeField] private Animator boxAnimator;
    [SerializeField] bool isHit = false;

    [Header("Audio")]
    public AudioSource boxAudio;
    [SerializeField] private AudioClip coinSound;

    
    [Header("Bounce Settings")]
    [SerializeField] private float bounceHeight = 0.2f;
    [SerializeField] private float bounceDuration = 0.3f;

    [Header("Sprite Settings")]
    [SerializeField] private GameObject questionBoxSprite; 
    [SerializeField] private Sprite emptyBoxSprite; 
    private SpriteRenderer QnBoxSprite;

    private int coinsLeft;
    private Vector3 originalPosition;

    void Start()
    {
        coinsLeft = coinsToSpawn;
        originalPosition = transform.position;

        // Get components if not assigned
        if (boxAnimator == null)
            boxAnimator = GetComponent<Animator>();
        if (boxAudio == null)
            boxAudio = GetComponent<AudioSource>();

        QnBoxSprite = GetComponent<SpriteRenderer>();
        PlayerMovement.OnGameRestart += ResetBox;
    }
    void OnDestroy()
    {
        // Unsubscribe to prevent memory leaks
        PlayerMovement.OnGameRestart -= ResetBox;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if hit from below by player
        if (collision.gameObject.CompareTag("Player") && !isHit)
        {
            Vector2 hitDirection = collision.contacts[0].normal;

            // Check if player hit from below (normal points up)
            if (hitDirection.y > 0.5f)
            {
                HitBox();
            }
        }
    }

    private void HitBox()
    {
        boxAudio.PlayOneShot(boxAudio.clip);

        if (!isHit)
        {
            isHit = true;
            StartCoroutine(BounceBox());
        }
        if (coinsLeft > 0)
        {
            SpawnCoin();
            coinsLeft = 0;
            if (boxAnimator != null)
            {
                boxAnimator.enabled = false;
            }       
            if (emptyBoxSprite != null && QnBoxSprite != null)
            {
                QnBoxSprite.sprite = emptyBoxSprite;
                Debug.Log("Changed to brown box sprite");
            }                       
        }
    }


    private System.Collections.IEnumerator BounceBox()
    {
        float elapsedTime = 0f;
        Vector3 startPos = originalPosition;
        Vector3 bouncePos = originalPosition + Vector3.up * bounceHeight;

        // Move up
        while (elapsedTime < bounceDuration / 2)
        {
            transform.position = Vector3.Lerp(startPos, bouncePos, elapsedTime / (bounceDuration / 2));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        elapsedTime = 0f;

        // Move back down
        while (elapsedTime < bounceDuration / 2)
        {
            transform.position = Vector3.Lerp(bouncePos, startPos, elapsedTime / (bounceDuration / 2));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = originalPosition;
    }

    private void SpawnCoin()
    {

        if (coinPrefab != null)
        {
            // Spawn coin above the box
            Vector3 spawnPosition = transform.position + Vector3.up * 1f;
            GameObject coin = Instantiate(coinPrefab, spawnPosition, Quaternion.identity);

            // Add upward force to coin
            Rigidbody2D coinRb = coin.GetComponent<Rigidbody2D>();
            if (coinRb != null)
            {
                coinRb.AddForce(Vector2.up * coinSpawnForce, ForceMode2D.Impulse);
            }

            if (boxAudio != null && coinSound != null)
            {
                boxAudio.PlayOneShot(coinSound);
            }
        }
    }

    public void ResetBox()
    {
        isHit = false;
        coinsLeft = coinsToSpawn;
        transform.position = originalPosition;
        
        // Reset animator state
        if (boxAnimator != null)
        {
            boxAnimator.enabled = true;
        }

    }
}
