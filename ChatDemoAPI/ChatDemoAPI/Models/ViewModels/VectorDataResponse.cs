namespace ChatDemoAPI.Models.ViewModels
{
    public class VectorDataResponse
    {
        public Guid ChatRobotId { get; set; }
        public Guid DocumentDetaiId { get; set;}
        public Guid DocumentId { get; set; }
        public string DocumentContent { get; set; } = string.Empty;
        public double VectorValue { get; set; }

        
    }
}
