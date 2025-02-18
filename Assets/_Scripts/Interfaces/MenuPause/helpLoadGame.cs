using System.Collections;
using UnityEngine;

public class helpLoadGame : MonoBehaviour
{
    private void OnEnable()
    {
        Time.timeScale = 1f;
        StartCoroutine(WaitForSecondsCoroutine(0.5f));
    }

    private IEnumerator WaitForSecondsCoroutine(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        Time.timeScale = 0f;
    }

}
