using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollector : MonoBehaviour
{

    CharacterStat player;
    CircleCollider2D playerPickupRange;

    public float pullSpeed;
    void Start()
    {
        player = FindObjectOfType<CharacterStat>();
        playerPickupRange = GetComponent<CircleCollider2D>();
    }
    void Update()
    {
        playerPickupRange.radius = player.CurrentPickupRange;
    }
    private void OnTriggerEnter2D(Collider2D col)
    {
        //check if the game object have the ICollectible interface
        if (col.gameObject.TryGetComponent(out ICollectible collectible))
        {
            StartCoroutine(SuckInCollectible(col.transform, collectible));
        }
    }
    private IEnumerator SuckInCollectible(Transform collectibleTransform, ICollectible collectible)
    {
        if (collectible is Heart && player.CurrentHealth == player.CurrentMaxHealth)
        {
            yield break; // Exit the coroutine if the player is at full health
        }
        float duration = 0.5f; // Duration to pull the item
        float elapsed = 0f;

        Vector2 startPosition = collectibleTransform.position;
        Vector2 targetPosition = transform.position; // Player's position

        while (elapsed < duration)
        {
            // Check if the collectible object is still active
            if (collectibleTransform != null)
            {
                collectibleTransform.position = Vector2.Lerp(startPosition, targetPosition, elapsed / duration);
                elapsed += Time.deltaTime;
                yield return null; // Wait for the next frame
            }
            else
            {
                // Exit the coroutine if the collectible is destroyed
                yield break;
            }
        }

        // Ensure the collectible ends up at the player's position
        if (collectibleTransform != null)
        {
            collectibleTransform.position = targetPosition;
            collectible.Collect();
        }
    }
}
