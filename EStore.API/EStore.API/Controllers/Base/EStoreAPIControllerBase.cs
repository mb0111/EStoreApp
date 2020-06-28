using EStore.API.Service.Helpers;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Security.Claims;

namespace EStore.API.Controllers.Base
{
    //
    // Summary:
    //     An extension class for ControllerBase MVC controller without view support.
    [Controller]
    public abstract class EStoreAPIControllerBase : ControllerBase
    {
        #region Private Property

        private Guid _defaultIdUser = Guid.Empty;
        private Guid _idUser;
        private EnRole _role;

        #endregion

        #region Public Property

        public Guid IdUser
        {
            get
            {
                if (User != null && User.Identity.IsAuthenticated && !User.Claims.IsNullOrEmpty())
                {
                    string id = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                    if (!id.IsNullOrWhiteSpace())
                    {
                        if (!Guid.TryParse(id, out _idUser))
                            _idUser = _defaultIdUser;
                    }
                    else
                        _idUser = _defaultIdUser;
                }
                else
                    _idUser = _defaultIdUser;
                return _idUser;
            }
        }

        public EnRole Role
        {
            get
            {
                if (User != null && User.Identity.IsAuthenticated && !User.Claims.IsNullOrEmpty())
                {
                    string role = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
                    if (!role.IsNullOrWhiteSpace())
                    {
                        if (Enum.TryParse(typeof(EnRole), role, out object roleObj))
                        {
                            _role = (EnRole)roleObj;
                        }
                    }
                }
                return _role;
            }
        }

        #endregion

    }
}
