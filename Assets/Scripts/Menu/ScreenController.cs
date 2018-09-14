using UnityEngine;
using UnityEngine.SceneManagement;

public class ScreenController : MonoBehaviour {

    public void TransitionToScene(string scene) {
        Debug.Log(SceneManager.GetActiveScene().name + "");
        SceneManager.LoadScene(scene);
    }
}
