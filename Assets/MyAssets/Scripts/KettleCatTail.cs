using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class KettleCatTail : MonoBehaviour {
    CoalChamber coalChamber;

    public enum AssimiliationState { Absorbing, NotAbsorbing };
    public AssimiliationState cockroachAssimilationState;

    public Sprite spriteCoal;
    
    public static float coalValAccumulated = 20f;
    public float coalCapacity = 100f;

    public List<GameObject> coalStorage;

    void Awake ()
    {
        coalValAccumulated = 20f;
        coalChamber = GetComponentInParent<CoalChamber>();
        cockroachAssimilationState = AssimiliationState.NotAbsorbing;
    }

	// Use this for initialization
	void Start () {
	    
	}
	
	// Update is called once per frame
	void Update () {
        coalValAccumulated -= 0.5f * Random.value * Time.deltaTime;
	}

    void OnTriggerStay2D (Collider2D col)
    {
        if (col.tag == "Bug" && col.isTrigger)
        {            
            Cockroach cockroach;

            if (col.gameObject != null)
            {
                cockroach = col.gameObject.GetComponent<Cockroach>();

                if (cockroach.CockroachState == Cockroach.State.Dead)
                {
                    col.enabled = false;
                    StartCoroutine(AbsorbCockroach(col));
                    coalValAccumulated -= 0.5f * Random.value;
                }
            }
        }
    }

    IEnumerator AbsorbCockroach (Collider2D col)
    {
        float i = 1.0f;
        while (i > 0f)
        {
            i -= Time.deltaTime;
            col.transform.localScale = new Vector2(transform.localScale.x * 0.4f * i, transform.localScale.y * 0.4f * i);

            transform.localScale = new Vector2(i + 1f, i + 1f);
            
            if (col.GetComponent<Rigidbody2D>() != null)
                col.GetComponent<Rigidbody2D>().AddForce((transform.position - col.transform.position) * 0.3f, ForceMode2D.Impulse);
            cockroachAssimilationState = AssimiliationState.Absorbing;
            yield return new WaitForEndOfFrame();
        }
        while (i < 1.0f)
        {
            i += 5f * Time.deltaTime;

            transform.localScale = new Vector2(i, i);
            
            yield return new WaitForEndOfFrame();
        }

        GameObject coal = new GameObject();        
        coalStorage.Add(coal);
        float coalVal = Random.value * 3f;
        float x = coalVal * 0.01f;
        if (coalValAccumulated <= coalCapacity)
            coalValAccumulated += coalVal;
        coal.transform.position = coalChamber.colCoalChamber.bounds.center;
        coal.transform.SetParent(coalChamber.trCoalChamber);
        coal.transform.localScale = new Vector2(x, x);
        coal.name = "Coal";
        coal.layer = 12;

        SpriteRenderer sprCoal = coal.AddComponent<SpriteRenderer>();
        sprCoal.sortingLayerName = "Player";
        sprCoal.sortingOrder = 2;        
        sprCoal.color = Color.black;
        sprCoal.sprite = spriteCoal;
        coal.AddComponent<CircleCollider2D>();
        Rigidbody2D rb2DCoal = coal.AddComponent<Rigidbody2D>();
        rb2DCoal.drag = 0.2f;
        rb2DCoal.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        Destroy(col.gameObject);
        cockroachAssimilationState = AssimiliationState.NotAbsorbing;
        yield break;
    }
}
