using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private Vector2 moveVector = Vector2.zero;
    private Vector2 rotateVector = Vector2.zero;
    private Rigidbody2D rb = null;

    private bool canDash = true;
    private bool isDashing;

    [Header("Dash")]
    [SerializeField] private float dashPower = 30f;
    [SerializeField] private float dashTime = 0.3f;
    [SerializeField] private float dashCooldown = 0f;


    [Header("Player")]
    [SerializeField] private float moveSpeed = 10f;
    [Range(0.1f, 2f)] [SerializeField] private float fireRate = 0.5f;
    [SerializeField] private bool isPlayer2;

    [Header("Game Objects")]
    [SerializeField] private GameObject Hand;
    [SerializeField] private GameObject HandSprite;
    [SerializeField] private GameObject Dots;
    [SerializeField] private GameObject BulletPrefab;
    [SerializeField] private Transform firingPoint1;
    [SerializeField] private Transform firingPoint2;
    [SerializeField] private Transform firingPoint3;
    [SerializeField] private TrailRenderer tr; 

    
    private Animator animator;

    private bool running = false;

    private Health health;

    private SpriteRenderer spr;
    private SpriteRenderer dotsRenderer;
    private SpriteRenderer handRenderer;

    private float fireTimer;
    private bool attacking;

    private void FixedUpdate()
    {
        if (isDashing){
            animator.CrossFade("Dodge", 0, 0);
            return;
        }


        if (moveVector == Vector2.zero) {
            running = false;
        } else {
            running = true;
        }
        

        if (isPlayer2) {
            spr.flipX = rotateVector.x > 0f;
            handRenderer.flipY = rotateVector.x > 0f;
        } else {
            spr.flipX = rotateVector.x < 0f;
            handRenderer.flipY = rotateVector.x < 0f;
        }

        rb.velocity = moveVector * moveSpeed;
        var angle = Mathf.Atan2(rotateVector.y, rotateVector.x) * Mathf.Rad2Deg;


        if (isPlayer2) {
            angle = Mathf.Atan2(- rotateVector.y, - rotateVector.x) * Mathf.Rad2Deg;
        }

        Hand.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);


        if (attacking && fireTimer <= 0f) {
            Shoot();
            fireTimer = fireRate;
        } else {
            fireTimer -= Time.deltaTime;
        }


        var state = GetState();
        Debug.Log(state);
        animator.CrossFade(state, 0, 0);
    }

    private void Shoot()
    {
       Instantiate(BulletPrefab, firingPoint1.position, firingPoint1.rotation);
       Instantiate(BulletPrefab, firingPoint2.position, firingPoint2.rotation);
       Instantiate(BulletPrefab, firingPoint3.position, firingPoint3.rotation);
    }

    private IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;
        rb.velocity = moveVector * dashPower ;
        tr.emitting = true;
        Physics2D.IgnoreLayerCollision(10, 11, true);
        yield return new WaitForSeconds(dashTime);
        dotsRenderer.color = Color.white;
        tr.emitting = false;
        isDashing = false;
        Physics2D.IgnoreLayerCollision(10, 11, false);
        yield return new WaitForSeconds(dashCooldown);
        dotsRenderer.color = Color.clear;
        canDash = true;
    }

    private void Awake() 
    {
        health = GetComponent<Health>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        spr = GetComponent<SpriteRenderer>();
        handRenderer = HandSprite.GetComponent<SpriteRenderer>();

        dotsRenderer = Dots.GetComponent<SpriteRenderer>();
        dotsRenderer.color = Color.clear;
    }

    public void AttackPeformed(InputAction.CallbackContext context)
    {
        if (context.phase is InputActionPhase.Performed) {
            attacking = true;
        } else {
            attacking = false;
        }
    }

    public void OnMovementPeformed(InputAction.CallbackContext value)
    {
        moveVector = value.ReadValue<Vector2>();
    }

    public void OnRotationPeformed(InputAction.CallbackContext value)
    {
        if (value.phase is InputActionPhase.Performed) {
            rotateVector = value.ReadValue<Vector2>();
        }

    }

    public void OnDashPeformed(InputAction.CallbackContext context) 
    {
        if (canDash)
        {   
            StartCoroutine(Dash());
        }
    }

    private string GetState()
    {
        if(health.isHurt) return "Hurt";
        if (running) return "Running";

        return "Idle";
    }

}
