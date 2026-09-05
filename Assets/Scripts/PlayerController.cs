using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;

    [Header("Components")]
    public Animator animator;
    public SpriteRenderer spriteRenderer;

    private Rigidbody2D rb;
    private Vector2 movement;
    private Vector2 lastDirection = Vector2.down;

    private bool isAttacking;
    private bool isDead;

    private string currentAnimation;

    private enum Direction
    {
        Down,
        Up,
        Right,
        Left
    }

    private Direction currentDirection = Direction.Down;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        if (animator == null)
            animator = GetComponent<Animator>();

        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (isDead)
            return;

        // Chuột trái để tấn công
        if (Input.GetMouseButtonDown(0) && !isAttacking)
        {
            StartAttack();
            return;
        }

        if (isAttacking)
        {
            movement = Vector2.zero;
            return;
        }

        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        // Ngăn di chuyển chéo nhanh hơn
        movement = movement.normalized;

        if (movement != Vector2.zero)
        {
            lastDirection = movement;
            UpdateDirection();
            PlayMoveAnimation();
        }
        else
        {
            PlayIdleAnimation();
        }
    }

    private void FixedUpdate()
    {
        if (isDead || isAttacking)
            return;

        rb.MovePosition(
            rb.position + movement * moveSpeed * Time.fixedDeltaTime
        );
    }

    private void UpdateDirection()
    {
        // Ưu tiên hướng có giá trị lớn hơn
        if (Mathf.Abs(lastDirection.x) > Mathf.Abs(lastDirection.y))
        {
            if (lastDirection.x > 0)
            {
                currentDirection = Direction.Right;
                spriteRenderer.flipX = false;
            }
            else
            {
                currentDirection = Direction.Left;

                // Hướng trái sử dụng animation hướng phải rồi lật lại
                spriteRenderer.flipX = true;
            }
        }
        else
        {
            spriteRenderer.flipX = false;

            if (lastDirection.y > 0)
                currentDirection = Direction.Up;
            else
                currentDirection = Direction.Down;
        }
    }

    private void PlayIdleAnimation()
    {
        switch (currentDirection)
        {
            case Direction.Down:
                PlayAnimation("Idle");
                break;

            case Direction.Up:
                PlayAnimation("Idleup");
                break;

            case Direction.Right:
            case Direction.Left:
                PlayAnimation("Idleright");
                break;
        }
    }

    private void PlayMoveAnimation()
    {
        switch (currentDirection)
        {
            case Direction.Down:
                PlayAnimation("Move");
                break;

            case Direction.Up:
                PlayAnimation("Moveup");
                break;

            case Direction.Right:
            case Direction.Left:
                PlayAnimation("Moveright");
                break;
        }
    }

    [Header("Combat Settings")]
    public float attackRange = 1.2f;

    private void StartAttack()
    {
        isAttacking = true;
        movement = Vector2.zero;

        // Thực hiện quét vùng sát thương cận chiến ngay khi bắt đầu tấn công
        PerformMeleeAttack();

        switch (currentDirection)
        {
            case Direction.Down:
                PlayAnimation("Attack");
                break;

            case Direction.Up:
                PlayAnimation("Attackup");
                break;

            case Direction.Right:
            case Direction.Left:
                PlayAnimation("Attackright");
                break;
        }
    }

    private void PerformMeleeAttack()
    {
        Vector2 attackOffset = Vector2.zero;
        switch (currentDirection)
        {
            case Direction.Down:
                attackOffset = Vector2.down * 0.8f;
                break;
            case Direction.Up:
                attackOffset = Vector2.up * 0.8f;
                break;
            case Direction.Right:
                attackOffset = Vector2.right * 0.8f;
                break;
            case Direction.Left:
                attackOffset = Vector2.left * 0.8f;
                break;
        }

        Vector2 attackPoint = (Vector2)transform.position + attackOffset;
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint, attackRange);

        int damageToDeal = 20;
        if (DefaultNamespace.GameManager.Instance != null && DefaultNamespace.GameManager.Instance.characterData != null)
        {
            damageToDeal = DefaultNamespace.GameManager.Instance.characterData.damage;
        }

        foreach (Collider2D enemyCollider in hitEnemies)
        {
            if (enemyCollider.CompareTag("Enemy"))
            {
                EnemyController enemy = enemyCollider.GetComponent<EnemyController>();
                if (enemy != null)
                {
                    enemy.TakeDamage(damageToDeal);
                }
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Vector2 attackOffset = Vector2.zero;
        switch (currentDirection)
        {
            case Direction.Down:
                attackOffset = Vector2.down * 0.8f;
                break;
            case Direction.Up:
                attackOffset = Vector2.up * 0.8f;
                break;
            case Direction.Right:
                attackOffset = Vector2.right * 0.8f;
                break;
            case Direction.Left:
                attackOffset = Vector2.left * 0.8f;
                break;
        }

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere((Vector2)transform.position + attackOffset, attackRange);
    }


    // Animation Event gọi hàm này ở frame cuối animation Attack
    public void EndAttack()
    {
        isAttacking = false;
        currentAnimation = "";

        PlayIdleAnimation();
    }

    public void Die()
    {
        if (isDead)
            return;

        isDead = true;
        isAttacking = false;
        movement = Vector2.zero;

        spriteRenderer.flipX = false;
        PlayAnimation("Die");
    }

    private void PlayAnimation(string animationName)
    {
        // Không chạy lại cùng một animation liên tục
        if (currentAnimation == animationName)
            return;

        currentAnimation = animationName;
        animator.Play(animationName);
    }
}