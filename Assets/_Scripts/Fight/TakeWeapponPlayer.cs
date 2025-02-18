using UnityEngine;

public class TakeWeapponPlayer : MonoBehaviour
{
    private Animator animator;
    private bool playingAudioPlayer = false;
    public bool isHoldingWeappon = false;

    [SerializeField] private string AnimationTakingWeappon; // Animacja wyjêcia broni
    [SerializeField] private string AnimationHidingWeappon; // Animacja schowania broni
    [SerializeField] private GameObject WeapponHide; // Po³o¿enie broni u pasa Postaci
    [SerializeField] private GameObject WeapponTake; // Po³o¿enie broni w rêku postaci


    private bool actionTriggered = false;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }
    private void Update()
    {
        if (!isHoldingWeappon)
        {
            if (animator.GetCurrentAnimatorStateInfo(0).IsName(AnimationTakingWeappon))
            {
                if (!playingAudioPlayer)
                {
                    AudioSystem.Instance.PlaySound(AudioSystem.Instance.audioSources.effectsAudioSource, AudioSystem.Instance.takeWeappon, false);
                    playingAudioPlayer = true;
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
                playingAudioPlayer = false;
            }
        } 
        else
        {
            if (animator.GetCurrentAnimatorStateInfo(0).IsName(AnimationHidingWeappon))
            {
                if (!playingAudioPlayer)
                {
                    AudioSystem.Instance.PlaySound(AudioSystem.Instance.audioSources.effectsAudioSource, AudioSystem.Instance.hideWeappon, false);
                    playingAudioPlayer = true;
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
                playingAudioPlayer = false;
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
