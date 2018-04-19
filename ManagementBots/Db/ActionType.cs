using System;
using System.Collections.Generic;

namespace ManagementBots.Db
{
    public partial class ActionType
    {
        public ActionType()
        {
            ActionHistory = new HashSet<ActionHistory>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public bool? Enable { get; set; }

        public ICollection<ActionHistory> ActionHistory { get; set; }
    }
}
