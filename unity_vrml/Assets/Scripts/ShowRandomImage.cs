using Firebase.Storage;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowRandomImage : MonoBehaviour {

	// Use this for initialization
	void Start () {
        getImageFromStorage("image_" + int.Parse(Random.Range(0, 405f).ToString()).ToString() + ".png", this.gameObject);
	}

    public void getImageFromStorage(string imageId, GameObject pane) {
        FirebaseStorage storage = storage = FirebaseStorage.GetInstance("gs://vrml-c8ee5.appspot.com/");
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
}
