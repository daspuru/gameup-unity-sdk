using UnityEngine;
using GameUp;
using System.Collections;
using System.Collections.Generic;
using System;

public class GameUpTest : MonoBehaviour
{
  Client.ErrorCallback failure = (status, reason) => {
    if (status >= 500) {
      Debug.LogError (status + ": " + reason);
    } else {
      Debug.LogWarning (status + ": " + reason);
    }
  };

  readonly string apikey = "cd711f5ef5804365a11120897f5d137e";
  readonly string achievementId = "c0b5ec14b0174f56bfae6f686c830c9a";
  readonly string leaderboardId = "b540acd249384c1784a95912a3f157c0";
  readonly string scriptId = "6ecc280b5b2b4c678eb53401c7133811";
  readonly string mailboxScriptId = "567417e01bd64533bb4677d997cc98bf";
  readonly string facebookToken = "invalid-token-1234";
  readonly string datastore_table = "unity";
  readonly string datastore_key = "army";

  void Start ()
  {
    string deviceId = SystemInfo.deviceUniqueIdentifier;
    Client.ApiKey = apikey;
    Client.EnableGZipRequest = true;
    Client.EnableGZipResponse = true;

    Client.Ping ((PingInfo server) => {
      Debug.Log ("Server Time: " + server.Time);
    }, failure);

    Client.Game ((Game game) => {
      Debug.Log (game.Name + " " + game.Description + " " + game.CreatedAt);
    }, failure);

    Debug.Log ("Anonymous Login with Id : " + deviceId + " ...");
    Client.LoginAnonymous (deviceId, (SessionClient s) => {
      Debug.Log ("Logged in anonymously: " + s.Token);

      testSerialise(s);
      testGamer(s);
      testAchievements(s);
      testLeaderboard(s);
      testScriptExecution(s);
      testDatastore(s);
      testMatch(s);

      testPush(s);

    }, failure);
  }

  void testSerialise(SessionClient session) {
    //Let's assume that we are about to save the session 
    //for later usage without having to re-login the user.
    String serializedSession = session.Serialize ();
    Debug.Log ("Saved session: " + serializedSession);

    //Let's assume that some time has passed and
    //that we are about to restore the session 
    SessionClient s = SessionClient.Deserialize (serializedSession);
    Debug.Log ("Restored session: " + s.Token);
  }

  void testGamer(SessionClient session) {
    session.Gamer ((Gamer gamer) => {
      Debug.Log ("Gamer Name: " + gamer.Name);
      Debug.Log ("Gamer ID: " + gamer.GamerId);
    }, failure);

    Client.CreateEmailAccount ("unitysdk@gameup.io", "password", "password", "UnitySDK Test", session, (SessionClient gus) => {
      session = gus;
      Debug.Log ("Created GameUp Account: " + session.Token);
    }, (status, reason) => { 
      Client.LoginEmail ("unitysdk@gameup.io", "password", (SessionClient gus) => {
        Debug.Log ("Logged in with GameUp Account: " + session.Token);
      }, failure);
    });

    Client.linkFacebook (session, facebookToken, () => {
      Debug.Log ("Facebook Linking successful: ");
    }, (status, reason) => {
      Debug.Log ("[Expected Failure] Facebook Linking Failed: " + status + " " + reason);
    });

    session.UpdateGamer ("UnitySDKTest", () => {
      Debug.Log ("Updated gamer's profile");
    }, failure);
  }

  void testAchievements(SessionClient session) {
    Client.Achievements ((AchievementList a) => {
      IEnumerator<Achievement> en = a.GetEnumerator ();
      en.MoveNext ();

      Debug.Log ("Achievements Count: " + a.Count + " " + en.Current.Name);
    }, failure);

    session.Achievement (achievementId, () => {
      Debug.Log ("Updated achievement");

      session.Achievements ((AchievementList al) => {
        Debug.Log ("Retrieved achievements " + al.Count);
        foreach (Achievement entry in al.Achievements) {
          Debug.LogFormat ("Name: " + entry.Name + " state: " + entry.State);
          Debug.LogFormat ("iscompleted: " + entry.IsCompleted());
        }
      }, failure);

    }, (Achievement a) => {
      Debug.Log ("Unlocked achievement");
    }, failure);

    session.Achievements ((AchievementList al) => {
      Debug.Log ("Retrieved achievements " + al.Count);
      foreach (Achievement entry in al.Achievements) {
        Debug.LogFormat ("Name: " + entry.Name + " state: " + entry.State);
      }
    }, failure);
  }

