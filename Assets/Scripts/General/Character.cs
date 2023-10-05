using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class Character : MonoBehaviour
{
    [Header("基本属性")]
    public float maxHealth;
    public float currentHealth;
    [Header("受伤无敌")]
    public float invincibleDuration;
    public float invincibleCounter;
    public bool isInvincible;
    public UnityEvent<Transform> OnTakeDamage;
    public UnityEvent OnDead;
    private void Start() {
        currentHealth = maxHealth;
    }
    private void Update() {
        invincibleCounter -= Time.deltaTime;
        if(invincibleCounter <= 0){
            isInvincible = false;
        }
    }
    public void TakeDamage(Attack attacker){
        if(isInvincible){
            return;
        }
        if(currentHealth - attacker.damage > 0){
            currentHealth -= attacker.damage;
            TriggerInvulnerable();
            OnTakeDamage?.Invoke(attacker.transform);
        }
        else{
            currentHealth = 0;
            OnDead?.Invoke();
        }
    }

    public void TriggerInvulnerable(){
        if(!isInvincible){
            isInvincible = true;
            invincibleCounter = invincibleDuration;
        }
    }
}
