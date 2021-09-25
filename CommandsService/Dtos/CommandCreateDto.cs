namespace CommandsService.Dtos
{
    using System.ComponentModel.DataAnnotations;

    public class CommandCreateDto
    {
        [Required]
        public string HowTo { get; set; }

        [Required]
        public string CommandLine { get; set; }
    }
}