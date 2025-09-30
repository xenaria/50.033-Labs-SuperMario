using UnityEngine;

public class Coin : MonoBehaviour
{
    [SerializeField] private int coinValue = 100;
    [SerializeField] private float lifeTime = 5f;
    [SerializeField] private Animator coinAnimator;

    void Start()
    {
        // Destroy coin after lifetime
        Destroy(gameObject, lifeTime);

        if (coinAnimator == null)
            coinAnimator = GetComponent<Animator>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            CollectCoin(other.gameObject);
        }
    }

    private void CollectCoin(GameObject player)
    {
        // Add score
        JumpOverGoomba scoreManager = FindFirstObjectByType<JumpOverGoomba>();
        if (scoreManager != null)
        {
            scoreManager.score += coinValue;
            // Update score display if needed
            PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();
            if (playerMovement != null && playerMovement.scoreText != null)
            {
                playerMovement.scoreText.text = "Score: " + scoreManager.score;
            }
        }

        // Play collection animation/sound here if needed

        // Destroy coin
        Destroy(gameObject);
    }
}