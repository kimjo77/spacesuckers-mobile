using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextGameScore : MonoBehaviour {

    private Text gameScore = null;
    public Image icon;
    public Player player;
    private Global global = null;

    // Update is called once per frame
    void Update() {
        if(global != null) {
            gameScore.enabled = true;
            icon.enabled = true;
            gameScore.text = global.GetScore(player) + "";
        } else {
            findGlobalGameObject();
            gameScore.enabled = false;
            icon.enabled = false;
        }
    }

    public void findGlobalGameObject() {
        gameScore = GetComponent<Text>();
        GameObject globalObject = GameObject.FindGameObjectWithTag("Global");
        if(globalObject != null) {
            global = globalObject.GetComponent<Global>();
        }
    }
}
