using UnityEngine;
using UnityEngine.SceneManagement;
public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance { get; private set; }
    public GameObject menuCanvas;
    public GameObject gameCanvas;
    public GameObject eggCanvas;
    public GameLogicController gameLogicController;
    
    private const string HasStartedKey = "HasStartedGame";
    private const string IsPetCreatedKey = "IsPetCreated";

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
        CheckGameStart();
    }

    private void CheckGameStart()
    {
        if (PlayerPrefs.GetInt(HasStartedKey, 0) == 1)
        {
            ActivateGame();
        }
        else
        {
            ActivateMenu();
        }
    }


    public void ActivateGame()
    {
        Debug.Log("Activating Game Canvas");
        eggCanvas.SetActive(false);
        menuCanvas.SetActive(false);
        gameCanvas.SetActive(true);
        gameLogicController.StartGame();
    }

    public void ActivateMenu()
    {
        Debug.Log("Activating Menu Canvas");
        gameLogicController.StopGame();
        eggCanvas.SetActive(false);
        gameCanvas.SetActive(false);
        menuCanvas.SetActive(true);
    }

    public void StartEggSequence()
    {
        menuCanvas.SetActive(false);
        eggCanvas.SetActive(true);
    }

    // Call this method when the player presses "Play Now"
    public void StartGame()
    {
        Debug.Log("Starting Game");
        ActivateGame();
        PlayerPrefs.SetInt(HasStartedKey, 1);
        PlayerPrefs.Save();
    }

    public void ResetGame()
    {
        Debug.Log("Resetting Game");
        PlayerPrefs.DeleteAll();
        PlayerPrefs.SetInt(HasStartedKey, 0);
        PlayerPrefs.SetInt(IsPetCreatedKey, 0);
        PlayerPrefs.Save();
        CheckGameStart();
    }
}
