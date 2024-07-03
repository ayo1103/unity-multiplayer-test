using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class Player : NetworkBehaviour
{
    [SerializeField] float movementSpeedBase = 5;

    private Animator animator;
    private Rigidbody2D rb;
    private float movementSpeedMultiplier;
    private Vector2 currentMoveDirection;
    
    // public int playerScore;
    public NetworkVariable<int> playerScore = new();

    public override void OnNetworkSpawn()
    {
        if (!IsOwner)
        {
            enabled = false;
            return;
        }

        playerScore.OnValueChanged += FindObjectOfType<PlayerUI>().UpdateScoreUI;
    }

    private void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        Move();
        Attack();
    }

    private void Move()
    {
        var joystickInput = new Vector2(Joystick.Instance.Horizontal, Joystick.Instance.Vertical);
        var keyboardHorizontal = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        var movementDirection = joystickInput == Vector2.zero
            ? keyboardHorizontal
            : joystickInput;

        Vector2 moveVector = movementDirection.normalized * (movementSpeedBase * movementSpeedMultiplier);

        animator.SetFloat("Speed", moveVector.magnitude);
        rb.velocity = moveVector;

        if (moveVector != Vector2.zero)
        {
            currentMoveDirection = new Vector2(moveVector.normalized.x, moveVector.normalized.y);
            animator.SetFloat("Horizontal", moveVector.normalized.x);
            animator.SetFloat("Vertical", moveVector.normalized.y);
        }
    }

    private bool _canAttack = true;

    private void Attack()
    {
        if (!Joystick.Instance.HasInput && Input.GetMouseButton(0))
        {
            movementSpeedMultiplier = 0.5f;
            animator.SetFloat("Attack", 1);

            if (_canAttack)
            {
                AttackServerRPC(currentMoveDirection);
                StartCoroutine(AttackCooldown());
            }
        }
        else
        {
            animator.SetFloat("Attack", 0);
            movementSpeedMultiplier = 1f;
        }
    }

    private IEnumerator AttackCooldown()
    {
        _canAttack = false;
        yield return new WaitForSeconds(0.15f);
        _canAttack = true;
    }

    [ServerRpc]
    private void AttackServerRPC(Vector2 direction)
    {
        RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, 1f, direction, 0, 1 << 6);

        if (hits.Length > 0)
        {
            hits[0].transform.GetComponent<HealthSystem>().OnDamageDealt(50);
            if (hits[0].transform.GetComponent<HealthSystem>().health < 0)
            {
                playerScore.Value++;
            }
        }
    }
}