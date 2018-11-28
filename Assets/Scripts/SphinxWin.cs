using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pocketsphinx;

public class SphinxWin : MonoBehaviour {
    private PsDecoder _decoder;
    private Translate translate;

    public string _mic;
    public string _detected;
    public int _micIndex = 0;
    
    private bool _listening;
    private bool _processing;
    
    private List<string> micOption = new List<string>();
    private AudioSource aud;
    public void Awake()
    {
        translate = GetComponent<Translate>();
        CmdLineConfig config = new CmdLineConfig();
        aud = GetComponent<AudioSource>();
        
        if (translate.translation == tlType.EN_MS)
        {
            config.Hmm = Application.dataPath + "/StreamingAssets/SphinxAssets/Models/en-us/en-us";
            config.Dict = Application.dataPath + "/StreamingAssets/SphinxAssets/Models/en-us/cmudict-en-us.dict";
            config.Jsgf = Application.dataPath + "/StreamingAssets/SphinxAssets/Models/demo.gram";
            config.Logfn = Application.dataPath + "/StreamingAssets/SphinxAssets/en-ms.txt";
        }
        else
        {
            config.Hmm = Application.dataPath + "/StreamingAssets/SphinxAssets/Models/malay/db_name";
            config.Dict = Application.dataPath + "/StreamingAssets/SphinxAssets/Models/malay/db_name.dict";
            config.Jsgf = Application.dataPath + "/StreamingAssets/SphinxAssets/Models/demo2.gram";
            config.Logfn = Application.dataPath + "/StreamingAssets/SphinxAssets/ms-en.txt";
        } 
        
        _decoder = new PsDecoder(config);

        _listening = true;
        _processing = false;

        foreach (string device in Microphone.devices)
        {
            micOption.Add(device);
            Debug.Log("Show All Devices: " + device);
        }
    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
    void Update()
    {
        if (translate.isDataLoaded)
        {
            if (_listening) { StartUtterance(); }
            if (_processing) { EndUtterance(); }
        }
    }

    IEnumerator CaptureUtterance() 
    {
        aud.clip = Microphone.Start(_mic, true, 3, 16000);

        int readHead = 0; // position in the clip we're reading from
        int writeHead;    // position in the clip last written too by mic
        int nFloatsToGet; // number of new samples available in the clip
        float[] buffer;   // buffer to copy clip data into

        while (true) {
            writeHead = Microphone.GetPosition(_mic);
            nFloatsToGet = (aud.clip.samples + writeHead - readHead) % aud.clip.samples;
            if (nFloatsToGet < 1024) {
                yield return null;
                continue;
            }

            buffer = new float[nFloatsToGet];

            aud.clip.GetData(buffer, readHead);
            readHead = (readHead + nFloatsToGet) % aud.clip.samples;
            _decoder.ProcessRaw(buffer, 0, nFloatsToGet);

            float score;
            string uttid;
            string hypothesis = _decoder.GetHypothesis(out score, out uttid);
            if (hypothesis != null)
            {
                _listening = false;
                _processing = true;
                yield break;
            }
            yield return null;
        }
    }

    void StartUtterance() 
    {
        _listening = false;

        _decoder.StartUtt(System.Guid.NewGuid().ToString());
        _mic = Microphone.devices[_micIndex];

        Debug.Log("Start");
        StartCoroutine(CaptureUtterance());
    }

    void EndUtterance()
    {
        _processing = false;

        StopAllCoroutines();
        Microphone.End(_mic);
        _decoder.EndUtt();

        float score;
        string uttid;
        string hypothesis = _decoder.GetHypothesis(out score, out uttid);
        _detected = hypothesis;
        translate.search.text = hypothesis;
        translate.SearchWord(hypothesis);
        Debug.Log("Detected: " + hypothesis);
        StartCoroutine(Wait());
    }

    IEnumerator Wait()
    {
        yield return new WaitForSeconds(0.2f);
        _listening = true;
    }
}
