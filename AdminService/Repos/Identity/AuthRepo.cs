 using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using AdminService.Entities.Identity;
using AdminService.Enums;
using AdminService.Helpers;
using AdminService.Misc;
using AdminService.ViewModels.Internal;

namespace AdminService.Repos.Identity
{
    public class AuthRepo : IAuthRepo, IDisposable
    {
        private ITokensRepo _tokensRepo;
        private IUsersRepo _usersRepo;
        private IStandardHelper _helper;
             
        public AuthRepo(ITokensRepo tokensRepo, IUsersRepo usersRepo, IStandardHelper helper)
        {            
            _tokensRepo = tokensRepo;
            _usersRepo = usersRepo;
            _helper = helper;            
        }        

        public Token GetToken(string token)
        {
            Token tokenEntity = _tokensRepo.Fetch(token);

            return tokenEntity;
        }       
               
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~AuthRepo()
        {
            // Finalizer calls Dispose(false)
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // free managed resources
                if (this._tokensRepo != null)
                {
                    _tokensRepo = null;
                }

                if (this._helper != null)
                {
                    _helper = null;
                }
            }
        }
    }

    public interface IAuthRepo
    {        
        Token GetToken(string token);       
    }
}
