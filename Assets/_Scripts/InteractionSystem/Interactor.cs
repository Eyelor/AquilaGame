using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Interactor : MonoBehaviour
{
    [SerializeField] private Transform _interactionPoint;
    [SerializeField] private float _interactionRadius = 2f;
    [SerializeField] private LayerMask _interactableMask;
    [SerializeField] private InteractionPromptUI _interactionPromptUI;
    [SerializeField] private bool _isFighting = false;

    private List<Collider> _colliders = new List<Collider>();

    private IInteractable _interactable;

    private void Start()
    {
        GameInputSystem.Instance.OnInteractionAction += GameInputSystem_OnInteractionAction;
    }

    private void GameInputSystem_OnInteractionAction(object sender, System.EventArgs e)
    {
        // Sprawdzamy, czy gracz jest w walce
        if (_isFighting)
        {
            Debug.Log("Player is fighting, you can't interact.");
            return; // Przerywamy dalsze wykonywanie, jeœli gracz walczy
        }

        // Wykonaj interakcjê, jeœli jest mo¿liwa
        if (_interactable != null)
        {
            _interactable.Interact(this);
        }
    }

    private void Update()
    {
        if (gameObject.GetComponent<PlayerController>() != null)
        {
            _isFighting = gameObject.GetComponent<PlayerController>().statistics.isFighting;
        }
        _colliders.Clear();
        Collider[] results = Physics.OverlapSphere(_interactionPoint.position, _interactionRadius, _interactableMask);
        _colliders.AddRange(results);

        if (_colliders.Count > 0)
        {
            Collider closestCollider = null;
            float closestDistanceSqr = float.MaxValue;

            foreach (Collider collider in _colliders)
            {
                Vector3 directionToCollider = collider.ClosestPoint(_interactionPoint.position) - _interactionPoint.position;
                float distanceSqr = directionToCollider.sqrMagnitude;

                if (distanceSqr < closestDistanceSqr)
                {
                    closestDistanceSqr = distanceSqr;
                    closestCollider = collider;
                }
            }

            if (closestCollider != null)
            {
                Debug.Log(closestCollider.name + " is " + closestDistanceSqr + " m away");
                if (closestCollider.name.StartsWith("MiniIslandPort"))
                {
                    gameObject.GetComponent<CheckIsland>().checkMiniIsland(closestCollider.name, "brak");
                }
                else if (closestCollider.name.StartsWith("MiniIsland"))
                {
                    gameObject.GetComponent<CheckIsland>().checkMiniIsland(closestCollider.name, closestCollider.GetComponent<IslandDataComponent>().islandData.affiliation);
                }
                _interactable = closestCollider.GetComponent<IInteractable>();

                if (_interactable != null)
                {
                    if (_isFighting)
                    {
                        _interactable.InteractionPrompt = "Gracz jest w stanie walki";
                    } else
                    {
                        string prompt = _interactable.GetPrompt(name);
                        if (closestCollider.name.StartsWith("MiniIslandPort"))
                        {
                            _interactable.InteractionPrompt = prompt;
                        }
                        else
                        {
                            _interactable.InteractionPrompt = "NACIŒNIJ " + GameInputSystem.Instance.GetBindingText(GameInputSystem.Binding.interact) + ", ABY " + prompt;
                        }
                    }                   
                    _interactionPromptUI.Close();

                    if (!_interactionPromptUI.IsDisplayed) _interactionPromptUI.SetUp(_interactable.InteractionPrompt);
                }
            }
        }
        else
        {
            if (_interactionPromptUI != null) _interactable = null;

            if (_interactionPromptUI.IsDisplayed) _interactionPromptUI.Close();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(_interactionPoint.position, _interactionRadius);
    }

    private void OnDestroy()
    {
        if (GameInputSystem.Instance != null)
        {
            GameInputSystem.Instance.OnInteractionAction -= GameInputSystem_OnInteractionAction;
            Debug.Log($"[Interactor-{name}] OnInteractionAction has {GameInputSystem.Instance.GetOnInteractionActionSubscribersCount()} subscribers after OnDestroy.");
        }
    }
}
