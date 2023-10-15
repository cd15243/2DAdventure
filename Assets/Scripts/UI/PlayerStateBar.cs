using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStateBar : MonoBehaviour
{
    public Image healthImage;
    public Image healthDelayImage;
    public Image powerImage;
    private bool isRecovering;
    private Character currentCharacter;

    private void Update() {
        if(healthDelayImage.fillAmount > healthImage.fillAmount){
            healthDelayImage.fillAmount -= Time.deltaTime;
        }
        else{
            healthDelayImage.fillAmount = healthImage.fillAmount;
        }

        if(isRecovering){
            float persentage = currentCharacter.currentPower/currentCharacter.maxPower;
            powerImage.fillAmount = persentage;

            if(persentage >= 1){
                isRecovering = false;
                return;
            }
        }
    }
    public void OnHealthChange(float persentage){
        healthImage.fillAmount = persentage;
    }

    public void OnPowerChange(Character character){
        currentCharacter = character;
        isRecovering = true;
    }
}
