using System;
using System.Reflection;
using System.Resources;
using System.Threading;

namespace ExpertLib
{
    
    
    internal partial class SR
    {
        
        internal static string ExceptionByteArrayValueMustBeGreaterThanZeroBytes
        {
            get
            {
                return Keys_SR.GetString(Keys_SR.ExceptionByteArrayValueMustBeGreaterThanZeroBytes);
            }
        }
        
        internal static string ExceptionElementNameExist
        {
            get
            {
                return Keys_SR.GetString(Keys_SR.ExceptionElementNameExist);
            }
        }
        
        internal static string ExceptionElementNotExist
        {
            get
            {
                return Keys_SR.GetString(Keys_SR.ExceptionElementNotExist);
            }
        }
        
        internal static string ExceptionInvalidStorageName
        {
            get
            {
                return Keys_SR.GetString(Keys_SR.ExceptionInvalidStorageName);
            }
        }
        
        internal static string LineSeperator
        {
            get
            {
                return Keys_SR.GetString(Keys_SR.LineSeperator);
            }
        }
        
        internal static string ExceptionCantBeDriveString(string paramName)
        {
            return Keys_SR.GetString(Keys_SR.ExceptionCantBeDriveString, new object[] {
                        paramName});
        }
        
        internal static string ExceptionDriveIsnReady(string drivename)
        {
            return Keys_SR.GetString(Keys_SR.ExceptionDriveIsnReady, new object[] {
                        drivename});
        }
        
        internal static string ExceptionEmptyString(string name)
        {
            return Keys_SR.GetString(Keys_SR.ExceptionEmptyString, new object[] {
                        name});
        }
        
        internal static string ExceptionEnumerationNotDefined(string variableName, string typeName)
        {
            return Keys_SR.GetString(Keys_SR.ExceptionEnumerationNotDefined, new object[] {
                        variableName,
                        typeName});
        }
        
        internal static string ExceptionErrorEmailString(string EmailString)
        {
            return Keys_SR.GetString(Keys_SR.ExceptionErrorEmailString, new object[] {
                        EmailString});
        }
        
        internal static string ExceptionExpectedType(string typeName)
        {
            return Keys_SR.GetString(Keys_SR.ExceptionExpectedType, new object[] {
                        typeName});
        }
        
        internal static string ExceptionFileNotExist(string fileName)
        {
            return Keys_SR.GetString(Keys_SR.ExceptionFileNotExist, new object[] {
                        fileName});
        }
        
        internal static string ExceptionInvalidatePathString(string paramName)
        {
            return Keys_SR.GetString(Keys_SR.ExceptionInvalidatePathString, new object[] {
                        paramName});
        }
        
        internal static string ExceptionInvalidFilterExpress(string filter)
        {
            return Keys_SR.GetString(Keys_SR.ExceptionInvalidFilterExpress, new object[] {
                        filter});
        }
        
        internal static string ExceptionInvalidNullNameArgument(string name)
        {
            return Keys_SR.GetString(Keys_SR.ExceptionInvalidNullNameArgument, new object[] {
                        name});
        }
        
        internal static string ExceptionNeedAbsolutePath(string paramName)
        {
            return Keys_SR.GetString(Keys_SR.ExceptionNeedAbsolutePath, new object[] {
                        paramName});
        }
        
        internal static string ExceptionNotValidRegexExpress(string regex)
        {
            return Keys_SR.GetString(Keys_SR.ExceptionNotValidRegexExpress, new object[] {
                        regex});
        }
        
        internal static string ExceptionNullOrZeroArray(string paramName)
        {
            return Keys_SR.GetString(Keys_SR.ExceptionNullOrZeroArray, new object[] {
                        paramName});
        }
        
        internal class Keys_SR
        {
            
            internal static System.Resources.ResourceManager resourceManager = new System.Resources.ResourceManager("ExpertLib.SR.srt", typeof(ExpertLib.SR).Assembly);
            
            internal const string ExceptionByteArrayValueMustBeGreaterThanZeroBytes = "ExceptionByteArrayValueMustBeGreaterThanZeroBytes";
            
            internal const string ExceptionCantBeDriveString = "ExceptionCantBeDriveString";
            
            internal const string ExceptionDriveIsnReady = "ExceptionDriveIsnReady";
            
            internal const string ExceptionElementNameExist = "ExceptionElementNameExist";
            
            internal const string ExceptionElementNotExist = "ExceptionElementNotExist";
            
            internal const string ExceptionEmptyString = "ExceptionEmptyString";
            
            internal const string ExceptionEnumerationNotDefined = "ExceptionEnumerationNotDefined";
            
            internal const string ExceptionErrorEmailString = "ExceptionErrorEmailString";
            
            internal const string ExceptionExpectedType = "ExceptionExpectedType";
            
            internal const string ExceptionFileNotExist = "ExceptionFileNotExist";
            
            internal const string ExceptionInvalidatePathString = "ExceptionInvalidatePathString";
            
            internal const string ExceptionInvalidFilterExpress = "ExceptionInvalidFilterExpress";
            
            internal const string ExceptionInvalidNullNameArgument = "ExceptionInvalidNullNameArgument";
            
            internal const string ExceptionInvalidStorageName = "ExceptionInvalidStorageName";
            
            internal const string ExceptionNeedAbsolutePath = "ExceptionNeedAbsolutePath";
            
            internal const string ExceptionNotValidRegexExpress = "ExceptionNotValidRegexExpress";
            
            internal const string ExceptionNullOrZeroArray = "ExceptionNullOrZeroArray";
            
            internal const string LineSeperator = "LineSeperator";
            
            internal static string GetString(string key)
            {
                return resourceManager.GetString(key, System.Globalization.CultureInfo.CurrentUICulture);
            }
            
            internal static string GetString(string key, object[] args)
            {
                string msg = resourceManager.GetString(key, System.Globalization.CultureInfo.CurrentUICulture);
                msg = string.Format(msg, args);
                return msg;
            }
        }
    }
}
