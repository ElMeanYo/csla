//INSTANT C# NOTE: Formerly VB.NET project-level imports:
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;

using System.Text;
using Csla;
using Csla.Security;
using Csla.Core;
using Csla.Serialization;
using Csla.DataPortalClient;
using System.ComponentModel;
using Csla.Validation;
using Csla.Silverlight;
using Csla.Server;

#if !SILVERLIGHT
using System.Data.SqlClient;
#endif

namespace Sample.Business
{
  [Serializable(), ObjectFactory("Sample.Business.CompanyObjectFactory, Sample.Business", "CreateCompany", "GetCompany", "SaveCompany", "")]
  public class CompanyObjectFactoryTarget : BusinessBase<CompanyObjectFactoryTarget>
  {


#if SILVERLIGHT
	  public CompanyObjectFactoryTarget()
	  {
	  }
#else
    private CompanyObjectFactoryTarget()
    {
    }
#endif

    private static PropertyInfo<int> CompanyIdProperty = RegisterProperty<int>(new PropertyInfo<int>("CompanyId", "Company Id", 0));
    public int CompanyId
    {
      get
      {
        return GetProperty(CompanyIdProperty);
      }
    }

    private static PropertyInfo<string> CompanyNameProperty = RegisterProperty<string>(new PropertyInfo<string>("CompanyName", "Company Name", string.Empty));
    public string CompanyName
    {
      get
      {
        return GetProperty(CompanyNameProperty);
      }
      set
      {
        SetProperty(CompanyNameProperty, value);
      }
    }

    private static PropertyInfo<SmartDate> DateAddedProperty = RegisterProperty<SmartDate>(new PropertyInfo<SmartDate>("DateAdded", "Date Added"));
    public string DateAdded
    {
      get
      {
        return GetProperty(DateAddedProperty).Text;
      }
      set
      {
        SmartDate test = new SmartDate();
        if (SmartDate.TryParse(value, ref test))
        {
          SetProperty(DateAddedProperty, test);
        }
      }
    }

    internal object DateAddedValue
    {
      get
      {
        return GetProperty(DateAddedProperty).DBValue;
      }
    }

    protected override void AddAuthorizationRules()
    {
      base.AddAuthorizationRules();
      string[] canWrite = new string[] { "AdminUser", "RegularUser" };
      string[] canRead = new string[] { "AdminUser", "RegularUser", "ReadOnlyUser" };
      string[] admin = new string[] { "AdminUser" };
      AuthorizationRules.AllowCreate(typeof(Company), admin);
      AuthorizationRules.AllowDelete(typeof(Company), admin);
      AuthorizationRules.AllowEdit(typeof(Company), canWrite);
      AuthorizationRules.AllowGet(typeof(Company), canRead);
      AuthorizationRules.AllowWrite(CompanyNameProperty, canWrite);
      AuthorizationRules.AllowWrite(DateAddedProperty, canWrite);
      AuthorizationRules.AllowRead(CompanyNameProperty, canRead);
      AuthorizationRules.AllowRead(CompanyIdProperty, canRead);
      AuthorizationRules.AllowRead(DateAddedProperty, canRead);
    }

    protected override void AddBusinessRules()
    {
      base.AddBusinessRules();
      ValidationRules.AddRule(CommonRules.StringRequired, new RuleArgs(CompanyNameProperty));
      ValidationRules.AddRule(CommonRules.StringMaxLength, new CommonRules.MaxLengthRuleArgs(CompanyNameProperty, 50));
      ValidationRules.AddRule<CompanyObjectFactoryTarget>(IsDateValid, DateAddedProperty);

    }

    private static bool IsDateValid(CompanyObjectFactoryTarget target, RuleArgs e)
    {
      SmartDate dateAdded = target.GetProperty(DateAddedProperty);
      if (!dateAdded.IsEmpty)
      {
        if (dateAdded.Date < new System.DateTime(2000, 1, 1))
        {
          e.Description = "Date must be greater that 1/1/2000!";
          return false;
        }
        else if (dateAdded.Date > DateTime.Today)
        {
          e.Description = "Date cannot be greater than today!";
          return false;
        }
      }
      else
      {
        e.Description = "Date added is required!";
        return false;
      }
      return true;
    }

    internal void SetID(int companyId)
    {
      LoadProperty(CompanyIdProperty, companyId);
    }

    public static void GetCompany(int companyId, EventHandler<DataPortalResult<Company>> handler)
    {
      DataPortal<Company> dp = new DataPortal<Company>();
      dp.FetchCompleted += handler;
      dp.BeginFetch(new SingleCriteria<Company, int>(companyId));
    }

    public static void CreateCompany(EventHandler<DataPortalResult<Company>> handler)
    {
      DataPortal<Company> dp = new DataPortal<Company>();
      dp.CreateCompleted += handler;
      dp.BeginCreate();
    }

    internal static CompanyObjectFactoryTarget LoadCompany(int companyId, string companyName, SmartDate dateAdded)
    {
      CompanyObjectFactoryTarget newCompany = new CompanyObjectFactoryTarget();
      newCompany.LoadProperty(CompanyIdProperty, companyId);
      newCompany.LoadProperty(CompanyNameProperty, companyName);
      newCompany.LoadProperty(DateAddedProperty, dateAdded);
      return newCompany;
    }

    internal static CompanyObjectFactoryTarget NewCompany()
    {
      CompanyObjectFactoryTarget aCompany = new CompanyObjectFactoryTarget();
      aCompany.ValidationRules.CheckRules();
      return aCompany;
    }

    internal static CompanyObjectFactoryTarget CloneCompany(Company source)
    {
      CompanyObjectFactoryTarget returnValue = new CompanyObjectFactoryTarget();
      returnValue.LoadProperty(CompanyIdProperty, source.CompanyId);
      returnValue.LoadProperty(CompanyNameProperty, source.CompanyName);
      returnValue.LoadProperty(DateAddedProperty, new SmartDate(source.DateAdded));
      returnValue.MarkOld();
      if (source.IsNew)
      {
        returnValue.MarkNew();
      }
      if (source.IsDirty)
      {
        returnValue.MarkDirty();
      }
      if (source.IsDeleted)
      {
        returnValue.MarkDeleted();
      }
      if (source.IsBusy)
      {
        returnValue.MarkBusy();
      }
      return returnValue;
    }

  }

} //end of root namespace