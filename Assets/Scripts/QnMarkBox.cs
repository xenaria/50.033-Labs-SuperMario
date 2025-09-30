using UnityEngine;

public class QuestionBox : MonoBehaviour
{
    [Header("Coin Settings")]
    [SerializeField] private GameObject coinPrefab;
    [SerializeField] private int coinsToSpawn = 1;
    [SerializeField] private float coinSpawnForce = 10f;

    [Header("Animation")]
    [SerializeField] private Animator boxAnimator;
    [SerializeField] private float hitAnimationDuration = 0.5f;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip coinSound;
    [SerializeField] private AudioClip emptyBoxSound;

    private bool isEmpty = false;
    private int coinsLeft;

    void Start()
    {
        coinsLeft = coinsToSpawn;

        // Get components if not assigned
        if (boxAnimator == null)
            boxAnimator = GetComponent<Animator>();
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if hit from below by player
        if (collision.gameObject.CompareTag("Player"))
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
        if (isEmpty)
        {
            // Play empty box animation and sound
            PlayEmptyBoxFeedback();
            return;
        }

        // Spawn coin
        SpawnCoin();

        // Play hit animation
        PlayHitAnimation();

        // Update state
        coinsLeft--;
        if (coinsLeft <= 0)
        {
            isEmpty = true;
            // Change to empty box sprite/animation
            boxAnimator.SetBool("isEmpty", true);
        }
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

            // Play coin sound
            if (audioSource != null && coinSound != null)
            {
                audioSource.PlayOneShot(coinSound);
            }
        }
    }

    private void PlayHitAnimation()
    {
        // Trigger hit animation
        boxAnimator.SetTrigger("onHit");

        // Small upward bump animation
        StartCoroutine(BumpAnimation());
    }

    private System.Collections.IEnumerator BumpAnimation()
    {
        Vector3 originalPosition = transform.position;
        Vector3 bumpPosition = originalPosition + Vector3.up * 0.2f;

        float timer = 0f;

        // Move up
        while (timer < hitAnimationDuration / 2)
        {
            timer += Time.deltaTime;
            float progress = timer / (hitAnimationDuration / 2);
            transform.position = Vector3.Lerp(originalPosition, bumpPosition, progress);
            yield return null;
        }

        timer = 0f;

        // Move back down
        while (timer < hitAnimationDuration / 2)
        {
            timer += Time.deltaTime;
            float progress = timer / (hitAnimationDuration / 2);
            transform.position = Vector3.Lerp(bumpPosition, originalPosition, progress);
            yield return null;
        }

        transform.position = originalPosition;
    }

    private void PlayEmptyBoxFeedback()
    {
        // Play empty sound
        if (audioSource != null && emptyBoxSound != null)
        {
            audioSource.PlayOneShot(emptyBoxSound);
        }

        // Small shake animation
        StartCoroutine(ShakeAnimation());
    }

    private System.Collections.IEnumerator ShakeAnimation()
    {
        Vector3 originalPosition = transform.position;
        float timer = 0f;

        while (timer < 0.2f)
        {
            timer += Time.deltaTime;
            float offsetX = Random.Range(-0.1f, 0.1f);
            transform.position = originalPosition + Vector3.right * offsetX;
            yield return null;
        }

        transform.position = originalPosition;
    }
}