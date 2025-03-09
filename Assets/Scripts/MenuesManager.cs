using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuesManager : MonoBehaviour
{
    [SerializeField] private Canvas start;
    [SerializeField] private Canvas pause;
    [SerializeField] private Canvas death;
    [SerializeField] private bool _Started = false;
    [SerializeField] private bool _Paused = false;
    [SerializeField] private bool _Died = false;

    void Update()
    {
        if (!_Started)
        {
            if (Input.GetKey(KeyCode.Return))
            {
                startGame();
            } else {
                return;
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape) && !_Died)
        {
            if (_Paused)
            {
                continueGame();
            } else {
                pauseGame();
            }
        }
    }

    // menus btns behaviour
    public void startGame()
    {
        Time.timeScale = 1f;
        start.gameObject.SetActive(false);
        _Started = true;
    }

    void pauseGame()
    {
        Time.timeScale = 0f;
        pause.gameObject.SetActive(true);
        _Paused = true;
    }

    public void continueGame()
    {
        Time.timeScale = 1f;
        pause.gameObject.SetActive(false);
        _Paused = false;
    }

    public void restartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void endGame()
    {
        _Died = true;
        death.gameObject.SetActive(true);
    }

    public bool Paused {
        get { return _Paused; }
    }

    public bool Died {
        get { return _Died; }
    }

    public bool Started {
        get { return _Started; }
    }
}
