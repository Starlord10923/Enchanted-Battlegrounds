using UnityEngine;

public class MagicBall : MonoBehaviour{
    public float speed = 18.0f;
    public int attackPoints=15;
    public int maxHits = 2;
    public string owner;
    Rigidbody rigidbody;
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
    }
    void Update()
    {
        rigidbody.velocity = transform.forward * speed;
    }
    void OnTriggerEnter(Collider other)
    {
        if (Vector3.Distance(transform.position, other.transform.position) < 6)
        {
            if (other.gameObject.CompareTag("Enemy") && owner == "Player" && other.GetComponent<UnitAttributes>()!=null)
            {
                UnitAttributes enemy = other.GetComponent<UnitAttributes>();
                enemy.GetHit(attackPoints + Random.Range(0, 5),GameObject.Find("Player").transform);
                maxHits--;
            }
            else if (other.gameObject.CompareTag("Player") && owner == "Enemy" && other.GetComponent<UnitAttributes>()!=null)
            {
                UnitAttributes player = other.gameObject.GetComponent<UnitAttributes>();
                player.GetHit(attackPoints + Random.Range(0, 5),GameObject.Find("Enemy").transform);
                maxHits--;
            }
        }
        if (maxHits <= 0)
            Destroy(gameObject);
    }

}
