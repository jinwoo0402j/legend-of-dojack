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

    #region �Է�

    private float moveHorizontal; // ���� �̵�
    private bool isWalking; //�ȴ� ��

    #endregion

    #region ����
    
    public float speed = 5f; //���ǵ�
    public float jumpForce = 10f; //������
    private bool isGrounded; //���� �پ��ִ� ��

    #endregion

    #region ����

    public int extraJumps = 1; //�ִ� ������ȸ
    private int jumpsLeft; //���� ���� ���� Ƚ��
    public float checkRadius = 0.5f; //���� ���� üũ
    private bool facingRight = true; //�Ĵٺ��� �ִ� ����

    #endregion

    private void Start()
    {
        //������Ʈ�� �޾ƿ���. 
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();

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
        processInput();

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
        processMovement();
    }

    /// <summary>�Է��� ó���Ѵ�.</summary>
    private void processInput()
    {
        // �����̽��� ������ �� ���� Ƚ���� ���������� ������ ����, ���� ���� Ƚ�� ����.
        if (Input.GetKeyDown(KeyCode.Space) && jumpsLeft > 0)
        {
            rb.velocity = Vector2.up * jumpForce;
            jumpsLeft--;
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
            rb.velocity = new Vector2(moveHorizontal * speed, rb.velocity.y);
        }

        //// �߷��� �����Ͽ� ĳ���� �������� �ε巴�� �ϴ� �ڵ�.
        if (rb.velocity.y < 0)
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * Time.deltaTime;
        }
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
