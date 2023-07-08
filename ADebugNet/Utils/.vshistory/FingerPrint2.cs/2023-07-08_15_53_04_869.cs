using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Management;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ADebugNet.Utils
{
    internal class FingerPrint2
    {
        private static string fingerPrint = string.Empty;

        internal static bool IsVirtualMachine
        {
            get
            {
                bool isVirtualMachine = false;
                try
                {
                    string str = ComputerSystemModel();
                    if (!string.IsNullOrWhiteSpace(str))
                        isVirtualMachine = (" " + str.ToLower() + " ").Replace("box", string.Empty).Contains(" virtual ");
                }
                catch (Exception ex)
                {
                }
                return isVirtualMachine;
            }
        }
        public static string Value()
        {
            string str = (string)null;
            if (string.IsNullOrWhiteSpace(fingerPrint))
            {
                bool isVirtualMachine = IsVirtualMachine;
                try
                {
                    str = BaseId();
                    //FingerPrint.fingerPrint = FingerPrint.GetHash("BASE >> " + str);
                }
                catch (Exception ex)
                {
                }
                if (!isVirtualMachine)
                    fingerPrint = GetHash("BASE >> " + str);
            }
            return fingerPrint;
        }


        public static string GetHash(string s)
        {

            if(string.IsNullOrWhiteSpace(s))
            {
                return string.Empty;
            }
            else
            {
                //return GetHexString(new MD5CryptoServiceProvider().ComputeHash(new ASCIIEncoding().GetBytes(s)));

                byte[] byteData = new ASCIIEncoding().GetBytes(s);
                byte[] hashBytes;

                using (MD5CryptoServiceProvider md5Provider = new MD5CryptoServiceProvider())
                {
                    hashBytes = md5Provider.ComputeHash(byteData);
                }

                string hashString = GetHexString(hashBytes);
                return hashString;
            }
             
          

        }
        private static string GetHexString(byte[] bt)
        {
            string hexString = string.Empty;
            for (int index = 0; index < bt.Length; ++index)
            {
                int num1 = (int)bt[index];
                int num2 = num1 & 15;
                int num3 = num1 >> 4 & 15;
                string str = num3 <= 9 ? hexString + num3.ToString() : hexString + ((char)(num3 - 10 + 65)).ToString();
                hexString = num2 <= 9 ? str + num2.ToString() : str + ((char)(num2 - 10 + 65)).ToString();
                if (index + 1 != bt.Length && (index + 1) % 2 == 0)
                    hexString += "-";
            }
            return hexString;
        }


        private static string Identifier(string wmiClass, string wmiProperty)
        {
            string str = "";
            foreach (ManagementBaseObject instance in new ManagementClass(wmiClass).GetInstances())
            {
                if (instance != null)
                {
                    if (str == "")
                    {
                        try
                        {
                            str = string.Format((IFormatProvider)CultureInfo.InvariantCulture, "{0}", new object[1]{instance[wmiProperty]});
                            break;
                        }
                        catch
                        {
                        }
                    }
                }
            }
            return str;
        }
        private static string Identifier(string wmiClass, string wmiProperty, string wmiMustBeTrue)
        {
            string str = "";
            foreach (ManagementObject instance in new ManagementClass(wmiClass).GetInstances())
            {
                if (instance[wmiMustBeTrue].ToString() == "True")
                {
                    if (str == "")
                    {
                        try
                        {
                            str = instance[wmiProperty].ToString();
                            break;
                        }
                        catch
                        {
                        }
                    }
                }
            }
            return str;
        }




        #region Identifiers strigns
        //CPU Identifier
        private static string CpuId()
        {
            //Uses first CPU identifier available in order of preference
            //Don't get all identifiers, as it is very time consuming
            string str1 = Identifier("Win32_Processor", "UniqueId");
            if (str1 == "") //If no UniqueID, use ProcessorID
            {
                str1 = Identifier("Win32_Processor", "ProcessorId");
                if (str1 == "") //If no ProcessorId, use Name
                {
                    string str2 = Identifier("Win32_Processor", "Name");
                    if (str2 == "") // If no Name, use Manufacturer
                    {
                        str2 = Identifier("Win32_Processor", "Manufacturer");
                    }

                    //Add clock speed for extra security
                    str1 = str2 + Identifier("Win32_Processor", "MaxClockSpeed");
                }
            }
            return str1;
        }
        //BIOS Identifier
        private static string BiosId()        
        {
            return
                Identifier("Win32_BIOS", "Manufacturer")
                + Identifier("Win32_BIOS", "SMBIOSBIOSVersion")
                + Identifier("Win32_BIOS", "IdentificationCode")
                + Identifier("Win32_BIOS", "SerialNumber")
                + Identifier("Win32_BIOS", "ReleaseDate")
                + Identifier("Win32_BIOS", "Version");

        }
        //Main physical hard drive ID
        private static string DiskId()        
        {
            return
                Identifier("Win32_DiskDrive", "Model")
                + Identifier("Win32_DiskDrive", "Manufacturer")
                + Identifier("Win32_DiskDrive", "Signature")
                + Identifier("Win32_DiskDrive", "TotalHeads");
        }
        //Motherboard ID
        private static string BaseId()         
        {
            return
                Identifier("Win32_BaseBoard", "Model")
                + Identifier("Win32_BaseBoard", "Manufacturer")
                + Identifier("Win32_BaseBoard", "Name")
                + Identifier("Win32_BaseBoard", "SerialNumber");

        }
        //Primary video controller ID
        private static string VideoId()
        {
            return 
                Identifier("Win32_VideoController", "DriverVersion") 
                + Identifier("Win32_VideoController", "Name");

        }
        //First enabled network card ID
        private static string MacId() 
        {
            return
                Identifier("Win32_NetworkAdapterConfiguration", "MACAddress", "IPEnabled"); 
        }
        //System ID
        private static string ComputerSystemModel()
        {
            return
                Identifier("Win32_ComputerSystem", "Model");
        }
        //Manufacturer ID
        private static string ComputerSystemManufacturer()
        {
            return  Identifier("Win32_ComputerSystem", "Manufacturer");
        }

        #endregion

    }
}
