using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class LoginCheck
{
    public string command;
    public string playerName;

    public static LoginCheck Deserialize(byte[] data)
    {
        LoginCheck result = new LoginCheck();
        using (var s = new MemoryStream(data))
        {
            using (var br = new BinaryReader(s))
            {
                result.command = br.ReadString();
                result.playerName = br.ReadString();
            }
        }
        return result;
    }

    public static byte[] Serialize(object customType)
    {
        LoginCheck customObject = customType as LoginCheck;
        if (customObject == null) { return null; }
        using (var s = new MemoryStream())
        {
            using (var bw = new BinaryWriter(s))
            {
                bw.Write(customObject.command);
                bw.Write(customObject.playerName);
                return s.ToArray();
            }
        }
    }
}
