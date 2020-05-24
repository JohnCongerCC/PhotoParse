using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PhotoParse.Models;

namespace PhotoParse
{
    public class PhotoDB 
    {
        private List<User> mUsers;
        private List<Photo> mPhotos;
        public List<User> Users
        {
            get 
            { 
                if (mUsers == null)
                    mUsers = GetUsers();
                return mUsers; 
            }
            set { mUsers = value; }
        }

        private List<User> GetUsers()
        {
            using (var context = new Context())
            {
                Users = context.User.Include(p => p.Lists).ThenInclude(i => i.Items).ToList(); 
                if (Users == null)
                    Users = new List<User>();
                return Users;
            }
        }
        
        public async Task<List<User>> GetUsersAsync()
        {
            var t = new Task<List<User>>(() => { return Users; });
            t.Start();
            return await t;
        }
        public async Task<IEnumerable<Models.ToDoListItem>> GetToDoListItemsAsync(string userName, string toDoListName)
        {
            var t = new Task<IEnumerable<Models.ToDoListItem>>(() => { return GetToDoListItems(userName, toDoListName); });
            t.Start();
            return await t;
        }

        public async Task<IEnumerable<Models.ToDoList>> GetToDoListByUserAsync(string UserName)
        {
            var t = new Task<List<Models.ToDoList>>(() =>
            {
                var User = Users.Where(w => w.Name == UserName).FirstOrDefault();
                return User == null ? new List<Models.ToDoList>() : User.Lists;
            });
            t.Start();
            return await t;
        }

        public IEnumerable<ToDoListItem> GetToDoListItems(string userName, string toDoListName)
        {
            using (var context = new Context())
            {
                var user = Users.Where(w => w.Name == userName).FirstOrDefault();
                if (user == null)
                    return new List<ToDoListItem>();

                var list = context.ToDoList.Include(p => p.Items).Where(w => w.Name == toDoListName).Include(i => i.User).FirstOrDefault();
                if (list == null)
                    return new List<ToDoListItem>();

                return list.Items ?? new List<ToDoListItem>();
            }
        }

        public void AddUser(string name)
        {
            using(var context = new Context())
            {
                context.Database.EnsureCreated();

                var newUser = new User { Name = name };
                context.User.Add(newUser);
                context.SaveChanges();
            }
            mUsers = null; //force reload
        }

        public void AddPhoto(string location)
        {
            using(var context = new Context())
            {
                context.Database.EnsureCreated();

                var newPhoto = new Photo { Location = location };
                context.Photo.Add(newPhoto);
                context.SaveChanges();
            }
            mPhotos = null; //force reload
        }

        public void AddPhoto(Photo photo)
        {
            using(var context = new Context())
            {
                context.Database.EnsureCreated();

                context.Photo.Add(photo);
                context.SaveChanges();
            }
            mPhotos = null; //force reload
        }

        public void AddToDoListToUser(string UserName, string ToDoListName)
        {
            using (var context = new Context())
            {
                var user = Users.Where(w => w.Name == UserName).FirstOrDefault();
            
                if (user == null)
                {
                    AddUser(UserName);
                    user = context.User.Where(w => w.Name == UserName).FirstOrDefault();
                }

                var theList = context.ToDoList
                    .Where(w => w.Name == ToDoListName 
                        && w.User.Name == UserName)
                    .Include(i => i.User)
                    .FirstOrDefault();

                if (theList == null)
                {
                    var newList = new Models.ToDoList { Name = ToDoListName, User = user };
                    context.ToDoList.Attach(newList);
                    context.SaveChanges();
                }
            }
            mUsers = null; //force reload
        }

        public void UnCompleteToDoListItem(string userName, string toDoListName, string itemName)
        {
            using (var context = new Context())
            {
                ToDoListItem item = GetItem(userName, toDoListName, itemName, context);
                item.Status = ItemStatus.ToDo;
                context.SaveChanges();
            }
            mUsers = null; //force reload
        }