  void testLeaderboard(SessionClient session) {
    Client.Leaderboards ((LeaderboardList list) => {
      foreach (Leaderboard entry in list.Leaderboards) {
        Debug.LogFormat ("Name: " + entry.Name + " sort: " + entry.Sort);
        if (entry.LeaderboardReset != null) {
          Debug.LogFormat ("Name: " + entry.Name + " reset type: " + entry.LeaderboardReset.Type + " reset hr: " + entry.LeaderboardReset.UtcHour);
        } else {
          Debug.LogFormat ("Name: " + entry.Name + " has no reset!");
        }
      }
    }, failure);
      
    string[] filters = new string[2];
    filters [0] = "unity_socialId";
    filters [1] = "missing_unity_socialId";

    Client.Leaderboard (leaderboardId, 10, null, true, filters, (Leaderboard l) => {
      foreach (Leaderboard.Entry en in l.Entries) {
        Debug.Log ("Leaderboard Name: " + l.Name + " " + l.PublicId + " " + l.Sort + " " + l.Type + " " + l.Entries.Length + " " + en.Name);
      }
    }, failure);

    session.UpdateLeaderboard (leaderboardId, DateTime.Now.Millisecond, (Rank r) => {
      Debug.Log ("Updated leaderboard. New rank " + r.Ranking + " for " + r.Name);
    }, failure);

    ScoretagTest scoretagtest = new ScoretagTest ();
    scoretagtest.Datetime = DateTime.Now.Millisecond;
    session.UpdateLeaderboard (leaderboardId, DateTime.Now.Millisecond, scoretagtest, "unity_socialId", (Rank r) => {
      Debug.Log ("Updated leaderboard with scoretags. New rank " + r.Ranking + " for " + r.Name + " with tags " + r.Scoretags.ToString ());
    }, failure);

    session.LeaderboardAndRank (leaderboardId, (LeaderboardAndRank lr) => {
      Debug.Log ("1-Retrieved Leaderboard and Rank: " + lr.Leaderboard.Name);
      Debug.Log ("1-Retrieved Leaderboard and Rank: " + lr.Rank.Name + " " + lr.Rank.Ranking);
    }, failure);

    session.LeaderboardAndRank (leaderboardId, 10, (LeaderboardAndRank lr) => {
      Debug.Log ("2-Retrieved Leaderboard Entries count:  " + lr.Leaderboard.Entries.Length);
      Debug.Log ("2-Retrieved Leaderboard and Rank: " + lr.Leaderboard.Name);
      Debug.Log ("2-Retrieved Leaderboard and Rank: " + lr.Rank.Name);
      if (lr.Rank.Scoretags != null) {
        Debug.Log ("2-ScoreTags: " + lr.Rank.Scoretags.ToString ());
      }
    }, failure);
      
    session.LeaderboardAndRank (leaderboardId, true, 10, true, filters, (LeaderboardAndRank lr) => {
      Debug.Log ("3-Retrieved Leaderboard Entries count:  " + lr.Leaderboard.Entries.Length);
      Debug.Log ("3-Retrieved Leaderboard and Rank: " + lr.Leaderboard.Name);
      Debug.Log ("3-Retrieved Leaderboard and Rank: " + lr.Rank.Name);
      if (lr.Rank.Scoretags != null) {
        Debug.Log ("3-ScoreTags: " + lr.Rank.Scoretags.ToString ());
      }
    }, failure);
  }

  void testDatastore (SessionClient session) {
    Dictionary<string, object> armyData = new Dictionary<string, object> ();
    armyData.Add ("soldiers", 1000);
    armyData.Add ("tombstones", 10);

    session.DatastorePut (datastore_table, datastore_key, armyData, DatastorePermission.ReadWrite, () => {
      Debug.Log ("Datastore PUT: " + datastore_key);

      session.DatastoreGet (datastore_table, datastore_key, "me", (DatastoreObject o) => {
        Debug.Log ("Retrieved datastore object owner: " + o.Metadata.Owner);
        Debug.Log ("Retrieved datastore object data: " + GameUp.SimpleJson.SerializeObject(o.Data));
        armyData.Remove ("soldiers");
        armyData.Add ("soldiers", 1);
        session.DatastoreUpdate (datastore_table, datastore_key, armyData, DatastorePermission.Inherit, 50000, () => {
          Debug.Log ("Updated Datastore data: " + datastore_key);
          session.DatastoreQuery(datastore_table, "value.tombstones > 5", (DatastoreSearchResultList results) => {
            Debug.Log ("Searched shared storage: " + results.Count);
            foreach (DatastoreObject result in results) {
              Debug.Log ("Retrieved datastore object owner: " + result.Metadata.Owner);
              Debug.Log ("Retrieved datastore object ttl: " + result.Metadata.TTL);
              Debug.Log ("Retrieved datastore object data: " + GameUp.SimpleJson.SerializeObject(result.Data));
            }
            session.DatastoreDelete (datastore_table, datastore_key, () => {
              Debug.Log ("Deleted datastore key: " + datastore_key);
            }, failure);

          }, failure);
        }, failure);
      }, failure);
    }, failure);
  }

