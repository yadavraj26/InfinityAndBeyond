using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityMovementAI;

public class Generator : MonoBehaviour
{
    public int width, height;
    public int initBlackAmount;
    public int[,] canvas;
    public int automataIterations=15;

    public GameObject quadToSpawn;
    public GameObject planet;
    public GameObject sun;
    public GameObject boundry;
    public GameObject protossShip;
    public GameObject zergShip;
    public GameObject terranShip;
    public SpawnerScript spawner;
    public GameManager gameManager;
    [SerializeField]
    public float seed;
    private GameObject[,] quadsInCanvas;
    // Start is called before the first frame update
    void Start()
    {        
        canvas = new int[width, height];
        for(int i = 0; i < 3; i++)
        {
            CreatePlanet();
        }
        CreateRandomoMap(initBlackAmount);
        ApplyCellularAutomata();
        SpawnQuads();
        planet=spawner.SpawnObject(planet, 2);
        sun=spawner.SpawnObject(sun, 2);
        protossShip = spawner.SpawnObject(protossShip, 4);
        protossShip.GetComponent<Protoss>().planet = planet;
        protossShip.GetComponent<Protoss>().sun = sun;
        protossShip.GetComponent<Protoss>().gameManager = gameManager;

        zergShip = spawner.SpawnObject(zergShip, 4);
        zergShip.GetComponent<ZergStarship>().planet = planet;
        zergShip.GetComponent<ZergStarship>().sun = sun;
        zergShip.GetComponent<ZergStarship>().gameManager = gameManager;

        terranShip = spawner.SpawnObject(terranShip, 4);
        terranShip.GetComponent<Terran>().planet = planet;
        terranShip.GetComponent<Terran>().sun = sun;
        terranShip.GetComponent<Terran>().gameManager = gameManager;

        protossShip.GetComponent<Protoss>().zergRef = zergShip.GetComponent<MovementAIRigidbody>();
        zergShip.GetComponent<ZergStarship>().protossRef = protossShip.GetComponent<MovementAIRigidbody>();
        terranShip.GetComponent<Terran>().protossRef = protossShip.GetComponent<MovementAIRigidbody>();
        terranShip.GetComponent<Terran>().ZergRef = zergShip.GetComponent<MovementAIRigidbody>();
        protossShip.GetComponent<Protoss>().terranRef=terranShip.GetComponent<MovementAIRigidbody>();
        zergShip.GetComponent<ZergStarship>().terranRef= terranShip.GetComponent<MovementAIRigidbody>(); ;

        gameManager.agents.Add(protossShip);
        gameManager.agents.Add(zergShip);
        gameManager.agents.Add(terranShip);

    }
    void CreatePlanet()
    {
        //seed = Time.time;
        System.Random randomGenerator = new System.Random((int)seed);

        int x = randomGenerator.Next(2, width - 2), y = randomGenerator.Next(2, height - 2);
        for(int i=x-2; i<= x+2; i++)
        {
            for(int j=y-2; j <= y + 2; j++)
            {
                canvas[x, y] = 1;
            }
        }
    }

    void SpawnQuads()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if(canvas[x,y]==1)
                    Instantiate(quadToSpawn, new Vector3(x, 0, y), quadToSpawn.transform.rotation);
            }
        }
    }

    void CreateRandomoMap(int amountOfBlack)
    {
        seed = Random.Range(0, 100);
        System.Random randomGenerator = new System.Random((int)seed);
        for(int x=0;x<width;x++)
        {
            for(int y=0;y<height;y++)
            {
                if (x <= 0 || x >= width - 1 || y <= 0 || y >= height - 1)
                {
                    canvas[x, y] = 0;

                    //Create Boundary for map
                    Instantiate(boundry, new Vector3(x , 0, y), quadToSpawn.transform.rotation);
                    
                    /*if (x==0)
                    {
                        Instantiate(boundry, new Vector3(x-1, 0, y), quadToSpawn.transform.rotation);
                    }
                    else if(x==width-1)
                    {
                        Instantiate(boundry, new Vector3(x + 1, 0, y), quadToSpawn.transform.rotation);
                    }
                    else if(y==0)
                    {
                        Instantiate(boundry, new Vector3(x, 0, y-1), quadToSpawn.transform.rotation);
                    }
                    else if(y==height-1)
                    {
                        Instantiate(boundry, new Vector3(x, 0, y+1), quadToSpawn.transform.rotation);
                    }*/
                    //SpawnQuads(new Vector3(x, 0, y));
                }
                else
                {
                    if (randomGenerator.Next(0, 100) < amountOfBlack)
                    {
                        canvas[x, y] = 1;
                        //SpawnQuads(new Vector3(x, 0, y));
                    }
                    else
                        canvas[x, y] = 0;
                }
            }
        }
    }


    void ApplyCellularAutomata()
    {
        int neighbourWalls;
        int[,] dummy = canvas;
        for (int n = 0; n < automataIterations; n++)
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    neighbourWalls = 0;
                    for (int i = x - 1; i <= x + 1; i++)
                    {
                        for (int j = y - 1; j <= y + 1; j++)
                        {
                            if (i >= 0 && i < width && j >= 0 && j < height)
                            {
                                if (i != x || j != y)
                                {
                                    neighbourWalls += canvas[i, j];
                                }
                            }
                            else
                                neighbourWalls++;
                        }
                    }
                    if (neighbourWalls > 4)
                    {
                        dummy[x, y] = 1;
                        
                    }
                    else if (neighbourWalls < 4)
                    {
                        dummy[x, y] = 0;
                    }
                    else
                    {
                        dummy[x, y] = canvas[x, y];
                    }
                }
            }
            canvas = dummy;
        }

    }

    
    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            /*foreach(GameObject x in GameObject.FindGameObjectsWithTag("Respawn"))
            {
                Destroy(x);
            }
            ApplyCellularAutomata();
            SpawnQuads();*/
            

        }
    }
}