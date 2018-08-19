using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : MonoBehaviour {

	// Use this for initialization
	void Start () {
        //this.GetComponent<Rigidbody2D>().AddForce(new Vector2(100,0), ForceMode2D.Impulse);
        //Vector2 initForce = 
        //this.GetComponent<Rigidbody2D>().AddForce(new Vector2(10,0), ForceMode2D.Impulse);
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        #region REAL PHYSICS
        /*GameObject[] blackholes = GameObject.FindGameObjectsWithTag("Blackhole");
        if(blackholes != null) {
            foreach(GameObject obj in blackholes) {
                Blackhole blackhole = obj.GetComponent<Blackhole>();
                Rigidbody2D blackholeRigid = obj.GetComponent<Rigidbody2D>();


                // METHOD_1

                //float r = Vector2.Distance(obj.transform.position, this.transform.position);
                //float totalForce = (blackhole.gravity * blackholeRigid.mass * this.GetComponent<Rigidbody2D>().mass) / (r * r);
                //Vector2 force = (obj.transform.position - this.transform.position).normalized * totalForce;

                // METHOD_2
                /*float r = Vector2.Distance(obj.transform.position, this.transform.position);
                if(r <= blackhole.radius) {
                    Debug.Log(r);
                    this.GetComponent<Rigidbody2D>().AddForce(this.transform.position * -Mathf.Sqrt(blackhole.gravity * blackholeRigid.mass / r));
                }*/

                // METHOD_3
                /*float r = Vector2.Distance(obj.transform.position, this.transform.position);
                if(r <= blackhole.radius) {
                    Debug.Log(r);
                    this.GetComponent<Rigidbody2D>().AddForce(this.transform.position * -Mathf.Sqrt(2 * blackhole.gravity * blackholeRigid.mass / r));
                }*/

                //float dist = difference.magnitude;
                //Vector3 gravityDirection = difference.normalized;
                //Vector3 gravityVector = (gravityDirection * blackhole.gravity) / (dist * dist);

                //this.GetComponent<Rigidbody2D>().AddForce(force);
            //}
        //}
        #endregion


    }
}
