using UnityEngine;
using Firebase.Firestore;
using Firebase.Extensions;
using System.Collections.Generic;
using System.Linq;

public class LeaderboardManager : MonoBehaviour
{
    public LeaderUI[] leaderEntries = new LeaderUI[7]; // Привяжи в инспекторе
    private FirebaseFirestore db;

    void Start()
    {
        db = FirebaseFirestore.DefaultInstance;
        LoadLeaderboard();
    }

    public void LoadLeaderboard()
    {
        db.Collection("users").GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.LogError("Error getting leaderboard data: " + task.Exception);
                return;
            }

            List<UserScore> users = new List<UserScore>();

            foreach (DocumentSnapshot doc in task.Result.Documents)
            {
                string name = "Unknown";
                long balls = 0;

                doc.TryGetValue("Name", out name);
                doc.TryGetValue("Balls", out balls);

                if (balls > 0)
                {
                    users.Add(new UserScore { name = name, balls = balls });
                }
            }

            var sorted = users.OrderByDescending(u => u.balls).Take(7).ToList();

           for (int i = 0; i < leaderEntries.Length; i++)
            {
                if (i < sorted.Count)
                {
                    leaderEntries[i].SetData(sorted[i].name, $"{sorted[i].balls} баллов");
                    leaderEntries[i].SetVisible(true);
                }
                else
                {
                    leaderEntries[i].SetVisible(false);
                }
            }
        });
    }

    private class UserScore
    {
        public string name;
        public long balls;
    }
}
