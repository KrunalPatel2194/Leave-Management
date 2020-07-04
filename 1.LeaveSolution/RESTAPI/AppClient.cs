using LeaveSolution.BAL.ServiceModels;
using LeaveSolution.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using static LeaveSolution.Models.Zfpa0025Response;

namespace LeaveSolution.RESTAPI
{
    public class ApiClient
    {
        private readonly HttpClient _httpClient;
        private Uri BaseEndpoint { get; set; }

        public ApiClient(Uri baseEndpoint)
        {
            if (baseEndpoint == null)
            {
                throw new ArgumentNullException("baseEndpoint");
            }
            BaseEndpoint = baseEndpoint;
            _httpClient = new HttpClient();
        }

        private Uri CreateRequestUri(string relativePath)
        {
            var endpoint = new Uri(BaseEndpoint, relativePath);
            var uriBuilder = new UriBuilder(endpoint);
            return uriBuilder.Uri;
        }

        private async Task<T> GetAsync<T>(Uri requestUrl)
        {
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            var response = await _httpClient.GetAsync(requestUrl, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();
            var data = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<T>(data);
        }

        public async Task<Zftm0008Response> GetEmpDetails(string param)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture, param));
            return await GetAsync<Zftm0008Response>(requestUrl);
        }

        public async Task<EMPATTENDANCE> GetEmpAttendenceDetails(string param)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture, param));
            return await GetAsync<EMPATTENDANCE>(requestUrl);
        }

        public async Task<EmployeId> GetEmpId(string param)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,param));
            return await GetAsync<EmployeId>(requestUrl);
        }

        public async Task<QUOTAOVERVIEWDETAILS> GetQuotaOverviewDetails(string param)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture, param));
            return await GetAsync<QUOTAOVERVIEWDETAILS>(requestUrl);
        }

        public async Task<EMPLEAVEHISTORY> GetLeaveHistory(string param)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture, param));
            return await GetAsync<EMPLEAVEHISTORY>(requestUrl);
        }

        public async Task<Zftm0006Response> GetHODDetails(string param)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,param));
            return await GetAsync<Zftm0006Response>(requestUrl);

        }
        public async Task<LEAVEREQUESTDETAILS> SubmitLeaveRequest(string param)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture, param));
            return await GetAsync<LEAVEREQUESTDETAILS>(requestUrl);
        }
    }
}
