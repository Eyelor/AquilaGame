using UnityEngine;

public class FastTravel : MonoBehaviour, IInteractable
{
    [SerializeField] private string _promptPlayer;
    [SerializeField] private GameObject _player;
    [SerializeField] private GameObject _fastTravelsParent;
    [SerializeField] private Terrain _terrain; // Terrain, na którym dokonywana jest szybka podró¿
    private GameObject[] _fastTravels;

    private string _interactionPrompt;
    public string InteractionPrompt
    {
        get => _interactionPrompt;
        set => _interactionPrompt = value;
    }

    public bool Interact(Interactor interactor)
    {
        // Pobieramy wszystkie obiekty szybkiej podró¿y z rodzica
        _fastTravels = new GameObject[_fastTravelsParent.transform.childCount];
        for (int i = 0; i < _fastTravelsParent.transform.childCount; i++)
        {
            _fastTravels[i] = _fastTravelsParent.transform.GetChild(i).gameObject;
        }

        if (interactor.name == "PlayerPirat")
        {
            Debug.Log(_promptPlayer);
            PlayerController.isTeleporting = true;

            // Wybieramy losowy obiekt szybkiej podró¿y, inny ni¿ ten, na którym interakcja by³a dokonana
            GameObject randomFastTravel;
            do
            {
                randomFastTravel = _fastTravels[Random.Range(0, _fastTravels.Length)];
            } while (randomFastTravel == gameObject); // Upewniamy siê, ¿e to nie ten sam obiekt

            // Przesuwamy gracza do nowej pozycji, odejmuj¹c 1 od X i Z
            Vector3 newPosition = randomFastTravel.transform.position;
            newPosition.x -= 3;
            newPosition.z -= 1;
            newPosition.y = _terrain.SampleHeight(new Vector3(newPosition.x, 0, newPosition.z));

            _player.GetComponent<Animator>().enabled = false;
            // Ustawiamy now¹ pozycjê gracza
            _player.transform.position = newPosition;

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
            return _promptPlayer;
        }
        else
        {
            return "";

        }
    }
}
