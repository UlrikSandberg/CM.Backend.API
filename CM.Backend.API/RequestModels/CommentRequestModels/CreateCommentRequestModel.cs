using System;
using System.ComponentModel.DataAnnotations;
using CM.Backend.API.EnumOptions;

namespace CM.Backend.API.RequestModels.CommentRequestModels
{
    public class CreateCommentRequestModel
    {
        [Required]
        public Guid ContextId { get; set; }
        [Required]
        public CommentContextTypes.ContextTypes ContextType { get; set; }
        [Required]
        public DateTime Date { get; set; }
        [Required]
        public string Comment { get; set; }
    }
}