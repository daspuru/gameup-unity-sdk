CHANGELOG
=========

### v0.21.1

* Fix match dequeue request method.

### v0.21.0

* Allow JSON array responses.
* Log non-json response.
* Add support for Datastore TTL.
* Add matchmaking queue status and dequeue calls.
* Add support for social IDs in Leaderboard.
* Use heroiclabs.com for server addresses.

### v0.20.1

* Fix problem with deserialising Cloud Code response.

### 0.20.0

* Add support for Cloudcode 2.0.

### 0.19.1

* Add convert methods to DatastoreObject.

### 0.19.0

* Add support for the Datastore API.
* Mark Cloud Storage and Shared Storage API as deprecated.

### 0.18.3

* Add support for total leaderboard ranks.

### 0.18.2

* Make LeaderboardAndRank constructor public.

### 0.18.1

* Catch argument exception when setting useragent, accept-encoding on Web player.

### 0.18.0

* Add support for Game Center.

### 0.17.3

* Fix to check gzip header on possibly gzipped content.

### 0.17.2

* Rename EnableGZip to EnableGZipRequest.
* Add separate flag EnableGZipResponse to enable SDK-level decompression of gzipped responses. Use static-ifs to enable based on target platform.
* Fix bug looking up Content-Encoding header.

### 0.17.1

* Fixed bug getting string of pre-gzipped data.
* Update to Unity 5.3.1f1.

### 0.17.0

* Add option to GZip requests.
* Bundle [Unity.IO.Compression](https://github.com/Hitcents/Unity.IO.Compression) with the SDK.

### 0.16.9

* Add generic storage get.

### 0.16.8

* Fix bug executing a script with no payload.

### 0.16.7

* Make the `Client.executeScript` methods static.

### 0.16.6

* Fix limit and offset defaults with `Client.Leaderboard` requests.

### 0.16.5

* Change Timezone field to String

### 0.16.4

* Use recommended Pascal case with public static variables.

### 0.16.3

* Set API and Accounts URLs to be public.

### 0.16.2

* Fix deserialisation bug with `Name` field in `Leaderboard` objects.

### 0.16.1

* Add `Match.WhoamiGamerId` to multiplayer `Match` objects.

### 0.16.0

* Support for direct matchmaking with gamer IDs.

### 0.15.4

* Fix bug where nickname and gamer IDs were constructed backwards in object.

### 0.15.3

* Don't set User-Agent header if build is for the Unity Webplayer.

### 0.15.2

* Fix bug where account checks would never match current gamer.

### 0.15.1

* Add ActiveGamers object with util methods.

### 0.15.0

* Additional gamer profile fields.
* Ability to request match updates from a point in time.

### 0.14.1

* Fix unused session tokens for linking/unlinking

### 0.14.0

* Add support for explicit account linking and unlinking.
* Add support for checking account existance.

### 0.13.2

* Add methods for raw JSON responses in `MakeRequest`.

### 0.13.1

* Support `IDictionary` input in turn submissions and serialise to JSON internally.

### 0.13.0

* Add support for filter criteria in turn-based multiplayer match setup.

### 0.12.1

* Set player nickname while registering a new account.

### 0.12.0

* Do not use `SimpleJson` to deserialise directly into C# types.
* BREAKING CHANGE: All of the SDK's POCOs were marked with setters to enable `SimpleJson` to deserialise directly into the types. The have now been removed and all POCOs are immutable (as always intended).

### 0.11.3

* Send an empty object to trick `WWW` into a POST request.

### 0.11.2

* Send empty body on DELETE requests so `WWW` makes a POST request.

### 0.11.1

* Use the SerializerStrategy with Message object deserialisation.

### 0.11.0

* Add (unannounced) Mailbox feature.

### 0.10.0

* Add (unannounced) Cloud Code feature for direct script execution.

### 0.9.3

* Expose option to retrieve `scoretags` in Leaderboard entries.

### 0.9.2

* Small enhancements to Leaderboard queries.

### 0.9.1

* Escape query params and handle PATCH methods correctly.

### 0.9.0

* Add Shared Storage API.

### 0.8.0

* BREAKING CHANGE: Due to limitations in SimpleJson, some fields in Achievement and Leaderboards had to be renamed so they could be parsed properly.
* Add callback to get payload data from the Ping operation.

### 0.7.0

* Add method to update a gamer's nickname.

### 0.6.1

* Fix deserialization of multiplayer entities.

### 0.6.0

* Add methods for the [Advanced Leaderboards](https://gameup.io/blog/2015/07/31/advanced-leaderboards/) feature.
* Add method to deserialise into user defined type with Cloud Storage.
* Add method to make custom API requests.

### 0.5.1

* Fix `SessionClient.Deserialize (string)`.
* Add methods to retrieve (raw) string response from Cloud Storage requests.

### 0.5.0

* Add JSON-based login, create and password reset for GameUp accounts.

### 0.4.1

* Regenerate the SDK DLL.

### 0.4.0

* Add In-App Purchase Verification support.
* Fix use of `new` with `WWWRequest` and `SingletonMonoBehaviour`.

### 0.3.0

* Add Push Notification support.
* Fix callback handler for Achievement responses.

### 0.2.0

* Add turn-based multiplayer features.

### 0.1.1

* Fix a bug in OAuth login requests.

### 0.1.0

* Initial release.
