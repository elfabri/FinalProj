using UnityEngine;

public class MenuesManager : MonoBehaviour
{
    [SerializeField] private Canvas start;
    [SerializeField] private Canvas pause;
    [SerializeField] private Canvas death;
    [SerializeField] public bool m_Started = false;
    [SerializeField] public bool m_Paused = false;

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

        if (Input.GetKeyDown(KeyCode.Escape))
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

    void restartGame()
    {

    }
}
