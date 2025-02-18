using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;

public class MobController : MonoBehaviour
{
    private Animator animator;
    private CharacterController characterController;
    private MobStatistics statistics;

    public Terrain terrain;
    public float turnSpeed = 200f;               // Szybko�� obrotu przeciwnika
    public float heightMargin = 20f;              // Margines dodawany do mediany wysoko�ci terenu
    private float maxYPosition;                  // Maksymalna dozwolona wysoko��, na kt�r� mo�e wej�� przeciwnik
    private Vector3 defaultCenter;

    public float moveIntervalMin = 0f;           // Minimalny czas mi�dzy ruchem
    public float moveIntervalMax = 3f;           // Maksymalny czas mi�dzy ruchem
    public float moveDurationMin = 2f;           // Minimalny czas trwania ruchu
    public float moveDurationMax = 10f;           // Maksymalny czas trwania ruchu
    public float detectionRadius = 2f;           // Promie� detekcji przeszk�d
    public float patrolDistanceLimit = 25f;

    private float moveIntervalTimer;
    private float moveDurationTimer;
    private bool isMoving;
    public bool toChangeDirection = true;
    private Vector3 moveDirection;

    // Komponenty do interakcji z graczem
    private SphereCollider mobInterectionCollider;
    private Transform player;
    private GameObject[] fastTravelPoints;
    private bool isChasingPlayer = false;
    public bool isMobFightingNow = false;
    private bool stoppedChasing = false;
    private bool isInPlayerTrigger = false;
    private bool isAttacking = false;
    private bool isTakingWeappon = false;
    private bool isHidingWeappon = false;
    private bool startOfDying = true;
    private bool endOfDying = true;
    private int mobExperience = 0;
    public static int ChasingMobCount = 0; // Licznik mob�w �cigaj�cych gracza

    // D�wi�k
    private AudioSource mobEffectsAudioSource; // Osobny AudioSource dla efekt�w d�wi�kowych mob�w
    private bool isRunningMobPlaying = false;
    public AudioClip running;

    void Start()
    {
        mobEffectsAudioSource = GetComponent<AudioSource>();
        mobEffectsAudioSource.playOnAwake = false;
        mobEffectsAudioSource.volume = AudioSystem.Instance.audioSources.playerEffectsAudioSource.volume;
        mobEffectsAudioSource.outputAudioMixerGroup = AudioSystem.Instance.audioSources.playerEffectsAudioSource.outputAudioMixerGroup;

        animator = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();
        mobInterectionCollider = gameObject.GetComponent<SphereCollider>();
        statistics = gameObject.GetComponent<MobStatisticComponent>().mobStatistics;
         
        defaultCenter = characterController.center;
        mobInterectionCollider.isTrigger = true;
        mobInterectionCollider.radius = Random.Range(5f, 20f);

        // Automatyczne znalezienie terenu na scenie
        terrain = GameObject.Find("Terrain").GetComponent<Terrain>();

        // Sprawdzenie, czy teren zosta� znaleziony
        if (terrain == null)
        {
            Debug.LogError("MobController: Nie znaleziono terenu o nazwie 'Terrain' na scenie.");
            return;
        }

        CalculateMaxYPosition();
        SetNewMovement();

        // Znajd� obiekt gracza
        player = GameObject.FindWithTag("Player").transform;
        // Pobranie wszystkich obiekt�w szybkiej podr�y
        fastTravelPoints = GameObject.FindGameObjectsWithTag("fastTravel");
    }

