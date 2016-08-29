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
using System.Collections.Generic;
using System.Collections;

namespace GameUp
{

  /// <summary> Represents matchmaking queue status. </summary>
  public class MatchQueueStatus
  {
    /// <summary> Filters used to queue the player. </summary>
    public readonly String[] Filters ;

    /// <summary> When the player was queued at initially </summary>
    public readonly long QueuedAt ;

    internal MatchQueueStatus (IDictionary<string, object> dict)
    {
      foreach (string key in dict.Keys) {
        object value;
        dict.TryGetValue (key, out value);
        if (value == null) {
          continue;
        }

        switch (key) {
        case "filters":
          List<string> filterList = new List<string> ();
          JsonArray filterArray = (JsonArray)value;
          foreach (string entryObject in filterArray) {
            filterList.Add (entryObject);
          }
          Filters = filterList.ToArray ();
          break;
        case "queued_at":
          QueuedAt = (long)value;
          break;
        }
      }
    }
  }

  /// <summary> Represents an active gamer in a match. </summary>
  public class ActiveGamer
  {
    /// <summary> Gamer ID. </summary>
    public readonly string GamerId;
    
    /// <summary> Gamer Nickname. </summary>
    public readonly string Nickname;
    
    internal ActiveGamer (string gamerId, string nickname)
    {
      this.GamerId = gamerId;
      this.Nickname = nickname;
    }
  }

  /// <summary> Represents all active gamers. </summary>
  public class ActiveGamers
  {
    /// <summary> List of all Active Gamers. </summary>
    public readonly List<ActiveGamer> Gamers;
    
    internal ActiveGamers (JsonArray dict)
    {
      JsonArray activeGamersArray = (JsonArray) dict;
      Gamers = new List<ActiveGamer>();
      foreach (JsonObject gamerPair in activeGamersArray) {
        object nickname;
        gamerPair.TryGetValue("nickname", out nickname);
        object gamerId;
        gamerPair.TryGetValue("gamer_id", out gamerId);

        ActiveGamer activeGamer = new ActiveGamer(gamerId.ToString(), nickname.ToString());
        Gamers.Add(activeGamer);
      }
    }

    /// <summary>
    /// Finds the first gamer that has the matching GamerId and returns Nickname
    /// </summary>
    /// <param name="gamerId">GamerID of the gamer to look for.</param>
    /// <param name="nickname">Nickname of the gamer if found, otherwise null</param>
    public void getNickname(string gamerId, out string nickname) {
      foreach (ActiveGamer gamer in Gamers) {
        if (gamer.GamerId.Equals(gamerId)) {
          nickname = gamer.Nickname;
          return;
        }
      }
      nickname = null;
    }

    /// <summary>
    /// Finds the first gamer that has the matching Nickname and returns GamerId
    /// </summary>
    /// <param name="nickname">Nickname of the gamer to look for.</param>
    /// <param name="gamerId">GamerId of the gamer if found, otherwise null</param>
    public void getGamerId(string nickname, out string gamerId) {
      foreach (ActiveGamer gamer in Gamers) {
        if (gamer.Nickname.Equals(nickname)) {
          gamerId = gamer.GamerId;
          return;
        }
      }
      gamerId = null;
    }
  }

  /// <summary>
  /// Represents a response containing GameUp Multiplayer Match with its metadata.
  /// </summary>
  public class Match
  {
    /// <summary> Nickname set for the current gamer in this given match
    /// If the current gamer's nickname is changed after the match is setup,
    /// the old nickname is still used hence the use of this 'whoami' field.
    /// </summary>
    [Obsolete("Whoami is deprecated, use WhoamiGamerId instead.")]
    public readonly String Whoami ;

    /// <summary> Gamer ID of the current player in this match. This is provided for convenience. </summary>
    public readonly String WhoamiGamerId ;

    /// <summary> Match ID </summary>
    public readonly String MatchId ;

    /// <summary> Match Filters </summary>
    public readonly String[] Filters ;

    /// <summary> Current turn number </summary>
    public readonly long TurnCount ;

    /// <summary> Name of gamer for the given turn  </summary>
    [Obsolete("Turn is deprecated, use TurnGamerId instead.")]
    public readonly String Turn ;

    /// <summary> ID of gamer for the given turn  </summary>
    public readonly String TurnGamerId ;

    /// <summary> Nickname of all the gamers in the current match </summary>
    [Obsolete("Gamers is deprecated, use ActiveGamers instead.")]
    public readonly String[] Gamers ;

    /// <summary> Active Gamers in this match. </summary>
    public readonly ActiveGamers ActiveGamers ;

    /// <summary> When the match was created </summary>
    public readonly long CreatedAt ;

    /// <summary> When the match was last updated </summary>
    public readonly long UpdatedAt ;

    /// <summary> Checks to see if the match is still ongoing or has ended. </summary>
    public readonly bool Active ;

    internal Match (IDictionary<string, object> dict)
    {
      foreach (string key in dict.Keys) {
        object value;
        dict.TryGetValue (key, out value);
        if (value == null) {
          continue;
        }
        string valueString = value.ToString ();
        
        switch (key) {
        case "match_id":
          MatchId = valueString;
          break;
        case "filters":
          List<string> filterList = new List<string> ();
          JsonArray filterArray = (JsonArray)value;
          foreach (string entryObject in filterArray) {
            filterList.Add (entryObject);
          }
          Filters = filterList.ToArray ();
          break;
        case "created_at":
          CreatedAt = (long)value;
          break;
        case "updated_at":
          UpdatedAt = (long)value;
          break;
        case "gamers":
          List<string> gamerList = new List<string> ();
          JsonArray gamerArray = (JsonArray)value;
          foreach (string entryObject in gamerArray) {
            gamerList.Add (entryObject);
          }
          Gamers = gamerList.ToArray ();
          break;
        case "active_gamers": 
          ActiveGamers = new ActiveGamers((JsonArray) value);
          break;
        case "active":
          Active = (Boolean)value;
          break;
        case "turn":
          Turn = valueString;
          break;
        case "turn_gamer_id":
          TurnGamerId = valueString;
          break;
        case "turn_count":
          TurnCount = (long)value;
          break;
        case "whoami":
          Whoami = valueString;
          break;
        case "whoami_gamer_id":
          WhoamiGamerId = valueString;
          break;
        }
      }
    }
  }
}
