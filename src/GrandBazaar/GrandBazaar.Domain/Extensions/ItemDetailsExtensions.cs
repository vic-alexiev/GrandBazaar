using GrandBazaar.Domain.Models;
using Nethereum.Hex.HexConvertors.Extensions;
using System.Numerics;

namespace GrandBazaar.Domain.Extensions
{
    public static class ItemDetailsExtensions
    {
        public static bool Valid(this ItemDetails itemDetails)
        {
            return itemDetails.Seller.HexToBigInteger(true) != BigInteger.Zero;
        }
    }
}
