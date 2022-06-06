using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class HeelsController : MonoBehaviour
{
    public PlayerController player;
    public ParticleSystem shoeParticle;
    public ParticleSystem shoeDropPart;
    public Transform leftHeel;
    public Transform rightHeel;
    public Heel leftHeelpref;
    public Heel rightHeelpref;
    public List<Heel> heelsL;
    public List<Heel> heelsR;
    public float heelOffset = -1;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
       
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Shoe"))
        {
            ParticleSystem particle = Instantiate(shoeParticle, other.transform.position, Quaternion.identity);
            FindObjectOfType<AudioManager>().Play("Ding");
            Destroy(other.gameObject);
            AddHeel();
        }

        if (other.CompareTag("Obstacle"))
        {
            ParticleSystem particle = Instantiate(shoeDropPart, other.transform.position, Quaternion.identity);
            FindObjectOfType<AudioManager>().Play("Pop");
            Damage();
        }
    }

    void AddHeel()
    {
        transform.Translate(Vector3.up * Mathf.Abs(heelOffset));
        
        Heel heelL = Instantiate(leftHeelpref, leftHeel);
        Heel heelR = Instantiate(rightHeelpref, rightHeel);
        heelL.transform.localPosition = new Vector3(heelL.transform.localPosition.x, heelL.transform.localPosition.y, heelL.transform.localPosition.z + heelsL.Count * heelOffset);
        heelR.transform.localPosition = new Vector3(heelR.transform.localPosition.x, heelR.transform.localPosition.y, heelR.transform.localPosition.z + heelsR.Count * heelOffset);
        heelsL.Add(heelL);
        heelsR.Add(heelR);
    }

    public void Damage()
    {
        if (heelsL.Count > 0 && heelsR.Count > 0)
        {
            Heel leftHeel = heelsL[heelsL.Count - 1];
            heelsL.Remove(leftHeel);
            Destroy(leftHeel.gameObject);

            Heel rightHeel = heelsR[heelsR.Count - 1];
            heelsR.Remove(rightHeel);
            Destroy(rightHeel.gameObject);

           
        }
        else
        {
            player.Death();
        }
    }

    public void Multiplier(Multiplier mult)
    {
        if (heelsL.Count > 0 && heelsR.Count > 0)
        {
            Heel leftHeel = heelsL[heelsL.Count - 1];
            heelsL.Remove(leftHeel);
            Destroy(leftHeel.gameObject);

            Heel rightHeel = heelsR[heelsR.Count - 1];
            heelsR.Remove(rightHeel);
            Destroy(rightHeel.gameObject);


            if(heelsL.Count == 0 && heelsR.Count == 0 && mult.isFinish == false)
            {
                player.Finish();
                player.nextLevel.SetActive(true);
            }
            
        }
        
    }

    public void HeelControl()
    {
        foreach(Heel heel in heelsL)
        {
            heel.transform.parent = null;
            heel.gameObject.AddComponent<Rigidbody>();
        }

        foreach (Heel heel in heelsR)
        {
            heel.transform.parent = null;
            heel.gameObject.AddComponent<Rigidbody>();
        }
    }
}