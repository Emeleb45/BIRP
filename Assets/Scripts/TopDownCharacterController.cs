using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cainos.PixelArtTopDown_Basic
{
    public class TopDownCharacterController : MonoBehaviour
    {
        public float speed;
        public Transform FrontAttackTransform; // Reference to the F Attack transform
        public Vector2 attackSize = new Vector2(1f, 1f);

        private Animator animator;
        private SpriteRenderer spriteRenderer;

        private int comboIndex = 0;
        private float comboTimer = 0f;
        private float comboTimeWindow = 1f;
        private float cooldownTimer = 0f;
        private float cooldownTime = 1.0f;

        private bool isAttacking = false;
        private GameObject parentEntity;
        private string[] comboTriggers = { "AM Player F Punch1", "AM Player F Punch2", "AM Player F Punch1" };

        private void Start()
        {
            animator = GetComponent<Animator>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            parentEntity = gameObject;
            SetAttackSize(attackSize);
        }
        private void OnDrawGizmos()
        {
            if (FrontAttackTransform != null)
            {
                // Draw a wireframe cube representing the attack area
                Gizmos.color = Color.red; // Set color to red for visibility
                Gizmos.DrawWireCube(FrontAttackTransform.position, FrontAttackTransform.localScale);
            }
        }
        private void Update()
        {
            // Handle movement input
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");

            animator.SetFloat("Horizontal", horizontal);
            animator.SetFloat("Vertical", vertical);

            Vector2 direction = new Vector2(horizontal, vertical).normalized;
            float currentSpeed = direction.magnitude * speed;

            animator.SetFloat("Speed", currentSpeed);
            GetComponent<Rigidbody2D>().velocity = direction * speed;

            // Set direction for animation
            if (horizontal > 0) { setDirection("R"); }
            else if (horizontal < 0) { setDirection("L"); }
            if (vertical > 0) { setDirection("B"); }
            else if (vertical < 0) { setDirection("F"); }

            if (comboTimer > 0) comboTimer -= Time.deltaTime;
            if (cooldownTimer > 0) cooldownTimer -= Time.deltaTime;

            if (Input.GetKeyDown(KeyCode.F))
            {
                HandlePunchInput();
            }

            if (comboTimer <= 0 && comboIndex > 0)
            {
                ResetCombo();
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

        private void HandlePunchInput()
        {
            if (cooldownTimer > 0 || isAttacking == true) return;

            if (comboIndex == 0 || (comboIndex > 0 && comboTimer > 0))
            {
                comboTimer = comboTimeWindow;
                animator.SetTrigger(comboTriggers[comboIndex]);
                isAttacking = true;
                comboIndex++;

                if (comboIndex >= comboTriggers.Length)
                {
                    StartCooldown();
                }
            }
        }

        private void ResetCombo()
        {
            comboIndex = 0;
            comboTimer = 0f;
        }

        private void StartCooldown()
        {
            cooldownTimer = cooldownTime;
            ResetCombo();
        }

        public void EndAttack()
        {
            isAttacking = false;


        }


        public void StartAttack()
        {


            CheckForDamage();
        }

        private void CheckForDamage()
        {
            // Get all colliders within the attackTransform's bounds
            Collider2D[] colliders = Physics2D.OverlapBoxAll(FrontAttackTransform.position, FrontAttackTransform.localScale, 0);

            foreach (Collider2D collider in colliders)
            {
                // Check if the collider has a Health component
                Health health = collider.GetComponent<Health>();

                // Check if the collider is on the same layer and is not the parent entity
                if (health != null && LayerMask.LayerToName(collider.gameObject.layer) == LayerMask.LayerToName(gameObject.layer))
                {
                    // Ensure the collider's parent is not the same as the script's parent
                    if (collider.transform.root != parentEntity.transform)
                    {
                        // Apply damage
                        health.TakeDamage(10); // Example damage value
                    }
                }
            }
        }
        private void SetAttackSize(Vector2 size)
        {
            // Ensure FrontAttackTransform has a BoxCollider2D to represent the attack size
            var collider = FrontAttackTransform.GetComponent<BoxCollider2D>();
            if (collider == null)
            {
                collider = FrontAttackTransform.gameObject.AddComponent<BoxCollider2D>();
            }
            collider.size = size;
        }
    }
}