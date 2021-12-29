
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Linq;
using System.Collections.Generic;

using AdminService.Data;
using AdminService.Entities.Identity;
using AdminService.ViewModels.Identity;


namespace AdminService.Repos.Identity
{
    public class UsersRepo : IUsersRepo, IDisposable
    {
        private IdentityDataContext _dbContext;
        private IdentityDataContextForSp _dbContextForSp;         

        public UsersRepo(IdentityDataContext context)
        {
            _dbContext = context;
        }

        public UsersRepo(IdentityDataContext context, IdentityDataContextForSp contextForSp)
        {
            _dbContext = context;
            _dbContextForSp = contextForSp;
        }

        public User Fetch(string id)
        {
            User item;

            try
            {
                item = _dbContext.Users.Where(x => x.Id == id).First<User>();
            }
            catch (InvalidOperationException)
            {
                item = null;
            }          

            return item;
        }

        public IEnumerable<UserList> List()
        {
            IEnumerable<UserList> results;
            try
            {
                results = _dbContextForSp.UserList.FromSqlRaw<UserList>("EXEC [dbo].[spListUsers]").ToList<UserList>();
            }
            catch (Exception)
            {
                results = null;
            }

            return results;
        }

        public void Create(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("UserProfile", "item object cannot be null");
            }

            _dbContext.Users.Add(user);
        }

        public void Update(User user, string fields)
        {
            _dbContext.Users.Attach(user);

            EntityEntry entry = this._dbContext.Entry(user);

            var split = fields.Split(',');

            for (var i = 0; i < split.Count(); ++i)
            {
                entry.Property(split[i].ToString().Trim()).IsModified = true;
            }
        }

        public void Delete(string id, DateTime dateDeleted)
        {
            string fields = "DateDeleted,IsDeleted";
            User result = this.Fetch(id);

            if (result == null)
            {
                result = null;
            }
            else
            {
                result.DateDeleted = dateDeleted;
                result.IsDeleted = true;

                _dbContext.Users.Attach(result);

                EntityEntry entry = this._dbContext.Entry(result);

                var split = fields.Split(',');

                for (var i = 0; i < split.Count(); ++i)
                {
                    entry.Property(split[i].ToString().Trim()).IsModified = true;
                }
            }
        }

        public void Restore(string id)
        {
            string fields = "DateDeleted,IsDeleted";
            User result = this.Fetch(id);

            if (result == null)
            {
                result = null;
            }
            else
            {
                result.DateDeleted = null;
                result.IsDeleted = false;

                _dbContext.Users.Attach(result);

                EntityEntry entry = this._dbContext.Entry(result);

                var split = fields.Split(',');

                for (var i = 0; i < split.Count(); ++i)
                {
                    entry.Property(split[i].ToString().Trim()).IsModified = true;
                }
            }
        }

        public void Destroy(string id)
        {
            User result = this.Fetch(id);

            if (result != null)
            {
                _dbContext.Users.Remove(result);
            }
        }

        public int SaveChanges()
        {
            return this._dbContext.SaveChanges();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~UsersRepo()
        {
            // Finalizer calls Dispose(false)
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_dbContext != null)
                {
                    _dbContext.Dispose();
                    _dbContext = null;
                }

                if (_dbContextForSp != null)
                {
                    _dbContextForSp.Dispose();
                    _dbContextForSp = null;
                }
            }
        }
    }

    public interface IUsersRepo
    {
        User Fetch(string id);
        IEnumerable<UserList> List();
        void Create(User item);     
        void Update(User item, string fields);
        void Delete(string id, DateTime dateDeleted);
        public int SaveChanges();
        void Restore(string id);
        void Destroy(string id);
    }
}
