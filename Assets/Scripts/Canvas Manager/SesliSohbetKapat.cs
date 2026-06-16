using UnityEngine;
using Photon.Voice.Unity;

public class SesliSohbetKapat : MonoBehaviour
{
    public KeyCode toggleKey = KeyCode.V;

    private Recorder recorder;
    private bool voiceEnabled = true;

    void Awake()
    {
        recorder = GetComponent<Recorder>();
        if (recorder == null)
        {
            enabled = false;
        }
    }

    void Start()
    {
        if (!enabled) return;

        recorder.RecordingEnabled = true;     
        recorder.TransmitEnabled = voiceEnabled; 
    }

    void Update()
    {
        if (!enabled) return;

        if (Input.GetKeyDown(toggleKey))
        {
            voiceEnabled = !voiceEnabled;
            recorder.TransmitEnabled = voiceEnabled;

        }
    }
}
