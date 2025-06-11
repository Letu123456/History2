using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Model
{
    public class User:IdentityUser
    {
        public int Point {  get; set; }
        public Boolean IsPremium { get; set; }
        public DateTime PremiumExpiryDate { get; set; }
        public ICollection<Comment>? Comments { get; set; } = new List<Comment>();
        public ICollection<RepliComment>? RepliComments { get; set; } = new List<RepliComment>();
        public ICollection<Favorite>? SaveButtons { get; set; } = new List<Favorite>();
        public ICollection<Blog>? Blogs { get; set; } = new List<Blog>();

        public ICollection<UserQuizResult>? UserQuizResults { get; } = new List<UserQuizResult>();
        
        public ICollection<Report>? Reports { get; } = new List<Report>();
        public ICollection<Notification>? ReceiverNoti { get; } = new List<Notification>();
        public ICollection<Notification>? SenderNoti { get; } = new List<Notification>();

        public ICollection<RedemptionHistory>? RedemptionHistories { get; set; } = new List<RedemptionHistory>();
        public ICollection<TransactionHistory>? TransactionHistories { get; set; } = new List<TransactionHistory>();
        public ICollection<Message>? Sender { get; set; } = new List<Message>();
        public ICollection<Message>? Receiver { get; set; } = new List<Message>();
        public ICollection<Video>? Video { get; set; } = new List<Video>();
        public ICollection<CommentVideo>? CommentVideos { get; set; } = new List<CommentVideo>();
        public ICollection<PaymentTransaction>? PaymentTransactions { get; set; } = new List<PaymentTransaction>();
        public ICollection<Cart>? Carts { get; set; } = new List<Cart>();
        public ICollection<Order>? Orders { get; set; } = new List<Order>();
        public ICollection<PaymentTransactionProduct>? PaymentTransactionProducts { get; set; } = new List<PaymentTransactionProduct>();

        public int? MuseumId {  get; set; }

        public Museum? Museum { get; set; }

        public string? Image {  get; set; }
        public string? Address {  get; set; }
    }
}