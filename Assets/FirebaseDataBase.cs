using System;
using System.Collections;
using System.Collections.Generic;
using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using UnityEngine;
using UnityEngine.Serialization;

public class FirebaseDataBase : MonoBehaviour
{
    private const int MaxScores = 5;
    private string logText = "";
    private string email = "";
    private int score = 100;
    const int kMaxLogSize = 16382;
    DependencyStatus dependencyStatus = DependencyStatus.UnavailableOther;
    public static FirebaseDataBase instance;

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
      //StartListener();
    }

    protected void StartListener() {
      // FirebaseDatabase.DefaultInstance
      //   .GetReference("User").OrderByChild("score")
      //   .ValueChanged += (object sender2, ValueChangedEventArgs e2) => {
      //     if (e2.DatabaseError != null) {
      //       Debug.LogError(e2.DatabaseError.Message);
      //       return;
      //     }
      //     Debug.Log("Received values for Leaders.");
      //     if (e2.Snapshot != null && e2.Snapshot.ChildrenCount > 0) {
      //       foreach (var childSnapshot in e2.Snapshot.Children) {
      //         if (childSnapshot.Child("score") == null
      //             || childSnapshot.Child("score").Value == null) {
      //           Debug.LogError("Bad data in sample.");
      //           break;
      //         } else
      //         {
      //           childSnapshot.Child("email").Value.ToString() + " - " +
      //                     childSnapshot.Child("score").Value.ToString());
      //         }
      //       }
      //     }
      //   };
    }
    
    // Output text to the debug log text field, as well as the console.
    public void DebugLog(string s) {
      Debug.Log(s);
      logText += s + "\n";

      while (logText.Length > kMaxLogSize) {
        int index = logText.IndexOf("\n");
        logText = logText.Substring(index + 1);
      }

    }

    // A realtime database transaction receives MutableData which can be modified
    // and returns a TransactionResult which is either TransactionResult.Success(data) with
    // modified data or TransactionResult.Abort() which stops the transaction with no changes.
    TransactionResult AddScoreTransaction(MutableData mutableData) {
      List<object> leaders = mutableData.Value as List<object>;

      if (leaders == null) {
        leaders = new List<object>();
      } else if (mutableData.ChildrenCount >= MaxScores) {
        // If the current list of scores is greater or equal to our maximum allowed number,
        // we see if the new score should be added and remove the lowest existing score.
        long minScore = long.MaxValue;
        object minVal = null;
        foreach (var child in leaders) {
          if (!(child is Dictionary<string, object>))
            continue;
          long childScore = (long)((Dictionary<string, object>)child)["score"];
          if (childScore < minScore) {
            minScore = childScore;
            minVal = child;
          }
        }
        // If the new score is lower than the current minimum, we abort.
        if (minScore > score) {
          return TransactionResult.Abort();
        }
        // Otherwise, we remove the current lowest to be replaced with the new score.
        leaders.Remove(minVal);
      }

      // Now we add the new score as a new entry that contains the email address and score.
      Dictionary<string, object> newScoreMap = new Dictionary<string, object>();
      newScoreMap["score"] = score;
      newScoreMap["email"] = email;
      leaders.Add(newScoreMap);

      // You must set the Value to indicate data at that location has changed.
      mutableData.Value = leaders;
      return TransactionResult.Success(mutableData);
    }

    public void AddScore() {
      if (score == 0 || string.IsNullOrEmpty(email)) {
        DebugLog("invalid score or email.");
        return;
      }
      DebugLog(String.Format("Attempting to add score {0} {1}",
        email, score.ToString()));

      DatabaseReference reference = FirebaseDatabase.DefaultInstance.GetReference("Leaders");

      DebugLog("Running Transaction...");
      // Use a transaction to ensure that we do not encounter issues with
      // simultaneous updates that otherwise might create more than MaxScores top scores.
      reference.RunTransaction(AddScoreTransaction)
        .ContinueWithOnMainThread(task => {
          if (task.Exception != null) {
            DebugLog(task.Exception.ToString());
          } else if (task.IsCompleted) {
            DebugLog("Transaction complete.");
          }
        });
    }
  
}
