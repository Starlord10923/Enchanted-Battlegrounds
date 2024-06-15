using System.Collections;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public float attackPoints = 15;
    public Transform ownerTransform;
    public CapsuleCollider weapon;
    public GameObject[] magicBall;
    public Transform magicOrigin;
    void Start()
    {
        weapon = GetComponent<CapsuleCollider>();
        attackPoints = transform.GetComponentInParent<UnitAttributes>().attackDamage;
        magicOrigin = transform.Find("MagicOrigin");

        FindOwner();
    }
    void FindOwner(){
        Transform parentObject = transform.parent;
        while(parentObject!=null && parentObject.tag!=null){
            if (parentObject.CompareTag("Player") || parentObject.CompareTag("Enemy")){
                transform.tag = parentObject.tag;
                ownerTransform = parentObject.transform;
                break;
            }
            parentObject = parentObject.parent;
        }
    }

    public void FireBall()
    {
        Quaternion ownerRotation = ownerTransform.rotation;
        int magicBallIndex = Random.Range(0, magicBall.Length);

        int numFireballs = Random.Range(1, 4);
        if (CompareTag("Enemy"))
            numFireballs = 1;

        UnitAttributes unitAttributes = ownerTransform.gameObject.GetComponent<UnitAttributes>();
        unitAttributes.SpendMana(numFireballs > 1 ? unitAttributes.fireballManaCost * (numFireballs - 1) : 0);
        
        if (numFireballs % 2 != 0)
        {
            for (int i = 0; i < numFireballs; i++)
            {
                float angle = new int[] { 0, -1, 1 }[i] * 1.75f;
                Vector3 offset = ownerTransform.right*angle;
                GameObject magicAttack = Instantiate(
                    magicBall[magicBallIndex],
                    ownerTransform.position + offset,
                    ownerRotation
                );
                if (transform.CompareTag("Enemy"))
                    magicAttack.transform.position += Vector3.up * 3.3f;

                magicAttack.GetComponent<MagicBall>().owner = ownerTransform.tag;
                Destroy(magicAttack, 8.0f);
            }
        }
        else
        {
            StartCoroutine(SpawnFireballs(ownerRotation, magicBallIndex, numFireballs));
        }
    }

    IEnumerator SpawnFireballs(Quaternion ownerRotation, int magicBallIndex, int numFireballs)
    {
        for (int i = 0; i < numFireballs; i++)
        {
            float delay = 0.15f; // Adjust the delay time as needed

            yield return new WaitForSeconds(delay);

            GameObject magicAttack = Instantiate(
                magicBall[magicBallIndex],
                ownerTransform.position + ownerTransform.forward * 2f, // Adjust the offset if needed
                ownerRotation
            );
            if (transform.CompareTag("Enemy"))
                magicAttack.transform.position += Vector3.up * 3;

            magicAttack.GetComponent<MagicBall>().owner = ownerTransform.tag;
            Destroy(magicAttack, 8.0f);
        }
    }

    void OnTriggerEnter(Collider other){
        // if (Vector3.Distance(transform.position, other.transform.position) < 6)
        // {
            if (other.gameObject.CompareTag("Enemy") && ownerTransform.CompareTag("Player") && other.GetComponent<UnitAttributes>()!=null)
            {
                UnitAttributes enemy = other.gameObject.GetComponent<UnitAttributes>();
                enemy.GetHit(attackPoints + Random.Range(0f, 5f),ownerTransform);
            }
            else if (other.gameObject.CompareTag("Player") && ownerTransform.CompareTag("Enemy") && other.GetComponent<UnitAttributes>()!=null)
            {
                UnitAttributes player = other.gameObject.GetComponent<UnitAttributes>();
                player.GetHit(attackPoints + Random.Range(0f, 5f),ownerTransform);
            }
        // }
    }
}
