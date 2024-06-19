using System.ComponentModel.DataAnnotations;

namespace GemNote.API.DTOs.FlashcardDtos;

public class UpdateFlashcardDto
{
	public int Id { get; set; }
	[Required]
	public string Front { get; set; }
	[Required]
	public string Back { get; set; }
}