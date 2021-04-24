using Anglerfish;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;

namespace UI
{
    public class UIController : MonoBehaviour
    {
        [SerializeField] Text fishEatenText;
        [SerializeField] Text timeText;
        [SerializeField] Text lightText;
        [SerializeField] GameObject dashIndicator;
        
        [SerializeField] GameObject gameCompletePanel;
        [SerializeField] Text timeValueText;

        [Inject] GameManager _gameManager;
        [Inject] AnglerfishController _anglerfish;

        public void Restart()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        
        void Start()
        {
            _gameManager.OnGameFinished += OnGameFinished;
        }

        void OnDestroy()
        {
            _gameManager.OnGameFinished -= OnGameFinished;
        }

        void Update()
        {
            dashIndicator.SetActive(_anglerfish.CanDash);
            timeText.text = $"Time: {_gameManager.CurrentTime}";
            lightText.text = $"Light: {_anglerfish.LightEnergy} / {_anglerfish.MaxLightEnergy}";
            fishEatenText.text = $"Fish eaten: {_gameManager.CurrentFish}";
        }

        void OnGameFinished(float time)
        {
            timeValueText.text = $"{time}";
            gameCompletePanel.SetActive(true);
        }
    }
}