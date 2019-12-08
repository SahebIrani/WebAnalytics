using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Demo.Statistics.Infrastructure.Entities
{
    public partial class Statistics : BaseEntity<long>
    {
        [Display(Name = "مرورگر", Order = 4), Column(Order = 4)]
        public string Browser { get; set; }

        [Display(Name = "تعداد", Order = 5), Column(Order = 5)]
        public int Count { get; set; }

        [Display(Name = "آی پی", Order = 6), Column(Order = 6)]
        public string IpAddress { get; set; }

        [Display(Name = "سیتم عامل", Order = 7), Column(Order = 7)]
        public string Os { get; set; }

        [Display(Name = "عامل کاربر", Order = 8), Column(Order = 8)]
        public string UserAgent { get; set; }

        [Display(Name = "شناسه کاربر", Order = 9), Column(Order = 9)]
        public string UserId { get; set; }

        [Display(Name = "متد", Order = 10), Column(Order = 10)]
        public string Method { get; set; }
        [Display(Name = "مسیر", Order = 11), Column(Order = 11)]
        public string Path { get; set; }
        [Display(Name = "ارجاع", Order = 12), Column(Order = 12)]
        public string Referer { get; set; }

        [Display(Name = "هاست", Order = 17), Column(Order = 17)]
        public string Host { get; set; }
        [Display(Name = "اچ تی پی اس", Order = 18), Column(Order = 18)]
        public string IsHTTPS { get; set; }
        [Display(Name = "شما", Order = 19), Column(Order = 19)]
        public string Scheme { get; set; }
        [Display(Name = "پروتکل", Order = 20), Column(Order = 20)]
        public string Protocol { get; set; }
        [Display(Name = "پذیرش زبان", Order = 21), Column(Order = 21)]
        public string AcceptLanguage { get; set; }
    }
}
