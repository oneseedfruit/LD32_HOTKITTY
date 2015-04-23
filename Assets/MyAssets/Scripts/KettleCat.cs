using UnityEngine;
using System.Collections;

public class KettleCat : MonoBehaviour
{
    WheelJoint2D[] wj2DKettleCat;
    JointMotor2D jm2DKettleCatFront;
    JointMotor2D jm2DKettleCatBack;
    Animator animKettleCat;
    AudioSource audioCat;
    Rigidbody2D rb2DCat;

    public static Transform trKettleCat;
    public static bool isFacingLeft = true;

    public SpriteRenderer srKettle;
    public SpriteRenderer srMouth;
    public Sprite spriteMouthSmile;
    public Sprite spriteMouthBlow;
    public AudioClip audioBlow;
    public Sprite[] spriteSteam = new Sprite[13];
    public float speed;

    public enum State { Idle, Blow, Dead };
    public State KettleState;

    KettleCatTail kctTail;
    SpriteRenderer sprKettle;    

    float temp = 0;
    float coalVal;
    float coalValAccumulated;

	void Awake ()
    {
        wj2DKettleCat = GetComponentsInChildren<WheelJoint2D>();
        animKettleCat = GetComponent<Animator>();
        trKettleCat = GetComponent<Transform>();
        kctTail = GetComponentInChildren<KettleCatTail>();
        sprKettle = GetComponentInChildren<SpriteRenderer>();
        audioCat = GetComponent<AudioSource>();
        rb2DCat = GetComponent<Rigidbody2D>();
        coalValAccumulated = KettleCatTail.coalValAccumulated;
        Cockroach.score = 0;
    }
    
    // Use this for initialization
	void Start () 
    {
        StartCoroutine(AnimateKettle());
        StartCoroutine(ControlKettle());
        StartCoroutine(ReactToTemp());
        coalVal = Random.Range(0.05f, 0.1f);
	}
	
	// Update is called once per frame
	void Update () 
    {
        if (!Mathf.Equals(rb2DCat.velocity.x, 0f))
            if (audioCat != null)
                if (Random.Range(0, 2) > 1)
                    if (!audioCat.isPlaying)
                    {
                        audioCat.priority = 199;
                        audioCat.Play();
                    }

        coalValAccumulated = KettleCatTail.coalValAccumulated;
        if (kctTail.cockroachAssimilationState != KettleCatTail.AssimiliationState.Absorbing)
        {
            if (Input.GetAxis("Horizontal") > 0)
            {
                isFacingLeft = false;
                transform.eulerAngles = new Vector3(0, 180f, 0);
            }
            if (Input.GetAxis("Horizontal") < 0)
            {
                isFacingLeft = true;
                transform.eulerAngles = new Vector3(0, 0, 0);
            }

            if (Input.GetAxis("Jump") > 0 && KettleState == State.Blow)
            {
                if (KettleCatTail.coalValAccumulated > 0)
                {
                    Sprite blowSteam;
                    for (int i = 0; i < spriteSteam.Length; i++)
                    {
                        if (i == Random.Range(0, spriteSteam.Length - 1))
                        {
                            blowSteam = spriteSteam[i];
                            GameObject steamShot = new GameObject();
                            steamShot.name = "Hot Steam";
                            steamShot.tag = "Steam";
                            Vector2 right;
                            if (!isFacingLeft)
                                right = new Vector2(srMouth.transform.position.x, srMouth.transform.position.y);
                            else
                                right = new Vector2(srMouth.transform.position.x, srMouth.transform.position.y);
                            steamShot.transform.position = right;
                            steamShot.AddComponent<Steam>();
                            steamShot.AddComponent<SpriteRenderer>().sprite = blowSteam;

                            AudioSource audioSteam = steamShot.AddComponent<AudioSource>();
                            audioSteam.clip = audioBlow;
                            audioSteam.playOnAwake = true;
                            audioSteam.priority = 200;
                            audioSteam.bypassReverbZones = false;
                            audioSteam.reverbZoneMix = 0f;
                            audioSteam.volume = 0.05f;
                            if (!audioSteam.isPlaying)
                                audioSteam.Play();

                            KettleCatTail.coalValAccumulated -= coalVal;
                            break;
                        }
                    }
                }                
             
                GameObject coalConsuming;
                float coalCost = coalValAccumulated - KettleCatTail.coalValAccumulated;
                for (int h = 0; h < kctTail.coalStorage.Count; h++)
                {
                    if (Random.Range(0, kctTail.coalStorage.Count - 1) == h)
                    {
                        coalConsuming = kctTail.coalStorage[h];                        
                        if (coalConsuming.transform.localScale.x <= 0f || coalConsuming.transform.localScale.y <= 0f)
                        {   
                            kctTail.coalStorage.Remove(kctTail.coalStorage[h]);
                            Destroy(coalConsuming);                            
                        }
                        else
                        {                            
                            coalConsuming.transform.localScale = new Vector2(coalConsuming.transform.localScale.x - 0.1f * coalCost, coalConsuming.transform.localScale.y - 0.1f * coalCost);
                        }
                        break;
                    }
                }                

                coalVal = Random.Range(0.05f, 0.1f);
            }
        }

        if (KettleCatTail.coalValAccumulated <= 0f)
        {
            if (Input.anyKey)
            {
                Cockroach.score = 0;
                Application.LoadLevel(0);
            }
        }
	}   

