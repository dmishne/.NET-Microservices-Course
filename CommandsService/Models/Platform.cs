namespace CommandsService.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class Platform
    {
        [Key]
        [Required]
        public int Id { get; set; }

        [Required]
        public int ExternalId { get; set; }

        [Required]
        public string Name { get; set; }

        public ICollection<Command> Commands { get; set; } = new List<Command>();
    }
}