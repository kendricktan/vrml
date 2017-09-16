using Firebase;
using Firebase.Storage;
using Firebase.Database;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Firebase.Unity.Editor;
using SimpleJSON;

public class FirebaseTest : MonoBehaviour
{
    DatabaseReference reference;

    Color defaultColor;
    Transform imagesHolder;
    Vector3 defaultScale;
    FirebaseStorage storage;
    public GameObject player;

    void Start() {
        imagesHolder = GameObject.Find("ImagesHolder").transform;
        defaultColor = Color.blue;
        defaultScale = new Vector3(0.01f, 0.01f, 0.01f);

        StartCoroutine(addImageTriggerScript());

        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://vrml-c8ee5.firebaseio.com/");
        storage = storage = FirebaseStorage.GetInstance("gs://vrml-c8ee5.appspot.com/");
        reference = FirebaseDatabase.DefaultInstance.RootReference;
        reference.Child("images_similar_100").ChildChanged += HandleChildChanged;
        reference.Child("neurons").Child("activations").ValueChanged += HandleNeuronChanges;
        reference.Child("is_training").ValueChanged += TrainingChanged;
    }

    IEnumerator addImageTriggerScript() {
        yield return new WaitForSeconds(3f);
        GameObject h1z1 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        h1z1.GetComponent<MeshRenderer>().enabled = false;
        h1z1.transform.SetParent(GameObject.Find("Hand1").transform.Find("BlankController_Hand1").transform.Find("SteamVR_RenderModel").transform);
        h1z1.transform.localScale = new Vector3(.1f, .1f, .1f);
        h1z1.transform.localPosition = new Vector3(0, 0, 0);
        h1z1.AddComponent<Rigidbody>();
        h1z1.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;
        h1z1.GetComponent<Rigidbody>().useGravity = false;
        h1z1.GetComponent<SphereCollider>().isTrigger = true;
        h1z1.AddComponent<TriggerImage>();
        GameObject tpain = GameObject.CreatePrimitive(PrimitiveType.Plane);
        tpain.transform.SetParent(GameObject.Find("Hand1").transform);
        tpain.transform.localScale = new Vector3(.02f, .02f, .02f);
        tpain.transform.localPosition = Vector3.zero;
        //tpain.transform.localPosition += new Vector3(0, .01f, 0);
        h1z1.GetComponent<TriggerImage>().pane = tpain;
        tpain.SetActive(false);

        GameObject h1z2 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        h1z2.GetComponent<MeshRenderer>().enabled = false;
        h1z2.transform.SetParent(GameObject.Find("Hand2").transform.Find("BlankController_Hand2").transform.Find("SteamVR_RenderModel").transform);
        h1z2.transform.localScale = new Vector3(.1f, .1f, .1f);
        h1z2.transform.localPosition = new Vector3(0, 0, 0);
        h1z2.AddComponent<Rigidbody>();
        h1z2.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;
        h1z2.GetComponent<Rigidbody>().useGravity = false;
        h1z2.GetComponent<SphereCollider>().isTrigger = true;
        h1z2.AddComponent<TriggerImage>();

        GameObject tpain2 = GameObject.CreatePrimitive(PrimitiveType.Plane);
        tpain2.transform.SetParent(GameObject.Find("Hand2").transform);
        tpain2.transform.localScale = new Vector3(.02f, .02f, .02f);
        tpain2.transform.localPosition = Vector3.zero;
        //tpain2.transform.localPosition += new Vector3(0, .01f, 0);
        h1z2.GetComponent<TriggerImage>().pane = tpain2;
        tpain2.SetActive(false);

    }

    bool start = true;
    void TrainingChanged(object sender, ValueChangedEventArgs args) {
        Debug.Log(args.Snapshot.GetRawJsonValue());
        if(args.Snapshot.Value.ToString() == "0" && !start) {
            //hideNeurons();
            deleteNeurons();
            reference.Child("query_coord").SetValueAsync("0,0,0");
        } else if (start) {
            start = false;
        }
    }

    private void writeBool(bool _state) {
        State state = new State(_state);
        string json = JsonUtility.ToJson(state);
        reference.Child("is_training").SetRawJsonValueAsync(json);
    }

    Vector3 string2Vector3(string x, string y, string z) {
        Vector3 toRet = new Vector3(float.Parse(x), float.Parse(y), float.Parse(z));
        return toRet;
    }

    int colCounter = 0;
    int rowCounter = 0;
    Dictionary<string, GameObject> neuronObjects;

