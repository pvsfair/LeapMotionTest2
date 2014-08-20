using UnityEngine;
using System.Collections;
using Leap;

public class MovingScriptTest : MonoBehaviour {
	//Leap Variables
	Controller leapController = new Controller();
	Frame currentFrame = Frame.Invalid;
	bool click = false;
	int palmLimit = 35;

	//Player Variables
	CharacterController chController;
	Vector3 moveDirection;
	float jumpSpeed = 1f;
	float Gravity = 3f;
	float moveSpeed = 5f;
	float lastYPosition = 0;

	string toGui;



	// Use this for initialization
	void Start () {

		chController = transform.GetComponent<CharacterController>();
	
	}
	
	// Update is called once per frame
	void Update () {

		currentFrame = leapController.Frame();

		Hand primeHand = frontMostHand();
		if(chController.isGrounded){
			if(primeHand.PalmPosition.x > palmLimit){
				moveDirection.x = moveSpeed * Time.deltaTime;	
			} else if(primeHand.PalmPosition.x < -palmLimit){
				moveDirection.x = -moveSpeed * Time.deltaTime;
			} else moveDirection.x = 0;
			if(primeHand.PalmPosition.y - lastYPosition > 12) moveDirection.y = jumpSpeed;
		}

		toGui = "X Hand Position: " + primeHand.PalmPosition;
		moveDirection.y -= Gravity * Time.deltaTime;

		chController.Move(moveDirection);

		lastYPosition = primeHand.PalmPosition.y;
	}

	private Hand frontMostHand(){
		Hand bestHand = Hand.Invalid;
		float minZ = float.MaxValue;

		foreach(Hand hand in currentFrame.Hands){
			if(hand.PalmPosition.z < minZ){
				minZ = hand.PalmPosition.z;
				bestHand = hand;
			}

		}

		return bestHand;
	}

	void OnGUI(){
		GUI.Label(new Rect(10,10,1000,100), toGui);
	}
}
