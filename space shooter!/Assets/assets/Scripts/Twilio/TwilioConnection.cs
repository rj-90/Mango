﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CI.HttpClient;
using Fungus;


[Serializable]
public class TwilioConversationData
{
    public string user_name = "";
    public bool crash = false;
    public bool easier = false;
}


public class TwilioConnection : MonoBehaviour
{
    public string Name ="";
    Flowchart fl;
    public TextMesh myText;
    string ngrokPath = "http://962fc00d.ngrok.io/";
    bool sendGet = false;
    
    TwilioConversationData convData = new TwilioConversationData();

    // Start is called before the first frame update\
    HttpClient client;
    void Start()
    {
        DontDestroyOnLoad(transform);
        client = new HttpClient();
        StartCoroutine(RepetitiveCountdown());
    }

    float currCountdownValue;
    public IEnumerator RepetitiveCountdown(float countdownValue = 8f)
    {
        currCountdownValue = countdownValue;
        while(true){
            while (currCountdownValue > 0)
            {
                yield return new WaitForSeconds(.5f);
                currCountdownValue -= .5f;
            }
            currCountdownValue = countdownValue;
            StartCoroutine(GetData());
            yield return null;
        }
    }

    public IEnumerator GetData()
    {
        bool done = false;
        string responseData;
        client.Get(new System.Uri(ngrokPath + "unity_game/data"), HttpCompletionOption.AllResponseContent, (r) =>
        {
            if(r.HasContent){
                responseData = r.ReadAsString();
                print(responseData);
                TwilioConversationData newConvData = JsonUtility.FromJson<TwilioConversationData>(responseData);
                print(newConvData.user_name);
                CheckDifference(newConvData);
            }
            done = true;
        });
        while(!done){
            yield return null;
        }
        yield return null;
    }

    public void CheckDifference(TwilioConversationData newData){
        if(newData.crash != convData.crash){
            crashChanged(newData.crash);
        }
        print(newData.user_name);
        print(convData.user_name);
        if(newData.user_name != convData.user_name){
            nameChanged(newData.user_name);
        }
        convData = newData;
        return;
    }

    void nameChanged(String user_name){
        print("Name changed!");
        myText.text = "HELLO " + user_name;
        Name = user_name;
        Fungus.Flowchart.BroadcastFungusMessage("R1");
        return;
    }

    //this is called after name changed sends the fungus message. 
    //not sure what is the varible being changed so for now setting it to public varible Name;
    public string returnName()
    {
        return Name;
    }
    void crashChanged(bool crash){
        print("crash changed!");
        if(crash){
           UnityEngine.Diagnostics.Utils.ForceCrash(UnityEngine.Diagnostics.ForcedCrashCategory.AccessViolation);
        }
        return;
    }
    // Update is called once per frame
    void Update()
    {

    }
}
