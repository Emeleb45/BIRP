using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cainos.PixelArtTopDown_Basic
{
    public class TopDownCharacterController : MonoBehaviour
    {
        public float speed;  

        private Animator animator; 
        private SpriteRenderer spriteRenderer; 

        private void Start()
        {
            animator = GetComponent<Animator>();
            spriteRenderer = GetComponent<SpriteRenderer>();
        }
        
        private void Update()
        {

            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");

            animator.SetFloat("Horizontal", horizontal);
            animator.SetFloat("Vertical", vertical);

            Vector2 direction = new Vector2(horizontal, vertical).normalized;
            float currentSpeed = direction.magnitude * speed;

            animator.SetFloat("Speed", currentSpeed);

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