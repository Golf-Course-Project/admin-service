using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore;

using AdminService.Entities.Identity;
using AdminService.Data;
using AdminService.Enums;
using AdminService.ViewModels.Identity;


namespace AdminService.Repos.Identity
{   
    public class CodesRepo: ICodesRepo, IDisposable
    {
        private IdentityDataContext _dbContext;
        private IdentityDataContextForSp _dbContextForSp;

        public CodesRepo(IdentityDataContext context)
        {
            _dbContext = context;
        }

        public CodesRepo(IdentityDataContext context, IdentityDataContextForSp contextForSp)
        {            
            _dbContext = context;
            _dbContextForSp = contextForSp;
        }

        public IEnumerable<CodesListWithUser> List()
        {
            IEnumerable<CodesListWithUser> results;
            try
            {
                results = _dbContextForSp.CodesListWithUser.FromSqlRaw<CodesListWithUser>("EXEC [dbo].[spListCodesWithUsers]").ToList<CodesListWithUser>();
            }
            catch (Exception)
            {
                results = null;
            }

            return results;
        }

        public Code Fetch(string id)
        {
            Code item;

            try
            {
                item = _dbContext.Codes.Where(x => x.Id == id).First<Code>();
            }
            catch (InvalidOperationException)
            {
                item = null;
            }

            return item;
        }

        public void Create(Code code)
        {
            if (code == null)
            {
                throw new ArgumentNullException("Code", "object cannot be null");
            }

            _dbContext.Codes.Add(code);
        }

        public void Update(Code code, string fields)
        {
            _dbContext.Codes.Attach(code);

            EntityEntry entry = this._dbContext.Entry(code);

            var split = fields.Split(',');

            for (var i = 0; i < split.Count(); ++i)
            {
                entry.Property(split[i].ToString().Trim()).IsModified = true;
            }
        }

        public void Destroy(Code code)
        {
            if (code != null)
            {
                _dbContext.Codes.Remove(code);
            }
        }

        public void Destroy(string id)
        {
            Code code = this.Fetch(id);

            if (code != null)
            {
                _dbContext.Codes.Remove(code);
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

        ~CodesRepo()
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
            }
        }       
    }

    public interface ICodesRepo
    {
        Code Fetch(string id);
        IEnumerable<CodesListWithUser> List();
        void Create(Code code);
        void Update(Code code, string fields);
        void Destroy(Code code);
        void Destroy(string id);
        public int SaveChanges();
    }
}
