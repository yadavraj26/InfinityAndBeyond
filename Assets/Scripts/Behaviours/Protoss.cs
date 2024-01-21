using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NPBehave;
using UnityMovementAI;
using System.Linq;

public class Protoss : MonoBehaviour
{

    public MovementAIRigidbody zergRef;
    public MovementAIRigidbody terranRef;
    public GameObject planet;
    public GameObject sun;
    public GameObject gun;
    public GameManager gameManager;
    public int sunSeekDist;
    

    enum Behaviour
    {
        Wander=0,
        SoakInSun=1,
        Attack=2,
        Flee=3
    }
    
    public int health;
    public int firePower;
    private SteeringBasics steeringBasics;
    private Pursue pursueComponent;
    private Wander2 wanderComponent;
    private Flee fleeComponent;
    private WallAvoidance wallAvoidanceRef;
    private NearSensor sensorComponent;
    private Behaviour currentBehaviour;
    private MovementAIRigidbody target; // List of enemy targets
    private Root currentBehaviourTree;                  
    private Blackboard blackboard;
    private List<int> utilityScores;
    private float fireDelay=2.0f;
    private float fireElapsedTime;
    private float sunSoakElapsed;
    private int sunDist;
    private int planetDist;
    private float utilityUpdateElapsed;
    private float detoriateElapsed;

    // Start is called before the first frame update
    void Start()
    {
        pursueComponent = GetComponent<Pursue>();
        wanderComponent = GetComponent<Wander2>();
        steeringBasics = GetComponent<SteeringBasics>();
        wallAvoidanceRef = GetComponent<WallAvoidance>();
        fleeComponent = GetComponent<Flee>();

        sensorComponent = transform.Find("Sensor").GetComponent<NearSensor>();
        currentBehaviour = Behaviour.Wander;
        SwitchTree(SelectBehaviourTree(currentBehaviour));

        utilityScores = new List<int>();
        utilityScores.Add(0); // Wander
        utilityScores.Add(0); // SoakInSun
        utilityScores.Add(0); //Pursue
        utilityScores.Add(0); //Flee
        //Debug.Log((int)Behaviour.Wander);
        utilityScores[(int)Behaviour.Wander] = 8;

        health = 20;
        firePower = 20;
    }

    private void UpdateScores()
    {
        utilityScores[(int)Behaviour.SoakInSun] = 5;
        utilityScores[(int)Behaviour.Wander] = 7;
        utilityScores[(int)Behaviour.Flee] = 5;
        utilityScores[(int)Behaviour.Attack] = 5;
        steeringBasics.maxVelocity = 5;
        if ((sensorComponent.targets.Contains(zergRef)|| sensorComponent.targets.Contains(terranRef))&&health < 20)
        {
            if(!sensorComponent.targets.TryGetValue(zergRef, out target))
                sensorComponent.targets.TryGetValue(terranRef, out target);
            utilityScores[(int)Behaviour.Flee] = 12;
        }
        else if (sensorComponent.targets.Contains(zergRef))
        {
            utilityScores[(int)Behaviour.Attack] = 10;
            sensorComponent.targets.TryGetValue(zergRef, out target);
        }
        else if (sensorComponent.targets.Contains(terranRef))
        {
            utilityScores[(int)Behaviour.Attack] = 10;
            sensorComponent.targets.TryGetValue(terranRef, out target);
        }
        else
        {
            utilityScores[(int)Behaviour.SoakInSun] = (int)System.Math.Ceiling((double)400 / health + 300 / firePower);
            utilityScores[(int)Behaviour.Wander] = 8;
        }
        //Debug.Log(utilityScores[(int)Behaviour.SoakInSun]+"    "+ health+"    "+ firePower+ "    "+ (int)((float)(250 / health) + (float)(250 / firePower)));
    }

    private void SwitchTree(Root t)
    {
        if (currentBehaviourTree != null) currentBehaviourTree.Stop();

        currentBehaviourTree = t;
        blackboard = currentBehaviourTree.Blackboard;

        #if UNITY_EDITOR
        Debugger debugger = (Debugger)this.gameObject.AddComponent(typeof(Debugger));
        debugger.BehaviorTree = currentBehaviourTree;
        #endif
        Debug.Log("protoss" + currentBehaviour);
        currentBehaviourTree.Start();
    }


    private Root SelectBehaviourTree(Behaviour behaviour)
    {
        switch (behaviour)
        {
            case Behaviour.Wander:
                //Debug.Log("Wander");
                return GuardPlanetBehaviour();

            case Behaviour.SoakInSun:
                //Debug.Log("soak in sun");
                return SoakInSunBehaviour();

            case Behaviour.Attack:
                //Debug.Log("attack");
                return AttackBehaviour();

            case Behaviour.Flee:
                //Debug.Log("flee");
                return FleeBehaviour();
                    
            
            default:
                return WanderBehaviour();
        }
    }

    public void Wander()
    {
        Vector3 accel = wanderComponent.GetSteering();
        if (wallAvoidanceRef.GetSteering().magnitude > 0.005)
        {
            accel = wallAvoidanceRef.GetSteering();
        }
        steeringBasics.Steer(accel);
        steeringBasics.LookWhereYoureGoing();
    }

    public void Seek(GameObject seekRef)
    {
        Vector3 accel = steeringBasics.Seek(seekRef.transform.position);
        if (wallAvoidanceRef.GetSteering().magnitude > 0.005)
        {
            accel = wallAvoidanceRef.GetSteering();
        }
        steeringBasics.Steer(accel);
        steeringBasics.LookWhereYoureGoing();

    }    

