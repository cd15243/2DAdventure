using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public PlayerStateBar playerStateBar;
    [Header("监听事件")]
    public CharacterEventSO healthEvent;

    private void OnEnable() {
        healthEvent.OnEventRaised += OnHealthEvent;
    }

    private void OnDisable() {
        healthEvent.OnEventRaised -= OnHealthEvent;
    }

    private void OnHealthEvent(Character character)
    {
        var percentage = character.currentHealth/character.maxHealth;
        playerStateBar.OnHealthChange(percentage);
        playerStateBar.OnPowerChange(character);
    }
}
