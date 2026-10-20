
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public int score = 0;
    public int health = 3;
    public float timer = 0f;
    public float scoreBoostUntil = 0f;

    private bool paused = false;
    private string pendingScene = "";

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        // Load scenes outside OnGUI
        if (pendingScene != "")
        {
            string nextScene = pendingScene;
            pendingScene = "";

            SceneManager.LoadScene(
                nextScene,
                LoadSceneMode.Single
            );
            return;
        }

        string scene = SceneManager.GetActiveScene().name;

        // MAIN MENU
        if (scene == "MainMenu")
        {
            UnlockMouse();
            return;
        }

        // END SCREEN
        if (scene == "EndScene")
        {
            UnlockMouse();

            if (Input.GetKeyDown(KeyCode.R))
            {
                RestartGame();
            }

            return;
        }

        if (!scene.StartsWith("Level"))
            return;

        if (!paused)
            timer += Time.deltaTime;

        // Pause with Escape
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            paused = !paused;
            Time.timeScale = paused ? 0f : 1f;

            if (paused)
                UnlockMouse();
        }

        if (paused)
        {
            UnlockMouse();
        }

        // Restart level if player falls
        GameObject player =
            GameObject.FindGameObjectWithTag("Player");

        if (player != null &&
            player.transform.position.y < -5f)
        {
            SceneManager.LoadScene(scene);
        }
    }

    void UnlockMouse()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void AddScore(int amount)
    {
        if (Time.time < scoreBoostUntil)
            amount *= 2;

        score += amount;
    }

    public void TakeDamage()
    {
        health--;

        if (health <= 0)
        {
            health = 3;

            SceneManager.LoadScene(
                SceneManager.GetActiveScene().buildIndex
            );
        }
    }

    public void ResetGame()
    {
        score = 0;
        health = 3;
        timer = 0f;
        scoreBoostUntil = 0f;

        paused = false;
        Time.timeScale = 1f;
    }

    public void RestartGame()
    {
        Debug.Log("Restart clicked - loading Level1");

        ResetGame();
        UnlockMouse();

        pendingScene = "Level1";
    }

    void OnGUI()
    {
        string scene = SceneManager.GetActiveScene().name;

        GUIStyle title = new GUIStyle(GUI.skin.label);
        title.fontSize = 32;
        title.fontStyle = FontStyle.Bold;
        title.alignment = TextAnchor.MiddleCenter;
        title.normal.textColor = Color.white;

        float x = Screen.width / 2f - 125;
        float y = Screen.height / 2f;

        // MAIN MENU
        if (scene == "MainMenu")
        {
            GUI.Label(
                new Rect(x - 75, y - 150, 400, 60),
                "My Platformer Game",
                title
            );

            if (GUI.Button(
                new Rect(x, y - 40, 250, 50),
                "PLAY"))
            {
                Debug.Log("Play button clicked");

                ResetGame();
                pendingScene = "Level1";
            }

            if (GUI.Button(
                new Rect(x, y + 20, 250, 50),
                "QUIT"))
            {
                Application.Quit();
            }

            return;
        }

        // END SCREEN
        if (scene == "EndScene")
        {
            GUI.Label(
                new Rect(x - 75, y - 140, 400, 60),
                "GAME COMPLETE!",
                title
            );

            GUI.Label(
                new Rect(x, y - 50, 300, 50),
                "Final Score: " + score,
                title
            );

            if (GUI.Button(
                new Rect(x, y + 40, 250, 50),
                "RESTART"))
            {
                RestartGame();
            }

            GUI.Label(
                new Rect(x, y + 100, 250, 30),
                "Press R to restart"
            );

            return;
        }

        if (!scene.StartsWith("Level"))
            return;

        // GAME HUD
        GUIStyle hud = new GUIStyle(GUI.skin.label);
        hud.fontSize = 22;
        hud.normal.textColor = Color.white;

        GUI.Label(
            new Rect(Screen.width - 200, 10, 190, 40),
            "Score: " + score,
            hud
        );

        GUI.Label(
            new Rect(10, 10, 200, 35),
            "Health: " + health,
            hud
        );

        GUI.Label(
            new Rect(10, 45, 200, 35),
            "Time: " + Mathf.FloorToInt(timer),
            hud
        );

        if (GUI.Button(
            new Rect(10, 90, 90, 35),
            "Pause"))
        {
            paused = !paused;
            Time.timeScale = paused ? 0f : 1f;

            if (paused)
                UnlockMouse();
        }

        // PAUSE MENU
        if (paused)
        {
            GUI.Label(
                new Rect(x, y - 100, 250, 50),
                "PAUSED",
                title
            );

            if (GUI.Button(
                new Rect(x, y, 250, 50),
                "RESUME"))
            {
                paused = false;
                Time.timeScale = 1f;
            }

            if (GUI.Button(
                new Rect(x, y + 60, 250, 50),
                "MAIN MENU"))
            {
                ResetGame();
                UnlockMouse();
                pendingScene = "MainMenu";
            }
        }
    }
}
