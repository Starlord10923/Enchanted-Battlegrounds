using System.Collections;
using UnityEngine;

public class CoffinRevival : MonoBehaviour
{
    public GameObject revivalUnit;

    public IEnumerator ReviveCoroutine(string ownerTag)
    {
        if (Physics.Raycast(transform.position + Vector3.up * 10, Vector3.down, out RaycastHit hit, Mathf.Infinity, LayerMask.GetMask("Terrain")))
        {
            Quaternion rotation = Quaternion.Euler(transform.position - GameObject.Find("Player").transform.position);
            GameObject soldierUnit = Instantiate(revivalUnit, hit.point, rotation);
            soldierUnit.tag = ownerTag;

            if (CompareTag("Enemy"))
            {
                if (soldierUnit.GetComponent<SoldierUnit>())
                    soldierUnit.GetComponent<SoldierUnit>().SetEnemyUnit();
                else if (soldierUnit.GetComponent<EnemyCommander>())
                    soldierUnit.GetComponent<EnemyCommander>().SetEnemyUnit();
            }else if(CompareTag("Player")){
                if (soldierUnit.GetComponent<SoldierUnit>())
                    soldierUnit.GetComponent<SoldierUnit>().SetPlayerUnit();
                else if (soldierUnit.GetComponent<EnemyCommander>())
                    soldierUnit.GetComponent<EnemyCommander>().SetPlayerUnit();
            }
        }

        yield return null; // Wait for one frame to allow the new playerUnit to initialize.

        Destroy(gameObject);
    }
}
