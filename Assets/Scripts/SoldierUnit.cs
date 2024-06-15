using UnityEngine;

public class SoldierUnit : Unit
{
    public bool canAttack = true;
    public float currentAttackInterval = 0f;
    void Update()
    {
        currentAttackInterval += Time.deltaTime;
        canAttack = currentAttackInterval > unitAttributes.attackInterval;
        if (canAttack)
            CheckAttackRange();
            
        MoveTowardsTarget();
    }
    void CheckAttackRange()
    {
        Collider[] hitEnemies = Physics.OverlapSphere(transform.position, unitAttributes.attackRange);
        foreach (Collider enemyCol in hitEnemies)
        {
            if (enemyCol.CompareTag(targetTag))
            {
                Attack();
                break;
            }
        }
        currentAttackInterval = 0f;
    }
    void Attack()
    {
        animator.SetTrigger("Attack" + Random.Range(1, 3));
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
    public void DieAction()
    {
        UnitDead();
        animator.SetBool("isDead", true);
    }
}
