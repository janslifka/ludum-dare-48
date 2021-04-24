using System;
using Anglerfish;
using UnityEngine;
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

        [SerializeField] float loosingInterestTime;

        [Inject] AnglerfishController _anglerfish;

        public event Action<FishController> OnEaten;
        public event Action<FishController> OnDespawn;

        FishState _state = FishState.Default;
        Rigidbody2D _rigidbody;

        float _currentSpeed;
        Vector2 _currentDirection;

        float AnglerFishDistance => Vector3.Distance(transform.position, _anglerfish.transform.position);
        
        bool InFeedingArea => AnglerFishDistance <= _anglerfish.EatingAreaRadius;

        bool InLureArea => AnglerFishDistance <= _anglerfish.LureAreaRadius;

        void Initialize(Vector3 position, Vector2 direction)
        {
            transform.position = position;

            _currentDirection = direction;
            _currentSpeed = Random.Range(-movementSpeedDev, movementSpeed) + movementSpeed;

            _rigidbody = GetComponent<Rigidbody2D>();
            _rigidbody.velocity = _currentDirection * _currentSpeed;

            _anglerfish.OnFishEaten += OnFishEaten;
            _anglerfish.OnLightOn += OnLightOn;
            _anglerfish.OnLightOff += OnLightOff;

            _state = FishState.Default;
//            image.color = Color.white;
            CancelInvoke();
        }

        void FixedUpdate()
        {
            if (_state == FishState.Lured)
            {
                var direction = (_anglerfish.transform.position - transform.position).normalized;
                _rigidbody.velocity = direction * _currentSpeed;
            }
        }

        void OnFishEaten()
        {
            if (InFeedingArea)
            {
                Flee();
            }
        }

        void Flee()
        {
//            image.color = Color.red;
            var fleeDirection = (transform.position - _anglerfish.transform.position).normalized;
            _rigidbody.velocity = fleeDirection * fleeingSpeed;
            _state = FishState.Fleeing;

            CancelInvoke();
            Invoke(nameof(BackToDefault), fleeingTime);
        }

        void BackToDefault()
        {
            _state = FishState.Default;
            _rigidbody.velocity = _currentDirection * _currentSpeed;
//            image.color = Color.white;
        }

        void OnLightOn()
        {
            if (InLureArea && _state != FishState.Fleeing)
            {
                _state = FishState.Lured;
//                image.color = Color.yellow;
            }
        }

        void OnLightOff()
        {
            if (_state == FishState.Lured)
            {
                _state = FishState.LoosingInterest;
                Invoke(nameof(BackToDefault), loosingInterestTime);
            }
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