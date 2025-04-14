using System;
using UnityEngine;
using Firebase.Firestore;
using TMPro;
using Firebase.Extensions;
using System.Diagnostics;
using System.Collections;
using System.IO;

public class FirestoreService : MonoBehaviour
{
    private FirebaseFirestore _db;
    private string _userId;
    
    public static FirestoreService Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {

        _db = FirebaseFirestore.DefaultInstance;
        
        _userId = GetPlayerId();
        LogAction("Player downlaoded a profile picture.");
    }

    private string GetPlayerId()
    {
        if (PlayerPrefs.HasKey("UserID"))
        {
            return PlayerPrefs.GetString("UserID");
        }
        _userId = Guid.NewGuid().ToString();
        PlayerPrefs.SetString("UserID", _userId);
        PlayerPrefs.Save();
        return _userId;
    }

    private void OnDestroy()
    {
        LogAction("Player ended the game.");
    }

    public void LogAction(FirebaseAction action)
    {
        

        _db.Collection("Profile Downloads").Document().SetAsync(action).ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                UnityEngine.Debug.LogError("❌ Firestore write failed: " + task.Exception);
            }
            else
            {
                UnityEngine.Debug.Log("✅ Firestore write successful!");
            }
        });

      

    }
    
    public void LogAction(string action)
    {
        FirebaseAction firebaseAction = new FirebaseAction
        {
            User = _userId,
            Action = action,
            Timestamp = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffZ")
        };
        LogAction(firebaseAction);
    }
}