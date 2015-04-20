using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Cockroach : MonoBehaviour {

    WheelJoint2D[] wj2DCockroach;
    JointMotor2D jm2DCockroachLeft;
    JointMotor2D jm2DCockroachRight;
    Rigidbody2D rb2DCockroach;
    Collider2D col2DCockroach;
    SpriteRenderer[] srCockroach;
    AudioSource audioCockroach;

    int healthVal;
    public static int score = 0;
    float speed = 0;

    public enum State { Alive, Dead };
    public State CockroachState;

    public static int CockroachCount = 0;

    void Awake ()
    {
        rb2DCockroach = GetComponent<Rigidbody2D>();
        col2DCockroach = GetComponent<Collider2D>();
        wj2DCockroach = GetComponentsInChildren<WheelJoint2D>();
        srCockroach = GetComponentsInChildren<SpriteRenderer>();
        healthVal = Random.Range(1, 10);
        CockroachState = State.Alive;
        audioCockroach = GetComponent<AudioSource>();
        CockroachCount++;
    }

	// Use this for initialization
	void Start () {
        
        float direction;

        if (Random.value >= 0.5f)
            direction = 1f;
        else
            direction = -1f;

        speed = direction * Random.Range(100f, 300f);

        StartCoroutine(DieWhenKilled());
	}
	
	// Update is called once per frame
	void Update () {
        if (healthVal <= 0)
            CockroachState = State.Dead;

        if (audioCockroach != null)
            if (Random.Range (0, 1) > 0)
                if (!audioCockroach.isPlaying)
                    audioCockroach.Play();
	}

    void FixedUpdate ()
    {
        if (CockroachState == State.Alive)
        {
            jm2DCockroachLeft.motorSpeed = speed;
            jm2DCockroachLeft.maxMotorTorque = 300f;

            jm2DCockroachRight.motorSpeed = speed;
            jm2DCockroachRight.maxMotorTorque = 300f;

            for (int i = 0; i < wj2DCockroach.Length; i++)
            {
                if (i == 0)
                    wj2DCockroach[i].motor = jm2DCockroachRight;
                else
                    wj2DCockroach[i].motor = jm2DCockroachLeft;
            }
        }
        else
        {
            jm2DCockroachLeft.motorSpeed = 0;
            jm2DCockroachRight.motorSpeed = 0;

            for (int i = 0; i < wj2DCockroach.Length; i++)
            {
                if (i == 0)
                    wj2DCockroach[i].motor = jm2DCockroachRight;
                else
                    wj2DCockroach[i].motor = jm2DCockroachLeft;
            }          

            col2DCockroach.isTrigger = true;            
        }
    }

    void OnTriggerEnter2D (Collider2D col)
    {
        if (col.tag == "Steam")
        {
            Steam steam = col.gameObject.GetComponent<Steam>();
            healthVal -= steam.dmgVal;
        }
    }

    void OnCollisionEnter2D (Collision2D col)
    {
        if (col.collider.tag == "Wall")
            speed = -speed;

        if (col.collider.tag == "Player")
            if (col.gameObject.transform.position.x < transform.position.x)
                speed = Mathf.Abs(speed);
            else
                speed = -Mathf.Abs(speed);
    }

    void OnDestroy ()
    {
        CockroachCount--;
        score++;
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

                    yield break;
            }
            yield return new WaitForEndOfFrame();
        }
    }
}
