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

namespace GameUp
{
  /// <summary>
  /// Datastore Permission
  /// </summary>
  public enum DatastorePermission
  {
    /// <summary> Indicates that the data cannot be modified or read by the client. If this is used, first write succeeds but follow up requests will fail.</summary>
    None = 1,
    /// <summary> Data is only available to be read by the owner. </summary>
    ReadOnly = 2,
    /// <summary> Indicates that the owner can write to the data but data cannot be retrieved back. </summary>
    WriteOnly = 3,
    /// <summary> Indicates that the owner can read and write to the data. </summary>
    ReadWrite = 4,
    /// <summary> Indicates that the data can be read by owner and everyone. Data is not modifiable. </summary>
    PublicRead = 5,
    /// <summary> Indicates that the data can be read by anyone and written by the owner. </summary>
    PublicReadOwnerWrite=6,
    /// <summary> Indicates that no permission should be supplied. If there is a previous permission available, that will be used. Otherwise table-level permissions are used. </summary>
    Inherit=0
  }

  public class DatastoreObjectMetadata {

    /// <summary> Key name. </summary>
    public readonly string Key;
    /// <summary> Owner of the data. This can be null. </summary>
    public readonly string Owner;
    /// <summary> Schema Version of this data. </summary>
    public readonly long SchemaVersion;
    /// <summary> When this data was last updated. </summary>
    public readonly long UpdatedAt;
    /// <summary> when this data was first stored. </summary>
    public readonly long CreatedAt;
    /// <summary> Permissions on this key. </summary>
    public readonly DatastorePermission Permission;
    /// <summary> TTL on this object. </summary>
    public readonly long TTL;

    internal DatastoreObjectMetadata (IDictionary<string, object> dict)
    {
      foreach (string key in dict.Keys) {
        object value;
        dict.TryGetValue (key, out value);
        if (value == null) {
          continue;
        }
        string valueString = value.ToString ();

        switch (key) {
        case "key":
          Key = valueString;
          break;
        case "owner":
          Owner = valueString;
          break;
        case "schema":
          IDictionary<string, object> schema = (IDictionary<string, object>)value;
          schema.TryGetValue ("version", out value);
          SchemaVersion = (long)value;
          break;
        case "permissions":
          Permission = DictionaryToPermission ((IDictionary<string, object>) value);
          break;
        case "created_at":
          CreatedAt = (long)value;
          break;
        case "updated_at":
          UpdatedAt = (long)value;
          break;
        case "ttl":
          TTL = (long)value;
          break;
        }
      }
    }

    internal static DatastorePermission DictionaryToPermission(IDictionary<string, object> permissions) {
      object value;
      permissions.TryGetValue ("read", out value);
      long read = (long)value;
      permissions.TryGetValue ("write", out value);
      long write = (long)value;

      if (read == 1 && write == 0) {
        return DatastorePermission.ReadOnly;
      } else if (read == 0 && write == 1) {
        return DatastorePermission.WriteOnly;
      } else if (read == 1 && write == 1) {
        return DatastorePermission.ReadWrite;
      } else if (read == 2 && write == 0) {
        return DatastorePermission.PublicRead;
      } else if (read == 2 && write == 1) {
        return DatastorePermission.PublicReadOwnerWrite;
      } else {
        return DatastorePermission.None;
      }
    }

    internal static IDictionary<string, object> ToDictionary (DatastorePermission permission) {
      int read = 0;
      int write = 0;

      switch (permission) {
      case DatastorePermission.Inherit:
        return null;
      case DatastorePermission.None:
        break;
      case DatastorePermission.ReadOnly:
        read = 1;
        break;
      case DatastorePermission.WriteOnly:
        write = 1;
        break;
      case DatastorePermission.ReadWrite:
        read = 1;
        write = 1;
        break;
      case DatastorePermission.PublicRead:
        read = 2;
        break;
      case DatastorePermission.PublicReadOwnerWrite:
        read = 2;
        write = 1;
        break;
      }

      IDictionary<string, object> value = new Dictionary<string, object> ();
      value.Add ("read", read);
      value.Add ("write", write);
      return value;
    }
  }

  /// <summary>
  /// Represents data stored in the Datastore.
  /// </summary>
  public class DatastoreObject
  {
    /// <summary> Metadata stored for this object. </summary>
    public readonly DatastoreObjectMetadata Metadata; 
    /// <summary> Data stored for this key. </summary>
    public readonly IDictionary<string, object> Data; 

    internal DatastoreObject (IDictionary<string, object> dict)
    {
      foreach (string key in dict.Keys) {
        object value;
        dict.TryGetValue (key, out value);
        if (value == null) {
          continue;
        }

        switch (key) {
        case "metadata":
          Metadata = new DatastoreObjectMetadata((IDictionary<string, object>)value);
          break;
        case "data":
          Data = (IDictionary<string, object>)value;
          break;
        }
      }
    }

    /// <summary> Convert Data to the specified user defined data type. </summary>
    public T ConvertData<T> ()
    {
      if (Data is JsonObject) {
        return SimpleJson.DeserializeObject<T> (Data.ToString ());
      }
      return SimpleJson.DeserializeObject<T> (SimpleJson.SerializeObject (Data));
    }

    /// <summary> Convert Data to the string. </summary>
    public string ConvertData ()
    {
      if (Data is JsonObject) {
        return Data.ToString ();
      }
      return SimpleJson.SerializeObject (Data);
    }
  }
}

