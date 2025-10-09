using UnityEngine;

public class SpawnCoin : MonoBehaviour
{
    [Header("Coin Settings")]
    public GameObject coinPrefab;
    public int coinsToSpawn = 1;
    public float coinSpawnForce = 10f;
    private int coinsLeft;
    private Vector3 originalPosition;
    public GameManager gameManager;

    [Header("Audio")]
    public AudioClip coinSound;
    public AudioSource boxAudio;

  
    void Start()
    {
        coinsLeft = coinsToSpawn;
        originalPosition = transform.position;
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

        if (coinsLeft > 0)
            {
            SpawnCoinAnimation();
            gameManager.IncreaseScore(1);
                coinsLeft -= 1;
            }
    }

    
    private void SpawnCoinAnimation()
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
}
