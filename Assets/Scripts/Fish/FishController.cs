using System;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

namespace Fish
{
    public class FishController : MonoBehaviour
    {
        [SerializeField] float movementSpeed;
        [SerializeField] float movementSpeedDev;

        public event Action<FishController> OnEaten;
        public event Action<FishController> OnDespawn;

        Rigidbody2D _rigidbody;
        float _movementSpeed;
        Vector2 _defaultDirection;

        void Initialize(Vector3 position, Vector2 direction)
        {
            transform.position = position;

            _defaultDirection = direction;
            _movementSpeed = Random.Range(-movementSpeedDev, movementSpeed) + movementSpeed;

            _rigidbody = GetComponent<Rigidbody2D>();
            _rigidbody.velocity = _defaultDirection * _movementSpeed;
        }

        void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.CompareTag("Anglerfish"))
            {
                OnEaten?.Invoke(this);
            }
        }

        void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("SpawnArea"))
            {
                OnDespawn?.Invoke(this);
            }
        }

        public class Factory : MonoMemoryPool<Vector3, Vector2, FishController>
        {
            protected override void Reinitialize(Vector3 position, Vector2 direction, FishController item)
            {
                item.Initialize(position, direction);
            }
        }
    }
}