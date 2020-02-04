using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public class ClientStateAction
    {
        [Key]
        public int ClientStateActionId { get; set; }

        public virtual StateActionState StateActionState { get; set; }

        public int StateActionStateId { get; set; }

        public virtual Client Client { get; set; }

        public int ClientId { get; set; }

        public DateTime JoinAction { get; set; }

        public string  Message { get; set; }
    }
}