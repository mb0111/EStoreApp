using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace EStore.API.Service.Helpers
{
    public static class Extensions
    {
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> items)
        {
            return (items == null || !items.Any());
        }

        public static bool IsNullOrEmpty<TArray>(this TArray[] array)
        {
            return (array == null || array.Length == 0);
        }

        public static bool IsNullOrWhiteSpace(this string item)
        {
            return string.IsNullOrWhiteSpace(item);
        }

        public static bool IsEmptyGuid(this Guid guid)
        {
            return (guid == Guid.Empty);
        }

        public static string GetEnumIntValuesAsStringWithComma<TEnum>() where TEnum : struct, IComparable, IConvertible, IFormattable
        {
            try
            {
                if (!typeof(TEnum).IsEnum)
                {
                    return "";
                }
                return string.Join(",", Enum.GetValues(typeof(TEnum)).Cast<int>());
            }
            catch (Exception)
            {
                return "";
            }
        }

        public static string GetStringFromEnum<TEnum>(int enumVal) where TEnum : struct, IComparable, IConvertible, IFormattable
        {
            if (!typeof(TEnum).IsEnum)
            {
                return "";
            }
            if (Enum.IsDefined(typeof(TEnum), enumVal))
            {
                return ((TEnum)(object)enumVal).ToString();
            }
            return null;
        }

        public static bool HasAllPropertiesNullOrEmpty<TObject>(this TObject obj)
        {
            var type = obj.GetType();
            var properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
            var hasProperty = properties.Select(x => x.GetValue(obj, null))
                                        .Any(y => y != null && !String.IsNullOrWhiteSpace(y.ToString()));
            return !hasProperty;
        }

        /// <summary>
        /// Return file name extension from path
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static string GetFileExtension(this string fileName)
        {
            try
            {
                return Path.GetExtension(fileName);
            }
            catch (Exception)
            {
                return "";
            }
        }

        #region Check Roles

        public static bool IsSystemOrAdmin(this EnRole role)
        {
            return SystemOrAdminRoles.Contains(role);
        }

        public static bool IsSeller(this EnRole role)
        {
            return role == EnRole.Seller;
        }

        public static bool IsByer(this EnRole role)
        {
            return role == EnRole.Byer;
        }

        #endregion

        #region Check Purchase Status

        public static bool IsValidPurchaseStatusForSeller(this EnPurchaseStatus purchaseStatus)
        {
            return SellerPurchaseStatuses.Contains(purchaseStatus);
        }

        public static bool IsValidPurchaseStatusForByer(this EnPurchaseStatus purchaseStatus)
        {
            return ByerPurchaseStatuses.Contains(purchaseStatus);
        }

        #endregion

        #region Private

        private static IEnumerable<EnRole> SystemOrAdminRoles => new List<EnRole>() { EnRole.System, EnRole.Admin };

        private static IEnumerable<EnPurchaseStatus> SellerPurchaseStatuses => new List<EnPurchaseStatus>() { EnPurchaseStatus.Confirmed, EnPurchaseStatus.Rejected };

        private static IEnumerable<EnPurchaseStatus> ByerPurchaseStatuses => new List<EnPurchaseStatus>() { EnPurchaseStatus.StandBy, EnPurchaseStatus.Cancelled };

        #endregion
    }
}
