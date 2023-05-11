using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class CharacterController : MonoBehaviour
{
    private Rigidbody2D rb; // ������ٵ� �߰�
    public Transform feet; // ���� ��ġ
    private Animator animator; //�ִϸ�����
    public LayerMask groundLayer; //� ���̷��� üũ�� ���ΰ�.
    private CharacterController characterController; //ĳ���� ��Ʈ�ѷ�
    [SerializeField] private TrailRenderer tr;

    #region �Է�

    private float moveHorizontal; // ���� �̵�
    private bool isWalking; //�ȴ� ��

    #endregion

    #region ����

    public float speed = 5f; //���ǵ�
    /*public float dashSpeed = 15f; // �뽬 ���ǵ�
    public float dashDuration = 0.5f; //�뽬 ���� �ð�
    private bool isDashing = false;*/
    public float jumpForce = 10f; //������
    private bool isGrounded; //���� �پ��ִ� ��

    #endregion

    #region ����

    public int extraJumps = 1; //�ִ� ������ȸ
    private int jumpsLeft; //���� ���� ���� Ƚ��
    public float checkRadius = 0.5f; //���� ���� üũ
    private bool facingRight = true; //�Ĵٺ��� �ִ� ����

    #endregion

    #region �뽬 ����

    private bool canDash = true;
    private bool isDashing;
    private float dashPower = 20f;
    private float dashtime = 0.3f;
    private float dashendtime = 0.20f;
    private float dashcooldown = 0.1f;


    #endregion



    private void Start()
    {
        //������Ʈ�� �޾ƿ���. 
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();
        tr = GetComponent<TrailRenderer>();

        //���� ���� Ƚ�� �ʱ�ȭ
        jumpsLeft = extraJumps;

        //���� ��ġ�� ã�´�.
        feet = transform.Find("Feet");
        if (feet == null)
        {
            //���� ��ã������.
            Debug.LogError("Feet transform not found!");
        }
    }

    private void Update()
    {
       if (isDashing)
        {
            return;
        }
        processInput();
        processInput2();

        isGrounded = Physics2D.OverlapCircle(feet.position, checkRadius, groundLayer);

        //���� ���� ��, ���� ���� Ƚ���� �ִ� Ƚ�� ��ŭ ä���.
        if (isGround())
        {
            jumpsLeft = extraJumps;
        }

        // ���� ���� ������Ʈ���� Animator �� ã�Ƽ� animator ������ �Ҵ��Ѵ�. 
        Animator animator = GetComponent<Animator>();

        // �ִϸ����� ������ ������ �� ���� ��ȯ�Ѵ�.
        if (animator != null)
        {
            //isWalking �� true�� �Ǹ� walk �Ű������� true�� �ٲ۴�. �̴� animator ������Ʈ���� walk�� �����ϰ� �����.
            animator.SetBool("walk", isWalking);
        }
    }

    private void FixedUpdate()
    {
        if (isDashing)
        {
            return;
        }

        processMovement();
    }

    /// <summary>�Է��� ó���Ѵ�.</summary>
    private void processInput()
    {
        // �����̽��� ������ �� ���� Ƚ���� ���������� ������ ����, ���� ���� Ƚ�� ����.
        if (Input.GetKeyDown(KeyCode.C) && jumpsLeft > 0)
        {
            
            rb.velocity = Vector2.up * jumpForce;
            jumpsLeft--;
        }

        // moveHorizontal ����, �̴� ���� �� �Է°� �Ҵ�.
        moveHorizontal = Input.GetAxis("Horizontal");

        // moveHorizontal , ���������� �����̴� ��ġ�� 0�� �ƴϸ� �ȴ������� �Ǵ��Ѵ�.
        isWalking = (moveHorizontal != 0);
    }

    private void processInput2()
    {
        // �����̽��� ������ �� ���� Ƚ���� ���������� ������ ����, ���� ���� Ƚ�� ����.
        if (Input.GetKeyDown(KeyCode.Z) && canDash)
        {
            StartCoroutine(Dash());
           
        }

        // moveHorizontal ����, �̴� ���� �� �Է°� �Ҵ�.
        moveHorizontal = Input.GetAxis("Horizontal");

        // moveHorizontal , ���������� �����̴� ��ġ�� 0�� �ƴϸ� �ȴ������� �Ǵ��Ѵ�.
        isWalking = (moveHorizontal != 0);
    }


    /// <summary>�������� ó���Ѵ�.</summary>
    private void processMovement()
    {
        // �ȴ� ���� ��
        if (isWalking)
        {
            // ���������� ���鼭 �ȴ� ���� �� �ø��� ����.
            if (moveHorizontal > 0 && facingRight)
            {
                flipModel();
            }
            // ĳ���Ͱ� �������� �����̰� ���� �������� �ٶ󺸴� �� �ƴҰ��.
            else if (moveHorizontal < 0 && !facingRight)
            {
                flipModel();
            }

            // ������ �ٵ��� ���ν�Ƽ �Ӽ��� ����. moveHorizontal �� ���� * speed�� ���ϰ� 
            if (!isDashing)
            {
                rb.velocity = new Vector2(moveHorizontal * speed, rb.velocity.y);
            }

        }

        //// �߷��� �����Ͽ� ĳ���� �������� �ε巴�� �ϴ� �ڵ�.
        if (rb.velocity.y < 0)
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * Time.deltaTime;
        }
    }



    public IEnumerator Dash()
    {
        canDash = false; // �뽬�� �Ұ��� ����. 
        isDashing = true; // ���� �뽬����
        float originalGravity = rb.gravityScale; // �뽬 ���� �߷��� �������� ����
        rb.gravityScale = 0f; // �뽬 ���� �߷��� �������� ����
        float originalVelocity = rb.velocity.x; // �뽬 ������ �� ĳ������ ���� �ӵ��� ������
        float originalTimeScale = Time.timeScale; // ������ �ð� ������ ���� ������

        if (transform.right.x > 0f)
        {
            // right
            rb.velocity = new Vector2(-dashPower, 0f);
        }
        else
        {
            // left
            rb.velocity = new Vector2(dashPower, 0f);
        }

        tr.emitting = true; // Ʈ���� ��
        
        yield return new WaitForSeconds(dashtime); // �뽬Ÿ�� ���� ����.

       
        
        Time.timeScale = 0.4f; // �뽬 ���� ���� �ð��� ������ ��
        
        yield return new WaitForSeconds(dashendtime); // �뽬Ÿ�� ���� ����.
        rb.velocity = new Vector2(originalVelocity, 0f); // �뽬�� ������ ĳ������ ���� �ӵ��� ���ư��� ��
        tr.emitting = false; // Ʈ���� �޽�
        rb.gravityScale = originalGravity;
        isDashing = false; // ���� �뽬���� �ƴ�
        Time.timeScale = originalTimeScale; // ���� �ð��� �ٽ� ������� ������
        yield return new WaitForSeconds(dashcooldown); // �뽬 ��ٿ� ����
        canDash = true; // ��� �ڵ� ���� ���� �ٽ� �뽬 ���ɻ��·� ��ȯ
    }









    /// <summary>
    /// ���� �ִ� �Ǵ��� �߿��� ��ü üũ�� �����Ͽ� �׶���
    /// ���̾�� ������ ������Ʈ�� üũ �Ǿ������� �������� �Ѵ�. 
    /// </summary>
    /// <returns>���� ������� true�� ��ȯ�մϴ�.</returns>
    private bool isGround()
    {
        return Physics2D.OverlapCircle(feet.position, checkRadius, groundLayer);
    }

    // �ø��� �����ϴ� �ڵ�
    private void flipModel()
    {
        facingRight = !facingRight;
        transform.Rotate(0f, 180f, 0f);
    }

    // ���� ��ġ���� , üũ ������ �׸���.
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(feet.position, checkRadius);
    }

}
