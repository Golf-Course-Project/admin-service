using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

using AdminService.Helpers;
using AdminService.Entities.Identity;
using AdminService.Enums;
using AdminService.Data;
using AdminService.ViewModels.Identity;

namespace AdminService.Repos.Identity
{   
    public class TokensRepo: ITokensRepo, IDisposable
    {
        private IdentityDataContext _dbContext;
        private IdentityDataContextForSp _dbContextForSp;
        private IStandardHelper _helper;
        
        public TokensRepo(IdentityDataContext context, IStandardHelper helper)
        {            
            _dbContext = context;
            _helper = helper;
        }

        public TokensRepo(IdentityDataContext context, IdentityDataContextForSp contextForSp, IStandardHelper helper)
        {
            _dbContext = context;
            _dbContextForSp = contextForSp;
            _helper = helper;
        }

        public Token Fetch(string id)
        {
            Token item;

            try
            {
                item = _dbContext.Tokens.Where(x => x.Id == id && x.Expiration > _helper.GetDateTime).First<Token>();
            }
            catch (InvalidOperationException)
            {
                item = null;
            }

            return item;            
        }

        public IEnumerable<Tokens> List()
        {
            IEnumerable<Tokens> results;

            try
            {
                results = this._dbContextForSp.Tokens.FromSqlRaw<Tokens>("EXEC [dbo].[spListUserTokens]").ToList<Tokens>();
            }
            catch (Exception)
            {
                results = null;
            }

            return results;
        }

        public void Destroy(string id)
        {
            Token result = this.Fetch(id);

            if (result == null)
            {
                throw new ArgumentNullException("token", "token not found");
            }            

            _dbContext.Tokens.Remove(result);
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

        ~TokensRepo()
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

    public interface ITokensRepo
    {      
        Token Fetch(string token);
        IEnumerable<Tokens> List();
        void Destroy(string id);
        int SaveChanges();
    }
}
