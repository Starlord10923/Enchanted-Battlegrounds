using UnityEngine;

public class EnemyCommander : Unit
{
    public bool canAttack = true;
    public float currentAttackInterval = 0f;
    public float reviveCost = 20f;
    public float reviveInterval=10f;
    public float currentReviveTime=0f;
    void Update()
    {
        currentAttackInterval += Time.deltaTime;
        canAttack = currentAttackInterval > unitAttributes.attackInterval;
        if(canAttack)
            CheckAttackRange();
            
        MoveTowardsTarget();

        currentReviveTime += Time.deltaTime;
        if(currentReviveTime>reviveInterval){
            currentReviveTime = 0f;
            ReviveUnit();
        }
    }
    void CheckAttackRange()
    {
        if(navMeshAgent.velocity.magnitude>0)
            Attack();
        currentAttackInterval = 0f;
    }
    void Attack(){
        animator.SetTrigger("Attack");
        if (Random.Range(0f, 1f) > 0.5)
        {
            if (weaponEquipped != null && unitAttributes.currentMana >= unitAttributes.fireballManaCost)
            {
                weaponEquipped.Invoke("FireBall", 0.5f);
                unitAttributes.SpendMana(unitAttributes.fireballManaCost);
            }
        }

    }
    public void GetHitAction(bool isDead,Transform attacker)
    {
        if (attacker.name == "PlayerUnit" || attacker.name == "EnemyUnit")
        {
            HitSound();
        }
        enemiesInRange.Add(attacker);

        animator.SetTrigger("GetHit");
        StartCoroutine(unitAttributes.KnockbackCoroutine());
        if (isDead)
            DieAction();
    }

    public void DieAction(){
        UnitDead();
        animator.SetBool("isDead", true);
    }
    void ReviveUnit()
    {
        GameObject coffin = GameObject.FindWithTag("Coffin");
        if (coffin != null && unitAttributes.currentMana >= reviveCost)
        {
            CoffinRevival coffinRevival = coffin.GetComponent<CoffinRevival>();
            StartCoroutine(coffinRevival.ReviveCoroutine(tag));
            unitAttributes.SpendMana(reviveCost);
            Destroy(coffin, 0.07f);
            
            GameObject.Find("Spawner").GetComponent<Spawner>().enemiesRemaining++;
            
        }
    }
}