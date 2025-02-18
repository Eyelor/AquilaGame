using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class deleteSave : MonoBehaviour
{
    public GameObject exitPanel; // Obiekt panelu UI do wy�wietlenia przy wyj�ciu
    public GameObject exitBackgroundPanel; // Panel przezroczystosci t�a przy wyj�ciu
    public TextMeshProUGUI nameOfSave; // Referencja do nazwy zapisu gry
    public TextMeshProUGUI nameOfPanel; // Referencja do teksu wy�wietlanego na panelu o potwierdzeniu usuni�cia zapisu
    public GameObject saveToDelete; // Referencja do obiektu zapisu, kt�ry b�dzie usuwany

    private void Awake()
    {
        // Upewnienie si�, �e panel jest domy�lnie niewidoczny
        if (exitPanel != null)
        {
            exitPanel.SetActive(false);
        }
        // Upewnienie si�, �e panel t�a przezroczysto�ci jest domy�lnie niewidoczny
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
                nameOfPanelComponent.text = "Usun�� " + nameOfSave.text.ToLower() + "?";
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
            // Nadaj nazw� rodzica obiektu, do kt�rego przypisany jest ten skrypt, dla obiektu saveToDelete
            saveToDelete.name = transform.parent.name;
        }
    }

    // Usuwanie aktywno�ci pod plansz� wyj�cia
    private void DisableBoxCollider2D()
    {
        // Znajd� wszystkie obiekty zawieraj�ce komponent "BoxCollider2D"
        BoxCollider2D[] scriptsToDisable = FindObjectsOfType<BoxCollider2D>();

        // Przejd� przez wszystkie znalezione skrypty i wy��cz je
        foreach (BoxCollider2D script in scriptsToDisable)
        {
            // Debug.Log(script);
            script.enabled = false;
        }
    }
}
