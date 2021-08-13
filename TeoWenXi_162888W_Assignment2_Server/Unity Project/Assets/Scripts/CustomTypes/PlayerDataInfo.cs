using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class PlayerDataInfo
{
    public string Username;

    //Position
    public float posX, posY, posZ;
    public float rotX, rotY, rotZ;
    public float cameraPosX, cameraPosY, cameraPosZ;
    public float cameraRotX, cameraRotY, cameraRotZ;
    public float petPosX, petPosY, petPosZ;
    public float petRotX, petRotY, petRotZ;

    //Inventory
    public string item1Name, item2Name;
    public float item1Attack, item2Attack;

    //Friend List
    public string friend1Name, friend2Name, friend3Name;
    public string pending1Name, pending2Name, pending3Name;

    public static PlayerDataInfo Deserialize(byte[] data)
    {
        PlayerDataInfo result = new PlayerDataInfo();
        using (var s = new MemoryStream(data))
        {
            using (var br = new BinaryReader(s))
            {
                result.Username = br.ReadString();

                //Position
                result.posX = br.ReadSingle();
                result.posY = br.ReadSingle();
                result.posZ = br.ReadSingle();
                result.rotX = br.ReadSingle();
                result.rotY = br.ReadSingle();
                result.rotZ = br.ReadSingle();
                result.cameraPosX = br.ReadSingle();
                result.cameraPosY = br.ReadSingle();
                result.cameraPosZ = br.ReadSingle();
                result.cameraRotX = br.ReadSingle();
                result.cameraRotY = br.ReadSingle();
                result.cameraRotZ = br.ReadSingle();
                result.petPosX = br.ReadSingle();
                result.petPosY = br.ReadSingle();
                result.petPosZ = br.ReadSingle();
                result.petRotX = br.ReadSingle();
                result.petRotY = br.ReadSingle();
                result.petRotZ = br.ReadSingle();

                //Inventory
                result.item1Name = br.ReadString();
                result.item1Attack = br.ReadSingle();
                result.item2Name = br.ReadString();
                result.item2Attack = br.ReadSingle();

                //Friend List
                result.friend1Name = br.ReadString();
                result.friend2Name = br.ReadString();
                result.friend3Name = br.ReadString();
                result.pending1Name = br.ReadString();
                result.pending2Name = br.ReadString();
                result.pending3Name = br.ReadString();
            }
        }
        return result;
    }

    public static byte[] Serialize(object customType)
    {
        PlayerDataInfo customObject = customType as PlayerDataInfo;
        if (customObject == null) { return null; }
        using (var s = new MemoryStream())
        {
            using (var bw = new BinaryWriter(s))
            {
                bw.Write(customObject.Username);

                //Position
                bw.Write(customObject.posX);
                bw.Write(customObject.posY);
                bw.Write(customObject.posZ);
                bw.Write(customObject.rotX);
                bw.Write(customObject.rotY);
                bw.Write(customObject.rotZ);
                bw.Write(customObject.cameraPosX);
                bw.Write(customObject.cameraPosY);
                bw.Write(customObject.cameraPosZ);
                bw.Write(customObject.cameraRotX);
                bw.Write(customObject.cameraRotY);
                bw.Write(customObject.cameraRotZ);
                bw.Write(customObject.petPosX);
                bw.Write(customObject.petPosY);
                bw.Write(customObject.petPosZ);
                bw.Write(customObject.petRotX);
                bw.Write(customObject.petRotY);
                bw.Write(customObject.petRotZ);

                //Inventory
                bw.Write(customObject.item1Name);
                bw.Write(customObject.item1Attack);
                bw.Write(customObject.item2Name);
                bw.Write(customObject.item2Attack);

                //Friend List
                bw.Write(customObject.friend1Name);
                bw.Write(customObject.friend2Name);
                bw.Write(customObject.friend3Name);
                bw.Write(customObject.pending1Name);
                bw.Write(customObject.pending2Name);
                bw.Write(customObject.pending3Name);
                return s.ToArray();
            }
        }
    }
}
