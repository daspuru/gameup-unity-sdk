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
using System.Net;
using UnityEngine;

namespace GameUp
{
  /// <summary>
  /// The game client for the GameUp service.
  /// </summary>
  public class Client
  {
    public static readonly string SCHEME = "https";
    public static readonly int PORT = 443;

    [Obsolete("Use Client.AccountsServer to modify the Accounts server host.")]
    public static string ACCOUNTS_SERVER = "accounts.gameup.io";

    [Obsolete("Use Client.ApiServer to modify the API server host.")]
    public static string API_SERVER = "api.gameup.io";

    public static bool EnableGZipRequest = false;
    public static bool EnableGZipResponse = false;

    /// <summary>
    /// Get or set the GameUp Accounts server host.
    /// </summary>
    /// <value>The GameUp Accounts server host.</value>
    public static string AccountsServer = ACCOUNTS_SERVER;

    /// <summary>
    /// Get or set the GameUp API server host.
    /// </summary>
    /// <value>The GameUp API server host.</value>
    public static string ApiServer = API_SERVER;

    /// <summary>
    /// Get or set the GameUp API key.
    /// </summary>
    /// <value>The GameUp API key.</value>
    public static string ApiKey { get; set; }

    private Client ()
    {
    }

    public delegate void PingCallback (PingInfo info);

    public delegate void ServerCallback (ServerInfo info);

    public delegate void GameCallback (Game game);

    public delegate void AchievementsCallback (AchievementList achievements);

    public delegate void LeaderboardsCallback (LeaderboardList leaderboards);

    public delegate void LeaderboardCallback (Leaderboard leaderboard);

    public delegate void LoginCallback (SessionClient session);

    public delegate void AccountCheckCallback (bool exists, bool isCurrentGamer);

    public delegate void SuccessCallback ();

    public delegate void GenericSuccessCallback<T> (T data);

    public delegate void ErrorCallback (int statusCode, string reason);

    public delegate void ScriptCallback (IDictionary<String, object> data);

    public delegate void ScriptRawCallback (string data);

    public delegate void RequestRawCallback (string data);

    /// <summary>
    /// Ping the GameUp service to check it is reachable.
    /// </summary>
    /// <param name="error">The callback to execute on error.</param>
    public static void Ping (ErrorCallback error)
    {
      Ping (() => {}, error);
    }

    /// <summary>
    /// Ping the GameUp service to check it is reachable.
    /// </summary>
    /// <param name="success">The callback to execute on success.</param>
    /// <param name="error">The callback to execute on error.</param>
    public static void Ping (SuccessCallback success, ErrorCallback error)
    {
      UriBuilder b = new UriBuilder (SCHEME, ApiServer, PORT, "/v0/");
      WWWRequest wwwRequest = new WWWRequest (b.Uri, "GET", ApiKey, "");
      wwwRequest.OnSuccess = (String jsonResponse) => {
        success ();
      };
      wwwRequest.OnFailure = (int statusCode, string reason) => {
        error (statusCode, reason);
      };
      wwwRequest.Execute ();
    }

    /// <summary>
    /// Ping the GameUp service to check it is reachable.
    /// </summary>
    /// <param name="success">The callback to execute on success and returns available ping data.</param>
    /// <param name="error">The callback to execute on error.</param>
    public static void Ping (PingCallback success, ErrorCallback error)
    {
      UriBuilder b = new UriBuilder (SCHEME, ApiServer, PORT, "/v0/");
      WWWRequest wwwRequest = new WWWRequest (b.Uri, "GET", ApiKey, "");
      wwwRequest.OnSuccess = (String jsonResponse) => {
        success (new PingInfo (SimpleJson.DeserializeObject<JsonObject> (jsonResponse)));
      };
      wwwRequest.OnFailure = (int statusCode, string reason) => {
        error (statusCode, reason);
      };
      wwwRequest.Execute ();
    }

    /// <summary>
    /// Retrieve global service information from GameUp; includes server
    /// time and other information.
    /// </summary>
    /// <param name="success">The callback to execute on success.</param>
    /// <param name="error">The callback to execute on error.</param>
    public static void Server (ServerCallback success, ErrorCallback error)
    {
      UriBuilder b = new UriBuilder (SCHEME, ApiServer, PORT, "/v0/server");
      WWWRequest wwwRequest = new WWWRequest (b.Uri, "GET", ApiKey, "");
      wwwRequest.OnSuccess = (String jsonResponse) => {
        success (new ServerInfo (SimpleJson.DeserializeObject<JsonObject> (jsonResponse)));
      };
      wwwRequest.OnFailure = (int statusCode, string reason) => {
        error (statusCode, reason);
      };
      wwwRequest.Execute ();
    }

    /// <summary>
    /// Retrieve information about the Game associated to the API Key. API Keys
    /// are obtained from the GameUp Dashboard - https://dashboard.gameup.io
    /// </summary>
    /// <param name="success">The callback to execute on success.</param>
    /// <param name="error">The callback to execute on error.</param>
    public static void Game (GameCallback success, ErrorCallback error)
    {
      UriBuilder b = new UriBuilder (SCHEME, ApiServer, PORT, "/v0/game");
      WWWRequest wwwRequest = new WWWRequest (b.Uri, "GET", ApiKey, "");
      wwwRequest.OnSuccess = (String jsonResponse) => {
        success (new Game (SimpleJson.DeserializeObject<JsonObject> (jsonResponse)));
      };
      wwwRequest.OnFailure = (int statusCode, string reason) => {
        error (statusCode, reason);
      };
      wwwRequest.Execute ();
    }

