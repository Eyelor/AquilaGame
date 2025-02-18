using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : Singleton<PlayerController>
{
    private Animator animator;             // Referencja do komponentu Animator postaci
    private CharacterController characterController;  // Referencja do komponentu CharacterController postaci
    private Camera mainCamera;             // Referencja do g��wnej kamery w scenie
    private StatisticsManager statisticsManager;

    [Header("Player Statistics")]
    [SerializeField] public PlayerStatistics statistics;

    [Header("Player Achievements")]
    [SerializeField] public PlayerAchievements achievements;
    public bool onNewIsland;

    [Header("Terrain Information")]
    [SerializeField] private Terrain terrain;
    [SerializeField] private float turnSpeed = 200f;         // Szybko�� obrotu postaci
    [SerializeField] private float heightMargin = 5f;        // Margines dodawany do mediany wysoko�ci terenu
    private float maxYPosition;            // Maksymalna dozwolona wysoko��, na kt�r� mo�e wej�� posta�

    private float lastCameraYaw;           // Ostatni obr�t kamery w osi Y
    private Vector3 defaultCenter;

    private bool isWalkingSoundPlaying = false;
    private bool isRunningSoundPlaying = false;
    public static bool isTeleporting = false;

    // Flagi do �ledzenia stan�w
    private bool isSprinting = false;
    private bool isJumping = false;
    private bool isAttacking = false;
    private bool isTakingWeappon = false;
    private bool isHidingWeappon = false;
    private bool isToggleSprintMode = false;
    private bool startOfDying = true;
    private bool endOfDying = true;

    [Header("Combat Settings")]
    [SerializeField] private float attackRange = 10f;  // Maksymalny zasi�g, w kt�rym gracz szuka przeciwnika
    [SerializeField] private float rotationSpeed = 100f; // Szybko�� rotacji do przeciwnika

    private void Start()
    {
        GameInputSystem.Instance.OnSprintAction += GameInputSystem_OnSprintAction;
        GameInputSystem.Instance.OnJumpAction += GameInputSystem_OnJumpAction;
        GameInputSystem.Instance.OnAttackAction += GameInputSystem_OnAttackAction;
        GameInputSystem.Instance.OnTakeWeapponAction += GameInputSystem_OnTakeWeapponAction;

        SetupToggleSprintMode();

        statisticsManager = StatisticsManager.Instance;
        statistics = statisticsManager.LoadStatistics();

        achievements = SaveManager.Instance.LoadPlayerAchievements();

        if (achievements == null)
        {
            achievements = new PlayerAchievements()
            {
                islandsVisited = 0,
                enemiesDefeated = 0,
                missionsCompleted = 0,
                goldEarned = 0,
            };
        }

        if (onNewIsland)
        {
            achievements.islandsVisited += 1;
            achievements.goldEarned += 100;
            statistics.experience += 50;
        }

        animator = GetComponent<Animator>();  // Inicjalizacja komponentu Animator
        characterController = GetComponent<CharacterController>();  // Inicjalizacja komponentu CharacterController
        mainCamera = Camera.main;  // Pobranie referencji do g��wnej kamery
        lastCameraYaw = mainCamera.transform.eulerAngles.y;  // Zapisanie pocz�tkowego obr�t kamery w osi Y

        // Zapisz domy�ln� centraln� pozycje CharacterController
        defaultCenter = characterController.center;
        CalculateMaxYPosition();  // Obliczenie maksymalnej dozwolonej wysoko�ci na podstawie terenu
        StartCoroutine(RegenerateHealth());
        // StartCoroutine(testExp());
    }

    private void GameInputSystem_OnJumpAction(object sender, System.EventArgs e)
    {
        isJumping = true;  // Ustaw flag� skoku, gdy wywo�ane jest odpowiednie zdarzenie
    }

    private void GameInputSystem_OnAttackAction(object sender, System.EventArgs e)
    {
        if (!GetComponent<TakeWeapponPlayer>().isHoldingWeappon)
        {
            isTakingWeappon = true;
        }
        else
        {
            isAttacking = true;
        }       
    }

    private void GameInputSystem_OnTakeWeapponAction(object sender, System.EventArgs e)
    {
        if (!GetComponent<TakeWeapponPlayer>().isHoldingWeappon)
        {
            isTakingWeappon = true;
        }
        else
        {
            isHidingWeappon = true;
        } 

    }

    private void GameInputSystem_OnSprintAction(object sender, System.EventArgs e)
    {
        if (isToggleSprintMode)
        {
            // W trybie prze��czania w��czaj/wy��czaj sprint za ka�dym razem, gdy wywo�ane zostanie zdarzenie
            isSprinting = !isSprinting;
        }
        else
        {
            // W trybie przytrzymywania po prostu w��czaj sprint, gdy przycisk jest wci�ni�ty
            isSprinting = true;
        }
    }

    private void SetupToggleSprintMode()
    {
        var settingsData = SaveManager.Instance.LoadSettingsData();

        if (settingsData != null)
        {
            if (!string.IsNullOrEmpty(settingsData.controls.runMode) && settingsData.controls.runMode == "PRZYTRZYMAJ")
            {
                isToggleSprintMode = false;
            }
            else if (!string.IsNullOrEmpty(settingsData.controls.runMode) && settingsData.controls.runMode == "PRZE��CZ")
            {
                isToggleSprintMode = true;
            }
        }
    }

    private void RotateTowardsNearestEnemy()
    {
        Transform nearestEnemy = FindNearestEnemy();

        if (nearestEnemy != null)
        {
            Vector3 directionToEnemy = (nearestEnemy.position - transform.position).normalized;
            directionToEnemy.y = 0; // Ignorujemy r�nic� w wysoko�ci

            Quaternion targetRotation = Quaternion.LookRotation(directionToEnemy);

            // Dodajemy dodatkowy k�t do rotacji, aby lekko przesun�� kierunek
            float additionalAngle = 10f; // Mo�esz dostosowa� t� warto��
            targetRotation *= Quaternion.Euler(0, additionalAngle, 0); // Dodajemy k�t w osi Y

            // Dostosowujemy obr�t a� do osi�gni�cia odpowiedniego k�ta
            while (Quaternion.Angle(transform.rotation, targetRotation) > 0.1f)
            {
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
        }
    }

    private Transform FindNearestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("enemy");
        Transform closestEnemy = null;
        float minDistance = attackRange;

        foreach (GameObject enemy in enemies)
        {
            float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);

            // Je�li przeciwnik jest w zasi�gu ataku i bli�ej ni� poprzednio znaleziony
            if (distanceToEnemy < minDistance)
            {
                minDistance = distanceToEnemy;
                closestEnemy = enemy.transform;
            }
        }
        return closestEnemy;
    }

    private void Update()
    {
        AnimatorStateInfo currentAnimationState = animator.GetCurrentAnimatorStateInfo(0);
        if (statistics.health <= 0)
        {
            if (startOfDying)
            {
                animator.SetBool("BackToLive", false);
                isAttacking = false;
                MobController.ChasingMobCount = 0;
                AudioSystem.Instance.StopSound(AudioSystem.Instance.audioSources.fightAudioSource);
                statistics.isFighting = false;
                statistics.isPlayerAlive = false;
                AudioSystem.Instance.PlaySound(AudioSystem.Instance.audioSources.effectsAudioSource, AudioSystem.Instance.playerDeathSound, false);
                Debug.Log("DAMAGE: PLAYER IS DEAD");
                SetDead();
                startOfDying = false;
            }

            if (currentAnimationState.IsName("Dead"))
            {
                animator.SetBool("IsDead1", false);
                animator.SetBool("IsDead2", false);
                animator.enabled = false;
                characterController.enabled = false;
                if (endOfDying)
                {
                    endOfDying = false;
                    StartCoroutine(BackPlayerToLive());
                }
            }
        }
        else
        {
            // je�eli zdobyto odpowiednie do�wiadczenie to nowy level
            if (statistics.experience >= statistics.experienceToNextLevel)
            {
                NewLevel();
            }
            
            // Do szybkiej podr�y
            if (isTeleporting && animator.enabled == false)
            {
                Vector3 pos = transform.position;
                if (transform.position == pos)
                {
                    isTeleporting = false;
                    animator.enabled = true;
                }
            }

            // Pobranie kierunku prz�d kamery
            Vector3 forward = mainCamera.transform.forward;
            forward.y = 0f;  // Wyzerowanie sk�adowej Y (unikamy ruchu w g�r�/na d�)
            forward.Normalize();  // Normalizacja wektora (d�ugo�� wektora = 1)

            // Pobranie kierunku prawo kamery
            Vector3 right = mainCamera.transform.right;
            right.y = 0f;  // Wyzerowanie sk�adowej Y
            right.Normalize();  // Normalizacja wektora

            // Obliczenie kierunku ruchu
            Vector2 movementVectorNormalized = GameInputSystem.Instance.GetMovementVectorNormalized();
            Vector3 moveDirection = (forward * movementVectorNormalized.y + right * movementVectorNormalized.x).normalized;  // Normalizacja kierunku ruchu (d�ugo�� wektora = 1)
            Vector3 move = moveDirection * Time.deltaTime;

            // Sprawdzenie, czy nowe po�o�enie postaci przekracza dozwolon� wysoko��
            Vector3 newPosition = transform.position + move;
            if (newPosition.y > maxYPosition)
            {
                move.y = 0;  // Zablokowanie ruchu w g�r�
                newPosition.y = maxYPosition;  // Ustawienie maksymalnej dozwolonej wysoko�ci
            }

            // Aktualizacja pozycji postaci, aby nie przekroczy�a maksymalnej wysoko�ci
            if (transform.position.y > maxYPosition)
            {
                Vector3 correctedPosition = transform.position;
                correctedPosition.y = maxYPosition;
                characterController.enabled = false;
                transform.position = correctedPosition;
                characterController.enabled = true;
            }

            // Obr�t postaci w stron� ruchu
            if (moveDirection != Vector3.zero)
            {
                // Obliczenie nowej rotacji postaci zgodnie z kierunkiem ruchu
                Quaternion toRotation = Quaternion.LookRotation(moveDirection, Vector3.up);

                // Pobranie normalnej powierzchni terenu pod postaci�
                Vector3 terrainNormal = GetTerrainNormal(transform.position);

                // Dostosowanie rotacji postaci do nachylenia terenu
                Quaternion slopeRotation = Quaternion.FromToRotation(Vector3.up, terrainNormal);

                // Zastosowanie obrotu w stron� ruchu i dostosowanie nachylenia
                transform.rotation = Quaternion.RotateTowards(transform.rotation, slopeRotation * toRotation, turnSpeed * Time.deltaTime);
            }


            AdjustCharacterControllerCenter(currentAnimationState);

            // Ustaw parametry animatora dla ruchu i skakania
            float walkSpeedMultiplier = isSprinting ? 1.0f : 0.4f;
            animator.SetFloat("WalkSpeed", moveDirection.magnitude * walkSpeedMultiplier);
            float walkSpeed = animator.GetFloat("WalkSpeed");
            // Sprawdzanie, czy posta� jest w trakcie skoku lub ataku
            bool isRunningAnimationPlaying = currentAnimationState.IsName("Run");
            bool isJumpingAnimationPlaying = currentAnimationState.IsTag("Jumping");
            bool isWeapponAnimationPlaying = currentAnimationState.IsTag("Weappon");
            bool isAttackingAnimationPlaying = currentAnimationState.IsTag("Attack");
            bool isDefenceAnimationPlaying = currentAnimationState.IsTag("Defence");

            // Kontrola d�wi�ku w zale�no�ci od animacji
            if (!isJumpingAnimationPlaying && !isAttackingAnimationPlaying 
                && !isWeapponAnimationPlaying && !isDefenceAnimationPlaying)
            {
                if (walkSpeed >= 0.1f && walkSpeed < 0.5f)  // Chodzenie
                {
                    StopCurrentSound(AudioSystem.Instance.run);
                    isRunningSoundPlaying = false;
                    PlaySoundIfNotPlaying(ref isWalkingSoundPlaying, AudioSystem.Instance.walk);
                }
                else if (walkSpeed >= 0.5f)  // Bieganie
                {
                    StopCurrentSound(AudioSystem.Instance.walk);
                    isWalkingSoundPlaying = false;
                    PlaySoundIfNotPlaying(ref isRunningSoundPlaying, AudioSystem.Instance.run);
                }
                else  // Post�j
                {
                    StopCurrentSound(AudioSystem.Instance.walk);
                    StopCurrentSound(AudioSystem.Instance.run);
                    isWalkingSoundPlaying = false;
                    isRunningSoundPlaying = false;
                }
            }
            else  // Je�li trwa animacja ataku lub skoku
            {
                StopCurrentSound(AudioSystem.Instance.walk);
                StopCurrentSound(AudioSystem.Instance.run);
                isWalkingSoundPlaying = false;
                isRunningSoundPlaying = false;
            }

            animator.SetBool("Jump", isJumping);
            if (isJumping)
            {
                isJumping = false;
            }

            animator.SetBool("TakeWeappon", isTakingWeappon);
            animator.SetBool("HideWeappon", isHidingWeappon);
            if (isTakingWeappon)
            {
                if (statistics.isFighting && !isAttackingAnimationPlaying && !isJumpingAnimationPlaying
                    && !isRunningAnimationPlaying && !isWeapponAnimationPlaying && !isDefenceAnimationPlaying) RotateTowardsNearestEnemy();
                isTakingWeappon = false;
            }
            if (isHidingWeappon)
            {
                isHidingWeappon = false;                
            }

            SetAttack();
            if (isAttacking)
            {
                if(statistics.isFighting && !isAttackingAnimationPlaying && !isJumpingAnimationPlaying 
                    && !isRunningAnimationPlaying && !isWeapponAnimationPlaying && !isDefenceAnimationPlaying) RotateTowardsNearestEnemy();
                isAttacking = false;
            }

            if (isDefenceAnimationPlaying || isAttackingAnimationPlaying 
                || isJumpingAnimationPlaying || isWeapponAnimationPlaying)
            {
                animator.SetBool("React1", false);
            }

            if (!isToggleSprintMode && !GameInputSystem.Instance.IsSprintKeyPressed())
            {
                isSprinting = false; // Wy��cz sprint, je�li klawisz sprintu nie jest przytrzymywany
            }
        }     
    }
    private IEnumerator RegenerateHealth()
    {
        while (statistics.health < statistics.maxHealth && !statistics.isFighting)
        {
            statistics.health += statistics.healthRegen;
            statistics.health = Mathf.Min(statistics.health, statistics.maxHealth); // Upewnienie si�, �e zdrowie nie przekroczy maksymalnego
            GetComponent<NumberParticleEmitter>().EmitNumberParticle("+" + statistics.healthRegen, "green");

            yield return new WaitForSeconds(1f);
        }
    }

    private IEnumerator BackPlayerToLive()
    {
        // Czekaj 5 sekund
        yield return new WaitForSeconds(5f);

        // Zresetuj po�o�enie gracza na pocz�tek mapki i odejmij jego exp
        int experiencePenalty = 100 * statistics.level;
        Transform signpostTransform = FindObjectOfType<Signpost>().transform;
        transform.position = new Vector3(signpostTransform.position.x - 5, signpostTransform.position.y, signpostTransform.position.z - 1);
        transform.rotation = new Quaternion(transform.rotation.x, 0, transform.rotation.z, transform.rotation.w);
        statistics.isPlayerAlive = true;
        statistics.health = statistics.maxHealth;
        if (statistics.experience > experiencePenalty)
        {
            statistics.experience -= experiencePenalty;
        } else
        {
            statistics.experience = 0;
        }
        animator.enabled = true;
        animator.SetBool("BackToLive", true);
        characterController.enabled = true;
        GetComponent<TakeWeapponPlayer>().HideWeapponAfterDeath();
        AudioSystem.Instance.PlayLocalIslandMusic();
        startOfDying = true;
        endOfDying = true;
    }

    IEnumerator testExp()
    {
        while (true)
        {
            statistics.experience += 10;
            yield return new WaitForSeconds(2f);
        }
        
    }
    private void NewLevel()
    {
        GetComponent<NumberParticleEmitter>().EmitNumberParticle("NOWY LEVEL", "gold");
        statistics.level++;
        statistics.experienceToNextLevel = (long)(100 * Math.Pow(statistics.level, 1.5));
        statistics.maxHealth += 5 * statistics.level;
        statistics.healthRegen += statistics.level % 4 == 0 ? 1 : 0;
        statistics.attack += statistics.level < 10 ? 1 : 2;
        statistics.luck += statistics.level % 3 == 0 ? 1 : 0;
        StartCoroutine(RegenerateHealth());
    }

    private void SetAttack()
    {
        int randomAttack = UnityEngine.Random.Range(1, 5);
        switch (randomAttack)
        {
            case 1:
                animator.SetBool("Attack1", isAttacking);
                break;
            case 2:
                animator.SetBool("Attack2", isAttacking);
                break;
            case 3:
                animator.SetBool("Attack3", isAttacking);
                break;
            case 4:
                animator.SetBool("Attack4", isAttacking);
                break;
        }
    }

    private void SetDead()
    {
        int randomAttack = UnityEngine.Random.Range(1, 3);
        switch (randomAttack)
        {
            case 1:
                animator.SetBool("IsDead1", startOfDying);
                break;
            case 2:
                animator.SetBool("IsDead2", startOfDying);
                break;
        }
    }

    // Funkcja do wywo�ania d�wi�ku
    private void PlaySoundIfNotPlaying(ref bool isSoundPlaying, AudioClip clip)
    {
        if (!isSoundPlaying)
        {
            AudioSystem.Instance.PlaySound(AudioSystem.Instance.audioSources.playerEffectsAudioSource, clip, true);
            isSoundPlaying = true;
        }
    }

    // Funkcja do zatrzymania aktualnie wykonywanego d�wi�ku postaci
    private void StopCurrentSound(AudioClip clip)
    {
        if (AudioSystem.Instance.audioSources.playerEffectsAudioSource.isPlaying &&
            AudioSystem.Instance.audioSources.playerEffectsAudioSource.clip == clip)
        {
            AudioSystem.Instance.StopSound(AudioSystem.Instance.audioSources.playerEffectsAudioSource);
        }
    }

    public void UpdateMobFight()
    {
        if (MobController.ChasingMobCount > 0)
        {
            statistics.isFighting = true;
            if (!AudioSystem.Instance.audioSources.fightAudioSource.isPlaying)
            {
                AudioSystem.Instance.PlaySound(AudioSystem.Instance.audioSources.fightAudioSource, AudioSystem.Instance.fightMusic, true);
                StartCoroutine(AudioSystem.Instance.FadeOut(AudioSystem.Instance.audioSources.musicAudioSource, 0.5f));
                // Zatrzymaj `playMusicCoroutine` tylko je�li jest uruchomiona
                if (AudioSystem.Instance.playMusicCoroutine != null)
                {
                    AudioSystem.Instance.StopCoroutine(AudioSystem.Instance.playMusicCoroutine);
                    AudioSystem.Instance.playMusicCoroutine = null;
                }
            }    
        }
        else
        {
            statistics.isFighting = false;
            StartCoroutine(RegenerateHealth());
            AudioSystem.Instance.StopSound(AudioSystem.Instance.audioSources.fightAudioSource);
            AudioSystem.Instance.PlayLocalIslandMusic();
        }
    }

    // Funkcja dostosowuj�ca CharacterController na podstawie nachylenia terenu
    private void AdjustCharacterControllerCenter(AnimatorStateInfo animationState)
    {
        Vector3 terrainNormal = GetTerrainNormal(transform.position);

        // Oblicz k�t nachylenia w stopniach
        float slopeAngle = Vector3.Angle(terrainNormal, Vector3.up);

        // Dostosuj �rodek CharacterController w zale�no�ci od k�ta nachylenia
        if (slopeAngle > 0)
        {
            float centerYAdjustment = Mathf.Floor(slopeAngle / 10) * 0.03f;
            characterController.center = new Vector3(defaultCenter.x, defaultCenter.y - centerYAdjustment, defaultCenter.z);
        }
        else
        {
            characterController.center = defaultCenter;
        }
    }

    private void CalculateMaxYPosition()
    {
        TerrainData terrainData = terrain.terrainData;

        // Zbieranie danych wysoko�ci terenu
        List<float> heights = new List<float>();
        for (int x = 250; x < terrainData.heightmapResolution - 250; x++)
        {
            for (int z = 250; z < terrainData.heightmapResolution - 250; z++)
            {
                float height = terrainData.GetHeight(x, z);
                heights.Add(height);
            }
        }

        // Obliczenie mediany wysoko�ci
        heights.Sort();
        float medianHeight = heights[heights.Count / 2];
        maxYPosition = medianHeight + heightMargin;

        Debug.Log("Maksymalna wysoko�� dla postaci: " + maxYPosition);
    }

    // Funkcja obliczaj�ca normaln� nachylenia terenu
    private Vector3 GetTerrainNormal(Vector3 position)
    {
        // Pobranie normalnej z danych terenu
        float x = (position.x - terrain.transform.position.x) / terrain.terrainData.size.x;
        float z = (position.z - terrain.transform.position.z) / terrain.terrainData.size.z;

        return terrain.terrainData.GetInterpolatedNormal(x, z);
    }

    protected override void OnDestroy()
    {
        if (GameInputSystem.Instance != null)
        {
            GameInputSystem.Instance.OnSprintAction -= GameInputSystem_OnSprintAction;
            Debug.Log($"[PlayerController] OnSprintAction has {GameInputSystem.Instance.GetOnSprintActionSubscribersCount()} subscribers after OnDestroy.");
            GameInputSystem.Instance.OnJumpAction -= GameInputSystem_OnJumpAction;
            Debug.Log($"[PlayerController] OnJumpAction has {GameInputSystem.Instance.GetOnJumpActionSubscribersCount()} subscribers after OnDestroy.");
            GameInputSystem.Instance.OnAttackAction -= GameInputSystem_OnAttackAction;
            Debug.Log($"[PlayerController] OnAttackAction has {GameInputSystem.Instance.GetOnAttackActionSubscribersCount()} subscribers after OnDestroy.");
            GameInputSystem.Instance.OnTakeWeapponAction -= GameInputSystem_OnTakeWeapponAction;
            Debug.Log($"[PlayerController] OnTakeWeapponAction has {GameInputSystem.Instance.GetOnTakeWeapponActionSubscribersCount()} subscribers after OnDestroy.");
        }

        base.OnDestroy();
    }
}
