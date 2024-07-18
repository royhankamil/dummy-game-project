using UnityEngine;
using VoltstroStudios.UnityWebBrowser;
using VoltstroStudios.UnityWebBrowser.Core;

public class ExampleLoadingSite : MonoBehaviour
{
    //You need a reference to UWB's WebBrowserClient, which is an object kept on BaseUwbClientManager
    //All of UWB's higher level components (such as WebBrowserUIBasic or WebBrowserUIFull) inherit from BaseUwbClientManager
    //so we can use that as the data type
	[SerializeField] private string weburl = "https://garuda-defi.bigbangx.io/testing-connect";
    [SerializeField] //SerializeField allows us to set this in the editor
    private BaseUwbClientManager clientManager;
        
    private WebBrowserClient webBrowserClient;

    private void Start()
    {
        //You could also use Unity's GetComponent<BaseUwbClientManager>() method if this script exists on the same object.

        //Makes life easier having a local reference to WebBrowserClient
        webBrowserClient = clientManager.browserClient;
    }

    //Call this from were ever, and it will load 'https://voltstro.dev'
    public void LoadMySite()
    {
        webBrowserClient.LoadUrl(weburl);
    }
}