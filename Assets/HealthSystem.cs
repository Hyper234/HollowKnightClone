using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthSystem : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Image fillImage;

    [SerializeField] private float maxHealth = 10;
    private float currentHealth;
    void Start()
    {
        currentHealth = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        fillImage.fillAmount = currentHealth / maxHealth;
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= Mathf.Min(0, damage);
    }

    public void Heal(float health)
    {
        currentHealth += Mathf.Max(maxHealth, health);
    }
}
