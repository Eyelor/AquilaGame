using UnityEngine;
using UnityEngine.Audio;


// Klasa pomocnicza dla testów
internal class FakePlayerController : MonoBehaviour
{
    internal PlayerStatistics statistics;
}

public class MobDamage : MonoBehaviour
{
    private GameObject mob;
    private Animator animator;
    internal MobStatistics statistics;
    private bool isDamagedPlayer = false;
    private AudioSource mobDamageAudioSource;
    private bool isAnimationDamage = false;

    void Start()
    {
        // Sprawdzenie, czy kod dzia³a w trybie testów
        if (!TestEnvironment.IsPlayModeTest)
        {
            mob = FindParentMob(transform);
            if (mob != null)
            {
                animator = mob.GetComponent<Animator>();
                statistics = mob.GetComponent<MobStatisticComponent>().mobStatistics;
            }
            mobDamageAudioSource = gameObject.AddComponent<AudioSource>();
            mobDamageAudioSource.playOnAwake = false;
            mobDamageAudioSource.volume = AudioSystem.Instance.audioSources.playerEffectsAudioSource.volume;
            mobDamageAudioSource.outputAudioMixerGroup = AudioSystem.Instance.audioSources.playerEffectsAudioSource.outputAudioMixerGroup;
        }      
    }

    private void Update()
    {
        if (!isAttackingAnimationPlaying() && isDamagedPlayer)
        {
            isDamagedPlayer = false;
        }
    }
    private GameObject FindParentMob(Transform child)
    {
        while (child != null)
        {
            if (child.GetComponent<MobStatisticComponent>() != null)
            {
                return child.gameObject;
            }
            child = child.parent;
        }
        return null; // Zwraca null, jeœli nie znaleziono obiektu moba
    }

    internal void OnTriggerEnter(Collider other)
    {
        if (!TestEnvironment.IsPlayModeTest)
        {
            if (isAttackingAnimationPlaying() && other.CompareTag("Player") && statistics.health > 0
                && !isDamagedPlayer && other.gameObject.GetComponent<PlayerController>().statistics.health > 0)
            {
                int inTarget = Random.Range(0, statistics.luck + 1) == 0 ? 0 : 1;
                int damage = inTarget * (int)(statistics.attack * Random.Range(0.9f, 1.4f));
                string damageInfo = damage == 0 ? "pud³o" : ("-" + damage.ToString());
                if (other.gameObject.GetComponent<PlayerController>().statistics.health - damage > 0)
                {
                    other.gameObject.GetComponent<NumberParticleEmitter>().EmitNumberParticle(damageInfo, "red");
                    isAnimationDamage = true;
                    other.gameObject.GetComponent<Animator>().SetBool("React1", isAnimationDamage);
                    isAnimationDamage = false;
                }
                else
                {
                    other.gameObject.GetComponent<NumberParticleEmitter>().EmitNumberParticle("PIRAT ZOSTA£ ZABITY", "redDeath");
                }
                randomSoundDamage(damage);
                other.gameObject.GetComponent<PlayerController>().statistics.health -= damage;
            
                Debug.Log("DAMAGE GIVEN TO PLAYER: " + damageInfo);
                Debug.Log("PLAYER HEALTH AFTER DAMAGE: " + other.gameObject.GetComponent<PlayerController>().statistics.health);
                isDamagedPlayer = true;
            }
        }       
        else
        {
            if (other.CompareTag("Player") && statistics.health > 0 && !isDamagedPlayer && other.gameObject.GetComponent<FakePlayerController>().statistics.health > 0)
            {
                int inTarget = Random.Range(0, statistics.luck + 1) == 0 ? 0 : 1;
                int damage = inTarget * (int)(statistics.attack * Random.Range(0.9f, 1.4f));
                string damageInfo = damage == 0 ? "miss" : ("-" + damage.ToString());
                other.gameObject.GetComponent<FakePlayerController>().statistics.health -= damage;
                Debug.Log("DAMAGE GIVEN TO PLAYER: " + damageInfo);
                Debug.Log("PLAYER HEALTH AFTER DAMAGE: " + other.gameObject.GetComponent<FakePlayerController>().statistics.health);
                isDamagedPlayer = true;
            }
        }
    }

    private bool isAttackingAnimationPlaying()
    {
        if (!TestEnvironment.IsPlayModeTest)
        {
            AnimatorStateInfo currentAnimationState = animator.GetCurrentAnimatorStateInfo(0);
            return currentAnimationState.IsTag("Attack"); 
        }
        else
        {
            return false;
        }
        
    }

    private void randomSoundDamage(int damage)
    {
        AudioClip clipToPlay;
        if (damage == 0)
        {
            // Ostatni element tablicy `soundsDamage` jest dŸwiêkiem "miss"
            clipToPlay = AudioSystem.Instance.soundsDamage[AudioSystem.Instance.soundsDamage.Length - 1];
        }
        else
        {
            // Losowy dŸwiêk obra¿eñ z pominiêciem ostatniego elementu (dŸwiêku "miss")
            int randomIndex = Random.Range(0, AudioSystem.Instance.soundsDamage.Length - 1);
            clipToPlay = AudioSystem.Instance.soundsDamage[randomIndex];
        }

        // Odtwarzanie wybranego dŸwiêku
        AudioSystem.Instance.PlaySound(mobDamageAudioSource, clipToPlay, false);
    }

}