    void Update()
    {
        AnimatorStateInfo currentAnimationState = animator.GetCurrentAnimatorStateInfo(0);
        if (statistics.health <= 0)
        {
            if (startOfDying)
            {
                ChasingMobCount--;
                PlayerController.Instance.UpdateMobFight();
                isMobFightingNow = false;
                isChasingPlayer = false;
                stoppedChasing = true;
                mobExperience = Random.Range(GetComponentInChildren<EnemyStatisticsUI>().GetMobLevel() * 10, (GetComponentInChildren<EnemyStatisticsUI>().GetMobLevel() * 10) + 10);
                GameObject.FindWithTag("Player").GetComponent<PlayerController>().statistics.experience += mobExperience;
                GameObject.FindWithTag("Player").GetComponent<PlayerController>().achievements.enemiesDefeated += 1;
                GetComponent<NumberParticleEmitter>().EmitNumberParticle("+" + mobExperience, "blue");
                Debug.Log("DAMAGE: MOB DEAD");
                MissionsManager.Instance.EnemyKilled(GetComponentInChildren<EnemyStatisticsUI>().GetMobLevel());
                SetDead();
                gameObject.tag = "Untagged";
                startOfDying = false;
            }

            if (currentAnimationState.IsName("Dead"))
            {
                animator.SetBool("IsDead1", false);
                animator.SetBool("IsDead2", false);
                animator.enabled = false;
                mobEffectsAudioSource.enabled = false;
                mobInterectionCollider.enabled = false;
                characterController.enabled = false;
                if (endOfDying)
                {
                    endOfDying = false;
                    StartCoroutine(DestroyAfterDelay());
                }          
            }
        }
        else
        {
            if (!PlayerController.Instance.statistics.isPlayerAlive)
            {
                if (GetComponent<TakeWeapponMob>().isHoldingWeappon)
                {
                    isHidingWeappon = true;
                }
                animator.SetBool("HideWeappon", isHidingWeappon);
                if (isHidingWeappon)
                {
                    isHidingWeappon = false;
                }
                animator.SetBool("IsFighting", false);
                if (!stoppedChasing) stoppedChasing = true;
                if (isChasingPlayer)
                {
                    isMobFightingNow = false;
                    isChasingPlayer = false;
                    StartCoroutine(RegenerateHealth());
                }
            }
            else
            {
                bool isAttackingAnimationPlaying = currentAnimationState.IsTag("Attack");
                bool isWeapponAnimationPlaying = currentAnimationState.IsTag("Weappon");
                if (isChasingPlayer && player != null)
                {
                    // Oblicz kierunek do gracza, ignoruj�c r�nic� wysoko�ci
                    Vector3 directionToPlayer = (player.position - transform.position).normalized;
                    directionToPlayer.y = 0; // Ignoruj r�nic� wysoko�ci
                    float distanceToPlayer = Vector3.Distance(player.position, transform.position);
                    float maxHearingDistance = 30f;

                    // Obr�t w kierunku gracza
                    Quaternion toRotation = Quaternion.LookRotation(directionToPlayer, Vector3.up);
                    Vector3 terrainNormal = GetTerrainNormal(transform.position);
                    Quaternion slopeRotation = Quaternion.FromToRotation(Vector3.up, terrainNormal);
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, slopeRotation * toRotation, turnSpeed * Time.deltaTime);

                    if (!GetComponent<TakeWeapponMob>().isHoldingWeappon)
                    {
                        isTakingWeappon = true;
                    }

                    if (distanceToPlayer <= maxHearingDistance && distanceToPlayer > 1.5f)
                    {
                        animator.SetBool("IsFighting", false);
                        // Im bli�ej gracz, tym wi�ksza g�o�no��
                        if (!isWeapponAnimationPlaying && !isAttackingAnimationPlaying)
                        {
                            mobEffectsAudioSource.volume = Mathf.Lerp(0, AudioSystem.Instance.audioSources.playerEffectsAudioSource.volume, 1 - (distanceToPlayer / maxHearingDistance));
                        }
                        else
                        {
                            mobEffectsAudioSource.volume = 0;
                        }
                    }
                        
                    else if (distanceToPlayer <= 1.5f)
                    {
                        animator.SetBool("IsFighting", true);
                        animator.SetFloat("WalkSpeed", 0f);
                        mobEffectsAudioSource.volume = 0;
                        if (!isAttackingAnimationPlaying)
                        {
                            isAttacking = true;
                        }
                    }
                    else
                    {
                        animator.SetBool("IsFighting", false);
                        mobEffectsAudioSource.volume = 0;
                    }
                    animator.SetBool("TakeWeappon", isTakingWeappon);                   
                    if (isTakingWeappon)
                    {
                        isTakingWeappon = false;
                    }

                    setAttack(1, 5);
                    if (isAttacking)
                    {
                        isAttacking = false;
                    }

                    if (!isAttackingAnimationPlaying && !animator.GetBool("IsFighting"))
                    {
                        // Ruch w kierunku gracza
                        Vector3 move = directionToPlayer * Random.Range(0.1f, 0.3f) * Time.deltaTime;
                        characterController.Move(move);
                        // Ustawienie pr�dko�ci animacji na pe�n� pr�dko�� (1.0f) podczas �cigania
                        animator.SetFloat("WalkSpeed", 1.0f);
                    }          
                }
                else
                {
                    if (GetComponent<TakeWeapponMob>().isHoldingWeappon)
                    {
                        isHidingWeappon = true;
                    }
                    animator.SetBool("HideWeappon", isHidingWeappon);
                    if (isHidingWeappon)
                    {
                        isHidingWeappon = false;
                    }
                    // Je�li nie �ciga gracza, wr�� do zwyk�ego zachowania
                    // Odliczanie do kolejnego ruchu
                    if (!isMoving)
                    {
                        moveIntervalTimer -= Time.deltaTime;
                        if (moveIntervalTimer <= 0f)
                        {
                            SetNewMovement();
                        }
                    }
                    else
                    {
                        moveDurationTimer -= Time.deltaTime;
                        if (moveDurationTimer <= 0f)
                        {
                            StopMovement();
                        }
                    }

                    if (isMoving)
                    {
                        // Wykrywanie przeszk�d oraz nachylenia terenu w promieniu detectionRadius
                        if ((DetectObstacle() && toChangeDirection) || (IsSlopeTooSteep() && toChangeDirection))
                        {
                            // Je�li wykryto przeszkod�, zmie� kierunek
                            ChangeDirection(140f, 220f);
                            moveDurationTimer = Random.Range(moveDurationMin, moveDurationMax);
                            toChangeDirection = false;
                            StartCoroutine(ResetChangeDirection());
                        }

                        if (isShortDistanceToFastTravel() && toChangeDirection && !stoppedChasing)
                        {
                            // Je�li fastTravel za blisko i gracz nie by� atakowany przez tego moba
                            // to zmie� kierunek z dala od fastTravel
                            ChangeDirection(170f, 190f);
                            moveDurationTimer = Random.Range(moveDurationMin, moveDurationMax);
                            toChangeDirection = false;
                            StartCoroutine(ResetChangeDirection());
                        }


                        Vector3 move = moveDirection * Time.deltaTime;

                        // Sprawdzenie, czy nowe po�o�enie postaci przekracza dozwolon� wysoko��
                        Vector3 newPosition = transform.position + move;
                        if ((newPosition.y > maxYPosition) && toChangeDirection)
                        {
                            move.y = 0;  // Zablokowanie ruchu w g�r�
                            newPosition.y = maxYPosition;  // Ustawienie maksymalnej dozwolonej wysoko�ci
                            ChangeDirection(140f, 220f);
                            moveDurationTimer = Random.Range(moveDurationMin, moveDurationMax);
                            toChangeDirection = false;
                            StartCoroutine(ResetChangeDirection());
                        }

                        // Poruszanie postaci
                        characterController.Move(move);  // Przesuni�cie postaci

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

                        AdjustCharacterControllerCenter();

                        // Ustawienie parametr�w animatora
                        animator.SetFloat("WalkSpeed", moveDirection.magnitude * 0.4f);
                    }
                    else
                    {
                        animator.SetFloat("WalkSpeed", 0f);  // Zatrzymanie animacji chodzenia
                    }
                }
            }
            
        }
        
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isInPlayerTrigger = true;
            if (isChasingPlayer != true)
            {          
                ChasingMobCount++;
                PlayerController.Instance.UpdateMobFight();
                isChasingPlayer = true; // Rozpocz�cie �cigania gracza
                isMobFightingNow = true;
            }          
            if (mobEffectsAudioSource != null && !isRunningMobPlaying)
            {
                AudioSystem.Instance.PlaySound(mobEffectsAudioSource, running, true);
                isRunningMobPlaying = true;
            }        
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isInPlayerTrigger = false;
            StartCoroutine(StopChasingAfterDelay(3f)); // Koniec �cigania gracza po 3 sekundach
        }
    }


    // Korutyna do zatrzymania �cigania po op�nieniu
    IEnumerator StopChasingAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (!isInPlayerTrigger)
        {
            if (mobEffectsAudioSource != null && mobEffectsAudioSource.isPlaying)
            {
                AudioSystem.Instance.StopSound(mobEffectsAudioSource);
                isRunningMobPlaying = false;
            }
            if (isChasingPlayer == true)
            {
                ChasingMobCount--;
                PlayerController.Instance.UpdateMobFight();
                isChasingPlayer = false;
                isMobFightingNow = false;
                stoppedChasing = true;
                // Rozpocznij regeneracj� zdrowia
                StartCoroutine(RegenerateHealth());
            }           
        }
    }

    IEnumerator RegenerateHealth()
    {
        while (statistics.health < statistics.maxHealth && !isChasingPlayer)
        {
            statistics.health += statistics.healthRegen;
            statistics.health = Mathf.Min(statistics.health, statistics.maxHealth); // Upewnienie si�, �e zdrowie nie przekroczy maksymalnego

            yield return new WaitForSeconds(1f);
        }
    }

    IEnumerator DestroyAfterDelay()
    {
        // Czekaj 5 sekund
        yield return new WaitForSeconds(5f);
        
        // Zniszcz obiekt
        Destroy(gameObject);
    }

    private void setAttack(int min, int max)
    {
        int randomAttack = Random.Range(min, max);
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
        int randomAttack = Random.Range(1, 3);
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

    // Funkcja wykrywaj�ca przeszkody z wykorzystaniem raycastingu
    bool DetectObstacle()
    {
        // Liczba promieni wychodz�cych z postaci (wi�cej promieni = dok�adniejsze wykrywanie, ale wi�ksze obci��enie)
        int rayCount = 10;

        // K�t rozproszenia promieni (np. +/- 20 stopni od kierunku ruchu)
        float angleSpread = 20f;

        // D�ugo�� promienia, jak daleko ma si�ga� wykrywanie przeszkody
        float rayDistance = detectionRadius;

        // Pozycja startowa promienia (pozycja postaci)
        Vector3 startPosition = transform.position + Vector3.up * 0.4f; // lekko nad ziemi�, aby wykrywa� przeszkody na poziomie wzroku

        for (int i = 0; i < rayCount; i++)
        {
            // Oblicz k�t dla ka�dego promienia
            float angle = -angleSpread + (angleSpread * 2) * (i / (float)(rayCount - 1));

            // Oblicz kierunek promienia, obracaj�c go o wyliczony k�t wok� osi Y (wertykalnej)
            Vector3 rayDirection = Quaternion.Euler(0f, angle, 0f) * moveDirection;

            // Wystrzel raycast w kierunku rayDirection
            if (Physics.Raycast(startPosition, rayDirection, out RaycastHit hit, rayDistance))
            {
                if (hit.collider is BoxCollider || hit.collider is MeshCollider)
                {
                    Debug.DrawRay(startPosition, rayDirection * rayDistance, Color.red); // Debugging: poka� raycast w edytorze
                    return true; // Wykryto przeszkod�
                }
            }

            // Debugging: poka� raycasty w edytorze Unity (zielone, je�li nie trafi�y w przeszkod�)
            Debug.DrawRay(startPosition, rayDirection * rayDistance, Color.green);
        }

        return false; // Nie wykryto przeszkody
    }

    // Sprawdzenie czy mob nie wchodzi na teren o zbyt du�ym nachyleniu
    bool IsSlopeTooSteep()
    {
        Vector3 terrainNormal = GetTerrainNormal(transform.position);
        float slopeAngle = Vector3.Angle(terrainNormal, Vector3.up);
        return slopeAngle > characterController.slopeLimit;
    }

    // Zmiana kierunku poruszania si� moba przy napotkaniu przeszkody
    void ChangeDirection(float valueMin, float valueMax)
    {
        // Obr�t kierunku ruchu
        Quaternion rotation = Quaternion.Euler(0f, Random.Range(valueMin, valueMax), 0f);
        moveDirection = rotation * moveDirection;

        // Normalizacja wektora kierunku ruchu, aby zapewni�, �e jego d�ugo�� wynosi 1
        moveDirection = moveDirection.normalized;
    }

    private bool isShortDistanceToFastTravel()
    {
        foreach (var fastTravelPoint in fastTravelPoints)
        {
            float distanceToFastTravel = Vector3.Distance(transform.position, fastTravelPoint.transform.position);

            // Je�li mob jest bli�ej ni� 25 jednostek od fastTravel
            if (distanceToFastTravel < patrolDistanceLimit)
            {
                return true;
            }
        }
        return false;
    }

    // Ustalenie kolejnej sekwencji ruchu i zatrzymania
    void SetNewMovement()
    {
        // Losowy kierunek ruchu
        float randomAngle = Random.Range(0f, 360f);
        moveDirection = new Vector3(Mathf.Sin(randomAngle), 0f, Mathf.Cos(randomAngle)).normalized;

        // Losowy czas trwania ruchu
        moveDurationTimer = Random.Range(moveDurationMin, moveDurationMax);

        // Ustawienie czasu do nast�pnego ruchu
        moveIntervalTimer = Random.Range(moveIntervalMin, moveIntervalMax);

        toChangeDirection = true;
        isMoving = true;
    }

    // Funkcja zatrzymymuj�ca moba
    void StopMovement()
    {
        isMoving = false;
        if(stoppedChasing && !isShortDistanceToFastTravel()) stoppedChasing = false;
    }

    // Zmienia wysoko�� CharacterControllera �eby posta� dopasowa�a si� do nachylenia
    void AdjustCharacterControllerCenter()
    {
        Vector3 terrainNormal = GetTerrainNormal(transform.position);

        // Calculate slope angle in degrees
        float slopeAngle = Vector3.Angle(terrainNormal, Vector3.up);

        // Adjust CharacterController center based on slope angle
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

    // Funkcja obliczaj�ca maksymaln� pozycj� w Y na jakiej mo�e znale�� si� mob
    void CalculateMaxYPosition()
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
    }

    // Funkcja obliczaj�ca normaln� nachylenia terenu
    Vector3 GetTerrainNormal(Vector3 position)
    {
        // Pobranie normalnej z danych terenu
        float x = (position.x - terrain.transform.position.x) / terrain.terrainData.size.x;
        float z = (position.z - terrain.transform.position.z) / terrain.terrainData.size.z;

        return terrain.terrainData.GetInterpolatedNormal(x, z);
    }

    // Korutyna do ustawienia toChangeDirection na true po sekundzie
    IEnumerator ResetChangeDirection()
    {
        yield return new WaitForSeconds(1f);
        toChangeDirection = true;
    }
}
