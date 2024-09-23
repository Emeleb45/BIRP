using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Cainos.PixelArtTopDown_Basic
{
    public class TopDownCharacterController : MonoBehaviour, IHittable
    {
        public float speed;
        public Transform FrontAttackTransform;
        public float blockCooldown = 1f;
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
        private bool isBlocking = false;
        private GameObject parentEntity;
        private string[] comboTriggers = { "AM Player Punch1", "AM Player Punch2", "AM Player Punch1" };
        public int health = 100;
        public float flashDuration = 0.1f;
        public GameObject HealthBar;

        private Color originalColor;

        private void Start()
        {


            animator = GetComponent<Animator>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                originalColor = spriteRenderer.color;
            }
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
            if (!isBlocking)
            {
                Vector2 direction = new Vector2(horizontal, vertical).normalized;
                float currentSpeed = direction.magnitude * speed;

                animator.SetFloat("Speed", currentSpeed);
                GetComponent<Rigidbody2D>().velocity = direction * speed;


            }



            if (horizontal > 0) { setDirection("R"); }
            else if (horizontal < 0) { setDirection("L"); }
            if (vertical > 0) { setDirection("B"); }
            else if (vertical < 0) { setDirection("F"); }

            if (comboTimer > 0) comboTimer -= Time.deltaTime;
            if (cooldownTimer > 0) cooldownTimer -= Time.deltaTime;

            if (Input.GetKeyDown(KeyCode.F))
            {
                if (isBlocking == true)
                {
                    HandlePunchInput();
                }


            }
            if (Input.GetKeyDown(KeyCode.Q))
            {
                StartBlockInput();
            }
            if (Input.GetKeyUp(KeyCode.Q))
            {
                EndBlockInput();
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
        private void StartBlockInput()
        {
            isBlocking = true;
            animator.SetBool("isBlocking", true);
        }
        private void EndBlockInput()
        {
            isBlocking = false;
            animator.SetBool("isBlocking", false);
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
                IHittable hittable = collider.GetComponent<IHittable>();
                if (hittable != null && LayerMask.LayerToName(collider.gameObject.layer) == LayerMask.LayerToName(gameObject.layer))
                {
                    if (collider.transform.root != parentEntity.transform)
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
        private void DrawAttackGizmo(Transform attackTransform)
        {
            if (attackTransform != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireCube(attackTransform.position, attackTransform.localScale);
            }
        }
        public int GetCurrentHealth()
        {
            return health;
        }

        public void TakeDamage(int damage)
        {
            if (!isBlocking)
            {
                health -= damage;
                HealthBar.GetComponent<Image>().fillAmount = health / 100f;
                if (health <= 0)
                {
                    Die();
                }
                else
                {
                    StartCoroutine(FlashColor(1f, 0f, 0f, 1f));
                }
            }
            else
            {
                StartCoroutine(FlashColor(0f, 0f, 1f, 1f));
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

        private IEnumerator FlashColor(float red, float green, float blue, float alpha)
        {
            if (spriteRenderer != null)
            {

                spriteRenderer.color = new Color(red, green, blue, alpha);
            }

            yield return new WaitForSeconds(flashDuration);

            if (spriteRenderer != null)
            {

                spriteRenderer.color = originalColor;
            }
        }
    }
}