using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Animations.Rigging;
using Cinemachine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public ParticleSystem shoeDropPart;
    public Collider colliderBoard;
    public GameObject nextLevel;
    public GameObject restartMenu;
    public GameObject vCamPlayer;
    public HeelsController heelController;
    public Rigidbody playerRb;
    public Rig rigL;
    public Rig rigR;
    public Animator playerAnim;
    public ParticleSystem gemParticle;
    public TMP_Text scoreText;
    public TMP_Text LevelText;
    public TMP_Text gemsOnlvl;
    public Joystick joystick;
    private int multiplier = 1;
    private float maxSpeed = 5.0f;
    private float speed = 5.0f;
    private float sideSpeed = 3.0f;
    private float jumpForce = 200f;
    private float landForce = 7f;
    public float targertWeight;
    float horizontalMove = 0f;
    public float boards = 2.0f;
    private int score;
    private int gemOnLvl;
    public bool isFinish; 
    // Start is called before the first frame update
    void Start()
    {   
        //PlayerPrefs.DeleteKey("gems");
        playerAnim.SetBool("Walk", true);
        LevelText.text = "Level  " + SceneManager.GetActiveScene().buildIndex;
        speed = maxSpeed;
    }

    private void Awake()
    {
        
        score = PlayerPrefs.GetInt("gems");
        gemsOnlvl.text = score.ToString();
        
    }

    
    // Update is called once per frame
    void Update()
    {
        horizontalMove = joystick.Horizontal * sideSpeed;
        transform.Translate((Vector3.forward * speed + horizontalMove * Vector3.right) * Time.deltaTime);
        transform.position = new Vector3(Mathf.Clamp(transform.position.x,-boards, boards), transform.position.y, transform.position.z);

        rigL.weight = Mathf.Lerp(rigL.weight, targertWeight, Time.deltaTime * 10f);
        rigR.weight = Mathf.Lerp(rigR.weight, targertWeight, Time.deltaTime * 10f);

    }


    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent<Gem>(out Gem gem))
        {   
            if(gem.Take())
            {
                Destroy(other.gameObject);
                FindObjectOfType<AudioManager>().Play("Ding");
                ParticleSystem particle = Instantiate(gemParticle, other.transform.position, Quaternion.identity);
                gemOnLvl++;
                score++;
                gemsOnlvl.text = score.ToString();
            }
        }
        if(other.CompareTag("RailTrigger") )
        {
            heelController.HeelControl();
            Falling();
        }

        if (other.CompareTag("LandTrigger"))
        {
            Destroy(other.gameObject);
            FindObjectOfType<AudioManager>().Stop("Clap");
            playerRb.AddForce(Vector3.up * landForce, ForceMode.Impulse);
            targertWeight = 0f;
            playerAnim.SetBool("Walk", true);
        }

        if(other.CompareTag("Finish"))
        {
            Finish();
        }
        if(other.CompareTag("BoardTrigger"))
        {
            heelController.HeelControl();
            colliderBoard.isTrigger = true;
            FindObjectOfType<AudioManager>().Play("Fall");
            Falling();
        }

        if(other.CompareTag("Stairs"))
        {
            if (heelController.heelsL.Count == 0 && heelController.heelsR.Count == 0)
            {
                Death();
                return;
            }
            Multiplier mult = other.gameObject.GetComponent<Multiplier>();
            multiplier = mult.mult;
            heelController.Multiplier(mult);
            FindObjectOfType<AudioManager>().Play("Pop");
            ParticleSystem particle = Instantiate(shoeDropPart, other.transform.position, Quaternion.identity);
            score = gemOnLvl * multiplier;
            PlayerPrefs.SetInt("gems", score);
        }
    }

    public void Death()
    {
        speed = 0;
        sideSpeed = 0;
        playerAnim.Play("Death");
        restartMenu.SetActive(true);
    }

    public void Finish()
    {
        speed = 0;
        sideSpeed = 0;
        playerAnim.Play("Dance");
        FindObjectOfType<AudioManager>().Play("Win");
        NextLevel();
    }

    public void Falling()
    {
        speed = 0;
        sideSpeed = 0;
        targertWeight = 0f;
        playerAnim.SetTrigger("Fall");
        vCamPlayer.GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineTransposer>().m_YDamping = 0;

        restartMenu.SetActive(true);
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Trampoline"))
        {
            playerRb.AddForce(Vector3.up * jumpForce);
            targertWeight = 1f;
            FindObjectOfType<AudioManager>().Play("Clap");
            playerAnim.SetBool("Walk", false);
        }
        if (collision.gameObject.CompareTag("Board"))
        {
            speed = 2;
            playerAnim.Play("Balancing");
            colliderBoard = collision.gameObject.GetComponent<Collider>();
        }
        if(collision.gameObject.CompareTag("Ground"))
        {
            speed = maxSpeed;
            playerAnim.Play("Walk");
        }

    }

    public void NextLevel()
    {
        scoreText.text = "Score: " + score.ToString();
        nextLevel.SetActive(true);
    }
    
}
