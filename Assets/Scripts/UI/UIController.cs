using Anglerfish;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;

namespace UI
{
    public class UIController : MonoBehaviour
    {
        [Header("Game")]
        [SerializeField] GameObject gameUIPanel;
        [SerializeField] Text timeText;
        [SerializeField] Image dashCover;
        [SerializeField] Image lightCover;
        [SerializeField] Image fishEatenBar;
        
        [Header("Game Complete")]
        [SerializeField] GameObject gameCompletePanel;

        [SerializeField] CanvasGroup gameCompleteCanvasGroup;
        [SerializeField] Text timeValueText;

        [Inject] GameManager _gameManager;
        [Inject] AnglerfishController _anglerfish;

        bool _gameComplete;

        public void Restart()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        public void Quit()
        {
            Application.Quit();
        }

        public void Menu()
        {
            SceneManager.LoadScene("Menu");
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
            timeText.text = Utils.FormatTime(_gameManager.CurrentTime);
            dashCover.fillAmount = _anglerfish.DashRemainingCooldown / _anglerfish.DashCooldown;
            lightCover.fillAmount = 1 - _anglerfish.LightEnergy / _anglerfish.MaxLightEnergy;
            fishEatenBar.fillAmount = (float) _gameManager.CurrentFish / GameManager.FishLimit;

            if (_gameComplete)
            {
                gameCompleteCanvasGroup.alpha = Mathf.Lerp(gameCompleteCanvasGroup.alpha, 1, 2 * Time.deltaTime);
            }
        }

        void OnGameFinished(float time)
        {
            timeValueText.text = Utils.FormatTime(time);
            
            gameUIPanel.SetActive(false);
            gameCompletePanel.SetActive(true);
            _gameComplete = true;
        }
    }
}