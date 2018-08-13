
namespace UcbManagementInformation.Web
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Data;
    using System.Linq;
    using System.ServiceModel.DomainServices.Hosting;
    using System.ServiceModel.DomainServices.Server;
    using UcbManagementInformation.Server.DataAccess;
    using System.ServiceModel.DomainServices.Server.ApplicationServices;
    using System.Web.Security;
    using UcbManagementInformation.Server.IoC.ServiceLocation;
    

    // Implements application logic using the UcbManagementInformationEntities context.
    // TODO: Add your application logic to these methods or in additional methods.
    // TODO: Wire up authentication (Windows/ASP.NET Forms) and uncomment the following to disable anonymous access
    // Also consider adding roles to restrict access as appropriate.
    // [RequiresAuthentication]
    [EnableClientAccess()]
    public class CustomAuthenticationService : DomainService, IAuthentication<UcbManagementInformation.Server.DataAccess.MCUser>
    {
        AdepUcbEntities ObjectContext = new AdepUcbEntities();
        private static UcbManagementInformation.Server.DataAccess.MCUser DefaultUser = new UcbManagementInformation.Server.DataAccess.MCUser()
        {
            Name = string.Empty,
            Forename = string.Empty,
            Surname = string.Empty,
            Email = string.Empty
        };

        public UcbManagementInformation.Server.DataAccess.MCUser Login(string userName, string password, bool isPersistent, string customData)
        {
            if (this.ValidateUser(userName, password))
            {
                FormsAuthentication.SetAuthCookie(userName, isPersistent);
                return this.GetUser(userName);
            }
            return null;
        }

        /*
         * Quick Start:
         *  It is preferrable to use an existing ASP.NET MembershipProvider or write one for your application
         *  as it improves security and makes reuse easier. This sample only uses rudimentary validation.
         */
        private bool ValidateUser(string username, string password)
        {
            
            return /*(username == password) &&*/ 
                //SimpleServiceLocator.Instance.Get<IUcbManagementInformationRepository<UcbManagementInformation.Server.DataAccess.User>>
                //(DataAccessUtilities.ConvertContext(this.ObjectContext)).Find
                DataAccessUtilities.RepositoryLocator<IUcbManagementInformationRepository<UcbManagementInformation.Server.DataAccess.MCUser>>(this.ObjectContext).Find
                (u => u.Name == username && u.Password == password).Any();
            
        }

        public UcbManagementInformation.Server.DataAccess.MCUser Logout()
        {
            FormsAuthentication.SignOut();
            return CustomAuthenticationService.DefaultUser;
        }

        public UcbManagementInformation.Server.DataAccess.MCUser GetUser()
        {
            if ((this.ServiceContext != null) &&
                (this.ServiceContext.User != null) &&
                this.ServiceContext.User.Identity.IsAuthenticated)
            {
                return this.GetUser(this.ServiceContext.User.Identity.Name);
            }
            return CustomAuthenticationService.DefaultUser;
        }

        private UcbManagementInformation.Server.DataAccess.MCUser GetUser(string userName)
        {
            MCUser userToReturn;
            List<string> roles = new List<string>();
            string userNameWithoutDomain = userName.Substring(userName.LastIndexOf('\\')+1);
            //userToReturn = DataAccessUtilities.RepositoryLocator<IUcbManagementInformationRepository<UcbManagementInformation.Server.DataAccess.MCUser>>
            //    (this.ObjectContext).Find(u => u.Name == userNameWithoutDomain).First();


             var StaffMember = DataAccessUtilities.RepositoryLocator<IUcbRepository<Staff>>
                 (this.ObjectContext).Find(u => u.StaffNumber == userNameWithoutDomain).First();

               userToReturn = new MCUser() { Forename = StaffMember.FirstName,Surname=StaffMember.LastName,Name=StaffMember.StaffNumber, Code = StaffMember.Code,RowIdentifier=StaffMember.RowIdentifier};
            //Populate Roles with the names of all the users roles.
               userToReturn.Roles = DataAccessUtilities.RepositoryLocator<IUcbRepository<StaffAttribute>>(this.ObjectContext).Find(x => x.Application.ApplicationName == "Ucb" && x.StaffCode == StaffMember.Code && x.ApplicationAttribute.IsRole && x.LookupValue == "Yes", "ApplicationAttribute").Select(x => x.ApplicationAttribute.AttributeName).ToList();
     

            //Populate Roles with the names of all the users roles.
            //userToReturn.Roles = DataAccessUtilities.RepositoryLocator<IMCUserRepository>().GetAllRoleNamesByUserCode(userToReturn.Code);
            return userToReturn;

        }

        public void UpdateUser(UcbManagementInformation.Server.DataAccess.MCUser user)
        {
            // Ensure the user data that will be modified represents the currently
            // authenticated identity 
            if ((this.ServiceContext.User == null) ||
                (this.ServiceContext.User.Identity == null) ||
                !string.Equals(this.ServiceContext.User.Identity.Name, user.Name, System.StringComparison.Ordinal))
            {
                throw new UnauthorizedAccessException("You are only authorized to modify your own profile.");
            }

            //this.ObjectContext.Users.AttachAsModified(user, this.ChangeSet.GetOriginal(user));

            IUcbManagementInformationRepository<UcbManagementInformation.Server.DataAccess.MCUser> UserRep = DataAccessUtilities.RepositoryLocator<IUcbManagementInformationRepository<UcbManagementInformation.Server.DataAccess.MCUser>>(this.ObjectContext);
            UserRep.Attach(user);
            UserRep.SaveChanges();
           
        }

        // Consider constraining the results of your query method.  If you need additional input you can
        // add parameters to this method or create additional query methods with different names.
        // To support paging you will need to add ordering to the 'Users' query.
        public IQueryable<UcbManagementInformation.Server.DataAccess.MCUser> GetUsers()
        {
            return this.ObjectContext.Staffs.Select<Staff, MCUser>(x => new MCUser() {  Forename = x.FirstName,Surname=x.LastName,Name=x.StaffNumber, Code = x.Code, RowIdentifier = x.RowIdentifier });
            //return this.ObjectContext.MCUsers;
        }
    }
}


