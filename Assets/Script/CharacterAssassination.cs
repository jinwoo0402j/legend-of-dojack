using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CharacterAssassination : MonoBehaviour
{
    private Animator animator; // 애니메이터
    private Rigidbody2D rb; // 리지드바디 추가
    public float speed = 5f; //스피드
    private float moveHorizontal; // 수평 이동
    public GameObject CharacterController; // 변수선언

    

    #region 레이더 관련
    public float assassinationRange = 1f; // 판정 범위의 변수 
    public LayerMask enemyLayers; // 적을 판단하는 레이더
    #endregion
    #region 데미지 수치 관련
    public float assassinationDamageRatio = 0.8f; // 암살 시 데미지 비율
    public float stunDuration = 3f; // 스턴 시간 
    #endregion
  

    private void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();      
    }
    private void Update()
    {   
        processInput_attack();
        processInput_specialattack();
    }

    /// <summary>스턴을 처리한다..</summary>
    //private IEnumerator StunEnemy(Collider2D enemy)
    //{
    //    enemy.gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
    //    enemy.gameObject.GetComponent<EnemyMovement>().enabled = false;
    //    enemy.gameObject.GetComponent<EnemyAttack>().enabled = false;
    //    enemy.gameObject.GetComponent<Animator>().SetTrigger("stunned");
    //    yield return new WaitForSeconds(stunDuration);
    //    enemy.gameObject.GetComponent<EnemyMovement>().enabled = true;
    //    enemy.gameObject.GetComponent<EnemyAttack>().enabled = true;
    //}
   
    /// <summary>공격버튼의 입력을 처리한다..</summary>
    private void processInput_attack()
    {
        //fire2 버튼울 누르면
        if (Input.GetButtonDown("Fire2"))
        {
            Debug.Log("fire2");
            GetComponent<Animator>().SetTrigger("Attack"); // 애니메이터에서 attack 상태로 변경

            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, assassinationRange, enemyLayers); // 콜라이더를 생성해서 위치와 범위안에 해당된 레이어에 있는 객체를 찾는다.
            foreach (Collider2D enemy in hitEnemies) // hitenemies 를 모두 enemy안에 할당하여 중복 처리할 수 있게 한다.
            {
                Vector2 direction = enemy.transform.position - transform.position; // enemy와 pc의 거리와 방향을 나타냄.
                RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, assassinationRange, enemyLayers); // 레이를 싸숴 맞춘다. 충돌하는 건 에너미들
                
                if (hit.collider != null && hit.collider.gameObject == enemy.gameObject) // 콜라이더에 무언가 hit 되고, 해당 콜라이더가 에너미라면. 
                {
                    EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth>(); // 상대의 체력은 상대 체력 코드에서 가져온다.

                    float damage = enemyHealth.maxHealth * assassinationDamageRatio; //데미지를 계산해서
                    enemyHealth.TakeDamage((int)damage); // 데미지를 준다.

                    //StartCoroutine(StunEnemy(enemy)); // 적에게 스턴을 준다.
                    break; // 끝
                }
            }
        }
    }

    private void processInput_specialattack()
    {
        //fire2 버튼울 누르면
        if (Input.GetButtonDown("Fire1"))
        {
            Debug.Log("fire1");
            GetComponent<Animator>().SetTrigger("SpecialAttack"); // 애니메이터에서 attack 상태로 변경

            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, assassinationRange, enemyLayers); // 콜라이더를 생성해서 위치와 범위안에 해당된 레이어에 있는 객체를 찾는다.
            foreach (Collider2D enemy in hitEnemies) // hitenemies 를 모두 enemy안에 할당하여 중복 처리할 수 있게 한다.
            {
                Vector2 direction = enemy.transform.position - transform.position; // enemy와 pc의 거리와 방향을 나타냄.
                RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, assassinationRange, enemyLayers); // 레이를 싸숴 맞춘다. 충돌하는 건 에너미들

                if (hit.collider != null && hit.collider.gameObject == enemy.gameObject) // 콜라이더에 무언가 hit 되고, 해당 콜라이더가 에너미라면. 
                {
                    EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth>(); // 상대의 체력은 상대 체력 코드에서 가져온다.

                    float damage = enemyHealth.maxHealth * assassinationDamageRatio; //데미지를 계산해서
                    enemyHealth.TakeDamage((int)damage); // 데미지를 준다.

                    //StartCoroutine(StunEnemy(enemy)); // 적에게 스턴을 준다.
                    break; // 끝
                }
            }
          
            CharacterController.GetComponent<CharacterController>().Dash();
           

        }
    }

    /// <summary>기즈모를 그려 눈에 보이게한다...</summary>
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, assassinationRange);
    }


  

}
