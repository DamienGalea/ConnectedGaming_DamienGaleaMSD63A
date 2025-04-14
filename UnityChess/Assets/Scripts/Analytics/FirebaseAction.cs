using Firebase.Firestore;

[FirestoreData]
public class FirebaseAction
{
    [FirestoreProperty]
    public string User { get; set; }
    
    [FirestoreProperty]
    public string Action { get; set; }
    
    [FirestoreProperty]
    public string Timestamp { get; set; }
}