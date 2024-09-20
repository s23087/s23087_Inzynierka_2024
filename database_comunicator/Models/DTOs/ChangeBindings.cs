namespace database_comunicator.Models.DTOs
{
    public class ChangeBindings
    {
        public required int UserId { get; set; }
        public IEnumerable<ModifyBinding> Bindings { get; set; } = new List<ModifyBinding>();
    }
}
