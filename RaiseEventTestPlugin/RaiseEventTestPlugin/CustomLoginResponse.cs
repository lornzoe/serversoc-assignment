using System.IO;

namespace TestPlugin
{
    class CustomLoginResponse
    {

        public string response { get; set; }

        public static byte[] Serialize(object o)
        {
            CustomLoginResponse customObject = o as CustomLoginResponse; // cast it first
            if (customObject == null)
                return (byte[])null;

            using (var s = new MemoryStream())
            {
                using (var bw = new BinaryWriter(s))
                {
                    bw.Write(customObject.response);

                    return s.ToArray();
                }
            }
        }

        public static object Deserialize(byte[] bytes)
        {
            CustomLoginResponse customObject = new CustomLoginResponse();
            using (var s = new MemoryStream(bytes))
            {
                using (var br = new BinaryReader(s))
                {
                    customObject.response = br.ReadString();
                }
            }
            return customObject;
        }
    }
}
