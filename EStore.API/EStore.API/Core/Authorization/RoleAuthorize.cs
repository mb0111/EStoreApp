using EStore.API.Service.Helpers;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Linq;
using System.Text;

namespace EStore.API.Core.Authorization
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = true, AllowMultiple = true)]
    public class RoleAuthorize : AuthorizeAttribute
    {
        private static readonly string[] RolesWhoAlwaysHaveAccess =
            new[] { Enum.GetName(typeof(EnRole), EnRole.System), Enum.GetName(typeof(EnRole), EnRole.Admin) };

        public RoleAuthorize(params object[] roles)
        {
            if (roles.Any(r => r.GetType().BaseType != typeof(Enum)))
            {
                throw new EStoreAPIException("Roles parameter may only contain enums of type EnRole");
            }

            var temp = roles.Select(r => AddSpacesToSentence(Enum.GetName(r.GetType(), r))).ToList();
            temp.AddRange(RolesWhoAlwaysHaveAccess);
            Roles = string.Join(",", temp);
        }

        private string AddSpacesToSentence(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return string.Empty;
            var newText = new StringBuilder(text.Length * 2);
            newText.Append(text[0]);
            for (int i = 1; i < text.Length; i++)
            {
                if (char.IsUpper(text[i]) && text[i - 1] != ' ')
                {
                    newText.Append(' ');
                }
                newText.Append(text[i]);
            }
            return newText.ToString();
        }
    }
}
