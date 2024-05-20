using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;
public class GameController : Singleton<GameController>
{
    [SerializeField] private GameObject playerPrefab;
    private GameSettings gameSettings;
    public PlayerController mainPlayerController { get; private set; }
    [SerializeField] private Transform startPoint;
    [SerializeField] private Transform endPoint;
    [SerializeField] private AudioClip winSfx;
    [SerializeField] private AudioClip winSoldierSfx;
    [SerializeField] private AudioClip loseSfx;
    [SerializeField] private GameObject prefabItemHealing;
    private AudioSource audioSource;

    public Vector3 endPointPosition
    {
        get
        {
            return endPoint.position;
        }
    }
    [SerializeField] private Transform parentEndPoint;
    [SerializeField] private ParticleSystem playerSpawnEffect;
    [SerializeField] private ParticleSystem endPointEffect;
    private List<PlayerController> enemies = new List<PlayerController>();
    public GameState gameState { get; private set; } = GameState.Loading;
    private float timeleftCounter;
    private int curWave;
    private bool bossFight = false;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }
    public void StartGame(GameSettings gameSettings)
    {
        this.gameSettings = gameSettings;
        SetY();
        bossFight = false;
        gameState = GameState.Ready;
        UIManager.Instance.ShowGameplayMenu();
        StartCoroutine(RoutineStartGame());
    }

    public void PauseGame()
    {
        gameState = GameState.Paused;
    }
    public void ResumeGame()
    {
        gameState = GameState.Gameplay;
    }

    private void SetY()
    {
        RaycastHit hit;

        if (Physics.Raycast(startPoint.transform.position + Vector3.up * 100f, Vector3.down, out hit, Mathf.Infinity))
        {
            Vector3 hitPoint = startPoint.position;
            hitPoint.y = hit.point.y;
            startPoint.position = hitPoint;
        }

        if (Physics.Raycast(endPoint.transform.position + Vector3.up * 100f, Vector3.down, out hit, Mathf.Infinity))
        {
            Vector3 hitPoint = endPoint.position;
            hitPoint.y = hit.point.y;
            endPoint.position = hitPoint;
        }

        foreach (Transform child in parentEndPoint)
        {
            if (Physics.Raycast(child.transform.position + Vector3.up * 100f, Vector3.down, out hit, Mathf.Infinity))
            {
                Vector3 hitPoint = child.position;
                hitPoint.y = hit.point.y;
                child.position = hitPoint;
            }
        }

    }
    IEnumerator RoutineStartGame()
    {
        curWave = 0;
        yield return new WaitForSeconds(1f);
        gameState = GameState.Gameplay;
        timeleftCounter = gameSettings.gameDuration;
        UIManager.Instance.uIGameplayMenu.ShowPlayerStats();
        SpawnPlayer();
        StartNewWave();
    }

    private void StartNewWave()
    {
        curWave++;
        enemies = new List<PlayerController>();
        CameraController.Instance.ForceMove(new Vector3(startPoint.position.x, CameraController.Instance.transform.position.y, CameraController.Instance.transform.position.z));
        mainPlayerController.SetToStartPoint(startPoint.position);
        playerSpawnEffect.Play();
        int endPointIndex = Mathf.Clamp(curWave - 1, 0, parentEndPoint.childCount - 1);
        Transform pickEndpoint = parentEndPoint.GetChild(endPointIndex);
        endPoint.transform.position = pickEndpoint.transform.position;
        int totalEnemy = gameSettings.enemiesPerWave + ((curWave - 1) * gameSettings.enemiesGrowUpPerWave);
        for (int i = 0; i < totalEnemy; i++)
        {
            SpawnEnemy(gameSettings.GetEnemy());
        }

        RespawnItemHealing();
    }

    private void RespawnItemHealing()
    {
        PoolingManager.Instance.DisablePool(prefabItemHealing);
        Vector3 spawnPosition = endPoint.transform.position + Vector3.left * 6;
        RaycastHit[] hits;
        hits = Physics.RaycastAll(spawnPosition + Vector3.up * 100f, Vector3.down, Mathf.Infinity);

        for (int i = 0; i < hits.Length; i++)
        {
            RaycastHit hit = hits[i];
            if (hit.collider.gameObject.tag.ToLower() == "ground" && hit.point.y > spawnPosition.y)
            {
                spawnPosition.y = hit.point.y;

            }

        }
        ItemHealing itemHealing = PoolingManager.Instance.GetGameObject<ItemHealing>(prefabItemHealing);
        itemHealing.transform.position = spawnPosition;
        itemHealing.gameObject.SetActive(true);
    }

    private void StartBossFight()
    {
        if (bossFight) return;
        UIManager.Instance.uIGameplayMenu.ShowBossFight();
        bossFight = true;
        SpawnEnemy(gameSettings.GetBoss());
    }
    private void EndWave()
    {
        if (GetActiveEnemies(mainPlayerController.teamId).Count > 0 || bossFight) return;
        StartCoroutine(RoutineEndWave());
        endPointEffect.Play();

    }

    private IEnumerator RoutineEndWave()
    {
        mainPlayerController.SetToStartPoint(startPoint.position);
        CameraController.Instance.cinemachineVirtualCamera.enabled = false;
        yield return new WaitForSeconds(1f);
        CameraController.Instance.cinemachineVirtualCamera.enabled = true;
        StartNewWave();
    }
    private void SpawnPlayer()
    {
        if (mainPlayerController == null)
            mainPlayerController = PoolingManager.Instance.GetGameObject<PlayerController>(playerPrefab);
        mainPlayerController.gameObject.SetActive(true);
        CameraController.Instance.SetTarget(mainPlayerController.transform);
        mainPlayerController.InitPlayer(GameManager.Instance.itemDatabase.CurrentWeapon, GameManager.Instance.itemDatabase.CurrentSoldier);
        mainPlayerController.GetComponent<UserInput>().Init();

    }

    public void EndGame()
    {
        ResumeGame();
        mainPlayerController.health.Hit(10000f);
    }

    private void Update()
    {
        if (gameState == GameState.Gameplay)
        {
            if (GameManager.godmod)
            {
                mainPlayerController.characterHandleWeapon.mainWeapon.damageCaused = 10000;
                mainPlayerController.characterHandleWeapon.secondWeapon.damageCaused = 10000;
            }

            timeleftCounter -= Time.deltaTime;
            if (timeleftCounter <= 0) timeleftCounter = 0;
            UIManager.Instance.uIGameplayMenu.SetTimeLeft(timeleftCounter);
            if (mainPlayerController.curCharacterCondition == PlayerController.CharacterConditions.Dead)
            {
                EndGame(false);
            }

            if (timeleftCounter <= 0)
            {
                StartBossFight();
            }

            if (Vector3.Distance(mainPlayerController.transform.position, endPoint.transform.position) <= 1f)
            {
                EndWave();
            }

            if (bossFight && GetActiveEnemies(mainPlayerController.teamId).Count <= 0)
            {
                EndGame(true);
            }
        }
    }

    private void EndGame(bool win)
    {
        StartCoroutine(RoutineEndGame(win));
    }
    int stars;
    int coins;
    private IEnumerator RoutineEndGame(bool win)
    {
        gameState = GameState.End;

        yield return StartCoroutine(CaculateStarsAndCoins(win));
        AudioManager.Instance.StopMainMusic();
        yield return new WaitForSeconds(3f);
        if (win) audioSource.PlayOneShot(winSfx);
        else audioSource.PlayOneShot(loseSfx);
        UIManager.Instance.HideGameplayMenu();
        UIManager.Instance.ShowResultMenu(win, 100);
        yield return new WaitForSeconds(2f);
        if (win) audioSource.PlayOneShot(winSoldierSfx);
        UIManager.Instance.ShowStarsPopup(stars);
    }

    public IEnumerator CaculateStarsAndCoins(bool win)
    {
        UIManager.Instance.ShowQuickLoadingScreen();
        stars = 0;
        coins = 0;
        float healthPercentage = mainPlayerController.health.currentHealth / mainPlayerController.characterProperties.maxHealth;
        if (win)
        {
            if (healthPercentage <= 0.2f)
            {
                stars = 1;
                coins = GameManager.Instance.generalSettings.oneStarCoins;
            }
            else if (healthPercentage <= 0.8f)
            {
                stars = 2;
                coins = GameManager.Instance.generalSettings.twoStarCoins;
            }
            else
            {
                stars = 3;
                coins = GameManager.Instance.generalSettings.threeStarCoins;
            }
        }
        else
        {
            if (timeleftCounter <= gameSettings.gameDuration * 0.6f)
            {
                coins = GameManager.Instance.generalSettings.zeroBossFightStarCoins;
            }
            else
            {
                coins = GameManager.Instance.generalSettings.zeroStarCoins;
            }
        }

        if (coins > 0)
        {
            BodyPlayGame bodyPlayGame = new BodyPlayGame();
            bodyPlayGame.score = coins;
            bodyPlayGame.star = stars;
            bodyPlayGame.gameSessionId = GameManager.Instance.playGameData.id;
            bodyPlayGame.gameId = APIHelper.gameId;
            bodyPlayGame.map = 1;
            bool success = false;
            if (GameManager.Instance.generalSettings.useAPI)
            {
                GameServices.Instance.PostGameResult(bodyPlayGame, (data) =>
                {
                    success = true;
                    UIManager.Instance.HideQuickLoadingScreen();
                }, (err) =>
                {
                    success = true;
                    UIManager.Instance.HideQuickLoadingScreen();
                });
            }
            else
            {
                UIManager.Instance.HideQuickLoadingScreen();
                success = true;
            }
            while (!success)
                yield return null;
        }
    }

    public List<PlayerController> GetActiveEnemies(int ownerTeamId)
    {
        if (ownerTeamId == mainPlayerController.teamId)
        {
            List<PlayerController> enemiesAlive = new List<PlayerController>();
            foreach (PlayerController player in enemies)
            {
                if (player.isActiveAndEnabled && player.curCharacterCondition != PlayerController.CharacterConditions.Dead)
                    enemiesAlive.Add(player);
            }
            enemies = enemiesAlive;
            return enemies;
        }
        else
        {
            return new List<PlayerController>() { mainPlayerController };
        }
    }
    public void SpawnEnemy(GameObject enemyPrefab)
    {
        BotController enemy = PoolingManager.Instance.GetGameObject<BotController>(enemyPrefab);
        Vector3 spawnPosition = Vector3.up;
        spawnPosition.x = Random.Range(mainPlayerController.transform.position.x + 10, endPoint.position.x);
        spawnPosition.x = Mathf.Clamp(spawnPosition.x, startPoint.position.x, endPoint.position.x);
        RaycastHit[] hits;
        hits = Physics.RaycastAll(spawnPosition + Vector3.up * 1000f, Vector3.down, Mathf.Infinity);
        for (int i = 0; i < hits.Length; i++)
        {
            RaycastHit hit = hits[i];
            if (hit.collider.gameObject.tag.ToLower() == "ground" && hit.point.y > spawnPosition.y)
            {
                spawnPosition.y = hit.point.y + 8f;
                break;
            }

        }
        enemy.Init(spawnPosition);

        enemies.Add(enemy.owner);
    }



}


public enum GameState
{
    Ready, Gameplay, Paused, End, Loading
}