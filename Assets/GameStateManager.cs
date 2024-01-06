using UnityEngine;
public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance { get; private set; }
    public GameObject menuCanvas;
    public GameObject gameCanvas;
    public GameLogicController gameLogicController;
    private const string HasStartedKey = "HasStartedGame";

    void Awake()
    {
        // Singleton pattern to ensure only one instance of this object exists
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // To persist across scenes
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Check if the player has previously started the game
        if (PlayerPrefs.GetInt(HasStartedKey, 0) == 1)
        {
            // If they have, activate the game directly
            ActivateGame();
        }
        else
        {
            // Otherwise, start with the menu
            ActivateMenu();
        }
    }

    public void ActivateGame()
    {
        menuCanvas.SetActive(false);
        gameCanvas.SetActive(true);
        gameLogicController.StartGame();
    }

    public void ActivateMenu()
    {
        menuCanvas.SetActive(true);
        gameCanvas.SetActive(false);
        gameLogicController.StopGame();
    }

    // Call this method when the player presses "Play Now"
    public void StartGame()
    {
        ActivateGame();
        PlayerPrefs.SetInt(HasStartedKey, 1);
        PlayerPrefs.Save();
    }

     public void ResetGame()
    {
        // Additional reset logic here...
        PlayerPrefs.SetInt(HasStartedKey, 0);
        PlayerPrefs.Save();
        ActivateMenu();
    }
}
