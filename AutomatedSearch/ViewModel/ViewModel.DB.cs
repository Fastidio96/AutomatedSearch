using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Serialization;
using AutomatedSearch.Model;

namespace AutomatedSearch.ViewModel
{
    public partial class ViewModel
    {
        private string _dirPath => AppDomain.CurrentDomain.BaseDirectory;
        private string _filePath => Path.Combine(_dirPath, "data.xml");

        private byte[] _key => Encoding.UTF8.GetBytes(Costants.KEY);

        public bool SaveToFile()
        {
            try
            {
#if DEBUG
                SaveToFileClean();
#endif

                using (Aes aes = Aes.Create())
                {
                    if (_key == null || _key.Length != 32)
                    {
                        SendMessage(string.Format("Invalid AES key provided! Must be 32 bytes length! Current length is {0}.", _key.Length));
                        return false;
                    }

                    aes.Key = _key;

                    using (MemoryStream ms = new MemoryStream())
                    using (FileStream fs = File.Open(_filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                    {
                        if (fs.Length == 0)
                        {
                            aes.GenerateIV();
                            fs.Write(aes.IV, 0, aes.IV.Length);
                        }
                        else
                        {
                            byte[] iv = new byte[aes.IV.Length];

                            fs.Read(iv, 0, iv.Length);

                            aes.IV = iv;
                        }

                        if ((aes.IV.Length % 8) != 0) // The iv hasn't generated properly
                        {
                            CleanDB();
                            return false;
                        }

                        fs.Seek(aes.IV.Length, SeekOrigin.Begin); // Skip the iv and prepare for write

                        using (ICryptoTransform cryptoTransform = aes.CreateEncryptor())
                        using (CryptoStream cs = new CryptoStream(fs, cryptoTransform, CryptoStreamMode.Write))
                        {
                            XmlSerializer xml = new XmlSerializer(typeof(AppData));
                            xml.Serialize(cs, AppData);

                            if (!cs.HasFlushedFinalBlock)
                            {
                                cs.FlushFinalBlock();
                            }

                            fs.Flush(); // The FileStream don't call the flush automatically
                            cs.Close();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                SendMessage(ex);
                CleanDB();
                return false;
            }

            return true;
        }

#if DEBUG

        public bool SaveToFileClean()
        {
            try
            {
                using (MemoryStream ms = new MemoryStream())
                using (FileStream fs = File.Open(Path.Combine(_dirPath, "cleandata.xml"), FileMode.OpenOrCreate, FileAccess.Write))
                {
                    XmlSerializer xml = new XmlSerializer(typeof(AppData));
                    xml.Serialize(fs, AppData);

                    fs.Flush();
                }
            }
            catch (Exception ex)
            {
                SendMessage(ex);
                return false;
            }

            return true;
        }

#endif

        public bool LoadFromFile()
        {
            try
            {
                using (Aes aes = Aes.Create())
                {
                    aes.Key = _key;

                    using (FileStream fs = File.Open(_filePath, FileMode.OpenOrCreate, FileAccess.Read))
                    {
                        if (fs.Length == 0)
                        {
                            fs.Flush();
                            return true;
                        }

                        byte[] iv = new byte[aes.IV.Length];

                        fs.Seek(0, SeekOrigin.Begin);
                        fs.Read(iv, 0, aes.IV.Length);
                        fs.Seek(aes.IV.Length, SeekOrigin.Begin);

                        aes.IV = iv;

                        if ((aes.IV.Length % 8) != 0) // The iv hasn't generated properly
                        {
                            CleanDB();
                            return false;
                        }

                        using (MemoryStream output = new MemoryStream())
                        using (ICryptoTransform cryptoTransform = aes.CreateDecryptor())
                        using (CryptoStream cs = new CryptoStream(fs, cryptoTransform, CryptoStreamMode.Read))
                        {
                            cs.CopyTo(output);
                            output.Seek(0, SeekOrigin.Begin);

                            XmlSerializer xml = new XmlSerializer(typeof(AppData));
                            AppData data = xml.Deserialize(output) as AppData;
                            if (data == null)
                            {
                                cs.Close();
                                CleanDB();

                                return false;
                            }

                            if (!cs.HasFlushedFinalBlock)
                            {
                                cs.FlushFinalBlock();
                            }

                            cs.Close();

                            AppData = data;
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                SendMessage(ex);
                CleanDB();
                return false;
            }

            return true;
        }

        private bool CleanDB()
        {
            try
            {
                File.Delete(_filePath);
#if DEBUG
                File.Delete(Path.Combine(_dirPath, "cleandata.xml"));
#endif
            }
            catch
            {
                return false;
            }

            return true;
        }
    }
}
