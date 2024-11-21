namespace database_communicator.Utils
{
    /// <summary>
    /// Utility function for warehouse services.
    /// </summary>
    public static class WarehouseUtils
    {
        /// <summary>
        /// Depend on given parameters will return appropriate status value to item.
        /// </summary>
        /// <param name="warQty">Item quantity that is physically in warehouse</param>
        /// <param name="outQty">>Item quantity that is available to buy from your clients.</param>
        /// <param name="deliveryExist">True if delivery exist or false if not</param>
        /// <returns>Name of item status.</returns>
        public static string GetItemStatus(int warQty, int outQty, bool deliveryExist)
        {
            if (warQty <= 0 && outQty > 0 && !deliveryExist) return "On request";
            if (warQty <= 0 && outQty <= 0 && deliveryExist) return "In delivery";
            if (warQty > 0 && outQty <= 0 && deliveryExist) return "In warehouse | In delivery";
            if (warQty > 0) return "In warehouse";
            return "Unavailable";
        }
    }
}
