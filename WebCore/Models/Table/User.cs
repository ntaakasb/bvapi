using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebCore.Models.Table
{
    [Table("USERS")]
    public class User
    {
        public long ID { get; set; }
        public string USERNAME { get; set; }
        public string PASSWORDHASH { get; set; }
        public string SALT { get; set; }
        public char ISACTIVE { get; set; }

    }
}