using UnityEngine;
using System.Collections;

public class CoalChamber : MonoBehaviour {

    public Transform trCoalChamber;
    public BoxCollider2D colCoalChamber;

    void Awake ()
    {
        trCoalChamber = GetComponent<Transform>();
        colCoalChamber = GetComponent<BoxCollider2D>();
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