    void HandleNeuronChanges(object sender, ValueChangedEventArgs args) {
        var n = JSON.Parse(args.Snapshot.GetRawJsonValue());
        neuronObjects = new Dictionary<string, GameObject>();

        for (int k = 0; k < n.Count -1; k++) {
            GameObject g = GameObject.Find(k.ToString());
            if(!g) {
                createCube(new Vector3(0 + (float)(.03 * rowCounter), 0, 0 + (float)(.03 * colCounter)), k.ToString());
                colCounter++;
                colCounter = colCounter % 25;
                if (colCounter == 0)
                    rowCounter++;
            } else {
                changeNColor(g, float.Parse(n[k].ToString()));
            }
        }
    }

    void hideNeurons() {
        imagesHolder.gameObject.SetActive(false);
    }

    void deleteNeurons() {
        foreach (Transform child in imagesHolder.transform) {
            GameObject.Destroy(child.gameObject);
        }
    }

    void changeNColor(GameObject g, float val) {
        g.GetComponent<Renderer>().material.color = Color.blue;
       g.GetComponent<Renderer>().material.color += new Color(0, 0, val * 10);
    }

    void HandleChildChanged(object sender, ChildChangedEventArgs args) {
        deleteNeurons();
        var n = JSON.Parse(args.Snapshot.GetRawJsonValue());
        for(int i = 0; i < n.Count; i++) {
            Debug.Log(n.AsArray[i]["id"]);
            Debug.Log(n.AsArray[i]["coord"]);
            Debug.Log(n.AsArray[i]["distance"]);

            if (!GameObject.Find(n.AsArray[i]["id"])) {
                createCube(string2Vector3(n.AsArray[i]["coord"]["x"], n.AsArray[i]["coord"]["y"], n.AsArray[i]["coord"]["z"]) / 2f, n.AsArray[i]["id"], 3f);
            } else {
                updateCube(string2Vector3(n.AsArray[i]["coord"]["x"], n.AsArray[i]["coord"]["y"], n.AsArray[i]["coord"]["z"]) / 2f, n.AsArray[i]["id"], Color.red * n.AsArray[i]["distance"]);
            }
        }
    }

    GameObject createCube(Vector3 pos, string name, float scaleMod = 0) {
        GameObject c = GameObject.CreatePrimitive(PrimitiveType.Cube);
        c.transform.SetParent(imagesHolder);
        c.transform.localPosition = pos;
        c.transform.localScale = defaultScale;
        c.GetComponent<Renderer>().material.color = defaultColor;
        c.name = name;
        c.tag = "InteractionCube";

        // rigidbody, no gravity
        Rigidbody c_rigidbody = c.AddComponent<Rigidbody>();
        c_rigidbody.useGravity = false;
        c_rigidbody.freezeRotation = true;
        c_rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;
        return c;
    }
    void updateCube(Vector3 pos, string name, Color color) {
        GameObject c = GameObject.Find(name);
        c.transform.localPosition = pos;
        c.GetComponent<Renderer>().material.color = color;
    }

    public void getImageFromStorage(string imageId, GameObject pane) {
        StorageReference reference = storage.GetReference(imageId);
        reference.GetDownloadUrlAsync().ContinueWith(task => {
            StartCoroutine(getImageViaWWW(task.Result.ToString(), pane));
        });
    }

    IEnumerator getImageViaWWW(string url, GameObject pane) // change this
    {
        Texture2D tex;
        tex = new Texture2D(4, 4, TextureFormat.DXT1, false);
        WWW www = new WWW(url);
        yield return www;
        www.LoadImageIntoTexture(tex);
        pane.GetComponent<Renderer>().material.mainTexture = tex;
    }

    void MsgChanged(object sender, ValueChangedEventArgs args) {
        Debug.Log(args.Snapshot.Value.ToString());
    }

    void TstMsgChanged(object sender, ChildChangedEventArgs args) {
        Debug.Log(args.Snapshot.Value.ToString());
    }
}

public class State
{
    public bool state;
    public State()
    {
    }

    public State(bool state)
    {
        this.state = state;
    }
}
[Serializable]
public class dataPoint {
    public string id;
    public Coords coords;
    public float distance;
}
[Serializable]
public class Coords {
    public string x;
    public string y;
    public string z;
}

public static class JsonHelper {
    public static T[] FromJson<T>(string json) {
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
        return wrapper.Items;
    }

    public static string ToJson<T>(T[] array) {
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.Items = array;
        return JsonUtility.ToJson(wrapper);
    }

    public static string ToJson<T>(T[] array, bool prettyPrint) {
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.Items = array;
        return JsonUtility.ToJson(wrapper, prettyPrint);
    }

    [Serializable]
    private class Wrapper<T> {
        public T[] Items;
    }
}