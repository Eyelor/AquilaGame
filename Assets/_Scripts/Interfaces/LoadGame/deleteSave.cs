using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class deleteSave : MonoBehaviour
{
    public GameObject exitPanel; // Obiekt panelu UI do wyœwietlenia przy wyjœciu
    public GameObject exitBackgroundPanel; // Panel przezroczystosci t³a przy wyjœciu
    public TextMeshProUGUI nameOfSave; // Referencja do nazwy zapisu gry
    public TextMeshProUGUI nameOfPanel; // Referencja do teksu wyœwietlanego na panelu o potwierdzeniu usuniêcia zapisu
    public GameObject saveToDelete; // Referencja do obiektu zapisu, który bêdzie usuwany

    private void Awake()
    {
        // Upewnienie siê, ¿e panel jest domyœlnie niewidoczny
        if (exitPanel != null)
        {
            exitPanel.SetActive(false);
        }
        // Upewnienie siê, ¿e panel t³a przezroczystoœci jest domyœlnie niewidoczny
        if (exitBackgroundPanel != null)
        {
            exitBackgroundPanel.SetActive(false);
        }
    }
    private void OnMouseDown()
    {
        if (!AudioSystem.Instance.audioSources.effectsAudioSource.isPlaying)
        {
            AudioSystem.Instance.PlaySound(AudioSystem.Instance.audioSources.effectsAudioSource, AudioSystem.Instance.buttonForwardSound, false);
        }
        DisableBoxCollider2D();

        if (nameOfPanel != null)
        {
            // Pobierz komponent TextMeshProUGUI z obiektu nameOfPanel
            TextMeshProUGUI nameOfPanelComponent = nameOfPanel.GetComponent<TextMeshProUGUI>();

            if (nameOfPanelComponent != null && nameOfSave != null)
            {
                // Dodaj tekst z nameOfSave do deletingTextMessage
                nameOfPanelComponent.text = "Usun¹æ " + nameOfSave.text.ToLower() + "?";
            }
        }
        if (exitPanel != null)
        {
            exitPanel.SetActive(true);
        }
        if (exitBackgroundPanel != null)
        {
            exitBackgroundPanel.SetActive(true);
        }

        // Przypisanie nazwy rodzica do obiektu saveToDelete
        if (saveToDelete != null && transform.parent != null)
        {
            // Nadaj nazwê rodzica obiektu, do którego przypisany jest ten skrypt, dla obiektu saveToDelete
            saveToDelete.name = transform.parent.name;
        }
    }

    // Usuwanie aktywnoœci pod plansz¹ wyjœcia
    private void DisableBoxCollider2D()
    {
        // ZnajdŸ wszystkie obiekty zawieraj¹ce komponent "BoxCollider2D"
        BoxCollider2D[] scriptsToDisable = FindObjectsOfType<BoxCollider2D>();

        // PrzejdŸ przez wszystkie znalezione skrypty i wy³¹cz je
        foreach (BoxCollider2D script in scriptsToDisable)
        {
            // Debug.Log(script);
            script.enabled = false;
        }
    }
}
