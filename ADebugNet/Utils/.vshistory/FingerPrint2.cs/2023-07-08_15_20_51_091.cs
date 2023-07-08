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


        private static string ComputerSystemModel() => Identifier("Win32_ComputerSystem", "Model");
        private static string ComputerSystemManufacturer() => Identifier("Win32_ComputerSystem", "Manufacturer");
        private static string MacId() => Identifier("Win32_NetworkAdapterConfiguration", "MACAddress", "IPEnabled");



    }
}
