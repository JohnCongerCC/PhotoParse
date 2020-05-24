using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PhotoParse.Models
{
    public enum ItemStatus
    {
        ToDo = 0, Complete = 1
    }
    public class ToDoListItem
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public ItemStatus Status { get; set; }
        public virtual ToDoList ToDoList { get; set; }
    }

    public class ToDoList
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public virtual List<ToDoListItem> Items { get; set; }
        public virtual User User { get; set; }

    }
    public class User
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public virtual List<ToDoList> Lists { get; set; }
    }

    public class Photo
    {
        public int ID { get; set; }
        public string Location { get; set; }
        public DateTime DateTaken { get; set; }
        public string FileName { get; set; }
        public string Ext { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public string HResolution { get; set; }
        public string VResolution { get; set; }
       
        public string CameraMaker { get; set; }
        public string CameraModel { get; set; }
        public string FocalLength { get; set; }
        public string MeteringMode { get; set; }
        public string Flash { get; set; }
        public string ThumbnailLength { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        
        public string Size { get; set; }




    }
}