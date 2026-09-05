using System.Collections;
using UnityEngine;
using DefaultNamespace;

public class EnemyController : MonoBehaviour
{
    // =========================================
    // DATA
    // =========================================
    [Header("Data")]
    public EnemyData enemyData;


    // =========================================
    // REFERENCES
    // =========================================
    [Header("References")]
    public Animator animator;
    public SpriteRenderer spriteRenderer;
    public EnemyHealth enemyHealth;


    // =========================================
    // DIRECTION
    // =========================================
    public enum Direction
    {
        Down,
        Up,
        Right,
        Left
    }


    private Direction currentDirection = Direction.Down;

    private Vector2 lastDirection = Vector2.down;

    private string currentAnimation = "";


    // =========================================
    // STATE
    // =========================================
    public enum EnemyState
    {
        Patrol,
        Chase,
        Attack
    }


    private EnemyState currentState = EnemyState.Patrol;

    private bool isAttacking = false;

    private bool isDead = false;


    // =========================================
    // TARGET
    // =========================================
    private enum TargetType
    {
        Player,
        Farm
    }


    private TargetType currentTargetType;

    private Transform currentTarget;

    private Transform player;

    private FarmHealth farmHealth;


    // 0.5 = 50% Farm
    // 0.5 = 50% Player
    [Header("Target Chance")]
    [Range(0f, 1f)]
    public float farmTargetChance = 0.5f;


    // =========================================
    // PATROL
    // =========================================
    [Header("Patrol")]
    public float moveDistance = 3f;


    public enum PatrolAxis
    {
        Horizontal,
        Vertical
    }


    public PatrolAxis patrolAxis = PatrolAxis.Horizontal;


    // =========================================
    // CHASE
    // =========================================
    [Header("Chase")]
    public float chaseRange = 5f;

    public float losePlayerRange = 8f;


    // =========================================
    // ATTACK
    // =========================================
    [Header("Attack")]
    public float attackRange = 1f;

    public float attackCoolDown = 1f;


    private float nextAttack = 0f;


    // =========================================
    // RUNTIME
    // =========================================
    private int currentHp;

    private float moveSpeed;

    private Vector3 startPosition;

    private int direction = 1;


