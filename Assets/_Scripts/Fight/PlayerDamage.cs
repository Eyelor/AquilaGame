using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDamage : MonoBehaviour
{
    private GameObject player;
    private Animator animator;
    internal HashSet<GameObject> damagedEnemies = new HashSet<GameObject>();
    internal PlayerStatistics statistics;
    private bool needToUpdate = false;

    void Start()
    {
        // Sprawdzenie, czy kod dzia³a w trybie testów
        if (!TestEnvironment.IsPlayModeTest)
        {
            player = GameObject.FindWithTag("Player");
            animator = player.GetComponent<Animator>();
        }
        needToUpdate = true;
    }

    void Update()
    {
        if (!TestEnvironment.IsPlayModeTest)
        {
            if (needToUpdate)
            {
                statistics = player.GetComponent<PlayerController>().statistics;
                needToUpdate = false;
            }
        }
    
        if (!isAttackingAnimationPlaying() && damagedEnemies.Count > 0)
        {
            // Zresetuj listê przeciwników gdy skoñczy siê animacja
            damagedEnemies.Clear();
        }
    }
    internal void OnTriggerEnter(Collider other)
    {
        if (isAttackingAnimationPlaying() && other.CompareTag("enemy") && statistics.isPlayerAlive
            && !damagedEnemies.Contains(other.gameObject) && other.gameObject.GetComponent<MobStatisticComponent>().mobStatistics.health > 0)
        {
            int inTarget = Random.Range(0, statistics.luck + 1) == 0 ? 0 : 1;
            int damage = inTarget * (int)(statistics.attack * Random.Range(0.9f, 1.5f) + Random.Range(1, statistics.level / 2));
            string damageInfo = damage == 0 ? "pud³o" : ("-" + damage.ToString());
            other.gameObject.GetComponent<MobStatisticComponent>().mobStatistics.health -= damage;

            // Sprawdzenie, czy kod dzia³a w trybie testów
            if (!TestEnvironment.IsPlayModeTest)
            {
                other.gameObject.GetComponent<NumberParticleEmitter>().EmitNumberParticle(damageInfo, "white");
                randomSoundDamage(damage);
                if (other.gameObject.GetComponent<MobStatisticComponent>().mobStatistics.health <= 0)
                {
                    AudioSystem.Instance.PlaySound(AudioSystem.Instance.audioSources.effectsAudioSource, AudioSystem.Instance.winningMobSound, false);
                }
            }
            damagedEnemies.Add(other.gameObject); // Dodaj przeciwnika do listy trafionych w tej animacji
            Debug.Log("DAMAGE GIVEN TO ENEMY " + damagedEnemies.Count + ": " + damageInfo);
            Debug.Log("ENEMY " + damagedEnemies.Count + " HEALTH AFTER DAMAGE: " + other.gameObject.GetComponent<MobStatisticComponent>().mobStatistics.health);        
        }
    }

    private bool isAttackingAnimationPlaying()
    {
        // Sprawdzenie, czy kod dzia³a w trybie testów
        if (!TestEnvironment.IsPlayModeTest)
        {
            AnimatorStateInfo currentAnimationState = animator.GetCurrentAnimatorStateInfo(0);
            return currentAnimationState.IsTag("Attack");
        } else
        {
            return true;
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
        AudioSystem.Instance.PlaySound(AudioSystem.Instance.audioSources.effectsAudioSource, clipToPlay, false);
    }
}
