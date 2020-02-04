using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;


namespace WebApplication1.Models
{
    public class StateAction
    {
        [Key]
        public int StateActionId { get; set; }

        public string Name { get; set; }

        public int? WaitTime { get; set; }

        public ICollection<StateActionState> StateActionStates { get; set; }
    }
}