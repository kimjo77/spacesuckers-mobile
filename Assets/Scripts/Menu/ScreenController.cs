using UnityEngine;
using UnityEngine.SceneManagement;

public class ScreenController : MonoBehaviour {

    public void TransitionToScene(string scene) {
        SceneManager.LoadScene(scene);
    }
}
