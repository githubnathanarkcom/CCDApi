using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;

namespace CCDApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CCDController : ControllerBase
    {
        private SqlConnection Connection
        {
            get
            {
                return new SqlConnection("Data Source=3.108.12.178;Initial Catalog=itmb;User ID=itmb1;Password=5z$Aex:x");
                //return new SqlConnection("Data Source=MACH-PC189; Initial Catalog=NAEventuallyDB; User ID=sa; Password=machintel@123");

            }
        }

        [HttpPost]
        public Companies[] GetCompanies(Filters filter)
        {
            DataSet dsCompanies = null;
            dsCompanies = GetCompanyDetails(filter);
            CCDData ccdData = PopulateCompaniesList(dsCompanies);

            return ccdData.Companies;
            //return OK(ccdData.Summary);
        }

        private DataSet GetCompanyDetails(Filters filter)
        {
            var company = ("@company", filter.Company);
            var employee = ("@Employee", filter.Employee);
            var industry = ("@Industry", filter.Industry);
            var revenue = ("@Revenue", filter.Revenue);
            var region = ("@Region", filter.Region);
            var pageNumber = ("@PageNumber", filter.PageNumber);
            var pageSize = ("@PageSize", filter.PageSize);
            return ExecuteSP(@"Deck7incDb_CCDB_Master_GetResults_NewFilters_API", company, employee, industry, revenue, region, pageNumber, pageSize);
        }

        private DataSet ExecuteStmt(string stmt, params (string, string)[] args)
        {
            SqlCommand sqlCmd = new SqlCommand();
            sqlCmd.CommandType = CommandType.Text;
            sqlCmd.CommandTimeout = 0;
            foreach (var arg in args)
            {
                sqlCmd.Parameters.AddWithValue(arg.Item1, arg.Item2 == null ? DBNull.Value : arg.Item2);
            }
            sqlCmd.CommandText = stmt;
            sqlCmd.Connection = Connection;
            SqlDataAdapter da = new SqlDataAdapter(sqlCmd);
            DataSet ds = new DataSet();
            da.Fill(ds);

            return ds;
        }

        private DataSet ExecuteSP(string stmt, params (string, string)[] args)
        {
            SqlCommand sqlCmd = new SqlCommand();
            sqlCmd.CommandType = CommandType.StoredProcedure;
            sqlCmd.CommandTimeout = 0;
            foreach (var arg in args)
            {
                sqlCmd.Parameters.AddWithValue(arg.Item1, arg.Item2 == null ? DBNull.Value : arg.Item2);
            }
            sqlCmd.CommandText = stmt;
            sqlCmd.Connection = Connection;
            SqlDataAdapter da = new SqlDataAdapter(sqlCmd);
            DataSet ds = new DataSet();
            da.Fill(ds);

            return ds;
        }

        private CCDData PopulateCompaniesList(DataSet dsCompanies)
        {
            CCDData ccdData = new();
            DataTable dt = dsCompanies.Tables[0];
            DataRow drSummary = dt.Rows[0];
            ccdData.Summary!.TotalNumberOfCompanies = drSummary["Companies"].ToString();
            ccdData.Summary!.TotalNumberOfCampaigns = drSummary["Campaigns"].ToString();
            dt = dsCompanies.Tables[1];
            List<Companies> companies = new();
            foreach (DataRow dr in dt.Rows)
            {
                Companies company = new Companies();
                company.TotalNumberOfCompanies = drSummary["Companies"].ToString();
                company.TotalNumberOfCampaigns = drSummary["Campaigns"].ToString();
                company.CompanyName = (dr["CompanyName"] as string);
                company.EmployeeSize = (dr["EmployeeSize"] as string);
                company.Industry = (dr["Industry"] as string);
                company.Revenue = (dr["Revenue"] as string);
                company.Description = (dr["Description"] as string);
                company.NoOfCampaigns = (dr["NoOfCampaigns"].ToString());
                company.Description = (dr["Description"] as string);
                company.Logo = (dr["PubsiteLogo"] as string);
                company.Location = (dr["location"] as string);

                companies.Add(company);
            }
            ccdData.Companies = companies.ToArray();
            return ccdData;
        }

        public class Companies
        {
            public string? TotalNumberOfCompanies { get; set; }
            public string? TotalNumberOfCampaigns { get; set; }
            public string? CompanyName { get; set; }
            public string? Description { get; set; }
            public string? Industry { get; set; }
            public string? EmployeeSize { get; set; }
            public string? Revenue { get; set; }
            public string? NoOfCampaigns { get; set; }
            public string? Logo { get; set; }
            public string? Location { get; set; }
        }

        public class Summary
        {
            public string? TotalNumberOfCompanies { get; set; }
            public string? TotalNumberOfCampaigns { get; set; }
        }

        public class CCDData
        {
            public Summary? Summary = new();
            public Companies[] Companies;
        }

        public class Filters
        {
            public string Company { get; set; }
            public string Employee { get; set; }
            public string Industry { get; set; }
            public string Revenue { get; set; }
            public string Region { get; set; }
            public string PageNumber { get; set; }
            public string PageSize { get; set; }
        }


    }
}
