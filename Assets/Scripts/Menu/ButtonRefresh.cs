using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonRefresh : MonoBehaviour {

    public RectTransform viewPortLobbyContainer;
    public RectTransform contentLobbyPrefabContainer;

    public void RefreshScrollView() {
        contentLobbyPrefabContainer.transform.SetParent(viewPortLobbyContainer,false);
    }
}
