using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LeaveSolution.Models
{
    public class ApproverLogin
    {
        public string ApproverMailID { get; set; }
        public string ApproverMobNo { get; set; }
        public string HODMailID { get; set; }
        public string HODMobNo { get; set; }
        public string language { get; set; }
        public string ApproverId { get; set; }
        public string ApproverName { get; set; }
        public string UserName { get; set; }
        public long Mobileno { get; set; }
        public string MailID { get; set; }
        public string Grade { get; set; }
        public string Dept { get; set; }

        [Display(Name = "User ID")]
        [Required]
        public string UserId { get; set; }
        [Required]
        [Display(Name = "Password")]
        public string Password { get; set; }
        public string Languagelist { get; set; }
    }
}
