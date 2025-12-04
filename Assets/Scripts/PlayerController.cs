using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;


public class PlayerController : MonoBehaviour
{
    Animator animator;
    private Rigidbody2D rigidBody;
    [SerializeField] private float speed = 5f;
    [SerializeField] private float jumpForce = 8f;
    [SerializeField] private float airControlForce = 10f;
    [SerializeField] private float airControlMax = 1.5f;

    private bool isGrounded;

    public TextMeshProUGUI uiText;
    int totalCoins;
    int coinsCollected;
    string curLevel;
    string nextLevel;


    private void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        coinsCollected = 0;
        totalCoins = GameObject.FindGameObjectsWithTag("Coin").Length;
        curLevel = SceneManager.GetActiveScene().name;
        if (curLevel == "Level1")
            nextLevel = "Level2";
        else if (curLevel == "Level2")
            nextLevel = "Finished";
    }

    IEnumerator DoDeath()
    {
        // freeze the rigidbody
        rigidBody.constraints = RigidbodyConstraints2D.FreezeAll;
        GetComponent<Renderer>().enabled = false;
        GetComponent<Collider2D>().enabled = false;
        yield return new WaitForSeconds(2);
        SceneManager.LoadScene(curLevel);
    }
    IEnumerator LoadNextLevel()
    {
        if (nextLevel != "Finished")
        {
            // hide the player
            GetComponent<Renderer>().enabled = false;
            yield return new WaitForSeconds(2);
            SceneManager.LoadScene(nextLevel);
        }
    }

    void Update()
    {
        float xSpeed = Mathf.Abs(rigidBody.linearVelocity.x);
        animator.SetFloat("xspeed", xSpeed);

        float ySpeed = rigidBody.linearVelocity.y;
        animator.SetFloat("yspeed", ySpeed);

        float blinkVal = Random.Range(0.0f, 200.0f);
        if (blinkVal < 1.0f)
            animator.SetTrigger("blinktrigger");

        if (rigidBody.linearVelocity.x * transform.localScale.x < 0.0f)
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);

        string uiString = "x " + coinsCollected + "/" + totalCoins;
        uiText.text = uiString;
    }
    private void FixedUpdate()
    {
        float h = Input.GetAxis("Horizontal");

        if (isGrounded)
        {
            if (Input.GetKey(KeyCode.Space))//(Input.GetAxis("Jump") > 0.1f)
                rigidBody.AddForce(new Vector2(0.0f, jumpForce), ForceMode2D.Impulse);
            else
                rigidBody.linearVelocity = new Vector2(speed * h, rigidBody.linearVelocityY);
        }
        else
        {
            float vx = rigidBody.linearVelocityX;
            if (h * vx < airControlMax)
                rigidBody.AddForce(new Vector2(h * airControlForce, 0));
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 3)
        {
            
            isGrounded = true;
        }
        
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 3)
        {

            isGrounded = false;
        }
    }

    void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.gameObject.tag == "Coin")
        {
            coinsCollected++;
            Destroy(coll.gameObject);
        }
        if (coll.gameObject.tag == "Death")
        {
            StartCoroutine(DoDeath());
        }
        if (coll.gameObject.tag == "LevelEnd")
        {
            coll.gameObject.SetActive(false);
            StartCoroutine(LoadNextLevel());
        }

    }
    

}