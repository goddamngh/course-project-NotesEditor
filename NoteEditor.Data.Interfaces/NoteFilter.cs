using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoteEditor.Data.Interfaces
{
    public record NoteFilter
    {
        public static NoteFilter Empty => new();

        public DateTime? StartDate { get; init; }
        public DateTime? EndDate { get; init; }
        public string? SearchText { get; init; }
        public Guid? CategoryId { get; init; }
        public Guid? UserId { get; init; }
    }
}
