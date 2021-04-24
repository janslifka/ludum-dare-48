using System;
using Anglerfish;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using Random = UnityEngine.Random;

namespace Fish
{
    public class FishController : MonoBehaviour
    {
        enum FishState
        {
            Default = 1,
            Fleeing = 2,
            Lured = 3,
            LoosingInterest = 4
        }

        [SerializeField] SpriteRenderer image;

        [SerializeField] float movementSpeed;
        [SerializeField] float movementSpeedDev;

        [SerializeField] float fleeingSpeed;
        [SerializeField] float fleeingTime;

        [Inject] AnglerfishController _anglerfish;

        public event Action<FishController> OnEaten;
        public event Action<FishController> OnDespawn;

        FishState _state = FishState.Default;
        Rigidbody2D _rigidbody;

        float _currentSpeed;
        Vector2 _currentDirection;
        
        void Initialize(Vector3 position, Vector2 direction)
        {
            transform.position = position;

            _currentDirection = direction;
            _currentSpeed = Random.Range(-movementSpeedDev, movementSpeed) + movementSpeed;

            _rigidbody = GetComponent<Rigidbody2D>();
            _rigidbody.velocity = _currentDirection * _currentSpeed;

            _anglerfish.OnFishEaten += OnFishEaten;
            
            image.color = Color.white;
            CancelInvoke();
        }

        void OnFishEaten()
        {
            var distance = Vector3.Distance(transform.position, _anglerfish.transform.position);
            
            Debug.Log($"Fish eaten, distance: {distance}, radius: {_anglerfish.EatingAreaRadius}");
            
            if (distance <= _anglerfish.EatingAreaRadius)
            {
                Flee();
            }
        }

        void Flee()
        {
            Debug.Log("THIS FISH FLEE");
            image.color = Color.red;
            var fleeDirection = (transform.position - _anglerfish.transform.position).normalized;
            _rigidbody.velocity = fleeDirection * fleeingSpeed;

            CancelInvoke();
            Invoke(nameof(StopFleeing), fleeingTime);
        }

        void StopFleeing()
        {
            _rigidbody.velocity = _currentDirection * _currentSpeed;
            image.color = Color.white;
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