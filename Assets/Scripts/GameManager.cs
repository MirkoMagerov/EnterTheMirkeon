using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public GameObject player;
    public GameObject shopControllerPrefab;
    public List<ShopItem> globalShopItems;
    public GameObject dungeonCamera;
    public DungeonGenerator dungeonGenerator;
    public GameObject pauseCanvas;
    public GameObject deathCanvas;
    public bool gamePaused = false;
    private bool playerDead = false;
    private MusicManager musicManager;

    [SerializeField] private Animator animator;
    [SerializeField] private GameObject panel;
    [SerializeField] private GameObject playerPrefab;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            DontDestroyOnLoad(dungeonCamera);
            player = Instantiate(playerPrefab);
            InputManager.Instance.GetInputActions().Disable();
            DontDestroyOnLoad(player);
            player.SetActive(false);
            musicManager = GetComponent<MusicManager>();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && SceneManager.GetActiveScene().name == "Dungeon" && !playerDead)
        {
            if (Time.timeScale == 0f)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void SpawnPlayerInFirstRoom(Vector3 spawnPoint)
    {
        player.SetActive(true);
        player.transform.position = spawnPoint;
        InputManager.Instance.GetInputActions().Enable();
        dungeonCamera.SetActive(true);
    }

    public void HealPlayer(int healAmount)
    {
        PlayerLife playerLife = player.GetComponent<PlayerLife>();
        playerLife.RegenerateHealth(healAmount);
    }

    public void IncreaseMaximumHealth(int maxHealthIncrease)
    {
        PlayerLife playerLife = player.GetComponent<PlayerLife>();
        playerLife.AddHearts(maxHealthIncrease);
    }

    public void EquipWeapon(Weapon weapon)
    {
        player.GetComponentInChildren<WeaponInventory>().PickUpWeapon(weapon);
    }

    public void StartGame()
    {
        ChangeSceneWithTransition("Dungeon", () =>
        {
            dungeonGenerator.GenerateDungeon();
        });
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void PauseGame()
    {
        musicManager.PauseMusic();
        gamePaused = true;
        Time.timeScale = 0f;
        pauseCanvas.SetActive(true);
        InputManager.Instance.GetInputActions().Disable();
        Cursor.visible = true;
    }

    public void ResumeGame()
    {
        musicManager.UnPauseMusic();
        gamePaused = false;
        Time.timeScale = 1f;
        pauseCanvas.SetActive(false);
        Cursor.visible = false;
        InputManager.Instance.GetInputActions().Enable();
    }

    public void RestartDungeon()
    {
        foreach (Transform child in dungeonGenerator.transform) { Destroy(child.gameObject); }

        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemies) { Destroy(enemy); }

        dungeonGenerator.GenerateDungeon();
    }

    public void ActivateDeathCanvas()
    {
        playerDead = true;
        InputManager.Instance.GetInputActions().Disable();
        Cursor.visible = true;
        gamePaused = true;
        deathCanvas.SetActive(true);
    }

    public void RestartAfterDeath()
    {
        playerDead = false;
        player = Instantiate(playerPrefab);
        player.SetActive(true);
        InputManager.Instance.GetInputActions().Enable();
        Cursor.visible = false;
        deathCanvas.SetActive(false);
        gamePaused = false;

        RestartDungeon();
        dungeonCamera.GetComponent<CameraController>().EnableCameraFollowing();
    }

    public void ChangeSceneWithTransition(string sceneName, System.Action onSceneLoaded = null)
    {
        StartCoroutine(LoadScene(sceneName, onSceneLoaded));
    }

    private IEnumerator LoadScene(string sceneName, System.Action onSceneLoaded)
    {
        panel.SetActive(true);
        animator.SetTrigger("end");
        yield return new WaitForSeconds(1.5f);

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        animator.Play("Start");
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        onSceneLoaded?.Invoke();

        yield return new WaitForSeconds(0.5f);
        panel.SetActive(false);
    }

    public void AfterBossDefeath()
    {
        foreach (Transform child in dungeonGenerator.transform) { Destroy(child.gameObject); }
        dungeonCamera.SetActive(false);
        InputManager.Instance.GetInputActions().Disable();
        player.SetActive(false);
        Cursor.visible = true;
    }

    public void PlayBossMusic()
    {
        musicManager.PlayBossMusic();
    }

    public void StopBossMusic()
    {
        musicManager.StopBossMusic();
    }
}
