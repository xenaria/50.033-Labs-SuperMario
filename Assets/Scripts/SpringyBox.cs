using UnityEngine;

public class SpringyBox : MonoBehaviour
{

    [Header("Bounce Settings")]
    [SerializeField] private float bounceHeight = 0.2f;
    [SerializeField] private float bounceDuration = 0.3f;

    private Vector3 originalPosition;
    private bool isBouncing = false;


    void Start()
    {
        originalPosition = transform.position;
    }
    
    void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if hit from below by player
        if (collision.gameObject.CompareTag("Player"))
        {
            Vector2 hitDirection = collision.contacts[0].normal;

            // Check if player hit from below (normal points up)
            if (hitDirection.y > 0.5f && !isBouncing)
            {
                StartCoroutine(BounceBox());
            }
        }
    }

    private System.Collections.IEnumerator BounceBox()
    {
        isBouncing = true;
        
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
        isBouncing = false;
    }

}
