using UnityEngine;
using System.Collections.Generic;

namespace UnityMovementAI
{
    public class ZergSensor : MonoBehaviour
    {
        HashSet<MovementAIRigidbody> _targets = new HashSet<MovementAIRigidbody>();
        HashSet<GameObject> _objects = new HashSet<GameObject>();

        public HashSet<MovementAIRigidbody> targets
        {
            get
            {
                /* Remove any MovementAIRigidbodies that have been destroyed */
                _targets.RemoveWhere(IsNull);
                return _targets;
            }
        }

        public HashSet<GameObject> objects
        {
            get
            {
                /* Remove any MovementAIRigidbodies that have been destroyed */
                _objects.RemoveWhere(IsNull);
                return _objects;
            }
        }

        static bool IsNull(MovementAIRigidbody r)
        {
            return (r == null || r.Equals(null));
        }

        static bool IsNull(GameObject r)
        {
            return (r == null || r.Equals(null));
        }

        void TryToAdd(Component other)
        {
            MovementAIRigidbody rb = other.GetComponent<MovementAIRigidbody>();
            if (rb != null)
            {
                _targets.Add(rb);
            }
            else if (other.gameObject.tag == "objects")
            {
                _objects.Add(other.gameObject);
            }
        }

        void TryToRemove(Component other)
        {
            MovementAIRigidbody rb = other.GetComponent<MovementAIRigidbody>();
            if (rb != null)
            {
                _targets.Remove(rb);
            }
            else if (other.gameObject.tag == "objects")
            {
                _objects.Remove(other.gameObject);
            }
        }

        void OnTriggerEnter(Collider other)
        {
            TryToAdd(other);
        }

        void OnTriggerExit(Collider other)
        {
            TryToRemove(other);
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            TryToAdd(other);
        }

        void OnTriggerExit2D(Collider2D other)
        {
            TryToRemove(other);
        }
    }
}