using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CharacterAssassination : MonoBehaviour
{
    private Animator animator; // �ִϸ�����
    private Rigidbody2D rb; // ������ٵ� �߰�
    public float speed = 5f; //���ǵ�
    private float moveHorizontal; // ���� �̵�
    public GameObject CharacterController; // ��������

    

    #region ���̴� ����
    public float assassinationRange = 1f; // ���� ������ ���� 
    public LayerMask enemyLayers; // ���� �Ǵ��ϴ� ���̴�
    #endregion
    #region ������ ��ġ ����
    public float assassinationDamageRatio = 0.8f; // �ϻ� �� ������ ����
    public float stunDuration = 3f; // ���� �ð� 
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

    /// <summary>������ ó���Ѵ�..</summary>
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
   
    /// <summary>���ݹ�ư�� �Է��� ó���Ѵ�..</summary>
    private void processInput_attack()
    {
        //fire2 ��ư�� ������
        if (Input.GetButtonDown("Fire2"))
        {
            Debug.Log("fire2");
            GetComponent<Animator>().SetTrigger("Attack"); // �ִϸ����Ϳ��� attack ���·� ����

            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, assassinationRange, enemyLayers); // �ݶ��̴��� �����ؼ� ��ġ�� �����ȿ� �ش�� ���̾ �ִ� ��ü�� ã�´�.
            foreach (Collider2D enemy in hitEnemies) // hitenemies �� ��� enemy�ȿ� �Ҵ��Ͽ� �ߺ� ó���� �� �ְ� �Ѵ�.
            {
                Vector2 direction = enemy.transform.position - transform.position; // enemy�� pc�� �Ÿ��� ������ ��Ÿ��.
                RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, assassinationRange, enemyLayers); // ���̸� �ν� �����. �浹�ϴ� �� ���ʹ̵�
                
                if (hit.collider != null && hit.collider.gameObject == enemy.gameObject) // �ݶ��̴��� ���� hit �ǰ�, �ش� �ݶ��̴��� ���ʹ̶��. 
                {
                    EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth>(); // ����� ü���� ��� ü�� �ڵ忡�� �����´�.

                    float damage = enemyHealth.maxHealth * assassinationDamageRatio; //�������� ����ؼ�
                    enemyHealth.TakeDamage((int)damage); // �������� �ش�.

                    //StartCoroutine(StunEnemy(enemy)); // ������ ������ �ش�.
                    break; // ��
                }
            }
        }
    }

    private void processInput_specialattack()
    {
        //fire2 ��ư�� ������
        if (Input.GetButtonDown("Fire1"))
        {
            Debug.Log("fire1");
            GetComponent<Animator>().SetTrigger("SpecialAttack"); // �ִϸ����Ϳ��� attack ���·� ����

            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, assassinationRange, enemyLayers); // �ݶ��̴��� �����ؼ� ��ġ�� �����ȿ� �ش�� ���̾ �ִ� ��ü�� ã�´�.
            foreach (Collider2D enemy in hitEnemies) // hitenemies �� ��� enemy�ȿ� �Ҵ��Ͽ� �ߺ� ó���� �� �ְ� �Ѵ�.
            {
                Vector2 direction = enemy.transform.position - transform.position; // enemy�� pc�� �Ÿ��� ������ ��Ÿ��.
                RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, assassinationRange, enemyLayers); // ���̸� �ν� �����. �浹�ϴ� �� ���ʹ̵�

                if (hit.collider != null && hit.collider.gameObject == enemy.gameObject) // �ݶ��̴��� ���� hit �ǰ�, �ش� �ݶ��̴��� ���ʹ̶��. 
                {
                    EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth>(); // ����� ü���� ��� ü�� �ڵ忡�� �����´�.

                    float damage = enemyHealth.maxHealth * assassinationDamageRatio; //�������� ����ؼ�
                    enemyHealth.TakeDamage((int)damage); // �������� �ش�.

                    //StartCoroutine(StunEnemy(enemy)); // ������ ������ �ش�.
                    break; // ��
                }
            }
          
            CharacterController.GetComponent<CharacterController>().Dash();
           

        }
    }

    /// <summary>����� �׷� ���� ���̰��Ѵ�...</summary>
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, assassinationRange);
    }


  

}
