using UnityEngine;
using TMPro;

[System.Serializable]
public class Particle
{
    public GameObject particlePrefab;
    public float particleLifetime; // Czas ¿ycia ka¿dej cz¹steczki
    public float particleSpeed;    // Prêdkoœæ ruchu cz¹steczki
    public string color;           // Nazwa koloru cz¹steczki
}

public class NumberParticleEmitter : MonoBehaviour
{
    [SerializeField] internal Particle[] particles;
    public Vector3 spawnOffset = Vector3.up; // Przesuniêcie pozycji pojawiania siê cz¹steczki

    public void EmitNumberParticle(string particleText, string color)
    {
        // ZnajdŸ prefabrykat dla danego koloru
        Particle selectedParticle = null;
        foreach (Particle particle in particles)
        {
            if (particle.color == color)
            {
                selectedParticle = particle;
                break;
            }
        }

        // Jeœli nie znaleziono cz¹steczki dla danego koloru, wyjdŸ z metody
        if (selectedParticle == null)
        {
            Debug.LogWarning("Brak cz¹steczki dla koloru: " + color);
            return;
        }

        // Ustawienie rotacji cz¹steczki
        Quaternion particleRotation = Quaternion.Euler(45, -45, 0);

        GameObject particleInstance = Instantiate(
            selectedParticle.particlePrefab,
            transform.position + spawnOffset,
            particleRotation
        );

        // Ustawienie tekstu w TextMeshPro na liczbê
        TextMeshPro tmp = particleInstance.GetComponent<TextMeshPro>();
        if (tmp != null)
        {
            tmp.text = particleText;
        }

        // Dodanie ruchu do cz¹steczki
        Rigidbody rb = particleInstance.AddComponent<Rigidbody>();
        rb.useGravity = false;
        rb.velocity = Vector3.up * selectedParticle.particleSpeed; // Ruch cz¹steczki w górê

        // Zniszczenie cz¹steczki po okreœlonym czasie
        Destroy(particleInstance, selectedParticle.particleLifetime);
    }
}
