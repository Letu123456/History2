using AutoMapper.Execution;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Model
{
    public class Message
    {
        [Key]
        public int Id { get; set; }

       
        public string? SenderId { get; set; }  // Người gửi

        
        public string? ReceiverId { get; set; }  // Người nhận

      
        public string Content { get; set; }

        public DateTime Timestamp { get; set; }


        [ForeignKey(nameof(SenderId))]
        public User? Sender { get; set; }

        [ForeignKey(nameof(ReceiverId))]
        public User? Receiver { get; set; }
    }
}
