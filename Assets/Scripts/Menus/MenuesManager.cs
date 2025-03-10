using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuesManager : MonoBehaviour
{
    [SerializeField] private Canvas start;
    [SerializeField] private Canvas pause;
    [SerializeField] private Canvas death;
    [SerializeField] private bool _started = false;
    [SerializeField] private bool _paused = false;
    [SerializeField] private bool _died = false;

    // menus btns behaviour
    public void startGame()
    {
        Time.timeScale = 1f;
        start.gameObject.SetActive(false);
        _started = true;
    }

    public void pauseGame()
    {
        Time.timeScale = 0f;
        pause.gameObject.SetActive(true);
        _paused = true;
    }

    public void continueGame()
    {
        Time.timeScale = 1f;
        pause.gameObject.SetActive(false);
        _paused = false;
    }

    public void restartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void endGame()
    {
        _died = true;
        death.gameObject.SetActive(true);
    }

    public bool Paused {
        get { return _paused; }
    }

    public bool Died {
        get { return _died; }
    }

    public bool Started {
        get { return _started; }
    }
}
