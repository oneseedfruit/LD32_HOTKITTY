using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Cockroachmaker : MonoBehaviour {

    public GameObject InstantiateThis;
    public Text txtScore;

    Cockroach cockroach;

    void Awake ()
    {
        if (InstantiateThis.tag == "Bug")
        {
            if (InstantiateThis.gameObject != null)
            {
                cockroach = InstantiateThis.GetComponent<Cockroach>();
            }
        }
    }

	// Use this for initialization
	void Start () {
        StartCoroutine(MakeCockroachTimer());
	}
	
	// Update is called once per frame
	void Update () {
        Debug.Log(KettleCatTail.coalValAccumulated);
        if (KettleCatTail.coalValAccumulated > 0 && Cockroach.score > 0)
        {
            if (Cockroach.score == 1)
            {
                txtScore.text = "YOU'VE CONSUMED " + Cockroach.score + " BUG!";
            }
            else if (Cockroach.score > 1)
            {
                txtScore.text = "YOU'VE CONSUMED " + Cockroach.score + " BUGS!";
            }
        }
        else if (KettleCatTail.coalValAccumulated <= 0)
            txtScore.text = "YOU'RE NOT HOT ANYMORE. GAME OVER. YOUR FINAL SCORE IS " + Cockroach.score + " PRESS ANY TO RESTART.";
	}

    IEnumerator MakeCockroachTimer ()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(0.5f, 5f));
            yield return StartCoroutine(MakeCockroach());
            yield return new WaitForEndOfFrame();
        }
    }

    IEnumerator MakeCockroach ()
    {
        float sizeVal = Random.Range(0.2f, 0.4f);
        cockroach.transform.localScale = new Vector2(sizeVal, sizeVal);
        if (Cockroach.CockroachCount <= 100f)
            Instantiate(cockroach, new Vector2(Random.Range(-15f, 15f), Random.Range(5f, 15f)), Quaternion.identity);
        yield break;
    }
}
