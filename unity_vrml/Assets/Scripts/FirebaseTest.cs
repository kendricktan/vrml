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

        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://vrml-c8ee5.firebaseio.com/");
        storage = FirebaseStorage.DefaultInstance;
        reference = FirebaseDatabase.DefaultInstance.RootReference;
        reference.Child("images_similar_100").ChildChanged += HandleChildChanged;
        reference.Child("neurons").Child("activations").ValueChanged += HandleNeuronChanges;
        reference.Child("is_training").ValueChanged += TrainingChanged;
    }

    bool start = true;
    void TrainingChanged(object sender, ValueChangedEventArgs args) {
        Debug.Log(args.Snapshot.GetRawJsonValue());
        if(args.Snapshot.Value.ToString() == "0" && !start) {
            hideNeurons();
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
                createCube(string2Vector3(n.AsArray[i]["coord"]["x"], n.AsArray[i]["coord"]["y"], n.AsArray[i]["coord"]["z"]) / 5f, n.AsArray[i]["id"]);
            } else {
                updateCube(string2Vector3(n.AsArray[i]["coord"]["x"], n.AsArray[i]["coord"]["y"], n.AsArray[i]["coord"]["z"]) / 5f, n.AsArray[i]["id"], Color.red * n.AsArray[i]["distance"]);
            }
        }
    }

    GameObject createCube(Vector3 pos, string name) {
        GameObject c = GameObject.CreatePrimitive(PrimitiveType.Cube);
        c.transform.SetParent(imagesHolder);
        c.transform.localPosition = pos;
        c.transform.localScale = defaultScale * 3;
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

    void getImageFromStorage(string imageId, string cubeid) {
        StorageReference reference = storage.GetReference("images/" + imageId + ".png");
        reference.GetDownloadUrlAsync().ContinueWith(task => {
            StartCoroutine(getImageViaWWW(task.Result.ToString(), cubeid));
        });
    }

    IEnumerator getImageViaWWW(string url, string cubeid) // change this
    {
        Texture2D tex;
        tex = new Texture2D(4, 4, TextureFormat.DXT1, false);
        WWW www = new WWW(url);
        yield return www;
        www.LoadImageIntoTexture(tex);
        GameObject.Find(cubeid).GetComponent<Renderer>().material.mainTexture = tex;
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