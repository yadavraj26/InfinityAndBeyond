using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sun : MonoBehaviour
{

    public int healAmount=100;
    public int killAmount=10;
    public int fireUpgradeAmount=100;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        switch (collision.gameObject.tag)
        {
            case "protoss":
                collision.gameObject.GetComponent<Protoss>().UpdateHealth(healAmount);
                collision.gameObject.GetComponent<Protoss>().UpdateFirePower(fireUpgradeAmount);
                break;
            case "zerg":
                //collision.gameObject.GetComponent<Zerg>().ReduceHealth(bulletPower / 5);
                break;
            default: Debug.Log("hit no one"); break;
        }
    }
}
