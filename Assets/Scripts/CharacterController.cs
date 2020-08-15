using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // include so we can load new scenes

public class CharacterController : MonoBehaviour
{
	//player's move speed 
	[Range(0.0f, 10.0f)] //Create a slider in the editor and set limits on moveSpeed
	public float moveSpeed;

	//player's jump force
	public float jumpForce = 400; 

	//player healt
	public int playerHealth = 1;

	//LayerMask determinate what is considered ground for the player
	public LayerMask whatIsGround; //Select in the dropbox the layer, also don't forget to make the layers first

	//Trasform just below feet for checking if the player is grounded
	public Transform groundCheck; //Make a gameobject under the player character and put in the editor

	//player can move?
	//We want this public so other scripts can acces it but we don't want to show in editor as it migth confuse designer
	[HideInInspector]
	public bool playerCanMove = true;


	//Private variables below

	private Transform _transform; //Store the reference of the trasform component
	private Rigidbody2D _rigidbody; //Store the reference of rigidbody component
	private Animator _animator; //Store the reference of the animator component

	//Hold player motion in this timestep or the velocity in the axis
	private float _vx;
	private float _vy;

	//Booleans to player tracking

	private bool _isFacingRight = true; //Check to change the scale to -1 to face left
	private bool _isGrounded = false; //Check if the player is on the ground (layer)
	private bool _isRunning = false; //Check if the player is Moving

	//Store the layer the player is on (setup in Awake)
	private int _playerLayer;

	//Number of layer that Platforms are on (setup in Awake)
	private int _platformLayer;

    void Awake()
    {
		//Get a reference to the component we are going to be changing and store a reference for efficiency purpose
		_transform = GetComponent<Transform>();//Reference of transform component

		_rigidbody = GetComponent<Rigidbody2D>();//Reference of rigidbody component
		//Some warmings if we don't have the rigidbody components in the gameobject	
		if (_rigidbody == null) // if Rigidbody is missing
			Debug.LogError("Rigidbody2D component missing from this gameobject");

		_animator = GetComponent<Animator>();//Reference of animator component
		//Some warmings if we don't have the animator components in the gameobject	
		if (_animator == null) // if Animator is missing
			Debug.LogError("Animator component missing from this gameobject");

		//determine the player's specified layer
		_playerLayer = this.gameObject.layer;

		//determine the plataform;s specified layer
		_platformLayer = LayerMask.NameToLayer("Platform");
	}

	void Update()
	{
		//Exit update if the player cannot move or game is paused
		if (!playerCanMove || (Time.timeScale == 0f))
        {
			return;
        }

		//Determine horizontal velocity change based on the horizontal input
		_vx = Input.GetAxisRaw("Horizontal");

		//Determine if running basede on the horizontal movement
		if(_vx != 0)
        {
			_isRunning = true;
        }
        else
        {
			_isRunning = false;
        }

		//Set the running animation state, this state is in the animator
		_animator.SetBool("Running", _isRunning); //Run or not the animation

		//Get the current vertical velocity from the rigidbody component
		_vy = _rigidbody.velocity.y;

		// Check to see if character is grounded by raycasting from the middle of the player
		// down to the groundCheck position and see if collected with gameobjects on the
		// whatIsGround layer
		_isGrounded = Physics2D.Linecast(_transform.position, groundCheck.position, whatIsGround);

		// Set the grounded animation states, this state is in the animator
		_animator.SetBool("Grounded", _isGrounded); //Run or not the animation

		//If the character is in the ground and player press the jump button, the allow to jump 
		if(_isGrounded && Input.GetButtonDown("Jump"))
        {
			DoJump();
        }

		// If the player stops jumping mid jump and player is not yet falling
		// then set the vertical velocity to 0 (he will start to fall from gravity)
		if (Input.GetButtonUp("Jump") && _vy > 0f)
		{
			_vy = 0f;
		}

		// Change the actual velocity on the rigidbody
		_rigidbody.velocity = new Vector2(_vx * moveSpeed, _vy);

		// If moving up then don't collide with platform layer
		// this allows the player to jump up through things on the platform layer
		// NOTE: requires the platforms to be on a layer named "Platform"
		Physics2D.IgnoreLayerCollision(_playerLayer, _platformLayer, (_vy > 0.0f));
	}

    void LateUpdate()
    {
		// get the current scale
		Vector3 localScale = _transform.localScale;

		if (_vx > 0) // moving right so face right
		{
			_isFacingRight = true;
		}
		else if (_vx < 0)// moving left so face left
		{ 
			_isFacingRight = false;
		}

		// check to see if scale x is right for the player
		// if not, multiple by -1 which is an easy way to flip a sprite
		if (((_isFacingRight) && (localScale.x < 0)) || ((!_isFacingRight) && (localScale.x > 0)))
		{
			localScale.x *= -1;
		}

		// update the scale
		_transform.localScale = localScale;
	}

	public void CollectCoin(int amount)
	{/*
		if (GameManager.gm) // add the points through the game manager, if it is available
			GameManager.gm.AddPoints(amount);*/
	}

	public void Respawn(Vector3 spawnloc)
	{
		UnFreezeMotion();
		playerHealth = 1;
		_transform.parent = null;
		_transform.position = spawnloc;
		_animator.SetTrigger("Respawn");
	}

	void DoJump()
	{
		//Reset current vertical motion to 0 prior to jump
		_vy = 0f;
		//Add a force in the up direction
		_rigidbody.AddForce(new Vector2(0, jumpForce));
	}

	// do what needs to be done to freeze the player
	void FreezeMotion()
	{
		playerCanMove = false;
		_rigidbody.velocity = new Vector2(0, 0);
		_rigidbody.isKinematic = true;
	}

	// do what needs to be done to unfreeze the player
	void UnFreezeMotion()
	{
		playerCanMove = true;
		_rigidbody.isKinematic = false;
	}

	// if the player collides with a MovingPlatform, then make it a child of that platform
	// so it will go for a ride on the MovingPlatform
	void OnCollisionEnter2D(Collision2D other)
	{
		if (other.gameObject.tag == "MovingPlatform")
		{
			this.transform.parent = other.transform;
		}
	}

	// if the player exits a collision with a moving platform, then unchild it
	void OnCollisionExit2D(Collision2D other)
	{
		if (other.gameObject.tag == "MovingPlatform")
		{
			this.transform.parent = null;
		}
	}
}
