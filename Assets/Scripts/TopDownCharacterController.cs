using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cainos.PixelArtTopDown_Basic
{
    public class TopDownCharacterController : MonoBehaviour
    {
        public float speed;
        public Transform FrontAttackTransform;
        public Transform LeftAttackTransform;
        public Transform RightAttackTransform;
        public Transform BackAttackTransform;
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
        private string[] comboTriggers = { "AM Player Punch1", "AM Player Punch2", "AM Player Punch1" };

        private void Start()
        {
            animator = GetComponent<Animator>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            parentEntity = gameObject;
            SetAttackSize(FrontAttackTransform, attackSize);
            SetAttackSize(LeftAttackTransform, attackSize);
            SetAttackSize(RightAttackTransform, attackSize);
            SetAttackSize(BackAttackTransform, attackSize);
        }
        private void OnDrawGizmos()
        {
            DrawAttackGizmo(FrontAttackTransform);
            DrawAttackGizmo(LeftAttackTransform);
            DrawAttackGizmo(RightAttackTransform);
            DrawAttackGizmo(BackAttackTransform);
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
            if (cooldownTimer > 0 || isAttacking) return;

            if (comboIndex == 0 || (comboIndex > 0 && comboTimer > 0))
            {
                comboTimer = comboTimeWindow;
                isAttacking = true;
                animator.SetTrigger(comboTriggers[comboIndex]);
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
            Transform attackTransform = GetCurrentAttackTransform();
            if (attackTransform == null) return;

            Collider2D[] colliders = Physics2D.OverlapBoxAll(attackTransform.position, attackTransform.localScale, 0);
            foreach (Collider2D collider in colliders)
            {
                Health health = collider.GetComponent<Health>();
                if (health != null && LayerMask.LayerToName(collider.gameObject.layer) == LayerMask.LayerToName(gameObject.layer))
                {
                    if (collider.transform.root != parentEntity.transform)
                    {
                        health.TakeDamage(10);
                    }
                }
            }
        }
        private Transform GetCurrentAttackTransform()
        {
            if (animator.GetBool("F")) return FrontAttackTransform;
            if (animator.GetBool("L")) return LeftAttackTransform;
            if (animator.GetBool("R")) return RightAttackTransform;
            if (animator.GetBool("B")) return BackAttackTransform;
            return null;
        }
        private void SetAttackSize(Transform attackTransform, Vector2 size)
        {
            if (attackTransform == null) return;

            var collider = attackTransform.GetComponent<BoxCollider2D>();
            if (collider == null)
            {
                collider = attackTransform.gameObject.AddComponent<BoxCollider2D>();
            }
            collider.size = size;
        }
        private void DrawAttackGizmo(Transform attackTransform)
        {
            if (attackTransform != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireCube(attackTransform.position, attackTransform.localScale);
            }
        }
    }
}