using UnityEngine;
using UnityEngine.SceneManagement;

namespace Menu
{
    public class MenuController : MonoBehaviour
    {
        [SerializeField] GameObject mainMenu;
        [SerializeField] GameObject credits;

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
