using System;
using System.Collections;
using System.Collections.Generic;
using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using UnityEngine;
using UnityEngine.Serialization;

public class FirebaseDBManager : MonoBehaviour
{
    private const int MaxScores = 5;
    private string logText = "";
    private string email = "";
    private int score = 100;
    const int kMaxLogSize = 16382;
    DependencyStatus dependencyStatus = DependencyStatus.UnavailableOther;
    private FirebaseDatabase _database;
    public static FirebaseDBManager instance;

    private void Awake()
    {
      if (instance == null)
      {
        instance = this;
        DontDestroyOnLoad(this);
      }
      else
      {
        DestroyImmediate(gameObject);
      }
    }
    protected virtual void Start() {
      FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task => {
        dependencyStatus = task.Result;
        if (dependencyStatus == DependencyStatus.Available) {
          InitializeFirebase();
        } else {
          Debug.LogError(
            "Could not resolve all Firebase dependencies: " + dependencyStatus);
        }
      });
    }
    // Initialize the Firebase database:
    protected virtual void InitializeFirebase() {
      FirebaseApp app = FirebaseApp.DefaultInstance;
      _database=FirebaseDatabase.DefaultInstance;
      Debug.Log("Done");
    }

    public void SetCoins(int coin)
    {
      _database.GetReference(SystemInfo.deviceUniqueIdentifier).SetRawJsonValueAsync(coin.ToString());
    }
}
