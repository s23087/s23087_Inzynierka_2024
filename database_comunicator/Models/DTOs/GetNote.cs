﻿namespace database_comunicator.Models.DTOs
{
    public class GetNote
    {
        public DateTime NoteDate { get; set; }
        public string Username { get; set; } = null!;
        public string Note { get; set; } = null!;
    }
}