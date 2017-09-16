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

public class FirebaseTest : MonoBehaviour
{
    DatabaseReference reference;

    Color defaultColor;
    Transform imagesHolder;
    Vector3 defaultScale;
    FirebaseStorage storage;
    public GameObject player;

    void Start()
    {
        // change color
        defaultColor = Color.blue;
        defaultScale = new Vector3(0.05f, 0.05f, 0.05f);

        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://vrml-c8ee5.firebaseio.com/");
        storage = FirebaseStorage.DefaultInstance;
        reference = FirebaseDatabase.DefaultInstance.RootReference;
        reference.Child("images").ChildAdded += HandleChildAdded;
        reference.Child("images").ChildChanged += HandleChildChanged;

        // possible start position
        Vector3 pos = player.transform.position - new Vector3(0, 0.5f, -1);
        createCube(pos, "test");
    }

    private void writeBool(bool _state)
    {
        State state = new State(_state);
        string json = JsonUtility.ToJson(state);
        reference.Child("ml-state").SetRawJsonValueAsync(json);
    }

    void HandleChildAdded(object sender, ChildChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }
        // Do something with the data in args.Snapshot
    }

    void HandleChildChanged(object sender, ChildChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }
        // Do something with the data in args.Snapshot
    }

    void createCube(Vector3 pos, string name)
    {
        GameObject c = GameObject.CreatePrimitive(PrimitiveType.Cube);
        c.transform.SetParent(imagesHolder);
        c.transform.localPosition = pos;
        c.transform.localScale = defaultScale;
        c.GetComponent<Renderer>().material.color = defaultColor;
        c.name = name;

        // rigidbody, no gravidy, is trigger
        Rigidbody c_rigidbody = c.AddComponent<Rigidbody>();
        c_rigidbody.useGravity = false;
        Collider c_collider = c.AddComponent<Collider>();
        c_collider.isTrigger = true;
        TriggerImage Sc_script = c.AddComponent<TriggerImage>();
    }

    void updateCube(Vector3 pos, string name, Color color)
    {
        GameObject c = GameObject.Find(name);
        c.transform.localPosition = pos;
        c.GetComponent<Renderer>().material.color = color;
    }

    void getImageFromStorage(string imageId, string cubeid)
    {
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