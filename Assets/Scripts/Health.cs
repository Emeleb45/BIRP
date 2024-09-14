using UnityEngine;
using System.Collections;
public class Health : MonoBehaviour
{
    public int health = 100;
    public float flashDuration = 0.1f;

    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }
    }
    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Die();
        }
        else
        {
            StartCoroutine(FlashRed());
        }
    }

    private void Die()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.red;
            spriteRenderer.sortingOrder = 1;
        }

        Collider2D[] colliders = GetComponents<Collider2D>();
        foreach (var collider in colliders)
        {
            Destroy(collider);
        }

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            Destroy(rb);
        }

        transform.rotation = Quaternion.Euler(0, 0, -90);

        Destroy(gameObject, 15f); 
    }
    private IEnumerator FlashRed()
    {

        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.red;
        }

        yield return new WaitForSeconds(flashDuration);

        if (spriteRenderer != null)
        {
            spriteRenderer.color = originalColor;
        }
    }
}
