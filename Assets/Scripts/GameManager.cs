using System;
using Anglerfish;
using UnityEngine;
using Zenject;

public class GameManager : ITickable
{
    const int FishLimit = 20;
    
    [Inject] AnglerfishController _anglerfish;
    
    int _currentFish = 0;
    float _currentTime = 0;

    public event Action<float> OnGameFinished;

    public int CurrentFish => _currentFish;
    public float CurrentTime => _currentTime;
    
    public void Tick()
    {
        _currentTime += Time.deltaTime;
    }

    public void FishEaten()
    {
        _currentFish++;
        _anglerfish.ResetDash();

        if (_currentFish == FishLimit)
        {
            OnGameFinished?.Invoke(_currentTime);
        }
    }
}