    public void Pursue()
    {
        Vector3 accel = pursueComponent.GetSteering(target);
        if (wallAvoidanceRef.GetSteering().magnitude > 0.005)
        {
            accel = wallAvoidanceRef.GetSteering();
        }
        steeringBasics.Steer(accel);
        steeringBasics.LookWhereYoureGoing();
    }

    public void FlyBy(GameObject targetToFlyBy)
    {
        Vector3 dirToMove = (targetToFlyBy.transform.position - gameObject.transform.position).normalized;
        Vector3 accel = steeringBasics.Seek(targetToFlyBy.transform.position - (dirToMove * 5));
        if (wallAvoidanceRef.GetSteering().magnitude > 0.005)
        {
            accel = wallAvoidanceRef.GetSteering();
        }
        steeringBasics.Steer(accel);
        steeringBasics.LookAtDirection(dirToMove);
    }

    public void Attack()
    {
        Vector3 gunLookAt = target.transform.position - gun.transform.position;
        Quaternion rotation = Quaternion.LookRotation(gunLookAt, Vector3.up);
        gun.transform.rotation = rotation;
        fireElapsedTime += Time.deltaTime;
        
        if (fireElapsedTime>fireDelay)
        {
            gun.GetComponent<Gun>().Fire((int)firePower);
            
            fireElapsedTime = 0;
        }
    }

    public void FleeBack()
    {
        steeringBasics.maxVelocity = 10;
        Vector3 accel = fleeComponent.GetSteering(target.gameObject.transform.position);
        if (wallAvoidanceRef.GetSteering().magnitude > 0.005)
        {
            accel = wallAvoidanceRef.GetSteering();
        }
        //Debug.Log("run");
        steeringBasics.Steer(accel);
        steeringBasics.LookWhereYoureGoing();
    }

    public void UpdateSunDist()
    {
        sunDist= (int)Vector3.Distance(gameObject.transform.position, sun.transform.position);
        sunDist = Mathf.Abs(sunDist);
        //float sunDist = (float)currentBehaviourTree.Blackboard["sunDist"];
        
        
        if (sunDist < 5)
        {
            UpdateHealth(1);
            UpdateFirePower(1);
        }
        blackboard["sunDist"] = sunDist;
    }


    public void UpdatePlanetDist()
    {
        planetDist = (int)Vector3.Distance(gameObject.transform.position, planet.transform.position);
        planetDist = Mathf.Abs(planetDist);

        blackboard["planetDist"] = planetDist;
        
    }


    public void GatherReward()
    {
        //int sunDist = (int)currentBehaviourTree.Blackboard["sunDist"];
        sunDist = (int)Vector3.Distance(gameObject.transform.position, sun.transform.position);
        sunDist = Mathf.Abs(sunDist);
        if (sunDist < sunSeekDist)
        {
            sunSoakElapsed += Time.deltaTime;
            if (sunSoakElapsed > 1.5)
            {
                UpdateHealth(7);
                UpdateFirePower(7);
                sunSoakElapsed = 0;
            }
        }
    }

    public BlackboardCondition SeekSun()
    {
        return new BlackboardCondition("sunDist", Operator.IS_GREATER, sunSeekDist, Stops.IMMEDIATE_RESTART,
            new Action(() => Seek(sun)));
    }

    public BlackboardCondition SeekPlanet()
    {
        return new BlackboardCondition("planetDist", Operator.IS_GREATER, sunSeekDist, Stops.IMMEDIATE_RESTART,
            new Action(() => Seek(planet)));
    }

    private Root WanderBehaviour()
    {
        return new Root(new Action(() => Wander()));
    }

    private Root GuardPlanetBehaviour()
    {
        return new Root(new Service(2.0f, UpdatePlanetDist,
            new Selector(SeekPlanet(), new Action(() => Wander()))));
    }

    private Root PursueBehaviour()
    {
        return new Root(new Action(() => Pursue()));
    }

    private Root SoakInSunBehaviour()
    {
        /*return new Root(new Service(2.0f, UpdateSunDist,
            new Selector(SeekSun(), new Action(()=>Wander()))));*/
        return new Root(new Sequence(new Action(() => Seek(sun)),new Action(()=>GatherReward())));
    }

    private Root AttackBehaviour()
    {
        return new Root(new Sequence(new Action(() => FlyBy(target.gameObject)), new Action(() => Attack())));
    }

    private Root FleeBehaviour()
    {
        return new Root(new Action(() => FleeBack()));
    }



    // Update is called once per frame
    void Update()
    {
        utilityUpdateElapsed += Time.deltaTime;
        if (utilityUpdateElapsed > 1.0f||true)
        {
            UpdateScores();
            int maxValue = utilityScores.Max(t => t);
            int maxIndex = utilityScores.IndexOf(maxValue);
            //Debug.Log(((Behaviour)maxIndex)+"   "+maxValue);
            if ((int)currentBehaviour != maxIndex)
            {
                currentBehaviour = (Behaviour)maxIndex;
                SwitchTree(SelectBehaviourTree(currentBehaviour));
                //Debug.Log(currentBehaviour);
            }
            utilityUpdateElapsed = 0;
        }

        /*detoriateElapsed += Time.deltaTime;
        if (detoriateElapsed > 2.0f && currentBehaviour!=Behaviour.SoakInSun)
        {
            health = health - 4;
            firePower = firePower - 4;
            detoriateElapsed=0;
        }*/


        //Debug.Log(health);
    }

    public void UpdateHealth(int alterAmount)
    {
        health += alterAmount;
        if (health <= 0)
        {
            health = 0;
            gameManager.KillPlayer(gameObject);
        }
        else if (health > 100)
            health = 100;
    }

    public void UpdateFirePower(int alterAmount)
    {
        firePower += alterAmount;
        if (firePower <= 0)
            firePower = 0;
        else if (firePower > 100)
            firePower = 100;
    }
}