    // =========================================
    // AWAKE
    // =========================================
    private void Awake()
    {
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }


        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }


        if (enemyHealth == null)
        {
            enemyHealth =
                GetComponentInChildren<EnemyHealth>();
        }
    }


    // =========================================
    // START
    // =========================================
    void Start()
    {
        startPosition = transform.position;


        // =====================================
        // TÌM PLAYER
        // =====================================
        GameObject playerObject =
            GameObject.FindGameObjectWithTag("Player");


        if (playerObject != null)
        {
            player = playerObject.transform;
        }
        else
        {
            Debug.LogWarning(
                "Không tìm thấy Player có Tag Player!"
            );
        }


        // =====================================
        // TÌM FARM
        // =====================================
        farmHealth =
            FindFirstObjectByType<FarmHealth>();


        if (farmHealth == null)
        {
            Debug.LogWarning(
                "Không tìm thấy FarmHealth trong Scene!"
            );
        }


        // =====================================
        // ENEMY DATA
        // =====================================
        if (enemyData != null)
        {
            currentHp = enemyData.hp;

            moveSpeed = enemyData.speed;


            if (enemyData.enemySprite != null &&
                spriteRenderer != null)
            {
                spriteRenderer.sprite =
                    enemyData.enemySprite;
            }


            if (enemyData.animatorController != null &&
                animator != null)
            {
                animator.runtimeAnimatorController =
                    enemyData.animatorController;
            }


            if (enemyHealth != null)
            {
                enemyHealth.Init(currentHp);
            }


            Debug.Log(
                $"[{enemyData.enemyName}] " +
                $"HP={currentHp} | " +
                $"Speed={moveSpeed} | " +
                $"Type={enemyData.enemyType} | " +
                $"CanFly={enemyData.canFly}"
            );
        }
        else
        {
            Debug.LogWarning(
                $"{gameObject.name}: Chưa gán EnemyData!"
            );


            currentHp = 10;

            moveSpeed = 2f;
        }


        // Animation ban đầu
        PlayIdleAnimation();


        // =====================================
        // RANDOM TARGET 50 / 50
        // =====================================
        ChooseTarget();
    }


    // =========================================
    // CHỌN TARGET
    // =========================================
    private void ChooseTarget()
    {
        bool canAttackPlayer =
            player != null;


        bool canAttackFarm =
            farmHealth != null &&
            !farmHealth.IsDestroyed();


        // Không còn mục tiêu nào
        if (!canAttackPlayer &&
            !canAttackFarm)
        {
            currentTarget = null;

            PlayIdleAnimation();

            return;
        }


        // =====================================
        // NẾU CẢ PLAYER VÀ FARM ĐỀU TỒN TẠI
        // =====================================
        if (canAttackPlayer &&
            canAttackFarm)
        {
            float random =
                UnityEngine.Random.value;


            // ===============================
            // FARM
            // ===============================
            if (random < farmTargetChance)
            {
                SelectFarmTarget();
            }


            // ===============================
            // PLAYER
            // ===============================
            else
            {
                SelectPlayerTarget();
            }
        }


        // =====================================
        // CHỈ CÒN FARM
        // =====================================
        else if (canAttackFarm)
        {
            SelectFarmTarget();
        }


        // =====================================
        // CHỈ CÒN PLAYER
        // =====================================
        else if (canAttackPlayer)
        {
            SelectPlayerTarget();
        }


        // Spawn xong sẽ đi tìm mục tiêu ngay
        currentState = EnemyState.Chase;
    }


    // =========================================
    // TARGET PLAYER
    // =========================================
    private void SelectPlayerTarget()
    {
        currentTargetType =
            TargetType.Player;


        currentTarget =
            player;


        Debug.Log(
            "[" + gameObject.name +
            "] Target = PLAYER"
        );
    }


    // =========================================
    // TARGET FARM
    // =========================================
    private void SelectFarmTarget()
    {
        currentTargetType =
            TargetType.Farm;


        // Chọn điểm gần Enemy nhất
        currentTarget =
            farmHealth.GetClosestTargetPoint(
                transform.position
            );


        Debug.Log(
            "[" + gameObject.name +
            "] Target = FARM"
        );
    }


    // =========================================
    // UPDATE
    // =========================================
    void Update()
    {
        if (isDead)
        {
            return;
        }


        // =====================================
        // FARM ĐÃ BỊ PHÁ HỦY
        // Enemy chuyển sang Player
        // =====================================
        if (currentTargetType == TargetType.Farm)
        {
            if (farmHealth == null ||
                farmHealth.IsDestroyed())
            {
                currentTarget = null;

                ChooseTarget();
            }
        }


        // =====================================
        // MẤT TARGET
        // =====================================
        if (currentTarget == null)
        {
            ChooseTarget();


            if (currentTarget == null)
            {
                PlayIdleAnimation();

                return;
            }
        }


        UpdateState();


        switch (currentState)
        {
            case EnemyState.Patrol:

                Patrol();

                break;


            case EnemyState.Chase:

                Chase();

                break;


            case EnemyState.Attack:

                Attack();

                break;
        }
    }


    // =========================================
    // UPDATE STATE
    // =========================================
    void UpdateState()
    {
        if (isAttacking)
        {
            return;
        }


        if (currentTarget == null)
        {
            return;
        }


        float distanceToTarget =
            Vector2.Distance(
                transform.position,
                currentTarget.position
            );


        switch (currentState)
        {
            // =================================
            // PATROL
            // =================================
            case EnemyState.Patrol:

                // Enemy Wave ưu tiên mục tiêu
                currentState =
                    EnemyState.Chase;

                break;


            // =================================
            // CHASE
            // =================================
            case EnemyState.Chase:

                if (distanceToTarget <=
                    attackRange)
                {
                    currentState =
                        EnemyState.Attack;
                }

                break;


            // =================================
            // ATTACK
            // =================================
            case EnemyState.Attack:

                if (distanceToTarget >
                    attackRange)
                {
                    currentState =
                        EnemyState.Chase;
                }

                break;
        }
    }


    // =========================================
    // DIRECTION
    // =========================================
    private void UpdateDirection()
    {
        if (spriteRenderer == null)
        {
            return;
        }


        if (Mathf.Abs(lastDirection.x) >
            Mathf.Abs(lastDirection.y))
        {
            if (lastDirection.x > 0)
            {
                currentDirection =
                    Direction.Right;


                spriteRenderer.flipX =
                    false;
            }
            else
            {
                currentDirection =
                    Direction.Left;


                spriteRenderer.flipX =
                    true;
            }
        }
        else
        {
            spriteRenderer.flipX =
                false;


            if (lastDirection.y > 0)
            {
                currentDirection =
                    Direction.Up;
            }
            else
            {
                currentDirection =
                    Direction.Down;
            }
        }
    }


    // =========================================
    // IDLE ANIMATION
    // =========================================
    public void PlayIdleAnimation()
    {
        PlayDirectionalAnimation(
            "Idle"
        );
    }


    // =========================================
    // MOVE ANIMATION
    // =========================================
    public void PlayMoveAnimation()
    {
        PlayDirectionalAnimation(
            "Move"
        );
    }


    // =========================================
    // ATTACK ANIMATION
    // =========================================
    public void PlayAttackAnimation()
    {
        PlayDirectionalAnimation(
            "Attack"
        );
    }


    // =========================================
    // DIE ANIMATION
    // =========================================
    public void PlayDieAnimation()
    {
        PlayDirectionalAnimation(
            "Die"
        );
    }


    // =========================================
    // DIRECTIONAL ANIMATION
    // =========================================
    private void PlayDirectionalAnimation(
        string baseAction
    )
    {
        if (animator == null)
        {
            return;
        }


        string dirSuffix = "";


        if (currentDirection ==
            Direction.Up)
        {
            dirSuffix = "up";
        }
        else if (
            currentDirection ==
            Direction.Right ||
            currentDirection ==
            Direction.Left)
        {
            dirSuffix = "right";
        }


        string targetState =
            baseAction + dirSuffix;


        // =====================================
        // Move / Moveup / Moveright
        // =====================================
        if (HasState(targetState))
        {
            PlayAnimation(
                targetState
            );

            return;
        }


        // =====================================
        // MoveUp / MoveRight
        // =====================================
        string pascalSuffix = "";


        if (currentDirection ==
            Direction.Up)
        {
            pascalSuffix = "Up";
        }
        else if (
            currentDirection ==
            Direction.Right ||
            currentDirection ==
            Direction.Left)
        {
            pascalSuffix = "Right";
        }


        if (HasState(
            baseAction +
            pascalSuffix))
        {
            PlayAnimation(
                baseAction +
                pascalSuffix
            );

            return;
        }


        // =====================================
        // FALLBACK
        // =====================================
        if (HasState(baseAction))
        {
            PlayAnimation(
                baseAction
            );

            return;
        }


        // =====================================
        // PREFIX ENEMY
        // =====================================
        if (enemyData != null &&
            !string.IsNullOrEmpty(
                enemyData.enemyName))
        {
            string prefix =
                enemyData.enemyName.Replace(
                    " ",
                    ""
                );


            if (HasState(
                prefix +
                targetState))
            {
                PlayAnimation(
                    prefix +
                    targetState
                );

                return;
            }


            if (HasState(
                prefix +
                baseAction +
                pascalSuffix))
            {
                PlayAnimation(
                    prefix +
                    baseAction +
                    pascalSuffix
                );

                return;
            }


            if (HasState(
                prefix +
                baseAction))
            {
                PlayAnimation(
                    prefix +
                    baseAction
                );

                return;
            }


            if (HasState(
                prefix +
                baseAction.ToLower()))
            {
                PlayAnimation(
                    prefix +
                    baseAction.ToLower()
                );

                return;
            }
        }


        // =====================================
        // SLIME FALLBACK
        // =====================================
        if (HasState(
            "Slime" +
            baseAction))
        {
            PlayAnimation(
                "Slime" +
                baseAction
            );

            return;
        }


        if (HasState(
            "Slime" +
            baseAction.ToLower()))
        {
            PlayAnimation(
                "Slime" +
                baseAction.ToLower()
            );

            return;
        }


        if (HasState("Mouse"))
        {
            PlayAnimation(
                "Mouse"
            );
        }
    }


    // =========================================
    // HAS STATE
    // =========================================
    private bool HasState(
        string stateName
    )
    {
        if (animator == null)
        {
            return false;
        }


        return animator.HasState(
            0,
            Animator.StringToHash(
                stateName
            )
        );
    }


    // =========================================
    // PLAY ANIMATION
    // =========================================
    private void PlayAnimation(
        string animationName
    )
    {
        if (currentAnimation ==
            animationName ||
            animator == null)
        {
            return;
        }


        currentAnimation =
            animationName;


        animator.Play(
            animationName
        );
    }


    // =========================================
    // PATROL
    // =========================================
    void Patrol()
    {
        if (isDead ||
            isAttacking)
        {
            return;
        }


        Vector2 dir =
            Vector2.zero;


        if (patrolAxis ==
            PatrolAxis.Horizontal)
        {
            dir =
                new Vector2(
                    direction,
                    0f
                );


            transform.Translate(
                dir *
                moveSpeed *
                Time.deltaTime
            );


            if (transform.position.x >=
                startPosition.x +
                moveDistance)
            {
                direction = -1;
            }
            else if (
                transform.position.x <=
                startPosition.x -
                moveDistance)
            {
                direction = 1;
            }


            lastDirection =
                new Vector2(
                    direction,
                    0f
                );
        }
        else
        {
            dir =
                new Vector2(
                    0f,
                    direction
                );


            transform.Translate(
                dir *
                moveSpeed *
                Time.deltaTime
            );


            if (transform.position.y >=
                startPosition.y +
                moveDistance)
            {
                direction = -1;
            }
            else if (
                transform.position.y <=
                startPosition.y -
                moveDistance)
            {
                direction = 1;
            }


            lastDirection =
                new Vector2(
                    0f,
                    direction
                );
        }


        UpdateDirection();

        PlayMoveAnimation();
    }


    // =========================================
    // CHASE TARGET
    // =========================================
    void Chase()
    {
        if (isDead ||
            isAttacking ||
            currentTarget == null)
        {
            return;
        }


        Vector2 dirToTarget =
            (Vector2)currentTarget.position -
            (Vector2)transform.position;


        if (dirToTarget !=
            Vector2.zero)
        {
            lastDirection =
                dirToTarget.normalized;


            UpdateDirection();
        }


        transform.position =
            Vector2.MoveTowards(
                transform.position,
                currentTarget.position,
                moveSpeed *
                Time.deltaTime
            );


        PlayMoveAnimation();
    }


    // =========================================
    // ATTACK TARGET
    // =========================================
    void Attack()
    {
        if (isDead)
        {
            return;
        }


        if (currentTarget == null)
        {
            ChooseTarget();

            return;
        }


        // =====================================
        // QUAY VỀ TARGET
        // =====================================
        Vector2 dirToTarget =
            (Vector2)currentTarget.position -
            (Vector2)transform.position;


        if (dirToTarget !=
            Vector2.zero)
        {
            lastDirection =
                dirToTarget.normalized;


            UpdateDirection();
        }


        // =====================================
        // COOLDOWN
        // =====================================
        if (Time.time <
            nextAttack)
        {
            if (!isAttacking)
            {
                PlayIdleAnimation();
            }


            return;
        }


        nextAttack =
            Time.time +
            attackCoolDown;


        StartCoroutine(
            PerformAttackCoroutine()
        );
    }


    // =========================================
    // PERFORM ATTACK
    // =========================================
    private IEnumerator PerformAttackCoroutine()
    {
        isAttacking = true;


        PlayAttackAnimation();


        // =====================================
        // DAMAGE TARGET
        // =====================================
        DealDamageToCurrentTarget();


        yield return new WaitForSeconds(
            0.4f
        );


        isAttacking = false;


        if (currentState ==
            EnemyState.Attack &&
            !isDead)
        {
            PlayIdleAnimation();
        }
    }


    // =========================================
    // DAMAGE TARGET
    // =========================================
    private void DealDamageToCurrentTarget()
    {
        int damage =
            enemyData != null
            ? enemyData.damage
            : 1;


        // =====================================
        // PLAYER
        // =====================================
        if (currentTargetType ==
            TargetType.Player)
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.ChangeHp(
                    -damage
                );


                Debug.Log(
                    "[" +
                    gameObject.name +
                    "] gây " +
                    damage +
                    " damage cho PLAYER"
                );
            }
        }


        // =====================================
        // FARM
        // =====================================
        else if (
            currentTargetType ==
            TargetType.Farm)
        {
            if (farmHealth != null &&
                !farmHealth.IsDestroyed())
            {
                farmHealth.TakeDamage(
                    damage
                );


                Debug.Log(
                    "[" +
                    gameObject.name +
                    "] gây " +
                    damage +
                    " damage cho FARM"
                );
            }
        }
    }


    // =========================================
    // TAKE DAMAGE
    // =========================================
    public void TakeDamage(
        int amount
    )
    {
        if (isDead)
        {
            return;
        }


        currentHp -= amount;


        Debug.Log(
            $"[{(enemyData != null ? enemyData.enemyName : gameObject.name)}]" +
            $" nhận {amount} damage | HP còn: {currentHp}"
        );


        if (enemyHealth != null)
        {
            enemyHealth.UpdateHP(
                currentHp
            );
        }


        if (currentHp <= 0)
        {
            Die();
        }
    }


    // =========================================
    // DIE
    // =========================================
    void Die()
    {
        if (isDead)
        {
            return;
        }


        isDead = true;

        isAttacking = false;


        Debug.Log(
            $"[{(enemyData != null ? enemyData.enemyName : gameObject.name)}]" +
            " đã bị tiêu diệt!"
        );


        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddScore(
                10
            );


            GameManager.Instance
                .TriggerFirstEnemyDialogue();
        }


        // =====================================
        // TẮT COLLIDER
        // =====================================
        Collider2D col =
            GetComponent<Collider2D>();


        if (col != null)
        {
            col.enabled = false;
        }


        // =====================================
        // TẮT THANH HP
        // =====================================
        if (enemyHealth != null)
        {
            enemyHealth.gameObject
                .SetActive(false);
        }


        // =====================================
        // DIE ANIMATION
        // =====================================
        PlayDieAnimation();


        // =====================================
        // DESTROY
        // =====================================
        Destroy(
            gameObject,
            0.8f
        );
    }
}