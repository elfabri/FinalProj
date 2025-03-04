using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuesManager : MonoBehaviour
{
    [SerializeField] private Canvas start;
    [SerializeField] private Canvas pause;
    [SerializeField] private Canvas death;
    [SerializeField] private bool m_Started = false;
    [SerializeField] private bool m_Paused = false;
    [SerializeField] private bool m_Died = false;

    void Update()
    {
        if (!m_Started)
        {
            if (Input.GetKey(KeyCode.Return))
            {
                startGame();
            } else {
                return;
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape) && !m_Died)
        {
            if (m_Paused)
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
        m_Started = true;
    }

    void pauseGame()
    {
        Time.timeScale = 0f;
        pause.gameObject.SetActive(true);
        m_Paused = true;
    }

    public void continueGame()
    {
        Time.timeScale = 1f;
        pause.gameObject.SetActive(false);
        m_Paused = false;
    }

    public void restartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void endGame()
    {
        m_Died = true;
        death.gameObject.SetActive(true);
    }

    public bool Paused {
        get { return m_Paused; }
    }

    public bool Died {
        get { return m_Died; }
    }

    public bool Started {
        get { return m_Started; }
    }
}