    void FixedUpdate ()
    {        
        jm2DKettleCatFront.motorSpeed = Input.GetAxis("Horizontal") * speed;
        jm2DKettleCatFront.maxMotorTorque = 300f;

        jm2DKettleCatBack.motorSpeed = Input.GetAxis("Horizontal") * speed;
        jm2DKettleCatBack.maxMotorTorque = 300f;

        for (int i = 0; i < wj2DKettleCat.Length; i++)
        {
            if (i == 0)
                wj2DKettleCat[i].motor = jm2DKettleCatFront;
            else
                wj2DKettleCat[i].motor = jm2DKettleCatBack;
        }
    }    

    IEnumerator ControlKettle ()
    {
        while (true)
        {
            if (Input.GetAxis("Jump") > 0)
            {
                yield return new WaitForEndOfFrame();
                KettleState = State.Blow;
            }
            else
            {
                yield return new WaitForSeconds(0.2f);
                KettleState = State.Idle;
            }
            yield return new WaitForEndOfFrame();
        }
    }

    IEnumerator AnimateKettle ()
    {
        while (true)
        {
            if (KettleState == State.Idle)
            {
                animKettleCat.SetBool("isBlowing", false);
                srMouth.sprite = spriteMouthSmile;
            }
            else if (KettleState == State.Blow)
            {
                animKettleCat.SetBool("isBlowing", true);
                srMouth.sprite = spriteMouthBlow;
            }
            yield return new WaitForEndOfFrame();
        }
    }

    IEnumerator ReactToTemp ()
    {
        while (true)
        {
            float pctCoalAccum = KettleCatTail.coalValAccumulated / kctTail.coalCapacity;
            sprKettle.color = new Color(sprKettle.color.r, 1 - pctCoalAccum, 1 - pctCoalAccum, sprKettle.color.a);

            Sprite ejectSteam;
            for (int i = 0; i < spriteSteam.Length; i++)
            {
                if (i == Random.Range(0, spriteSteam.Length - 1) && pctCoalAccum > 0.5f)
                {
                    ejectSteam = spriteSteam[i];
                    GameObject steamShot = new GameObject();
                    steamShot.name = "Evaporated Steam";
                    steamShot.tag = "Evaporated Steam";
                    Vector2 up;
                        up = new Vector2(transform.position.x, transform.position.y + 2.5f);                    
                    steamShot.transform.position = up;
                    steamShot.AddComponent<Steam>();
                    steamShot.AddComponent<SpriteRenderer>().sprite = ejectSteam;
                    steamShot.GetComponent<Rigidbody2D>().AddForce(Vector2.up * 5f, ForceMode2D.Impulse);
                    break;
                }
            }

            yield return new WaitForEndOfFrame();
        }
    }
}
