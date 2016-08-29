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

using UnityEngine;

namespace GameUp
{

  /// <summary>
  /// Represents a GameUp leaderboard metadata and top ranked gamers.
  /// </summary>
  public class Leaderboard : IEnumerable<Leaderboard.Entry>
  {
    /// <summary> Leaderboard ID. </summary>
    public readonly String LeaderboardId ;

    /// <summary> Leaderboard display name. </summary>
    public readonly String Name ;

    /// <summary> Leaderboard public readonly identifier. </summary>
    public readonly String PublicId ;

    /// <summary> Sort order indicator. </summary>
    public readonly LeaderboardSort Sort ;

    /// <summary> Type indicator. </summary>
    public readonly LeaderboardType Type ;

    /// <summary> Leaderboard display hint. </summary>
    public readonly String DisplayHint ;

    /// <summary> Leaderboard tags. </summary>
    public readonly String[] Tags ;

    /// <summary> A leaderboard's score limit. </summary>
    public readonly long ScoreLimit ;

    /// <summary> Leaderboard creation unix time. </summary>
    public readonly long CreatedAt ;

    /// <summary> Leaderboard Reset configuration. </summary>
    public readonly Reset LeaderboardReset ;

    /// <summary> The limit on entries in the leaderboard. </summary>
    public readonly long Limit ;

    /// <summary> The current offset of the leaderboard entries. </summary>
    public readonly long Offset ;

    /// <summary> The Total number of leaderboard entries. </summary>
    public readonly long Total ;

    /// <summary> The current offset key of the leaderboard entries. </summary>
    public readonly String OffsetKey;

    /// <summary>
    /// The top ranked gamers on this board, up to 50. Already sorted according
    /// to the leaderboard sort settings.
    /// </summary>
    public readonly Entry[] Entries ;

    /// <summary>
    /// Leaderboard sort order hint.
    /// </summary>
    public enum LeaderboardSort
    {

      /// <summary> Indicates the entries should be sorted ascending by score. </summary>
      ASC,

      /// <summary> Indicates the entries should be sorted descending by score. </summary>
      DESC
    }

    /// <summary>
    /// The type of a leaderboard.
    /// </summary>
    public enum LeaderboardType
    {
      /// <summary> Standard best score, one entry per gamer leaderboard type. </summary>
      RANK
    }

    internal Leaderboard (IDictionary<string, object> dict)
    {
      foreach (string key in dict.Keys) {
        object value;
        dict.TryGetValue (key, out value);
        if (value == null) {
          continue;
        }
        string valueString = value.ToString ();

        switch (key) {
        case "leaderboard_id":
          LeaderboardId = valueString;
          break;
        case "name":
          Name = valueString;
          break;
        case "public_id":
          PublicId = valueString;
          break;
        case "offset_key":
          OffsetKey = valueString;
          break;
        case "sort":
          switch (valueString) {
          case "asc":
            Sort = LeaderboardSort.ASC;
            break;
          case "desc":
            Sort = LeaderboardSort.DESC;
            break;
          }
          break;
        case "type":
          Type = LeaderboardType.RANK;
          break;
        case "display_hint":
          DisplayHint = valueString;
          break;
        case "tags":
          List<string> tagList = new List<string> ();
          JsonArray tagArray = (JsonArray)value;
          foreach (string entryObject in tagArray) {
            tagList.Add (entryObject);
          }
          Tags = tagList.ToArray ();
          break;
        case "score_limit":
          ScoreLimit = (long)value;
          break;
        case "created_at":
          CreatedAt = (long)value;
          break;
        case "leaderboard_reset":
          LeaderboardReset = new Reset ((IDictionary<string, object>)value);
          break;
        case "limit":
          Limit = (long)value;
          break;
        case "offset":
          Offset = (long)value;
          break;
        case "total":
          Total = (long)value;
          break;
        case "entries":
          List<Leaderboard.Entry> resultList = new List<Leaderboard.Entry> ();
          JsonArray jsonArray = (JsonArray)value;
          foreach (object entryObject in jsonArray) {
            Leaderboard.Entry entry = new Leaderboard.Entry ((IDictionary<string, object>)entryObject);
            resultList.Add (entry);
          }
          Entries = resultList.ToArray ();
          break;
        }
      }
    }
      
