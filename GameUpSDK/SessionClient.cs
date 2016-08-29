// Copyright 2015 GameUp.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace GameUp
{
  /// <summary>
  /// A session client for gamer actions in the GameUp service.
  /// </summary>
  public class SessionClient
  {
    public long CreatedAt { get ; set ; }

    public string Token { get ; set ; }

    public string ApiKey { get ; set ; }

    public SessionClient ()
    {
      this.CreatedAt = (DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond);
    }

    public delegate void GamerCallback (Gamer gamer);

    public delegate void StorageGetCallback (IDictionary<String, string> data);

    public delegate void StorageGetRawCallback (string value);

    public delegate void AchievementCallback ();

    public delegate void AchievementUnlockedCallback (Achievement achievement);

    public delegate void UpdateLeaderboardCallback (Rank rank);

    public delegate void LeaderboardAndRankCallback (LeaderboardAndRank leaderboard);

    public delegate void MatchesCallback (MatchList matches);

    public delegate void MatchChangeListCallback(MatchChangeList matchChanges);

    public delegate void MatchCallback (Match match);

    public delegate void TurnCallback (MatchTurnList turns);

    public delegate void TurnSubmitSuccessCallback ();

    public delegate void MatchCreateCallback (Match match);

    public delegate void MatchQueueCallback ();

    public delegate void MatchEndCallback (String matchId);

    public delegate void MatchLeaveCallback (String matchId);

    public delegate void MatchQueueStatusCallback (MatchQueueStatus queueStatus);

    public delegate void MatchDequeueCallback ();

    public delegate void PurchaseVerifyCallback (PurchaseVerification purchaseVerification);

    public delegate void SharedStorageCallback (SharedStorageObject sharedStorageObject);

    public delegate void SharedStorageQueryCallback (SharedStorageSearchResults sharedStorageSearchResults);

    public delegate void MessageListCallback (MessageList messageList);

    public delegate void MessageCallback (Message message);

    public delegate void DatastoreCallback (DatastoreObject datastoreObject);

    public delegate void DatastoreRawCallback (string value);

    public delegate void DatastoreQueryCallback (DatastoreSearchResultList datastoreSearchResults);

    public string Serialize ()
    {
      return SimpleJson.SerializeObject (this);
    }

    public static SessionClient Deserialize (string session)
    {
      return SimpleJson.DeserializeObject<SessionClient> (session);
    }

    static string ListToString(IList objects) {
      if (objects.Count == 0) {
        return "[]";
      }

      string result = "[\"" + objects[0].ToString() + "\"";
      for (int i = 1; i < objects.Count; i++) {
        result += ",\"" + objects[i].ToString() + "\"";
      }

      result += "]";
      return result;
    }

    /// <summary>
    /// Ping the GameUp service to check it is reachable and the current session
    /// is still valid.
    /// </summary>
    /// <param name="success">The callback to execute on success.</param>
    /// <param name="error">The callback to execute on error.</param>
    public void Ping (Client.SuccessCallback success, Client.ErrorCallback error)
    {
      UriBuilder b = new UriBuilder (Client.SCHEME, Client.ApiServer, Client.PORT, "/v0/");
      WWWRequest wwwRequest = new WWWRequest (b.Uri, "GET", ApiKey, Token);
      wwwRequest.OnSuccess = (String jsonResponse) => {
        success ();
      };
      wwwRequest.OnFailure = (int statusCode, string reason) => {
        error (statusCode, reason);
      };
      wwwRequest.Execute ();
    }

    /// <summary>
    /// Get information about the current logged in gamer.
    /// </summary>
    /// <param name="success">The callback to execute on success.</param>
    /// <param name="error">The callback to execute on error.</param>
    public void Gamer (GamerCallback success, Client.ErrorCallback error)
    {
      UriBuilder b = new UriBuilder (Client.SCHEME, Client.AccountsServer, Client.PORT, "/v0/gamer");
      WWWRequest wwwRequest = new WWWRequest (b.Uri, "GET", ApiKey, Token);
      wwwRequest.OnSuccess = (String jsonResponse) => {
        success (new Gamer (SimpleJson.DeserializeObject<JsonObject> (jsonResponse)));
      };
      wwwRequest.OnFailure = (int statusCode, string reason) => {
        error (statusCode, reason);
      };
      wwwRequest.Execute ();
    }

    /// <summary>
    /// Update nickname of the current logged in gamer.
    /// </summary>
    /// <param name="nickname">Current gamer's new nickname.</param>
    /// <param name="success">The callback to execute on success.</param>
    /// <param name="error">The callback to execute on error.</param>
    public void UpdateGamer (string nickname, Client.SuccessCallback success, Client.ErrorCallback error)
    {
      UriBuilder b = new UriBuilder (Client.SCHEME, Client.AccountsServer, Client.PORT, "/v0/gamer");
      WWWRequest wwwRequest = new WWWRequest (b.Uri, "POST", ApiKey, Token);
      wwwRequest.SetBody ("{\"nickname\":\"" + nickname + "\"}");
      wwwRequest.OnSuccess = (String jsonResponse) => {
        success ();
      };
      wwwRequest.OnFailure = (int statusCode, string reason) => {
        error (statusCode, reason);
      };
      wwwRequest.Execute ();
    }

    /// <summary>
    /// Store the supplied IDictionary with the given key into Cloud Storage.
    /// </summary>
    /// <param name="key">The name of the key.</param>
    /// <param name="data">The data dictionary to store.</param>
    /// <param name="success">The callback to execute on success.</param>
    /// <param name="error">The callback to execute on error.</param>
    [Obsolete("Cloud Storage is deprecated, use Datastore instead.")]
    public void StoragePut (string key, IDictionary<string, string> data, Client.SuccessCallback success, Client.ErrorCallback error)
    {
      StoragePut (key, data, success, error);
    }

    /// <summary>
    /// Store the supplied object with the given key into Cloud Storage.
    /// </summary>
    /// <param name="key">The name of the key.</param>
    /// <param name="data">The data object to store.</param>
    /// <param name="success">The callback to execute on success.</param>
    /// <param name="error">The callback to execute on error.</param>
    [Obsolete("Cloud Storage is deprecated, use Datastore instead.")]
    public void StoragePut<T> (string key, T data, Client.SuccessCallback success, Client.ErrorCallback error)
    {
      string value = SimpleJson.SerializeObject (data);
      StoragePut (key, value, success, error);
    }

    /// <summary>
    /// Store the supplied value with the given key into Cloud Storage.
    /// </summary>
    /// <param name="key">The name of the key.</param>
    /// <param name="value">The string value to store.</param>
    /// <param name="success">The callback to execute on success.</param>
    /// <param name="error">The callback to execute on error.</param>
    [Obsolete("Cloud Storage is deprecated, use Datastore instead.")]
    public void StoragePut (string key, string value, Client.SuccessCallback success, Client.ErrorCallback error)
    {
      string path = "/v0/gamer/storage/" + Uri.EscapeUriString (key);
      UriBuilder b = new UriBuilder (Client.SCHEME, Client.ApiServer, Client.PORT, path);
      WWWRequest wwwRequest = new WWWRequest (b.Uri, "PUT", ApiKey, Token);
      wwwRequest.SetBody (value);
      wwwRequest.OnSuccess = (String jsonResponse) => {
        success ();
      };
      wwwRequest.OnFailure = (int statusCode, string reason) => {
        error (statusCode, reason);
      };
      wwwRequest.Execute ();
    }

    /// <summary>
    /// Fetch the object for the given key from Cloud Storage.
    /// </summary>
    /// <param name="key">The name of the key.</param>
    /// <param name="success">The callback to execute on success.</param>
    /// <param name="error">The callback to execute on error.</param>
    [Obsolete("Cloud Storage is deprecated, use Datastore instead.")]
    public void StorageGet (string key, StorageGetCallback success, Client.ErrorCallback error)
    {
      WWWRequest wwwRequest = BuildStorageGetRequest (key, error);
      wwwRequest.OnSuccess = (String jsonResponse) => {
        IDictionary<string, string> rawResponse = SimpleJson.DeserializeObject<IDictionary<string, string>> (jsonResponse);
        string data;
        rawResponse.TryGetValue ("value", out data);
        success (SimpleJson.DeserializeObject<Dictionary<string, string>> (data));
      };
      wwwRequest.Execute ();
    }

    /// <summary>
    /// Fetch the string object for the given key from Cloud Storage.
    /// </summary>
    /// <param name="key">The name of the key.</param>
    /// <param name="success">The callback to execute on success.</param>
    /// <param name="error">The callback to execute on error.</param>
    [Obsolete("Cloud Storage is deprecated, use Datastore instead.")]
    public void StorageGet (string key, StorageGetRawCallback success, Client.ErrorCallback error)
    {
      WWWRequest wwwRequest = BuildStorageGetRequest (key, error);
      wwwRequest.OnSuccess = (String jsonResponse) => {
        IDictionary<string, string> rawResponse = SimpleJson.DeserializeObject<IDictionary<String, string>> (jsonResponse);
        string data;
        rawResponse.TryGetValue ("value", out data);
        success (data);
      };
      wwwRequest.Execute ();
    }

    /// <summary>
    /// Fetch the string object for the given key from Cloud Storage.
    /// </summary>
    /// <param name="key">The name of the key.</param>
    /// <param name="success">The callback to execute on success.</param>
    /// <param name="error">The callback to execute on error.</param>
    [Obsolete("Cloud Storage is deprecated, use Datastore instead.")]
    public void StorageGet<T> (string key, Client.GenericSuccessCallback<T> success, Client.ErrorCallback error)
    {
      WWWRequest wwwRequest = BuildStorageGetRequest (key, error);
      wwwRequest.OnSuccess = (String jsonResponse) => {
        IDictionary<string, string> rawResponse = SimpleJson.DeserializeObject<IDictionary<String, string>> (jsonResponse);
        string data;
        rawResponse.TryGetValue ("value", out data);
        success (SimpleJson.DeserializeObject<T> (data));
      };
      wwwRequest.Execute ();
    }

    private WWWRequest BuildStorageGetRequest (string key, Client.ErrorCallback error)
    {
      string path = "/v0/gamer/storage/" + Uri.EscapeUriString (key);
      UriBuilder b = new UriBuilder (Client.SCHEME, Client.ApiServer, Client.PORT, path);
      WWWRequest wwwRequest = new WWWRequest (b.Uri, "GET", ApiKey, Token);
      wwwRequest.OnFailure = (int statusCode, string reason) => {
        error (statusCode, reason);
      };
      return wwwRequest;
    }

    /// <summary>
    /// Delete the object with the supplied key from Cloud Storage.
    /// </summary>
    /// <param name="key">The name of the key.</param>
    /// <param name="success">The callback to execute on success.</param>
    /// <param name="error">The callback to execute on error.</param>
    [Obsolete("Cloud Storage is deprecated, use Datastore instead.")]
    public void StorageDelete (string key, Client.SuccessCallback success, Client.ErrorCallback error)
    {
      string path = "/v0/gamer/storage/" + Uri.EscapeUriString (key);
      UriBuilder b = new UriBuilder (Client.SCHEME, Client.ApiServer, Client.PORT, path);
      WWWRequest wwwRequest = new WWWRequest (b.Uri, "DELETE", ApiKey, Token);
      wwwRequest.SetBody ("{}");
      wwwRequest.OnSuccess = (String jsonResponse) => {
        success ();
      };
      wwwRequest.OnFailure = (int statusCode, string reason) => {
        error (statusCode, reason);
      };
      wwwRequest.Execute ();
    }

    /// <summary>
    /// Submit progress for an achievement.
    /// </summary>
    /// <param name="id">The ID of the achievement.</param>
    /// <param name="success">The callback to execute on success.</param>
    /// <param name="successUnlocked">The callback to execute on success and unlock of an achievement</param>
    /// <param name="error">The callback to execute on error.</param>
    public void Achievement (string id, AchievementCallback success, AchievementUnlockedCallback successUnlock, Client.ErrorCallback error)
    {
      Achievement (id, 1, success, successUnlock, error);
    }

    /// <summary>
    /// Submit the supplied count as progress for an achievement.
    /// </summary>
    /// <param name="achievementId">The ID of the achievement.</param>
    /// <param name="count">The progress count to submit for the achievement.</param>
    /// <param name="success">The callback to execute on success.</param>
    /// <param name="successUnlocked">The callback to execute on success and unlock of an achievement</param>
    /// <param name="error">The callback to execute on error.</param>
    public void Achievement (string achievementId, int count, AchievementCallback success, AchievementUnlockedCallback successUnlock, Client.ErrorCallback error)
    {
      string path = "/v0/gamer/achievement/" + achievementId;
      UriBuilder b = new UriBuilder (Client.SCHEME, Client.ApiServer, Client.PORT, path);
      WWWRequest wwwRequest = new WWWRequest (b.Uri, "POST", ApiKey, Token);
      wwwRequest.SetBody ("{\"count\":" + count + "}");
      wwwRequest.OnSuccess = (String jsonResponse) => {
        if (jsonResponse.Length == 0) {
          success ();
        } else {
          successUnlock (new Achievement (SimpleJson.DeserializeObject<JsonObject> (jsonResponse)));
        }
      };
      wwwRequest.OnFailure = (int statusCode, string reason) => {
        error (statusCode, reason);
      };
      wwwRequest.Execute ();
    }

    /// <summary>
    /// Get a list of achievements available in the game; with the gamer's progress
    /// and any completed achievements.
    /// </summary>
    /// <param name="success">The callback to execute on success.</param>
    /// <param name="error">The callback to execute on error.</param>
    public void Achievements (Client.AchievementsCallback success, Client.ErrorCallback error)
    {
      UriBuilder b = new UriBuilder (Client.SCHEME, Client.ApiServer, Client.PORT, "/v0/gamer/achievement");
      WWWRequest wwwRequest = new WWWRequest (b.Uri, "GET", ApiKey, Token);
      wwwRequest.OnSuccess = (String jsonResponse) => {
        success (new AchievementList (SimpleJson.DeserializeObject<JsonObject> (jsonResponse)));
      };
      wwwRequest.OnFailure = (int statusCode, string reason) => {
        error (statusCode, reason);
      };
      wwwRequest.Execute ();
    }

    /// <summary>
    /// Submit the supplied score to the specified leaderboard. The new score will only
    /// overwrite the previous score if it is "better" according to the sorting rules;
    /// nevertheless the current gamer's rank will always be returned.
    /// This sets the Scoretags associated with the gamer's rank to 'null'
    /// </summary>
    /// <param name="id">The ID of the leaderboard.</param>
    /// <param name="score">The new score to submit to the leaderboard.</param>
    /// <param name="success">The callback to execute on success.</param>
    /// <param name="error">The callback to execute on error.</param>
    public void UpdateLeaderboard (string id, long score, UpdateLeaderboardCallback success, Client.ErrorCallback error)
    {
      UpdateLeaderboard (id, score, null, null, success, error);
    }

    /// <summary>
    /// Submit the supplied score to the specified leaderboard. The new score will only
    /// overwrite the previous score if it is "better" according to the sorting rules;
    /// nevertheless the current gamer's rank will always be returned.
    /// </summary>
    /// <param name="id">The ID of the leaderboard.</param>
    /// <param name="score">The new score to submit to the leaderboard.</param>
    /// <param name="scoreTags">Tags to persist with this leaderboard update.</param>
    /// <param name="success">The callback to execute on success.</param>
    /// <param name="error">The callback to execute on error.</param>
    public void UpdateLeaderboard<T> (string id, long score, T scoreTags, UpdateLeaderboardCallback success, Client.ErrorCallback error)
    {
      string tags = null;
      if (scoreTags != null) {
        tags = SimpleJson.SerializeObject (scoreTags);
      }
      UpdateLeaderboard (id, score, tags, success, error);
    }

    /// <summary>
    /// Submit the supplied score to the specified leaderboard. The new score will only
    /// overwrite the previous score if it is "better" according to the sorting rules;
    /// nevertheless the current gamer's rank will always be returned.
    /// </summary>
    /// <param name="id">The ID of the leaderboard.</param>
    /// <param name="score">The new score to submit to the leaderboard.</param>
    /// <param name="scoreTags">Tags to persist with this leaderboard update.</param>
    /// <param name="success">The callback to execute on success.</param>
    /// <param name="error">The callback to execute on error.</param>
    public void UpdateLeaderboard (string id, long score, string scoreTags, UpdateLeaderboardCallback success, Client.ErrorCallback error)
    {
      UpdateLeaderboard (id, score, scoreTags, null, success, error);
    }

    /// <summary>
    /// Submit the supplied score to the specified leaderboard. The new score will only
    /// overwrite the previous score if it is "better" according to the sorting rules;
    /// nevertheless the current gamer's rank will always be returned.
    /// </summary>
    /// <param name="id">The ID of the leaderboard.</param>
    /// <param name="score">The new score to submit to the leaderboard.</param>
    /// <param name="scoreTags">Tags to persist with this leaderboard update.</param>
    /// <param name="socialId">Social ID to store against this leaderboard entry.</param>
    /// <param name="success">The callback to execute on success.</param>
    /// <param name="error">The callback to execute on error.</param>
    public void UpdateLeaderboard<T> (string id, long score, T scoreTags, string socialId, UpdateLeaderboardCallback success, Client.ErrorCallback error)
    {
      string tags = null;
      if (scoreTags != null) {
        tags = SimpleJson.SerializeObject (scoreTags);
      }
      UpdateLeaderboard (id, score, tags, socialId, success, error);
    }

    /// <summary>
    /// Submit the supplied score to the specified leaderboard. The new score will only
    /// overwrite the previous score if it is "better" according to the sorting rules;
    /// nevertheless the current gamer's rank will always be returned.
    /// </summary>
    /// <param name="id">The ID of the leaderboard.</param>
    /// <param name="score">The new score to submit to the leaderboard.</param>
    /// <param name="scoreTags">Tags to persist with this leaderboard update - must be a valid json object or null.</param>
    /// <param name="socialId">Social ID to store against this leaderboard entry.</param>
    /// <param name="success">The callback to execute on success.</param>
    /// <param name="error">The callback to execute on error.</param>
    public void UpdateLeaderboard (string id, long score, string scoreTags, string socialId, UpdateLeaderboardCallback success, Client.ErrorCallback error)
    {
      string path = "/v0/leaderboard/" + id;
      UriBuilder b = new UriBuilder (Client.SCHEME, Client.ApiServer, Client.PORT, path);
      WWWRequest wwwRequest = new WWWRequest (b.Uri, "PUT", ApiKey, Token);

      if (scoreTags == null) {
        scoreTags = "null";
      }

      String body = "{\"score\":" + score + ", \"scoretags\":" + scoreTags + "}";
      wwwRequest.SetBody (body);
      
      wwwRequest.OnSuccess = (String jsonResponse) => {
        success (new Rank (SimpleJson.DeserializeObject<JsonObject> (jsonResponse)));
      };
      wwwRequest.OnFailure = (int statusCode, string reason) => {
        error (statusCode, reason);
      };
      wwwRequest.Execute ();
    }

    /// <summary>
    /// Fetch the leaderboard with the top 50 ranked gamers and the current gamer's
    /// ranking. Scoretags are not retrieved.
    /// </summary>
    /// <param name="id">The ID of the leaderboard.</param>
    /// <param name="success">The callback to execute on success.</param>
    /// <param name="error">The callback to execute on error.</param>
    public void LeaderboardAndRank (string id, LeaderboardAndRankCallback success, Client.ErrorCallback error)
    {
      LeaderboardAndRank (id, false, 10, null, success, error);
    }

    /// <summary>
    /// Fetch the leaderboard with the top 50 ranked gamers and the current gamer's
    /// ranking.
    /// </summary>
    /// <param name="id">The ID of the leaderboard.</param>
    /// <param name="scoretags">Whether to retrieve scoretags or not.</param>
    /// <param name="success">The callback to execute on success.</param>
    /// <param name="error">The callback to execute on error.</param>
    public void LeaderboardAndRank (string id, Boolean scoretags, LeaderboardAndRankCallback success, Client.ErrorCallback error)
    {
      LeaderboardAndRank (id, scoretags, 10, null, success, error);
    }

    /// <summary>
    /// Fetch the leaderboard with the ranked gamers. Automatically finds the offset
    /// of the current gamer's rank based on the limit given. Scoretags are not retrieved.
    ///
    /// For example, if the limit is 50, and the current gamer's rank is 153,
    /// result will be ranks between 150-200, with the 3rd entry belonging to the current gamer.
    ///
    /// </summary>
    /// <param name="id">The ID of the leaderboard.</param>
    /// <param name="limit">Number of entries to return. Integer between 10 and 50 inclusive</param>
    /// <param name="success">The callback to execute on success.</param>
    /// <param name="error">The callback to execute on error.</param>
    public void LeaderboardAndRank (string id, int limit, LeaderboardAndRankCallback success, Client.ErrorCallback error)
    {
      LeaderboardAndRank (id, false, limit, null, success, error);
    }

    /// <summary>
    /// Fetch the leaderboard with the ranked gamers. Automatically finds the offset
    /// of the current gamer's rank based on the limit given.
    ///
    /// For example, if the limit is 50, and the current gamer's rank is 153,
    /// result will be ranks between 150-200, with the 3rd entry belonging to the current gamer.
    ///
    /// </summary>
    /// <param name="id">The ID of the leaderboard.</param>
    /// <param name="scoretags">Whether to retrieve scoretags or not.</param>
    /// <param name="limit">Number of entries to return. Integer between 10 and 50 inclusive</param>
    /// <param name="success">The callback to execute on success.</param>
    /// <param name="error">The callback to execute on error.</param>
    public void LeaderboardAndRank (string id, Boolean scoretags, int limit, LeaderboardAndRankCallback success, Client.ErrorCallback error)
    {
      LeaderboardAndRank (id, scoretags, limit, null, success, error);
    }

    /// <summary>
    /// Fetch the leaderboard with the number of ranked gamers by limit with the offset
    /// from the top of the leaderboard ranking. Scoretags are not retrieved.
    /// </summary>
    /// <param name="id">The ID of the leaderboard.</param>
    /// <param name="limit">Number of entries to return. Integer between 10 and 50 inclusive.</param>
    /// <param name="offset">Starting point to return ranking. Must be positive, if negative it is treated as an "auto offset".</param>
    /// <param name="success">The callback to execute on success.</param>
    /// <param name="error">The callback to execute on error.</param>
    [Obsolete("Offset is deprecated, use OffsetKey instead.")]
    public void LeaderboardAndRank (string id, int limit, long offset, LeaderboardAndRankCallback success, Client.ErrorCallback error)
    {
      LeaderboardAndRank (id, false, limit, offset, success, error);
    }

    /// <summary>
    /// Fetch the leaderboard with the the number of ranked gamers by limit with the offset
    /// from the top of the leaderboard ranking.
    /// </summary>
    /// <param name="id">The ID of the leaderboard.</param>
    /// <param name="scoretags">Whether to retrieve scoretags or not.</param>
    /// <param name="limit">Number of entries to return. Integer between 10 and 50 inclusive.</param>
    /// <param name="offset">Starting point to return ranking. Must be positive, if negative it is treated as an "auto offset".</param>
    /// <param name="success">The callback to execute on success.</param>
    /// <param name="error">The callback to execute on error.</param>
    [Obsolete("Offset is deprecated, use OffsetKey instead.")]
    public void LeaderboardAndRank (string id, Boolean scoretags, int limit, long offset, LeaderboardAndRankCallback success, Client.ErrorCallback error)
    {
      LeaderboardAndRank (id, scoretags, limit, offset, offset < 0, success, error);
    }

    [Obsolete("Offset is deprecated, use OffsetKey instead.")]
    private void LeaderboardAndRank (string id, Boolean withScoretags, int limit, long offset, Boolean autoOffset, LeaderboardAndRankCallback success, Client.ErrorCallback error)
    {
      string path = "/v0/leaderboard/" + id;
      string queryParam = "?offset=" + offset + "&limit=" + limit + "&auto_offset=" + autoOffset + "&with_scoretags=" + withScoretags;
      UriBuilder b = new UriBuilder (Client.SCHEME, Client.ApiServer, Client.PORT, path, queryParam);
      WWWRequest wwwRequest = new WWWRequest (b.Uri, "GET", ApiKey, Token);
      wwwRequest.OnSuccess = (String jsonResponse) => {
        success (new LeaderboardAndRank (SimpleJson.DeserializeObject<JsonObject> (jsonResponse)));
      };
      wwwRequest.OnFailure = (int statusCode, string reason) => {
        error (statusCode, reason);
      };
      wwwRequest.Execute ();
    }

    /// <summary>
    /// Fetch the leaderboard with the the number of ranked gamers by limit with the offset
    /// from the top of the leaderboard ranking.
    /// </summary>
    /// <param name="id">The ID of the leaderboard.</param>
    /// <param name="scoretags">Whether to retrieve scoretags or not.</param>
    /// <param name="limit">Number of entries to return. Integer between 10 and 50 inclusive.</param>
    /// <param name="offsetKey">Starting point to return ranking. Can be nil or the value returned to you from previous query.</param>
    /// <param name="success">The callback to execute on success.</param>
    /// <param name="error">The callback to execute on error.</param>
    public void LeaderboardAndRank (string id, Boolean scoretags, int limit, String offsetKey, LeaderboardAndRankCallback success, Client.ErrorCallback error)
    {
      LeaderboardAndRank (id, scoretags, limit, offsetKey, false, null, success, error);
    }

    /// <summary>
    /// Fetch the leaderboard with the the number of ranked gamers by limit with the offset
    /// from the top of the leaderboard ranking.
    /// </summary>
    /// <param name="id">The ID of the leaderboard.</param>
    /// <param name="scoretags">Whether to retrieve scoretags or not.</param>
    /// <param name="limit">Number of entries to return. Integer between 10 and 50 inclusive.</param>
    /// <param name="offsetKey">Starting point to return ranking. Can be nil or the value returned to you from previous query.</param>
    /// <param name="socialIds">Filter leaderboard entries by matching 'social_id' scoretags with the supplied values.</param>
    /// <param name="success">The callback to execute on success.</param>
    /// <param name="error">The callback to execute on error.</param>
    public void LeaderboardAndRank (string id, Boolean scoretags, int limit, string offsetKey, string[] socialIds, LeaderboardAndRankCallback success, Client.ErrorCallback error)
    {
      LeaderboardAndRank (id, scoretags, limit, offsetKey, false, socialIds, success, error);
    }

    /// <summary>
    /// Fetch the leaderboard with the the number of ranked gamers by limit with the offset
    /// from the top of the leaderboard ranking.
    /// </summary>
    /// <param name="id">The ID of the leaderboard.</param>
    /// <param name="scoretags">Whether to retrieve scoretags or not.</param>
    /// <param name="limit">Number of entries to return. Integer between 10 and 50 inclusive.</param>
    /// <param name="autoOffset">Whether to find the current player's rank automatically and return relative results.</param>
    /// <param name="socialIds">Filter leaderboard entries by matching 'social_id' scoretags with the supplied values.</param>
    /// <param name="success">The callback to execute on success.</param>
    /// <param name="error">The callback to execute on error.</param>
    public void LeaderboardAndRank (string id, Boolean scoretags, int limit, bool autoOffset, string[] socialIds, LeaderboardAndRankCallback success, Client.ErrorCallback error)
    {
      LeaderboardAndRank (id, scoretags, limit, null, autoOffset, socialIds, success, error);
    }

    private void LeaderboardAndRank (string id, Boolean withScoretags, int limit, string offsetKey, Boolean autoOffset, string[] socialIds, LeaderboardAndRankCallback success, Client.ErrorCallback error)
    {
      string path = "/v0/leaderboard/" + id;
      string queryParam = "?limit=" + limit 
        + "&auto_offset=" + autoOffset 
        + "&with_scoretags=" + withScoretags;

      if (offsetKey != null) {
        queryParam = queryParam + "&offset_key=" + offsetKey;
      }

      if (socialIds != null && socialIds.Length > 0) {
        String filters = socialIds[0];
        for (int i = 1; i < socialIds.Length; i++) {
          filters += "," + socialIds[i];
        }
        queryParam = queryParam + "&social_ids=" + filters;
      }

      UriBuilder b = new UriBuilder (Client.SCHEME, Client.ApiServer, Client.PORT, path, queryParam);
      WWWRequest wwwRequest = new WWWRequest (b.Uri, "GET", ApiKey, Token);
      wwwRequest.OnSuccess = (String jsonResponse) => {
        success (new LeaderboardAndRank (SimpleJson.DeserializeObject<JsonObject> (jsonResponse)));
      };
      wwwRequest.OnFailure = (int statusCode, string reason) => {
        error (statusCode, reason);
      };
      wwwRequest.Execute ();
    }

    /// <summary>
    /// Retrieve a list of matches the gamer is part of, along with the metadata for each match. 
    /// </summary>
    /// <param name="success">The callback to execute on success.</param>
    /// <param name="error">The callback to execute on error.</param>
    public void GetAllMatches (MatchesCallback success, Client.ErrorCallback error)
    {
      UriBuilder b = new UriBuilder (Client.SCHEME, Client.ApiServer, Client.PORT, "/v0/gamer/match");
      WWWRequest wwwRequest = new WWWRequest (b.Uri, "GET", ApiKey, Token);
      wwwRequest.OnSuccess = (String jsonResponse) => {
        success (new MatchList (SimpleJson.DeserializeObject<JsonObject> (jsonResponse)));
      };
      wwwRequest.OnFailure = (int statusCode, string reason) => {
        error (statusCode, reason);
      };
      wwwRequest.Execute ();  
    }

    /// <summary>
    /// Retrieve a particular match's status and metadata.
    /// </summary>
    /// <param name="matchId">The match identifier</param>
    /// <param name="success">The callback to execute on success.</param>
    /// <param name="error">The callback to execute on error.</param>
    public void GetMatch (string matchId, MatchCallback success, Client.ErrorCallback error)
    {
      string path = "/v0/gamer/match/" + matchId;
      UriBuilder b = new UriBuilder (Client.SCHEME, Client.ApiServer, Client.PORT, path);
      WWWRequest wwwRequest = new WWWRequest (b.Uri, "GET", ApiKey, Token);
      wwwRequest.OnSuccess = (String jsonResponse) => {
        success (new Match (SimpleJson.DeserializeObject<JsonObject> (jsonResponse)));
      };
      wwwRequest.OnFailure = (int statusCode, string reason) => {
        error (statusCode, reason);
      };
      wwwRequest.Execute ();
    }

    /// <summary>
    /// Get turn data for a particular match, only returning turns newer than the identified one.
    /// </summary>
    /// <param name="matchId">The match identifier</param>
    /// <param name="turnNumber">The turn number to start from, not inclusive. Use '0' to get all the turns in the match</param>
    /// <param name="success">The callback to execute on success.</param>
    /// <param name="error">The callback to execute on error.</param>
    public void GetTurnData (string matchId, int turnNumber, TurnCallback success, Client.ErrorCallback error)
    {
      string path = "/v0/gamer/match/" + matchId + "/turn/" + turnNumber;
      UriBuilder b = new UriBuilder (Client.SCHEME, Client.ApiServer, Client.PORT, path);
      WWWRequest wwwRequest = new WWWRequest (b.Uri, "GET", ApiKey, Token);
      wwwRequest.OnSuccess = (String jsonResponse) => {
        success (new MatchTurnList (SimpleJson.DeserializeObject<JsonObject> (jsonResponse)));
      };
      wwwRequest.OnFailure = (int statusCode, string reason) => {
        error (statusCode, reason);
      };
      wwwRequest.Execute ();
    }

    /// <summary>
    /// Retrieved the list of changed match along with turn data since a given timestamp.
    /// </summary>
    /// <param name="latestMatchUpdateTimestamp">The latest value of the UpdatedAt field of all matches</param>
    /// <param name="success">The callback to execute on success.</param>
    /// <param name="error">The callback to execute on error.</param>
    public void GetChangedMatches (long latestMatchUpdateTimestamp, MatchChangeListCallback success, Client.ErrorCallback error)
    {
      string path = "/v0/gamer/matches/?since=" + latestMatchUpdateTimestamp;
      UriBuilder b = new UriBuilder (Client.SCHEME, Client.ApiServer, Client.PORT, path);
      WWWRequest wwwRequest = new WWWRequest (b.Uri, "GET", ApiKey, Token);
      wwwRequest.OnSuccess = (String jsonResponse) => {
        success (new MatchChangeList (SimpleJson.DeserializeObject<JsonObject> (jsonResponse)));
      };
      wwwRequest.OnFailure = (int statusCode, string reason) => {
        error (statusCode, reason);
      };
      wwwRequest.Execute ();
    }

    /// <summary>
    /// Submit turn data to the specified match.
    /// </summary>
    /// <param name="matchId">The match identifier</param>
    /// <param name="turn">Last seen turn number - this is used as a basic consistency check</param>
    /// <param name="nextGamer">Which gamer the next turn goes to</param>
    /// <param name="turnData">Turn data to submit</param>
    /// <param name="success">The callback to execute on success.</param>
    /// <param name="error">The callback to execute on error.</param>
    [Obsolete("SubmitTurn is deprecated, use SubmitTurnGamerId instead.")]
    public void SubmitTurn (string matchId, int turn, string nextGamer, string turnData, Client.SuccessCallback success, Client.ErrorCallback error)
    {
      string body = "{\"last_turn\":" + turn + "," +
        "\"next_gamer\":\"" + nextGamer + "\"," +
        "\"data\":\"" + turnData + "\"}";
      submitTurn (matchId, body, success, error);
    }

    /// <summary>
    /// Submit turn data to the specified match.
    /// </summary>
    /// <param name="matchId">The match identifier</param>
    /// <param name="turn">Last seen turn number - this is used as a basic consistency check</param>
    /// <param name="nextGamer">Which gamer the next turn goes to</param>
    /// <param name="turnData">
    /// Turn data to submit. The IDictionary<string, object> is serialised to a json string
    /// and further escaped into a normal string. 
    /// You need to double-deserialise if you would like to get the IDictionary back.
    /// </param>
    /// <param name="success">The callback to execute on success.</param>
    /// <param name="error">The callback to execute on error.</param>
    [Obsolete("SubmitTurn is deprecated, use SubmitTurnGamerId instead.")]
    public void SubmitTurn (string matchId, int turn, string nextGamer, IDictionary turnData, Client.SuccessCallback success, Client.ErrorCallback error)
    {
      string json = SimpleJson.SerializeObject (turnData);
      json = SimpleJson.SerializeObject (json);

      string body = "{\"last_turn\":" + turn + "," +
        "\"next_gamer\":\"" + nextGamer + "\"," +
          "\"data\":" + json + "}";

      submitTurn (matchId, body, success, error);
    }

    /// <summary>
    /// Submit turn data to the specified match.
    /// </summary>
    /// <param name="matchId">The match identifier</param>
    /// <param name="turn">Last seen turn number - this is used as a basic consistency check</param>
    /// <param name="nextGamerId">Which gamer ID the next turn goes to</param>
    /// <param name="turnData">Turn data to submit</param>
    /// <param name="success">The callback to execute on success.</param>
    /// <param name="error">The callback to execute on error.</param>
    public void SubmitTurnGamerId (string matchId, int turn, string nextGamerId, string turnData, Client.SuccessCallback success, Client.ErrorCallback error)
    {
      string body = "{\"last_turn\":" + turn + "," +
        "\"next_gamer_id\":\"" + nextGamerId + "\"," +
          "\"data\":\"" + turnData + "\"}";
      submitTurn (matchId, body, success, error);
    }
    
    /// <summary>
    /// Submit turn data to the specified match.
    /// </summary>
    /// <param name="matchId">The match identifier</param>
    /// <param name="turn">Last seen turn number - this is used as a basic consistency check</param>
    /// <param name="nextGamer">Which gamer ID the next turn goes to</param>
    /// <param name="turnData">
    /// Turn data to submit. The IDictionary<string, object> is serialised to a json string
    /// and further escaped into a normal string. 
    /// You need to double-deserialise if you would like to get the IDictionary back.
    /// </param>
    /// <param name="success">The callback to execute on success.</param>
    /// <param name="error">The callback to execute on error.</param>
    public void SubmitTurnGamerId (string matchId, int turn, string nextGamerId, IDictionary turnData, Client.SuccessCallback success, Client.ErrorCallback error)
    {
      string json = SimpleJson.SerializeObject (turnData);
      json = SimpleJson.SerializeObject (json);
      
      string body = "{\"last_turn\":" + turn + "," +
        "\"next_gamer_id\":\"" + nextGamerId + "\"," +
          "\"data\":" + json + "}";
      
      submitTurn (matchId, body, success, error);
    }

    private void submitTurn(string matchId, string body, Client.SuccessCallback success, Client.ErrorCallback error) {
      string path = "/v0/gamer/match/" + matchId + "/turn";
      UriBuilder b = new UriBuilder (Client.SCHEME, Client.ApiServer, Client.PORT, path);
      WWWRequest wwwRequest = new WWWRequest (b.Uri, "POST", ApiKey, Token);
      
      wwwRequest.SetBody (body);
      
      wwwRequest.OnSuccess = (String jsonResponse) => {
        success ();
      };
      wwwRequest.OnFailure = (int statusCode, string reason) => {
        error (statusCode, reason);
      };
      wwwRequest.Execute ();
    }

    /// <summary>
    /// Request a new match. If there are not enough waiting gamers, the current gamer will be added to the queue instead.
    /// </summary>
    /// <param name="requiredGamers">The minimal required number of gamers needed to create a new match</param>
    /// <param name="success">The callback to execute on success.</param>
    /// <param name="error">The callback to execute on error.</param>
    public void CreateMatch (int requiredGamers,  MatchCreateCallback success, MatchQueueCallback queued, Client.ErrorCallback error)
    {
      CreateMatch (requiredGamers, null, success, queued, error);
    }

    /// <summary>
    /// Request a new match. If there are not enough waiting gamers, the current gamer will be added to the queue instead.
    /// </summary>
    /// <param name="requiredGamers">The minimal required number of gamers needed to create a new match</param>
    /// <param name="matchFilters">
    /// String list to filter on matches to create or join. Exact strings matching only. Up to 8 filters. 
    /// Recommended to use values such as "[team,rank=7]" for team-based matches with players with ranks equal to 7.
    /// </param>
    /// <param name="success">The callback to execute on success.</param>
    /// <param name="error">The callback to execute on error.</param>
    public void CreateMatch (int requiredGamers, List<String> matchFilters, MatchCreateCallback success, MatchQueueCallback queued, Client.ErrorCallback error)
    {
      string path = "/v0/gamer/match/";
      UriBuilder b = new UriBuilder (Client.SCHEME, Client.ApiServer, Client.PORT, path);
      WWWRequest wwwRequest = new WWWRequest (b.Uri, "POST", ApiKey, Token);
      
      string body = "{\"players\":" + requiredGamers + "}";
      if (matchFilters != null && matchFilters.Count > 0) {
        body = "{\"players\":" + requiredGamers + ", \"filters\":" + ListToString(matchFilters) + "}";
      }
      wwwRequest.SetBody (body);
      
      wwwRequest.OnSuccess = (String jsonResponse) => {
        if (jsonResponse.Length == 0) {
          queued ();
        } else {
          success (new Match (SimpleJson.DeserializeObject<JsonObject> (jsonResponse)));
        }
      };
      wwwRequest.OnFailure = (int statusCode, string reason) => {
        error (statusCode, reason);
      };
      wwwRequest.Execute ();
    }

    /// <summary>
    /// Request a new match with the given players. The current gamer is part of the match and will be set as the first turn taker.
    /// </summary>
    /// <param name="gamers">List of Gamer ID of other players</param>
    /// <param name="success">The callback to execute on success.</param>
    /// <param name="error">The callback to execute on error.</param>
    public void CreateMatch (List<String> gamers, MatchCreateCallback success, Client.ErrorCallback error)
    {
      CreateMatch (gamers, null, success, error);
    }
    
    /// <summary>
    /// Request a new match with the given players. The current gamer is part of the match and will be set as the first turn taker.
    /// </summary>
    /// <param name="gamers">List of Gamer ID of other players</param>
    /// <param name="matchFilters">
    /// String list to filter on matches to create or join. Exact strings matching only. Up to 8 filters. 
    /// </param>
    /// <param name="success">The callback to execute on success.</param>
    /// <param name="error">The callback to execute on error.</param>
    public void CreateMatch (List<String> gamers, List<String> matchFilters, MatchCreateCallback success, Client.ErrorCallback error)
    {
      string path = "/v0/gamer/match/";
      UriBuilder b = new UriBuilder (Client.SCHEME, Client.ApiServer, Client.PORT, path);
      WWWRequest wwwRequest = new WWWRequest (b.Uri, "PUT", ApiKey, Token);
      
      string body = "{\"gamers\":" + ListToString(gamers) + "}";
      if (matchFilters != null && matchFilters.Count > 0) {
        body = "{\"gamers\":" + ListToString(gamers) + ", \"filters\":" + ListToString(matchFilters) + "}";
      }
      wwwRequest.SetBody (body);
      
      wwwRequest.OnSuccess = (String jsonResponse) => {
        success (new Match (SimpleJson.DeserializeObject<JsonObject> (jsonResponse)));
      };
      wwwRequest.OnFailure = (int statusCode, string reason) => {
        error (statusCode, reason);
      };
      wwwRequest.Execute ();
    }

    /// <summary>
    /// End match. This will only work if it's the current gamer's turn.
    /// </summary>
    /// <param name="matchId">The match identifier</param>
    /// <param name="success">The callback to execute on success.</param>
    /// <param name="error">The callback to execute on error.</param>
    public void EndMatch (string matchId, MatchEndCallback success, Client.ErrorCallback error)
    {
      string path = "/v0/gamer/match/" + matchId;
      UriBuilder b = new UriBuilder (Client.SCHEME, Client.ApiServer, Client.PORT, path);
      WWWRequest wwwRequest = new WWWRequest (b.Uri, "POST", ApiKey, Token);
      
      wwwRequest.SetBody ("{\"action\":\"end\"}");
      
      wwwRequest.OnSuccess = (String jsonResponse) => {
        success (matchId);
      };
      wwwRequest.OnFailure = (int statusCode, string reason) => {
        error (statusCode, reason);
      };
      wwwRequest.Execute ();
    }

    /// <summary>
    /// Leave match. This will only work if it's NOT the current gamer's turn.
    /// </summary>
    /// <param name="matchId">The match identifier</param>
    /// <param name="success">The callback to execute on success.</param>
    /// <param name="error">The callback to execute on error.</param>
    public void LeaveMatch (string matchId, MatchLeaveCallback success, Client.ErrorCallback error)
    {
      string path = "/v0/gamer/match/" + matchId;
      UriBuilder b = new UriBuilder (Client.SCHEME, Client.ApiServer, Client.PORT, path);
      WWWRequest wwwRequest = new WWWRequest (b.Uri, "POST", ApiKey, Token);
      
      wwwRequest.SetBody ("{\"action\":\"leave\"}");
      
      wwwRequest.OnSuccess = (String jsonResponse) => {
        success (matchId);
      };
      wwwRequest.OnFailure = (int statusCode, string reason) => {
        error (statusCode, reason);
      };
      wwwRequest.Execute ();
    }

    /// <summary>
    /// Get queue status for the current player.
    /// </summary>
    /// <param name="success">The callback to execute on success.</param>
    /// <param name="error">The callback to execute on error.</param>
    public void GetMatchQueueStatus (MatchQueueStatusCallback success, Client.ErrorCallback error)
    {
      string path = "/v0/match/queue/";
      UriBuilder b = new UriBuilder (Client.SCHEME, Client.ApiServer, Client.PORT, path);
      WWWRequest wwwRequest = new WWWRequest (b.Uri, "GET", ApiKey, Token);
      wwwRequest.OnSuccess = (String jsonResponse) => {
        success (new MatchQueueStatus(SimpleJson.DeserializeObject<JsonObject> (jsonResponse)));
      };
      wwwRequest.OnFailure = (int statusCode, string reason) => {
        error (statusCode, reason);
      };
      wwwRequest.Execute ();
    }

    /// <summary>
    /// Remove the current player from matchmaking queue.
    /// </summary>
    /// <param name="success">The callback to execute on success.</param>
    /// <param name="error">The callback to execute on error.</param>
    public void MatchDequeue (MatchDequeueCallback success, Client.ErrorCallback error)
    {
      string path = "/v0/match/queue/";
      UriBuilder b = new UriBuilder (Client.SCHEME, Client.ApiServer, Client.PORT, path);
      WWWRequest wwwRequest = new WWWRequest (b.Uri, "DELETE", ApiKey, Token);
      wwwRequest.OnSuccess = (String jsonResponse) => {
        success ();
      };
      wwwRequest.OnFailure = (int statusCode, string reason) => {
        error (statusCode, reason);
      };
      wwwRequest.Execute ();
    }


    /// <summary>
    /// Subscribes to the GameUp Push Notification using a Device Token.
    /// It will automatically detect whether the device is Android or iOS (default).
    /// </summary>
    /// <param name="deviceToken">The device token</param>
    /// <param name="segments">List of Segments to subscribe to. An empty list is acceptable.</param>
    /// <param name="success">The callback to execute on success.</param>
    /// <param name="error">The callback to execute on error.</param>
    public void SubscribePush (String deviceToken, String[] segments, Client.SuccessCallback success, Client.ErrorCallback error)
    {
      string path = "/v0/gamer/push/";
      UriBuilder b = new UriBuilder (Client.SCHEME, Client.ApiServer, Client.PORT, path);
      WWWRequest wwwRequest = new WWWRequest (b.Uri, "PUT", ApiKey, Token);

      String segmentsString = "[";
      if (segments.Length > 0) {
        segmentsString += segments [0];
        for (int i = 1; i < segments.Length; i++) {
          segmentsString += "," + segments [i];
        }
      }
      segmentsString += "]";

      String platform = "ios";
      #if UNITY_ANDROID
        platform = "android" ;
      #endif

      String requestBody = "{" +
        "\"platform\":\"" + platform + "\"," +
        "\"id\":\"" + deviceToken + "\"," +
        "\"multiplayer\":false," +
        "\"segments\":" + segmentsString +
        "}";
      wwwRequest.SetBody (requestBody);
      
      wwwRequest.OnSuccess = (String jsonResponse) => {
        success ();
      };
      wwwRequest.OnFailure = (int statusCode, string reason) => {
        error (statusCode, reason);
      };
      wwwRequest.Execute ();
    }

    /// <summary>
    /// Verify an Apple purchase with the remote service.
    /// </summary>
    /// <param name="receipt">The encoded receipt data returned by the purchase.</param>
    /// <param name="productId">The ID of the product that was purchased.</param>
    /// <param name="success">The callback to execute on success.</param>
    /// <param name="error">The callback to execute on error.</param>
    public void PurchaseVerifyApple (string receipt, string productId, PurchaseVerifyCallback success, Client.ErrorCallback error)
    {
      string path = "/v0/gamer/purchase/verify/apple";
      UriBuilder b = new UriBuilder (Client.SCHEME, Client.ApiServer, Client.PORT, path);
      WWWRequest wwwRequest = new WWWRequest (b.Uri, "POST", ApiKey, Token);
      wwwRequest.SetBody ("{\"product_id\":\"" + productId + "\",\"receipt_data\":\"" + receipt + "\"}");
      wwwRequest.OnSuccess = (String jsonResponse) => {
        success (new PurchaseVerification (SimpleJson.DeserializeObject<JsonObject> (jsonResponse)));
      };
      wwwRequest.OnFailure = (int statusCode, string reason) => {
        error (statusCode, reason);
      };
      wwwRequest.Execute ();
    }

    /// <summary>
    /// Verify a Google product purchase with the remote service.
    /// </summary>
    /// <param name="token">The purchase token returned by the purchase.</param>
    /// <param name="productId">The ID of the product that was purchased.</param>
    /// <param name="success">The callback to execute on success.</param>
    /// <param name="error">The callback to execute on error.</param>
    public void PurchaseVerifyGoogleProduct (string token, string productId, PurchaseVerifyCallback success, Client.ErrorCallback error)
    {
      string path = "/v0/gamer/purchase/verify/google";
      UriBuilder b = new UriBuilder (Client.SCHEME, Client.ApiServer, Client.PORT, path);
      WWWRequest wwwRequest = new WWWRequest (b.Uri, "POST", ApiKey, Token);
      wwwRequest.SetBody ("{\"product_id\":\"" + productId + "\",\"purchase_token\":\"" + token + "\",\"type\":\"product\"}");
      wwwRequest.OnSuccess = (String jsonResponse) => {
        success (new PurchaseVerification (SimpleJson.DeserializeObject<JsonObject> (jsonResponse)));
      };
      wwwRequest.OnFailure = (int statusCode, string reason) => {
        error (statusCode, reason);
      };
      wwwRequest.Execute ();
    }

    /// <summary>
    /// Verify a Google subscription purchase with the remote service.
    /// </summary>
    /// <param name="token">The purchase token returned by the purchase.</param>
    /// <param name="subscriptionId">The ID of the subscription that was purchased.</param>
    /// <param name="success">The callback to execute on success.</param>
    /// <param name="error">The callback to execute on error.</param>
    public void PurchaseVerifyGoogleSubscription (string token, string subscriptionId, PurchaseVerifyCallback success, Client.ErrorCallback error)
    {
      string path = "/v0/gamer/purchase/verify/google";
      UriBuilder b = new UriBuilder (Client.SCHEME, Client.ApiServer, Client.PORT, path);
      WWWRequest wwwRequest = new WWWRequest (b.Uri, "POST", ApiKey, Token);
      wwwRequest.SetBody ("{\"product_id\":\"" + subscriptionId + "\",\"purchase_token\":\"" + token + "\",\"type\":\"subscription\"}");
      wwwRequest.OnSuccess = (String jsonResponse) => {
        success (SimpleJson.DeserializeObject<PurchaseVerification> (jsonResponse));
      };
      wwwRequest.OnFailure = (int statusCode, string reason) => {
        error (statusCode, reason);
      };
      wwwRequest.Execute ();
    }

    /// <summary>
    /// Get data in Shared Storage matching the query.
    /// </summary>
    /// <param name="luceneQuery">Lucene-like query used to match.</param>
    /// <param name="success">The callback to execute on success.</param>
    /// <param name="error">The callback to execute on error.</param>
    [Obsolete("Shared Storage is deprecated, use Datastore instead.")]
    public void SharedStorageSearchGet (string luceneQuery, SharedStorageQueryCallback success, Client.ErrorCallback error)
    {
      SharedStorageSearchGet (luceneQuery, null, null, 10, 0, success, error);
    }

    /// <summary>
    /// Get data in Shared Storage matching the query.
    /// </summary>
    /// <param name="luceneQuery">Lucene-like query used to match.</param>
    /// <param name="filterKey">Key name to restrict searches to. Only results among those keys will be returned. Can be null.</param>
    /// <param name="success">The callback to execute on success.</param>
    /// <param name="error">The callback to execute on error.</param>
    [Obsolete("Shared Storage is deprecated, use Datastore instead.")]
    public void SharedStorageSearchGet (string luceneQuery, string filterKey, SharedStorageQueryCallback success, Client.ErrorCallback error)
    {
      SharedStorageSearchGet (luceneQuery, filterKey, null, 10, 0, success, error);
    }

    /// <summary>
    /// Get data in Shared Storage matching the query.
    /// </summary>
    /// <param name="luceneQuery">Lucene-like query used to match.</param>
    /// <param name="filterKey">Key name to restrict searches to. Only results among those keys will be returned. Can be null.</param>
    /// <param name="sort">Lucene-like sort clauses used to order search results. Can be null.</param>
    /// <param name="success">The callback to execute on success.</param>
    /// <param name="error">The callback to execute on error.</param>
    [Obsolete("Shared Storage is deprecated, use Datastore instead.")]
    public void SharedStorageSearchGet (string luceneQuery, string filterKey, string sort, SharedStorageQueryCallback success, Client.ErrorCallback error)
    {
      SharedStorageSearchGet (luceneQuery, filterKey, sort, 10, 0, success, error);
    }

    /// <summary>
    /// Get data in Shared Storage matching the query.
    /// </summary>
    /// <param name="luceneQuery">Lucene-like query used to match.</param>
    /// <param name="filterKey">Key name to restrict searches to. Only results among those keys will be returned. Can be null.</param>
    /// <param name="sort">Lucene-like sort clauses used to order search results. Can be null.</param>
    /// <param name="limit">Maximum number of results to return.</param>
    /// <param name="success">The callback to execute on success.</param>
    /// <param name="error">The callback to execute on error.</param>
    [Obsolete("Shared Storage is deprecated, use Datastore instead.")]
    public void SharedStorageSearchGet (string luceneQuery, string filterKey, string sort, int limit, SharedStorageQueryCallback success, Client.ErrorCallback error)
    {
      SharedStorageSearchGet (luceneQuery, filterKey, sort, 10, 0, success, error);
    }

    /// <summary>
    /// Get data in Shared Storage matching the query. Use this to paginate the results.
    /// </summary>
    /// <param name="luceneQuery">Lucene-like query used to match.</param>
    /// <param name="filterKey">Key name to restrict searches to. Only results among those keys will be returned. Can be null.</param>
    /// <param name="sort">Lucene-like sort clauses used to order search results. Can be null.</param>
    /// <param name="limit">Maximum number of results to return.</param>
    /// <param name="offset">Starting position of the result.</param>
    /// <param name="success">The callback to execute on success.</param>
    /// <param name="error">The callback to execute on error.</param>
    [Obsolete("Shared Storage is deprecated, use Datastore instead.")]
    public void SharedStorageSearchGet (string luceneQuery, string filterKey, string sort, int limit, int offset, SharedStorageQueryCallback success, Client.ErrorCallback error)
    {
      string path = "/v0/gamer/shared";
      string queryParam = "?query=" + Uri.EscapeUriString (luceneQuery) + "&limit=" + limit + "&offset=" + offset;
      if (filterKey != null) {
        queryParam += "&filter_key=" + Uri.EscapeUriString (filterKey);
      } 
      if (sort != null) {
        queryParam += "&sort=" + Uri.EscapeUriString (sort);
      }

      UriBuilder b = new UriBuilder (Client.SCHEME, Client.ApiServer, Client.PORT, path, queryParam);
      WWWRequest wwwRequest = new WWWRequest (b.Uri, "GET", ApiKey, Token);

      wwwRequest.OnSuccess = (String jsonResponse) => {
        success (new SharedStorageSearchResults (SimpleJson.DeserializeObject<JsonObject> (jsonResponse)));
      };
      wwwRequest.OnFailure = (int statusCode, string reason) => {
        error (statusCode, reason);
      };
      wwwRequest.Execute ();
    }

    /// <summary>
    /// Get data in SharedStorage matching the given key. 
    /// </summary>
    /// <param name="key">Data in shared storage in the given key. Alphanumeric characters only.</param>
    /// <param name="success">The callback to execute on success.</param>
    /// <param name="error">The callback to execute on error.</param>
    [Obsolete("Shared Storage is deprecated, use Datastore instead.")]
    public void SharedStorageGet (string key, SharedStorageCallback success, Client.ErrorCallback error)
    {
      string path = "/v0/gamer/shared/" + Uri.EscapeUriString (key);
      UriBuilder b = new UriBuilder (Client.SCHEME, Client.ApiServer, Client.PORT, path);
      WWWRequest wwwRequest = new WWWRequest (b.Uri, "GET", ApiKey, Token);
      wwwRequest.OnSuccess = (String jsonResponse) => {
        success (new SharedStorageObject (SimpleJson.DeserializeObject<JsonObject> (jsonResponse)));
      };
      wwwRequest.OnFailure = (int statusCode, string reason) => {
        error (statusCode, reason);
      };
      wwwRequest.Execute ();
    }

    /// <summary>
    /// Get data in SharedStorage matching the given key. 
    /// </summary>
    /// <param name="key">Data in shared storage in the given key. Alphanumeric characters only.</param>
    /// <param name="success">The callback to execute on success.</param>
    /// <param name="error">The callback to execute on error.</param>
    [Obsolete("Shared Storage is deprecated, use Datastore instead.")]
    public void SharedStorageGet (string key, StorageGetRawCallback success, Client.ErrorCallback error)
    {
      string path = "/v0/gamer/shared/" + Uri.EscapeUriString (key);
      UriBuilder b = new UriBuilder (Client.SCHEME, Client.ApiServer, Client.PORT, path);
      WWWRequest wwwRequest = new WWWRequest (b.Uri, "GET", ApiKey, Token);
      wwwRequest.OnSuccess = (String jsonResponse) => {
        success (jsonResponse);
      };
      wwwRequest.OnFailure = (int statusCode, string reason) => {
        error (statusCode, reason);
      };
      wwwRequest.Execute ();
    }

    /// <summary>
    /// Put data in SharedStorage for the given key. 
    /// </summary>
    /// <param name="key">Key to store data. Alphanumeric characters only.</param>
    /// <param name="data">Data to store in the key.</param>
    /// <param name="success">The callback to execute on success.</param>
    /// <param name="error">The callback to execute on error.</param>
    [Obsolete("Shared Storage is deprecated, use Datastore instead.")]
    public void SharedStoragePut<T> (string key, T data, Client.SuccessCallback success, Client.ErrorCallback error)
    {
      string value = SimpleJson.SerializeObject (data);
      SharedStoragePut (key, value, success, error);
    }

    /// <summary>
    /// Put data in SharedStorage for the given key. 
    /// </summary>
    /// <param name="key">Key to store data. Alphanumeric characters only.</param>
    /// <param name="data">Data to store in the key.</param>
    /// <param name="success">The callback to execute on success.</param>
    /// <param name="error">The callback to execute on error.</param>
    [Obsolete("Shared Storage is deprecated, use Datastore instead.")]
    public void SharedStoragePut (string key, string data, Client.SuccessCallback success, Client.ErrorCallback error)
    {
      string path = "/v0/gamer/shared/" + Uri.EscapeUriString (key) + "/public";
      UriBuilder b = new UriBuilder (Client.SCHEME, Client.ApiServer, Client.PORT, path);
      WWWRequest wwwRequest = new WWWRequest (b.Uri, "PUT", ApiKey, Token);

      wwwRequest.SetBody (data);

      wwwRequest.OnSuccess = (String jsonResponse) => {
        success ();
      };
      wwwRequest.OnFailure = (int statusCode, string reason) => {
        error (statusCode, reason);
      };
      wwwRequest.Execute ();
    }

    /// <summary>
    /// Partially update data in SharedStorage for the given key. 
    /// - If data doesn't exist, it will be added
    /// - If data exists, then the matching portion will be overwritten
    /// - If data exists, but new data is 'null' then the matching portion will be erased.
    /// </summary>
    /// <param name="key">Key to update data. Alphanumeric characters only.</param>
    /// <param name="data">Data to update in the key.</param>
    /// <param name="success">The callback to execute on success.</param>
    /// <param name="error">The callback to execute on error.</param>
    [Obsolete("Shared Storage is deprecated, use Datastore instead.")]
    public void SharedStorageUpdate<T> (string key, T data, Client.SuccessCallback success, Client.ErrorCallback error)
    {
      string value = SimpleJson.SerializeObject (data);
      SharedStorageUpdate (key, value, success, error);
    }

    /// <summary>
    /// Partially update data in SharedStorage for the given key. 
    /// - If data doesn't exist, it will be added
    /// - If data exists, then the matching portion will be overwritten
    /// - If data exists, but new data is 'null' then the matching portion will be erased.
    /// </summary>
    /// <param name="key">Key to update data. Alphanumeric characters only.</param>
    /// <param name="data">Data to update in the key.</param>
    /// <param name="success">The callback to execute on success.</param>
    /// <param name="error">The callback to execute on error.</param>
    [Obsolete("Shared Storage is deprecated, use Datastore instead.")]
    public void SharedStorageUpdate (string key, string data, Client.SuccessCallback success, Client.ErrorCallback error)
    {
      string path = "/v0/gamer/shared/" + Uri.EscapeUriString (key) + "/public";
      UriBuilder b = new UriBuilder (Client.SCHEME, Client.ApiServer, Client.PORT, path);
      WWWRequest wwwRequest = new WWWRequest (b.Uri, "PATCH", ApiKey, Token);

      wwwRequest.SetBody (data);
      
      wwwRequest.OnSuccess = (String jsonResponse) => {
        success ();
      };
      wwwRequest.OnFailure = (int statusCode, string reason) => {
        error (statusCode, reason);
      };
      wwwRequest.Execute ();
    }

    /// <summary>
    /// Delete data in SharedStorage for the given key. 
    /// </summary>
    /// <param name="key">Key to delete data.</param>
    /// <param name="success">The callback to execute on success.</param>
    /// <param name="error">The callback to execute on error.</param>
    [Obsolete("Shared Storage is deprecated, use Datastore instead.")]
    public void SharedStorageDelete (string key, Client.SuccessCallback success, Client.ErrorCallback error)
    {
      string path = "/v0/gamer/shared/" + Uri.EscapeUriString (key) + "/public";
      UriBuilder b = new UriBuilder (Client.SCHEME, Client.ApiServer, Client.PORT, path);
      WWWRequest wwwRequest = new WWWRequest (b.Uri, "DELETE", ApiKey, Token);
      wwwRequest.SetBody ("{}");
      wwwRequest.OnSuccess = (String jsonResponse) => {
        success ();
      };
      wwwRequest.OnFailure = (int statusCode, string reason) => {
        error (statusCode, reason);
      };
      wwwRequest.Execute ();
    }

    /// <summary>
    /// Executes a script on the server. Payload can be null or any object.
    /// </summary>
    /// <param name="scriptId">Script ID to execute on the server</param>
    /// <param name="payload">Payload that your script expects. Will be serialised to Json automatically. Can be set to null</param>
    /// <param name="success">The callback to execute on success.</param>
    /// <param name="error">The callback to execute on error.</param>
    [Obsolete("Execute Script is deprecated, use CloudCodeExecuteFunction instead.")]
    public void executeScript<T> (string scriptId, T payload, Client.ScriptCallback success, Client.ErrorCallback error)
    {
      string data = SimpleJson.SerializeObject (payload);
      executeScript (scriptId, data, success, error);
    }

    /// <summary>
    /// Executes a script on the server. Payload can be null or an empty string.
    /// </summary>
    /// <param name="scriptId">Script ID to execute on the server</param>
    /// <param name="payload">Payload that your script expects. Will be serialised to Json automatically. Can be set to null or an empty string</param>
    /// <param name="success">The callback to execute on success.</param>
    /// <param name="error">The callback to execute on error.</param>
    [Obsolete("Execute Script is deprecated, use CloudCodeExecuteFunction instead.")]
    public void executeScript (string scriptId, string payload, Client.ScriptCallback success, Client.ErrorCallback error)
    {
      executeScript (scriptId, payload, (string response) => {
        success (SimpleJson.DeserializeObject<IDictionary<string, object>> (response));
      }, error);
    }
    
    /// <summary>
    /// Executes a script on the server. Payload can be null or an empty string.
    /// </summary>
    /// <param name="scriptId">Script ID to execute on the server</param>
    /// <param name="payload">Payload that your script expects. Will be serialised to Json automatically. Can be set to null or an empty string</param>
    /// <param name="success">The callback to execute on success.</param>
    /// <param name="error">The callback to execute on error.</param>
    [Obsolete("Execute Script is deprecated, use CloudCodeExecuteFunction instead.")]
    public void executeScript (string scriptId, string payload, Client.ScriptRawCallback success, Client.ErrorCallback error)
    {
      string path = "/v0/game/script/" + Uri.EscapeUriString (scriptId);
      UriBuilder b = new UriBuilder (Client.SCHEME, Client.ApiServer, Client.PORT, path);
      WWWRequest wwwRequest = new WWWRequest (b.Uri, "POST", ApiKey, Token);

      if (payload != null && payload.Length != 0) {
        wwwRequest.SetBody (payload);
      } else {
        wwwRequest.SetBody ("{}");
      }

      wwwRequest.OnSuccess = (String jsonResponse) => {
        success (jsonResponse);
      };
      wwwRequest.OnFailure = (int statusCode, string reason) => {
        error (statusCode, reason);
      };
      wwwRequest.Execute ();
    }

    /// <summary>
    /// Checks and retrieves all messages from the server.
    /// </summary>
    /// <param name="fetchBody">Whether to retrieve message body as well. Recommended false as response could be large.</param>
    /// <param name="success">The callback to execute on success.</param>
    /// <param name="error">The callback to execute on error.</param>
    public void MessageList (Boolean fetchBody, MessageListCallback success, Client.ErrorCallback error)
    {
      this.MessageList (fetchBody, 0, success, error);
    }

    /// <summary>
    /// Checks and retrieves all messages from the server since a given UTC timestamp in millisecond.
    /// </summary>
    /// <param name="fetchBody">Whether to retrieve message body as well. Recommended false as response could be large.</param>
    /// <param name="newerThanUtcMillis">Get messages that are newer than this timestamp.</param>
    /// <param name="success">The callback to execute on success.</param>
    /// <param name="error">The callback to execute on error.</param>
    public void MessageList (Boolean fetchBody, long newerThanUtcMillis, MessageListCallback success, Client.ErrorCallback error)
    {
      string path = "/v0/gamer/message/?with_body=" + Uri.EscapeUriString (fetchBody.ToString ()) + "&since=" + Uri.EscapeUriString (newerThanUtcMillis.ToString ());
      UriBuilder b = new UriBuilder (Client.SCHEME, Client.ApiServer, Client.PORT, path);
      WWWRequest wwwRequest = new WWWRequest (b.Uri, "GET", ApiKey, Token);
      wwwRequest.OnSuccess = (String jsonResponse) => {
        success (new MessageList (SimpleJson.DeserializeObject<JsonObject> (jsonResponse)));
      };
      wwwRequest.OnFailure = (int statusCode, string reason) => {
        error (statusCode, reason);
      };
      wwwRequest.Execute ();
    }

    /// <summary>
    /// Retrieves a message from the mailbox. This sets the message as READ.
    /// </summary>
    /// <param name="messageId">ID of the message to be retrieved from the player's mailbox.</param>
    /// <param name="fetchBody">Whether to retrieve message body as well.</param>
    /// <param name="success">The callback to execute on success.</param>
    /// <param name="error">The callback to execute on error.</param>
    public void MessageGet (string messageId, Boolean fetchBody, MessageCallback success, Client.ErrorCallback error)
    {
      string path = "/v0/gamer/message/" + Uri.EscapeUriString (messageId) + "/?with_body=" + Uri.EscapeUriString (fetchBody.ToString ());
      UriBuilder b = new UriBuilder (Client.SCHEME, Client.ApiServer, Client.PORT, path);
      WWWRequest wwwRequest = new WWWRequest (b.Uri, "GET", ApiKey, Token);
      wwwRequest.OnSuccess = (String jsonResponse) => {
        success (new Message (SimpleJson.DeserializeObject<JsonObject> (jsonResponse)));
      };
      wwwRequest.OnFailure = (int statusCode, string reason) => {
        error (statusCode, reason);
      };
      wwwRequest.Execute ();
    }

    /// <summary>
    /// Deletes a message from the mailbox.
    /// </summary>
    /// <param name="messageId">ID of the message to be deleted from the player's mailbox.</param>
    /// <param name="success">The callback to execute on success.</param>
    /// <param name="error">The callback to execute on error.</param>
    public void MessageDelete (string messageId, Client.SuccessCallback success, Client.ErrorCallback error)
    {
      string path = "/v0/gamer/message/" + Uri.EscapeUriString (messageId);
      UriBuilder b = new UriBuilder (Client.SCHEME, Client.ApiServer, Client.PORT, path);
      WWWRequest wwwRequest = new WWWRequest (b.Uri, "DELETE", ApiKey, Token);
      wwwRequest.SetBody ("{}");
      wwwRequest.OnSuccess = (String jsonResponse) => {
        success ();
      };
      wwwRequest.OnFailure = (int statusCode, string reason) => {
        error (statusCode, reason);
      };
      wwwRequest.Execute ();
    }

    /// <summary>
    /// Make a generic request with pre-set ApiKey and Token.
    /// </summary>
    /// <param name="uri">The URI to send request to.</param>
    /// <param name="method">The method type of the request.</param>
    /// <param name="body">The body of the request.</param>
    /// <param name="success">The callback to execute on success.</param>
    /// <param name="error">The callback to execute on error.</param>
    public void MakeRequest <T> (Uri uri, string method, string body, Client.GenericSuccessCallback<T> success, Client.ErrorCallback error)
    {
      Client.RequestRawCallback rawCallback = (string jsonResponse) => {
        success (SimpleJson.DeserializeObject<T> (jsonResponse));
      };
      this.MakeRequest (uri, method, body, rawCallback, error);
    }

    /// <summary>
    /// Make a generic request with pre-set ApiKey and Token.
    /// </summary>
    /// <param name="uri">The URI to send request to.</param>
    /// <param name="method">The method type of the request.</param>
    /// <param name="body">The body of the request.</param>
    /// <param name="success">The callback to execute on success.</param>
    /// <param name="error">The callback to execute on error.</param>
    public void MakeRequest (Uri uri, string method, string body, Client.RequestRawCallback success, Client.ErrorCallback error)
    {
      WWWRequest wwwRequest = new WWWRequest (uri, method, ApiKey, Token);
      wwwRequest.SetBody (body);
      
      wwwRequest.OnSuccess = (String jsonResponse) => {
        success (jsonResponse);
      };
      wwwRequest.OnFailure = (int statusCode, string reason) => {
        error (statusCode, reason);
      };
      wwwRequest.Execute ();
    }
      
    /// <summary>
    /// Perform a search query on the data in a given Datastore table, using Lucene-like query syntax.
    /// </summary>
    /// <param name="tableName">Table name to search.</param>
    /// <param name="luceneQuery">Lucene-like query used to match.</param>
    /// <param name="success">The callback to execute on success.</param>
    /// <param name="error">The callback to execute on error.</param>
    public void DatastoreQuery (string tableName, string luceneQuery, DatastoreQueryCallback success, Client.ErrorCallback error)
    {
      DatastoreQuery (tableName, luceneQuery, null, 10, 0, success, error);
    }

    /// <summary>
    /// Perform a search query on the data in a given Datastore table, using Lucene-like query syntax.
    /// </summary>
    /// <param name="tableName">Table name to search.</param>
    /// <param name="luceneQuery">Lucene-like query used to match.</param>
    /// <param name="sort">Lucene-like sort clauses used to order search results. Can be null.</param>
    /// <param name="success">The callback to execute on success.</param>
    /// <param name="error">The callback to execute on error.</param>
    public void DatastoreQuery (string tableName, string luceneQuery, string sort, DatastoreQueryCallback success, Client.ErrorCallback error)
    {
      DatastoreQuery (tableName, luceneQuery, sort, 10, 0, success, error);
    }

    /// <summary>
    /// Perform a search query on the data in a given Datastore table, using Lucene-like query syntax.
    /// </summary>
    /// <param name="tableName">Table name to search.</param>
    /// <param name="luceneQuery">Lucene-like query used to match.</param>
    /// <param name="sort">Lucene-like sort clauses used to order search results. Can be null.</param>
    /// <param name="limit">Maximum number of results to return.</param>
    /// <param name="success">The callback to execute on success.</param>
    /// <param name="error">The callback to execute on error.</param>
    public void DatastoreQuery (string tableName, string luceneQuery, string sort, int limit, DatastoreQueryCallback success, Client.ErrorCallback error)
    {
      DatastoreQuery (tableName, luceneQuery, sort, 10, 0, success, error);
    }

    /// <summary>
    /// Perform a search query on the data in a given Datastore table, using Lucene-like query syntax.
    /// </summary>
    /// <param name="tableName">Table name to search.</param>
    /// <param name="luceneQuery">Lucene-like query used to match.</param>
    /// <param name="sort">Lucene-like sort clauses used to order search results. Can be null.</param>
    /// <param name="limit">Maximum number of results to return.</param>
    /// <param name="offset">Starting position of the result.</param>
    /// <param name="success">The callback to execute on success.</param>
    /// <param name="error">The callback to execute on error.</param>
    public void DatastoreQuery (string tableName, string luceneQuery, string sort, int limit, int offset, DatastoreQueryCallback success, Client.ErrorCallback error)
    {
      string path = "/v0/datastore/"+tableName;
      string queryParam = "?query=" + Uri.EscapeUriString (luceneQuery) + "&limit=" + limit + "&offset=" + offset;
      if (sort != null) {
        queryParam += "&sort=" + Uri.EscapeUriString (sort);
      }

      UriBuilder b = new UriBuilder (Client.SCHEME, Client.ApiServer, Client.PORT, path, queryParam);
      WWWRequest wwwRequest = new WWWRequest (b.Uri, "GET", ApiKey, Token);

      wwwRequest.OnSuccess = (String jsonResponse) => {
        success (new DatastoreSearchResultList (SimpleJson.DeserializeObject<JsonObject> (jsonResponse)));
      };
      wwwRequest.OnFailure = (int statusCode, string reason) => {
        error (statusCode, reason);
      };
      wwwRequest.Execute ();
    }

    /// <summary>
    /// Retrieve a specific key from the given Datastore table, where the key has no owner set.
    /// </summary>
    /// <param name="table">Table name to retrieve data. Alphanumeric characters only.</param>
    /// <param name="key">Key to retrieve. Alphanumeric characters only.</param>
    /// <param name="success">The callback to execute on success.</param>
    /// <param name="error">The callback to execute on error.</param>
    public void DatastoreGet (string table, string key, DatastoreCallback success, Client.ErrorCallback error)
    {
      string path = "/v0/datastore/" + Uri.EscapeUriString (table) + "/" + Uri.EscapeUriString (key);
      UriBuilder b = new UriBuilder (Client.SCHEME, Client.ApiServer, Client.PORT, path);
      WWWRequest wwwRequest = new WWWRequest (b.Uri, "GET", ApiKey, Token);
      wwwRequest.OnSuccess = (String jsonResponse) => {
        success (new DatastoreObject (SimpleJson.DeserializeObject<JsonObject> (jsonResponse)));
      };
      wwwRequest.OnFailure = (int statusCode, string reason) => {
        error (statusCode, reason);
      };
      wwwRequest.Execute ();
    }

    /// <summary>
    /// Retrieve a specific key from the given Datastore table, where the key has no owner set.
    /// </summary>
    /// <param name="table">Table name to retrieve data. Alphanumeric characters only.</param>
    /// <param name="key">Key to retrieve. Alphanumeric characters only.</param>
    /// <param name="success">The callback to execute on success.</param>
    /// <param name="error">The callback to execute on error.</param>
    public void DatastoreGet (string table, string key, DatastoreRawCallback success, Client.ErrorCallback error)
    {
      string path = "/v0/datastore/" + Uri.EscapeUriString (table) + "/" + Uri.EscapeUriString (key);
      UriBuilder b = new UriBuilder (Client.SCHEME, Client.ApiServer, Client.PORT, path);
      WWWRequest wwwRequest = new WWWRequest (b.Uri, "GET", ApiKey, Token);
      wwwRequest.OnSuccess = (String jsonResponse) => {
        success (jsonResponse);
      };
      wwwRequest.OnFailure = (int statusCode, string reason) => {
        error (statusCode, reason);
      };
      wwwRequest.Execute ();
    }

    /// <summary>
    /// Get data in a table matching the given key. 
    /// </summary>
    /// <param name="table">Table name to retrieve data. Alphanumeric characters only.</param>
    /// <param name="key">Key to retrieve. Alphanumeric characters only.</param>
    /// <param name="owner">The owner to retrieve the key for. Must be a Gamer ID, or the value “me” representing the current user.</param>
    /// <param name="success">The callback to execute on success.</param>
    /// <param name="error">The callback to execute on error.</param>
    public void DatastoreGet (string table, string key, string owner, DatastoreCallback success, Client.ErrorCallback error)
    {
      string path = "/v0/datastore/" + Uri.EscapeUriString (table) + "/" + Uri.EscapeUriString (key) + "/" + Uri.EscapeUriString (owner);
      UriBuilder b = new UriBuilder (Client.SCHEME, Client.ApiServer, Client.PORT, path);
      WWWRequest wwwRequest = new WWWRequest (b.Uri, "GET", ApiKey, Token);
      wwwRequest.OnSuccess = (String jsonResponse) => {
        success (new DatastoreObject (SimpleJson.DeserializeObject<JsonObject> (jsonResponse)));
      };
      wwwRequest.OnFailure = (int statusCode, string reason) => {
        error (statusCode, reason);
      };
      wwwRequest.Execute ();
    }

    /// <summary>
    /// Get data in a table matching the given key. 
    /// </summary>
    /// <param name="table">Table name to retrieve data. Alphanumeric characters only.</param>
    /// <param name="key">Key to retrieve. Alphanumeric characters only.</param>
    /// <param name="owner">The owner to retrieve the key for. Must be a Gamer ID, or the value “me” representing the current user.</param>
    /// <param name="success">The callback to execute on success.</param>
    /// <param name="error">The callback to execute on error.</param>
    public void DatastoreGet (string table, string key, string owner, DatastoreRawCallback success, Client.ErrorCallback error)
    {
      string path = "/v0/datastore/" + Uri.EscapeUriString (table) + "/" + Uri.EscapeUriString (key) + "/" + Uri.EscapeUriString (owner);
      UriBuilder b = new UriBuilder (Client.SCHEME, Client.ApiServer, Client.PORT, path);
      WWWRequest wwwRequest = new WWWRequest (b.Uri, "GET", ApiKey, Token);
      wwwRequest.OnSuccess = (String jsonResponse) => {
        success (jsonResponse);
      };
      wwwRequest.OnFailure = (int statusCode, string reason) => {
        error (statusCode, reason);
      };
      wwwRequest.Execute ();
    }

    /// <summary>
    /// Set the value of a specific key in the given Datastore table, where the key owner is the gamer making the request. 
    /// If the key does not exist, it will be created.
    /// Any existing data for the specified key will be completely replaced.
    /// </summary>
    /// <param name="table">The name of the table to write to.</param>
    /// <param name="key">Key to store data. Alphanumeric characters only.</param>
    /// <param name="data">Data to store in the key.</param>
    /// <param name="success">The callback to execute on success.</param>
    /// <param name="error">The callback to execute on error.</param>
    public void DatastorePut<T> (String table, string key, T data, Client.SuccessCallback success, Client.ErrorCallback error)
    {
      DatastorePut (table, key, SimpleJson.SerializeObject (data), DatastorePermission.Inherit, -1, success, error);
    }
      
    /// <summary>
    /// Update the value of a specific key in the given Datastore table, where the key owner is the gamer making the request. 
    /// If the key does not exist, it will be created.
    /// Any existing data for the specified key will be merged with the new data. 
    /// Fields specified in the new input will replace their old values, if any. 
    /// Fields present in the existing data but missing in the new input will remain unchanged.
    /// </summary>
    /// <param name="table">The name of the table to write to.</param>
    /// <param name="key">Key to update data. Alphanumeric characters only.</param>
    /// <param name="data">Data to update in the key.</param>
    /// <param name="permission">Set permissions for this key.</param>
    /// <param name="success">The callback to execute on success.</param>
    /// <param name="error">The callback to execute on error.</param>
    public void DatastorePut<T> (String table, string key, T data, DatastorePermission permission, Client.SuccessCallback success, Client.ErrorCallback error)
    {
      DatastorePut (table, key, SimpleJson.SerializeObject (data), permission, -1, success, error);
    }

    /// <summary>
    /// Update the value of a specific key in the given Datastore table, where the key owner is the gamer making the request. 
    /// If the key does not exist, it will be created.
    /// Any existing data for the specified key will be merged with the new data. 
    /// Fields specified in the new input will replace their old values, if any. 
    /// Fields present in the existing data but missing in the new input will remain unchanged.
    /// </summary>
    /// <param name="table">The name of the table to write to.</param>
    /// <param name="key">Key to update data. Alphanumeric characters only.</param>
    /// <param name="data">Data to update in the key.</param>
    /// <param name="permission">Set permissions for this key.</param>
    /// <param name="ttl">TTL in milliseconds for this data.</param>
    /// <param name="success">The callback to execute on success.</param>
    /// <param name="error">The callback to execute on error.</param>
    public void DatastorePut<T> (String table, string key, T data, DatastorePermission permission, long ttl, Client.SuccessCallback success, Client.ErrorCallback error)
    {
      DatastorePut (table, key, SimpleJson.SerializeObject (data), permission, ttl, success, error);
    }

    /// <summary>
    /// Set the value of a specific key in the given Datastore table, where the key owner is the gamer making the request. 
    /// If the key does not exist, it will be created.
    /// Any existing data for the specified key will be completely replaced.
    /// </summary>
    /// <param name="table">The name of the table to write to.</param>
    /// <param name="key">Key to store data. Alphanumeric characters only.</param>
    /// <param name="data">Data to store in the key.</param>
    /// <param name="success">The callback to execute on success.</param>
    /// <param name="error">The callback to execute on error.</param>
    public void DatastorePut (string table, string key, string data, Client.SuccessCallback success, Client.ErrorCallback error)
    {
      DatastoreSave ("PUT", table, key, data, DatastorePermission.Inherit, -1, success, error);
    }

    /// <summary>
    /// Set the value of a specific key in the given Datastore table, where the key owner is the gamer making the request. 
    /// If the key does not exist, it will be created.
    /// Any existing data for the specified key will be completely replaced.
    /// </summary>
    /// <param name="table">The name of the table to write to.</param>
    /// <param name="key">Key to store data. Alphanumeric characters only.</param>
    /// <param name="data">Data to store in the key.</param>
    /// <param name="permission">Set permissions for this key.</param>
    /// <param name="success">The callback to execute on success.</param>
    /// <param name="error">The callback to execute on error.</param>
    public void DatastorePut (string table, string key, string data, DatastorePermission permission, Client.SuccessCallback success, Client.ErrorCallback error)
    {
      DatastoreSave ("PUT", table, key, data, permission, -1, success, error);
    }

    /// <summary>
    /// Set the value of a specific key in the given Datastore table, where the key owner is the gamer making the request. 
    /// If the key does not exist, it will be created.
    /// Any existing data for the specified key will be completely replaced.
    /// </summary>
    /// <param name="table">The name of the table to write to.</param>
    /// <param name="key">Key to store data. Alphanumeric characters only.</param>
    /// <param name="data">Data to store in the key.</param>
    /// <param name="permission">Set permissions for this key.</param>
    /// <param name="ttl">TTL in milliseconds for this data.</param>
    /// <param name="success">The callback to execute on success.</param>
    /// <param name="error">The callback to execute on error.</param>
    public void DatastorePut (string table, string key, string data, DatastorePermission permission, long ttl, Client.SuccessCallback success, Client.ErrorCallback error)
    {
      DatastoreSave ("PUT", table, key, data, permission, ttl, success, error);
    }

    /// <summary>
    /// Update the value of a specific key in the given Datastore table, where the key owner is the gamer making the request. 
    /// If the key does not exist, it will be created.
    /// Any existing data for the specified key will be merged with the new data. 
    /// Fields specified in the new input will replace their old values, if any. 
    /// Fields present in the existing data but missing in the new input will remain unchanged.
    /// </summary>
    /// <param name="table">The name of the table to write to.</param>
    /// <param name="key">Key to update data. Alphanumeric characters only.</param>
    /// <param name="data">Data to update in the key.</param>
    /// <param name="success">The callback to execute on success.</param>
    /// <param name="error">The callback to execute on error.</param>
    public void DatastoreUpdate<T> (string table, string key, T data, Client.SuccessCallback success, Client.ErrorCallback error)
    {
      DatastoreUpdate (table, key, SimpleJson.SerializeObject (data), DatastorePermission.Inherit, success, error);
    }

    /// <summary>
    /// Update the value of a specific key in the given Datastore table, where the key owner is the gamer making the request. 
    /// If the key does not exist, it will be created.
    /// Any existing data for the specified key will be merged with the new data. 
    /// Fields specified in the new input will replace their old values, if any. 
    /// Fields present in the existing data but missing in the new input will remain unchanged.
    /// </summary>
    /// <param name="table">The name of the table to write to.</param>
    /// <param name="key">Key to update data. Alphanumeric characters only.</param>
    /// <param name="data">Data to update in the key.</param>
    /// <param name="permission">Set permissions for this key.</param>
    /// <param name="success">The callback to execute on success.</param>
    /// <param name="error">The callback to execute on error.</param>
    public void DatastoreUpdate<T> (string table, string key, T data, DatastorePermission permission, Client.SuccessCallback success, Client.ErrorCallback error)
    {
      DatastoreUpdate (table, key, SimpleJson.SerializeObject (data), permission, -1, success, error);
    }

    /// <summary>
    /// Update the value of a specific key in the given Datastore table, where the key owner is the gamer making the request. 
    /// If the key does not exist, it will be created.
    /// Any existing data for the specified key will be merged with the new data. 
    /// Fields specified in the new input will replace their old values, if any. 
    /// Fields present in the existing data but missing in the new input will remain unchanged.
    /// </summary>
    /// <param name="table">The name of the table to write to.</param>
    /// <param name="key">Key to update data. Alphanumeric characters only.</param>
    /// <param name="data">Data to update in the key.</param>
    /// <param name="permission">Set permissions for this key.</param>
    /// <param name="ttl">TTL in milliseconds for this data.</param>
    /// <param name="success">The callback to execute on success.</param>
    /// <param name="error">The callback to execute on error.</param>
    public void DatastoreUpdate<T> (string table, string key, T data, DatastorePermission permission, long ttl, Client.SuccessCallback success, Client.ErrorCallback error)
    {
      DatastoreUpdate (table, key, SimpleJson.SerializeObject (data), permission, ttl, success, error);
    }

    /// <summary>
    /// Update the value of a specific key in the given Datastore table, where the key owner is the gamer making the request. 
    /// If the key does not exist, it will be created.
    /// Any existing data for the specified key will be merged with the new data. 
    /// Fields specified in the new input will replace their old values, if any. 
    /// Fields present in the existing data but missing in the new input will remain unchanged.
    /// </summary>
    /// <param name="table">The name of the table to write to.</param>
    /// <param name="key">Key to update data. Alphanumeric characters only.</param>
    /// <param name="data">Data to update in the key.</param>
    /// <param name="success">The callback to execute on success.</param>
    /// <param name="error">The callback to execute on error.</param>
    public void DatastoreUpdate (string table, string key, string data, Client.SuccessCallback success, Client.ErrorCallback error)
    {
      DatastoreSave ("PATCH", table, key, data, DatastorePermission.Inherit, -1, success, error);
    }

    /// <summary>
    /// Update the value of a specific key in the given Datastore table, where the key owner is the gamer making the request. 
    /// If the key does not exist, it will be created.
    /// Any existing data for the specified key will be merged with the new data. 
    /// Fields specified in the new input will replace their old values, if any. 
    /// Fields present in the existing data but missing in the new input will remain unchanged.
    /// </summary>
    /// <param name="table">The name of the table to write to.</param>
    /// <param name="key">Key to update data. Alphanumeric characters only.</param>
    /// <param name="data">Data to update in the key.</param>
    /// <param name="permission">Set permissions for this key.</param>
    /// <param name="success">The callback to execute on success.</param>
    /// <param name="error">The callback to execute on error.</param>
    public void DatastoreUpdate (string table, string key, string data, DatastorePermission permission, Client.SuccessCallback success, Client.ErrorCallback error)
    {
      DatastoreSave ("PATCH", table, key, data, permission, -1, success, error);
    }

    /// <summary>
    /// Update the value of a specific key in the given Datastore table, where the key owner is the gamer making the request. 
    /// If the key does not exist, it will be created.
    /// Any existing data for the specified key will be merged with the new data. 
    /// Fields specified in the new input will replace their old values, if any. 
    /// Fields present in the existing data but missing in the new input will remain unchanged.
    /// </summary>
    /// <param name="table">The name of the table to write to.</param>
    /// <param name="key">Key to update data. Alphanumeric characters only.</param>
    /// <param name="data">Data to update in the key.</param>
    /// <param name="permission">Set permissions for this key.</param>
    /// <param name="ttl">TTL in milliseconds for this data.</param>
    /// <param name="success">The callback to execute on success.</param>
    /// <param name="error">The callback to execute on error.</param>
    public void DatastoreUpdate (string table, string key, string data, DatastorePermission permission, long ttl, Client.SuccessCallback success, Client.ErrorCallback error)
    {
      DatastoreSave ("PATCH", table, key, data, permission, ttl, success, error);
    }

    private void DatastoreSave(string method, string table, string key, string data, DatastorePermission permission, long ttl, Client.SuccessCallback success, Client.ErrorCallback error) {
      string path = "/v0/datastore/" + Uri.EscapeUriString (table) + "/" + Uri.EscapeUriString (key);
      UriBuilder b = new UriBuilder (Client.SCHEME, Client.ApiServer, Client.PORT, path);
      WWWRequest wwwRequest = new WWWRequest (b.Uri, method, ApiKey, Token);

      string payload = "{";
      if (permission != DatastorePermission.Inherit) {
        string p = SimpleJson.SerializeObject (DatastoreObjectMetadata.ToDictionary(permission));  
        payload += "\"permissions\":" + p + ",";
      }

      if (ttl > 0) {
        payload += "\"ttl\":" + ttl + ",";
      }

      payload += "\"data\":" + data + "}";

      wwwRequest.SetBody (payload);

      wwwRequest.OnSuccess = (String jsonResponse) => {
        success ();
      };
      wwwRequest.OnFailure = (int statusCode, string reason) => {
        error (statusCode, reason);
      };
      wwwRequest.Execute ();
    }

    /// <summary>
    /// Delete a specific key from the given Datastore table, where the owner of the key is the gamer account making the request.
    /// </summary>
    /// <param name="table">The name of the table to delete from.</param>
    /// <param name="key">Key to delete data.</param>
    /// <param name="success">The callback to execute on success.</param>
    /// <param name="error">The callback to execute on error.</param>
    public void DatastoreDelete (string table, string key, Client.SuccessCallback success, Client.ErrorCallback error)
    {
      string path = "/v0/datastore/" + Uri.EscapeUriString (table) + "/" + Uri.EscapeUriString (key);
      UriBuilder b = new UriBuilder (Client.SCHEME, Client.ApiServer, Client.PORT, path);
      WWWRequest wwwRequest = new WWWRequest (b.Uri, "DELETE", ApiKey, Token);
      wwwRequest.SetBody ("{}");
      wwwRequest.OnSuccess = (String jsonResponse) => {
        success ();
      };
      wwwRequest.OnFailure = (int statusCode, string reason) => {
        error (statusCode, reason);
      };
      wwwRequest.Execute ();
    }

    /// <summary>
    /// Executes a cloud code function on the server. Payload can be null or any object.
    /// </summary>
    /// <param name="module">Cloud Code Module name</param>
    /// <param name="function">Cloud Code function to invoke which belongs to the Module</param>
    /// <param name="payload">Payload that your script expects. Will be serialised to Json automatically. Can be set to null</param>
    /// <param name="success">The callback to execute on success.</param>
    /// <param name="error">The callback to execute on error.</param>
    public void executeCloudCodeFunction<T> (string module, string function, T payload, Client.ScriptCallback success, Client.ErrorCallback error)
    {
      string data = SimpleJson.SerializeObject (payload);
      executeCloudCodeFunction (module, function, data, success, error);
    }

    /// <summary>
    /// Executes a cloud code function on the server. Payload can be null or an empty string.
    /// </summary>
    /// <param name="module">Cloud Code Module name</param>
    /// <param name="function">Cloud Code function to invoke which belongs to the Module</param>
    /// <param name="payload">Payload that your script expects. Will be serialised to Json automatically. Can be set to null or an empty string</param>
    /// <param name="success">The callback to execute on success.</param>
    /// <param name="error">The callback to execute on error.</param>
    public void executeCloudCodeFunction (string module, string function, string payload, Client.ScriptCallback success, Client.ErrorCallback error)
    {
      executeCloudCodeFunction (module, function, payload, (string response) => {
        success (SimpleJson.DeserializeObject<JsonObject> (response));
      }, error);
    }

    /// <summary>
    /// Executes a cloud code function on the server. Payload can be null or an empty string.
    /// </summary>
    /// <param name="module">Cloud Code Module name</param>
    /// <param name="function">Cloud Code function to invoke which belongs to the Module</param>
    /// <param name="payload">Payload that your script expects. Will be serialised to Json automatically. Can be set to null or an empty string</param>
    /// <param name="success">The callback to execute on success.</param>
    /// <param name="error">The callback to execute on error.</param>
    public void executeCloudCodeFunction (string module, string function, string payload, Client.ScriptRawCallback success, Client.ErrorCallback error)
    {
      string path = "/v0/cloudcode/" + Uri.EscapeUriString (module) + "/" + Uri.EscapeUriString (function);
      UriBuilder b = new UriBuilder (Client.SCHEME, Client.ApiServer, Client.PORT, path);
      WWWRequest wwwRequest = new WWWRequest (b.Uri, "POST", ApiKey, Token);

      if (payload != null && payload.Length != 0) {
        wwwRequest.SetBody (payload);
      } else {
        wwwRequest.SetBody ("{}");
      }

      wwwRequest.OnSuccess = (String jsonResponse) => {
        success (jsonResponse);
      };
      wwwRequest.OnFailure = (int statusCode, string reason) => {
        error (statusCode, reason);
      };
      wwwRequest.Execute ();
    }

    /// <summary>
    /// Executes a cloud code function on the server.
    /// </summary>
    /// <param name="module">Cloud Code Module name</param>
    /// <param name="function">Cloud Code function to invoke which belongs to the Module</param>
    /// <param name="success">The callback to execute on success.</param>
    /// <param name="error">The callback to execute on error.</param>
    public void executeCloudCodeFunction (string module, string function, Client.ScriptCallback success, Client.ErrorCallback error)
    {
      executeCloudCodeFunction (module, function, (string response) => {
        success (SimpleJson.DeserializeObject<JsonObject> (response));
      }, error);
    }

    /// <summary>
    /// Executes a cloud code function on the server.
    /// </summary>
    /// <param name="module">Cloud Code Module name</param>
    /// <param name="function">Cloud Code function to invoke which belongs to the Module</param>
    /// <param name="success">The callback to execute on success.</param>
    /// <param name="error">The callback to execute on error.</param>
    public void executeCloudCodeFunction (string module, string function, Client.ScriptRawCallback success, Client.ErrorCallback error)
    {
      string path = "/v0/cloudcode/" + Uri.EscapeUriString (module) + "/" + Uri.EscapeUriString (function);
      UriBuilder b = new UriBuilder (Client.SCHEME, Client.ApiServer, Client.PORT, path);
      WWWRequest wwwRequest = new WWWRequest (b.Uri, "GET", ApiKey, Token);

      wwwRequest.OnSuccess = (String jsonResponse) => {
        success (jsonResponse);
      };
      wwwRequest.OnFailure = (int statusCode, string reason) => {
        error (statusCode, reason);
      };
      wwwRequest.Execute ();
    }
  }
}

