using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Menu
{
    public class MenuController : MonoBehaviour
    {
        [SerializeField] GameObject mainMenu;
        [SerializeField] GameObject credits;
        [SerializeField] Text bestTimeText;

        void Start()
        {
            var bestTime = Utils.GetBestTime();
            bestTimeText.text = bestTime > 0 ? Utils.FormatTime(bestTime) : "-";
        }

        public void OpenMainMenu()
        {
            mainMenu.SetActive(true);
            credits.SetActive(false);
        }

        public void OpenCredits()
        {
            mainMenu.SetActive(false);
            credits.SetActive(true);
        }

        public void QuitGame()
        {
            Application.Quit();
        }

        public void Play()
        {
            SceneManager.LoadScene("Main");
        }
    }
}
