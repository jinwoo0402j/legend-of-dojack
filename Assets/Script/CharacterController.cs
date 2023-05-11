using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class CharacterController : MonoBehaviour
{
    private Rigidbody2D rb; // 리지드바디 추가
    public Transform feet; // 발의 위치
    private Animator animator; //애니메이터
    public LayerMask groundLayer; //어떤 레이러를 체크할 것인가.
    private CharacterController characterController; //캐릭터 컨트롤러
    [SerializeField] private TrailRenderer tr;

    #region 입력

    private float moveHorizontal; // 수평 이동
    private bool isWalking; //걷는 중

    #endregion

    #region 물리

    public float speed = 5f; //스피드
    /*public float dashSpeed = 15f; // 대쉬 스피드
    public float dashDuration = 0.5f; //대쉬 지속 시간
    private bool isDashing = false;*/
    public float jumpForce = 10f; //점프량
    private bool isGrounded; //땅에 붙어있는 중

    #endregion

    #region 상태

    public int extraJumps = 1; //최대 점프기회
    private int jumpsLeft; //현재 남은 점프 횟수
    public float checkRadius = 0.5f; //공격 범위 체크
    private bool facingRight = true; //쳐다보고 있는 방향

    #endregion

    #region 대쉬 관련

    private bool canDash = true;
    private bool isDashing;
    private float dashPower = 20f;
    private float dashtime = 0.3f;
    private float dashendtime = 0.20f;
    private float dashcooldown = 0.1f;


    #endregion



    private void Start()
    {
        //컴포넌트들 받아오기. 
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();
        tr = GetComponent<TrailRenderer>();

        //현재 점프 횟수 초기화
        jumpsLeft = extraJumps;

        //발의 위치를 찾는다.
        feet = transform.Find("Feet");
        if (feet == null)
        {
            //발이 안찾아진다.
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

        //땅에 있을 때, 점프 가능 횟수를 최대 횟수 만큼 채운다.
        if (isGround())
        {
            jumpsLeft = extraJumps;
        }

        // 현재 게임 오브젝트에서 Animator 를 찾아서 animator 변수에 할당한다. 
        Animator animator = GetComponent<Animator>();

        // 애니메이터 변수가 없으면 널 값을 반환한다.
        if (animator != null)
        {
            //isWalking 이 true가 되면 walk 매개변수도 true로 바꾼다. 이는 animator 컴포넌트에서 walk를 실행하게 만든다.
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

    /// <summary>입력을 처리한다.</summary>
    private void processInput()
    {
        // 스페이스바 눌렀을 때 점프 횟수가 남아있으면 점프를 실행, 이후 점프 횟수 차감.
        if (Input.GetKeyDown(KeyCode.C) && jumpsLeft > 0)
        {
            
            rb.velocity = Vector2.up * jumpForce;
            jumpsLeft--;
        }

        // moveHorizontal 선언, 이는 수평 축 입력값 할당.
        moveHorizontal = Input.GetAxis("Horizontal");

        // moveHorizontal , 수평축으로 움직이는 수치가 0이 아니면 걷는중으로 판단한다.
        isWalking = (moveHorizontal != 0);
    }

    private void processInput2()
    {
        // 스페이스바 눌렀을 때 점프 횟수가 남아있으면 점프를 실행, 이후 점프 횟수 차감.
        if (Input.GetKeyDown(KeyCode.Z) && canDash)
        {
            StartCoroutine(Dash());
           
        }

        // moveHorizontal 선언, 이는 수평 축 입력값 할당.
        moveHorizontal = Input.GetAxis("Horizontal");

        // moveHorizontal , 수평축으로 움직이는 수치가 0이 아니면 걷는중으로 판단한다.
        isWalking = (moveHorizontal != 0);
    }


    /// <summary>움직임을 처리한다.</summary>
    private void processMovement()
    {
        // 걷는 중일 때
        if (isWalking)
        {
            // 오른쪽으로 가면서 걷는 중일 때 플립을 실행.
            if (moveHorizontal > 0 && facingRight)
            {
                flipModel();
            }
            // 캐릭터가 왼쪽으로 움직이고 현재 오른쪽을 바라보는 게 아닐경우.
            else if (moveHorizontal < 0 && !facingRight)
            {
                flipModel();
            }

            // 리지드 바디의 벨로시티 속성을 변경. moveHorizontal 이 값에 * speed를 곱하고 
            if (!isDashing)
            {
                rb.velocity = new Vector2(moveHorizontal * speed, rb.velocity.y);
            }

        }

        //// 중력을 보완하여 캐릭터 움직임을 부드럽게 하는 코드.
        if (rb.velocity.y < 0)
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * Time.deltaTime;
        }
    }



    public IEnumerator Dash()
    {
        canDash = false; // 대쉬는 불가능 상태. 
        isDashing = true; // 현재 대쉬중임
        float originalGravity = rb.gravityScale; // 대쉬 동안 중력을 적용하지 않음
        rb.gravityScale = 0f; // 대쉬 동안 중력을 적용하지 않음
        float originalVelocity = rb.velocity.x; // 대쉬 시작할 때 캐릭터의 이전 속도를 저장함
        float originalTimeScale = Time.timeScale; // 원래의 시간 스케일 값을 저장함

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

        tr.emitting = true; // 트레일 온
        
        yield return new WaitForSeconds(dashtime); // 대쉬타임 동안 진행.

       
        
        Time.timeScale = 0.4f; // 대쉬 동안 게임 시간을 느리게 함
        
        yield return new WaitForSeconds(dashendtime); // 대쉬타임 동안 진행.
        rb.velocity = new Vector2(originalVelocity, 0f); // 대쉬가 끝나면 캐릭터의 이전 속도로 돌아가게 함
        tr.emitting = false; // 트레일 펄스
        rb.gravityScale = originalGravity;
        isDashing = false; // 이제 대쉬중임 아님
        Time.timeScale = originalTimeScale; // 게임 시간을 다시 원래대로 복구함
        yield return new WaitForSeconds(dashcooldown); // 대쉬 쿨다운 진행
        canDash = true; // 모든 코드 실행 이후 다시 대쉬 가능상태로 전환
    }









    /// <summary>
    /// 땅에 있는 판단은 발에서 구체 체크를 생성하여 그라운드
    /// 레이어로 지정된 오브젝트에 체크 되었는지를 기준으로 한다. 
    /// </summary>
    /// <returns>땅에 닿았으면 true를 반환합니다.</returns>
    private bool isGround()
    {
        return Physics2D.OverlapCircle(feet.position, checkRadius, groundLayer);
    }

    // 플립을 실행하는 코드
    private void flipModel()
    {
        facingRight = !facingRight;
        transform.Rotate(0f, 180f, 0f);
    }

    // 발의 위치에서 , 체크 범위를 그린다.
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(feet.position, checkRadius);
    }

}