  void testMatch (SessionClient session)
  {
    session.Gamer ((Gamer gamer) => {
      session.GetAllMatches ((MatchList matches) => {
        Debug.Log ("Retrieved Matches. Size: " + matches.Count);
      }, failure);

      List<String> matchFilters = new List<string> ();
      matchFilters.Add ("device=unity");
      matchFilters.Add ("rank=7");
      session.CreateMatch (2, matchFilters, (Match match) => {
        Debug.Log ("New match created. Match ID: " + match.MatchId);
        String matchId = match.MatchId;

        session.GetMatch (matchId, (Match newMatch) => {
          Debug.Log ("Got match details. Match turn count: " + newMatch.TurnCount);
        }, failure);

        session.MatchDequeue(() => {
          Debug.Log ("Match dequeue successful.");
        }, failure);

        if (match.TurnGamerId.Equals (gamer.GamerId)) {
          Debug.Log ("Match details : " + match.MatchId + ". Submitting a turn for " + match.WhoamiGamerId);
          IDictionary turnData = new Dictionary<string, string> {{"turndata", "Unity SDK Turn Data"}};
          session.SubmitTurnGamerId (matchId, (int)match.TurnCount, gamer.GamerId, turnData, () => {
            session.GetTurnData (matchId, 0, (MatchTurnList turns) => {
              Debug.Log ("Got Turns. Count is: " + turns.Count);
              foreach (MatchTurn matchTurn in turns) {
                // we can update the match state to sync it with the most recent turns
                Debug.LogFormat ("User '{0}' played turn number '{1}'.", matchTurn.Gamer, matchTurn.TurnNumber);
                Debug.LogFormat ("Turn data: '{0}'.", matchTurn.Data);
              }
              session.EndMatch (matchId, (String id) => {
                Debug.Log ("Match ended: " + id);
              }, failure);
            }, failure);
          }, failure);
        } else {
          session.LeaveMatch (matchId, (String id) => {
            Debug.Log ("Left match: " + id);
          }, failure);
        }

      }, () => {
        Debug.Log ("Gamer queued");

        session.GetMatchQueueStatus ((MatchQueueStatus queueStatus) => { 
          Debug.Log ("Matchmaking queued at: " + queueStatus.QueuedAt);
          Debug.Log ("Matchmaking queue filters: " + GameUp.SimpleJson.SerializeObject(queueStatus.Filters));
        }, failure);

      }, failure);

      //instead of '0' use Match.UpdatedAt ...
      session.GetChangedMatches (0, (MatchChangeList list) => {
        Debug.Log (list.Count + " matches have changed.");
      }, failure);
    }, failure);
  }

  void testScriptExecution(SessionClient session) {
    IDictionary<string, object> scriptData = new Dictionary<string, object> ();
    scriptData.Add ("a", 1);
    scriptData.Add ("b", 2);
    session.executeScript (scriptId, scriptData, (IDictionary<string, object> response) => {
      Debug.Log ("Executed script with result:" + GameUp.SimpleJson.SerializeObject (response));
    }, failure);

    session.executeScript (mailboxScriptId, scriptData, (IDictionary<string, object> response) => {
      Debug.Log ("Executed mailbox");
      session.MessageList (false, (MessageList ms) => {
        Debug.Log ("Got " + ms.Count + " messages back");
        IEnumerator<Message> messages = ms.GetEnumerator ();
        messages.MoveNext ();
        session.MessageGet (messages.Current.MessageId, true, (Message message) => {
          Debug.Log ("Got message with ID" + message.MessageId + " with subject: " + message.Subject);
          session.MessageDelete (messages.Current.MessageId, () => {
            Debug.Log ("Deleted message with ID" + message.MessageId);
          }, failure);
        }, failure);
      }, failure);
    }, failure);
      
    session.executeCloudCodeFunction ("test", "log", scriptData, (IDictionary<string, object> response) => {
      Debug.Log ("Executed cloud code with result:" + GameUp.SimpleJson.SerializeObject (response));
    }, failure);
  }

  void testPush (SessionClient session) {
    string[] segments = new string[0];
    session.SubscribePush ("invalidPushToken", segments, () => {
      Debug.Log ("Push registration successful");
    }, failure);
  }

  void Update (){}
}

class ScoretagTest
{
  public long Datetime { get ; set ; }
}
