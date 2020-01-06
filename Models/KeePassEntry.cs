using System.ComponentModel.DataAnnotations;

namespace KeePassWeb.Models
{
    public class KeePassEntry
    {
        public string ID { get; set; }

        [Required]
        public string Group { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }

        public string URL { get; set; }

        public string Notes { get; set; }
    }
}
