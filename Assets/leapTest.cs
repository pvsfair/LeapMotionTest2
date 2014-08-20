using UnityEngine;
using System.Collections;
using Leap;

public class leapTest : MonoBehaviour {
	public GameObject[] fingers;

	private string toGui;

	Controller controller = new Controller();
	Frame currentFrame = Frame.Invalid;


	// Use this for initialization
	void Start () {
		controller.EnableGesture (Gesture.GestureType.TYPECIRCLE);
		controller.EnableGesture (Gesture.GestureType.TYPEKEYTAP);
		controller.EnableGesture (Gesture.GestureType.TYPESCREENTAP);
		controller.EnableGesture (Gesture.GestureType.TYPESWIPE);
	}
	
	// Update is called once per frame
	void Update () {
		currentFrame = controller.Frame();

		Hand primeHand = frontMostHand();

		if(primeHand.IsValid){
			gameObject.transform.position = primeHand.PalmPosition.ToUnityTranslated();

			Vector3[] fingerTipPositions = getFingersPositions(primeHand);

			for(int i = 0; i < fingers.GetLength(0); i++){
				if(i < fingerTipPositions.GetLength(0)){
					fingers[i].transform.position = fingerTipPositions[i];
					if(!fingers[i].renderer.enabled) fingers[i].renderer.enabled = true;
				} else {
					fingers[i].renderer.enabled = false;
				}
			}
		}
		gestures();
	}
	
	void OnGUI(){
		GUI.Label(new Rect(10,10,1000,100), toGui);
	}

	private void gestures(){
		GestureList gestures = currentFrame.Gestures ();
		for (int i = 0; i < gestures.Count; i++) {
			Gesture gesture = gestures [i];
			
			switch (gesture.Type) {
			case Gesture.GestureType.TYPECIRCLE:
				CircleGesture circle = new CircleGesture (gesture);
				
				// Calculate clock direction using the angle between circle normal and pointable
				string clockwiseness;
				if (circle.Pointable.Direction.AngleTo (circle.Normal) <= System.Math.PI / 4) {
					//Clockwise if angle is less than 90 degrees
					clockwiseness = "clockwise";
				} else {
					clockwiseness = "counterclockwise";
				}
				
				float sweptAngle = 0;
				
				// Calculate angle swept since last frame
				if (circle.State != Gesture.GestureState.STATESTART) {
					CircleGesture previousUpdate = new CircleGesture (controller.Frame (1).Gesture (circle.Id));
					sweptAngle = (circle.Progress - previousUpdate.Progress) * 360;
				}
				
				toGui = "Circle id: " + circle.Id
				               + ", " + circle.State
				               + ", progress: " + circle.Progress
				               + ", radius: " + circle.Radius
				               + ", angle: " + sweptAngle
				               + ", " + clockwiseness;
				break;
			case Gesture.GestureType.TYPESWIPE:
				SwipeGesture swipe = new SwipeGesture (gesture);
				toGui = "Swipe id: " + swipe.Id
				               + ", " + swipe.State
				               + ", position: " + swipe.Position
				               + ", direction: " + swipe.Direction
				               + ", speed: " + swipe.Speed;
				break;
			case Gesture.GestureType.TYPEKEYTAP:
				KeyTapGesture keytap = new KeyTapGesture (gesture);
				toGui = "Tap id: " + keytap.Id
				               + ", " + keytap.State
				               + ", position: " + keytap.Position
				               + ", direction: " + keytap.Direction;
				break;
			case Gesture.GestureType.TYPESCREENTAP:
				ScreenTapGesture screentap = new ScreenTapGesture (gesture);
				toGui = "Tap id: " + screentap.Id
				               + ", " + screentap.State
				               + ", position: " + screentap.Position
				               + ", direction: " + screentap.Direction;
				break;
			default:
				toGui = "Unknown gesture type.";
				break;
			}
		}
	}

	private Hand frontMostHand(){
		float minZ = float.MaxValue;
		Hand forwardHand = Hand.Invalid;

		foreach(Hand hand in currentFrame.Hands){
			if(hand.PalmPosition.z < minZ){
				minZ = hand.PalmPosition.z;
				forwardHand = hand;
			}
		}
		return forwardHand;
	}

	private Vector3[] getFingersPositions(Hand hand){
		Vector3 [] retArr = new Vector3[hand.Fingers.Count];

		for(int i = 0; i < hand.Fingers.Count; i++) retArr[i] = hand.Fingers[i].TipPosition.ToUnityTranslated();

		return retArr;
	}
}