        public void CompleteToDoListItem(string userName, string toDoListName, string itemName)
        {
            using (var context = new Context())
            {
                ToDoListItem item = GetItem(userName, toDoListName, itemName, context);
                item.Status = ItemStatus.Complete;
                context.SaveChanges();
            }
            mUsers = null; //force reload
        }

        public void UpdateToDoListItem(string userName, string toDoListName, string oldItemName, string newItemName)
        {
            using (var context = new Context())
            {
                ToDoListItem item = GetItem(userName, toDoListName, oldItemName, context);
                item.Name = newItemName;
                context.SaveChanges();
            }
            mUsers = null; //force reload
        }

        private ToDoListItem GetItem(string userName, string toDoListName, string ItemName, Context context)
        {
            ValidateUserAndList(userName, toDoListName, context, out User user, out Models.ToDoList list);
            var item = list.Items.Where(w => w.Name == ItemName).FirstOrDefault();
            if (item == null)
                throw new DataMisalignedException("To Do List item does not exist");
            return item;
        }

        public void UpdateToDoList(string UserName, string OldListName, string NewListName)
        {
            using (var context = new Context())
            {
                ValidateUserAndList(UserName, OldListName, context, out User user, out Models.ToDoList list);
                list.Name = NewListName;
                context.SaveChanges();
            }
            mUsers = null; //force reload
        }

        public void AddToDoListItem(string UserName, string ToDoListName, string ItemName)
        {
            using (var context = new Context())
            {
                Users = context.User.Include(p => p.Lists).ThenInclude(i => i.Items).ToList(); 
                var user = Users.Where(w => w.Name == UserName).FirstOrDefault();
        
                if (user == null)
                {
                    user = new User { Name = UserName, Lists = new List<Models.ToDoList>() };
                    context.User.Add(user);
                }

                var list = context.ToDoList.Where(w => w.Name == ToDoListName && w.User.Name == UserName).FirstOrDefault();
                if (list == null)
                {
                    list = new Models.ToDoList { Name = ToDoListName, Items = new List<ToDoListItem>() };
                    user.Lists.Add(list);
                    context.ToDoList.Add(list);
                }
                if (list.Items == null)
                    list.Items = new List<ToDoListItem>();

                var newitem = new ToDoListItem { Name = ItemName, Status = ItemStatus.ToDo };
                list.Items.Add(newitem);
                context.ToDoListItem.Add(newitem);
                context.SaveChanges();
            }
            mUsers = null; //force reload
        }

        public void DeleteToDoListForUser(string UserName, string ToDoListName)
        {
            using (var context = new Context())
            {
                ValidateUserAndList(UserName, ToDoListName, context, out User user, out Models.ToDoList list);
                user.Lists.Remove(list);
                context.ToDoList.Remove(list);
                context.SaveChanges();
            }
            mUsers = null; //force reload
        }

        public void DeleteToDoListItem(string userName, string toDoListName, string itemName)
        {
            using (var context = new Context())
            {
                ValidateUserAndList(userName, toDoListName, context, out User user, out Models.ToDoList list);
                var item = list.Items.Where(w => w.Name == itemName).FirstOrDefault();
                if (item == null)
                    throw new DataMisalignedException("To Do List item does not exist");

                list.Items.Remove(item);
                context.ToDoListItem.Remove(item);
                context.SaveChanges();
            }
            mUsers = null; //force reload
        }

        private void ValidateUserAndList(string UserName, string ToDoListName, Context context, out User user, out Models.ToDoList list)
        {
            Users = context.User.Include(p => p.Lists).ThenInclude(i => i.Items).ToList(); 
            user = Users.Where(w => w.Name == UserName).FirstOrDefault();
        
            if (user == null)
                throw new DataMisalignedException("User does not exist");

            list = context.ToDoList.Where(w => w.Name == ToDoListName && w.User.Name == UserName).FirstOrDefault();
            if (list == null)
                throw new DataMisalignedException("To Do List does not exist");
        }
    }
}