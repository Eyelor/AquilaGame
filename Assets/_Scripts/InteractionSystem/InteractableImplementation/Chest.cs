using UnityEngine;


public class Chest : MonoBehaviour, IInteractable
{
    [SerializeField] private string _promptPlayer;
    [SerializeField] private string _promptPlayer2;
    private bool hasDrop = true;
    private int chestExperience = 15;

    private PlayerStatistics _statistics;
    private GameObject _player;
    private Animator _animator;

    private string _interactionPrompt;
    public string InteractionPrompt
    {
        get => _interactionPrompt;
        set => _interactionPrompt = value;
    }

    public bool Interact(Interactor interactor)
    {
        if (interactor.name == "PlayerPirat")
        {
            Debug.Log(_promptPlayer);

            _animator = GetComponentInChildren<Animator>();
            _player = GameObject.FindWithTag("Player");
            _statistics = _player.GetComponent<PlayerController>().statistics;

            if (_animator.GetBool("IsOpened"))
            {
                _animator.SetTrigger("CloseChestTrigger");
                _animator.SetBool("IsOpened", false);
                AudioSystem.Instance.PlaySound(AudioSystem.Instance.audioSources.effectsAudioSource, AudioSystem.Instance.chestClose, false);
            }
            else
            {
                _animator.SetTrigger("OpenChestTrigger");
                _animator.SetBool("IsOpened", true);
                AudioSystem.Instance.PlaySound(AudioSystem.Instance.audioSources.effectsAudioSource, AudioSystem.Instance.chestOpen, false);
                if (hasDrop)
                {
                    GetComponent<NumberParticleEmitter>().EmitNumberParticle("+"+ chestExperience, "blue");
                    _statistics.experience += chestExperience;
                    if(_statistics.experience >= _statistics.experienceToNextLevel)
                    {
                        AudioSystem.Instance.PlaySound(AudioSystem.Instance.audioSources.effectsAudioSource, AudioSystem.Instance.winningMobSound, false);
                    }
                    MissionsManager.Instance.ChestOpenedFirstTime();
                    hasDrop = false;
                }
            }

            return true;
        }
        else
        {
            return false;
        }
    }

    public string GetPrompt(string name)
    {
        if (name == "PlayerPirat")
        {
            if (GetComponentInChildren<Animator>().GetBool("IsOpened"))
            {
                return _promptPlayer2;
            } else
            {
                return _promptPlayer;
            }         
        }
        else
        {
            return "";
        }
    }
}
