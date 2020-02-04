using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public class ClientState
    {
        [Key]
        public int ClientStateId { get; set; }

        public string  Name { get; set; }

        public ICollection<StateActionState> StateActionState { get; set; }
    }
}