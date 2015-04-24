using UnityEngine;
using System.Collections;

public class Steam : MonoBehaviour {

    public float dmgVal;

    BoxCollider2D colSteam;
    SpriteRenderer srSteam;
    Rigidbody2D rb2DSteam;

    void Awake ()
    {        
        transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        colSteam = gameObject.AddComponent<BoxCollider2D>();
        colSteam.offset = new Vector2(-0.07265925f, 0.04843903f);
        colSteam.size = new Vector2(4.439236f, 4.293915f);
        colSteam.isTrigger = true;
        rb2DSteam = gameObject.AddComponent<Rigidbody2D>();
        rb2DSteam.gravityScale = 0.1f;
        dmgVal = Random.value * 10;
    }

	// Use this for initialization
	void Start () {
        srSteam = GetComponent<SpriteRenderer>();
        srSteam.sortingLayerName = "Steam";
        srSteam.color = new Color(srSteam.color.r, srSteam.color.g, srSteam.color.b, srSteam.color.a * 0.5f);        
        StartCoroutine(Disappear(srSteam));
	}
	
	// Update is called once per frame
	void Update () {
        dmgVal = Random.value * 10;
	}

    void OnTriggerEnter2D (Collider2D col)
    {
        
    }

    IEnumerator Disappear(SpriteRenderer srSteam)
    {
        float i = 1.0f;
        while (i > 0f)
        {
            i -= 1.5f * Time.deltaTime;            

            if (this.tag != "Evaporated Steam")
            {
                srSteam.color = new Color(srSteam.color.r, srSteam.color.g, srSteam.color.b, i);

                if (!KettleCat.isFacingLeft)
                {
                    rb2DSteam.AddForce(Vector2.right * i, ForceMode2D.Impulse);
                }
                else if (KettleCat.isFacingLeft)
                {
                    rb2DSteam.AddForce(-Vector2.right * i, ForceMode2D.Impulse);
                }

                float j = 1 - i;
                transform.localScale = new Vector2(j, j);
            }
            else if (this.tag == "Evaporated Steam")
            {
                transform.localScale = Vector2.one * 0.5f;
                srSteam.color = new Color(srSteam.color.r, srSteam.color.g, srSteam.color.b, 0.05f * i);
                int k = Random.value > 0.5f ? 1 : -1;
                rb2DSteam.AddForce(k * Vector2.right * 50f);
            }

            yield return new WaitForEndOfFrame();
        }

        if (srSteam.color.a <= 0f)
        {
            Destroy(this.gameObject);
            StopAllCoroutines();
        }

        yield break;
    }
}
