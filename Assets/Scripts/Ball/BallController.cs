using System;
using UnityEngine;

namespace BullBrukBruker
{
    public class BallController : MonoBehaviour, IPooled
    {
        [field: SerializeField] public ObjectID ID { get; private set; }
        [field: SerializeField] public BallMovingForward Moving { get; private set; }
        [field: SerializeField] public BallCollision Collision { get; private set; }
        [field: SerializeField] public ObjectDespawning Despawning { get; private set; }

        public Action<GameObject> ReleaseCallback { get; set; }

        protected virtual void OnEnable()
        {
            LevelManager.Instance.AddBall(gameObject);
        }

        protected virtual void OnDisable()
        {
            LevelManager.Instance.RemoveBall(gameObject);

            ReleaseCallback?.Invoke(gameObject);
        }

        public void Update()
        {
            Moving.Move();

            Collision.CalculateCellGroup();
            Collision.CalculateCollision();
        }
    }
}