namespace CRM.DTOs
{
    public class GorevOzetViewModel
    {
        public List<GorevDTO> Bugun { get; set; } = new();
        public List<GorevDTO> Yaklasan { get; set; } = new();
        public List<GorevDTO> Geciken { get; set; } = new();
    }
}
