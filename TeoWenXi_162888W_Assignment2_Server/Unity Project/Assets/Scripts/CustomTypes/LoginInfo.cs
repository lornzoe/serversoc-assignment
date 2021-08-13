using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class LoginInfo
{
    public string Username { get; set; }
    public string Password { get; set; }

    public static LoginInfo Deserialize(byte[] data)
    {
        LoginInfo result = new LoginInfo();
        using (var s = new MemoryStream(data))
        {
            using (var br = new BinaryReader(s))
            {
                result.Username = br.ReadString();
                result.Password = br.ReadString();
            }
        }
        return result;
    }

    public static byte[] Serialize(object customType)
    {
        LoginInfo customObject = customType as LoginInfo;
        if (customObject == null) { return null; }
        using (var s = new MemoryStream())
        {
            using (var bw = new BinaryWriter(s))
            {
                bw.Write(customObject.Username);
                bw.Write(customObject.Password);
                return s.ToArray();
            }
        }
    }
}
