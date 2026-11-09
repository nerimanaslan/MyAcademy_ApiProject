using System.ComponentModel.DataAnnotations;

namespace MyAcademy_ApiWebUIProject.ViewModels.Admin
{
    public class CreateTeamViewModel
    {
        [Required(ErrorMessage = "Takım adı boş bırakılamaz.")]
        [StringLength(40, ErrorMessage = "Takım adı en fazla 40 karakter olabilir.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Kısa ad boş bırakılamaz.")]
        [StringLength(3, MinimumLength = 3, ErrorMessage = "Kısa ad 3 harf olmalıdır.")]
        public string Code { get; set; } = string.Empty;

        [Required(ErrorMessage = "Şehir boş bırakılamaz.")]
        public string City { get; set; } = string.Empty;

        [Required(ErrorMessage = "Stadyum boş bırakılamaz.")]
        public string Stadium { get; set; } = string.Empty;

        [Range(1000, 100000, ErrorMessage = "Kapasite 1.000 ile 100.000 arasında olmalıdır.")]
        public int Capacity { get; set; } = 20000;

        [Range(1850, 2026, ErrorMessage = "Kuruluş yılı 1850 ile 2026 arasında olmalıdır.")]
        public int Founded { get; set; } = 1900;

        public string? LogoUrl { get; set; }
        public string ColorA { get; set; } = "#1E5BD8";
        public string ColorB { get; set; } = "#FFFFFF";
        public string Region { get; set; } = "north";
        public string Status { get; set; } = "active";
    }
}
