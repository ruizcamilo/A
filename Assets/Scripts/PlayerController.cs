﻿//using System.Threading.Tasks.Dataflow;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float maxspeed;
    public float dashSpeed;
    public float speed;
    public float maxDashSpeed;
    public bool grounded;
    public float jumpPower;

    //Factores de debuff
    public float debuffTime = 10f;
    public float debuffFactorSlow = 3f;
    public float debuffFactorHeavy = 3f;

    //Esto se hace para que la variable se puede acceder desde otros scripts pero no se muestra en el inspector de Unity
    [HideInInspector]
    public bool playerCanMove = true;

    //Variables privadas
    private Rigidbody2D rb2d;
    private Weapon w;
    private Animator anim;
    private Renderer rend; 
    private Color original;

    private BoxCollider2D box;

    //Debuffs
    bool _playerIsSlow = false;
    bool _playerIsHeavy = false;
    bool _playerIsSingleJump = false;


    private bool jump, walk,dash;
    private bool shoot;
    private RaycastHit2D _hit;
    private Vector2 _inputAxis;

    public LayerMask pisoLayerMask;

    // Start is called before the first frame update
    void Start()
    {
        box = transform.GetComponent<BoxCollider2D>();
        rb2d = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        w = GetComponent<Weapon>();
        rend = GetComponent<Renderer>();
        original = rend.material.color;
    }

    // Update is called once per frame
    void Update()
    {
        //Variable para evitar que se hagan actualizaciones si el jugador no se puede mover o el juego está pausado
        //Falta implementar la parte de pausar.
        if (!playerCanMove)
            return;

        anim.SetFloat("Speed", Mathf.Abs(rb2d.velocity.x));
        anim.SetBool("Grounded", IsGrounded());

        setColor();

        if(Input.GetKeyDown(KeyCode.Space))
        {
            shoot = true;
            anim.SetBool("Shoot", shoot);
        }

        //if (Input.GetKeyDown(KeyCode.UpArrow) && grounded)
        //{
        //    jump = true;
        //}
       // HandleMove();


        _inputAxis = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        if (IsGrounded())
        {

            jump = true;
            walk = true;
            dash = true;

        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            walk = false;
            if (IsGrounded())
            {
                rb2d.velocity = new Vector2(rb2d.velocity.x, 0);
                rb2d.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
                //rb2d.AddForce(new Vector2(0, jumpPower));

            }
            else
            {
                if (jump)
                {
                    rb2d.velocity = new Vector2(rb2d.velocity.x, 0);
                    rb2d.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
                    //rb2d.AddForce(new Vector2(0, jumpPower*2));

                    jump = false;
                }
            }
        }
    }

    
    void FixedUpdate(){

        float h = Input.GetAxis("Horizontal");
        if (h != 0)
        {
            if (Input.GetKeyDown(KeyCode.LeftShift)&&dash)
            {
                Debug.Log("dashprron");
                rb2d.velocity = new Vector2 (h * dashSpeed, rb2d.velocity.y);
                dash = false;
    
            }
            else
            {
                rb2d.AddForce(Vector2.right * speed * h);

                float limitedSpeed = Mathf.Clamp(rb2d.velocity.x, -maxspeed, maxspeed);
                rb2d.velocity = new Vector2(limitedSpeed, rb2d.velocity.y);
            }
            
            
        }

        if (h > 0.1f)
        {
            transform.eulerAngles = new Vector3(0, 0, 0);
        }

        if (h < -0.1f)
        {
            transform.eulerAngles = new Vector3(0, 180, 0);
        }

        //if(jump)
        //{
        //    rb2d.velocity = new Vector2(rb2d.velocity.x, 0);
        //    rb2d.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
        //    jump = false;
        //}

        if (shoot)
        {
            anim.SetBool("Shoot", false);
        }

    }

    //Slows down or returns the player to its orginal movement speed by a defined factor
    public void slowDownPlayer()
    {
        _playerIsSlow = true;
        maxspeed /= debuffFactorSlow;
        StartCoroutine(slowDownPlayerRoutine());
    }
    //Slow down player coroutine
    IEnumerator slowDownPlayerRoutine()
    {
        yield return new WaitForSecondsRealtime(debuffTime);
        maxspeed *= debuffFactorSlow;
        _playerIsSlow = false;
    }

    //Makes the player heavier 
    public void heavierPlayer()
    {

        _playerIsHeavy = true;
        rb2d.gravityScale *= debuffFactorHeavy;
        StartCoroutine(heavierPlayerRoutine());
    }

    //Make player heavier coroutine
    IEnumerator heavierPlayerRoutine()
    {
        yield return new WaitForSecondsRealtime(debuffTime);
        rb2d.gravityScale /= debuffFactorHeavy;
        _playerIsHeavy = true;

    }


    void OnBecameInvisible(){
        transform.position = new Vector3(-2,0,0);
    }

    void setColor()
    {
        switch (this.w.type)
        {
            case 0:
                this.rend.material.color = original;
                break;
            case 1:
                this.rend.material.color = Color.red;
                break;
            case 2:
                this.rend.material.color = Color.blue;
                break;
            default:
                break;
        }
    }
    private bool IsGrounded()
    {
        _hit = Physics2D.BoxCast(box.bounds.center, box.bounds.size, 0f, Vector2.down, .1f, pisoLayerMask);
        Debug.Log(_hit.collider);
        return _hit.collider != null;

    }
    
}
