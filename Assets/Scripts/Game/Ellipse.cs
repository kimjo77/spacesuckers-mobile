using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class Ellipse : MonoBehaviour {

    public int vertexCount = 50;
    public float radius = 50;

    private LineRenderer line;

    void Start() {
        line = gameObject.GetComponent<LineRenderer>();
        line.positionCount = vertexCount;
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        line.sortingLayerID = spriteRenderer.sortingLayerID;
        line.sortingOrder = spriteRenderer.sortingOrder;
    }

    void LateUpdate() {
        line.positionCount = vertexCount;
        CreatePoints();
    }

    private void CreatePoints() {

        float degreeInterval = 360 / vertexCount;

        Vector2 position;
        int i;

        for(i = 0; i < vertexCount-1; i++) {
            position = GetLocation(degreeInterval * i, radius);
            line.SetPosition(i, position);
        }
        position = GetLocation(degreeInterval * i, radius);
        line.SetPosition(i, position);
    }

    private Vector2 GetLocation(float angle, float radius) {
        float x = Mathf.Sin(Mathf.Deg2Rad * angle) * radius;
        float y = Mathf.Cos(Mathf.Deg2Rad * angle) * radius;
        return new Vector2(x, y);
    }
}
