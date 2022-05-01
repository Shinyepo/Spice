using Spice.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Spice.Utility
{
    public static class SD
    {
        public const string DefaultFoodImage = "default_food.png";

        public const string ManagerUser = "Manager";
        public const string KitchenUser = "Kitchen";
        public const string FrontDeskUser = "FrontDesk";
        public const string CustomerEndUser = "Customer";

        public const string StatusSubmited = "Accepted";
        public const string StatusInProcess = "In Preparation";
        public const string StatusReady = "Ready for Pick Up";
        public const string StatusReadyDelivery = "In Delivery";
        public const string StatusCompleted = "Completed";
        public const string StatusCancelled = "Canceled";

        public const string PaymentStatusPending = "Payment in Progress";
        public const string PaymentStatusApproved = "Payment Complete";
        public const string PaymentStatusRejected = "Payment Declined";


        public static string ConvertToRawHtml(string source)
        {
            char[] array = new char[source.Length];
            int arrayIndex = 0;
            bool inside = false;

            for (int i = 0; i < source.Length; i++)
            {
                char let = source[i];
                if (let == '<')
                {
                    inside = true;
                    continue;
                }
                if (let == '>')
                {
                    inside = false;
                    continue;
                }
                if (!inside)
                {
                    array[arrayIndex] = let;
                    arrayIndex++;
                }
            }
            return new string(array, 0, arrayIndex);
        }

        public static double DiscountedPrice(CouponModel CouponFromDb, double OriginalOrderTotal)
        {
            if (CouponFromDb == null) return OriginalOrderTotal;
            else
            {
                if (CouponFromDb.MinimumAmount > OriginalOrderTotal)
                {
                    return OriginalOrderTotal;
                }
                else
                {
                    if (Convert.ToInt32(CouponFromDb.Type) == (int)CouponModel.ECouponType.PLN)
                    {
                        return Math.Round(OriginalOrderTotal - CouponFromDb.Discount, 2);
                    }

                    if (Convert.ToInt32(CouponFromDb.Type) == (int)CouponModel.ECouponType.Procentowy)
                    {
                        return Math.Round(OriginalOrderTotal - (OriginalOrderTotal * CouponFromDb.Discount / 100), 2);
                    }
                }
            }
            return OriginalOrderTotal;
        }

    }
}
