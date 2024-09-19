using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cainos.PixelArtTopDown_Basic
{
    public class EnemyScript : MonoBehaviour, IHittable
    {
        public int health = 100;
        public float flashDuration = 0.1f;
        private Color originalColor;
        public float speed;
        public float chaseRange = 5.0f;
        public float attackRange = 1.0f;
        public Transform FrontAttackTransform;
        public Transform LeftAttackTransform;
        public Transform RightAttackTransform;
        public Transform BackAttackTransform;
        public Vector2 attackSize = new Vector2(1f, 1f);
        private Animator animator;
        private SpriteRenderer spriteRenderer;
        private Transform player;
        private TopDownCharacterController playerControler;
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
            if (spriteRenderer != null)
            {
                originalColor = spriteRenderer.color;
            }
            parentEntity = gameObject;


            player = GameObject.FindGameObjectWithTag("Player").transform;
            if (player != null)
            {
                playerControler = player.GetComponent<TopDownCharacterController>();
            }

            SetAttackSize(FrontAttackTransform, attackSize);
            SetAttackSize(LeftAttackTransform, attackSize);
            SetAttackSize(RightAttackTransform, attackSize);
            SetAttackSize(BackAttackTransform, attackSize);
        }

        private void Update()
        {
            if (player == null || playerControler == null) return;

            float distanceToPlayer = Vector2.Distance(transform.position, player.position);


            if (playerControler.GetCurrentHealth() <= 0)
            {

                StopMovement();
                animator.SetFloat("Speed", 0);
                animator.SetFloat("Horizontal", 0);
                animator.SetFloat("Vertical", 0);
                return;
            }

            if (distanceToPlayer <= attackRange)
            {
                HandleAttack();
            }
            else if (distanceToPlayer <= chaseRange)
            {
                ChasePlayer();
            }
            else
            {

                StopMovement();
                animator.SetFloat("Speed", 0);
                animator.SetFloat("Horizontal", 0);
                animator.SetFloat("Vertical", 0);
            }

            if (comboTimer > 0) comboTimer -= Time.deltaTime;
            if (cooldownTimer > 0) cooldownTimer -= Time.deltaTime;

            if (comboTimer <= 0 && comboIndex > 0)
            {
                ResetCombo();
            }
        }

        private void ChasePlayer()
        {
            Vector2 direction = (player.position - transform.position).normalized;
            float currentSpeed = direction.magnitude * speed;

            animator.SetFloat("Speed", currentSpeed);
            animator.SetFloat("Horizontal", direction.x);
            animator.SetFloat("Vertical", direction.y);

            GetComponent<Rigidbody2D>().velocity = direction * speed;

            if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
            {
                if (direction.x > 0) setDirection("R");
                else setDirection("L");
            }
            else
            {
                if (direction.y > 0) setDirection("B");
                else setDirection("F");
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

        private void HandleAttack()
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
                if (collider.CompareTag("Player"))
                {
                    IHittable hittable = collider.GetComponent<IHittable>();
                    if (hittable != null && LayerMask.LayerToName(collider.gameObject.layer) == LayerMask.LayerToName(gameObject.layer))
                    {
                        hittable.TakeDamage(10);
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

        private void OnDrawGizmos()
        {
            DrawAttackGizmo(FrontAttackTransform);
            DrawAttackGizmo(LeftAttackTransform);
            DrawAttackGizmo(RightAttackTransform);
            DrawAttackGizmo(BackAttackTransform);
        }

        private void DrawAttackGizmo(Transform attackTransform)
        {
            if (attackTransform != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireCube(attackTransform.position, attackTransform.localScale);
            }
        }

        private void StopMovement()
        {
            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.velocity = Vector2.zero;
            }
        }
        public int GetCurrentHealth()
        {
            return health;
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
            Transform shadowTransform = transform.Find("Shadow");
            if (shadowTransform != null)
            {
                shadowTransform.gameObject.SetActive(false);
            }

            var movementScripts = GetComponents<MonoBehaviour>();
            foreach (var script in movementScripts)
            {
                if (script is TopDownCharacterController || script is EnemyScript)
                {
                    script.enabled = false;
                }
            }
            Animator animator = GetComponent<Animator>();
            if (animator != null)
            {
                animator.SetFloat("Horizontal", 0f);
                animator.SetFloat("Vertical", 0f);
                animator.SetFloat("Speed", 0f);
                animator.SetBool("isBlocking", false);
                animator.SetTrigger("Die");
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
}