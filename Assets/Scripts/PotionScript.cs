using UnityEngine;
public enum Potion{Health,Mana};
public class PotionScript : MonoBehaviour
{
    public Potion potion;
    public float recoverAmount = 40f;
    void OnTriggerEnter(Collider other)
    {
        if (other.transform.GetComponent<UnitAttributes>() != null){
            if(other.GetComponent<PlayerController>()!=null)
                other.GetComponent<PlayerController>().PlayPotionSound();
            UnitAttributes unitAttributes = other.transform.GetComponent<UnitAttributes>();
            if (potion == Potion.Health && unitAttributes.currentHP!=unitAttributes.maxHP){
                unitAttributes.GetHP(recoverAmount);
                GameObject.Find("Spawner").GetComponent<Spawner>().currentPotions--;
                Destroy(transform.parent.gameObject);
            }
            if (potion == Potion.Mana && unitAttributes.hasMana && unitAttributes.currentMana!=unitAttributes.maxMana){
                unitAttributes.GetMana(recoverAmount);
                GameObject.Find("Spawner").GetComponent<Spawner>().currentPotions--;
                Destroy(transform.parent.gameObject);
            }
        }
    }
    
}
