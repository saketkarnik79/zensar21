using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoApp.Models
{
    internal class User
    {
        // Generate auto implemented property Id
        public int Id { get; set; }

        // Generate auto implemented property UserName
        public string UserName { get; set; }

        // Generate auto implemented property Password
        public string Password { get; set; }
    }
}
