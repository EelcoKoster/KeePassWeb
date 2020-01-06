using System.ComponentModel.DataAnnotations;

namespace KeePassWeb.Models
{
    public class KeePassEntry
    {
        public string ID { get; set; }
        public string Group { get; set; }
        public string Title { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string URL { get; set; }
        public string Notes { get; set; }
    }
}
