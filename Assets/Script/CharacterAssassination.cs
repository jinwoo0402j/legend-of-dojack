using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAssassination : MonoBehaviour
{
    public float assassinationRange = 1f;
    public LayerMask enemyLayers;
    public float stunDuration = 3f;
    public float assassinationDamageRatio = 0.8f;

    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
        
    }

    private void Update()
    {
         if (Input.GetButtonDown("Fire2"))
        {
            Debug.Log("fire2");
            GetComponent<Animator>().SetTrigger("Attack");
            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, assassinationRange, enemyLayers);
            foreach (Collider2D enemy in hitEnemies)
            {
                Vector2 direction = enemy.transform.position - transform.position;
                RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, assassinationRange, enemyLayers);
                if (hit.collider != null && hit.collider.gameObject == enemy.gameObject)
                {
                    EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth>();
                    float damage = enemyHealth.maxHealth * assassinationDamageRatio;
                    enemyHealth.TakeDamage((int)damage);
                    StartCoroutine(StunEnemy(enemy));
                    break;
                }
            }
        }
    }

    private IEnumerator StunEnemy(Collider2D enemy)
    {
        enemy.gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        //enemy.gameObject.GetComponent<EnemyMovement>().enabled = false;
        //enemy.gameObject.GetComponent<EnemyAttack>().enabled = false;
        enemy.gameObject.GetComponent<Animator>().SetTrigger("stunned");
        yield return new WaitForSeconds(stunDuration);
        //enemy.gameObject.GetComponent<EnemyMovement>().enabled = true;
        //enemy.gameObject.GetComponent<EnemyAttack>().enabled = true;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, assassinationRange);
    }
}
