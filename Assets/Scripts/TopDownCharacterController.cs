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



            // Move the character using Rigidbody2D
            GetComponent<Rigidbody2D>().velocity = direction * speed;
            if (horizontal > 0)
            {
                setDirection("R");
            }
            else if (horizontal < 0)
            {
                setDirection("L");
            }
            if (vertical > 0)
            {
                setDirection("B");
            }
            else if (vertical < 0)
            {
                setDirection("F");
            }

        }
        void setDirection(string direction)
        {
            animator.SetBool("R", false);
            animator.SetBool("L", false);
            animator.SetBool("B", false);
            animator.SetBool("F", false);

            animator.SetBool(direction, true);
        }
    }
}