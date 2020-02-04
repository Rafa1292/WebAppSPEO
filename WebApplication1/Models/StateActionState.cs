using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public class StateActionState
    {
        [Key]
        public int StateActionStateId { get; set; }

        public virtual StateAction StateAction { get; set; }

        public int StateActionId { get; set; }

        public virtual ClientState ClientState { get; set; }

        public int ClientStateId { get; set; }
    }
}