namespace TaskManagement.Core.DTOs
{

    /// <summary>
    /// HATEOAS link representation for RESTful APIs
    /// </summary>
    public class LinkDto
    {
        public string Href { get; set; }
        public string Rel { get; set; } // Relationship type: self, update, delete, etc.
        public string Method { get; set; } // HTTP method: GET, POST, PUT, DELETE
    }
}