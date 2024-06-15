using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Unit : MonoBehaviour
{
    public NavMeshAgent navMeshAgent;
    public Transform player;
    public Transform enemyCommander;
    public UnitAttributes unitAttributes;
    public Animator animator;
    public CharacterController characterController;
    public Material knockbackEffectMaterial;
    public Material unitMaterial;
    public GameObject deathCoffin;
    public List<Transform> enemiesInRange = new List<Transform>();
    public AudioSource audioSourceWalk;
    public AudioSource audioSourceAttack;
    public Weapon weaponEquipped;
    public string targetTag;
    void Start(){
        navMeshAgent = GetComponent<NavMeshAgent>();
        unitAttributes = GetComponent<UnitAttributes>();
        animator = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();
        knockbackEffectMaterial = GetComponentInChildren<SkinnedMeshRenderer>().materials[1];
        unitMaterial = GetComponentInChildren<SkinnedMeshRenderer>().materials[2];
        audioSourceWalk = GetComponents<AudioSource>()[0];
        audioSourceAttack = GetComponents<AudioSource>()[1];
        weaponEquipped = GetComponentInChildren<Weapon>();
        
        player = GameObject.Find("Player").GetComponent<Transform>();
        if (gameObject.name == "Enemy")
            enemyCommander = gameObject.transform;
        else if(GameObject.Find("Enemy")!=null)
            enemyCommander = GameObject.Find("Enemy").GetComponent<Transform>();

        if (transform.CompareTag("Player"))
            SetPlayerUnit();
        else if (transform.CompareTag("Enemy"))
            SetEnemyUnit();

        navMeshAgent.speed = unitAttributes.speed;
    }
    public void MoveTowardsTarget()
    {
        navMeshAgent.speed = unitAttributes.speed;
        enemiesInRange.RemoveAll(enemy => enemy == null);
        
        if (!unitAttributes.isDead && navMeshAgent != null && navMeshAgent.enabled && navMeshAgent.isOnNavMesh && enemiesInRange.Count > 0)
        {
            animator.SetBool("FoundTarget", true);
            navMeshAgent.SetDestination(enemiesInRange[0].position);
            WalkSound();
        }
        else if (gameObject.tag == player.tag)
        {
            animator.SetBool("FoundTarget", true);
            Vector3 followPosition = player.position + player.forward * 25 + Vector3.up * 10;
            if (Physics.Raycast(followPosition, Vector3.down, out RaycastHit hit, Mathf.Infinity, LayerMask.GetMask("Terrain")))
            {
                navMeshAgent.SetDestination(hit.point);
                WalkSound();
            }
        }
        else
        {
            animator.SetBool("FoundTarget", false);
            StopWalkSound();
        }
    }

    public void InstantiateDeathCoffinOnTerrain()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position + Vector3.up * 10f, Vector3.down, out hit, Mathf.Infinity, LayerMask.GetMask("Terrain")))
        {
            Vector3 terrainNormal = hit.normal;
            Quaternion rotationOnTerrain = Quaternion.FromToRotation(Vector3.up, terrainNormal) * deathCoffin.transform.rotation;
            Instantiate(deathCoffin, hit.point - terrainNormal * 0.7f, rotationOnTerrain);   
        }else
            Debug.LogError("Failed to instantiate deathCoffin on the terrain.");
    }
    
    public void UnitDead()
    {
        if (gameObject.CompareTag("Enemy"))
            GameObject.Find("Spawner").GetComponent<Spawner>().EnemyDecrement();
        if (gameObject.CompareTag("Player"))
            GameObject.Find("Player").GetComponent<PlayerController>().currentSoldierUnits--;
        if(Application.isPlaying)
            InstantiateDeathCoffinOnTerrain();
    }
    void OnTriggerEnter(Collider other)
    {
        if (!string.IsNullOrEmpty(targetTag) && other.CompareTag(targetTag))
            enemiesInRange.Add(other.transform);
    }

    void OnTriggerExit(Collider other)
    {
        while (enemiesInRange.Contains(other.transform))
            enemiesInRange.Remove(other.transform);
    }
    public void SetEnemyUnit(){
        if(name!="Enemy")
            gameObject.name = "EnemyUnit";
        gameObject.tag = enemyCommander.tag;
        targetTag = player.tag;
        unitMaterial.SetColor("_Color", Color.red);
    }
    public void SetPlayerUnit(){
        gameObject.name = "PlayerUnit";
        gameObject.tag = player.tag;
        targetTag = "Enemy";
        unitMaterial.SetColor("_Color", Color.cyan);
    }
    public void HitSound(){
        audioSourceAttack.Play();
    }
    public void WalkSound(){
        if(!audioSourceWalk.isPlaying)
            audioSourceWalk.Play();
    }
    public void StopWalkSound(){
        audioSourceWalk.Stop();
    }
}