

namespace SiteRequest.Models
{
   
    public class TeamRequest
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Alias { get; set; }
        public string SiteType { get; set; }
        public string Language { get; set; }
        public string Owners { get; set; }
        public string Members { get; set; }
        public string RequestedBy { get; set; }
        public string Privacy { get; set; }
        public string Classification { get; set; }
        public string GroupId { get; set; }
        public string IsClone { get; set; }
        public string CloneParts { get; set; }
        public string SiteTemplateType { get; set; }
        public string SiteTemplateTypeId { get; set; }      
        public string ApprovedBy { get; set; }
        public string Status { get; set; }
        public string RequestedDate { get; set; }
        public string ApprovedDate { get; set; }
        public string Comments { get; set; }
        public string SiteDesignTemplate { get; set; }
    }
}