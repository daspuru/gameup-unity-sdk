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
  /// <summary>
  /// Represents a gamer's detailed standing on a leaderboard.
  /// </summary>
  public class Rank
  {

    /// <summary> Nickname, suitable for public readonly display. </summary>
    public readonly String Name ;

    /// <summary> Social ID </summary>
    public readonly string socialId ;

    /// <summary> Most up to date rank. </summary>
    public readonly long Ranking ;

    /// <summary> When the latest rank was calculated. </summary>
    public readonly long RankAt ;

    /// <summary> The best score the gamer has entered on this leaderboard. </summary>
    public readonly long Score ;

    /// <summary> When the best score was recorded. </summary>
    public readonly long ScoreAt ;

    /// <summary>
    /// If this data is in response to a leaderboard submission, and the score
    /// submitted replaces the previous one, this field will contain that
    /// previous value.
    /// </summary>
    public readonly long LastScore ;

    /// <summary> When the previous score was submitted. </summary>
    public readonly long LastScoreAt ;

    /// <summary> What the rank on this leaderboard was when it was previously checked. </summary>
    public readonly long LastRank ;

    /// <summary> When the previous rank was calculated. </summary>
    public readonly long LastRankAt ;

    /// <summary> The highest rank this gamer has ever had on this leaderboard. </summary>
    public readonly long BestRank ;

    /// <summary> When the highest rank was recorded. </summary>
    public readonly long BestRankAt ;

    /// <summary> The Total number of leaderboard ranks. Useful for displaying 'Ranking of TotalRanks'. </summary>
    public readonly long TotalRanks ;

    ///<summary> Scoretags of this entry. </summary>
    public readonly IDictionary<String, Object> Scoretags ;

    /// <returns>
    /// true if this is the first time the current gamer appears on this
    ///         leaderboard, false otherwise.
    /// </returns>
    public bool isNew ()
    {
      return LastRank == 0;
    }

    /// <returns>
    /// true if the response indicates the gamer has a new best score on
    /// this leaderboard, false otherwise.
    /// </returns>
    public bool isNewScore ()
    {
      return Score != LastScore;
    }

    /// <returns>
    /// true if the rank has changed since it was last checked,
    /// regardless if it's now higher or lower, false otherwise.
    /// </returns>
    public bool isNewRank ()
    {
      return Ranking != LastRank;
    }

    /// <returns>
    /// true if this response contains a new all-time best rank on this
    /// leaderboard, false otherwise.
    /// </returns>
    public bool isNewBestRank ()
    {
      return Ranking == BestRank && RankAt == BestRankAt;
    }

    ///<summary> Convert Scoretags to the specified user defined data type. </summary>
    public T ConvertScoretags<T> ()
    {
      string json = SimpleJson.SerializeObject (Scoretags);
      return SimpleJson.DeserializeObject<T> (json, null);
    }

    internal Rank (IDictionary<string, object> dict)
    {
      foreach (string key in dict.Keys) {
        object value;
        dict.TryGetValue (key, out value);
        if (value == null) {
          continue;
        }
        
        switch (key) {
        case "name":
          Name = (string)value;
          break;
        case "social_id":
          socialId = (string)value;
          break;
        case "rank":
          Ranking = (long)value;
          break;
        case "rank_at":
          RankAt = (long)value;
          break;
        case "score":
          Score = (long)value;
          break;
        case "score_at":
          ScoreAt = (long)value;
          break;
        case "last_score":
          LastScore = (long)value;
          break;
        case "last_score_at":
          LastScoreAt = (long)value;
          break;
        case "last_rank":
          LastRank = (long)value;
          break;
        case "last_rank_at":
          LastRankAt = (long)value;
          break;
        case "best_rank":
          BestRank = (long)value;
          break;
        case "best_rank_at":
          BestRankAt = (long)value;
          break;
        case "total_ranks":
          TotalRanks = (long)value;
          break;
        case "scoretags":
          Scoretags = (IDictionary<string,object>)value;
          break;
        }
      }
    }
  }
}
