using UnityEngine;

public class TakeWeapponMob : MonoBehaviour
{
    private Animator animator;
    private bool playingAudioMob = false;
    public bool isHoldingWeappon = false;
    private AudioSource mobEffectsAudioSource;
    private Transform player;

    [SerializeField] internal string AnimationTakingWeappon; // Animacja wyjêcia broni
    [SerializeField] internal string AnimationHidingWeappon; // Animacja schowania broni
    [SerializeField] internal GameObject WeapponHide; // Po³o¿enie broni u pasa Postaci
    [SerializeField] internal GameObject WeapponTake; // Po³o¿enie broni w rêku postaci
    [SerializeField] internal GameObject AudioSourceHolder; 

    private bool actionTriggered = false;

    private void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
        animator = GetComponent<Animator>();
        mobEffectsAudioSource = AudioSourceHolder.AddComponent<AudioSource>();
        mobEffectsAudioSource.playOnAwake = false;
        mobEffectsAudioSource.volume = AudioSystem.Instance.audioSources.playerEffectsAudioSource.volume;
        mobEffectsAudioSource.outputAudioMixerGroup = AudioSystem.Instance.audioSources.playerEffectsAudioSource.outputAudioMixerGroup;
    }
    private void Update()
    {
        // Oblicz kierunek do gracza, ignoruj¹c ró¿nicê wysokoœci
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        directionToPlayer.y = 0; 
        float distanceToPlayer = Vector3.Distance(player.position, transform.position);
        float maxHearingDistance = 30f;
        mobEffectsAudioSource.volume = Mathf.Lerp(0, AudioSystem.Instance.audioSources.playerEffectsAudioSource.volume, 1 - (distanceToPlayer / maxHearingDistance));

        if (!isHoldingWeappon)
        {
            if (animator.GetCurrentAnimatorStateInfo(0).IsName(AnimationTakingWeappon))
            {
                if (!playingAudioMob)
                {
                    AudioSystem.Instance.PlaySound(mobEffectsAudioSource, AudioSystem.Instance.takeWeappon, false);
                    playingAudioMob = true;
                } 
                // Pobierz czas trwania animacji (0.0 - 1.0)
                float normalizedTime = animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
                if (normalizedTime >= 0.4f && !actionTriggered)
                {
                    if (WeapponHide != null)
                        WeapponHide.SetActive(false);

                    if (WeapponTake != null)
                        WeapponTake.SetActive(true);

                    actionTriggered = true;
                    isHoldingWeappon = true;
                }
            }
            else
            {
                // Resetuj flagi, gdy animacja jest zakoñczona
                actionTriggered = false;
                playingAudioMob = false;
            }
        } 
        else
        {
            if (animator.GetCurrentAnimatorStateInfo(0).IsName(AnimationHidingWeappon))
            {
                if (!playingAudioMob)
                {
                    AudioSystem.Instance.PlaySound(mobEffectsAudioSource, AudioSystem.Instance.hideWeappon, false);
                    playingAudioMob = true;
                }
                // Pobierz czas trwania animacji (0.0 - 1.0)
                float normalizedTime = animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
                if (normalizedTime >= 0.6f && !actionTriggered)
                {
                    if (WeapponTake != null)
                        WeapponTake.SetActive(false);

                    if (WeapponHide != null)
                        WeapponHide.SetActive(true);

                    actionTriggered = true;
                    isHoldingWeappon = false;
                }
            }
            else
            {
                // Resetuj flagi, gdy animacja jest zakoñczona
                actionTriggered = false;
                playingAudioMob = false;
            }
        }
        
    }
    public void HideWeapponAfterDeath()
    {
        if (isHoldingWeappon)
        {
            if (WeapponTake != null)
                WeapponTake.SetActive(false);

            if (WeapponHide != null)
                WeapponHide.SetActive(true);
            isHoldingWeappon = false;
        }
    }
}
