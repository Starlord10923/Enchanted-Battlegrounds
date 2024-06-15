using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class UnitAttributes : MonoBehaviour
{
    public float speed = 1f;
    public float maxHP = 100;
    public float currentHP;
    public bool hasMana = false;
    public float maxMana = 50;
    public float currentMana;
    private float currentManaDisplay;
    public float manaGainPerSecond = 3;
    public float fireballManaCost = 8;
    public Slider healthSlider;
    public Slider manaSlider;
    public float attackRange = 6;
    public float attackDamage = 15;
    public float attackInterval = 4.0f;
    public float knockbackDuration = 0.4f;
    public bool isDead = false;
    public Transform attackPoint;

    void Start()
    {
        healthSlider = transform.Find("HealthCanvas").GetComponentInChildren<Slider>();
        currentHP = maxHP;
        healthSlider.maxValue = maxHP;
        healthSlider.value = currentHP;
        if(transform.Find("ManaCanvas")!=null){
            manaSlider = transform.Find("ManaCanvas").GetComponentInChildren<Slider>();
            hasMana = true;
            currentMana = maxMana;
            currentManaDisplay = currentMana;
            manaSlider.maxValue = maxMana;
            manaSlider.value = currentManaDisplay;
            StartCoroutine(GainManaConstantly());
        }
    }
    public void GetHit(float attackPoints,Transform attacker){
        if (isDead)
            return;
        StartCoroutine(HealthBarChange(currentHP));
        currentHP -= attackPoints;

        if (currentHP <= 0){
            isDead = true;
            Die();
        }

        if(GetComponent<EnemyCommander>()!=null)
            GetComponent<EnemyCommander>().GetHitAction(isDead,attacker);
        if(GetComponent<SoldierUnit>()!=null)
            GetComponent<SoldierUnit>().GetHitAction(isDead,attacker);
        if(GetComponent<PlayerController>()!=null)
            GetComponent<PlayerController>().GetHitAction(isDead,attacker);
    }
    void Die(){
        Destroy(gameObject, 2.5f);
    }
    public IEnumerator GainManaConstantly(){
        while(true){
            if(currentMana<=maxMana){
                currentMana += manaGainPerSecond * Time.deltaTime;
                currentMana = Mathf.Clamp(currentMana,0, maxMana);
                currentManaDisplay = Mathf.Lerp(currentManaDisplay,currentMana,0.05f);
                manaSlider.value = currentManaDisplay;
            }
            yield return new WaitForEndOfFrame();
        }
    }
    public IEnumerator HealthBarChange(float previousHP){
        float healthDecreaseTime = 0.2f;
        float currentTime = 0f;
        while(currentTime<healthDecreaseTime){
            currentTime += Time.deltaTime;
            healthSlider.value = Mathf.Lerp(previousHP,currentHP,currentTime/healthDecreaseTime);
            yield return new WaitForEndOfFrame();
        }
        yield return null;
    }
    public void GetHitAction(float attackPoints,Transform attacker){
        if(isDead){
            return;
        }
        currentHP -= attackPoints;
        StartCoroutine(HealthBarChange(currentHP));

        if(GetComponent<EnemyCommander>()!=null){
            GetComponent<EnemyCommander>().GetHitAction(isDead, attacker);
        }else if(GetComponent<SoldierUnit>()!=null){
            GetComponent<SoldierUnit>().GetHitAction(isDead, attacker);
        }else if(GetComponent<PlayerController>()!=null){
            GetComponent<PlayerController>().GetHitAction(isDead, attacker);
        }
        
    }
    public IEnumerator KnockbackCoroutine(){
        NavMeshAgent navMeshAgent = GetComponent<Unit>().navMeshAgent;
        UnitAttributes unitAttributes = GetComponent<Unit>().unitAttributes;
        Material knockbackEffectMaterial = GetComponent<Unit>().knockbackEffectMaterial;

        if (navMeshAgent != null && navMeshAgent.isOnNavMesh && navMeshAgent.isActiveAndEnabled){            
            navMeshAgent.enabled = false;

            float knockbackScaleInitial = 1.0f;
            float knockbackAlphaInitial = 1f;
            float knockbackScaleFinal = 1.45f;
            float knockbackAlphaFinal = 0f;

            float elapsedTime = 0f;

            while (elapsedTime < unitAttributes.knockbackDuration){
                float knockbackScaleCurrent = Mathf.Lerp(knockbackScaleInitial, knockbackScaleFinal, elapsedTime / unitAttributes.knockbackDuration);
                float knockbackAlphaCurrent = Mathf.Lerp(knockbackAlphaInitial, knockbackAlphaFinal, elapsedTime / unitAttributes.knockbackDuration);
                knockbackEffectMaterial.SetFloat("_Scale", knockbackScaleCurrent);
                knockbackEffectMaterial.SetFloat("_Alpha", knockbackAlphaCurrent);

                elapsedTime += Time.deltaTime;
                yield return null;
            }

            knockbackEffectMaterial.SetFloat("_Scale", knockbackScaleInitial);
            knockbackEffectMaterial.SetFloat("_Alpha", knockbackAlphaFinal);

            navMeshAgent.enabled = true;
        }
    }
    public IEnumerator GainManaContinuously(){
        while(true){
            if(currentMana<maxMana){
                currentMana += manaGainPerSecond * Time.deltaTime;
                currentMana = Mathf.Clamp(currentMana, 0, maxMana);
                currentManaDisplay = Mathf.Lerp(currentManaDisplay, currentMana, 0.15f);
                manaSlider.value = currentManaDisplay;
            }
            yield return new WaitForEndOfFrame();
        }
    }
    public void GetMana(float amount){
        currentMana = Mathf.Clamp(currentMana + amount, 0, maxMana);
    }
    public void SpendMana(float amount){
        currentMana = Mathf.Clamp(currentMana - amount, 0, maxMana);
    }
    public void GetHP(float amount){
        currentHP = Mathf.Clamp(currentHP + amount, 0, maxHP);
        StartCoroutine(HealthBarChange(currentHP));
    }
}
