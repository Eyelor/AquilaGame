using UnityEngine;
using TMPro;

[System.Serializable]
public class Particle
{
    public GameObject particlePrefab;
    public float particleLifetime; // Czas �ycia ka�dej cz�steczki
    public float particleSpeed;    // Pr�dko�� ruchu cz�steczki
    public string color;           // Nazwa koloru cz�steczki
}

public class NumberParticleEmitter : MonoBehaviour
{
    [SerializeField] internal Particle[] particles;
    public Vector3 spawnOffset = Vector3.up; // Przesuni�cie pozycji pojawiania si� cz�steczki

    public void EmitNumberParticle(string particleText, string color)
    {
        // Znajd� prefabrykat dla danego koloru
        Particle selectedParticle = null;
        foreach (Particle particle in particles)
        {
            if (particle.color == color)
            {
                selectedParticle = particle;
                break;
            }
        }

        // Je�li nie znaleziono cz�steczki dla danego koloru, wyjd� z metody
        if (selectedParticle == null)
        {
            Debug.LogWarning("Brak cz�steczki dla koloru: " + color);
            return;
        }

        // Ustawienie rotacji cz�steczki
        Quaternion particleRotation = Quaternion.Euler(45, -45, 0);

        GameObject particleInstance = Instantiate(
            selectedParticle.particlePrefab,
            transform.position + spawnOffset,
            particleRotation
        );

        // Ustawienie tekstu w TextMeshPro na liczb�
        TextMeshPro tmp = particleInstance.GetComponent<TextMeshPro>();
        if (tmp != null)
        {
            tmp.text = particleText;
        }

        // Dodanie ruchu do cz�steczki
        Rigidbody rb = particleInstance.AddComponent<Rigidbody>();
        rb.useGravity = false;
        rb.velocity = Vector3.up * selectedParticle.particleSpeed; // Ruch cz�steczki w g�r�

        // Zniszczenie cz�steczki po okre�lonym czasie
        Destroy(particleInstance, selectedParticle.particleLifetime);
    }
}
