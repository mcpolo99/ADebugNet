using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Management;
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
                            str = string.Format((IFormatProvider)CultureInfo.InvariantCulture, "{0}", new object[1]
                            {
                instance[wmiProperty]
                            });
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
        private static string CpuId()
        {
            string str1 = Identifier("Win32_Processor", "UniqueId");
            if (str1 == "")
            {
                str1 = Identifier("Win32_Processor", "ProcessorId");
                if (str1 == "")
                {
                    string str2 = Identifier("Win32_Processor", "Name");
                    if (str2 == "")
                        str2 = Identifier("Win32_Processor", "Manufacturer");
                    str1 = str2 + Identifier("Win32_Processor", "MaxClockSpeed");
                }
            }
            return str1;
        }
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
        private static string DiskId()
        {
            return
                Identifier("Win32_DiskDrive", "Model")
                + Identifier("Win32_DiskDrive", "Manufacturer")
                + Identifier("Win32_DiskDrive", "Signature")
                + Identifier("Win32_DiskDrive", "TotalHeads");
        }
        private static string BaseId()
        {
            return
                Identifier("Win32_BaseBoard", "Model")
                + Identifier("Win32_BaseBoard", "Manufacturer")
                + Identifier("Win32_BaseBoard", "Name")
                + Identifier("Win32_BaseBoard", "SerialNumber");

        }
        private static string VideoId()
        {
            return 
                Identifier("Win32_VideoController", "DriverVersion") 
                + Identifier("Win32_VideoController", "Name");

        }
        private static string MacId() 
        {
            return
                Identifier("Win32_NetworkAdapterConfiguration", "MACAddress", "IPEnabled"); 
        }
        private static string ComputerSystemModel() => Identifier("Win32_ComputerSystem", "Model");
        private static string ComputerSystemManufacturer() => Identifier("Win32_ComputerSystem", "Manufacturer");

        #endregion

    }
}
