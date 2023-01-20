using System;
using System.Threading.Tasks;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;
using UnityEngine;
using UnityEngine.AI;
using GCharacter = GameCreator.Runtime.Characters.Character;

namespace ClaraMundi
{
    public class NPCMoveController : NetworkBehaviour
    {
        public NavMeshAgent Agent;
        public bool canMove = true;
        public bool readyToMove = false;
        public SphereCollider area;
        public GCharacter character;
        public float moveInterval = 5f;
        [SyncVar] public float movementSpeed = 2;

        public bool spawnIsOrigin = true;
        private Vector3 origin;
        [SyncVar] private Vector3 destination;
        private Vector3 lastDestination;

        private Transform t;

        private void Awake()
        {
            t = transform;
            origin = t.position;
        }

        public override void OnStartServer()
        {
            base.OnStartServer();
            readyToMove = true;
        }

        public void SetDestination(Vector3 dest)
        {
            destination = dest;
        }

        private void Update()
        {
            if (IsServer && canMove)
            {
                if (!spawnIsOrigin)
                    origin = t.position;
                if (readyToMove)
                    MoveToNewDestination();
            }

            if (destination == Vector3.zero) return;
            if (lastDestination == destination) return;
            character.Motion.MoveToLocation(new Location(destination), 0.0f, OnReachedDestination, 1);
            lastDestination = destination;
        }

        private void MoveToNewDestination()
        {
            if (!gameObject.activeInHierarchy) return;
            if (!isActiveAndEnabled) return;
            readyToMove = false;
            var nextDestination = RandomNavSphere(origin, area.radius, LayerMask.GetMask("Default"));
            if (nextDestination != Vector3.zero)
                destination = nextDestination;
        }

        private async void OnReachedDestination(GCharacter _character)
        {
            if (!gameObject.activeInHierarchy) return;
            if (!isActiveAndEnabled) return;
            if (!IsServer) return;
            await Task.Delay((int)(moveInterval * 1000));
            readyToMove = true;
        }

        public static Vector3 RandomNavSphere(Vector3 origin, float distance, int layermask)
        {
            Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * distance;

            randomDirection += origin;

            NavMeshHit navHit;

            NavMesh.SamplePosition(randomDirection, out navHit, distance, layermask);

            return navHit.position;
        }
    }
}