    /// <summary>
    /// An entry for a player in a leaderboard.
    /// </summary>
    public class Entry
    {
      /// <summary> The rank in a player's entry. </summary>
      public readonly long Rank ;

      /// <summary> Nickname, suitable for public readonly display. </summary>
      public readonly String Name ;

      /// <summary> The score in a player's entry. </summary>
      public readonly long Score ;

      /// <summary> When the score was submitted to this leaderboard. </summary>
      public readonly long ScoreAt ;

      /// <summary> Scoretags in this entry. </summary>
      public readonly IDictionary<String, object> Scoretags ;

      internal Entry (IDictionary<string, object> dict)
      {
        foreach (string key in dict.Keys) {
          object value;
          dict.TryGetValue (key, out value);
          if (value == null) {
            continue;
          }

          switch (key) {
          case "rank":
            Rank = (long)value;
            break;
          case "name":
            Name = (string)value;
            break;
          case "score":
            Score = (long)value;
            break;
          case "score_at":
            ScoreAt = (long)value;
            break;
          case "scoretags":
            Scoretags = (IDictionary<string,object>)value;
            break;
          }
        }
      }

      /// <summary> Convert Scoretags to the specified user defined data type. </summary>
      public T ConvertScoretags<T> ()
      {
        string json = SimpleJson.SerializeObject (Scoretags);
        return SimpleJson.DeserializeObject<T> (json, null);
      }
    }

    /// <summary>
    /// The reset information for a daily, weekly, or monthly leaderboard.
    /// </summary>
    public class Reset
    {

      /// <summary> Leaderboard Reset Type - daily, weekly or monthly. </summary>
      public readonly ResetType Type ;

      /// <summary> Leaderboard Reset UTC Hour </summary>
      public readonly long UtcHour ;

      /// <summary> Leaderboard Reset Day in a week; will be 0 if unset. </summary>
      public readonly long DayOfWeek ;

      /// <summary> Leaderboard Reset Day in a month; will be 0 if unset. </summary>
      public readonly long DayOfMonth ;

      /// <summary>
      /// The reset type for the leaderboard.
      /// </summary>
      public enum ResetType
      {

        /// <summary> Daily value. </summary>
        DAILY,

        /// <summary> Weekly value. </summary>
        WEEKLY,

        /// <summary> Monthly value. </summary>
        MONTHLY
      }

      internal Reset (IDictionary<string, object> dict)
      {
        foreach (string key in dict.Keys) {
          object value;
          dict.TryGetValue (key, out value);
          if (value == null) {
            continue;
          }

          switch (key) {
          case "type":
            String typeString = (string)value;
            switch (typeString) {
            case "daily":
              Type = ResetType.DAILY;
              break;
            case "weekly":
              Type = ResetType.WEEKLY;
              break;
            case "monthly":
              Type = ResetType.MONTHLY;
              break;
            }
            break;
          case "utc_hour":
            UtcHour = (long)value;
            break;
          case "day_of_week":
            DayOfWeek = (long)value;
            break;
          case "day_of_month":
            DayOfMonth = (long)value;
            break;
          }
        }
      }
    }

    public IEnumerator<Leaderboard.Entry> GetEnumerator ()
    {
      return (new List<Entry> (Entries)).GetEnumerator ();
    }

    private IEnumerator GetEnumerator1 ()
    {
      return this.GetEnumerator ();
    }

    IEnumerator IEnumerable.GetEnumerator ()
    {
      return GetEnumerator1 ();
    }
  }
}

