using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Cockroach : MonoBehaviour {

    public enum State { Alive, Dead };
    public State CockroachState;

    public static int CockroachCount = 0;
    public static int score = 0;

    WheelJoint2D[] wj2DCockroach;
    JointMotor2D jm2DCockroachLeft;
    JointMotor2D jm2DCockroachRight;
    Rigidbody2D rb2DCockroach;
    Collider2D col2DCockroach;
    SpriteRenderer[] srCockroach;
    AudioSource audioCockroach;

    float healthVal;    
    float speed = 0;
    
    void Awake ()
    {
        CockroachState = State.Alive;
        CockroachCount++;

        wj2DCockroach = GetComponentsInChildren<WheelJoint2D>();
        rb2DCockroach = GetComponent<Rigidbody2D>();
        col2DCockroach = GetComponent<Collider2D>();        
        srCockroach = GetComponentsInChildren<SpriteRenderer>();
        audioCockroach = GetComponent<AudioSource>();

        healthVal = Random.value * 350f * transform.localScale.x;
    }

	// Use this for initialization
	void Start () {
        float direction = Random.value >= 0.5f ? 1f : -1f;
        speed = direction * Random.Range(100f, 500f);

        StartCoroutine(ReactToHurt());
        StartCoroutine(DieWhenKilled());
	}
	
	// Update is called once per frame
	void Update () {
        if (healthVal <= 0)
            CockroachState = State.Dead;

        if (audioCockroach != null && !audioCockroach.isPlaying && Random.value < 0.001f)
            audioCockroach.Play();
	}

    void FixedUpdate ()
    {
        if (CockroachState == State.Alive)
        {
            jm2DCockroachLeft.motorSpeed = speed;
            jm2DCockroachLeft.maxMotorTorque = 500f;

            jm2DCockroachRight.motorSpeed = speed;
            jm2DCockroachRight.maxMotorTorque = 500f;

            for (int i = 0; i < wj2DCockroach.Length; i++)
            {
                wj2DCockroach[i].motor = i == 0 ? jm2DCockroachRight : jm2DCockroachLeft;                
            }

            if (rb2DCockroach.velocity.x == 0f)
                rb2DCockroach.AddForce(Vector2.up * 3f, ForceMode2D.Impulse);
        }
        else
        {
            jm2DCockroachLeft.motorSpeed = 0;
            jm2DCockroachRight.motorSpeed = 0;

            for (int i = 0; i < wj2DCockroach.Length; i++)
            {
                wj2DCockroach[i].motor = i == 0 ? jm2DCockroachRight : jm2DCockroachLeft;
            }          

            col2DCockroach.isTrigger = true;            
        }
    }

    void OnTriggerEnter2D (Collider2D col)
    {
        if (col.tag == "Steam")
        {
            Steam steam = col.gameObject.GetComponent<Steam>();
            healthVal -= 0.5f * steam.dmgVal;            
        }
    }

    void OnCollisionEnter2D (Collision2D col)
    {
        if (col.collider.tag == "Wall" || col.collider.tag == "Bug")
            speed = -speed;

        jm2DCockroachLeft.motorSpeed = 0;
        jm2DCockroachRight.motorSpeed = 0;

        for (int i = 0; i < wj2DCockroach.Length; i++)
        {
            if (i == 0)
                wj2DCockroach[i].motor = jm2DCockroachRight;
            else
                wj2DCockroach[i].motor = jm2DCockroachLeft;
        }          
    }

    void OnDestroy ()
    {
        CockroachCount--;
        score++;
    }

    IEnumerator ReactToHurt()
    {
        while (true)
        {
            float health = healthVal;
            yield return new WaitForEndOfFrame();

            if (health > healthVal)
            {
                if (CockroachState != State.Dead)
                {
                    float i = 1.0f;
                    while (i > 0)
                    {
                        i -= 10f * Time.deltaTime;
                        for (int j = 0; j < srCockroach.Length; j++)
                        {
                            srCockroach[j].color = new Color(i, srCockroach[j].color.g - i, srCockroach[j].color.b, 2f * i);
                            if (CockroachState == State.Dead)
                                srCockroach[j].color = Color.black;
                        }
                        yield return new WaitForEndOfFrame();
                    }
                    while (i < 1.0f)
                    {
                        i += 10f * Time.deltaTime;
                        for (int j = 0; j < srCockroach.Length; j++)
                        {
                            srCockroach[j].color = new Color(Color.red.r, Color.red.g, i + Color.black.b, i);
                            if (CockroachState == State.Dead)
                                srCockroach[j].color = Color.black;
                        }
                        yield return new WaitForEndOfFrame();
                    }
                }
                else
                {
                    for (int j = 0; j < srCockroach.Length; j++)
                    {
                        srCockroach[j].color = Color.black;
                    }
                }
            }

            if (CockroachState == State.Dead)
                yield break;

            yield return new WaitForEndOfFrame();
        }
    }

    IEnumerator DieWhenKilled ()
    {
        while (true)
        {
            if (CockroachState == State.Dead)
            {
                transform.rotation = Quaternion.Euler(transform.rotation.x, transform.rotation.y, (Random.Range(-1f, 1f) * 120f));

                for (int i = 0; i < srCockroach.Length; i++ )
                {
                    srCockroach[i].color = Color.black;
                }

                yield return new WaitForEndOfFrame();

                yield break;
            }
            yield return new WaitForEndOfFrame();
        }
    }
}
