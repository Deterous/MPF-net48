﻿using System;
using System.Reflection;
using MPF.Core.Data;
using Newtonsoft.Json.Linq;
using SabreTools.RedumpLib.Data;

namespace MPF.Core.Utilities
{
    public static class Tools
    {
        #region Byte Arrays

        /// <summary>
        /// Search for a byte array in another array
        /// </summary>
        public static bool Contains(this byte[] stack, byte[] needle, out int position, int start = 0, int end = -1)
        {
            // Initialize the found position to -1
            position = -1;

            // If either array is null or empty, we can't do anything
            if (stack == null || stack.Length == 0 || needle == null || needle.Length == 0)
                return false;

            // If the needle array is larger than the stack array, it can't be contained within
            if (needle.Length > stack.Length)
                return false;

            // If start or end are not set properly, set them to defaults
            if (start < 0)
                start = 0;
            if (end < 0)
                end = stack.Length - needle.Length;

            for (int i = start; i < end; i++)
            {
                if (stack.EqualAt(needle, i))
                {
                    position = i;
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// See if a byte array starts with another
        /// </summary>
        public static bool StartsWith(this byte[] stack, byte[] needle)
        {
            return stack.Contains(needle, out int _, start: 0, end: 1);
        }

        /// <summary>
        /// Get if a stack at a certain index is equal to a needle
        /// </summary>
        private static bool EqualAt(this byte[] stack, byte[] needle, int index)
        {
            // If we're too close to the end of the stack, return false
            if (needle.Length >= stack.Length - index)
                return false;

            for (int i = 0; i < needle.Length; i++)
            {
                if (stack[i + index] != needle[i])
                    return false;
            }

            return true;
        }

        #endregion

        #region Support

        /// <summary>
        /// Verify that, given a system and a media type, they are correct
        /// </summary>
        public static Result GetSupportStatus(RedumpSystem? system, MediaType? type)
        {
            // No system chosen, update status
            if (system == null)
                return Result.Failure("Please select a valid system");

            // If we're on an unsupported type, update the status accordingly
            switch (type)
            {
                // Fully supported types
                case MediaType.BluRay:
                case MediaType.CDROM:
                case MediaType.DVD:
                case MediaType.FloppyDisk:
                case MediaType.HardDisk:
                case MediaType.CompactFlash:
                case MediaType.SDCard:
                case MediaType.FlashDrive:
                case MediaType.HDDVD:
                    return Result.Success($"{type.LongName()} ready to dump");

                // Partially supported types
                case MediaType.GDROM:
                case MediaType.NintendoGameCubeGameDisc:
                case MediaType.NintendoWiiOpticalDisc:
                    return Result.Success($"{type.LongName()} partially supported for dumping");

                // Special case for other supported tools
                case MediaType.UMD:
                    return Result.Failure($"{type.LongName()} supported for submission info parsing");

                // Specifically unknown type
                case MediaType.NONE:
                    return Result.Failure($"Please select a valid media type");

                // Undumpable but recognized types
                default:
                    return Result.Failure($"{type.LongName()} media are not supported for dumping");
            }
        }
        
        /// <summary>
        /// Returns false if a given InternalProgram does not support a given MediaType
        /// </summary>
        public static bool ProgramSupportsMedia(InternalProgram program, MediaType? type)
        {
            // If the media type is not set, return false
            if (type == null || type == MediaType.NONE)
                return false;

            switch (program)
            {
                case InternalProgram.Redumper:
                    switch (type)
                    {
                        // Formats considered at least partially dumpable by Redumper
                        case MediaType.BluRay:
                        case MediaType.CDROM:
                        case MediaType.DVD:
                        case MediaType.GDROM:
                        case MediaType.HDDVD:
                            return true;

                        // All other formats considered unsupported
                        default:
                            return false;
                    }
                case InternalProgram.Aaru:
                case InternalProgram.DiscImageCreator:
                    switch (type)
                    {
                        // Formats considered at least partially supported
                        case MediaType.BluRay:
                        case MediaType.CDROM:
                        case MediaType.DVD:
                        case MediaType.FloppyDisk:
                        case MediaType.HardDisk:
                        case MediaType.CompactFlash:
                        case MediaType.SDCard:
                        case MediaType.FlashDrive:
                        case MediaType.HDDVD:
                        case MediaType.NintendoGameCubeGameDisc:
                        case MediaType.NintendoWiiOpticalDisc:
                            return true;

                        // All other formats considered unsupported
                        default:
                            return false;
                    }
                // All other InternalPrograms are not used for dumping
                default:
                    return false;
            }
        }

        #endregion

        #region Versioning

        /// <summary>
        /// Check for a new MPF version
        /// </summary>
        /// <returns>
        /// Bool representing if the values are different.
        /// String representing the message to display the the user.
        /// String representing the new release URL.
        /// </returns>
        public static (bool different, string message, string url) CheckForNewVersion()
        {
            try
            {
                // Get current assembly version
                var assemblyVersion = Assembly.GetEntryAssembly()?.GetName()?.Version;
                if (assemblyVersion == null)
                    return (false, "Assembly version could not be determined", null);

                string version = $"{assemblyVersion.Major}.{assemblyVersion.Minor}.{assemblyVersion.Build}";

                // Get the latest tag from GitHub
                var (tag, url) = GetRemoteVersionAndUrl();
                bool different = version != tag;

                string message = $"Local version: {version}"
                    + $"{Environment.NewLine}Remote version: {tag}"
                    + (different
                        ? $"{Environment.NewLine}The update URL has been added copied to your clipboard"
                        : $"{Environment.NewLine}You have the newest version!");

                return (different, message, url);
            }
            catch (Exception ex)
            {
                return (false, ex.ToString(), null);
            }
        }

        /// <summary>
        /// Get the current informational version formatted as a string
        /// </summary>
        public static string GetCurrentVersion()
        {
            try
            {
                var assembly = Assembly.GetEntryAssembly();
                if (assembly == null)
                    return null;

                var assemblyVersion = Attribute.GetCustomAttribute(assembly, typeof(AssemblyInformationalVersionAttribute)) as AssemblyInformationalVersionAttribute;
                return assemblyVersion?.InformationalVersion;
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        /// <summary>
        /// Get the latest version of MPF from GitHub and the release URL
        /// </summary>
        private static (string tag, string url) GetRemoteVersionAndUrl()
        {
            using (var wc = new System.Net.WebClient())
            {
                wc.Headers["User-Agent"] = "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:64.0) Gecko/20100101 Firefox/64.0";

                // TODO: Figure out a better way than having this hardcoded...
                string url = "https://api.github.com/repos/Deterous/MPF-Legacy/releases/latest";
                string latestReleaseJsonString = wc.DownloadString(url);
                var latestReleaseJson = JObject.Parse(latestReleaseJsonString);
                string latestTag = latestReleaseJson["tag_name"].ToString();
                string releaseUrl = latestReleaseJson["html_url"].ToString();

                return (latestTag, releaseUrl);
            }
        }

        #endregion
    }
}
