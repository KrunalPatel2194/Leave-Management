using System;
using System.Collections.Generic;
using System.Text;
using LeaveSolution.DAL.Repository;
using LeaveSolution.DAL.SMS_Mail;
namespace LeaveSolution.DAL.Interfaces
{
   public interface ISMSUtility
    {

     //   string UserName();

         bool SendSMS(string mobileNumber, string message);
       // string AuthenticateUser(string userName, string password);
    }
}
