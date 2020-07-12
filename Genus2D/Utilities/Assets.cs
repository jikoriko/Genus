using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Genus2D.Graphics;

namespace Genus2D.Utililities
{
    public class Assets
    {

        private static string ASSETS = "Assets/";
        private static string TEXTURES = "Textures/";
        private static string SHADERS = "Shaders/";
        private static string FONTS = "Fonts/";
        //private static string WINDOWS_FONTS = "";

        private static Dictionary<string, Texture> _textures = new Dictionary<string, Texture>();
        private static Dictionary<string, Shader> _shaders = new Dictionary<string, Shader>();
        private static Dictionary<string, TrueTypeFont> _fonts = new Dictionary<string, TrueTypeFont>();

        public static void PreLoadTextures()
        {
            List<string> allDirectories = GetDirectories(ASSETS + TEXTURES);
            
            foreach (string path in allDirectories)
            {
                DirectoryInfo info = new DirectoryInfo(path);
                FileInfo[] files = info.GetFiles();
                foreach (FileInfo file in files)
                {
                    string fullPath = path + "/" + file.Name;
                    fullPath = fullPath.Replace(ASSETS + TEXTURES, "");
                    LoadTexture(fullPath);
                }
            }
        }

        private static List<string> GetDirectories(string path)
        {
            List<string> directories = new List<string>();

            string[] currentDirectories = Directory.GetDirectories(path);
            directories.AddRange(currentDirectories);
            foreach (string directory in currentDirectories)
            {
                directories.AddRange(GetDirectories(directory));
            }

            return directories;
        }

        public static bool LoadTexture(string filename)
        {
            if (_textures.ContainsKey(filename))
                return true;
            else
            {
                try
                {
                    Texture texture = new Texture(ASSETS + TEXTURES + filename);
                    _textures.Add(filename, texture);
                    return true;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
            return false;
        }

        public static Texture GetTexture(string filename)
        {
            if (_textures.ContainsKey(filename))
                return _textures[filename];
            else if (LoadTexture(filename))
                return _textures[filename];
            return null;
        }

        public static bool LoadShader(string vertFilename, string fragFilename)
        {
            string name = vertFilename + "||" + fragFilename;
            if (_shaders.ContainsKey(name))
                return true;
            else
            {
                try
                {
                    Shader shader = new Shader(ASSETS + SHADERS + vertFilename, ASSETS + SHADERS + fragFilename);
                    _shaders.Add(name, shader);
                    return true;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
            return false;
        }

        public static Shader GetShader(string vertFilename, string fragFilename)
        {
            string name = vertFilename + "||" + fragFilename;
            if (_shaders.ContainsKey(name))
                return _shaders[name];
            else if (LoadShader(vertFilename, fragFilename))
                return _shaders[name];
            return null;
        }

        public static bool LoadFont(string filename, int fontSize)
        {
            string name = filename + "/" + fontSize;
            if (_fonts.ContainsKey(name))
                return true;
            else
            {
                try
                {
                    TrueTypeFont font = new TrueTypeFont(ASSETS + FONTS + filename, fontSize);
                    _fonts.Add(name, font);
                    return true;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
            return false;
        }

        public static TrueTypeFont GetFont(string filename, int fontSize)
        {
            string name = filename + "/" + fontSize;
            if (LoadFont(filename, fontSize))
                return _fonts[name];
            return null;
        }

        public static void Destroy()
        {
            for (int i = 0; i < _textures.Count; i++)
            {
                _textures.ElementAt(i).Value.Destroy();
            }
            _textures.Clear();
            for (int i = 0; i < _shaders.Count; i++)
            {
                _shaders.ElementAt(i).Value.Destroy();
            }
            _shaders.Clear();
        }

    }
}
