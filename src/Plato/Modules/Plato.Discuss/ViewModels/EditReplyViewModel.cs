using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Plato.Discuss.ViewModels
{

    public class EditReplyViewModel
    {

        public int Id { get; set; }

        public int EntityId { get; set; }

        [Required]
        [MinLength(5)]
        [Display(Name = "message")]
        public string Message { get; set; }

        public string EditorHtmlName { get; set; }

    }


}
