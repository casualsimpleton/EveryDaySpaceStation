//////////////////////////////////////////////////////////////////////////////////////////
// Every Day Space Station
// http://everydayspacestation.tumblr.com
//////////////////////////////////////////////////////////////////////////////////////////
// FileSystem - Handles all the file i/o as well as knowing various pathing and directories
// Created: December 3 2015
// CasualSimpleton <casualsimpleton@gmail.com>
// Last Modified: December 3 2015
// CasualSimpleton
//////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using EveryDaySpaceStation;
using EveryDaySpaceStation.Utils;
using EveryDaySpaceStation.Json;
using EveryDaySpaceStation.DataTypes;

namespace EveryDaySpaceStation
{
    public static partial class FileSystem
    {
        #region Image Stuff
        static public Texture2D LoadImageFromFileName(string fileName, TextureFormat imgFormat = TextureFormat.ARGB32)
        {
            string fileAndPath = string.Format("{0}{1}{2}{1}{3}{1}{4}", _appDataDirectory, System.IO.Path.DirectorySeparatorChar, _clientDataDirectory, _clientGameDataDirectory, fileName);

            return LoadImageFromFileAndPath(fileAndPath, imgFormat);
        }

        static public Texture2D LoadImageFromFileAndPath(string fileAndPath, TextureFormat imgFormat = TextureFormat.ARGB32)
        {
            if (!File.Exists(fileAndPath))
            {
                Debug.LogWarning(string.Format("Could not find art piece '{0}'.", fileAndPath));
                return null;
            }

            Texture2D newTexture = new Texture2D(4, 4, imgFormat, false, false);

            byte[] byteData = null;
            using (FileStream fs = File.OpenRead(fileAndPath))
            {
                byteData = new byte[fs.Length];
                ReadWholeDataStreamArray(fs, ref byteData);
            }

            newTexture.LoadImage(byteData);

            Debug.Log(string.Format("Loaded image {0} as ({1}w {2}h) and format '{3}'", fileAndPath, newTexture.width, newTexture.height, imgFormat));

            newTexture.filterMode = FilterMode.Point;

            return newTexture;
        }

        /// <summary>
        /// Reads data into a complete array, throwing an EndOfStreamException
        /// if the stream runs out of data first, or if an IOException
        /// naturally occurs.
        /// </summary>
        /// <param name="stream">The stream to read data from</param>
        /// <param name="data">The array to read bytes into. The array
        /// will be completely filled from the stream, so an appropriate
        /// size must be given.</param>
        static public void ReadWholeDataStreamArray(Stream stream, ref byte[] data)
        {
            int offset = 0;
            int remaining = data.Length;
            while (remaining > 0)
            {
                int read = stream.Read(data, offset, remaining);
                if (read <= 0)
                {
                    throw new EndOfStreamException(string.Format("End of stream reached with {0} bytes left to read", remaining));
                }

                remaining -= read;
                offset += read;
            }
        }
        #endregion

        #region Client Config Loading/Processing
        
        #endregion
    }
}