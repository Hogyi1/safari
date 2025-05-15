using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class GameEnd : MonoBehaviour
{
    Coroutine Coroutine;
    // Update is called once per frame
    void LateUpdate()
    {
        if (TouristManager.Instance.OverallMood <= 30f && Coroutine.IsUnityNull())
        {
            Coroutine = StartCoroutine(EndGame());

        }
    }

    private IEnumerator EndGame()
    {
        GameEvents.Instance.RequestAlert(
            success: false,
            message: "You lost the game! The park will now close.",
            fadeInTime: 0.25f,
            displayTime: 4.0f,
            fadeOutTime: 0.4f
        );

        yield return new WaitForSeconds(5);

        SceneLoadManager.LoadScene("MainMenu");
    }
}
