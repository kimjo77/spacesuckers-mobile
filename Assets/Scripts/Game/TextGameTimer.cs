using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextGameTimer : MonoBehaviour {

    private Text gameTimer = null;
    private Global global = null;
	
	// Update is called once per frame
	void Update() {
        if(global != null) {
            gameTimer.enabled = true;
            gameTimer.text = global.getCoundownTimerTime();
        } else {
            findGlobalGameObject();
            gameTimer.enabled = false;
        }
	}

    public void findGlobalGameObject() {
        gameTimer = GetComponent<Text>();
        GameObject globalObject = GameObject.FindGameObjectWithTag("Global");
        if(globalObject != null) {
            global = globalObject.GetComponent<Global>();
        }
    }
}
