using UnityEngine;
using UnityEngine.UI;
using Valve.VR;

public class ColorWheel : MonoBehaviour {
    private Valve.VR.EVRButtonId touchPad = Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad;
    private Valve.VR.EVRButtonId menuButton = Valve.VR.EVRButtonId.k_EButton_ApplicationMenu;

    private SteamVR_Controller.Device controller;//{ get { return SteamVR_Controller.Input((int)trackedObj.index); } }
    //private SteamVR_TrackedObject trackedObj;

	private float hue, saturation, value = 1f;
    Vector2 touchpad;
    private float sensitivityX = 1.5F;
    private Vector3 playerPos;

    public bool changeColors = false;
    public GameObject colorStuff;

    FirebaseTest ft;

    void Start() {
        ft = GameObject.Find("Scripts").GetComponent<FirebaseTest>();
    }

    void Update() {
        if(changeColors) {
            if (SteamVR_Controller.Input((int)SteamVR_Controller.GetDeviceIndex(SteamVR_Controller.DeviceRelation.Rightmost)).GetTouch(touchPad)) {
                float a = Vector2.Angle(new Vector2(0, 1), SteamVR_Controller.Input((int)SteamVR_Controller.GetDeviceIndex(SteamVR_Controller.DeviceRelation.Rightmost)).GetAxis(EVRButtonId.k_EButton_SteamVR_Touchpad));
                if (SteamVR_Controller.Input((int)SteamVR_Controller.GetDeviceIndex(SteamVR_Controller.DeviceRelation.Rightmost)).GetAxis(EVRButtonId.k_EButton_SteamVR_Touchpad)[0] < 0) {
                    a = 180 + Vector2.Angle(new Vector2(0, -1), SteamVR_Controller.Input((int)SteamVR_Controller.GetDeviceIndex(SteamVR_Controller.DeviceRelation.Rightmost)).GetAxis(EVRButtonId.k_EButton_SteamVR_Touchpad));
                }
                ChangedHueSaturation(SteamVR_Controller.Input((int)SteamVR_Controller.GetDeviceIndex(SteamVR_Controller.DeviceRelation.Rightmost)).GetAxis(EVRButtonId.k_EButton_SteamVR_Touchpad), a);
            }
        }

        if(SteamVR_Controller.Input((int)SteamVR_Controller.GetDeviceIndex(SteamVR_Controller.DeviceRelation.Rightmost)).GetPressDown(menuButton)) {
            changeColors = !changeColors;
            colorStuff.SetActive(changeColors);
        }

        if (ft.changeWhichCubesToColor) {
            ft.changeWhichCubesToColor = false;
            holder = GameObject.Find("SecondImagesHolder").transform;
        }
               
    }
   
	private void ChangedHueSaturation(Vector2 touchpadAxis, float touchpadAngle) {
		float normalAngle = touchpadAngle - 90;
		if (normalAngle < 0)
			normalAngle = 360 + normalAngle;
		
		Debug.Log ("ChangeColor: Trackpad axis at: " + touchpadAxis + " (" + normalAngle + " degrees)");

		float rads = normalAngle * Mathf.PI / 180;
		float maxX = Mathf.Cos (rads);
		float maxY = Mathf.Sin (rads);

		float currX = touchpadAxis.x;
		float currY = touchpadAxis.y;

		float percentX = Mathf.Abs (currX / maxX);
		float percentY = Mathf.Abs (currY / maxY);

		this.hue = normalAngle / 360.0f;
		this.saturation = (percentX + percentY) / 2;

		UpdateColor ();
	}

    public Transform holder;
	private void UpdateColor() {
		Color color = Color.HSVToRGB(this.hue, this.saturation, this.value);
        foreach(Transform c in holder) {
            c.GetComponent<Renderer>().material.color = color;
        }
        GameObject.Find("Scripts").GetComponent<FirebaseTest>().colorToUse = color;
    }




}

    
