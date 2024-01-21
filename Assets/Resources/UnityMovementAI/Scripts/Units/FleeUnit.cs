using UnityEngine;

namespace UnityMovementAI
{
    public class FleeUnit : MonoBehaviour
    {
        public Transform target;

        SteeringBasics steeringBasics;
        Flee flee;
        WallAvoidance wallAvoidanceRef;

        void Start()
        {
            steeringBasics = GetComponent<SteeringBasics>();
            wallAvoidanceRef = GetComponent<WallAvoidance>();
            flee = GetComponent<Flee>();
        }

        void FixedUpdate()
        {
            
            Vector3 accel = flee.GetSteering(target.position);
            if (wallAvoidanceRef.GetSteering().magnitude >0.005)
            {
                accel = wallAvoidanceRef.GetSteering();
            }
            steeringBasics.Steer(accel);
            steeringBasics.LookWhereYoureGoing();
        }
    }
}