    /// <summary>
    /// Get a list of achievements available in the game. This excludes any gamer
    /// information for the achievements; their progress or completion timestamps.
    /// See SessionClient.Achievements (...) for Gamer information.
    /// </summary>
    /// <param name="success">The callback to execute on success.</param>
    /// <param name="error">The callback to execute on error.</param>
    public static void Achievements (AchievementsCallback success, ErrorCallback error)
    {
      UriBuilder b = new UriBuilder (SCHEME, ApiServer, PORT, "/v0/game/achievement");
      WWWRequest wwwRequest = new WWWRequest (b.Uri, "GET", ApiKey, "");
      wwwRequest.OnSuccess = (String jsonResponse) => {
        success (new AchievementList (SimpleJson.DeserializeObject<JsonObject> (jsonResponse)));
      };
      wwwRequest.OnFailure = (int statusCode, string reason) => {
        error (statusCode, reason);
      };
      wwwRequest.Execute ();
    }

    /// <summary>
    /// Retrieve the list of available leaderboards for this game.
    /// </summary>
    /// <param name="success">The callback to execute on success.</param>
    /// <param name="error">The callback to execute on error.</param>
    public static void Leaderboards (LeaderboardsCallback success, ErrorCallback error)
    {
      UriBuilder b = new UriBuilder (SCHEME, ApiServer, PORT, "/v0/game/leaderboard");
      WWWRequest wwwRequest = new WWWRequest (b.Uri, "GET", ApiKey, "");
      wwwRequest.OnSuccess = (String jsonResponse) => {
        success (new LeaderboardList (SimpleJson.DeserializeObject<JsonObject> (jsonResponse)));
      };
      wwwRequest.OnFailure = (int statusCode, string reason) => {
        error (statusCode, reason);
      };
      wwwRequest.Execute ();
    }

    /// <summary>
    /// Get the leaderboard with the supplied ID; this will contain the top ranked
    /// gamers on the leaderboard.
    /// </summary>
    /// <param name="id">The ID of the Leaderboard.</param>
    /// <param name="success">The callback to execute on success.</param>
    /// <param name="error">The callback to execute on error.</param>
    public static void Leaderboard (string id, LeaderboardCallback success, ErrorCallback error)
    {
      Leaderboard (id, 50, 0, false, success, error);
    }

