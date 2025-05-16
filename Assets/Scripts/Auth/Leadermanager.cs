using UnityEngine;
using Firebase.Firestore;
using Firebase.Extensions;
using System.Collections.Generic;
using System.Linq;

public class LeaderboardManager : MonoBehaviour
{
    public LeaderUI[] leaderEntries = new LeaderUI[7];
    public GameObject leaderboardRoot;
    public GameObject loadingSpinner;

    private FirebaseFirestore db;

    void Start()
    {
        db = FirebaseFirestore.DefaultInstance;

        // Показываем кэш мгновенно, если есть
        if (CachedLeaderboard.TopUsers.Count > 0)
        {
            Display(CachedLeaderboard.TopUsers);
            if (leaderboardRoot != null) leaderboardRoot.SetActive(true);
        }
        else
        {
            if (leaderboardRoot != null) leaderboardRoot.SetActive(false);
            if (loadingSpinner != null) loadingSpinner.SetActive(true);
        }

        // Загружаем из Firestore и обновляем
        LoadFromFirestore();
    }

    private void LoadFromFirestore()
    {
        db.Collection("users").GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.LogError("Ошибка загрузки лидерборда: " + task.Exception);
                return;
            }

            List<(string name, long balls)> users = new();

            foreach (DocumentSnapshot doc in task.Result.Documents)
            {
                string name = "Unknown";
                long balls = 0;

                doc.TryGetValue("Name", out name);
                doc.TryGetValue("Balls", out balls);

                if (balls > 0)
                {
                    users.Add((name, balls));
                }
            }

            var sorted = users.OrderByDescending(u => u.balls).Take(leaderEntries.Length).ToList();
            CachedLeaderboard.TopUsers = sorted; // 🧠 кэшируем

            Display(sorted);

            if (loadingSpinner != null) loadingSpinner.SetActive(false);
            if (leaderboardRoot != null) leaderboardRoot.SetActive(true);
        });
    }

    private void Display(List<(string name, long balls)> data)
    {
        for (int i = 0; i < leaderEntries.Length; i++)
        {
            if (i < data.Count)
            {
                leaderEntries[i].SetData(data[i].name, $"{data[i].balls} баллов");
                leaderEntries[i].SetVisible(true);
            }
            else
            {
                leaderEntries[i].SetVisible(false);
            }
        }
    }
}
