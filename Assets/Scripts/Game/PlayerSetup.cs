using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerSetup : NetworkBehaviour {

    public Behaviour[] componentsToDisable;

	// Use this for initialization
	void Start () {
        DisableComponents();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void DisableComponents() {
        for(int i = 0; i < componentsToDisable.Length; i++) {
            if(!isLocalPlayer) {
                if(componentsToDisable[i] != null) {
                    componentsToDisable[i].enabled = false;
                }
            }
        }
    }
}
