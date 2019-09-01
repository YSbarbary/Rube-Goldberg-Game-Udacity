using UnityEngine;
using UnityEngine.EventSystems;

using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Valve.VR;
using Valve.VR.InteractionSystem;

[AddComponentMenu("Radial Menu Framework/RMF Core Script")]
public class RMF_RadialMenu : MonoBehaviour {

    private Selectable selectedObject;
    public SteamVR_Action_Vector2 actionNavigateMenu;
    public SteamVR_Action_Boolean actionThumbstickIsPressed;
    public SteamVR_Action_Boolean actionConfirm;
    public List<GameObject> objectPrefabList;
    private SteamVR_Behaviour_Pose m_PoseBehaviour;
    public float throwForce = 1.5f;
    private Player player = null;
    private bool firstTimeSwitch = true;
    private bool firstTimeGrabItem = true;

    //private MenuManager menu; //I make it as Singlton class so we can access it directly without creating a reference in this class





    [HideInInspector]
    public RectTransform rt;
    //public RectTransform baseCircleRT;
    //public Image selectionFollowerImage;


    /* 
[Tooltip("Adjusts the radial menu for use with a gamepad or joystick. You might need to edit this script if you're not using the default horizontal and vertical input axes.")]
public bool useGamepad = false;

[Tooltip("With lazy selection, you only have to point your mouse (or joystick) in the direction of an element to select it, rather than be moused over the element entirely.")]
public bool useLazySelection = true;


[Tooltip("If set to true, a pointer with a graphic of your choosing will aim in the direction of your mouse. You will need to specify the container for the selection follower.")]
public bool useSelectionFollower = true;

[Tooltip("If using the selection follower, this must point to the rect transform of the selection follower's container.")]
public RectTransform selectionFollowerContainer;
*/
    [Tooltip("This is the text object that will display the labels of the radial elements when they are being hovered over. If you don't want a label, leave this blank.")]
    public Text textLabel;

    [Tooltip("This is the list of radial menu elements. This is order-dependent. The first element in the list will be the first element created, and so on.")]
    public List<RMF_RadialMenuElement> elements = new List<RMF_RadialMenuElement>();


    [Tooltip("Controls the total angle offset for all elements. For example, if set to 45, all elements will be shifted +45 degrees. Good values are generally 45, 90, or 180")]
    public float globalOffset = 0f;

    /*   
    [HideInInspector]
    public float currentAngle = 0f; //Our current angle from the center of the radial menu.


    [HideInInspector]
    public int index = 0; //The current index of the element we're pointing at.

    private int elementCount;

    private float angleOffset; //The base offset. For example, if there are 4 elements, then our offset is 360/4 = 90

    private int previousActiveIndex = 0; //Used to determine which buttons to unhighlight in lazy selection.

    private PointerEventData pointer;

    */

    void Awake()
    {
        /*
        pointer = new PointerEventData(EventSystem.current);

        rt = GetComponent<RectTransform>();

        if (rt == null)
            Debug.LogError("Radial Menu: Rect Transform for radial menu " + gameObject.name + " could not be found. Please ensure this is an object parented to a canvas.");

        if (useSelectionFollower && selectionFollowerContainer == null)
            Debug.LogError("Radial Menu: Selection follower container is unassigned on " + gameObject.name + ", which has the selection follower enabled.");

        elementCount = elements.Count;

        angleOffset = (360f / (float)elementCount);

        //Loop through and set up the elements.
        for (int i = 0; i < elementCount; i++) {
            if (elements[i] == null) {
                Debug.LogError("Radial Menu: element " + i.ToString() + " in the radial menu " + gameObject.name + " is null!");
                continue;
            }
            elements[i].parentRM = this;

            elements[i].setAllAngles((angleOffset * i) + globalOffset, angleOffset);

            elements[i].assignedIndex = i;

        }
        */
    }



    void Start()
    {

        selectedObject = GetComponentInChildren<Selectable>();
        InputModule.instance.HoverBegin(selectedObject.gameObject);

        // menu = GetComponent<MenuManager>();

        player = Player.instance;

        if (player == null)
        {
            Debug.LogError("<b>[SteamVR Interaction]</b> Teleport: No Player instance found in map.");
            Destroy(this.gameObject);
            return;
        }

        /* 

        if (useGamepad) {
            EventSystem.current.SetSelectedGameObject(gameObject, null); //We'll make this the active object when we start it. Comment this line to set it manually from another script.
            if (useSelectionFollower && selectionFollowerContainer != null)
                selectionFollowerContainer.rotation = Quaternion.Euler(0, 0, -globalOffset); //Point the selection follower at the first element.
        }
        */
    }

    Vector2 axis;
    Selectable nextSelectableObject;
    Hand hand;
    Vector3 objectPosition;
    Quaternion prefabRotation;
    public GameObject followHeadReference;
    Vector3 followHeadPosition;

