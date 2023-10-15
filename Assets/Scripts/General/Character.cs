using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class Character : MonoBehaviour
{
    [Header("基本属性")]
    public float maxHealth;
    public float currentHealth;
    public float maxPower;
    public float currentPower;
    public float powerRecoverSpeed;
    [Header("受伤无敌")]
    public float invincibleDuration;
    public float invincibleCounter;
    public bool isInvincible;
    public UnityEvent<Transform> OnTakeDamage;
    public UnityEvent<Character> OnHealthChange;
    public UnityEvent OnDead;
    private void Start() {
        currentHealth = maxHealth;
        currentPower = maxPower;
        OnHealthChange?.Invoke(this);
    }
    private void Update() {
        invincibleCounter -= Time.deltaTime;
        if(invincibleCounter <= 0){
            isInvincible = false;
        }
        
        if(currentPower < maxPower){
            currentPower += powerRecoverSpeed * Time.deltaTime;
            // OnHealthChange?.Invoke(this);
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

        OnHealthChange?.Invoke(this);
    }

    public void TriggerInvulnerable(){
        if(!isInvincible){
            isInvincible = true;
            invincibleCounter = invincibleDuration;
        }
    }

    public void OnSlide(float cost){
        currentPower -= cost;
        OnHealthChange?.Invoke(this);
    }
}
