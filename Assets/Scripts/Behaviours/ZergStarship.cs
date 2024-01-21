using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NPBehave;
using UnityMovementAI;
using System.Linq;

public class ZergStarship : MonoBehaviour
{

    public MovementAIRigidbody protossRef;
    public MovementAIRigidbody terranRef;
    public GameObject planet;
    public GameObject sun;
    public GameObject gun;
    public GameManager gameManager;
    public int sunSeekDist;
    

    enum Behaviour
    {
        Wander=0,
        GoNearPlanet=1,
        Attack=2,
        Flee=3,
        Destroy=4
    }
    
    public int health;
    public float firePower;
    private SteeringBasics steeringBasics;
    private Pursue pursueComponent;
    private Wander2 wanderComponent;
    private Flee fleeComponent;
    private WallAvoidance wallAvoidanceRef;
    private ZergSensor sensorComponent;
    private Behaviour currentBehaviour;
    private MovementAIRigidbody target;
    private GameObject objectInRange;
    private Root currentBehaviourTree;                  
    private Blackboard blackboard;
    private List<int> utilityScores;
    private float fireDelay=2.0f;
    private float fireElapsedTime;
    private float SoakElapsed;
    private int sunDist;
    private int planetDist;
    private float utilityUpdateElapsed;

    // Start is called before the first frame update
    void Start()
    {
        pursueComponent = GetComponent<Pursue>();
        wanderComponent = GetComponent<Wander2>();
        steeringBasics = GetComponent<SteeringBasics>();
        wallAvoidanceRef = GetComponent<WallAvoidance>();
        fleeComponent = GetComponent<Flee>();

        sensorComponent = transform.Find("Sensor").GetComponent<ZergSensor>();
        currentBehaviour = Behaviour.Wander;
        SwitchTree(SelectBehaviourTree(currentBehaviour));

        utilityScores = new List<int>();
        utilityScores.Add(0); // Wander
        utilityScores.Add(0); // SoakInPlanet
        utilityScores.Add(0); //Attack
        utilityScores.Add(0); //Flee
        utilityScores.Add(0); //Destroy
        //Debug.Log((int)Behaviour.Wander);
        utilityScores[(int)Behaviour.Wander] = 8;

        health = 10;
        firePower = 30;

        
    }

    private void UpdateScores()
    {
        utilityScores[(int)Behaviour.GoNearPlanet] = 5;
        utilityScores[(int)Behaviour.Wander] = 6;
        utilityScores[(int)Behaviour.Flee] = 5;
        utilityScores[(int)Behaviour.Attack] = 5;
        utilityScores[(int)Behaviour.Destroy] = 5;
        steeringBasics.maxVelocity = 5;
        if (sensorComponent.objects.Contains(sun))
        {
            sensorComponent.objects.TryGetValue(sun, out objectInRange);
            utilityScores[(int)Behaviour.Flee] = 10;
        }
        else if ((sensorComponent.targets.Contains(protossRef) && sensorComponent.targets.Contains(terranRef))&&health < 35)
        {
            if(!sensorComponent.targets.TryGetValue(protossRef, out target))
                sensorComponent.targets.TryGetValue(terranRef, out target);
            utilityScores[(int)Behaviour.Flee] = 12;
        }
        else if (sensorComponent.targets.Contains(protossRef)|| sensorComponent.targets.Contains(terranRef))
        {
            if (!sensorComponent.targets.TryGetValue(protossRef, out target))
                sensorComponent.targets.TryGetValue(terranRef, out target);
            utilityScores[(int)Behaviour.Attack] = 10;
        }
        else if (firePower > 70)
        {
                utilityScores[(int)Behaviour.Destroy] = 15;
        }
        else if(sensorComponent.objects.Contains(planet))
        {
            utilityScores[(int)Behaviour.GoNearPlanet] = (int)System.Math.Ceiling((double)300 / health + 300 / firePower);
            utilityScores[(int)Behaviour.Wander] = 6;
            
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
        Debug.Log("zerg"+currentBehaviour);
        currentBehaviourTree.Start();
    }


    private Root SelectBehaviourTree(Behaviour behaviour)
    {
        switch (behaviour)
        {
            case Behaviour.Wander:
                //Debug.Log("Wander");
                return WanderBehaviour();

            case Behaviour.GoNearPlanet:
                //Debug.Log("soak in sun");
                return SoakInPlanetBehaviour();

            case Behaviour.Attack:
                //Debug.Log("attack");
                return AttackBehaviour();

            case Behaviour.Flee:
                //Debug.Log("flee");
                return FleeBehaviour();

            case Behaviour.Destroy:
                return DestroyPlanetBehaviour();
                    
            
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
        Vector3 accel = steeringBasics.Seek(targetToFlyBy.transform.position-(dirToMove*5));
        if (wallAvoidanceRef.GetSteering().magnitude > 0.005)
        {
            accel = wallAvoidanceRef.GetSteering();
        }
        steeringBasics.Steer(accel);
        steeringBasics.LookAtDirection(dirToMove);
    }    

    public void Attack(GameObject toShoot)
    {
        Vector3 gunLookAt = toShoot.transform.position - gun.transform.position;
        Quaternion rotation = Quaternion.LookRotation(gunLookAt, Vector3.up);
        gun.transform.rotation = rotation;
        fireElapsedTime += Time.deltaTime;
        
        if (fireElapsedTime>fireDelay)
        {
            gun.GetComponent<Gun>().Fire((int)firePower);
            
            fireElapsedTime = 0;
        }
    }

    public void FleeBack(GameObject fleeFrom)
    {
        steeringBasics.maxVelocity = 10;
        Vector3 accel = fleeComponent.GetSteering(fleeFrom.transform.position);
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
        planetDist = (int)Vector3.Distance(gameObject.transform.position, planet.transform.position);
        planetDist = Mathf.Abs(planetDist);
        if (planetDist < sunSeekDist)
        {
            SoakElapsed += Time.deltaTime;
            if (SoakElapsed > 1.5)
            {
                UpdateHealth(2);
                UpdateFirePower(4);
                SoakElapsed = 0;
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

    private Root SoakInPlanetBehaviour()
    {
        /*return new Root(new Service(2.0f, UpdateSunDist,
            new Selector(SeekSun(), new Action(()=>Wander()))));*/
        return new Root(new Sequence(new Action(() => Seek(planet)),new Action(()=>GatherReward())));
    }

    private Root AttackBehaviour()
    {
        return new Root(new Sequence(new Action(() => FlyBy(target.gameObject)), new Action(() => Attack(target.gameObject))));
    }

    private Root FleeBehaviour()
    {
        return new Root(new Action(() => FleeBack(sun)));
    }

    private Root DestroyPlanetBehaviour()
    {
        return new Root(new Sequence(new Action(() => FlyBy(planet)), new Action(() => Attack(planet))));
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

            if ((int)currentBehaviour != maxIndex)
            {
                currentBehaviour = (Behaviour)maxIndex;
                SwitchTree(SelectBehaviourTree(currentBehaviour));

            }
            utilityUpdateElapsed = 0;
        }
        firePower = firePower + (float)(Time.deltaTime * 0.5);
    }

    public void UpdateHealth(int alterAmount)
    {
        health += alterAmount;
        if (health <= 0)
            gameManager.KillPlayer(gameObject);
        else if (health > 70)
            health = 70;
    }

    public void UpdateFirePower(int alterAmount)
    {
        firePower += alterAmount;
        if (firePower <= 0)
            firePower = 0;
        
    }
}