    /// <summary>
    /// Get the leaderboard with the supplied ID; this will contain the top ranked
    /// gamers on the leaderboard.
    /// </summary>
    /// <param name="id">The ID of the Leaderboard.</param>
    /// <param name="limit">Number of entries to return. Default is 50.</param>
    /// <param name="offset">Entries to start from with the leaderboard list. Default is 0.</param>
    /// <param name="withScoretags">Include Scoretags in the leaderboard entries.</param>
    /// <param name="success">The callback to execute on success.</param>
    /// <param name="error">The callback to execute on error.</param>
    public static void Leaderboard (string id, int limit, int offset, bool withScoretags, LeaderboardCallback success, ErrorCallback error)
    {
      string path = "/v0/game/leaderboard/" + id;
      string queryParam = "?offset=" + offset + "&limit=" + limit + "&with_scoretags=" + withScoretags;
      // HttpUtility.ParseQueryString is not available in Unity.
      UriBuilder b = new UriBuilder (SCHEME, ApiServer, PORT, path, queryParam);
      WWWRequest wwwRequest = new WWWRequest (b.Uri, "GET", ApiKey, "");
      wwwRequest.OnSuccess = (String jsonResponse) => {
        success (new Leaderboard (SimpleJson.DeserializeObject<JsonObject> (jsonResponse)));
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
    public static void executeScript<T> (string scriptId, T payload, ScriptCallback success, Client.ErrorCallback error)
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
    public static void executeScript (string scriptId, string payload, ScriptCallback success, Client.ErrorCallback error)
    {
      executeScript (scriptId, payload, (string response) => {
        success (SimpleJson.DeserializeObject<JsonObject> (response));
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
    public static void executeScript (string scriptId, string payload, ScriptRawCallback success, Client.ErrorCallback error)
    {
      string path = "/v0/game/script/" + Uri.EscapeUriString (scriptId);
      UriBuilder b = new UriBuilder (Client.SCHEME, Client.ApiServer, Client.PORT, path);
      WWWRequest wwwRequest = new WWWRequest (b.Uri, "POST", ApiKey, "");

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
      WWWRequest wwwRequest = new WWWRequest (b.Uri, "POST", ApiKey, "");

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
      WWWRequest wwwRequest = new WWWRequest (b.Uri, "GET", ApiKey, "");

      wwwRequest.OnSuccess = (String jsonResponse) => {
        success (jsonResponse);
      };
      wwwRequest.OnFailure = (int statusCode, string reason) => {
        error (statusCode, reason);
      };
      wwwRequest.Execute ();
    }

    /// <summary>
    /// Create a new account for the gamer using a GameUp Email / Password combination and link an existing gamer token.
    /// </summary>
    /// <param name="email">Gamer's Email.</param>
    /// <param name="password">Gamer's Password.</param>
    /// <param name="confirm_password">Gamer's Password Confirmation</param>
    /// <param name="name">Gamer's Name - Optional</param>
    /// <param name="success">The callback to execute on success.</param>
    /// <param name="error">The callback to execute on error.</param>
    [Obsolete("CreateGameUpAccount is deprecated, use CreateEmailAccount instead.")]
    public static void CreateGameUpAccount (string email, string password, string confirm_password, string name, LoginCallback success, ErrorCallback error)
    {
      CreateEmailAccount (email, password, confirm_password, name, "", null, success, error);
    }

    /// <summary>
    /// Create a new account for the gamer using an Email / Password combination and link an existing gamer token.
    /// </summary>
    /// <param name="email">Gamer's Email.</param>
    /// <param name="password">Gamer's Password.</param>
    /// <param name="confirm_password">Gamer's Password Confirmation</param>
    /// <param name="name">Gamer's Name - Optional</param>
    /// <param name="success">The callback to execute on success.</param>
    /// <param name="error">The callback to execute on error.</param>
    public static void CreateEmailAccount (string email, string password, string confirm_password, string name, LoginCallback success, ErrorCallback error)
    {
      CreateEmailAccount (email, password, confirm_password, name, "", null, success, error);
    }

    /// <summary>
    /// Create a new account for the gamer using a GameUp Email / Password combination with a nickname.
    /// </summary>
    /// <param name="email">Gamer's Email.</param>
    /// <param name="password">Gamer's Password.</param>
    /// <param name="confirm_password">Gamer's Password Confirmation</param>
    /// <param name="name">Gamer's Name - Optional</param>
    /// <param name="nickname">Gamer's Nickname - Optional</param>
    /// <param name="success">The callback to execute on success.</param>
    /// <param name="error">The callback to execute on error.</param>
    [Obsolete("CreateGameUpAccount is deprecated, use CreateEmailAccount instead.")]
    public static void CreateGameUpAccount (string email, string password, string confirm_password, string name, string nickname, LoginCallback success, ErrorCallback error)
    {
      CreateEmailAccount (email, password, confirm_password, name, nickname, null, success, error);
    }

    /// <summary>
    /// Create a new account for the gamer using an Email / Password combination with a nickname.
    /// </summary>
    /// <param name="email">Gamer's Email.</param>
    /// <param name="password">Gamer's Password.</param>
    /// <param name="confirm_password">Gamer's Password Confirmation</param>
    /// <param name="name">Gamer's Name - Optional</param>
    /// <param name="nickname">Gamer's Nickname - Optional</param>
    /// <param name="success">The callback to execute on success.</param>
    /// <param name="error">The callback to execute on error.</param>
    public static void CreateEmailAccount (string email, string password, string confirm_password, string name, string nickname, LoginCallback success, ErrorCallback error)
    {
      CreateEmailAccount (email, password, confirm_password, name, nickname, null, success, error);
    }

    /// <summary>
    /// Create a new account for the gamer using a GameUp Email / Password combination and link an existing gamer token.
    /// </summary>
    /// <param name="email">Gamer's Email.</param>
    /// <param name="password">Gamer's Password.</param>
    /// <param name="confirm_password">Gamer's Password Confirmation</param>
    /// <param name="name">Gamer's Name</param>
    /// <param name="session">An existing session client.</param>
    /// <param name="success">The callback to execute on success.</param>
    /// <param name="error">The callback to execute on error.</param>
    [Obsolete("CreateGameUpAccount is deprecated, use CreateEmailAccount instead.")]
    public static void CreateGameUpAccount (string email, string password, string confirm_password, string name, SessionClient session, LoginCallback success, ErrorCallback error)
    {
      CreateGameUpAccount (email, password, confirm_password, name, "", session, success, error);
    }

    /// <summary>
    /// Create a new account for the gamer using an Email / Password combination and link an existing gamer token.
    /// </summary>
    /// <param name="email">Gamer's Email.</param>
    /// <param name="password">Gamer's Password.</param>
    /// <param name="confirm_password">Gamer's Password Confirmation</param>
    /// <param name="name">Gamer's Name</param>
    /// <param name="session">An existing session client.</param>
    /// <param name="success">The callback to execute on success.</param>
    /// <param name="error">The callback to execute on error.</param>
    public static void CreateEmailAccount (string email, string password, string confirm_password, string name, SessionClient session, LoginCallback success, ErrorCallback error)
    {
      CreateEmailAccount (email, password, confirm_password, name, "", session, success, error);
    }

    /// <summary>
    /// Create a new account for the gamer using a GameUp Email / Password combination with a nickname and link an existing gamer token.
    /// </summary>
    /// <param name="email">Gamer's Email.</param>
    /// <param name="password">Gamer's Password.</param>
    /// <param name="confirm_password">Gamer's Password Confirmation</param>
    /// <param name="name">Gamer's Name</param>
    /// <param name="nickname">Gamer's Nickname - Optional</param>
    /// <param name="session">An existing session client.</param>
    /// <param name="success">The callback to execute on success.</param>
    /// <param name="error">The callback to execute on error.</param>
    [Obsolete("CreateGameUpAccount is deprecated, use CreateEmailAccount instead.")]
    public static void CreateGameUpAccount (string email, string password, string confirm_password, string name, string nickname, SessionClient session, LoginCallback success, ErrorCallback error)
    {
      CreateEmailAccount (email, password, confirm_password, name, nickname, session, success, error);
    }

    /// <summary>
    /// Create a new account for the gamer using an Email / Password combination with a nickname and link an existing gamer token.
    /// </summary>
    /// <param name="email">Gamer's Email.</param>
    /// <param name="password">Gamer's Password.</param>
    /// <param name="confirm_password">Gamer's Password Confirmation</param>
    /// <param name="name">Gamer's Name</param>
    /// <param name="nickname">Gamer's Nickname - Optional</param>
    /// <param name="session">An existing session client.</param>
    /// <param name="success">The callback to execute on success.</param>
    /// <param name="error">The callback to execute on error.</param>
    public static void CreateEmailAccount (string email, string password, string confirm_password, string name, string nickname, SessionClient session, LoginCallback success, ErrorCallback error)
    {
      UriBuilder b = new UriBuilder (SCHEME, AccountsServer, PORT, "/v0/gamer/account/email/create");
      String token = session == null ? "" : session.Token;
      WWWRequest wwwRequest = new WWWRequest (b.Uri, "POST", ApiKey, token);

      string body = "{\"email\":\"" + email + "\", \"password\":\"" + password + "\", \"confirm_password\":\"" + confirm_password + "\"";
      if (name != null && name.Trim ().Length > 0) {
        body += ", \"name\":\"" + name + "\"";
      }
      if (nickname != null && nickname.Trim ().Length > 0) {
        body += ", \"nickname\":\"" + nickname + "\"";
      }
      body += "}";
      wwwRequest.SetBody (body);

      wwwRequest.OnSuccess = (String jsonResponse) => {
        success (createSessionClient (jsonResponse));
      };
      wwwRequest.OnFailure = (int statusCode, string reason) => {
        error (statusCode, reason);
      };
      wwwRequest.Execute ();
    }

    /// <summary>
    /// Send a password recovery email to the gamer
    /// </summary>
    /// <param name="email">Gamer's Email.</param>
    /// <param name="success">The callback to execute on success.</param>
    /// <param name="error">The callback to execute on error.</param>
    [Obsolete("ResetEmailGameUp is deprecated, use ResetEmailAccount instead.")]
    public static void ResetEmailGameUp (string email, SuccessCallback success, ErrorCallback error)
    {
      ResetEmailAccount (email, success, error);
    }

    /// <summary>
    /// Send a password recovery email to the gamer
    /// </summary>
    /// <param name="email">Gamer's Email.</param>
    /// <param name="success">The callback to execute on success.</param>
    /// <param name="error">The callback to execute on error.</param>
    public static void ResetEmailAccount (string email, SuccessCallback success, ErrorCallback error)
    {
      UriBuilder b = new UriBuilder (SCHEME, AccountsServer, PORT, "/v0/gamer/account/email/reset/send");
      WWWRequest wwwRequest = new WWWRequest (b.Uri, "POST", ApiKey, "");

      wwwRequest.SetBody ("{\"email\": \"" + email + "\"}");

      wwwRequest.OnSuccess = (String jsonResponse) => {
        success ();
      };
      wwwRequest.OnFailure = (int statusCode, string reason) => {
        error (statusCode, reason);
      };
      wwwRequest.Execute ();  
    }

    /// <summary>
    /// Use a unique ID to generate an account for a gamer. This ID should be cached on the
    /// gaming device so it can be re-used to re-login the gamer to the same account. See
    /// the documentation for more information.
    /// </summary>
    /// <param name="id">The unique ID used for the gamer (could be a device ID).</param>
    /// <param name="success">The callback to execute on success.</param>
    /// <param name="error">The callback to execute on error.</param>
    public static void LoginAnonymous (string id, LoginCallback success, ErrorCallback error)
    {
      if (id.Equals ("cd9e459ea708a948d5c2f5a6ca8838cf")) {
        throw new ArgumentException ("This specific ID is invalid. Please refer to this bug report for more information: https://fogbugz.unity3d.com/default.asp?743378_mpthnvaql975mjmi");
      }

      string body = "{\"id\": \"" + id + "\"}";
      SendAccountRequest ("login", "anonymous", body, null, error, (String jsonResponse) => {
        success (createSessionClient (jsonResponse));
      });
    }
    
    /// <summary>
    /// Login a gamer using a GameUp Email / Password combination.
    /// </summary>
    /// <param name="email">Gamer's Email.</param>
    /// <param name="password">Gamer's Password.</param>
    /// <param name="success">The callback to execute on success.</param>
    /// <param name="error">The callback to execute on error.</param>
    [Obsolete("LoginGameUp is deprecated, use LoginEmail instead.")]
    public static void LoginGameUp (string email, string password, LoginCallback success, ErrorCallback error)
    {
      string body = "{\"email\": \"" + email + "\", \"password\":\"" + password + "\"}";
      SendAccountRequest ("login", "email", body, null, error, (String jsonResponse) => {
        success (createSessionClient (jsonResponse));
      });
    }
    
    /// <summary>
    /// Login a gamer using a GameUp Email / Password combination and link an existing gamer token.
    /// </summary>
    /// <param name="email">Gamer's Email.</param>
    /// <param name="password">Gamer's Password.</param>
    /// <param name="session">An existing session client.</param>
    /// <param name="success">The callback to execute on success.</param>
    /// <param name="error">The callback to execute on error.</param>
    [Obsolete("LoginGameUp is deprecated, use LoginEmail instead.")]
    public static void LoginGameUp (string email, string password, SessionClient session, LoginCallback success, ErrorCallback error)
    {
      string body = "{\"email\": \"" + email + "\", \"password\":\"" + password + "\"}";
      SendAccountRequest ("login", "email", body, session, error, (String jsonResponse) => {
        success (createSessionClient (jsonResponse));
      });
    }
    
    /// <summary>
    /// Login a gamer using a Email / Password combination.
    /// </summary>
    /// <param name="email">Gamer's Email.</param>
    /// <param name="password">Gamer's Password.</param>
    /// <param name="success">The callback to execute on success.</param>
    /// <param name="error">The callback to execute on error.</param>
    public static void LoginEmail (string email, string password, LoginCallback success, ErrorCallback error)
    {
      string body = "{\"email\": \"" + email + "\", \"password\":\"" + password + "\"}";
      SendAccountRequest ("login", "email", body, null, error, (String jsonResponse) => {
        success (createSessionClient (jsonResponse));
      });
    }

    /// <summary>
    /// Login a gamer using a Facebook OAuth Token. An OAuth token can be obtained from the
    /// Facebook SDK.
    /// </summary>
    /// <param name="accessToken">The Facebook OAuth token.</param>
    /// <param name="success">The callback to execute on success.</param>
    /// <param name="error">The callback to execute on error.</param>
    public static void LoginOAuthFacebook (string accessToken, LoginCallback success, ErrorCallback error)
    {
      LoginOAuth ("facebook", accessToken, null, success, error);
    }

    /// <summary>
    /// Login a gamer using a Facebook OAuth Token; or link a Facebook account to their
    /// current session.
    /// </summary>
    /// <param name="accessToken">The Facebook OAuth token.</param>
    /// <param name="session">An existing gamer session client.</param>
    /// <param name="success">The callback to execute on success.</param>
    /// <param name="error">The callback to execute on error.</param>
    [Obsolete("LoginOAuthFacebook with Linking is deprecated, use LoginOAuthFacebook instead and explicitly link accounts.")]
    public static void LoginOAuthFacebook (string accessToken, SessionClient session, LoginCallback success, ErrorCallback error)
    {
      LoginOAuth ("facebook", accessToken, session, success, error);
    }

    /// <summary>
    /// Login a gamer using a Google OAuth Token. An OAuth token can be obtained from the
    /// Google SDK.
    /// </summary>
    /// <param name="accessToken">The Google OAuth token.</param>
    /// <param name="success">The callback to execute on success.</param>
    /// <param name="error">The callback to execute on error.</param>
    public static void LoginOAuthGoogle (string accessToken, LoginCallback success, ErrorCallback error)
    {
      LoginOAuth ("google", accessToken, null, success, error);
    }

    /// <summary>
    /// Login a gamer using a Google OAuth token; or link a Google account to their
    /// current session.
    /// </summary>
    /// <param name="accessToken">The Google OAuth token.</param>
    /// <param name="session">An existing gamer session client.</param>
    /// <param name="success">The callback to execute on success.</param>
    /// <param name="error">The callback to execute on error.</param>
    [Obsolete("LoginOAuthGoogle with Linking is deprecated, use LoginOAuthGoogle instead and explicitly link accounts.")]
    public static void LoginOAuthGoogle (string accessToken, SessionClient session, LoginCallback success, ErrorCallback error)
    {
      LoginOAuth ("google", accessToken, session, success, error);
    }

    /// <summary>
    /// Login a gamer using a Tango Access Token.
    /// </summary>
    /// <param name="accessToken">The Tango Access Token.</param>
    /// <param name="success">The callback to execute on success.</param>
    /// <param name="error">The callback to execute on error.</param>
    public static void LoginTango (string accessToken, LoginCallback success, ErrorCallback error)
    {
      LoginOAuth ("tango", accessToken, null, success, error);
    }

    /// <summary>
    /// Login a gamer using a Game Center. 
    /// NOTE: You cannot use UnityEngine.SocialPlatforms.GameCenter as it doesn't expose the relevant data needed.
    /// </summary>
    /// <param name="playerId">Game Center GKPlayer Player ID.</param>
    /// <param name="bundleId">Application Bundle ID.</param>
    /// <param name="timestamp">Timestamp returned in the iOS local GKPlayer identification callback.</param>
    /// <param name="base64salt">Base64-encoded salt value returned in the iOS local GKPlayer identification callback.</param>
    /// <param name="base64signature">Base64-encoded signature value returned in the iOS local GKPlayer identification callback.</param>
    /// <param name="publicUrlKey">The Public Key URL returned in the iOS local GKPlayer identification callback.</param>
    /// <param name="success">The callback to execute on success.</param>
    /// <param name="error">The callback to execute on error.</param>
    public static void LoginGameCenter (string playerId, string bundleId, long timestamp, string base64salt, string base64signature, string publicKeyUrl, LoginCallback success, ErrorCallback error)
    {
      string body = "{\"player_id\":\"" + playerId + "\"," +
        "\"bundle_id\":\"" + bundleId + "\"," +
        "\"salt\":\"" + base64salt + "\"," +
        "\"signature\":\"" + base64signature + "\"," +
        "\"public_key_url\":\"" + publicKeyUrl + "\"," +
        "\"timestamp\":" + timestamp + "}";
      LoginOAuth ("gamecenter", body, null, success, error);
    }

    /// <summary>
    /// Link an Anonymous ID to the given gamer session.
    /// </summary>
    /// <param name="session">Existing session to link to</param>
    /// <param name="id">Anonymous ID to link</param>
    /// <param name="success">The callback to execute on success.</param>
    /// <param name="error">The callback to execute on error.</param>
    public static void linkAnonymous(SessionClient session, string id, SuccessCallback success, ErrorCallback error) {

      if (id.Equals ("cd9e459ea708a948d5c2f5a6ca8838cf")) {
        throw new ArgumentException ("This specific ID is invalid. Please refer to this bug report for more information: https://fogbugz.unity3d.com/default.asp?743378_mpthnvaql975mjmi");
      }

      string body = "{\"id\":\"" + id + "\"}";
      SendAccountRequest ("link", "anonymous", body, session, error, (String jsonResponse) => {
        success ();
      });
    }

    /// <summary>
    /// Link a Facebook Profile to the given gamer session.
    /// </summary>
    /// <param name="session">Existing session to link to</param>
    /// <param name="accessToken">Acccess Token of the Facebook account to link</param>
    /// <param name="success">The callback to execute on success.</param>
    /// <param name="error">The callback to execute on error.</param>
    public static void linkFacebook(SessionClient session, string accessToken, SuccessCallback success, ErrorCallback error) {
      string body = "{\"access_token\":\"" + accessToken + "\"}";
      SendAccountRequest ("link", "facebook", body, session, error, (String jsonResponse) => {
        success ();
      });
    }

    /// <summary>
    /// Link a Google Account to the given gamer session.
    /// </summary>
    /// <param name="session">Existing session to link to</param>
    /// <param name="accessToken">Acccess Token of the Google account to link</param>
    /// <param name="success">The callback to execute on success.</param>
    /// <param name="error">The callback to execute on error.</param>
    public static void linkGoogle(SessionClient session, string accessToken, SuccessCallback success, ErrorCallback error) {
      string body = "{\"access_token\":\"" + accessToken + "\"}";
      SendAccountRequest ("link", "google", body, session, error, (String jsonResponse) => {
        success ();
      });
    }

    /// <summary>
    /// Link a Tango Account to the given gamer session.
    /// </summary>
    /// <param name="session">Existing session to link to</param>
    /// <param name="accessToken">Acccess Token of the Tango account to link</param>
    /// <param name="success">The callback to execute on success.</param>
    /// <param name="error">The callback to execute on error.</param>
    public static void linkTango(SessionClient session, string accessToken, SuccessCallback success, ErrorCallback error) {
      string body = "{\"access_token\":\"" + accessToken + "\"}";
      SendAccountRequest ("link", "tango", body, session, error, (String jsonResponse) => {
        success ();
      });
    }

    /// <summary>
    /// Link a Game Center Account to the given gamer session.
    /// NOTE: You cannot use UnityEngine.SocialPlatforms.GameCenter as it doesn't expose the relevant data needed.
    /// </summary>
    /// <param name="session">Existing session to link to.</param>
    /// <param name="playerId">Game Center GKPlayer Player ID.</param>
    /// <param name="bundleId">Application Bundle ID.</param>
    /// <param name="timestamp">Timestamp returned in the iOS local GKPlayer identification callback.</param>
    /// <param name="base64salt">Base64-encoded salt value returned in the iOS local GKPlayer identification callback.</param>
    /// <param name="base64signature">Base64-encoded signature value returned in the iOS local GKPlayer identification callback.</param>
    /// <param name="publicUrlKey">The Public Key URL returned in the iOS local GKPlayer identification callback.</param>
    /// <param name="success">The callback to execute on success.</param>
    /// <param name="error">The callback to execute on error.</param>
    public static void LinkGameCenter (SessionClient session, string playerId, string bundleId, long timestamp, string base64salt, string base64signature, string publicKeyUrl, SuccessCallback success, ErrorCallback error)
    {
      string body = "{\"player_id\":\"" + playerId + "\"," +
        "\"bundle_id\":\"" + bundleId + "\"," +
        "\"salt\":\"" + base64salt + "\"," +
        "\"signature\":\"" + base64signature + "\"," +
        "\"public_key_url\":\"" + publicKeyUrl + "\"," +
        "\"timestamp\":" + timestamp + "}";
      SendAccountRequest ("link", "gamecenter", body, session, error, (String jsonResponse) => {
        success ();
      });
    }

    /// <summary>
    /// Unlinks an Anonymous ID from the given gamer session.
    /// </summary>
    /// <param name="session">Existing session to unlink from</param>
    /// <param name="accessToken">Anonymous ID to unlink</param>
    /// <param name="success">The callback to execute on success.</param>
    /// <param name="error">The callback to execute on error.</param>
    public static void unlinkAnonymous(SessionClient session, string id, SuccessCallback success, ErrorCallback error) {
      string body = "{\"id\":\"" + id + "\"}";
      SendAccountRequest ("unlink", "anonymous", body, session, error, (String jsonResponse) => {
        success ();
      });
    }

    /// <summary>
    /// Unlinks an email address from the given gamer session.
    /// </summary>
    /// <param name="session">Existing session to unlink from</param>
    /// <param name="accessToken">Email address to unlink</param>
    /// <param name="success">The callback to execute on success.</param>
    /// <param name="error">The callback to execute on error.</param>
    public static void unlinkEmail(SessionClient session, string email, SuccessCallback success, ErrorCallback error) {
      string body = "{\"email\":\"" + email + "\"}";
      SendAccountRequest ("unlink", "email", body, session, error, (String jsonResponse) => {
        success ();
      });
    }

    /// <summary>
    /// Unlinks a Facebook Account from the given gamer session.
    /// </summary>
    /// <param name="session">Existing session to unlink from</param>
    /// <param name="accessToken">Facebook profile ID to unlink</param>
    /// <param name="success">The callback to execute on success.</param>
    /// <param name="error">The callback to execute on error.</param>
    public static void unlinkFacebook(SessionClient session, string facebookId, SuccessCallback success, ErrorCallback error) {
      string body = "{\"id\":\"" + facebookId + "\"}";
      SendAccountRequest ("unlink", "facebook", body, session, error, (String jsonResponse) => {
        success ();
      });
    }

    /// <summary>
    /// Unlinks a Google Account from the given gamer session.
    /// </summary>
    /// <param name="session">Existing session to unlink from</param>
    /// <param name="accessToken">Google account ID to unlink</param>
    /// <param name="success">The callback to execute on success.</param>
    /// <param name="error">The callback to execute on error.</param>
    public static void unlinkGoogle(SessionClient session, string googleId, SuccessCallback success, ErrorCallback error) {
      string body = "{\"id\":\"" + googleId + "\"}";
      SendAccountRequest ("unlink", "google", body, session, error, (String jsonResponse) => {
        success ();
      });
    }

    /// <summary>
    /// Unlinks a Tango Account from the given gamer session.
    /// </summary>
    /// <param name="session">Existing session to unlink from</param>
    /// <param name="accessToken">Tango account ID to unlink</param>
    /// <param name="success">The callback to execute on success.</param>
    /// <param name="error">The callback to execute on error.</param>
    public static void unlinkTango(SessionClient session, string tangoId, SuccessCallback success, ErrorCallback error) {
      string body = "{\"id\":\"" + tangoId + "\"}";
      SendAccountRequest ("unlink", "tango", body, session, error, (String jsonResponse) => {
        success ();
      });
    }

    /// <summary>
    /// Unlinks a Game Center ID from the given gamer session.
    /// </summary>
    /// <param name="session">Existing session to unlink from</param>
    /// <param name="playerId">Game Center Player ID to unlink</param>
    /// <param name="success">The callback to execute on success.</param>
    /// <param name="error">The callback to execute on error.</param>
    public static void UnlinkGameCenter(SessionClient session, String playerId, SuccessCallback success, ErrorCallback error) {
      string body = "{\"player_id\":\"" + playerId + "\"}";
      SendAccountRequest ("unlink", "gamecenter", body, session, error, (String jsonResponse) => {
        success ();
      });
    }

    /// <summary>
    /// Checks to see if the given Anonymous ID is associated with a gamer account.
    /// </summary>
    /// <param name="session">Existing session to check against</param>
    /// <param name="accessToken">Anonymous ID token to check</param>
    /// <param name="success">The callback to execute on success.</param>
    /// <param name="error">The callback to execute on error.</param>
    public static void checkAnonymous(SessionClient session, string id, AccountCheckCallback success, ErrorCallback error) {
      string body = "{\"id\":\"" + id + "\"}";
      SendAccountRequest ("check", "anonymous", body, session, error, (String jsonResponse) => {
        JsonObject result = SimpleJson.DeserializeObject<JsonObject> (jsonResponse);
        object exists;
        object currentGamer;
        result.TryGetValue("exists", out exists);
        result.TryGetValue("current_gamer", out currentGamer);
        success ((Boolean) exists, (Boolean) currentGamer);
      });
    }

    /// <summary>
    /// Checks to see if the given email address is associated with a gamer account.
    /// </summary>
    /// <param name="session">Existing session to check against</param>
    /// <param name="accessToken">Email address to check</param>
    /// <param name="success">The callback to execute on success.</param>
    /// <param name="error">The callback to execute on error.</param>
    public static void checkEmail(SessionClient session, string email, AccountCheckCallback success, ErrorCallback error) {
      string body = "{\"email\":\"" + email + "\"}";
      SendAccountRequest ("check", "email", body, session, error, (String jsonResponse) => {
        JsonObject result = SimpleJson.DeserializeObject<JsonObject> (jsonResponse);
        object exists;
        object currentGamer;
        result.TryGetValue("exists", out exists);
        result.TryGetValue("current_gamer", out currentGamer);
        success ((Boolean) exists, (Boolean) currentGamer);
      });
    }

    /// <summary>
    /// Checks to see if the given Facebook Profile is associated with a gamer account.
    /// </summary>
    /// <param name="session">Existing session to check against</param>
    /// <param name="accessToken">Facebook access token to check</param>
    /// <param name="success">The callback to execute on success.</param>
    /// <param name="error">The callback to execute on error.</param>
    public static void checkFacebook(SessionClient session, string accessToken, AccountCheckCallback success, ErrorCallback error) {
      string body = "{\"access_token\":\"" + accessToken + "\"}";
      SendAccountRequest ("check", "facebook", body, session, error, (String jsonResponse) => {
        JsonObject result = SimpleJson.DeserializeObject<JsonObject> (jsonResponse);
        object exists;
        object currentGamer;
        result.TryGetValue("exists", out exists);
        result.TryGetValue("current_gamer", out currentGamer);
        success ((Boolean) exists, (Boolean) currentGamer);
      });
    }

    /// <summary>
    /// Checks to see if the given Google Account is associated with a gamer account.
    /// </summary>
    /// <param name="session">Existing session to check against</param>
    /// <param name="accessToken">Google access token to check</param>
    /// <param name="success">The callback to execute on success.</param>
    /// <param name="error">The callback to execute on error.</param>
    public static void checkGoogle(SessionClient session, string accessToken, AccountCheckCallback success, ErrorCallback error) {
      string body = "{\"access_token\":\"" + accessToken + "\"}";
      SendAccountRequest ("check", "google", body, session, error, (String jsonResponse) => {
        JsonObject result = SimpleJson.DeserializeObject<JsonObject> (jsonResponse);
        object exists;
        object currentGamer;
        result.TryGetValue("exists", out exists);
        result.TryGetValue("current_gamer", out currentGamer);
        success ((Boolean) exists, (Boolean) currentGamer);
      });
    }

    /// <summary>
    /// Checks to see if the given Tango Account is associated with a gamer account.
    /// </summary>
    /// <param name="session">Existing session to check against</param>
    /// <param name="accessToken">Tango access token to check</param>
    /// <param name="success">The callback to execute on success.</param>
    /// <param name="error">The callback to execute on error.</param>
    public static void checkTango(SessionClient session, string accessToken, AccountCheckCallback success, ErrorCallback error) {
      string body = "{\"access_token\":\"" + accessToken + "\"}";
      SendAccountRequest ("check", "tango", body, session, error, (String jsonResponse) => {
        JsonObject result = SimpleJson.DeserializeObject<JsonObject> (jsonResponse);
        object exists;
        object currentGamer;
        result.TryGetValue("exists", out exists);
        result.TryGetValue("current_gamer", out currentGamer);
        success ((Boolean) exists, (Boolean) currentGamer);
      });
    }

    /// <summary>
    /// Checks to see if the given Game Center ID is associated with a gamer account.
    /// NOTE: You cannot use UnityEngine.SocialPlatforms.GameCenter as it doesn't expose the relevant data needed.
    /// </summary>
    /// <param name="session">Existing session to check against.</param>
    /// <param name="playerId">Game Center GKPlayer Player ID.</param>
    /// <param name="bundleId">Application Bundle ID.</param>
    /// <param name="timestamp">Timestamp returned in the iOS local GKPlayer identification callback.</param>
    /// <param name="base64salt">Base64-encoded salt value returned in the iOS local GKPlayer identification callback.</param>
    /// <param name="base64signature">Base64-encoded signature value returned in the iOS local GKPlayer identification callback.</param>
    /// <param name="publicUrlKey">The Public Key URL returned in the iOS local GKPlayer identification callback.</param>
    /// <param name="success">The callback to execute on success.</param>
    /// <param name="error">The callback to execute on error.</param>
    public static void CheckGameCenter (SessionClient session, string playerId, string bundleId, long timestamp, string base64salt, string base64signature, string publicKeyUrl, AccountCheckCallback success, ErrorCallback error)
    {
      string body = "{\"player_id\":\"" + playerId + "\"," +
        "\"bundle_id\":\"" + bundleId + "\"," +
        "\"salt\":\"" + base64salt + "\"," +
        "\"signature\":\"" + base64signature + "\"," +
        "\"public_key_url\":\"" + publicKeyUrl + "\"," +
        "\"timestamp\":" + timestamp + "}";
      SendAccountRequest ("check", "gamecenter", body, session, error, (String jsonResponse) => {
        JsonObject result = SimpleJson.DeserializeObject<JsonObject> (jsonResponse);
        object exists;
        object currentGamer;
        result.TryGetValue("exists", out exists);
        result.TryGetValue("current_gamer", out currentGamer);
        success ((Boolean) exists, (Boolean) currentGamer);
      });
    }

    /// <summary>
    /// Make a generic request with pre-set ApiKey.
    /// </summary>
    /// <param name="uri">The URI for the request.</param>
    /// <param name="method">The method for the request.</param>
    /// <param name="body">The body for the request, may be null.</param>
    /// <param name="success">The callback to execute on success.</param>
    /// <param name="error">The callback to execute on error.</param>
    public static void MakeRequest <T> (Uri uri, string method, string body, GenericSuccessCallback<T> success, ErrorCallback error)
    {
      RequestRawCallback rawCallback = (string jsonResponse) => {
        success (SimpleJson.DeserializeObject<T> (jsonResponse));
      };
      MakeRequest (uri, method, body, rawCallback, error);
    }

    /// <summary>
    /// Make a generic request with pre-set ApiKey.
    /// </summary>
    /// <param name="uri">The URI for the request.</param>
    /// <param name="method">The method for the request.</param>
    /// <param name="body">The body for the request, may be null.</param>
    /// <param name="success">The callback to execute on success.</param>
    /// <param name="error">The callback to execute on error.</param>
    public static void MakeRequest (Uri uri, string method, string body, RequestRawCallback success, ErrorCallback error)
    {
      WWWRequest wwwRequest = new WWWRequest (uri, method, ApiKey, "");
      wwwRequest.SetBody (body);
      
      wwwRequest.OnSuccess = (String jsonResponse) => {
        success (jsonResponse);
      };
      wwwRequest.OnFailure = (int statusCode, string reason) => {
        error (statusCode, reason);
      };
      wwwRequest.Execute ();
    }

    private static void LoginOAuth (string type, string accessToken, SessionClient session, LoginCallback success, ErrorCallback error)
    {
      string body = "{\"access_token\":\"" + accessToken + "\"}";
      SendAccountRequest ("login", type, body, session, error, (String jsonResponse) => {
        success (createSessionClient (jsonResponse));
      });
    }

    private static void SendAccountRequest(string requestType, string accountType, string body, SessionClient session, ErrorCallback error, WWWRequest.SuccessCallback success) {
      string endpoint = "/v0/gamer/" + requestType + "/" + accountType;
      UriBuilder b = new UriBuilder (SCHEME, AccountsServer, PORT, endpoint);
      String token = session == null ? "" : session.Token;
      WWWRequest wwwRequest = new WWWRequest (b.Uri, "POST", ApiKey, token);

      wwwRequest.SetBody (body);
      if (success != null) {
        wwwRequest.OnSuccess = success;
      }
      wwwRequest.OnFailure = (int statusCode, string reason) => {
        error (statusCode, reason);
      };
      wwwRequest.Execute ();
    }

    private static SessionClient createSessionClient (String jsonResponse)
    {
      JsonObject json = SimpleJson.DeserializeObject<JsonObject> (jsonResponse);
      String token = System.Convert.ToString (json ["token"]);
      SessionClient sessionClient = new SessionClient ();
      sessionClient.ApiKey = ApiKey;
      sessionClient.Token = token;
      return sessionClient;
    }
  }
}
