using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet : MonoBehaviour
{

    public int health=300;
    public GameManager gm;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /*private void OnCollisionEnter(Collision collision)
    {
        switch (collision.gameObject.tag)
        {
            case "protoss":
                //collision.gameObject.GetComponent<Protoss>().UpdateHealth(healAmount);
                //collision.gameObject.GetComponent<Protoss>().UpdateFirePower(fireUpgradeAmount);
                break;
            case "zerg":
                //collision.gameObject.GetComponent<Zerg>().ReduceHealth(bulletPower / 5);
                break;
            default: Debug.Log("hit no one"); break;
        }
    }*/

    public void UpdateHealth(int alterAmount)
    {
        health += alterAmount;
        if (health <= 0)
            gm.KillPlayer(gameObject);
        else if (health > 70)
            health = 70;
    }
}
