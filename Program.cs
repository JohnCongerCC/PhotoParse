using System;
using System.Collections.Generic;
using System.Linq;
using ImageMagick;
using MetadataExtractor;
using MetadataExtractor.Formats.Exif;
using System.IO;
using PhotoParse.Models;

namespace PhotoParse
{
    class Program
    {
        static void Main(string[] args)
        {
            // const string APIKEY = "AIzaSyDjc2n4NvDyILzbUfk-LPilLjxu_xIy_UE";

           
            
            var Repo = new PhotoDB();
            var Start = new DirectoryInfo(@"F:\Backup\My Pictures"); //D:\My Pictures -- G:\Public\Data\My Pictures --F:\Pictures
            var lst = new Dictionary<string,List<string>>();
            WalkDirectoryTree(Start, lst);
            List<string> ListOfJpg;
            List<string> ListOfJPG;
            List<string> ListOfPNG;

            Console.WriteLine("Writing jpg to Database...");
            if(lst.TryGetValue(".jpg", out ListOfJpg))
                foreach (var jpg in ListOfJpg)
                     Repo.AddPhoto(CreatePhoto(jpg));

            Console.WriteLine("Writing PNG to Database...");
             if(lst.TryGetValue(".PNG", out ListOfPNG))
                foreach (var PNG in ListOfPNG)
                     Repo.AddPhoto(CreatePhoto(PNG));
            
            Console.WriteLine("Writing JPG to Database...");
             if(lst.TryGetValue(".JPG", out ListOfJPG))
                foreach (var JPG in ListOfJPG)
                     Repo.AddPhoto(CreatePhoto(JPG));
            
        }

        static Photo CreatePhoto(string path)
        {
            Photo photo = new Photo();
            photo.Location = path;
            try
            {
                photo.DateTaken = File.GetLastWriteTime(path);
                var FileInfo = new FileInfo(path);
                photo.Size = FileInfo.Length.ToString();
                photo.FileName = FileInfo.Name;
                photo.Ext = FileInfo.Extension;
                
                using (var image = new MagickImage(path))
                {
                    photo.Height = image.Height;
                    photo.Width = image.Width;
                }
                var gps = ImageMetadataReader.ReadMetadata(path).OfType<GpsDirectory>().FirstOrDefault();
                if(gps != null)
                {
                    var location = gps.GetGeoLocation();
                    if(location != null)
                    {
                        photo.Latitude = location.Latitude.ToString();
                        photo.Longitude = location.Longitude.ToString();
                    }
                }
                
                var directories = ImageMetadataReader.ReadMetadata(path);
                foreach (var directory in directories)
                    foreach (var tag in directory.Tags)
                    {
                        if(tag.Name.Contains("Make"))
                            photo.CameraMaker = tag.Description.ToString();

                        if(tag.Name.Contains("Model"))
                            photo.CameraModel = tag.Description.ToString();
                        
                        if(tag.Name.Contains("X Resolution"))
                            photo.HResolution = tag.Description.ToString();

                        if(tag.Name.Contains("Y Resolution"))
                            photo.VResolution = tag.Description.ToString();
                        
                        if(tag.Name.Contains("Focal Length"))
                            photo.FocalLength = tag.Description.ToString();

                        if(tag.Name.Contains("Thumbnail Length"))
                            photo.ThumbnailLength = tag.Description.ToString();
                        
                        if(tag.Name.Contains("Metering Mode"))
                            photo.MeteringMode = tag.Description.ToString();
                        
                        if(tag.Name.Contains("Flash"))
                            photo.Flash = tag.Description.ToString();
                    }

            }
            catch (System.Exception)
            {
                Console.WriteLine("Could not get data for:" + path);
            }
            

            // Console.WriteLine("http://maps.googleapis.com/maps/api/geocode/xml?key={2}&latlng={0},{1}", location.Latitude, location.Longitude, APIKEY);
            Console.Write(".");
            return photo;
        }

        static void WalkDirectoryTree(System.IO.DirectoryInfo root, Dictionary<string,List<string>> TheList)
        {
            System.IO.FileInfo[] files = null;
            System.IO.DirectoryInfo[] subDirs = null;

            // First, process all the files directly under this folder
            try
            {
                files = root.GetFiles("*.*");
            }
            // This is thrown if even one of the files requires permissions greater
            // than the application provides.
        
            catch (System.IO.DirectoryNotFoundException e)
            {
                Console.WriteLine(e.Message);
            }

            if (files != null)
            {
                foreach (System.IO.FileInfo fi in files)
                {
                    // In this example, we only access the existing FileInfo object. If we
                    // want to open, delete or modify the file, then
                    // a try-catch block is required here to handle the case
                    // where the file has been deleted since the call to TraverseTree().
                    Console.WriteLine(fi.FullName);
                    
                    AddToDictionary(TheList, fi.Extension, fi.FullName);
                }

                // Now find all the subdirectories under this directory.
                subDirs = root.GetDirectories();

                foreach (System.IO.DirectoryInfo dirInfo in subDirs)
                {
                    // Resursive call for each subdirectory.
                    WalkDirectoryTree(dirInfo, TheList);
                }
            }
        }
        static void AddToDictionary(Dictionary<string,List<string>> Dic, string Ext, string FullName)
        {
            List<string> TheList;
            if (Dic.TryGetValue(Ext, out TheList))
                TheList.Add(FullName);
            else
            {
                TheList = new List<string>();
                TheList.Add(FullName);
                Dic.Add(Ext, TheList);
            }
        }
    }
}