    // Update is called once per frame
    void Update()
    {


        axis = actionNavigateMenu.GetAxis(SteamVR_Input_Sources.Any);
        nextSelectableObject = null;

        textLabel.text = (selectedObject.gameObject.GetComponentInParent<RMF_RadialMenuElement>()).label;

        // Debug.Log("The selectable Object is : " + selectedObject.gameObject);
        // Debug.Log("The textLabel.text is : " + textLabel.text);


        if (axis.x < -0.1 )
        {
            if (actionThumbstickIsPressed.GetStateDown(SteamVR_Input_Sources.Any))
            {
                nextSelectableObject = selectedObject.FindSelectableOnLeft();
                if (firstTimeSwitch)
                {
                    firstTimeSwitch = false;
                    MenuManager.instace.switchHint = false;
                    MenuManager.instace.DisableSwitchGrabHints();
                }
            }
        }
        else if (axis.x > 0.1 )
        {
            if (actionThumbstickIsPressed.GetStateDown(SteamVR_Input_Sources.Any))
            {
                nextSelectableObject = selectedObject.FindSelectableOnRight();
                if (firstTimeSwitch)
                {
                    firstTimeSwitch = false;
                    MenuManager.instace.switchHint = false;
                    MenuManager.instace.DisableSwitchGrabHints();
                }
            }
        }

        if (nextSelectableObject)
        {
            InputModule.instance.HoverEnd(selectedObject.gameObject);
            InputModule.instance.HoverBegin(nextSelectableObject.gameObject);
            selectedObject = nextSelectableObject;
        }


        if (actionConfirm.GetStateDown(SteamVR_Input_Sources.Any) && MenuManager.instace.menuObject.activeInHierarchy)
        {
            InputModule.instance.Submit(selectedObject.gameObject);
            int indexSelectedElement = this.elements.IndexOf(selectedObject.gameObject.GetComponentInParent<RMF_RadialMenuElement>());
            //Debug.Log("The action Confirm on index : " + indexSelectedElement);


            hand = player.leftHand;
           
            if (player.leftHand.transform.position.y == 0)
            {
                hand = player.rightHand;
            }

            followHeadPosition = followHeadReference.transform.position;
            objectPosition = new Vector3(followHeadPosition.x, 0.40f, followHeadPosition.z + 0.5f);
            prefabRotation = objectPrefabList[indexSelectedElement].transform.rotation;//selectedObject.gameObject.GetComponentInChildren<Image>().transform.rotation; //new Quaternion(0, 0, 0 , 0);

            if (indexSelectedElement == 1) //Trampoline 
            { objectPosition.y = 0.25f; }
          /*  else if (indexSelectedElement == 0) //Wood
            { leftHandPosition.x = -0.5f; }*/

            Instantiate(objectPrefabList[indexSelectedElement], objectPosition, prefabRotation);
            // objectPrefabList[indexSelectedElement].GetComponent<Rigidbody>().useGravity = false;
            MenuManager.instace.menuObject.SetActive(!MenuManager.instace.menuObject.activeSelf); //Now the trigger will be for interact only

            if (firstTimeGrabItem)
            {
                firstTimeGrabItem = false;
                MenuManager.instace.grabItemsHint = false;
                MenuManager.instace.DisableSwitchGrabHints();
            }
        }
    }

 


    
    /*
      void Update()
    {
    //If your gamepad uses different horizontal and vertical joystick inputs, change them here!
    //==============================================================================================
    bool joystickMoved = Input.GetAxis("Horizontal") != 0.0 || Input.GetAxis("Vertical") != 0.0;
    //==============================================================================================


    float rawAngle;

    if (!useGamepad)
        rawAngle = Mathf.Atan2(Input.mousePosition.y - rt.position.y, Input.mousePosition.x - rt.position.x) * Mathf.Rad2Deg;
    else
        rawAngle = Mathf.Atan2(Input.GetAxis("Vertical"), Input.GetAxis("Horizontal")) * Mathf.Rad2Deg;

    //If no gamepad, update the angle always. Otherwise, only update it if we've moved the joystick.
    if (!useGamepad)
        currentAngle = normalizeAngle(-rawAngle + 90 - globalOffset + (angleOffset / 2f));
    else if (joystickMoved)
        currentAngle = normalizeAngle(-rawAngle + 90 - globalOffset + (angleOffset / 2f));

    //Handles lazy selection. Checks the current angle, matches it to the index of an element, and then highlights that element.
    if (angleOffset != 0 && useLazySelection) {

        //Current element index we're pointing at.
        index = (int)(currentAngle / angleOffset);

        if (elements[index] != null) {

            //Select it.
            selectButton(index);

            //If we click or press a "submit" button (Button on joystick, enter, or spacebar), then we'll execut the OnClick() function for the button.
            if (Input.GetMouseButtonDown(0) || Input.GetButtonDown("Submit")) {

                ExecuteEvents.Execute(elements[index].button.gameObject, pointer, ExecuteEvents.submitHandler);


            }
        }

    }

    //Updates the selection follower if we're using one.
    if (useSelectionFollower && selectionFollowerContainer != null) {
        if (!useGamepad || joystickMoved)
            selectionFollowerContainer.rotation = Quaternion.Euler(0, 0, rawAngle + 270);


    }
}
    */

    /*
    //Selects the button with the specified index.
    private void selectButton(int i) {

          if (elements[i].active == false) {

            elements[i].highlightThisElement(pointer); //Select this one

            if (previousActiveIndex != i) 
                elements[previousActiveIndex].unHighlightThisElement(pointer); //Deselect the last one.
            

        }

        previousActiveIndex = i;

    }

    //Keeps angles between 0 and 360.
    private float normalizeAngle(float angle) {

        angle = angle % 360f;

        if (angle < 0)
            angle += 360;

        return angle;

    }
    */

}
