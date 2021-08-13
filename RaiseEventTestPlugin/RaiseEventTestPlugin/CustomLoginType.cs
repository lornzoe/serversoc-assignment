using System.IO;

namespace TestPlugin
{
    class CustomLoginType
    {
        public string username { get; set; }
        public string password { get; set; }

        public static byte[] Serialize(object o)
        {
            CustomLoginType customObject = o as CustomLoginType; // cast it first
            if (customObject == null)
                return (byte[])null;

            using (var s = new MemoryStream())
            {
                using (var bw = new BinaryWriter(s))
                {
                    bw.Write(customObject.username);
                    bw.Write(customObject.password);

                    return s.ToArray();
                }
            }
        }

        public static object Deserialize(byte[] bytes)
        {
            CustomLoginType customObject = new CustomLoginType();
            using (var s = new MemoryStream(bytes))
            {
                using (var br = new BinaryReader(s))
                {
                    customObject.username = br.ReadString();
                    customObject.password = br.ReadString();

                }
            }
            return customObject;
        }
    }
}
