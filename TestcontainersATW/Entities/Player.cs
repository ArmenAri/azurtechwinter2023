using System.ComponentModel.DataAnnotations;

namespace TestcontainersATW.Entities;

public class Player
{
    public Guid Id { get; set; }

    [MaxLength(255)]
    public string Name { get; set; } = string.Empty;

    public int HealthPoints { get; set; }

    public int Strength { get; set; }
}