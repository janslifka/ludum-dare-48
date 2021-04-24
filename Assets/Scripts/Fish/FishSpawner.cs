using Anglerfish;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

namespace Fish
{
    public class FishSpawner : MonoBehaviour
    {
        [SerializeField] int maxFish;

        [Inject] FishController.Factory _fishFactory;
        [Inject] AnglerfishController _anglerfish;
        [Inject] GameManager _gameManager;

        int _fishAlive;

        void Start()
        {
            EnsureFishCount();
        }

        void EnsureFishCount()
        {
            while (_fishAlive < maxFish)
            {
                SpawnFish();
            }
        }

        void OnFishEaten(FishController fishController)
        {
            _gameManager.FishEaten();
            DespawnFish(fishController);
        }

        void OnFishDespawn(FishController fishController)
        {
            DespawnFish(fishController);
        }

        void SpawnFish()
        {
            var (position, direction) = RandomFishPosition();
            var fish = _fishFactory.Spawn(position, direction);

            Bind(fish);

            _fishAlive++;
        }

        (Vector3, Vector2) RandomFishPosition()
        {
            var unitCirclePoint = Random.insideUnitCircle.normalized;
            var center = (Vector2) _anglerfish.transform.position;
            var position = center + unitCirclePoint * _anglerfish.SpawnAreaRadius;

            var rotation = Quaternion.AngleAxis(Random.Range(-60, 60), Vector3.forward);
            var direction = rotation * -unitCirclePoint;
            return (position, direction);
        }

        void DespawnFish(FishController fish)
        {
            Unbind(fish);
            _fishFactory.Despawn(fish);
            _fishAlive--;
            EnsureFishCount();
        }

        void Bind(FishController fish)
        {
            fish.OnDespawn += OnFishDespawn;
            fish.OnEaten += OnFishEaten;
        }

        void Unbind(FishController fish)
        {
            fish.OnDespawn -= OnFishDespawn;
            fish.OnEaten -= OnFishEaten;
        }
    }
}