using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class HealthSystem : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Image fillImage;

    [Header("Health")]
    [SerializeField] private float maxHealth = 10;

    private Animator animator;

    private float currentHealth;

    void Start()
    {
        animator = GetComponent<Animator>();

        currentHealth = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        fillImage.fillAmount = currentHealth / maxHealth;
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;

        //death when hp is 0 or lower
        if(currentHealth <= 0)
        {
            GetComponent<PlayerMovement>().DisableMovement();
            animator.SetTrigger("death");
        }
    }

    public void Heal(float health)
    {
        currentHealth = Mathf.Min(maxHealth, currentHealth + health);
    }
}
