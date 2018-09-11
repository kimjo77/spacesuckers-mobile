using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonTest : MonoBehaviour {

    public RectTransform parentContainer;

    public void AddButton(Button buttonPrefab) {
        Button buttonObject = Instantiate(buttonPrefab);
        buttonObject.transform.SetParent(parentContainer, false);
    }
}
