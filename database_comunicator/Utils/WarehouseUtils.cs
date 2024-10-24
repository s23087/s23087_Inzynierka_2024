﻿namespace database_communicator.Utils
{
    public static class WarehouseUtils
    {
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
