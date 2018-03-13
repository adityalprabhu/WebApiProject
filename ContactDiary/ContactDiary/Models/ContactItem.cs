using System;

namespace ContactDiary.Models
{
    //Model for ContactItem saved in the DB
    public class ContactItem
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Number { get; set; }
        public string Email { get; set; }
    }
}
