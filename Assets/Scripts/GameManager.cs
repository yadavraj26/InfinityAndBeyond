using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public List<GameObject> agents;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void KillPlayer(GameObject objectToKill)
    {
        agents.Remove(objectToKill);
        Destroy(objectToKill);
        if(!(agents.Count>1))
        {
            Debug.Log(agents[0].name + " won the war");
            Time.timeScale = 0;
        }
    }

    public void PlanetKilled()
    {
        Debug.Log(" Zerg destroyed the planet and won the war");
        Time.timeScale = 0;
    }
}
