using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Windows.Speech;

public class DetectorDeVoz : MonoBehaviour
{

    KeywordRecognizer keywordRecognizer;
    Dictionary<string, System.Action> keywords = new Dictionary<string, System.Action>();


    void Start()
    {
        keywords.Add("left", PintarAmarillo);
        keywords.Add("center", PintarRojo);
        keywords.Add("over", Tangamandapio);

        keywordRecognizer = new KeywordRecognizer(keywords.Keys.ToArray());
        keywordRecognizer.OnPhraseRecognized += KeywordRecognizer_OnPhraseRecognized;
        keywordRecognizer.Start();

    }



    private void KeywordRecognizer_OnPhraseRecognized(PhraseRecognizedEventArgs args)
    {
        System.Action keywordAction;
        // if the keyword recognized is in our dictionary, call that Action.
        if (keywords.TryGetValue(args.text, out keywordAction))
        {
            keywordAction.Invoke();
        }
    }

    private void PintarAmarillo()
    {
        Debug.Log("Dije la palabra amaillo");
    }

    private void PintarRojo() {
        Debug.Log("Dije la palabra Rojo");
    }

    private void Tangamandapio()
    {
        Debug.Log("Dije la palabra Tangamandapio");

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
