using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cainos.PixelArtTopDown_Basic
{
    public class TopDownCharacterController : MonoBehaviour
    {
        public float speed;  // Speed of the character's movement

        private Animator animator; // Reference to the Animator component
        private SpriteRenderer spriteRenderer; // Reference to the SpriteRenderer component

        private void Start()
        {
            animator = GetComponent<Animator>();
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void Update()
        {
            // Get input values for movement
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");

            // Set the direction parameters for the blend tree
            animator.SetFloat("Horizontal", horizontal);
            animator.SetFloat("Vertical", vertical);

            // Calculate movement direction and speed
            Vector2 direction = new Vector2(horizontal, vertical).normalized;
            float currentSpeed = direction.magnitude * speed;

            // Set the Speed parameter to control idle and walking animations
            animator.SetFloat("Speed", currentSpeed);

            // Flip the sprite based on the horizontal input to handle left/right movement
            if (horizontal > 0)
            {
                spriteRenderer.flipX = false;  // Facing right
            }
            else if (horizontal < 0)
            {
                spriteRenderer.flipX = true;   // Facing left
            }

            // Move the character using Rigidbody2D
            GetComponent<Rigidbody2D>().velocity = direction * speed;
        }
    }
}
