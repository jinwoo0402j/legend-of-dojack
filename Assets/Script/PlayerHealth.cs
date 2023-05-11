using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;

    //public GameObject gameOverUI;
    public GameObject player;

    private bool isGameOver = false;

    void Start()
    {
        currentHealth = maxHealth;
        //gameOverUI.SetActive(false);
    }

    public void TakeDamage(int damageAmount)
    {
        if (isGameOver) return; // 이미 게임 오버 상태면 데미지를 받지 않음

        currentHealth -= damageAmount;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        isGameOver = true;
        player.SetActive(false); // 캐릭터 오브젝트 비활성화
        //gameOverUI.SetActive(true);
        LoadMainMenu();
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene("GameOver");
    }
}
