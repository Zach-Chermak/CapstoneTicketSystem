using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace Capstone.Models
{
    public class TicketModel
    {
        public int Id { get; set; }
        //Datetime of the initial creation of the ticket
        public DateTime CreationDate { get; set; }
        public string Subject { get; set; }
        public string Info { get; set; }
        public int Priority { get; set; }
        //Id of the user that created the ticket
        public int CreationId { get; set; }

        //New property for comment section 
        public List<String>? Comments { get; set; }


        //Database Properties 
        public int? AdminId { get; set; }
        public DateTime? LastUpdate { get; set; }


        public void AddComment(string comment)
        {
            if (Comments == null)
            {
                Comments = new List<string>();
            }
            Comments.Add(comment);
        }

        public void RemoveComment(string comment)
        {
            if (Comments != null && Comments.Contains(comment))
            {
                Comments.Remove(comment);
            }
        }




    }

    
    
}
